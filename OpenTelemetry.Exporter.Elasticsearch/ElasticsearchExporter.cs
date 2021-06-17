namespace OpenTelemetry.Exporter.Elasticsearch
{
    public abstract class ElasticsearchExporter<T> : BaseExporter<T>
        where T : class
    {
        protected readonly ElasticsearchExporterOptions Options;

        protected readonly ElasticsearchExporterState State;

        protected ElasticsearchExporter(ElasticsearchExporterOptions options)
        {
            this.Options = options;

            State = ElasticsearchExporterState.Create(options);
            State.DiscoverClusterVersion();
            State.RegisterTemplateIfNeeded();
        }
    }
}
