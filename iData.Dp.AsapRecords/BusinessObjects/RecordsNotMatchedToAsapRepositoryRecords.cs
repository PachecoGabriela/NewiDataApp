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
    [Persistent("RecordsNotMatchedToAsapRepositoryRecords")]
    [ImageName("DataProduct")]
    [ModelDefault("Caption", "ASAP Records Unmatched")]
    public class RecordsNotMatchedToAsapRepositoryRecords : XPLiteObject
    { 
        public RecordsNotMatchedToAsapRepositoryRecords(Session session)
            : base(session)
        {
        }
        [Key, Browsable(false)]
        public int Oid { get; set; }
        [ModelDefault("Caption", "ASAP Technical Name")]
        public string AsapTechnicalName { get; set; }
        [ModelDefault("Caption", "ASAP Version")]
        public string AsapVersion { get; set; }
        [ModelDefault("Caption", "System Type")]
        public string DeviceReferenceSystemType { get; set; }
    }
}