using LBJ.APIServices.Contracts;
using LBJ.Helpers;
using LBJ.Models.Twitter;
using LBJ.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace LBJ.APIServices;
public class TwitterService : ITwitterService
{
    private readonly HttpClient _httpClient;
    private readonly OAuth1Helper _oauthHelper;
    private readonly ILogger<TwitterService> _logger;
    private readonly IOptions<TwitterOptions> _options;

    public TwitterService(
        IOptions<TwitterOptions> options,
        ILogger<TwitterService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("TwitterClient");
        _oauthHelper = new OAuth1Helper(options.Value);
        _options = options;
    }

    public async Task<string> PublishTweetAsync(string message)
    {
        try
        {
            var payload = new { text = message };
            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, _options.Value.BaseUrl)
            {
                Content = content
            };

            var authHeader = _oauthHelper.GenerateAuthorizationHeader(_options.Value.BaseUrl, "POST");
            request.Headers.Add("Authorization", authHeader);

            var response = await _httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Raw API Response: {RawResponse}", responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Twitter API error: {StatusCode} - {Content}",
                    response.StatusCode, responseContent);
                throw new Exception($"Error posting tweet: {response.StatusCode} - {responseContent}");
            }

            var tweetResponse = JsonSerializer.Deserialize<TweetResponse>(responseContent);

            _logger.LogInformation("Tweet posted with ID: {TweetId}", tweetResponse.Data.Id);
            return tweetResponse.Data.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting tweet: {Message}", ex.Message);
            throw;
        }
    }
}