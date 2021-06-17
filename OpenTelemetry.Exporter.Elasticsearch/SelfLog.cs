using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    internal static class SelfLog
    {
        public static void WriteLine(string format, object arg0)
        {
            Console.WriteLine(format, arg0);
        }
    }
}
