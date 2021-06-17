using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    public class ElasticsearchActivityExporter : ElasticsearchExporter<Activity>
    {
        public ElasticsearchActivityExporter(ElasticsearchExporterOptions options)
           : base(options)
        {
        }

        public override ExportResult Export(in Batch<Activity> activityBatch)
        {
            try
            {
                var payload = CreatePlayLoad(activityBatch);
                if (payload == null)
                {
                    return ExportResult.Success;
                }

                var result = State.Client.BulkAsync<DynamicResponse>(PostData.MultiJson(payload));

                return ExportResult.Success;
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Failed to create the template. {0}", ex);
                return ExportResult.Failure;
            }
        }

        private IEnumerable<string> CreatePlayLoad(in Batch<Activity> activityBatch)
        {
            if (!State.TemplateRegistrationSuccess && Options.RegisterTemplateFailure == RegisterTemplateRecovery.FailExporter)
            {
                return null;
            }

            var payload = new List<string>();
            foreach (var activity in activityBatch)
            {
                var indexName = State.GetIndexForActivity(activity, activity.StartTimeUtc.ToUniversalTime());

                var action = CreateElasticAction(
                    indexName: indexName,
                    mappingType: Options.TypeName);
                payload.Add(LowLevelRequestResponseSerializer.Instance.SerializeToString(action));

                var sw = new StringWriter();
                State.Formatter.Format(activity, sw);
                payload.Add(sw.ToString());
            }

            return payload;
        }

        private object CreateElasticAction(string indexName, string mappingType = null)
        {
            var actionPayload = new ElasticActionPayload(
                indexName: indexName,
                mappingType: mappingType
            );

            var action = (object)new ElasticCreateAction(actionPayload);
            return action;
        }
    }
}
