using DevExpress.EasyTest.Framework;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

// To run functional tests for ASP.NET Web Forms and ASP.NET Core Blazor XAF Applications,
// install browser drivers: https://www.selenium.dev/documentation/getting_started/installing_browser_drivers/.
//
// -For Google Chrome: download "chromedriver.exe" from https://chromedriver.chromium.org/downloads.
// -For Microsoft Edge: download "msedgedriver.exe" from https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/.
//
// Selenium requires a path to the downloaded driver. Add a folder with the driver to the system's PATH variable.
//
// Refer to the following article for more information: https://docs.devexpress.com/eXpressAppFramework/403852/

namespace iData.Module.E2E.Tests;

public class iDataTests : IDisposable
{
    const string BlazorAppName = "iDataBlazor";
    const string AppDBName = "iData";
    EasyTestFixtureContext FixtureContext { get; } = new EasyTestFixtureContext();

    public iDataTests()
    {
        FixtureContext.RegisterApplications(
            new BlazorApplicationOptions(BlazorAppName, string.Format(@"{0}\..\..\..\..\iData.Blazor.Server", Environment.CurrentDirectory),
                            browser: "Chrome",
                            webDriverPath: string.Format(@"{0}\..\..\..\..\..\libs", Environment.CurrentDirectory)
        ));
        FixtureContext.RegisterDatabases(new DatabaseOptions(AppDBName, "iDataEasyTest", server: @"(localdb)\mssqllocaldb"));
    }
    public void Dispose()
    {
        FixtureContext.CloseRunningApplications();
    }
    [Theory]
    [InlineData(BlazorAppName)]
    public void TestBlazorApp(string applicationName)
    {
        FixtureContext.DropDB(AppDBName);
        var appContext = FixtureContext.CreateApplicationContext(applicationName);
        appContext.RunApplication();
        Assert.True(appContext.Navigate("My Details"));
        Assert.True(appContext.Navigate("Role"));
        Assert.True(appContext.Navigate("Users"));
    }
}
