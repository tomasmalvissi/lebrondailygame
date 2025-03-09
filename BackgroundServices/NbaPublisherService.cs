using LBJ.APIServices.Contracts;
using LBJ.Helpers;
using LBJ.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LBJ.BackgroudServices;

public class NbaPublisherService : IHostedService
{
    private readonly INbaService _nbaService;
    private readonly ITwitterService _twitterService;
    private readonly ILogger<NbaPublisherService> _logger;

    public NbaPublisherService(
        INbaService nbaService,
        ITwitterService twitterService,
        ILogger<NbaPublisherService> logger)
    {
        _nbaService = nbaService;
        _twitterService = twitterService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting NBA Publisher Service");

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
                _logger.LogInformation("Successfully tweeted about Lakers game");
            }
            else
            {
                _logger.LogInformation("No matches found for today");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in NBA publisher service");
            // En GitHub Actions, un código de salida distinto de cero indicará fallo
            Environment.ExitCode = 1;
        }
        finally
        {
            // Indicar que el host debe detenerse después de completar esta ejecución
            _logger.LogInformation("NBA Publisher Service completed, stopping host");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping NBA Publisher Service");
        return Task.CompletedTask;
    }
}
