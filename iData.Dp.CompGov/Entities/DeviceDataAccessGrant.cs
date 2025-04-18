using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;

namespace iData.Dp.CompGov.Entities
{
    public class DeviceDataAccessGrant : BaseObject
    {
        private string _serial;
        private string _systemReferenceType;
        private UseCase _useCase;
        private string _deviceMaterialNumber;
        private string _systemType;

        public DeviceDataAccessGrant(Session session) : base(session) { }

        public string Serial
        {
            get => _serial;
            set => SetPropertyValue(nameof(Serial), ref _serial, value);
        }

        public string SystemReferenceType
        {
            get => _systemReferenceType;
            set => SetPropertyValue(nameof(SystemReferenceType), ref _systemReferenceType, value);
        }
        public string DeviceMaterialNumber
        {
            get => _deviceMaterialNumber;
            set => SetPropertyValue(nameof(DeviceMaterialNumber), ref _deviceMaterialNumber, value);
        }
        public string SystemType
        {
            get => _systemType;
            set => SetPropertyValue(nameof(SystemType), ref _systemType, value);
        }

        [Association, Aggregated]
        [DisplayName("Applicable Periods")]
        public XPCollection<DeviceDataAccessGrantValidityPeriod> ValidityPeriods => GetCollection<DeviceDataAccessGrantValidityPeriod>("ValidityPeriods");

        [Association, VisibleInDetailView(false)]
        public UseCase UseCase
        {
            get => _useCase;
            set => SetPropertyValue(nameof(UseCase), ref _useCase, value);
        }
    }
}
