using LBJ.APIServices.Contracts;
using LBJ.Helpers;
using LBJ.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LBJ.BackgroudServices;

public class NbaPublisherService : BackgroundService
{
    private readonly INbaService _nbaService;
    private readonly ITwitterService _twitterService;
    private readonly IOptions<NbaApiOptions> _options;
    private readonly ILogger<NbaPublisherService> _logger;

    public NbaPublisherService(
        INbaService nbaService,
        ITwitterService twitterService,
        IOptions<NbaApiOptions> options,
        ILogger<NbaPublisherService> logger)
    {
        _nbaService = nbaService;
        _twitterService = twitterService;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting NBA service");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Obtener partidos
                var match = await _nbaService.GetLakersGames();

                if (match is not null && match.HomeTeamScore is 0)
                {
                    // Formatear mensaje
                    var message = TweetMessageFormatHelper.FormatResponseForTweet(match);

                    // Publicar en Twitter
                    await _twitterService.PublishTweetAsync(message);

                    _logger.LogInformation("Succesfully tweeted");
                }
                else
                {
                    _logger.LogInformation("Not matches found from today");
                }

                // Determinar intervalo de espera
                var delayMinutes = _options.Value.CheckIntervalHours;
                _logger.LogInformation("Waiting {Minutes} minutes until next cicle", delayMinutes);

                await Task.Delay(TimeSpan.FromMinutes(delayMinutes), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in publish cicle");

                // Esperar menos tiempo en caso de error antes de reintentar
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
