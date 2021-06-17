using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    public class ElasticsearchExporterState
    {
        private static readonly Regex IndexFormatRegex = new Regex(@"^(.*)(?:\{0\:.+\})(.*)$");

        public static ElasticsearchExporterState Create(ElasticsearchExporterOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return new ElasticsearchExporterState(options);
        }

        private readonly ElasticLowLevelClient _client;
        private readonly ITextFormatter _formatter;
        private readonly ElasticsearchExporterOptions _options;

        public IElasticLowLevelClient Client => _client;

        public string DiscoveredVersion { get; private set; }

        public ITextFormatter Formatter => _formatter;

        public bool IncludeTypeName => false;

        public bool TemplateRegistrationSuccess { get; private set; }

        private ElasticsearchExporterState(ElasticsearchExporterOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.IndexFormat)) throw new ArgumentException("options.IndexFormat");
            if (string.IsNullOrWhiteSpace(options.TemplateName)) throw new ArgumentException("options.TemplateName");

            _options = options;

            var configuration = new ConnectionConfiguration(options.ConnectionPool, options.Connection, options.Serializer)
                .RequestTimeout(options.ConnectionTimeout);

            if (options.ModifyConnectionSettings != null)
                configuration = options.ModifyConnectionSettings(configuration);

            configuration.ThrowExceptions();

            _client = new ElasticLowLevelClient(configuration);
            _formatter = new ElasticsearchJsonFormatter();
        }

        public string GetIndexForActivity(Activity activity, DateTimeOffset offset)
        {
            return string.Format(_options.IndexFormat, offset).ToLowerInvariant();
        }

        /// <summary>
        /// Register the elasticsearch index template if the provided options mandate it.
        /// </summary>
        public void RegisterTemplateIfNeeded()
        {
            if (!_options.AutoRegisterTemplate) return;

            try
            {
                if (!_options.OverwriteTemplate)
                {
                    var templateExistsResponse = _client.Indices.TemplateExistsForAll<VoidResponse>(_options.TemplateName, new IndexTemplateExistsRequestParameters()
                    {
                        RequestConfiguration = new RequestConfiguration() { AllowedStatusCodes = new[] { 200, 404 } }
                    });
                    if (templateExistsResponse.HttpStatusCode == 200)
                    {
                        TemplateRegistrationSuccess = true;

                        return;
                    }
                }

                var result = _client.Indices.PutTemplateForAll<StringResponse>(_options.TemplateName, GetTemplatePostData(),
                    new PutIndexTemplateRequestParameters
                    {
                        IncludeTypeName = IncludeTypeName ? true : (bool?)null
                    });

                if (!result.Success)
                {
                    ((IElasticsearchResponse)result).TryGetServerErrorReason(out var serverError);
                    SelfLog.WriteLine("Unable to create the template. {0}", serverError);

                    if (_options.RegisterTemplateFailure == RegisterTemplateRecovery.FailExporter)
                        throw new Exception($"Unable to create the template named {_options.TemplateName}.", result.OriginalException);

                    TemplateRegistrationSuccess = false;
                }
                else
                    TemplateRegistrationSuccess = true;

            }
            catch (Exception ex)
            {
                TemplateRegistrationSuccess = false;

                SelfLog.WriteLine("Failed to create the template. {0}", ex);

                if (_options.RegisterTemplateFailure == RegisterTemplateRecovery.FailExporter)
                    throw;
            }
        }

        private PostData GetTemplatePostData()
        {
            //PostData no longer exposes an implicit cast from object.  Previously it supported that and would inspect the object Type to
            //determine if it it was a literal string to write directly or if it was an object that it needed to serialize.  Now the onus is 
            //on us to tell it what type we are passing otherwise if the user specified the template as a json string it would be serialized again.
            var template = GetTemplateData();
            if (template is string s)
            {
                return PostData.String(s);
            }
            else
            {
                return PostData.Serializable(template);
            }
        }

        private object GetTemplateData()
        {
            var settings = new Dictionary<string, string>();
            settings.Add("index.refresh_interval", "5s");

            var templateMatchString = IndexFormatRegex.Replace(_options.IndexFormat, @"$1*$2");

            return ElasticsearchTemplateProvider.GetTemplate(
                _options,
                DiscoveredVersion,
                settings,
                templateMatchString,
                _options.AutoRegisterTemplateVersion);

        }

        public void DiscoverClusterVersion()
        {
            try
            {
                var response = _client.Cat.Nodes<StringResponse>(new CatNodesRequestParameters()
                {
                    Headers = new[] { "v" }
                });
                if (!response.Success) return;

                DiscoveredVersion = response.Body.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault();

                if (DiscoveredVersion?.StartsWith("7.") ?? false)
                    _options.TypeName = "_doc";
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Failed to discover the cluster version. {0}", ex);
            }
        }
    }
}
