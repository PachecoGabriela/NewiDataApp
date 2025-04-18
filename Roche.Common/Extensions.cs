using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
  
    public static string GetConfigValue(this XafApplication application, string name)
    {
        if (application is BlazorApplication blazorApplication)
        {
            var configuration = blazorApplication.ServiceProvider.GetRequiredService<IConfiguration>();
            return configuration[name];
        }
        else
        {
            throw new InvalidOperationException("This extension method is only supported for Blazor applications.");
        }
    }

    public static IConfiguration GetConfig(this XafApplication application)
    {
        if (application is BlazorApplication blazorApplication)
        {
            return blazorApplication.ServiceProvider.GetRequiredService<IConfiguration>();
        }
        else
        {
            throw new InvalidOperationException("This extension method is only supported for Blazor applications.");
        }
    }
}


