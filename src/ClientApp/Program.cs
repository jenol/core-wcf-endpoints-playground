// See https://aka.ms/new-console-template for more information
using ClientApp;
using Contracts;
using System.ServiceModel;
using System.ServiceModel.Channels;

class Program
{
    static async Task<T> WithServiceClient<T>(
        EndpointConfiguration endpointConfiguration, 
        Uri endpointUri, Func<ServiceClient, Task<T>> task)
    {
        var client = new ServiceClient(endpointConfiguration, endpointUri);
        return await WithServiceClient(client, task);
    }

    static async Task<T> WithServiceClient<T>(ServiceClient client, Func<ServiceClient, Task<T>> task)
    {
        using (var scope = new OperationContextScope(client.InnerChannel))
        {
            var result = await task(client);

            var httpProperties = (HttpResponseMessageProperty)OperationContext.Current
                             .IncomingMessageProperties[HttpResponseMessageProperty.Name];

            Console.WriteLine($"Content-Length: {httpProperties.Headers["Content-Length"]}");

            return result;
        }
    }

    static async Task<CompositeType> RunTest2(ServiceClient client)
    {
        var complexData = await client.GetDataUsingDataContract(new CompositeType
        {
            BoolValue = true,
            StringValue = Guid.NewGuid().ToString()
        });

        return complexData;
    }

    static async Task Main(string[] args)
    {
        await Task.Delay(5000);

        Console.WriteLine("SOAP");

        await WithServiceClient(
            EndpointConfiguration.SOAP,
            new Uri("http://localhost:8080/Service1.svc"),
            RunTest2);

        Console.WriteLine("MTOM");

        await WithServiceClient(
            EndpointConfiguration.MTOM,
            new Uri("http://localhost:8081/Service2.svc"),
            RunTest2);

        Console.WriteLine("Binary");

        var o = await WithServiceClient(
            EndpointConfiguration.Binary,
            new Uri("http://localhost:8082/Service3.svc"),
            RunTest2);

        Console.WriteLine("Protobuf");

        o = await WithServiceClient(
            EndpointConfiguration.Proto,
            new Uri("http://localhost:8083/Service4.svc"), 
            RunTest2);

        Console.WriteLine("end");
    }
}