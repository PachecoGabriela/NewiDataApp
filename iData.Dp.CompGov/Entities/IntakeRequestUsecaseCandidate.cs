using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Roche.Common;

namespace iData.Dp.CompGov.Entities
{
    [System.ComponentModel.DisplayName("Intake Request Candidate")]

    public class IntakeRequestUsecaseCandidate : BaseBO
    {
        public IntakeRequestUsecaseCandidate(Session session) : base(session) { }

        private string title;
        [RuleRequiredField("IntakeRequestCandidateTitleRequired", DefaultContexts.Save)]
        [ToolTip("Enter the title of the use case.")]
        [Size(100)]
        public string Title
        {
            get => title;
            set => SetPropertyValue(nameof(Title), ref title, value);
        }

        private string summary;
        [ToolTip("Brief smmary of the use case, including key objectives.")]
        [Size(SizeAttribute.Unlimited)]
        public string Summary
        {
            get => summary;
            set => SetPropertyValue(nameof(Summary), ref summary, value);
        }

        private string description;
        [ToolTip("Detail the use case, outlining the process, stakeholders involved, and expected outcomes.")]
        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get => description;
            set => SetPropertyValue(nameof(Description), ref description, value);
        }

        private string businessValue;
        [ToolTip("Describe the business value or ROI of this use case; include quantifiable metrics if possible.")]
        [Size(SizeAttribute.Unlimited)]
        public string BusinessValue
        {
            get => businessValue;
            set => SetPropertyValue(nameof(BusinessValue), ref businessValue, value);
        }

        private string mainContact;
        [ToolTip("Name the primary contact person for this use case, including their role and eMail.")]
        [Size(500)]
        public string MainContact
        {
            get => mainContact;
            set => SetPropertyValue(nameof(MainContact), ref mainContact, value);
        }

        private string otherContacts;
        [ToolTip("List other key contacts related to this use case, including their role and eMail")]
        [Size(1000)]
        public string OtherContacts
        {
            get => otherContacts;
            set => SetPropertyValue(nameof(OtherContacts), ref otherContacts, value);
        }

        private string sponsor;
        [ToolTip("Name the business sponsor for this use case, including their role and eMail.")]
        [Size(500)]
        public string Sponsor
        {
            get => sponsor;
            set => SetPropertyValue(nameof(Sponsor), ref sponsor, value);
        }

        private string region;
        [ToolTip("Specify the geographic or organizational region from which this use case seeks data.")]
        [Size(500)]
        public string Region
        {
            get => region;
            set => SetPropertyValue(nameof(Region), ref region, value);
        }

        private string instrumentLines;
        [ToolTip("Indicate the instrument lines or technologies from which data is sought for this use case.")]
        [Size(500)]
        public string InstrumentLines
        {
            get => instrumentLines;
            set => SetPropertyValue(nameof(InstrumentLines), ref instrumentLines, value);
        }

        private string dataProducts; 
        [ToolTip("List the data products this use case is interested in. If specific products aren't known or don't exist yet, provide a description of the data needed.")]
        [Size(500)]
        public string DataProducts
        {
            get => dataProducts;
            set => SetPropertyValue(nameof(DataProducts), ref dataProducts, value);
        }

        private string additionalMaterialLinks;
        [ToolTip("Provide URLs to any documents, presentations, or other materials related to the use case.")]
        [Size(SizeAttribute.Unlimited)]
        public string AdditionalMaterialLinks
        {
            get => additionalMaterialLinks;
            set => SetPropertyValue(nameof(AdditionalMaterialLinks), ref additionalMaterialLinks, value);
        }

        private string urgencyLevel;
        [ToolTip("Provide a rough estimate of the urgency or preferred timeline for data access for this use case. Describe whether the need is immediate, within a few months, or longer-term.")]
        [Size(500)]
        public string UrgencyLevel
        {
            get => urgencyLevel;
            set => SetPropertyValue(nameof(UrgencyLevel), ref urgencyLevel, value);
        }

        private string endUsers;
        [ToolTip("Identify the final users who will directly benefit from or interact with the use case outputs.")]
        [Size(500)]
        public string EndUsers
        {
            get => endUsers;
            set => SetPropertyValue(nameof(EndUsers), ref endUsers, value);
        }

        private string dataUsers;
        [ToolTip("List individuals or groups who will handle the data aspects of this use case, such as data scientists, IT teams, business intelligence team.")]
        [Size(500)]
        public string DataUsers
        {
            get => dataUsers;
            set => SetPropertyValue(nameof(DataUsers), ref dataUsers, value);
        }

        private string notes;
        [ToolTip("Add any additional notes or comments about this intake request that may be helpful.")]
        [Size(SizeAttribute.Unlimited)]
        public string Notes
        {
            get => notes;
            set => SetPropertyValue(nameof(Notes), ref notes, value);
        }

        private IntakeRequesterDepartment requesterDepartment;
        [Association("IntakeRequesterDepartment-IntakeRequestCandidates")]
        [RuleRequiredField("IntakeRequestCandidateRequesterDepartmentRequired", DefaultContexts.Save,
            CustomMessageTemplate = "Selecting a department is mandatory, as edit rights are managed based on department.")]
        [ToolTip("Select the department making the request. It is crucial to choose correctly because editing rights are managed by department.")]
        public IntakeRequesterDepartment RequesterDepartment
        {
            get => requesterDepartment;
            set => SetPropertyValue(nameof(RequesterDepartment), ref requesterDepartment, value);
        }

        public override string DisplayName => Title;
    }
}
