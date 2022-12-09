using CoreWCF;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract, System.ServiceModel.ServiceContract]
    public interface IService
    {
        [OperationContract, System.ServiceModel.OperationContract]
        Task<CompositeType> GetDataUsingDataContract(CompositeType composite);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        [DataMember(Order = 1)]
        public bool BoolValue
        {
            get;set;
        }

        [DataMember(Order = 2)]
        public string StringValue
        {
            get;set;
        }
    }
}
