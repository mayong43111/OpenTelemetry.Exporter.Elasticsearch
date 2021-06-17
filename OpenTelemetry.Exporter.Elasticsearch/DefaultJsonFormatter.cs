using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    public abstract class DefaultJsonFormatter : ITextFormatter
    {
        private readonly IDictionary<Type, Action<object, bool, TextWriter>> _literalWriters;

        public void Format(Activity activity, TextWriter output)
        {
            throw new NotImplementedException();
        }
    }
}
