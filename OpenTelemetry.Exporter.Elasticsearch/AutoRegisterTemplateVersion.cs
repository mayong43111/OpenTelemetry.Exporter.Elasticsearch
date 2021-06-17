using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    /// <summary>
    ///
    /// </summary>
    public enum AutoRegisterTemplateVersion
    {
        /// <summary>
        /// Elasticsearch version &lt;= 2.4
        /// </summary>
        //ESv2 = 0,
        /// <summary>
        /// Elasticsearch version &lt;= version 5.6
        /// </summary>
        //ESv5 = 1,
        /// <summary>
        /// Elasticsearch version &gt;= version 6.0
        /// </summary>
        //ESv6 = 2,
        /// <summary>
        /// Elasticsearch version &gt;= version 7.0
        /// </summary>
        ESv7 = 3
    }
}
