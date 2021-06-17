using System;
using System.Collections.Generic;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    public class ElasticsearchTemplateProvider
    {
        public static object GetTemplate(ElasticsearchExporterOptions options,
            string discoveredVersion,
            Dictionary<string, string> settings,
            string templateMatchString,
            AutoRegisterTemplateVersion version = AutoRegisterTemplateVersion.ESv7)
        {
            switch (version)
            {
                case AutoRegisterTemplateVersion.ESv7:
                    return GetTemplateESv7(options, discoveredVersion, settings, templateMatchString);
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }
        }

        private static object GetTemplateESv7(ElasticsearchExporterOptions options, string discoveredVersion,
            Dictionary<string, string> settings,
            string templateMatchString)
        {
            object mappings = new
            {
                dynamic_templates = new List<Object>
                {
                    //when you use serilog as an adaptor for third party frameworks
                    //where you have no control over the log message they typically
                    //contain {0} ad infinitum, we force numeric property names to
                    //contain strings by default.
                    {
                        new
                        {
                            numerics_in_fields = new
                            {
                                path_match = @"fields\.[\d+]$",
                                match_pattern = "regex",
                                mapping = new
                                {
                                    type = "text",
                                    index = true,
                                    norms = false
                                }
                            }
                        }
                    },
                    {
                        new
                        {
                            string_fields = new
                            {
                                match = "*",
                                match_mapping_type = "string",
                                mapping = new
                                {
                                    type = "text",
                                    index = true,
                                    norms = false,
                                    fields = new
                                    {
                                        raw = new
                                        {
                                            type = "keyword",
                                            index = true,
                                            ignore_above = 256
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                properties = new Dictionary<string, object>
                {
                    {"message", new {type = "text", index = true}},
                    {
                        "exceptions", new
                        {
                            type = "nested",
                            properties = new Dictionary<string, object>
                            {
                                {"Depth", new {type = "integer"}},
                                {"RemoteStackIndex", new {type = "integer"}},
                                {"HResult", new {type = "integer"}},
                                {"StackTraceString", new {type = "text", index = true}},
                                {"RemoteStackTraceString", new {type = "text", index = true}},
                                {
                                    "ExceptionMessage", new
                                    {
                                        type = "object",
                                        properties = new Dictionary<string, object>
                                        {
                                            {"MemberType", new {type = "integer"}},
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            mappings = discoveredVersion?.StartsWith("6.") ?? false ? new { _doc = mappings } : mappings;

            Dictionary<string, object> aliases = new Dictionary<string, object>();

            //If index alias or aliases are specified
            if (options.IndexAliases?.Length > 0)
                foreach (var alias in options.IndexAliases)
                {
                    //Added blank object for alias to make look like this in JSON:
                    //"alias_1" : {}
                    aliases.Add(alias, new object());
                }

            return new
            {
                index_patterns = new[] { templateMatchString },
                settings = settings,
                mappings = mappings,
                aliases = aliases,
            };
        }
    }
}
