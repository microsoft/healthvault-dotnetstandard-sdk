using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.HealthVault.AspNetCore;
using Microsoft.HealthVault.Configuration;

namespace SandboxWebCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHealthVault(new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("79aec84a-1131-435d-bdcc-40de9b870c35"),
                DefaultHealthVaultShellUrl = new Uri("https://account.healthvault-ppe.com/"),
                DefaultHealthVaultUrl = new Uri("https://platform.healthvault-ppe.com/platform/"),
                RestHealthVaultUrl = new Uri("https://data.ppe.microsofthealth.net")
            });

            // Sign-out to remove the user cookie.
            app.Map("/logout", builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    string identityName = context.User.Identity.Name;
                    await context.Authentication.SignOutAsync(HealthVaultAuthenticationMiddleware.AuthenticationScheme);
                    await context.Response.WriteAsync("<html><body>");
                    await context.Response.WriteAsync("You have been logged out. Goodbye " + identityName + "<br>");
                    await context.Response.WriteAsync("<a href=\"/\">Home</a>");
                    await context.Response.WriteAsync("</body></html>");
                });
            });

            // Display the remote error
            app.Map("/error", builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<html><body>");
                    await context.Response.WriteAsync("An remote failure has occurred: " + context.Request.Query["FailureMessage"] + "<br>");
                    await context.Response.WriteAsync("<a href=\"/\">Home</a>");
                    await context.Response.WriteAsync("</body></html>");
                });
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
