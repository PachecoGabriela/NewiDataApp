using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Text.Json;

public class CustomGroupClaimAction : ClaimAction
{
    public CustomGroupClaimAction(string claimType, string valueType) : base(claimType, valueType) { }

    public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
    {
        if (userData.TryGetProperty("custom:groups", out JsonElement claimValues))
        {
            var claimString = claimValues.GetString();
            try
            {
                var roles = JsonSerializer.Deserialize<List<string>>(claimString);
                if (roles != null)
                {
                    roles.ForEach(role => identity.AddClaim(new Claim(ClaimType, role, ValueType, issuer)));
                }
            }
            catch (JsonException)
            {
                identity.AddClaim(new Claim(ClaimType, claimString, ValueType, issuer));
            }
        }
    }
}