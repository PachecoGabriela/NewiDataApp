using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Office.Blazor;
using DevExpress.ExpressApp.Office;
using DevExpress.ExpressApp.CloneObject;
using iData.Dp.CompGov.Entities;

namespace iData.Dp.CompGov;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class CompGovModule : ModuleBase
{
    public CompGovModule()
    {
        // 
        // CompGovModule
        // 
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(CloneObjectModule));
        RequiredModuleTypes.Add(typeof(OfficeModule));
        RequiredModuleTypes.Add(typeof(OfficeBlazorModule));
        RequiredModuleTypes.Add(typeof(Roche.Common.CommonModule));
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application)
    {
        base.Setup(application);

        application.ObjectSpaceCreated += (obj, ea) =>
        {
            if (ea.ObjectSpace is NonPersistentObjectSpace nonPersistentObjectSpace)
            {
                nonPersistentObjectSpace.ObjectByKeyGetting += nonPersistentObjectSpace_ObjectByKeyGetting;
            }
        };
    }
    private void nonPersistentObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e)
    {
        IObjectSpace objectSpace = (IObjectSpace)sender;
        if (e.ObjectType.IsAssignableFrom(typeof(ConnectedDevicesDp)))
        {
            //e.Object = objectSpace.CreateObject<ConnectedDevicesDp>();
            e.Object = new ConnectedDevicesDp();
        }
    }


    public override void CustomizeTypesInfo(ITypesInfo typesInfo)
    {
        base.CustomizeTypesInfo(typesInfo);
        CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
    }
}
