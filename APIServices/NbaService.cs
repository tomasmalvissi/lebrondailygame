using LBJ.APIServices.Contracts;
using LBJ.Models.Nba;
using LBJ.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Runtime;

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
            var response = await _httpClient.GetFromJsonAsync<GameResponse>(url);
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
        return DateTime.Now.ToString("yyyy-MM-dd");
    }
}