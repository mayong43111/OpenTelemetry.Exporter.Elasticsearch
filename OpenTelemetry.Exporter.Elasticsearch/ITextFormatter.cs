using System.Diagnostics;
using System.IO;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    /// <summary>
    /// Formats log events in a textual representation.
    /// </summary>
    public interface ITextFormatter
    {
        /// <summary>
        /// Format the log event into the output.
        /// </summary>
        /// <param name="activity">The event to format.</param>
        /// <param name="output">The output.</param>
        void Format(Activity activity, TextWriter output);
    }
}
