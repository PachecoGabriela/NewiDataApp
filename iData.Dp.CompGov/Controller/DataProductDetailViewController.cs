using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.XtraSpreadsheet.Model;
using iData.Dp.CompGov.Entities;
using iData.Dp.CompGov.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Roche.Common.Security;
using System.Reflection;
using System.Text.RegularExpressions;

namespace iData.Dp.CompGov.Controller
{
    public class DataProductDetailViewController : ObjectViewController<DetailView, iDataDataProduct>, IProgressReporter
    {

        public SimpleAction CreateRecreateIamRole { get; private set; }

        private readonly ITerraformService _terraformService;
        private readonly RedshiftService _redshiftService;


        public DataProductDetailViewController()
        {
            CreateRecreateIamRole = new SimpleAction(this, "CreateRecreateIamRole", "aws sync")
            {
                Caption = "Create/Recreate Identity Pool, Roles and Rules",
                ImageName = "Action_Refresh",
                TargetObjectsCriteria = "GlobalId Is Not Null"
            };
            CreateRecreateIamRole.Execute += CreateRecreateIamRole_Execute;
        }

        [ActivatorUtilitiesConstructor]
        public DataProductDetailViewController(ITerraformService terraformService, RedshiftService redshiftService) : this()
        {
            _redshiftService = redshiftService;
            _terraformService = terraformService;
        }

        private string CleanShellOutput(string s)
        {
            return new Regex(@"\x1B\[[^@-~]*[@-~]").Replace(s, "");
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        private async void CreateRecreateIamRole_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var dp = ViewCurrentObject;
            dp.LogOutput = string.Empty;
            dp.Save();
            ObjectSpace.CommitChanges();

            ReportProgress($"{DateTime.Now} ---------------------------------------------------------------------------------------------");

            var conf = ObjectSpace.GetObjects<RedshiftConfig>().Single();

            var roleNames = dp.RequiresUseCaseSpecficEndpoints
                        ? dp.UseCaseEndpoints.ToDictionary(i => i.EndpointName, i => i.EndpointName)
                        : new Dictionary<string, string> { { dp.GlobalId, dp.RedshiftTableName } };

            ReportProgress($"Creating/replacing redshift user(s): {string.Join(", ", roleNames)}.");

            var results = new List<bool>();

            //not parallel execution on purpose, race cnoditions in DB
            foreach (var roleName in roleNames)
            {
                bool result = await _redshiftService.CreateReadonlyRole(this, roleName.Key, roleName.Value);
                results.Add(result);

                ReportProgress($"Role creation for {roleName.Key} completed with result: {result}");
            }

            if (!results.All(i => i))
            {
                ReportProgress("Redshift user creation failed.");
                return;
            }

            ReportProgress($"Preparing and executing Terraform Code for Data Product: {dp.GlobalId}.");

            string terraformPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Terraform");

            var terraform = await _terraformService.CreateTerraformConfiguration(
                terraformPath,
                dp.GlobalId,
                dp.RequiresUseCaseSpecficEndpoints
                    ? dp.UseCaseEndpoints.ToDictionary(i => i.CIDMGroup, i => i.EndpointName)
                    : new() { { dp.CIDMGroup, dp.GlobalId } },
                this
            );

            bool initSuccess = await _terraformService.InitializeTerraform(terraformPath, dp.GlobalId, this);

            if (!initSuccess)
            {
                ReportProgress("Terraform init failed.");
                return;
            }

            bool planSuccess = await _terraformService.PlanTerraform(terraformPath, this);
            if (!planSuccess)
            {
                ReportProgress("Terraform plan failed.");
                return;
            }

            bool applySuccess = await _terraformService.ApplyTerraform(terraformPath, this);
            ReportProgress(applySuccess ? "Terraform operations completed successfully." : "Terraform apply failed.");

            if (applySuccess)
            {
                ViewCurrentObject.LatestTerraform = terraform;

                ObjectSpace.CommitChanges();
                View.Refresh(true);
            }
            await Task.Delay(100); // Adjust delay if necessary
            var jsRuntime = ObjectSpace.ServiceProvider.GetService<IJSRuntime>();
            await jsRuntime.InvokeVoidAsync("moveCursorToEndByClass", "iDataDataProduct_LogOutput");
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
                message = CleanShellOutput(message);
                ViewCurrentObject.LogOutput = $"{ViewCurrentObject.LogOutput}{Environment.NewLine}{message}";
            }

            ExecuteJs.InvokeVoidAsync("moveCursorToEndByClass", "iDataDataProduct_LogOutput");
        }
    }
}
