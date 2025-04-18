using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using iData.Dp.AsapRecords.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iData.Dp.AsapRecords.Controllers
{
    public partial class DisableLinkListViewController : ViewController
    {
        public DisableLinkListViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(AsapEntryRecord);
            
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View is ListView { Editor: DxGridListEditor dxGridListEditor })
            {
                dxGridListEditor.CustomizeViewItemControl<LookupPropertyEditor>(this, editor => editor.ShowLink = false);
                dxGridListEditor.CustomizeViewItemControl<ObjectPropertyEditor>(this, editor => editor.ShowLink = false);
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            if (View is ListView { Editor: DxGridListEditor dxGridListEditor })
            {

                foreach (var editor in dxGridListEditor.PropertyEditors)
                {
                    if (editor is LookupPropertyEditor lookupPropertyEditor)
                    {
                        lookupPropertyEditor.ShowLink = false;
                    }
                    else if (editor is ObjectPropertyEditor objectPropertyEditor)
                    {
                        objectPropertyEditor.ShowLink = false;
                    }
                }
            }

        }
        protected override void OnDeactivated()
        {
            
            base.OnDeactivated();
        }
    }
}
