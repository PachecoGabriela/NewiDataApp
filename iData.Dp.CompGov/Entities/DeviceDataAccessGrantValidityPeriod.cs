using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;

namespace iData.Dp.CompGov.Entities
{
    [System.ComponentModel.DisplayName("Device-data Access Grant Validity Period")]
    public class DeviceDataAccessGrantValidityPeriod : BaseObject
    {
        private DateTime _start;
        private DateTime? _end;
        private DeviceDataAccessGrant _deviceDataAccessGrant;

        public DeviceDataAccessGrantValidityPeriod(Session session) : base(session) { }

        public DateTime Start
        {
            get => _start;
            set => SetPropertyValue(nameof(Start), ref _start, value);
        }

        public DateTime? End
        {
            get => _end;
            set => SetPropertyValue(nameof(End), ref _end, value);
        }

        [Association, VisibleInDetailView(false)]
        public DeviceDataAccessGrant DeviceDataAccessGrant
        {
            get => _deviceDataAccessGrant;
            set => SetPropertyValue(nameof(DeviceDataAccessGrant), ref _deviceDataAccessGrant, value);
        }
    }
}
