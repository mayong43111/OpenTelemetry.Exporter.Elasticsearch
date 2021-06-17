using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    /// <summary>
    /// Specifies what to do when the template could not be created. This can mean that your data is not correctly indexed, so you might want to handle this failure.
    /// </summary>
    public enum RegisterTemplateRecovery
    {
        /// <summary>
        /// Ignore the issue and keep indexing. This is the default option.
        /// </summary>
        IndexAnyway = 1,

        /// <summary>
        /// When the template cannot be registered, throw an exception and fail the exporter.
        /// </summary>
        FailExporter = 8
    }
}
