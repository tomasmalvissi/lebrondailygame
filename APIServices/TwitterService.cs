using LBJ.APIServices.Contracts;
using LBJ.Helpers;
using LBJ.Models.Twitter;
using LBJ.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Web;

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
            bool isDuplicate = await IsDuplicateTweetAsync(message);
            if (isDuplicate)
            {
                _logger.LogInformation("Tweet not published because it's a duplicate: {Message}", message);
                Environment.Exit(0);
            }

            var payload = new { text = message };
            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, _options.Value.BaseUrl + "/tweets")
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

    public async Task<bool> IsDuplicateTweetAsync(string message)
    {
        try
        {
            string userId = _options.Value.UserId;
            string endpoint = $"{_options.Value.BaseUrl}users/{userId}/tweets";

            var uriBuilder = new UriBuilder(endpoint);
            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
            var authHeader = _oauthHelper.GenerateAuthorizationHeader(uriBuilder.Uri.ToString(), "GET");
            request.Headers.Add("Authorization", authHeader);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Raw API Response: {RawResponse}", responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Twitter API error when searching tweets: {StatusCode} - {Content}",
                    response.StatusCode, responseContent);
                return false;
            }

            var tweetsResponse = JsonSerializer.Deserialize<TweetsResponse>(responseContent);

            if (tweetsResponse?.Data != null)
            {
                string normalizedMessage = TweetMessageFormatHelper.NormalizeTextForComparison(message);
                foreach (var tweet in tweetsResponse.Data)
                {
                    if (TweetMessageFormatHelper.NormalizeTextForComparison(tweet.Text) == normalizedMessage)
                    {
                        _logger.LogWarning("Duplicate tweet found with ID: {TweetId}", tweet.Id);
                        return true;
                    }
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for duplicate tweets: {Message}", ex.Message);
            return false;
        }
    }
}