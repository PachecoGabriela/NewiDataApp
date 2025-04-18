using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using iData.Dp.CompGov.Entities;
using iData.Dp.CompGov.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Roche.Common.Security;
namespace iData.Dp.CompGov.Controller
{
    public class UseCaseSpecificDataProductEndpointDetailViewController : ObjectViewController<DetailView, UseCaseSpecificDataProductEndpoint>, IProgressReporter
    {
        public SimpleAction CreateRecreateRedshiftViewAction { get; private set; }

        private readonly RedshiftService _redshiftService;

        public UseCaseSpecificDataProductEndpointDetailViewController()
        {
            CreateRecreateRedshiftViewAction = new SimpleAction(
                 this, "CreateRecreateRedshiftView", "aws sync")
            {
                Caption = "Create/Recreate Redshift View",
                ImageName = "Action_Refresh",
                TargetObjectsCriteria = "DataProduct Is Not Null And EndpointName Is Not Null"
            };
            CreateRecreateRedshiftViewAction.Execute += CreateRecreateRedshiftViewAction_Execute;

        }

        [ActivatorUtilitiesConstructor]
        public UseCaseSpecificDataProductEndpointDetailViewController(RedshiftService redshiftService) : this()
        {
            _redshiftService = redshiftService;
        }

        public void ReportProgress(string message)
        {
            if (ObjectSpace == null)
            {
                Console.WriteLine("ERROR: ReportProgress() ObjectSpace is null. Unable to write message: " + message);
                return;
            }

            var ExecuteJs = ObjectSpace.ServiceProvider.GetService<IJSRuntime>();

            if (!string.IsNullOrWhiteSpace(message))
            {
                ViewCurrentObject.LogOutput = $"{ViewCurrentObject.LogOutput}{Environment.NewLine}{message}";
            }

            ExecuteJs.InvokeVoidAsync("moveCursorToEndByClass", "UseCaseSpecificDataProductEndpoint_LogOutput");
        }

        private async void CreateRecreateRedshiftViewAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                var endpoint = ViewCurrentObject;
                endpoint.LogOutput = string.Empty;
                endpoint.Save();
                ObjectSpace.CommitChanges();

                CreateUpdateAppRole();

                ReportProgress($"{DateTime.Now} ---------------------------------------------------------------------------------------------");
                ReportProgress($"Creating use case specific endpoint for data product: {endpoint.DataProduct.Name} in Redshift.");

                bool success = await _redshiftService.ValidateConnection(this);

                if (success)
                    success = await _redshiftService.DropMaterializedView(this, endpoint.EndpointName);
                if (success)
                    success = await _redshiftService.CreateMaterializedView(this, endpoint);

                ReportProgress(success ? "Endpoint created successfully." : "There were errors, endpoint could not be created.");

                if (ObjectSpace == null)
                {
                    Console.WriteLine("ERROR: CreateRecreateRedshiftViewAction_Execute() ObjectSpace is null. Unable to save changes.");
                    return;
                }
                ObjectSpace.CommitChanges();
                View.Refresh(true);

                await Task.Delay(100);
                var jsRuntime = ObjectSpace.ServiceProvider.GetService<IJSRuntime>();
                await jsRuntime.InvokeVoidAsync("moveCursorToEndByClass", "UseCaseSpecificDataProductEndpoint_LogOutput");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void CreateUpdateAppRole()
        {
            var ep = ViewCurrentObject;

            // make sure to save the role directly to DB, even if the user doesn't save the current object
            using var os = Application.CreateObjectSpace<CidmRole>();
            CidmRole role = ep.CidmRole != null ? os.GetObject(ep.CidmRole) : os.CreateObject<CidmRole>();
            if (role.IsDeleted)
            {
                role = os.CreateObject<CidmRole>();
            }
            role.Name = $"Endpoint {ep.EndpointName}";
            role.CidmGroup = ep.CIDMGroup;
            role.PermissionPolicy = SecurityPermissionPolicy.DenyAllByDefault;
            os.Delete(role.TypePermissions);
            role.AddObjectPermission<UseCaseSpecificDataProductEndpoint>("Read", $"Oid = '{ep.Oid}'", SecurityPermissionState.Allow);
            var dpMembers = new List<string>
            {
                nameof(iDataDataProduct.Oid),
                nameof(iDataDataProduct.Name),
                nameof(iDataDataProduct.DisplayName),
                nameof(iDataDataProduct.GlobalId),
            }; 
            role.AddMemberPermission<iDataDataProduct>("Read", string.Join(';', dpMembers), $"[UseCaseEndpoints][[Oid] = '{ep.Oid}']", SecurityPermissionState.Allow);
            var ucMembers = new List<string>
            {
                nameof(UseCase.Oid),
                nameof(UseCase.Description),
                nameof(UseCase.DisplayName),
                nameof(UseCase.Title),
            };
            role.AddMemberPermission<UseCase>("Read", string.Join(';', ucMembers), $"[UseCaseEndpoints][[Oid] = '{ep.Oid}']", SecurityPermissionState.Allow);
            role.AddTypePermission<RedshiftConfig>("Read", SecurityPermissionState.Allow);

            role.AddNavigationPermission("Application/NavigationItems/Items/Governance/Items/UseCaseSpecificDataProductEndpoint_ListView_Endpoints", SecurityPermissionState.Allow);

            os.GetObject(ep).CidmRole = role;

            os.CommitChanges();
            View.Refresh(true);
        }
    }
}