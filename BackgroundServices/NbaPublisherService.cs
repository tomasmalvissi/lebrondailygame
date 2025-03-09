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
            if (match.Id is not 0 && match.HomeTeamScore is 0) //New Game
            {
                // Formatear mensaje
                var message = TweetMessageFormatHelper.FormatResponseForNewGame(match);
                // Publicar en Twitter
                await _twitterService.PublishTweetAsync(message);
                _logger.LogInformation("Successfully tweeted about Lakers game");
            }
            else if (match.Id is not 0 && match.Status is "Final") //Results from Game
            {
                // Formatear mensaje
                var message = TweetMessageFormatHelper.FormatResponseForFinishGame(match);
                // Publicar en Twitter
                await _twitterService.PublishTweetAsync(message);
                _logger.LogInformation("Successfully tweeted about Lakers game");
            }
            else
            {
                _logger.LogInformation("No matches found for today");
                Environment.Exit(0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in NBA publisher service");
            Environment.Exit(1);
        }
        finally
        {
            // Indicar que el host debe detenerse después de completar esta ejecución
            _logger.LogInformation("NBA Publisher Service completed, stopping host");
            Environment.Exit(0);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping NBA Publisher Service");
        return Task.CompletedTask;
    }
}
