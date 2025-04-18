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
    [ImageName("MappingTerm")]
    [ModelDefault("Caption", "ASAP Mapping Term")]
    [RuleCombinationOfPropertiesIsUnique("AsapEntry, Term", DefaultContexts.Save, "AsapEntry, Term", CustomMessageTemplate = "Please check if the term already exists.")]
    public class AsapMappingTerm : BaseBO
    { 
        public AsapMappingTerm(Session session)
            : base(session)
        {
        }


        AsapEntryRecord asapEntry;
        bool isPrimary;
        string note;
        string term;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [RuleRequiredField("TermRequired - AsapMappingTerm", DefaultContexts.Save)]
        public string Term
        {
            get => term;
            set => SetPropertyValue(nameof(Term), ref term, value);
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Note
        {
            get => note;
            set => SetPropertyValue(nameof(Note), ref note, value);
        }

        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public bool IsPrimary
        {
            get => isPrimary;
            set => SetPropertyValue(nameof(IsPrimary), ref isPrimary, value);
        }

        [Association("AsapEntryNew-MappingTerms")]
        public AsapEntryRecord AsapEntry
        {
            get => asapEntry;
            set => SetPropertyValue(nameof(AsapEntry), ref asapEntry, value);
        }

        public override string DisplayName => Term;

        protected override void OnSaving()
        {
            base.OnSaving();
        }
    }
}