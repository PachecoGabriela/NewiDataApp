using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using Roche.Common.Workflow.DialogModels;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.Persistent.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Roche.Common.Workflow.Controllers
{
    public class WorkflowListViewController : ObjectViewController<ListView, BaseWorkflowBO>
    {
        public PopupWindowShowAction PublishAction { get; set; }
        public PopupWindowShowAction RevertToDraftAction { get; set; }

        public WorkflowListViewController()
        {
            PublishAction = new PopupWindowShowAction(this, "PublishAction", "Edit")
            {
                Caption = "Publish",
                TargetObjectsCriteria = "[WorkflowStatus] = ##Enum#Roche.Common.EWorkflowStatus,Draft#",
                TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueForAll,
                TargetObjectType = typeof(BaseWorkflowBO)
            };
            PublishAction.CustomizePopupWindowParams += PublishAction_CustomizePopupWindowParams;
            PublishAction.Execute += PublishAction_Execute;

            RevertToDraftAction = new PopupWindowShowAction(this, "RevertToDraftAction", "Edit")
            {
                Caption = "Revert to Draft",
                TargetObjectsCriteria = "[WorkflowStatus] = ##Enum#Roche.Common.EWorkflowStatus,Published#",
                TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueForAll,
                TargetObjectType = typeof(BaseWorkflowBO)
            };
            RevertToDraftAction.CustomizePopupWindowParams += RevertToDraftAction_CustomizePopupWindowParams;
            RevertToDraftAction.Execute += RevertToDraftAction_Execute;
        }

        private void PublishAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(WorkflowCommentDialogModel));
            var o = os.CreateObject<WorkflowCommentDialogModel>();
            var v = Application.CreateDetailView(os, o);
            v.ViewEditMode = ViewEditMode.Edit;
            v.Caption = "Publish " + View.SelectedObjects.Count + " Record(s)";
            e.View = v;
        }

        private void PublishAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var m = (WorkflowCommentDialogModel)e.PopupWindowViewCurrentObject;

            foreach (var dr in ViewSelectedObjects)
            {
                if (!string.IsNullOrEmpty(m.Comment))
                {
                    dr.WorkflowComment = m.Comment;
                }
                dr.WorkflowStatus = EWorkflowStatus.Published;
            }

            try
            {
                ObjectSpace.CommitChanges();  
                UnselectAllRows();
                View.Refresh(true);
            }
            catch (ValidationException ex)
            {
                ObjectSpace.Rollback();
                View.Refresh(false);

                e.PopupWindow.Close();

                throw ex;
            }


        }

        private void RevertToDraftAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(WorkflowCommentDialogModel));
            var o = os.CreateObject<WorkflowCommentDialogModel>();
            var v = Application.CreateDetailView(os, o);
            v.ViewEditMode = ViewEditMode.Edit;
            v.Caption = "Revert " + View.SelectedObjects.Count + " Record(s) to Draft";
            e.View = v;
        }

        private void RevertToDraftAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var m = (WorkflowCommentDialogModel)e.PopupWindowViewCurrentObject;

            foreach (var dr in ViewSelectedObjects)
            {
                if (!string.IsNullOrEmpty(m.Comment))
                {
                    dr.WorkflowComment = m.Comment;
                }
                dr.WorkflowStatus = EWorkflowStatus.Draft;
            }

            ObjectSpace.CommitChanges();
            UnselectAllRows();
            View.Refresh(true);
        }

        private void UnselectAllRows()
        {
            if (View.Editor is DxGridListEditor v)
            {
                v.UnselectAll();
            }
        }
    }
}