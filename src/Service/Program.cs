using Contracts;
using CoreWCF.Configuration;
using CoreWcfProtoEndpointBehavior;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services.AddTransient<Service>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
    options.ListenAnyIP(8081);
    options.ListenAnyIP(8082);
    options.ListenAnyIP(8083);
});

var app = builder.Build();

const bool isHttps = false;

// Slowest, SOAP
var basicHttpBinding = new BasicHttpBinding(isHttps ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None)
{ 
    MessageEncoding = WSMessageEncoding.Text
};

// Faster, base64 binary in a SOAP envelope
var mtomHttpBinding = new BasicHttpBinding(isHttps ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None)
{
    MessageEncoding = WSMessageEncoding.Mtom
};

// Fastest but only .NET clients, Content-Type `application/soap+msbin1`
var httpBinaryBinding = new CustomBinding(
                new BinaryMessageEncodingBindingElement 
                { 
                    // CompressionFormat = CompressionFormat.GZip
                },
                isHttps ? new HttpsTransportBindingElement() : new HttpTransportBindingElement());

var protobufHttpBinding = new BasicHttpBinding(isHttps ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None)
{
    MessageEncoding = WSMessageEncoding.Text
};

try
{
    app.UseServiceModel(serviceBuilder =>
    {
        try
        {
            serviceBuilder.AddService<Service>();

            serviceBuilder.AddServiceEndpoint<Service, IService>(
                basicHttpBinding, 
                new Uri("http://localhost:8080/Service1.svc"));

            serviceBuilder.AddServiceEndpoint<Service, IService>(
                mtomHttpBinding, 
                new Uri("http://localhost:8081/Service2.svc"));

            serviceBuilder.AddServiceEndpoint<Service, IService>(
                httpBinaryBinding, 
                new Uri("http://localhost:8082/Service3.svc"));

            serviceBuilder.AddServiceEndpoint<Service, IService>(
                protobufHttpBinding, 
                new Uri("http://localhost:8083/Service4.svc"), 
                endpoint =>
                {
                    endpoint.EndpointBehaviors.Add(new ProtoEndpointBehavior());
                });

            var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();

            serviceMetadataBehavior.HttpsGetEnabled = isHttps;
            serviceMetadataBehavior.HttpGetEnabled = !isHttps;
        }
        catch (Exception e)
        {
            throw;
        }
    });

    app.Run();
}
catch (Exception e)
{
    throw;
}