using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.SecurityToken;

public class StsHelper
{
    private readonly AmazonCognitoIdentityClient _cognitoIdentityClient;
    private readonly string _identityPoolId;

    public StsHelper(string identityPoolId)
    {
        _cognitoIdentityClient = new AmazonCognitoIdentityClient();
        _identityPoolId = identityPoolId;
    }

    public async Task<string> GetCurrentRoleArnAsync(Credentials credentials)
    {
        var stsClient = new AmazonSecurityTokenServiceClient(credentials);

        var getCallerIdentityRequest = new Amazon.SecurityToken.Model.GetCallerIdentityRequest();
        var response = await stsClient.GetCallerIdentityAsync(getCallerIdentityRequest);

        return response.Arn;
    }
    public async Task<Credentials> GetCredentialsAsync(string idToken)
    {
        var getIdRequest = new GetIdRequest
        {
            IdentityPoolId = _identityPoolId,
            Logins = new Dictionary<string, string>
            {
                { $"cognito-idp.eu-central-1.amazonaws.com/eu-central-1_D0ieJaCxH", idToken }
            }
        };

        var getIdResponse = await _cognitoIdentityClient.GetIdAsync(getIdRequest);
        var identityId = getIdResponse.IdentityId;

        var getCredentialsRequest = new GetCredentialsForIdentityRequest
        {
            CustomRoleArn = "arn:aws:iam::842507363149:role/iData_Chris_test_02",
            IdentityId = identityId,
            Logins = new Dictionary<string, string>
            {
                { $"cognito-idp.eu-central-1.amazonaws.com/eu-central-1_D0ieJaCxH", idToken }
            }
        };

        var response = await _cognitoIdentityClient.GetCredentialsForIdentityAsync(getCredentialsRequest);

        var roleArn = await GetCurrentRoleArnAsync(response.Credentials);
        Console.WriteLine($"Current Role ARN: {roleArn}");

        return response.Credentials;
    }
}
