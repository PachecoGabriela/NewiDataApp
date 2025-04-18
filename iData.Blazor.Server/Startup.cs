using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.Circuits;
using iData.Blazor.Server.Services;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Roche.Common.Security;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using DevExpress.ExpressApp.Dashboards.Blazor;
using DevExpress.ExpressApp.Office.Blazor;
using iData.Dp.CompGov;
using Roche.Common.AuditTrail;

namespace iData.Blazor.Server;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(typeof(Microsoft.AspNetCore.SignalR.HubConnectionHandler<>), typeof(ProxyHubConnectionHandler<>));

        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddHttpContextAccessor();
        services.AddScoped<CircuitHandler, CircuitHandlerProxy>();

        services.AddXaf(Configuration, builder =>
        {
            builder.UseApplication<iDataBlazorApplication>();
            builder.Modules
                .AddAuditTrailXpo(options =>
                {
                    options.AuditDataItemPersistentType = typeof(AuditDataItemPersistentUTC);
                    options.Events.OnCustomCreateAuditDataStore = context =>
                    {
                        context.AuditDataStore = new CustomAuditDataStore();
                    };
                })
                .AddCloningXpo()
                .AddConditionalAppearance()
                .AddValidation(options =>
                {
                    options.AllowValidationDetailsAccess = false;
                })
                .AddViewVariants()
                .Add<iData.Module.iDataModule>()
                .Add<iData.Dp.ActivatedAsaps.ActivatedAsapsModule>()
                .Add<iData.Dp.CompGov.CompGovModule>()
                .Add<iData.Dp.AsapRecords.AsapRecordsModule>()
                .Add<iDataBlazorModule>();

            builder.ObjectSpaceProviders
                .AddSecuredXpo((serviceProvider, options) =>
                {
                    options.ConnectionString = Configuration["ConnectionString"];
                    options.ThreadSafe = true;
                    options.UseSharedDataStoreProvider = true;
                })
               .AddNonPersistent();

            builder.Security
                .UseIntegratedMode(options =>
                {
                    options.RoleType = typeof(CidmRole);
                    options.UserType = typeof(PermissionPolicyUser);

                    options.UseXpoPermissionsCaching();
                    options.Events.OnSecurityStrategyCreated += securityStrategy =>
                    {
                        ((SecurityStrategy)securityStrategy).PermissionsReloadMode = PermissionsReloadMode.NoCache;
                    };
                })
                .AddPasswordAuthentication()
                .AddAuthenticationProvider<CustomAuthenticationProvider>();

        });
        services.AddComputationalGovernance();
        services.AddXafDashboards();
        services.AddXafOffice();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(options =>
              {
                  options.LoginPath = "/LoginPage";
              })
              .AddOAuth("AWS Cognito", "Roche Credentials", options =>
              {
                  options.ClientId = Configuration["COGNITO_CLIENT_ID"];
                  options.ClientSecret = Configuration["COGNITO_CLIENT_SECRET"];

                  var cognitoDomain = Configuration["COGNITO_DOMAIN"];
                  options.AuthorizationEndpoint = $"{cognitoDomain}/oauth2/authorize";
                  options.TokenEndpoint = $"{cognitoDomain}/oauth2/token";
                  options.UserInformationEndpoint = $"{cognitoDomain}/oauth2/userInfo";

                  options.Scope.Add("openid");
                  options.Scope.Add("email");
                  options.Scope.Add("profile");

                  options.CallbackPath = new PathString("/signin-oidc");

                  options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                  options.ClaimActions.MapJsonKey(ClaimTypes.Name, "preferred_username");
                  options.ClaimActions.MapJsonKey("sub", "sub");
                  options.ClaimActions.Add(new CustomGroupClaimAction(ClaimTypes.Role, ClaimValueTypes.String));

                  options.Events = new OAuthEvents
                  {
                      OnCreatingTicket = async context =>
                      {
                          var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                          request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                          request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                          var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                          response.EnsureSuccessStatusCode();
                          var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                          context.RunClaimActions(json.RootElement);
                      }
                  };
              });
    }

    private static ForwardedHeadersOptions GetForwardedHeadersOptions()
    {
        ForwardedHeadersOptions forwardedHeadersOptions = new ForwardedHeadersOptions()
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };
        forwardedHeadersOptions.KnownNetworks.Clear();
        forwardedHeadersOptions.KnownProxies.Clear();
        return forwardedHeadersOptions;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseForwardedHeaders(GetForwardedHeadersOptions());

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseHttpsRedirection();
        app.UseRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseXaf();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapXafDashboards();
        });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapXafEndpoints();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
            endpoints.MapControllers();
        });

    }
}
