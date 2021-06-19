/* ------------------------------------------------------------------------- *
thZero.NetCore.Library.Asp.Instrumentation.Healthchecks
Copyright (C) 2016-2021 thZero.com

<development [at] thzero [dot] com>

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 * ------------------------------------------------------------------------- */

using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace thZero.AspNetCore
{
    public class HealthChecksInstrumentationStartupExtension : BaseStartupExtension
    {
        #region Public Methods
        public override void ConfigureInitializePost(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IServiceProvider svp)
        {
            base.ConfigureInitializePost(app, env, loggerFactory, svp);

            ConfigureInitializeHealthChecks(app);
        }

        public override void ConfigureServicesInitializeMvcPre(IServiceCollection services, IWebHostEnvironment env, IConfiguration configuration)
        {
            base.ConfigureServicesInitializeMvcPre(services, env, configuration);

            ConfigureServicesHealthChecks(services.AddHealthChecks());
        }
        #endregion

        #region Protected Methods
        protected virtual void ConfigureInitializeHealthChecks(IApplicationBuilder app)
        {
            UseHealthCheck(app);
        }

        protected virtual void ConfigureServicesHealthChecks(IHealthChecksBuilder builder)
        {
        }

        protected void UseHealthCheck(IApplicationBuilder app, string routeFragment = null, HealthCheckOptions options = null, bool useRoute = true)
        {
            string route = (useRoute ? Route : string.Empty);
            if (!string.IsNullOrEmpty(routeFragment))
            {
                routeFragment = (!routeFragment.StartsWith("/") ? "/" : string.Empty) + routeFragment;
                route += routeFragment;
            }

            if (string.IsNullOrEmpty(route))
                throw new InvalidHealthCheckRouteException();

            if (options != null)
                app.UseHealthChecks(route, options);
            else
                app.UseHealthChecks(route);
        }
        #endregion

        #region Protected Properties
        protected string Route { get; set; } = "/diagnostics/health";
        #endregion
    }

    public class InvalidHealthCheckRouteException : Exception
    {
        public InvalidHealthCheckRouteException() : base("Invalid health check route.")
        {
        }
    }
}
