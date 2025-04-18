using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System.Security.Claims;
using Roche.Common.Security;

namespace iData.Blazor.Server.Services;

public class CustomAuthenticationProviderOptions
{

    public bool SnycRolesWithClaimGroups { get; set; } = true;
}

public class CustomAuthenticationProvider : IAuthenticationProviderV2
{
    private readonly IPrincipalProvider principalProvider;
    private readonly IConfiguration configuration;

    public CustomAuthenticationProvider(IPrincipalProvider principalProvider, IConfiguration configuration)
    {
        this.principalProvider = principalProvider;
        this.configuration = configuration;
    }


    public object Authenticate(IObjectSpace objectSpace)
    {
        Console.WriteLine("Log - Starting authentication process.");

        ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)principalProvider.User;
        var userName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

        Console.WriteLine($"Log - Attempting to authenticate user: {userName}");

        var user = objectSpace.FirstOrDefault<PermissionPolicyUser>(i => i.UserName == userName);

        if (user == null)
        {
            Console.WriteLine("Log - No existing user found. Creating a new user.");
            user = objectSpace.CreateObject<PermissionPolicyUser>();
            user.UserName = userName;
        }
        else
        {
            Console.WriteLine("Log - Existing user found.");
        }

        // Extract role names from the current user's claims
        var claimRoleNames = claimsPrincipal
            .FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .ToList();

        Console.WriteLine($"Log - Roles found in claims: {string.Join(", ", claimRoleNames)}");

        // Remove roles from the user that are not present in the claim roles
        foreach (CidmRole userRole in user.Roles.ToList())
        {
            if (!claimRoleNames.Contains(userRole.CidmGroup))
            {
                Console.WriteLine($"Log - Removing role not present in claims: {userRole.CidmGroup}");
                user.Roles.Remove(userRole);
            }
        }

        var existingRoles = objectSpace.GetObjects<CidmRole>()
            .Where(i => !string.IsNullOrEmpty(i.CidmGroup))
            .GroupBy(role => role.CidmGroup);

        // Add to the user any roles present in claim roles but missing from the user's roles
        foreach (var roleName in claimRoleNames)
        {
            var roles = existingRoles.FirstOrDefault(r => r.Key == roleName);
            if (roles != null)
            {

                foreach (var roleToAdd in roles)
                {
                    if (!user.Roles.Contains(roleToAdd))
                    {
                        Console.WriteLine($"Log - Adding missing role from claims: {roleName} - {roleToAdd.Name}");
                        user.Roles.Add(roleToAdd);
                    }
                }
            }
        }

        objectSpace.CommitChanges();
        Console.WriteLine("Log - Authentication process completed. Changes committed.");

        return new UserResult<PermissionPolicyUser>(user); ;
    }

}

