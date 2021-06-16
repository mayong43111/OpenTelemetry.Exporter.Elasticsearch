using OpenTelemetry.Exporter.Elasticsearch;
using System;

namespace OpenTelemetry.Trace
{
    public static class ElasticsearchExporterHelperExtensions
    {
        /// <summary>
        /// Adds Elasticsearch Exporter as a configuration to the OpenTelemetry ILoggingBuilder.
        /// </summary>
        /// <param name="builder"><see cref="TracerProviderBuilder"/> options to use.</param>
        /// <param name="configure">Exporter configuration options.</param>
        /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
        public static TracerProviderBuilder AddElasticsearchExporter(this TracerProviderBuilder builder, Action<ElasticsearchExporterOptions> configure = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var options = new ElasticsearchExporterOptions();
            configure?.Invoke(options);

            var exporter = new ElasticsearchActivityExporter(options);

            if (options.ExportProcessorType == ExportProcessorType.Simple)
            {
                return builder.AddProcessor(new SimpleActivityExportProcessor(exporter));
            }
            else
            {
                return builder.AddProcessor(new BatchActivityExportProcessor(
                    exporter,
                    options.BatchExportProcessorOptions.MaxQueueSize,
                    options.BatchExportProcessorOptions.ScheduledDelayMilliseconds,
                    options.BatchExportProcessorOptions.ExporterTimeoutMilliseconds,
                    options.BatchExportProcessorOptions.MaxExportBatchSize));
            }
        }
    }
}
