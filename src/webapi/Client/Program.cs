using System.Security.Cryptography.X509Certificates;
using Models;
using Newtonsoft.Json;

#if RELEASE
const string baseUrl = "https://mtls-example-webapi.friday.local";
#else
const string baseUrl = "https://localhost:5684";
#endif

Console.WriteLine("Getting the weather forecast... ");

// The client certificate is copied during app build...
var clientCertPath = Path.Join(AppContext.BaseDirectory, "client.pfx");
var clientCert = new X509Certificate2(clientCertPath);

using var httpClient = new HttpClient(new HttpClientHandler
{
    ClientCertificates = { clientCert }
});

var httpRequestMsg = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/WeatherForecast");

var httpResponseMsg = await httpClient.SendAsync(httpRequestMsg);

if (!httpResponseMsg.IsSuccessStatusCode)
    throw new Exception("The server didn't like our request :(");

var contentStream = await httpResponseMsg.Content.ReadAsStringAsync();
var response = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(contentStream);

foreach (var weatherForecast in response!)
{
    Console.WriteLine($"Day {weatherForecast.Date:MM/dd/yyyy}: Weather {weatherForecast.Summary}: Temperature {weatherForecast.TemperatureC}º C");
}