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
using DevExpress.Xpo;
using FuzzySharp;
using iData.Dp.AsapRecords.BusinessObjects;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iData.Dp.AsapRecords.Controllers
{
    public static class GlobalContext
    {
        public static RecordsNotMatchedToAsapRepositoryRecords SelectedRecord { get; set; }
    }

    public partial class UnmatchedRecordsController : ViewController
    {
        private PopupWindowShowAction showPopupAction;
        RecordsNotMatchedToAsapRepositoryRecords CurrentObject;
        IObjectSpace os;
        public UnmatchedRecordsController()
        {
            InitializeComponent();
            TargetObjectType = typeof(RecordsNotMatchedToAsapRepositoryRecords);

            showPopupAction = new PopupWindowShowAction(this, "OpenMappingTerm", PredefinedCategory.ListView);
            showPopupAction.ToolTip = "Add this term to an existing ASAP Entry or create a new one";
            showPopupAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            showPopupAction.ImageName = "OpenTerm";
            showPopupAction.CustomizePopupWindowParams += ShowListViewAction_CustomizePopupWindowParams;
            showPopupAction.Executed += PopupAction_Executed;
        }

        private void PopupAction_Executed(object sender, ActionBaseEventArgs e)
        {
            var popupWindowView = e as PopupWindowShowActionExecuteEventArgs;

            if (popupWindowView?.PopupWindowView.SelectedObjects.Count == 1)
            {
                var selectedObject = (AsapEntryRecord)popupWindowView.PopupWindowView.SelectedObjects[0];
                var objectSpace = Application.CreateObjectSpace();
                var currentObject = objectSpace.GetObject(selectedObject);


                var ExistingMappingTerm = objectSpace.GetObjects<AsapMappingTerm>()
                    .FirstOrDefault(v => v.AsapEntry is not null && v.Term.ToLower() == GlobalContext.SelectedRecord.AsapTechnicalName.ToLower() &&
                                         v.AsapEntry.Oid == currentObject.Oid);

                if (ExistingMappingTerm is null)
                {
                    var newMappingTerm = objectSpace.CreateObject<AsapMappingTerm>();
                    newMappingTerm.Term = GlobalContext.SelectedRecord.AsapTechnicalName;
                    newMappingTerm.IsPrimary = false;

                    currentObject.MappingTerms.Add(newMappingTerm);

                    CriteriaOperator criteriaVersion = CriteriaOperator.And(new BinaryOperator("Version", GlobalContext.SelectedRecord.AsapVersion), new BinaryOperator("AsapEntry", currentObject.Oid));

                    var ExistingVersion = objectSpace.FindObject<AsapVersion>(criteriaVersion);

                    if (ExistingVersion is null)
                    {
                        var newVersion = objectSpace.CreateObject<AsapVersion>();
                        newVersion.Version = GlobalContext.SelectedRecord.AsapVersion;

                        currentObject.Versions.Add(newVersion);
                    }

                }
                else
                {
                    CriteriaOperator criteriaVersion = CriteriaOperator.And(new BinaryOperator("Version", GlobalContext.SelectedRecord.AsapVersion), new BinaryOperator("AsapEntry", currentObject.Oid));

                    var ExistingVersion = objectSpace.FindObject<AsapVersion>(criteriaVersion);

                    if (ExistingVersion is null)
                    {
                        var newVersion = objectSpace.CreateObject<AsapVersion>();
                        newVersion.Version = GlobalContext.SelectedRecord.AsapVersion;

                        currentObject.Versions.Add(newVersion);
                    }
                }

                var detailView = Application.CreateDetailView(objectSpace, currentObject, true);

                var showViewParameters = new ShowViewParameters(detailView)
                {
                    Context = TemplateContext.View,
                    TargetWindow = TargetWindow.Current
                };

                var sourceFrame = this.Frame;

                Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(sourceFrame, showPopupAction));
            }
        }

        private void ShowListViewAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            CurrentObject = View.CurrentObject as RecordsNotMatchedToAsapRepositoryRecords;
            if (CurrentObject is not null)
            {
                GlobalContext.SelectedRecord = CurrentObject;
                List<AsapEntryRecord> RelatedEntries = LookSimilarRecords(CurrentObject.AsapTechnicalName);

                var newObjectSpace = Application.CreateObjectSpace(typeof(AsapEntryRecord));

                List<Guid> oids = RelatedEntries.Select(obj => obj.Oid).ToList();
                var collectionSource = new CollectionSource(newObjectSpace, typeof(AsapEntryRecord));
                collectionSource.Criteria["FilterEntry"] = new InOperator("Oid", oids);


                var newObjects = newObjectSpace.GetObjectsQuery<AsapEntryRecord>()
                                .Where(obj => oids.Contains(obj.Oid))
                                .ToList();

                foreach (var originalObj in RelatedEntries)
                {
                    var newObj = newObjects.FirstOrDefault(o => o.Oid == originalObj.Oid);
                    if (newObj != null)
                    {
                        newObj.Similarity = originalObj.Similarity;
                    }
                }
                collectionSource.Sorting.Add(new SortProperty("Similarity", DevExpress.Xpo.DB.SortingDirection.Descending));
                var listView = Application.CreateListView("AsapEntryRecord_ListView_SimilarRecords", collectionSource, false);
                e.View = listView;
            }


        }

        private List<AsapEntryRecord> LookSimilarRecords(string asapTechnicalName)
        {
            List<AsapMappingTerm> Terms = os.GetObjects<AsapMappingTerm>().ToList();
            Terms.RemoveAll(t => string.IsNullOrEmpty(t.Term));

            List<AsapEntryRecord> Records = new List<AsapEntryRecord>();

            foreach (var term in Terms)
            {
                int similarity = Fuzz.Ratio(term.Term, asapTechnicalName);

                if (similarity >= 10)
                {
                    if (term.AsapEntry != null)
                    {
                        term.AsapEntry.Similarity = similarity;
                        Records.Add(term.AsapEntry);
                    }
                }
            }

            Records = Records.Distinct().ToList();

            return Records;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            os = Application.CreateObjectSpace();
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
