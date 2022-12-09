using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;

namespace CoreWcfProtoEndpointBehavior
{
    public class ProtoEndpointBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            ReplaceDataContractSerializerOperationBehavior(endpoint);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            ReplaceDataContractSerializerOperationBehavior(endpoint);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        private static void ReplaceDataContractSerializerOperationBehavior(ServiceEndpoint serviceEndpoint)
        {
            foreach (var operationDescription in serviceEndpoint.Contract.Operations)
            {
                var behaviorType = typeof(DataContractSerializerOperationBehavior);

                if (!operationDescription.OperationBehaviors.TryGetValue(behaviorType, out IOperationBehavior dcsOperationBehavior))
                {
                    continue;
                }

                operationDescription.OperationBehaviors.Remove(typeof(DataContractSerializerOperationBehavior));

                operationDescription.OperationBehaviors.Add(new ProtoOperationBehavior(operationDescription)
                {
                    MaxItemsInObjectGraph = ((DataContractSerializerOperationBehavior)dcsOperationBehavior).MaxItemsInObjectGraph
                });
            }
        }
    }
}
