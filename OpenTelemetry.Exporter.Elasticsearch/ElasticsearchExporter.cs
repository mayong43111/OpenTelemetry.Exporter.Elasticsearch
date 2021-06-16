namespace OpenTelemetry.Exporter.Elasticsearch
{
    public abstract class ElasticsearchExporter<T> : BaseExporter<T>
        where T : class
    {
        private readonly ElasticsearchExporterOptions options;

        protected ElasticsearchExporter(ElasticsearchExporterOptions options)
        {
            this.options = options;
        }
    }
}
