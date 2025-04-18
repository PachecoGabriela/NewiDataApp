using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.AuditTrail;
using iData.Blazor.Server.Services;
using Roche.Common.AuditTrail;
using System.Runtime.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace iData.Blazor.Server;

public class iDataBlazorApplication : BlazorApplication
{

    private const string dateTimeFormat = "dd-MMM-yyyy HH:mm:ss";
    public iDataBlazorApplication()
    {
        ApplicationName = "iData";
        CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
        DatabaseVersionMismatch += iDataBlazorApplication_DatabaseVersionMismatch;
        MaxLogonAttemptCount = Int32.MaxValue;
    }

    protected override IFrameTemplate CreateDefaultTemplate(TemplateContext context)
    {
        if (context == TemplateContext.LogonWindow)
        {
            return new SSOLogin() { };
        }
        return base.CreateDefaultTemplate(context);
    }



    protected override void OnSetupStarted()
    {
        DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;

        IAuditTrailService auditService = AuditTrailService.GetService(ServiceProvider);
        auditService.SaveAuditTrailData += Instance_SaveAuditTrailData;
        auditService.CustomizeAuditTrailSettings += Instance_CustomizeAuditTrailSettings;

        base.OnSetupStarted();
    }

    protected override void OnSetupComplete()
    {
        base.OnSetupComplete();

        var configuration = this.ServiceProvider.GetService<IConfiguration>();
        RoleConfigurationUpdater.CheckAndUpdateRoleGroupNames(this.ObjectSpaceProvider, configuration);

        foreach (IModelClass modelClass in Model.BOModel)
        {
            foreach (IModelMember member in modelClass.OwnMembers)
            {
                if (member.MemberInfo.MemberType == typeof(DateTime) || member.MemberInfo.MemberType == typeof(DateTime?))
                {
                    member.DisplayFormat = dateTimeFormat;
                    member.EditMask = dateTimeFormat;
                }
            }
        }
    }

    private void Instance_CustomizeAuditTrailSettings(object sender, CustomizeAuditTrailSettingsEventArgs e)
    {
        foreach (var i in e.AuditTrailSettings.TypesToAudit.ToList())
        {
            if (i.ClassInfo.HasAttribute(typeof(AuditTrailDisableAttribute)))
                e.AuditTrailSettings.RemoveType(i.ClassInfo.ClassType);
        }
    }

    private void Instance_SaveAuditTrailData(object sender, SaveAuditTrailDataEventArgs e)
    {
        for (int i = e.AuditTrailDataItems.Count - 1; i >= 0; i--)
        {
            var auditTrailItem = e.AuditTrailDataItems[i];

            if (auditTrailItem.MemberInfo != null && auditTrailItem.MemberInfo.HasAttribute(typeof(AuditTrailDisableAttribute)))
            {
                e.AuditTrailDataItems.RemoveAt(i);
            }
            if (auditTrailItem.NewValue is DateTime dt)
            {
                auditTrailItem.NewValue = dt.ToString(dateTimeFormat);
            }
            if (auditTrailItem.OldValue is DateTime dt2)
            {
                auditTrailItem.OldValue = dt2.ToString(dateTimeFormat);
            }
        }
    }

    private void iDataBlazorApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e)
    {
        e.Updater.Update();
        e.Handled = true;
    }
}
