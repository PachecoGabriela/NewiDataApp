using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.DesignTime;
using DevExpress.ExpressApp.Design;
using DevExpress.Xpo.Logger;
using Roche.Common;

namespace iData.Blazor.Server;

public class Program : IDesignTimeApplicationFactory
{
    private static bool ContainsArgument(string[] args, string argument)
    {
        return args.Any(arg => arg.TrimStart('/').TrimStart('-').ToLower() == argument.ToLower());
    }
    public static int Main(string[] args)
    {
        FrameworkSettings.DefaultSettingsCompatibilityMode = FrameworkSettingsCompatibilityMode.Latest;

        CultureInfo.CurrentCulture = CultureHelper.GetCulture();
        CultureInfo.DefaultThreadCurrentCulture = CultureHelper.GetCulture();
        CultureInfo.CurrentUICulture = CultureHelper.GetCulture();
        CultureInfo.DefaultThreadCurrentUICulture = CultureHelper.GetCulture();

        IHost host = CreateHostBuilder(args).Build();
        host.Run();
        return 0;
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    XafApplication IDesignTimeApplicationFactory.Create()
    {
        IHostBuilder hostBuilder = CreateHostBuilder(Array.Empty<string>());
        return DesignTimeApplicationFactoryHelper.Create(hostBuilder);
    }
}
