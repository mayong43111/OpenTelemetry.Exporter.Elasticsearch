using System.Runtime.Serialization;

namespace OpenTelemetry.Exporter.Elasticsearch
{
    sealed class ElasticCreateAction
    {
        public ElasticCreateAction(ElasticActionPayload payload)
        {
            Payload = payload;
        }

        [DataMember(Name = "create")]
        public ElasticActionPayload Payload { get; }
    }
}
