using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Roche.Common.Workflow.DialogModels;

namespace Roche.Common.Workflow.Controllers
{
    public class ObsoleteApproveRejectDialogController : ObjectViewController<DetailView, ObsoleteApproveRejectDialogModel>
    {
        public SimpleAction ApproveObsoleteRequestAction { get; set; }
        public SimpleAction RejectObosoleteRequestAction { get; set; }

        public ObsoleteApproveRejectDialogController()
        {
            ApproveObsoleteRequestAction = new SimpleAction(this, "approveObsoleteRequestAction", PredefinedCategory.PopupActions)
            {
                Caption = "Approve"
            };
            RejectObosoleteRequestAction = new SimpleAction(this, "rejectObosoleteRequestAction", PredefinedCategory.PopupActions)
            {
                Caption = "Reject"
            };

            ApproveObsoleteRequestAction.Execute += ApproveObsoleteRequestAction_Execute;
            RejectObosoleteRequestAction.Execute += RejectObosoleteRequestAction_Execute;
        }

        private void RejectObosoleteRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ViewCurrentObject.IsApproved = false;
            Frame.GetController<DialogController>().CloseAction.DoExecute();
        }
        private void ApproveObsoleteRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ViewCurrentObject.IsApproved = true;
            Frame.GetController<DialogController>().CloseAction.DoExecute();
        }
    }
}
