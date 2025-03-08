using System.Text.Json.Serialization;

namespace LBJ.Models.Twitter;

public class TweetData
{
    [JsonPropertyName("edit_history_tweet_ids")]
    public List<string> EditHistoryTweetIds { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}
