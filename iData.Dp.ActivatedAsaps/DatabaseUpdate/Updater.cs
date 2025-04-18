using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using Microsoft.Extensions.DependencyInjection;
using iData.AsapsPipeline.Entities;
using System.Reflection;
using Parquet.Serialization;
using DevExpress.ExpressApp.Security;
using Roche.Common.Security;
using Microsoft.Extensions.Configuration;
using CsvHelper;
using System.Globalization;
using System.Text;
using DevExpress.ExpressApp.Dashboards;
using iData.Dp.ActivatedAsaps.Properties;

namespace iData.Dp.ActivatedAsaps.DatabaseUpdate;

public class Updater : ModuleUpdater
{
    private IConfiguration _configuration;
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion)
    {
    }

    public Updater(IObjectSpace objectSpace, Version currentDBVersion, IConfiguration configuration) : this(objectSpace, currentDBVersion)
    {
        _configuration = configuration;
    }

    public override void UpdateDatabaseAfterUpdateSchema()
    {
        base.UpdateDatabaseAfterUpdateSchema();

        SetupRoles();
        InsertDashboards();
    }

    private void InsertDashboards()
    {
        if (ObjectSpace.FirstOrDefault<DashboardData>(r => r.Title == "Respiratory Flex Adoption") == null)
        {
            DashboardsModule.AddDashboardData<DashboardData>(ObjectSpace, "Respiratory Flex Adoption", Resources.RespiratoryFlexAdoption);
            ObjectSpace.CommitChanges();
        }
    }
    public override void UpdateDatabaseBeforeUpdateSchema()
    {
        base.UpdateDatabaseBeforeUpdateSchema();
    }

    private void SetupRoles()
    {
        var dpRole = ObjectSpace.FirstOrDefault<CidmRole>(r => r.Name == "DP Consumer - Activated ASAPs");

        if (dpRole == null)
        {
            dpRole = ObjectSpace.CreateObject<CidmRole>();
        }

        dpRole.Name = "DP Consumer - Activated ASAPs";
        dpRole.IsAdministrative = false;
        dpRole.PermissionPolicy = SecurityPermissionPolicy.DenyAllByDefault;

        dpRole.AddTypePermission<DpActivatedAsap>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
        dpRole.AddTypePermission<DashboardData>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

        dpRole.AddNavigationPermission(@"Application/NavigationItems/Items/DP - Activated ASAPs/Items/Currently Activated ASAPs", SecurityPermissionState.Allow);
        dpRole.AddNavigationPermission(@"Application/NavigationItems/Items/DP - Activated ASAPs/Items/Currently Activated ASAPs", SecurityPermissionState.Allow);
        dpRole.AddNavigationPermission(@"Application/NavigationItems/Items/DP - Activated ASAPs/Items/Respiratory Flex Adoption", SecurityPermissionState.Allow);

        dpRole.CidmGroup = _configuration["CIDM_ROLE_ASAP_DP_CONSUMER"];
        ObjectSpace.CommitChanges();
    }
}
