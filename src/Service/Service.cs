using Contracts;

namespace MyService
{
    public class Service : IService
    {
        private readonly ILogger<Service> _logger;

        public Service(ILogger<Service> logger)
        {
            _logger = logger;
        }


        public async Task<CompositeType> GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }

            var endpointAddress = OperationContext.Current.EndpointDispatcher.EndpointAddress.ToString();

            _logger.LogInformation("You called {0} endpointAddress. StringValue: {1}", endpointAddress, composite.StringValue);

            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return await Task.FromResult(composite);
        }
    }

 }
