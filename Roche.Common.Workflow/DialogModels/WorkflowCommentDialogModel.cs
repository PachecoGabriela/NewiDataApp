using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Roche.Common.Workflow.DialogModels
{
    [NonPersistent]
    public class WorkflowCommentDialogModel : BaseObject
    {
        string comment;

        public WorkflowCommentDialogModel(Session session) : base(session) { }

        [Size(SizeAttribute.Unlimited)]
        [DisplayName("Comment")]
        public string Comment
        {
            get { return comment; }
            set { SetPropertyValue(nameof(Comment), ref comment, value); }
        }
    }
}
