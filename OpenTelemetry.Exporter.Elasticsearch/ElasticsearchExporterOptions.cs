using Elasticsearch.Net;
using System;
using System.Diagnostics;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    public class ElasticsearchExporterOptions
    {
        /// <summary>
        /// When set to true the sink will register an index template for the logs in elasticsearch.
        /// This template is optimized to deal with serilog events
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

        /// <summary>
        /// Gets or sets the export processor type to be used with Jaeger Exporter.
        /// </summary>
        public ExportProcessorType ExportProcessorType { get; set; } = ExportProcessorType.Batch;

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
    }
}
