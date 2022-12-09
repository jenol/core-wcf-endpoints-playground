using CoreWCF.Description;
using ProtoBuf.Meta;
using ProtoBuf.ServiceModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;

namespace CoreWcfProtoEndpointBehavior
{
    public sealed class ProtoOperationBehavior : DataContractSerializerOperationBehavior
    {
        private TypeModel _model;

        public ProtoOperationBehavior(OperationDescription operation) : base(operation)
        {
            _model = RuntimeTypeModel.Default;
        }

        public TypeModel Model
        {
            get { return _model; }
            set
            {
                _model = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            if (_model is null)
            {
                throw new InvalidOperationException("No Model instance has been assigned to the ProtoOperationBehavior");
            }

            return XmlProtoSerializer.TryCreate(_model, type) ?? base.CreateSerializer(type, name, ns, knownTypes);
        }
    }
}
