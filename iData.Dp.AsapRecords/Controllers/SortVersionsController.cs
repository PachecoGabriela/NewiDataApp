using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
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
    
    public partial class SortVersionsController : ViewController<DetailView>
    {
        public SortVersionsController()
        {
            InitializeComponent();
            TargetObjectType = typeof(AsapEntryRecord);
            
        }
        protected override void OnActivated()
        {
            base.OnActivated();


            if (View is DetailView detailView)
            {
                var CurrentAsapEntry = detailView.CurrentObject as AsapEntryRecord;
                SortVersions(CurrentAsapEntry);
            }
        }

        private void SortVersions(AsapEntryRecord currentAsapEntry)
        {

            if (currentAsapEntry != null)
            {
                if (currentAsapEntry.Versions.Count() > 1)
                {
                    
                    if (View.ObjectSpace is XPObjectSpace xpObjectSpace)
                    {
                        
                        var objetosOrdenados = currentAsapEntry.Versions
                            .OfType<AsapVersion>() 
                            .Select(o => new {
                                VersionObject = o,
                                Version = o.VersionTmp
                            })
                            .OrderBy(x => x.Version) 
                            .Select(x => x.VersionObject) 
                            .ToList();

                        

                        foreach (var obj in currentAsapEntry.Versions.ToList())
                        {
                            currentAsapEntry.Versions.Remove(obj);
                        }


                        foreach (var obj in objetosOrdenados)
                        {
                            currentAsapEntry.Versions.Add(obj); 
                        }

                       
                        View.ObjectSpace.CommitChanges();
                    }
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
