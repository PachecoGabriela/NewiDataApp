using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Roche.Common.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace iData.Dp.AsapRecords.BusinessObjects
{
    [DefaultClassOptions]
    [ImageName("compliance2")]
    [ModelDefault("Caption", "ASAP Version")]
    [RuleCombinationOfPropertiesIsUnique("ASAPEntry, AsapVersion, MaterialNumber", DefaultContexts.Save, "Version, MaterialNumber, AsapEntry", CustomMessageTemplate = "Please check if the version or the material number already exists.")]
    public class AsapVersion : BaseWorkflowBO
    {
        public AsapVersion(Session session) : base(session) { }


        AsapEntryRecord asapEntry;
        string materialNumber;
        string version;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [Persistent("AsapVersion")]
        [ModelDefault("Caption","ASAP Version")]
        [RuleRequiredField("ASAPVersionRequired - AsapVersion", DefaultContexts.Save)]
        public string Version
        {
            get => version;
            set => SetPropertyValue(nameof(Version), ref version, value);
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [RuleRequiredField("MaterialNumberRequired - AsapVersion", DefaultContexts.Save, TargetCriteria = "WorkflowStatus = 1")]
        public string MaterialNumber
        {
            get => materialNumber;
            set => SetPropertyValue(nameof(MaterialNumber), ref materialNumber, value);
        }

        [Association("AsapEntryNew-Versions")]
        public AsapEntryRecord AsapEntry
        {
            get => asapEntry;
            set => SetPropertyValue(nameof(AsapEntry), ref asapEntry, value);
        }

        [NonPersistent]
        [ModelDefault("Caption","Workflow Status")]
        public string WorkStatus
        {
            get => GetWorkFlowStatus();
        }

        private string GetWorkFlowStatus()
        {
            string Status = this.WorkflowStatus.ToString();
            return Status;
        }

        [NonPersistent]
        [Browsable(false)]
        public Version VersionTmp
        {
            get
            {
                // Convierte el campo de string a un objeto Version
                if (!string.IsNullOrEmpty(this.Version))
                    return new Version(this.Version);
                else
                    return null;
            }
        }

        public override string DisplayName => Version;

        protected override void OnSaving()
        {
            base.OnSaving();

            if (this.WorkflowStatus == Roche.Common.EWorkflowStatus.Published)
            { 
                if (AsapEntry is not null)
                    AsapEntry.VerifyIfVersionPublished();
            }
                
        }
    }
}