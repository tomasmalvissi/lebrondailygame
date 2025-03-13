using LBJ.APIServices.Contracts;
using LBJ.Models.Nba;
using LBJ.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Runtime;
using System.Text.Json;

namespace LBJ.APIServices;

public class NbaService : INbaService
{
    private readonly ILogger<NbaService> _logger;
    private readonly HttpClient _httpClient;
    private readonly NbaApiOptions _options;

    public NbaService(
            ILogger<NbaService> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<NbaApiOptions> options)
    {
        _logger = logger;
        _options = options.Value;

        _httpClient = httpClientFactory.CreateClient("BallDontLie");
    }

    public async Task<Game> GetLakersGames()
    {
        var games = await GetGamesForDateAsync(FormatDateToRequest());

        games = games.OrderByDescending(x => x.Id).ToList();

        var lakers = games.FirstOrDefault(g =>
                        g.HomeTeam.Id == _options.TeamId ||
                        g.VisitorTeam.Id == _options.TeamId);

        return lakers ?? new Game();
    }

    private async Task<List<Game>> GetGamesForDateAsync(string date)
    {
        try
        {
            var url = $"{_options.BaseUrl.TrimEnd('/')}/games?dates[]={date}";
            _logger.LogInformation("Raw API Request: {RawRequest}", url);
            var rawResponse = await _httpClient.GetStringAsync(url);

            _logger.LogInformation("Raw API Response: {RawResponse}", rawResponse);
            var response = JsonSerializer.Deserialize<GameResponse>(rawResponse);

            return response?.Data ?? new List<Game>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener partidos de la API para la fecha {Date}", date);
            throw;
        }
    }

    private string FormatDateToRequest()
    {
        DateTime today = DateTime.Now;
        DateTime yesterday = today.AddDays(-1);
        return $"{yesterday:yyyy-MM-dd}&{today:yyyy-MM-dd}";
    }
}