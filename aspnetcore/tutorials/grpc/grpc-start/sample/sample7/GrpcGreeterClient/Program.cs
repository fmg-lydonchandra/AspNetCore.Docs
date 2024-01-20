using System.Net;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcGreeterClient;

var proxy = new WebProxy("http://127.0.0.1:8888");//charles
// var proxy = new WebProxy("http://127.0.0.1:8866"); //fiddler
var httpClientHandler = new HttpClientHandler()
{
    Proxy = proxy,
    UseProxy = true,
    // UseProxy = false,
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
};

var httpClient = new HttpClient(httpClientHandler)
{
    DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower,
    DefaultRequestVersion = HttpVersion.Version20
};
// proxy smoke test
// httpClient.BaseAddress = new Uri("http://www.google.com");
// var result = httpClient.GetStringAsync("/").GetAwaiter().GetResult();

// Grpc must run as HTTPS so it is proxied through Charles
var channel = GrpcChannel.ForAddress("https://127.0.0.1:5000", new GrpcChannelOptions()
{
    HttpClient = httpClient
});

var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
