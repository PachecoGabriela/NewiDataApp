using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using iData.Dp.AsapRecords.BusinessObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iData.Dp.AsapRecords.Controllers
{
    
    public partial class AsapEntryViewController : ViewController
    {
        
        public AsapEntryViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(AsapEntryRecord);
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();

            DisableEnableCheckBoxes();
            View.ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
        }

        private async void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            AsapEntryRecord Entry = (AsapEntryRecord)this.View.CurrentObject;
            if (Entry != null)
            {
                if (Entry.VersionPublished)
                {
                    var myvar = ObjectSpace.ServiceProvider.GetRequiredService<IJSRuntime>();
                    await Task.Delay(100);
                    await myvar.InvokeVoidAsync("DisableCheckBox");
                }
                else
                {
                    var myvar = ObjectSpace.ServiceProvider.GetRequiredService<IJSRuntime>();
                    await Task.Delay(100);
                    await myvar.InvokeVoidAsync("EnableCheckBox");
                }
            }
        }

        private async void DisableEnableCheckBoxes()
        {
            AsapEntryRecord Entry = (AsapEntryRecord)this.View.CurrentObject;
            if (Entry != null)
            {
                if (Entry.VersionPublished)
                {
                    var myvar = ObjectSpace.ServiceProvider.GetRequiredService<IJSRuntime>();
                    await Task.Delay(100);
                    await myvar.InvokeVoidAsync("DisableCheckBox");
                }
                else
                {
                    var myvar = ObjectSpace.ServiceProvider.GetRequiredService<IJSRuntime>();
                    await Task.Delay(100);
                    await myvar.InvokeVoidAsync("EnableCheckBox");
                }
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
