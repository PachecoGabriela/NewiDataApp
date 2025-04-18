using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Templates;
using DevExpress.ExpressApp.Blazor.Templates.Toolbar.ActionControls;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates.ActionControls;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace iData.Blazor.Server
{
    public class SSOLogin : WindowTemplateBase
    {
        public SSOLogin()
        {

            AdditionalToolbar = new DxToolbarAdapter(new DxToolbarModel());
            AdditionalToolbar.ImageSize = 16;
            AdditionalToolbar.ToolbarModel.CssClass = "logon-bottom-toolbar";
            AdditionalToolbar.AddActionContainer("AdditionalLogonActions");
        }
        protected override IEnumerable<IActionControlContainer> GetActionControlContainers() =>
            AdditionalToolbar.ActionContainers;
        protected override RenderFragment CreateComponent() => SSOLoginComponent.Create(this);
        //public DxToolbarAdapter Toolbar { get; }
        public DxToolbarAdapter AdditionalToolbar { get; }
        public string HeaderCaption { get; set; }
        protected override void BeginUpdate()
        {
            base.BeginUpdate();
            //((ISupportUpdate)Toolbar).BeginUpdate();
            ((ISupportUpdate)AdditionalToolbar).BeginUpdate();
        }
        protected override void EndUpdate()
        {
            ((ISupportUpdate)AdditionalToolbar).EndUpdate();
            //((ISupportUpdate)Toolbar).EndUpdate();
            base.EndUpdate();
        }
    }
}
