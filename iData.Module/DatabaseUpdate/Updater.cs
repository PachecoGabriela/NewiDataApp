using CsvHelper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Microsoft.Extensions.Configuration;
using Roche.Common.AuditTrail;
using Roche.Common.Security;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace iData.Module.DatabaseUpdate;
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
        FixAuditTrailTable();
        SetupRoles();
    }
    void FixAuditTrailTable()
    {
        // change datatype of OldValue/NewValue fields in audittrail table to "text"

//        var dbconn = (ObjectSpace as XPObjectSpace).Connection;
//        var dbcommand = dbconn.CreateCommand();
//        dbcommand.CommandText = """
//DO
//$do$
//BEGIN
//   IF (SELECT pg_typeof(adip."NewValue")::text FROM "AuditDataItemPersistent" adip limit 1)<>'text' THEN
//	ALTER TABLE "AuditDataItemPersistent" ALTER COLUMN "OldValue" TYPE text USING "OldValue"::text;
//	ALTER TABLE "AuditDataItemPersistent" ALTER COLUMN "NewValue" TYPE text USING "NewValue"::text;      
//   END IF;
//END
//$do$
//""";
//        dbcommand.ExecuteNonQuery();
    }

    private void SetupRoles()
    {
        var adminRole = ObjectSpace.FirstOrDefault<CidmRole>(r => r.Name == "Product Manager");
        if (adminRole == null)
        {
            adminRole = ObjectSpace.CreateObject<CidmRole>();
        }

        adminRole.Name = "Product Manager";
        adminRole.IsAdministrative = true;
        adminRole.PermissionPolicy = SecurityPermissionPolicy.AllowAllByDefault;
        adminRole.CidmGroup = _configuration["CIDM_ROLE_PRODUCT_MANAGER"];

        var asapEditor = ObjectSpace.FirstOrDefault<CidmRole>(r => r.Name == "ASAP Repository Editor");
        if (asapEditor == null)
        {
            asapEditor = ObjectSpace.CreateObject<CidmRole>();
            asapEditor.Name = "ASAP Repository Editor";
            asapEditor.IsAdministrative = false;
            asapEditor.PermissionPolicy = SecurityPermissionPolicy.DenyAllByDefault;
            asapEditor.AddTypePermission<AuditDataItemPersistent>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            asapEditor.AddNavigationPermission(@"Application/NavigationItems/Items/ASAPs/Items/ASAPRepository", SecurityPermissionState.Allow);
        }
        asapEditor.CidmGroup = _configuration["CIDM_ROLE_ASAP_REPOSITORY_EDITOR"];

        var asapReadOnly = ObjectSpace.FirstOrDefault<CidmRole>(r => r.Name == "ASAP ReadOnly");
        if (asapReadOnly == null)
        {
            asapReadOnly = ObjectSpace.CreateObject<CidmRole>();
            asapReadOnly.Name = "ASAP ReadOnly";
            asapReadOnly.IsAdministrative = false;
            asapReadOnly.PermissionPolicy = SecurityPermissionPolicy.DenyAllByDefault;
            asapReadOnly.AddTypePermission<AuditDataItemPersistent>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            asapReadOnly.AddActionPermission("RevertToDraftAction");
            asapReadOnly.AddActionPermission("PublishAction");
            asapReadOnly.AddActionPermission("SaveAndClose");
            asapReadOnly.AddNavigationPermission(@"Application/NavigationItems/Items/ASAPs/Items/ASAPRepository", SecurityPermissionState.Allow);
        }
        asapReadOnly.CidmGroup = _configuration["CIDM_ROLE_ASAP_READ_ONLY"];

        ObjectSpace.CommitChanges();
    }

    public override void UpdateDatabaseBeforeUpdateSchema()
    {
        base.UpdateDatabaseBeforeUpdateSchema();
    }
}
