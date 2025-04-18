using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Roche.Common.AuditTrail
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class AuditDataItemPersistentUTC : AuditDataItemPersistent
    {
        public AuditDataItemPersistentUTC(Session session) : base(session) { }

        [NonPersistent]
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string Record
        {
            get
            {
                if (AuditedObject.Target is BaseBO)
                {
                    var bo = AuditedObject.Target as BaseBO;
                    return "Record ID : [" + bo.RecordIdentifier + "] | Current Display Name: [" + bo.DisplayName + "]";

                }
                return AuditedObject.DisplayName;
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            ModifiedOn = ModifiedOn.ToUniversalTime();
            //truncate milliseconds
            ModifiedOn = ModifiedOn.AddTicks(-ModifiedOn.Ticks % TimeSpan.TicksPerSecond);

            if ((PropertyName ?? "").StartsWith("Workflow"))
            {
                OperationType = "Workflow";
            }
        }
    }
}
