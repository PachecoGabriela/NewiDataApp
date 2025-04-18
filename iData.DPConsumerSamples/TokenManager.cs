using System.IdentityModel.Tokens.Jwt;

public class TokenManager
{
    private readonly CognitoAuth _cognitoAuth;
    private readonly TokenStorage _tokenStorage;

    public TokenManager(CognitoAuth cognitoAuth)
    {
        _cognitoAuth = cognitoAuth;
        _tokenStorage = new TokenStorage();
    }
    public async Task<CognitoTokens> GetValidTokensAsync()
    {
        var tokens = _tokenStorage.RetrieveTokens();
        if (tokens == null || IsTokenExpired(tokens.IdToken))
        {
            tokens = await RefreshTokensAsync(tokens?.RefreshToken);
            _tokenStorage.StoreTokens(tokens);
        }
        return tokens;
    }

    private bool IsTokenExpired(string token)
    {
        if (string.IsNullOrEmpty(token)) return true;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        return jsonToken.ValidTo < DateTime.UtcNow;
    }
    private async Task<CognitoTokens> RefreshTokensAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new Exception("No refresh token available.");
        }

        var refreshResponse = await _cognitoAuth.RefreshTokensAsync(refreshToken);
        return new CognitoTokens
        {
            IdToken = refreshResponse.IdToken,
            AccessToken = refreshResponse.AccessToken,
            RefreshToken = refreshToken, 
            ExpiresInSeconds = refreshResponse.ExpiresInSeconds
        };
    }
    public async Task<CognitoTokens> ExchangeCodeForTokensAndStoreAsync(string code)
    {
        var tokens = await _cognitoAuth.ExchangeCodeForTokensAsync(code);
        _tokenStorage.StoreTokens(tokens);
        return tokens;
    }
}
