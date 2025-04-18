using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Dashboards.Blazor;
using DevExpress.ExpressApp.ApplicationBuilder;
using iData.Module;
using iData.AsapsPipeline.Entities;

namespace iData.Dp.ActivatedAsaps;

public sealed class ActivatedAsapsModule : ModuleBase
{
    public ActivatedAsapsModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Blazor.SystemModule.SystemBlazorModule));
        RequiredModuleTypes.Add(typeof(DashboardsBlazorModule));

    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB, Application.GetConfig());
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application)
    {
        base.Setup(application);
    }
    public override void CustomizeTypesInfo(ITypesInfo typesInfo)
    {
        base.CustomizeTypesInfo(typesInfo);
        CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
    }

    public static void ConfigureWebApi(IWebApiApplicationBuilder builder)
    {
        builder.Modules.Add<ActivatedAsapsModule>();

        builder.ConfigureOptions(options =>
        {
            options.BusinessObject<DpActivatedAsap>()
                .ConfigureController(b => b.ReadOnly());
        });
    }
}
