using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;

namespace Roche.Common.Security
{
    [System.ComponentModel.DisplayName("Role")]
    public class CidmRole : PermissionPolicyRole
    {
        public CidmRole(Session session) : base(session) { }

        private string _cidmGroup;

        [Size(200)]
        [DisplayName("CIDM Group")]
        public string CidmGroup
        {
            get => _cidmGroup;
            set => SetPropertyValue(nameof(CidmGroup), ref _cidmGroup, value);
        }
    }
}
