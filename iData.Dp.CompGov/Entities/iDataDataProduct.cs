using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Roche.Common;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using Roche.Common.Security;

namespace iData.Dp.CompGov.Entities
{
    [System.ComponentModel.DisplayName("iData Data Product")]


    public class iDataDataProduct : BaseBO
    {
        private string name;

        private string latestTerraform;
        private string logOutput;

        public iDataDataProduct(Session session) : base(session) { }

        [RuleRequiredField("iDataDataProductNameRequired", DefaultContexts.Save)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }


        private string globalId;

        [RuleRequiredField("iDataDataProductGlobalIdRequired", DefaultContexts.Save)]
        [RuleUniqueValue("iDataDataProductGlobalIdUnique", DefaultContexts.Save)]
        public string GlobalId
        {
            get => globalId;
            set => SetPropertyValue(nameof(GlobalId), ref globalId, value);
        }


        private bool requiresUseCaseSpecficEndpoints;

        public bool RequiresUseCaseSpecficEndpoints
        {
            get => requiresUseCaseSpecficEndpoints;
            set => SetPropertyValue(nameof(RequiresUseCaseSpecficEndpoints), ref requiresUseCaseSpecficEndpoints, value);
        }

        private string _CIDMGroup;


        [Appearance("HideCIDMGroup", Criteria = "RequiresUseCaseSpecficEndpoints", Visibility = ViewItemVisibility.Hide)]
        public string CIDMGroup
        {
            get => _CIDMGroup;
            set => SetPropertyValue(nameof(CIDMGroup), ref _CIDMGroup, value);
        }

        private string redshiftTableName;

        [RuleRequiredField("RedshiftConfig_RedshiftTableName_Required", DefaultContexts.Save)]
        public string RedshiftTableName
        {
            get => redshiftTableName;
            set => SetPropertyValue(nameof(RedshiftTableName), ref redshiftTableName, value);
        }

        [Size(SizeAttribute.Unlimited)]
        [Appearance("LatestTerraform", Enabled = false)]
        public string LatestTerraform
        {
            get => latestTerraform;
            set => SetPropertyValue(nameof(LatestTerraform), ref latestTerraform, value);
        }



        [Size(SizeAttribute.Unlimited)]
        [Appearance("ShowLogOutput", Enabled = false)]
        [Appearance("BlackBackground - UseCaseSpecificDataProductEndpoint", BackColor = "Black", FontColor = "White")]
        public string LogOutput
        {
            get => logOutput;
            set => SetPropertyValue(nameof(LogOutput), ref logOutput, value);
        }


        [Appearance("HideUseCaseEndpoints", Criteria = "!RequiresUseCaseSpecficEndpoints", Visibility = ViewItemVisibility.Hide)]
        [Association, Aggregated, VisibleInDetailView(true)]
        public XPCollection<UseCaseSpecificDataProductEndpoint> UseCaseEndpoints
        {
            get { return GetCollection<UseCaseSpecificDataProductEndpoint>(nameof(UseCaseEndpoints)); }
        }

        [Association()]
        [DisplayName("Data Elements")]
        public XPCollection<DataElement> DataElements
        {
            get { return GetCollection<DataElement>(nameof(DataElements)); }
        }

        [Appearance("HideCIDMRole", Criteria = "RequiresUseCaseSpecficEndpoints", Visibility = ViewItemVisibility.Hide)]
        [Appearance("iDataDataProduct_CidmRole", Enabled = false)]
        [VisibleInListView(false)]
        public CidmRole CidmRole
        {
            get => GetPropertyValue<CidmRole>();
            set => SetPropertyValue(nameof(CidmRole), value);
        }

        public override string DisplayName => Name;


        [VisibleInDetailView(false), VisibleInListView(false)]
        public string EndpointViewer => RedshiftTableName?.ToLowerInvariant();
    }
}
