using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Roche.Common.Workflow
{
    [NonPersistent]  
    public abstract class BaseWorkflowBO : BaseBO
    {
        EWorkflowStatus workflowStatus;

        string workflowComment;

        protected BaseWorkflowBO(Session session) : base(session) { }

        [NonCloneable]
        public EWorkflowStatus WorkflowStatus
        {
            get { return workflowStatus; }
            set { SetPropertyValue(nameof(WorkflowStatus), ref workflowStatus, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        [NonPersistent]
        [Browsable(false)]
        public string WorkflowComment
        {
            get { return workflowComment; }
            set { SetPropertyValue(nameof(WorkflowComment), ref workflowComment, value); }
        }

        protected override void OnSaving()
        {
            if (Session.IsNewObject(this))
            {
                WorkflowComment = "Initial Creation";
            }
                     
            base.OnSaving();
        }

        public virtual void ThrowIfCannotBeObsoleted() { }
    }
}