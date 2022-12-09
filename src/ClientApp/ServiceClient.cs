using System.ServiceModel;
using System.ServiceModel.Channels;
using Contracts;
using ProtoBuf.ServiceModel;

namespace ClientApp
{
    internal class ServiceClient : ClientBase<IService>, IService
    {
        private readonly EndpointConfiguration _endpointConfiguration;

        public ServiceClient(EndpointConfiguration endpointConfiguration, Uri remoteAddress) : 
            base(GetBindingForEndpoint(endpointConfiguration), new EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            _endpointConfiguration = endpointConfiguration;
        }

        public async Task<CompositeType> GetDataUsingDataContract(CompositeType composite)
        {
            return await base.Channel.GetDataUsingDataContract(composite);
        }

        public virtual Task OpenAsync()
        {
            return Task.Factory.FromAsync(((ICommunicationObject)(this)).BeginOpen(null, null), new Action<IAsyncResult>(((ICommunicationObject)this).EndOpen));
        }

        protected override IService CreateChannel()
        {
            if (_endpointConfiguration == EndpointConfiguration.Proto)
            {
                ChannelFactory.Endpoint.EndpointBehaviors.Add(new ProtoEndpointBehavior());
            }

            return base.CreateChannel();
        }

        private static Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            switch (endpointConfiguration)
            {
                case EndpointConfiguration.SOAP:
                    {
                        BasicHttpBinding result = new BasicHttpBinding
                        {
                            MaxBufferSize = int.MaxValue,
                            ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                            MaxReceivedMessageSize = int.MaxValue,
                            AllowCookies = true
                        };
                        return result;
                    }

                case EndpointConfiguration.MTOM:
                    {
                        var result = new BasicHttpBinding
                        {
                            MaxBufferSize = int.MaxValue,
                            ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                            MaxReceivedMessageSize = int.MaxValue,
                            AllowCookies = true,
                            MessageEncoding = WSMessageEncoding.Mtom
                        };
                        return result;
                    }

                case EndpointConfiguration.Binary:
                    {
                        var result = new CustomBinding();
                        result.Elements.Add(new BinaryMessageEncodingBindingElement());
                        HttpTransportBindingElement httpBindingElement = new HttpTransportBindingElement();
                        httpBindingElement.AllowCookies = true;
                        httpBindingElement.MaxBufferSize = int.MaxValue;
                        httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                        result.Elements.Add(httpBindingElement);
                        return result;
                    }

                case EndpointConfiguration.Proto:
                    {
                        var result = new BasicHttpBinding();
                        result.MaxBufferSize = int.MaxValue;
                        result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                        result.MaxReceivedMessageSize = int.MaxValue;
                        result.AllowCookies = true;
                        return result;
                    }

                default:
                    throw new InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
            }
        }
    }
}
