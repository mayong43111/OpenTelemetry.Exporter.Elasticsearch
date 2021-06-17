using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    public class ElasticsearchExporterOptions
    {
        /// <summary>
        /// The default Elasticsearch type name used for Elasticsearch versions prior to 7.
        /// <para>As of <c>Elasticsearch 7</c> and up <c>_type</c> has been removed.</para>
        /// </summary>
        public static string DefaultTypeName { get; } = "_doc";

        private readonly Uri _defaultNode = new Uri("http://localhost:9200");

        /// <summary>
        /// When set to true the sink will register an index template for the logs in elasticsearch.
        /// This template is optimized to deal with  events
        /// </summary>
        public bool AutoRegisterTemplate { get; set; }

        /// <summary>
        /// When using the <see cref="AutoRegisterTemplate"/> feature, this allows to set the Elasticsearch version. Depending on the
        /// version, a template will be selected. Defaults to pre 5.0.
        /// </summary>
        public AutoRegisterTemplateVersion AutoRegisterTemplateVersion { get; set; }

        /// <summary>
        /// Gets or sets the BatchExportProcessor options. Ignored unless ExportProcessorType is BatchExporter.
        /// </summary>
        public BatchExportProcessorOptions<Activity> BatchExportProcessorOptions { get; set; } = new BatchExportProcessorOptions<Activity>();

        ///<summary>
        /// Allows you to override the connection used to communicate with elasticsearch.
        /// </summary>
        public IConnection Connection { get; set; }

        /// <summary>
        /// The connection pool describing the cluster to write event to
        /// </summary>
        public IConnectionPool ConnectionPool { get; private set; }

        /// <summary>
        /// The connection timeout (in milliseconds) when sending bulk operations to elasticsearch (defaults to 5000).
        /// </summary>
        public TimeSpan ConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the export processor type to be used with Jaeger Exporter.
        /// </summary>
        public ExportProcessorType ExportProcessorType { get; set; } = ExportProcessorType.Batch;

        /// <summary>
        /// A callback which can be used to handle logevents which are not submitted to Elasticsearch
        /// like when it is unable to accept the events. This is optional and depends on the EmitEventFailure setting.
        /// </summary>
        //public Action<LogEvent> FailureCallback { get; set; }

        /// <summary>
        /// Index aliases. Sets alias/aliases to an index in elasticsearch.
        /// Tested and works with ElasticSearch 7.x
        /// When using the <see cref="AutoRegisterTemplate"/> feature, this allows you to set index aliases.
        /// If not provided, index aliases will be blank.
        /// </summary>
        public string[] IndexAliases { get; set; }

        ///<summary>
        /// The index name formatter. A string.Format using the DateTimeOffset of the event is run over this string.
        /// defaults to "logstash-{0:yyyy.MM.dd}"
        /// Needs to be lowercased.
        /// </summary>
        public string IndexFormat { get; set; }

        ///<summary>
        /// Connection configuration to use for connecting to the cluster.
        /// </summary>
        public Func<ConnectionConfiguration, ConnectionConfiguration> ModifyConnectionSettings { get; set; }

        /// <summary>
        /// When using the <see cref="AutoRegisterTemplate"/> feature, this allows you to overwrite the template in Elasticsearch if it already exists.
        /// Defaults to: false
        /// </summary>
        public bool OverwriteTemplate { get; set; }

        /// <summary>
        /// Specifies the option on how to handle failures when writing the template to Elasticsearch. This is only applicable when using the AutoRegisterTemplate option.
        /// </summary>
        public RegisterTemplateRecovery RegisterTemplateFailure { get; set; }

        ///<summary>
        /// When passing a serializer unknown object will be serialized to object instead of relying on their ToString representation
        /// </summary>
        public IElasticsearchSerializer Serializer { get; set; }

        ///<summary>
        /// When using the <see cref="AutoRegisterTemplate"/> feature this allows you to override the default template name.
        /// Defaults to: telemetry-events-template
        /// </summary>
        public string TemplateName { get; set; }

        ///<summary>
        /// The default elasticsearch type name to use for the log events. Defaults to: logevent.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Configures the elasticsearch exporter
        /// </summary>
        /// <param name="node">The node to write to</param>
        public ElasticsearchExporterOptions(Uri node)
            : this(new[] { node }) { }

        /// <summary>
        /// Configures the elasticsearch  exporter
        /// </summary>
        /// <param name="nodes">The nodes to write to</param>
        public ElasticsearchExporterOptions(IEnumerable<Uri> nodes)
            : this()
        {
            var materialized = nodes?.Where(n => n != null).ToArray();
            if (materialized == null || materialized.Length == 0)
                materialized = new[] { _defaultNode };
            if (materialized.Length == 1)
                ConnectionPool = new SingleNodeConnectionPool(materialized.First());
            else
                ConnectionPool = new StaticConnectionPool(materialized);
        }

        public ElasticsearchExporterOptions()
        {
            this.AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7;
            this.ConnectionPool = new SingleNodeConnectionPool(_defaultNode);
            this.ConnectionTimeout = TimeSpan.FromSeconds(5);
            this.IndexFormat = "logstash-{0:yyyy.MM.dd}";
            this.RegisterTemplateFailure = RegisterTemplateRecovery.IndexAnyway;
            this.TemplateName = "telemetry-events-template";
            this.TypeName = DefaultTypeName;
        }
    }
}
