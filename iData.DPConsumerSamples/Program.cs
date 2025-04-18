var cognitoAuth = new CognitoAuth();
var tokenManager = new TokenManager(cognitoAuth);

CognitoTokens tokens = null;

try
{
    // Try to retrieve valid tokens from storage
    tokens = await tokenManager.GetValidTokensAsync();
}
catch (Exception ex)
{
    try
    {
        // If no valid tokens are found, perform the token exchange and store the tokens
        //sign in here:https://idata-dev.auth.eu-central-1.amazoncognito.com/oauth2/authorize?client_id=3t2dfvjt1j2pe23i0r2lbabahl&response_type=code
        //copy the code that is returend in the URL

        var code = "c65f693a-1c54-4d8c-b9bc-65df83f76a7e";
        tokens = await tokenManager.ExchangeCodeForTokensAndStoreAsync(code);
        Console.WriteLine("Exchanged authorization code for tokens and stored them.");
    }
    catch (Exception ex2)
    {
        Console.WriteLine($"Failed to exchange authorization code for tokens: {ex2.Message}");
        return; // Exit the program if token exchange fails
    }
}

var idToken = tokens.IdToken;


var stsHelper = new StsHelper("eu-central-1:0b0dc7d9-12b0-4b1b-afdf-116437995245");
var roleArn = "arn:aws:iam::842507363149:role/iData_Chris_test_01";

var credentials = await stsHelper.GetCredentialsAsync(idToken);

Console.WriteLine("Assumed role credentials:");
Console.WriteLine($"AccessKeyId: {credentials.AccessKeyId}");
Console.WriteLine($"SecretAccessKey: {credentials.SecretKey}");
Console.WriteLine($"SessionToken: {credentials.SessionToken}");