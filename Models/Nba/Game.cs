using System.Text.Json.Serialization;

namespace LBJ.Models.Nba;

public class Game
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("home_team")]
    public Team HomeTeam { get; set; }

    [JsonPropertyName("home_team_score")]
    public int HomeTeamScore { get; set; }

    [JsonPropertyName("visitor_team")]
    public Team VisitorTeam { get; set; }

    [JsonPropertyName("visitor_team_score")]
    public int VisitorTeamScore { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("datetime")]
    public string Datetime { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; }

    [JsonPropertyName("period")]
    public int Period { get; set; }

    [JsonPropertyName("season")]
    public int Season { get; set; }
}
