using System;
using System.Diagnostics;
using System.IO;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    public class ElasticsearchJsonFormatter : ITextFormatter
    {
        public void Format(Activity activity, TextWriter output)
        {
            throw new NotImplementedException();
        }
    }
}
