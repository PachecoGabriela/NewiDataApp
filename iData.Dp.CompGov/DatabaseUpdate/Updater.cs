using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using Microsoft.Extensions.DependencyInjection;
using Roche.Common.Security;
using DevExpress.ExpressApp.Security;
using DevExpress.Map.Native;
using iData.Dp.CompGov.Entities;

namespace iData.Dp.CompGov.DatabaseUpdate;

public class Updater : ModuleUpdater
{
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion)
    {
    }
    public override void UpdateDatabaseAfterUpdateSchema()
    {
        base.UpdateDatabaseAfterUpdateSchema();

        var redShiftConfig = ObjectSpace.FirstOrDefault<RedshiftConfig>(r => r.Id == 1);
        
        if (redShiftConfig == null)
        {
            redShiftConfig = ObjectSpace.CreateObject<RedshiftConfig>();
            redShiftConfig.Id = 1;
            redShiftConfig.Schema = "idata";
            redShiftConfig.ExternalSchema = "idata_ext";
            redShiftConfig.DatabaseName = "dev";
            redShiftConfig.Region = "eu-central-1";
            redShiftConfig.WorkGroupName = "idata-wg-dev-rcn-eu-central-1";
            redShiftConfig.SecretArn = "redshift!idata-ns-dev-rcn-admin";
            ObjectSpace.CommitChanges();
        }


    }
    public override void UpdateDatabaseBeforeUpdateSchema()
    {
        base.UpdateDatabaseBeforeUpdateSchema();
    }
}
