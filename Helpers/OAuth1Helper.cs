using LBJ.Options;
using System.Security.Cryptography;
using System.Text;

namespace LBJ.Helpers;

public class OAuth1Helper
{
    private readonly TwitterOptions _options;

    public OAuth1Helper(TwitterOptions options)
    {
        _options = options;
    }

    public string GenerateAuthorizationHeader(string url, string method)
    {
        string nonce = Convert.ToBase64String(
            new ASCIIEncoding().GetBytes(
                DateTime.Now.Ticks.ToString()
            )
        );

        var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        string timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

        var parameters = new Dictionary<string, string>
            {
                { "oauth_consumer_key", _options.ConsumerKey },
                { "oauth_nonce", nonce },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_timestamp", timestamp },
                { "oauth_token", _options.AccessToken },
                { "oauth_version", "1.0" }
            };

        var signatureBase = new StringBuilder();
        signatureBase.Append(method + "&");
        signatureBase.Append(Uri.EscapeDataString(url) + "&");

        var paramString = parameters.OrderBy(p => p.Key)
            .Select(p => string.Format("{0}={1}", Uri.EscapeDataString(p.Key), Uri.EscapeDataString(p.Value)))
            .Aggregate((a, b) => a + "&" + b);

        signatureBase.Append(Uri.EscapeDataString(paramString));

        var signingKey = Uri.EscapeDataString(_options.ConsumerSecret) + "&" +
                         Uri.EscapeDataString(_options.AccessTokenSecret);

        var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
        var signatureBytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBase.ToString()));
        var signature = Convert.ToBase64String(signatureBytes);

        parameters.Add("oauth_signature", signature);

        var headerFormat = "OAuth {0}";
        var headerParams = parameters.OrderBy(p => p.Key)
            .Select(p => string.Format("{0}=\"{1}\"",
                Uri.EscapeDataString(p.Key),
                Uri.EscapeDataString(p.Value)))
            .Aggregate((a, b) => a + ", " + b);

        return string.Format(headerFormat, headerParams);
    }
}
