using DevExpress.ExpressApp.Blazor.Components;

namespace iData.Blazor.Server.Controllers
{
    public class CustomAlertsHandler : AlertsHandler
    {
        protected override void Refresh()
        {
            foreach (var alert in AlertsToDisplay)
            {
                if ((alert.Duration == 0 || alert.Duration == AlertTemplate.DefaultDuration))
                {
                    alert.Duration = 0;
                    alert.NeedAutoClose = false;
                }
            }
            base.Refresh();
        }
    }
}
