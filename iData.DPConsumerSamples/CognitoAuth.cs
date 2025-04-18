using System.Text;
using Newtonsoft.Json.Linq;
public class CognitoAuth
{
    private static readonly HttpClient client = new HttpClient();

    private const string RedirectUrl = "https://ui.dev.idata.navify.com/signin-oidc";
    private const string CognitoClientId = "3t2dfvjt1j2pe23i0r2lbabahl";
    private const string CognitoClientSecret = "c68n2lvedo4v9rhvhj8rdnt08l1lclastb06bna7pdrtf9cfn6f";
    private const string CognitoDomain = "https://idata-dev.auth.eu-central-1.amazoncognito.com";
    private const string TokenUrl = CognitoDomain + "/oauth2/token";

    public async Task<CognitoTokens> ExchangeCodeForTokensAsync(string code)
    {
        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{CognitoClientId}:{CognitoClientSecret}"));

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("redirect_uri", RedirectUrl),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", CognitoClientId)
        });

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

        var response = await client.PostAsync(TokenUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();

        var responseBody = JObject.Parse(responseString);

        return new CognitoTokens
        {
            IdToken = responseBody["id_token"].ToString(),
            AccessToken = responseBody["access_token"].ToString(),
            RefreshToken = responseBody["refresh_token"].ToString(),
            ExpiresInSeconds = int.Parse(responseBody["expires_in"].ToString())
        };
    }

    public async Task<RefreshTokenResponse> RefreshTokensAsync(string refreshToken)
    {
        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{CognitoClientId}:{CognitoClientSecret}"));

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("client_id", CognitoClientId),
            new KeyValuePair<string, string>("refresh_token", refreshToken)
        });

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

        var response = await client.PostAsync(TokenUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();

        var responseBody = JObject.Parse(responseString);

        return new RefreshTokenResponse
        {
            IdToken = responseBody["id_token"].ToString(),
            AccessToken = responseBody["access_token"].ToString(),
            TokenType = responseBody["token_type"].ToString(),
            ExpiresInSeconds = int.Parse(responseBody["expires_in"].ToString())
        };
    }
}
