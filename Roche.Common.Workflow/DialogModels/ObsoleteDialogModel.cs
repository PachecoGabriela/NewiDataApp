using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Roche.Common.Workflow.DialogModels
{
    [NonPersistent]
    [System.ComponentModel.DisplayName("Revoke Request for Record(s)")]
    public class ObsoleteDialogModel : BaseObject
    {
        string obsoleteRequestRationale;

        public ObsoleteDialogModel(Session session) : base(session) { }

        [Size(SizeAttribute.Unlimited)]
        [DisplayName("Comment")]
        public string ObsoleteRequestRationale
        {
            get { return obsoleteRequestRationale; }
            set { SetPropertyValue(nameof(ObsoleteRequestRationale), ref obsoleteRequestRationale, value); }
        }
    }
}
