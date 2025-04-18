using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.BaseImpl;

namespace Roche.Common.AuditTrail
{
    public class CustomAuditDataStore : AuditDataStore<AuditDataItemPersistentUTC, AuditedObjectWeakReference>
    {
        protected override string GetDefaultStringRepresentation(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is string valStr)
            {
                return valStr;
            }
            if (value is BaseBO bo)
            {
                return "Record ID : [" + bo.RecordIdentifier + "] | Current Display Name: [" + bo.DisplayName + "]";
            }
            
            return base.GetDefaultStringRepresentation(value);
        }
    }
}
