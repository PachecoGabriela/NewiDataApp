using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using Roche.Common;

namespace iData.Dp.CompGov.Entities
{

    [DefaultClassOptions]
    public class UseCase : BaseBO
    {
        private string _title;
        private string _description;
        private string _id;
        private bool _isActive;

        public UseCase(Session session) : base(session) { }

        public string Title
        {
            get => _title;
            set => SetPropertyValue(nameof(Title), ref _title, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetPropertyValue(nameof(IsActive), ref _isActive, value);
        }

        [Size(SizeAttribute.Unlimited)]
        [EditorAlias("HtmlPropertyEditor")]
        public string Description
        {
            get => _description;
            set => SetPropertyValue(nameof(Description), ref _description, value);
        }

        public string ID
        {
            get => _id;
            set => SetPropertyValue(nameof(ID), ref _id, value);
        }

        [Association, Aggregated]
        [DisplayName("Applicable Devices")]
        public XPCollection<DeviceDataAccessGrant> DeviceDataAccessGrants => GetCollection<DeviceDataAccessGrant>("DeviceDataAccessGrants");

        [Association, Aggregated]
        public XPCollection<DatasourceAccessGrant> DatasourceAccessGrants => GetCollection<DatasourceAccessGrant>("DatasourceAccessGrants");

        public override string DisplayName => Title;


        [Association, Aggregated]
        [DisplayName("iData Endpoints")]
        public XPCollection<UseCaseSpecificDataProductEndpoint> UseCaseEndpoints
        {
            get { return GetCollection<UseCaseSpecificDataProductEndpoint>(nameof(UseCaseEndpoints)); }
        }
    }
}
