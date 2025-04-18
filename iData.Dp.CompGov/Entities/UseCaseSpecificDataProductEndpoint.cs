using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Validation;
using Roche.Common.Security;
using System.ComponentModel;

namespace iData.Dp.CompGov.Entities
{
    public class UseCaseSpecificDataProductEndpoint : BaseObject
    {
        private string endpointName;
        
        private string logOutput;

        private iDataDataProduct dataProduct;
        private UseCase _useCase;

        public UseCaseSpecificDataProductEndpoint(Session session) : base(session) { }


        [RuleRequiredField("UseCaseSpecificDataProductEndpoint_DataProduct_Required", DefaultContexts.Save)]
        [Association]
        public iDataDataProduct DataProduct
        {
            get => dataProduct;
            set => SetPropertyValue(nameof(DataProduct), ref dataProduct, value);
        }

        [RuleRequiredField("UseCaseSpecificDataProductEndpoint_UseCase_Required", DefaultContexts.Save)]
        [Association]
        public UseCase UseCase
        {
            get => _useCase;
            set => SetPropertyValue(nameof(UseCase), ref _useCase, value);
        }

        [Appearance("EndpointName", Enabled = false)]
        [RuleUniqueValue("UseCaseSpecificDataProductEndpoint_EndpointName_Unique", DefaultContexts.Save)]
        public string EndpointName
        {
            get => endpointName;
            set => SetPropertyValue(nameof(EndpointName), ref endpointName, value);
        }


        private string _CIDMGroup;
        [RuleUniqueValue("UseCaseSpecificDataProductEndpoint_CIDMGroup_Unique", DefaultContexts.Save, CustomMessageTemplate = "The CIDMGroup value must be unique.")]
        [RuleRequiredField("UseCaseSpecificDataProductEndpoint_CIDMGroup_Required", DefaultContexts.Save)]
        public string CIDMGroup
        {
            get => _CIDMGroup;
            set => SetPropertyValue(nameof(CIDMGroup), ref _CIDMGroup, value);
        }

        [Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.HtmlPropertyEditor)]
        [Appearance("UseCaseSpecificDataProductEndpoint_ShowLogOutput", Enabled = false)]
        [Appearance("UseCaseSpecificDataProductEndpoint_BlackBackground", BackColor = "Black", FontColor = "White")]
        public string LogOutput
        {
            get => logOutput;
            set => SetPropertyValue(nameof(LogOutput), ref logOutput, value);
        }

        [Appearance("UseCaseSpecificDataProductEndpoint_CidmRole", Enabled = false)]
        public CidmRole CidmRole
        {
            get => GetPropertyValue<CidmRole>();
            set => SetPropertyValue(nameof(CidmRole), value);
        }

        [VisibleInDetailView(false), VisibleInListView(false)]
        public string EndpointViewer => EndpointName?.ToLowerInvariant();

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == nameof(UseCase))
            {               
                if (newValue is UseCase UseCase)
                {
                    EndpointName = $"{DataProduct.GlobalId}_{UseCase.ID}";
                }
                else
                {
                    EndpointName = null;
                }
            }
        }
    }
}
