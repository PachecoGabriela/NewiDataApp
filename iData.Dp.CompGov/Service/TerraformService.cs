using Microsoft.Extensions.Configuration;
using Scriban;
using System.Diagnostics;
using System.Reflection;

namespace iData.Dp.CompGov.Service
{
    public class TerraformService : ITerraformService
    {
        public IConfiguration Configuration { get; }

        public TerraformService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<string> CreateTerraformConfiguration(string terraformPath, string dataProductId, Dictionary<string, string> cidmGroup2IamRole, IProgressReporter progressReporter)
        {
            string templateText = ReadEmbeddedResource("iData.Dp.CompGov.Terraform.MainTemplate.scriban");

            var model = new
            {
                DataProductId = dataProductId,
                CognitoClientId = Configuration["COGNITO_CLIENT_ID"],
                CognitoUserPoolId = Configuration["COGNITO_USER_POOL_ID"],
                IamRolesWithRedshiftDataApiAccess = cidmGroup2IamRole.Select(i => i.Value).ToList(),
                CidmGroups = cidmGroup2IamRole.Select(i => new
                {
                    CidmGroup = i.Key,
                    Role = i.Value
                }).ToList(),
            };

            var template = Template.Parse(templateText);
            var result = template.Render(model);

            File.WriteAllText(Path.Combine(terraformPath, "main.tf"), result);

            progressReporter.ReportProgress(result);
            progressReporter.ReportProgress("Generated main.tf using template.");

            return result;
        }

        public async Task<bool> InitializeTerraform(string terraformPath, string dataProductId, IProgressReporter progressReporter)
        {
            string key = $"terraform/{dataProductId}/state.tfstate";
            string bucketName = $"computational-governance-terraform-state-{Configuration["ENVIRONMENT"]}";
            string arguments = $"init -backend-config=\"key={key}\" -backend-config=\"bucket={bucketName}\" -reconfigure";

            return await ExecuteTerraformCommand("terraform", arguments, terraformPath, progressReporter);
        }


        public async Task<bool> PlanTerraform(string terraformPath, IProgressReporter progressReporter)
        {
            return await ExecuteTerraformCommand("terraform", "plan", terraformPath, progressReporter);
        }

        public async Task<bool> ApplyTerraform(string terraformPath, IProgressReporter progressReporter)
        {
            return await ExecuteTerraformCommand("terraform", "apply -auto-approve", terraformPath, progressReporter);
        }

        private async Task<bool> ExecuteTerraformCommand(string command, string arguments, string workingDirectory, IProgressReporter progressReporter)
        {
            return await Task.Run(() =>
            {
                progressReporter.ReportProgress($"Executing {command} {arguments} in {workingDirectory}");

                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = command,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.OutputDataReceived += (sender, args) => progressReporter.ReportProgress(args.Data);
                    process.ErrorDataReceived += (sender, args) => progressReporter.ReportProgress(args.Data);

                    try
                    {
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        progressReporter.ReportProgress("Unable to start terraform command: " + ex.ToString());
                        return false;
                    }
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        progressReporter.ReportProgress($"Command failed with exit code {process.ExitCode}");
                        return false;
                    }
                }
                return true;
            });
        }


        private static string ReadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
