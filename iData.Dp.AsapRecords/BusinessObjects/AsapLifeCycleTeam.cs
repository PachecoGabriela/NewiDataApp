using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
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

namespace iData.Dp.AsapRecords.BusinessObjects
{
    [DefaultClassOptions]
    [Persistent("LifeCycleTeam")]
    [ImageName("Refresh")]
    [ModelDefault("Caption", "ASAP Life Cycle Team")]
    public class AsapLifeCycleTeam : BaseBO
    { 
        public AsapLifeCycleTeam(Session session)
            : base(session)
        {
        }


        string lifeCycleTeam;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [RuleRequiredField("LifeCycleTeam - AsapLifeCycleTeam", DefaultContexts.Save)]
        public string LifeCycleTeam
        {
            get => lifeCycleTeam;
            set => SetPropertyValue(nameof(LifeCycleTeam), ref lifeCycleTeam, value);
        }

        public override string DisplayName => LifeCycleTeam;
    }
}