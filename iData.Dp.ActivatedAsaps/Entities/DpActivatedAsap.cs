using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace iData.AsapsPipeline.Entities
{
    [System.ComponentModel.DisplayName("Activated ASAP Record")]
    public class DpActivatedAsap : BaseObject
    {
        public DpActivatedAsap(Session session) : base(session) { }

        [Indexed, Persistent("snapshotdate")]
        [ModelDefault("ToolTip", "When was the information about installed ASAPs received from the instrument?")]
        public DateTime SnapshotDate { get; set; }

        [Indexed, Persistent("asapsoftwarename"), Size(100)]
        [ModelDefault("ToolTip", "The name of the Assay Specific Analysis Software Package.")]
        [DisplayName("ASAP Software Name")]
        public string AsapSoftwareName { get; set; }

        [Indexed, Persistent("asaptechnicalname"), Size(100)]
        [ModelDefault("ToolTip", "The ASAP name as found in data transferred back from the device, sometimes with slight variations from the software name, e.g. HPV-GT vs HPV GT")]
        [DisplayName("ASAP Technical Name")]
        public string AsapTechnicalName { get; set; }

        [Indexed, Persistent("asapversion"), Size(10)]
        [ModelDefault("ToolTip", "The version of the Assay Specific Analysis Package.")]
        [DisplayName("ASAP Version")]
        public string AsapVersion { get; set; }

        [Indexed, Persistent("asapmaterialnumber"), Size(100)]
        [ModelDefault("ToolTip", "The Material Number of the Assay Specific Analysis Package as listed in the SAP Material Master.")]
        [DisplayName("ASAP Material Number")]
        public string AsapMaterialNumber { get; set; }

        [Persistent("asapmaterialnamelong"), Size(250)]
        [ModelDefault("ToolTip", "The long name of the Assay Specific Analysis Package as in SAP Material Master.")]
        [DisplayName("ASAP Material Name Long")]
        public string AsapMaterialNameLong { get; set; }

        [Persistent("asapmaterialnamemedium"), Size(250)]
        [ModelDefault("ToolTip", "The medium name of the Assay Specific Analysis Package as in SAP Material Master.")]
        [DisplayName("ASAP Material Name Medium")]
        public string AsapMaterialNameMedium { get; set; }

        [Persistent("asapusi"), Size(100)]
        [ModelDefault("ToolTip", "The Universal Service Identifier of the ASAP Package.")]
        [DisplayName("ASAP USI")]
        public string AsapUsi { get; set; }

        [Persistent("asaploinccode"), Size(200)]
        [ModelDefault("ToolTip", "The LOINC Code of the ASAP Package.")]
        [DisplayName("ASAP LOINC Code")]
        public string AsapLoincCode { get; set; }

        [Persistent("asapgtin"), Size(20)]
        [ModelDefault("ToolTip", "The Global Trade Item Number of the ASAP Package.")]
        [DisplayName("ASAP GTIN")]
        public string AsapGtin { get; set; }

        [Indexed, Persistent("asaplifecycleteam"), Size(200)]
        [ModelDefault("ToolTip", "The LCT under which the ASAP Assay falls, such as Blood Screening, Cervical Cx, or Infectious Disease.")]
        [DisplayName("ASAP Lifecycle Team")]
        public string AsapLifeCycleTeam { get; set; }

        [Indexed, Persistent("asapassay"), Size(200)]
        [ModelDefault("ToolTip", "The Assay of this Analysis Package.")]
        [DisplayName("ASAP Assay")]
        public string AsapAssay { get; set; }

        [Indexed, Persistent("deviceserialno"), Size(20)]
        [ModelDefault("ToolTip", "The serial number of the device.")]
        public string DeviceSerialNo { get; set; }

        [Indexed, Persistent("devicereferencesystemtype"), Size(100)]
        [ModelDefault("ToolTip", "Harmonized system type as provided by the “connected devices” data product of the entitlements domain.")]
        public string DeviceReferenceSystemType { get; set; }

        [Persistent("devicematerialnumber"), Size(20)]
        [ModelDefault("ToolTip", "The material number of the device.")]
        public string DeviceMaterialNumber { get; set; }

        [Persistent("devicematerialname"), Size(200)]
        [ModelDefault("ToolTip", "The material name of the device.")]
        public string DeviceMaterialName { get; set; }

        [Persistent("devicesoldtoaccountnumber"), Size(50)]
        [ModelDefault("ToolTip", "Account number that identifies the legal counterpart in a transaction related to this device.")]
        public string DeviceSoldToAccountNumber { get; set; }

        [Persistent("devicebilltoaccountnumber"), Size(50)]
        [ModelDefault("ToolTip", "Account number that identifies the recipient of the invoice for a supply-chain transaction related to the device.")]
        public string DeviceBillToAccountNumber { get; set; }

        [Persistent("devicelocationaccountnumber"), Size(50)]
        [ModelDefault("ToolTip", "Account number that identifies where the device is located.")]
        public string DeviceLocationAccountNumber { get; set; }

        [Persistent("deviceshiptoaccountnumber"), Size(50)]
        [ModelDefault("ToolTip", "Account number that identifies where the device was shipped to.")]
        public string DeviceShipToAccountNumber { get; set; }

        [Persistent("devicelocationaccountname"), Size(250)]
        [ModelDefault("ToolTip", "The account name corresponding to the LocationAccountNumber.")]
        public string DeviceLocationAccountName { get; set; }

        [Persistent("devicelocation"), Size(250)]
        [ModelDefault("ToolTip", "The approximate installed location of this instrument as recorded in the Equipment Master System.")]
        public string DeviceLocation { get; set; }

        [Persistent("devicelocationcountrycode"), Indexed, Size(5)]
        [ModelDefault("ToolTip", "The two-letter country code of the device location.")]
        public string DeviceLocationCountryCode { get; set; }

        [Persistent("devicelongitude")]
        [ModelDefault("ToolTip", "The longitude of the device location.")]
        public double? DeviceLongitude { get; set; }

        [Persistent("devicelatitude")]
        [ModelDefault("ToolTip", "The latitude of the device location.")]
        public double? DeviceLatitude { get; set; }


        [Indexed, Persistent("ismostcurrentsnapshot")]
        [ModelDefault("ToolTip", "True if the snapshot data is the most current information received from this instrument, false if historical.")]
        public bool IsMostCurrentSnapshot { get; set; }

        [Indexed, Persistent("isactivated")]
        [ModelDefault("ToolTip", "True if the ASAP is activated on the device, false otherwise. An entry with false will appear only once after deactivation.")]
        public bool IsActivated { get; set; }

        [Indexed, Persistent("wasnewlyactivated")]
        [ModelDefault("ToolTip", "True if the ASAP is newly activated in this snapshot and was not activated in the previous snapshot.")]
        public bool WasNewlyActivated { get; set; }

        [Indexed, Persistent("wasdeactivated")]
        [ModelDefault("ToolTip", "True if the ASAP was activated in the previous snapshot but is now deactivated, indicating a recent deactivation.")]
        public bool WasDeactivated { get; set; }
    }
}
