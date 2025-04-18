using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace Roche.Common.Controllers
{
    public class ReadOnlyEntityViewController : ViewController
    {
        public ReadOnlyEntityViewController()
        {
            TargetObjectType = typeof(IReadOnlyEntity);
            TargetViewType = ViewType.Any;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            // Disable modification-related Actions
            new ActionBase[] {
            //Frame.GetController<ListViewControllerBase>()?.EditAction,
                Frame.GetController<NewObjectViewController>()?.NewObjectAction,
                Frame.GetController<DeleteObjectsViewController>()?.DeleteAction,
                Frame.GetController<ModificationsController>()?.SaveAction,
                Frame.GetController<ModificationsController>()?.SaveAndCloseAction,
                Frame.GetController<ModificationsController>()?.SaveAndNewAction,
                Frame.GetController<LinkUnlinkController>()?.LinkAction,
                Frame.GetController<LinkUnlinkController>()?.UnlinkAction
            }
            .Where(action => action != null)
            .ToList()
            .ForEach(action => action.Active["ReadOnly"] = false);
        }

        protected override void OnDeactivated()
        {
            new ActionBase[] {
            Frame.GetController<NewObjectViewController>()?.NewObjectAction,
            Frame.GetController<DeleteObjectsViewController>()?.DeleteAction,
            Frame.GetController<ModificationsController>()?.SaveAction,
            Frame.GetController<ModificationsController>()?.SaveAndCloseAction,
            Frame.GetController<ModificationsController>()?.SaveAndNewAction,
            Frame.GetController<LinkUnlinkController>()?.LinkAction,
            Frame.GetController<LinkUnlinkController>()?.UnlinkAction
        }
            .Where(action => action != null)
            .ToList()
            .ForEach(action => action.Active["ReadOnly"] = true);

            base.OnDeactivated();
        }
    }
}
