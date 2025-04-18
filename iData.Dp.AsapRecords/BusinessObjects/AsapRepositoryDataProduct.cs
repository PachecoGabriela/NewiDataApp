using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace iData.Dp.AsapRecords.BusinessObjects
{
    [DefaultClassOptions]
    [Persistent("AsapRepositoryDataProduct")]
    [ImageName("DataProduct")]
    [ModelDefault("Caption", "ASAP Repository Data Product")]
    public class AsapRepositoryDataProduct : XPLiteObject
    { 
        public AsapRepositoryDataProduct(Session session)
            : base(session)
        {
        }

        [Key, Browsable(false)]
        public int Oid { get; set; }
        [ModelDefault("Caption", "ASAP Name")]
        public string AsapName { get; set; }
        public string LifeCycleTeam { get; set; }
        public string SystemType { get; set; }
        public string Assay { get; set; }
        public string USI { get; set; }
        public string LoincCode { get; set; }
        public string AsapVersion { get; set; }
        public string MappingTerm { get; set; }
        public bool IsPrimary { get; set; }
        public string WorkflowStatus { get; set; }
        public string MaterialNumber { get; set; }
    }
}