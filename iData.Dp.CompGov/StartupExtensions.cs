using Amazon.RedshiftDataAPIService;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Dashboards.Blazor.Services;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509.Qualified;
using iData.Dp.CompGov.Entities;
using iData.Dp.CompGov.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iData.Dp.CompGov
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddComputationalGovernance(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<RedshiftService>();
            services.AddScoped<ITerraformService, TerraformService>();

            return services;
        }
    }
}

