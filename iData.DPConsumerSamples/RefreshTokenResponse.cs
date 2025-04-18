public class RefreshTokenResponse
{
    public string IdToken { get; set; }
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresInSeconds { get; set; }
}
