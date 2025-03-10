using System.Text.Json.Serialization;

namespace LBJ.Models.Twitter;

public class TweetsResponse
{
    [JsonPropertyName("data")]
    public List<TweetData> Data { get; set; }
}