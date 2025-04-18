using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Roche.Common;

namespace iData.Dp.CompGov.Entities
{
    [System.ComponentModel.DisplayName("Intake Requester Department")]
    public class IntakeRequesterDepartment : BaseBO
    {
        public IntakeRequesterDepartment(Session session) : base(session) { }

        private string abbreviation;
        [Size(100)]
        public string Abbreviation
        {
            get => abbreviation;
            set => SetPropertyValue(nameof(Abbreviation), ref abbreviation, value);
        }

        private string fullName;
        [Size(200)]
        public string FullName
        {
            get => fullName;
            set => SetPropertyValue(nameof(FullName), ref fullName, value);
        }

        [VisibleInDetailView(false)]
        [Association("IntakeRequesterDepartment-IntakeRequestCandidates")]
        public XPCollection<IntakeRequestUsecaseCandidate> IntakeRequestCandidates
        {
            get { return GetCollection<IntakeRequestUsecaseCandidate>(nameof(IntakeRequestCandidates)); }
        }

        public override string DisplayName => Abbreviation;
    }
}
