using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var url = builder.Configuration.GetValue<string>("services:service:api:0");

using var httpClient = new HttpClient();
while (true)
{
    try
    {
        var content = await httpClient.GetStringAsync(url + "/roll");
        Console.WriteLine(content);
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine(ex.Message);
    }

    Thread.Sleep(50);
}