using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System.ComponentModel;

namespace iData.Dp.CompGov.Entities;

[DomainComponent]
public class ConnectedDevicesDp
{
    [VisibleInDetailView(false)]
    public string DisplayName => "";

    [Browsable(false)]
    [DevExpress.ExpressApp.Data.Key]
    public int Oid { get; set; } = 1;

    public string ViewName { get; set; } = "idata_ext.connected_devices_data_product";
}