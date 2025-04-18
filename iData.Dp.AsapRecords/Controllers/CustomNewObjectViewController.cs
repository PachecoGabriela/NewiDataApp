using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using iData.Dp.AsapRecords.BusinessObjects;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iData.Dp.AsapRecords.Controllers
{
    
    public partial class CustomNewObjectViewController : BlazorNewObjectViewController
    {
        private object _selectedObject;
        public CustomNewObjectViewController()
        {
            InitializeComponent();
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View is ListView listView && View.Id == "AsapEntryRecord_ListView_SimilarRecords")
                NewObjectAction.Execute += NewObjectAction_Execute;
        }

        private void NewObjectAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            var selectedRecord = GlobalContext.SelectedRecord;

            if (selectedRecord != null)
            {
                var newObjectSpace = Application.CreateObjectSpace();
                var newRegistro = (AsapEntryRecord)newObjectSpace.CreateObject(typeof(AsapEntryRecord));
                newRegistro.AsapName = selectedRecord.AsapTechnicalName;
                if (selectedRecord.DeviceReferenceSystemType == "cobas 5800")
                    newRegistro.SystemType = "Cobas 5800";
                else
                    newRegistro.SystemType = "Cobas 6800;Cobas 8800";

                var newVersion = newObjectSpace.CreateObject<AsapVersion>();
                newVersion.Version = GlobalContext.SelectedRecord.AsapVersion;

                newRegistro.Versions.Add(newVersion);

                DetailView dv = Application.CreateDetailView(newObjectSpace, "AsapEntryRecord_DetailView", true);
                dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit; 
                dv.CurrentObject = newRegistro;


                ShowViewParameters showViewParameters = new ShowViewParameters();
                showViewParameters.CreatedView = dv;
                showViewParameters.TargetWindow = TargetWindow.NewWindow;

                Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));

                View.Close(false);
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
