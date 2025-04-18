using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;

namespace iData.Dp.CompGov.Entities
{
    public class DatasourceAccessGrant : BaseObject
    {
        private string _title;
        private string globalId;
        private UseCase _useCase;

        public DatasourceAccessGrant(Session session) : base(session) { }

        public string Title
        {
            get => _title;
            set => SetPropertyValue(nameof(Title), ref _title, value);
        }

        public string GlobalId
        {
            get => globalId;
            set => SetPropertyValue(nameof(GlobalId), ref globalId, value);
        }

        [Association, VisibleInDetailView(false)]
        public UseCase UseCase
        {
            get => _useCase;
            set => SetPropertyValue(nameof(UseCase), ref _useCase, value);
        }
    }
}
