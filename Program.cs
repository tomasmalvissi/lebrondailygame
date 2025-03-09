using LBJ.APIServices;
using LBJ.APIServices.Contracts;
using LBJ.BackgroudServices;
using LBJ.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // Setup configurations
                services.Configure<NbaApiOptions>(
                    hostContext.Configuration.GetSection(NbaApiOptions.ConfigSection));
                services.Configure<TwitterOptions>(
                    hostContext.Configuration.GetSection(TwitterOptions.ConfigSection));

                // Setup HttpFactory
                services.AddHttpClient("TwitterClient");
                services.AddHttpClient("BallDontLie", client =>
                {
                    var nbaOptions = hostContext.Configuration.GetSection(NbaApiOptions.ConfigSection).Get<NbaApiOptions>();
                    client.BaseAddress = new Uri(nbaOptions.BaseUrl);
                    client.DefaultRequestHeaders.Add("Authorization", nbaOptions.ApiKey);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(30));

                // Setup BG
                services.AddHostedService<NbaPublisherService>();

                // Setup services
                services.AddTransient<INbaService, NbaService>();
                services.AddSingleton<ITwitterService, TwitterService>();
            })
            .ConfigureLogging((hostContext, loggingBuilder) =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            })
            .Build();

        await host.RunAsync();
    }
}