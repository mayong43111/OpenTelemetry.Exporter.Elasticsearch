using System;
using System.Diagnostics;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    public class ElasticsearchActivityExporter : ElasticsearchExporter<Activity>
    {
        public ElasticsearchActivityExporter(ElasticsearchExporterOptions options)
           : base(options)
        {
        }

        public override ExportResult Export(in Batch<Activity> batch)
        {
            throw new NotImplementedException();
        }
    }
}
