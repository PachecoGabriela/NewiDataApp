using Newtonsoft.Json;

public class TokenStorage
{
    private const string TokenFilePath = "tokens.json";

    public void StoreTokens(CognitoTokens tokens)
    {
        var json = JsonConvert.SerializeObject(tokens);
        File.WriteAllText(TokenFilePath, json);
    }

    public CognitoTokens RetrieveTokens()
    {
        if (!File.Exists(TokenFilePath))
        {
            return null;
        }

        var json = File.ReadAllText(TokenFilePath);
        return JsonConvert.DeserializeObject<CognitoTokens>(json);
    }
}
