using DevExpress.ExpressApp;
using iData.Dp.CompGov.Entities;
using iData.Dp.CompGov.Service;
using Microsoft.Extensions.DependencyInjection;

namespace iData.Dp.CompGov.Controller
{
    public class UseCaseSpecificDataProductEndpointController : ViewController, IProgressReporter
    {
        private readonly RedshiftService _redshiftService;
        public UseCaseSpecificDataProductEndpointController()
        {
            TargetObjectType = typeof(UseCaseSpecificDataProductEndpoint);
        }

        [ActivatorUtilitiesConstructor]
        public UseCaseSpecificDataProductEndpointController(RedshiftService redshiftService) : this()
        {
            _redshiftService = redshiftService;
        }
        public void ReportProgress(string message)
        {
            Application.ShowViewStrategy.ShowMessage(message, InformationType.Info);
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            ObjectSpace.ObjectDeleting += ObjectSpace_ObjectDeleting;
        }

        protected override void OnDeactivated()
        {

            ObjectSpace.ObjectDeleting -= ObjectSpace_ObjectDeleting;
            base.OnDeactivated();
        }

        private void ObjectSpace_ObjectDeleting(object sender, ObjectsManipulatingEventArgs e)
        {
            foreach (object obj in e.Objects)
            {
                if (obj is UseCaseSpecificDataProductEndpoint endpoint)
                {
                    _redshiftService.DropMaterializedView(this, endpoint.EndpointName);
                }
            }
        }
    }
}
