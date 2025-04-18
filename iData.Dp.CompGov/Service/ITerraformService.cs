namespace iData.Dp.CompGov.Service
{
    public interface ITerraformService
    {
        Task<bool> InitializeTerraform(string terraformPath, string dataProductId, IProgressReporter progressReporter);
        Task<bool> PlanTerraform(string terraformPath, IProgressReporter progressReporter);
        Task<bool> ApplyTerraform(string terraformPath, IProgressReporter progressReporter);
        Task<string> CreateTerraformConfiguration(string terraformPath, string dataProductId, Dictionary<string, string> cidmGroup2IamRole, IProgressReporter progressReporter);
    }
}
