using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using Roche.Common.Workflow.DialogModels;
using DevExpress.Persistent.Validation;

namespace Roche.Common.Workflow.Controllers
{
    
    public class WorkflowDetailViewController : ObjectViewController<DetailView, BaseWorkflowBO>
    {
        public PopupWindowShowAction PublishAction { get; set; }
        public PopupWindowShowAction RevertToDraftAction { get; set; }

        public WorkflowDetailViewController()
        {
            PublishAction = new PopupWindowShowAction(this, "PublishActionSingle", "Edit")
            {
                Caption = "Publish",
                TargetObjectsCriteria = "[WorkflowStatus] = ##Enum#Roche.Common.EWorkflowStatus,Draft#",
                TargetObjectType = typeof(BaseWorkflowBO)
            };
            PublishAction.CustomizePopupWindowParams += PublishAction_CustomizePopupWindowParams;
            PublishAction.Execute += PublishAction_Execute;

            RevertToDraftAction = new PopupWindowShowAction(this, "RevertToDraftActionSingle", "Edit")
            {
                Caption = "Revert to Draft",
                TargetObjectsCriteria = "[WorkflowStatus] = ##Enum#Roche.Common.EWorkflowStatus,Published#",
                TargetObjectType = typeof(BaseWorkflowBO)
            };
            RevertToDraftAction.CustomizePopupWindowParams += RevertToDraftAction_CustomizePopupWindowParams;
            RevertToDraftAction.Execute += RevertToDraftAction_Execute;

            Activated += (s, e) => this.View.AllowEdit["Workflow"] = ViewCurrentObject.WorkflowStatus == EWorkflowStatus.Draft;
            Deactivated += (s, e) => this.View.AllowEdit.RemoveItem("Workflow");
        }

        private void PublishAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(WorkflowCommentDialogModel));
            var o = os.CreateObject<WorkflowCommentDialogModel>();
            var v = Application.CreateDetailView(os, o);
            v.ViewEditMode = ViewEditMode.Edit;
            v.Caption = "Publish Record";
            e.View = v;
        }

        private void PublishAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var m = (WorkflowCommentDialogModel)e.PopupWindowViewCurrentObject;
            var bo = (BaseWorkflowBO)View.CurrentObject;

            if (!string.IsNullOrEmpty(m.Comment))
            {
                bo.WorkflowComment = m.Comment;
            }
            bo.WorkflowStatus = EWorkflowStatus.Published;

            try
            {
                ObjectSpace.CommitChanges();
                this.View.AllowEdit["Workflow"] = ViewCurrentObject.WorkflowStatus == EWorkflowStatus.Draft;
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
            v.Caption = "Revert Record to Draft";
            e.View = v;
        }

        private void RevertToDraftAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var m = (WorkflowCommentDialogModel)e.PopupWindowViewCurrentObject;
            var bo = (BaseWorkflowBO)View.CurrentObject;

            if (!string.IsNullOrEmpty(m.Comment))
            {
                bo.WorkflowComment = m.Comment;
            }
            bo.WorkflowStatus = EWorkflowStatus.Draft;
            ObjectSpace.CommitChanges();
            this.View.AllowEdit["Workflow"] = ViewCurrentObject.WorkflowStatus == EWorkflowStatus.Draft;
            View.Refresh(true);
        }
    }
}
