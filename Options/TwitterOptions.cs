namespace LBJ.Options;

public class TwitterOptions
{
    public const string ConfigSection = "Twitter";

    public string BaseUrl { get; set; }
    public string BearerToken { get; set; }
    public string ConsumerKey { get; set; }
    public string ConsumerSecret { get; set; }
    public string AccessToken { get; set; }
    public string AccessTokenSecret { get; set; }
}
