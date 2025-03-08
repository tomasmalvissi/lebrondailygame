using System.Text.Json.Serialization;

namespace LBJ.Models.Twitter;

public class TweetResponse
{
    [JsonPropertyName("data")]
    public TweetData Data { get; set; }
}