using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Roche.Common.Workflow;

namespace iData.Dp.CompGov.Entities
{
    public class DataElement : BaseWorkflowBO
    {
        private string _riraDocumentId;
        private string _riraLink;
        private string _attributeName;
        private string _description;
        private string _type;
        private string _example;

        public DataElement(Session session) : base(session) { }

        [Size(200)]
        [RuleRequiredField("AttributeNameRequired", DefaultContexts.Save)]
        [DisplayName("Attribute Name")]
        public string AttributeName
        {
            get => _attributeName;
            set => SetPropertyValue(nameof(AttributeName), ref _attributeName, value);
        }

        [Size(SizeAttribute.Unlimited)]
        [DisplayName("Description")]
        public string Description
        {
            get => _description;
            set => SetPropertyValue(nameof(Description), ref _description, value);
        }

        [Size(100)]
        [DisplayName("Type")]
        public string Type
        {
            get => _type;
            set => SetPropertyValue(nameof(Type), ref _type, value);
        }

        [Size(500)]
        [DisplayName("Example")]
        public string Example
        {
            get => _example;
            set => SetPropertyValue(nameof(Example), ref _example, value);
        }

        [Size(200)]
        [RuleRequiredField("RiraDocumentIdRequired", DefaultContexts.Save)]
        [DisplayName("RIRA Document ID")]
        public string RiraDocumentId
        {
            get => _riraDocumentId;
            set => SetPropertyValue(nameof(RiraDocumentId), ref _riraDocumentId, value);
        }

        [Size(500)]
        [DisplayName("RIRA Link")]
        public string RiraLink
        {
            get => _riraLink;
            set => SetPropertyValue(nameof(RiraLink), ref _riraLink, value);
        }

        [Association]
        [DisplayName("Used in Data Products")]
        public XPCollection<iDataDataProduct> iDataDataProducts
        {
            get { return GetCollection<iDataDataProduct>(nameof(iDataDataProducts)); }
        }
        public override string DisplayName => AttributeName;
    }

}
