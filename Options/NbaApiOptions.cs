namespace LBJ.Options;

public class NbaApiOptions
{
    public const string ConfigSection = "NbaApi";

    public string BaseUrl { get; set; }
    public int TeamId { get; set; }
    public string ApiKey { get; set; }
    public double CheckIntervalHours { get; set; }
}
