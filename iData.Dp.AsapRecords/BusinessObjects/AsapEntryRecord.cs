using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Roche.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Web;
using DevExpress.Data.Filtering;

namespace iData.Dp.AsapRecords.BusinessObjects
{
    [DefaultClassOptions]
    [ModelDefault("Caption","ASAP Entry")]
    [ImageName("AsapEntry")]
    [Persistent("AsapEntryNew")]
    public class AsapEntryRecord : BaseBO
    { 
        public AsapEntryRecord(Session session)
            : base(session)
        {
        }


        int similarity;
        string systemTypeListView;
        bool versionPublished;
        string loincCode;
        string uSI;
        string systemType;
        AsapAssay assay;
        AsapLifeCycleTeam lifeCycleTeam;
        string asapName;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [Appearance("Disable AsapName - AsapEntry", Enabled = false, Criteria = "VersionPublished")]
        [ModelDefault("Caption", "ASAP Name")]
        [RuleRequiredField("AsapNameRequired - AsapEntry", DefaultContexts.Save)]
        public string AsapName
        {
            get => asapName;
            set => SetPropertyValue(nameof(AsapName), ref asapName, value);
        }

        [Appearance("Disable LifeCycleTeam - AsapEntry", Enabled = false, Criteria = "VersionPublished")]
        [RuleRequiredField("LifeCycleTeamRequired - AsapEntry", DefaultContexts.Save, TargetCriteria = "VersionPublished")]
        public AsapLifeCycleTeam LifeCycleTeam
        {
            get => lifeCycleTeam;
            set => SetPropertyValue(nameof(LifeCycleTeam), ref lifeCycleTeam, value);
        }

        [Appearance("Disable Assay - AsapEntry", Enabled = false, Criteria = "VersionPublished")]
        [RuleRequiredField("AssayRequired - AsapEntry", DefaultContexts.Save, TargetCriteria = "VersionPublished")]
        public AsapAssay Assay
        {
            get => assay;
            set => SetPropertyValue(nameof(Assay), ref assay, value);
        }

        [Appearance("Disable SystemType - AsapEntry", Enabled = false, Criteria = "VersionPublished")]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [EditorAlias("SystemTypeEditor")]
        [RuleRequiredField("SystemTypeRequired - AsapEntry", DefaultContexts.Save)]
        public string SystemType
        {
            get => systemType;
            set => SetPropertyValue(nameof(SystemType), ref systemType, value);
        }


        [Appearance("Disable USI - AsapEntry", Enabled = false, Criteria = "VersionPublished")]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string USI
        {
            get => uSI;
            set => SetPropertyValue(nameof(USI), ref uSI, value);
        }

        [Appearance("Disable LoincCode - AsapEntry", Enabled = false, Criteria = "VersionPublished")]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string LoincCode
        {
            get => loincCode;
            set => SetPropertyValue(nameof(LoincCode), ref loincCode, value);
        }

        [NonPersistent]
        public int Similarity
        {
            get => similarity;
            set => SetPropertyValue(nameof(Similarity), ref similarity, value);
        }

        [Association("AsapEntryNew-Versions"), DevExpress.Xpo.Aggregated]
        public XPCollection<AsapVersion> Versions
        {
            get
            {
                return GetCollection<AsapVersion>(nameof(Versions));
            }
        }

        [Association("AsapEntryNew-MappingTerms"), DevExpress.Xpo.Aggregated]
        public XPCollection<AsapMappingTerm> MappingTerms
        {
            get
            {
                return GetCollection<AsapMappingTerm>(nameof(MappingTerms));
            }
        }

        [Browsable(false)]
        public bool VersionPublished
        {
            get => VerifyIfVersionPublished();
        }

        public bool VerifyIfVersionPublished()
        {
            bool IsVersionPublished = false;

            var IsPublished = this.Versions.FirstOrDefault(x => x.WorkflowStatus == EWorkflowStatus.Published);

            if (IsPublished != null)
                IsVersionPublished = true;

            return IsVersionPublished;
        }

        public void UpdateLoincCode()
        {
            if (!string.IsNullOrEmpty(USI) && USI.Contains('^') && USI.EndsWith("^LN"))
            {
                LoincCode = USI.Substring(0, USI.IndexOf('^'));
            }
            else
            {
                LoincCode = null;
            }
        }

        public void UpdateSystemType()
        {
            if (!string.IsNullOrEmpty(this.SystemType))
            {
                if (this.SystemType.EndsWith(";"))
                    this.SystemType = this.SystemType.Remove(this.SystemType.Length - 1);
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            UpdateLoincCode();
            UpdateSystemType();
            CreatePrimaryMappingTerm();
        }

        private void CreatePrimaryMappingTerm()
        {
            var PrimaryTerm = this.MappingTerms.FirstOrDefault(x => x.IsPrimary);
            if (PrimaryTerm is null)
            {
                AsapMappingTerm MappingTerm = new AsapMappingTerm(Session);
                MappingTerm.Term = this.AsapName;
                MappingTerm.IsPrimary = true;
                MappingTerm.AsapEntry = this;
            }
        }

        public override string DisplayName => AsapName;
    }
}