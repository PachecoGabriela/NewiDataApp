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
    [Persistent("Assay")]
    [ImageName("compliance1")]
    [ModelDefault("Caption", "ASAP Assay")]
    public class AsapAssay : BaseBO
    { 
        public AsapAssay(Session session)
            : base(session)
        {
        }


        string assay;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [RuleRequiredField("DescriptionRequired - Assay", DefaultContexts.Save)]
        public string Assay
        {
            get => assay;
            set => SetPropertyValue(nameof(Assay), ref assay, value);
        }

        public override string DisplayName => Assay;
    }
}