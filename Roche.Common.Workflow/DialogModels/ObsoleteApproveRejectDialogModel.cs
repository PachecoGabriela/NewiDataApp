using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Roche.Common.Workflow.DialogModels
{
    [NonPersistent]
    [System.ComponentModel.DisplayName("Approve/Reject Revoke Request(s)")]
    public class ObsoleteApproveRejectDialogModel : BaseObject
    {
        string comment;
        
        [VisibleInDetailView(false)]
        public bool IsApproved { get; set; }

        public ObsoleteApproveRejectDialogModel(Session session) : base(session) { }

        [Size(SizeAttribute.Unlimited)]
        [DisplayName("Comment")]
        public string Comment
        {
            get { return comment; }
            set { SetPropertyValue(nameof(Comment), ref comment, value); }
        }
    }
}
