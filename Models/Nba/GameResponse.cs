using System.Text.Json.Serialization;

namespace LBJ.Models.Nba;
public class GameResponse
{
    [JsonPropertyName("data")]
    public List<Game> Data { get; set; }

    [JsonPropertyName("meta")]
    public Meta Meta { get; set; }
}