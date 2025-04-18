using DevExpress.ExpressApp;
using Roche.Common.Security;

namespace iData.Blazor.Server.Services
{
    public static class RoleConfigurationUpdater
    {
        public static void CheckAndUpdateRoleGroupNames(IObjectSpaceProvider objectSpaceProvider, IConfiguration configuration)
        {
            using (var objectSpace = objectSpaceProvider.CreateUpdatingObjectSpace(true))
            {
                var rolesToUpdate = new Dictionary<string, string>
            {
                { "Product Manager", configuration["CIDM_ROLE_PRODUCT_MANAGER"] },
                { "ASAP Repository Editor", configuration["CIDM_ROLE_ASAP_REPOSITORY_EDITOR"] },
                { "ASAP ReadOnly", configuration["CIDM_ROLE_ASAP_READ_ONLY"] }
            };

                foreach (var roleEntry in rolesToUpdate)
                {
                    var role = objectSpace.FirstOrDefault<CidmRole>(r => r.Name == roleEntry.Key);
                    if (role != null && role.CidmGroup != roleEntry.Value)
                    {
                        role.CidmGroup = roleEntry.Value;
                    }
                }

                if (objectSpace.IsModified)
                {
                    objectSpace.CommitChanges();
                }
            }
        }
    }
}
