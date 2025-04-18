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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iData.Dp.AsapRecords.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ModifySaveButtonCaptionController : ViewController
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public ModifySaveButtonCaptionController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (View.Id == "AsapEntryRecord_Versions_ListView")
            {
                var newObjectViewController = Frame.GetController<NewObjectViewController>();
                if (newObjectViewController != null)
                {
                    newObjectViewController.NewObjectAction.ExecuteCompleted += NewObjectAction_ExecuteCompleted;
                }
            }
        }

        private void NewObjectAction_ExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            if (e.ShowViewParameters != null && e.ShowViewParameters.Controllers != null)
            {
                var dialogController = e.ShowViewParameters.Controllers
                    .OfType<DialogController>()
                    .FirstOrDefault();

                if (dialogController != null)
                    dialogController.AcceptAction.Caption = "Ok";
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
