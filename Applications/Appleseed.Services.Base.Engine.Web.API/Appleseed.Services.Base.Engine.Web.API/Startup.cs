using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cassandra;

namespace Appleseed.Services.Base.Engine.API
{
    public class Startup
    {
        private string appConfigSource = "127.0.0.1";

        private int appConfigSourcePort = 9042;

        public Startup(IHostingEnvironment env)
        {
            Cluster cluster = Cluster.Builder().WithPort(appConfigSourcePort).AddContactPoint(appConfigSource).WithDefaultKeyspace("appleseed_search_engines").Build();
            Cassandra.ISession session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();

            // TODO: Remove hard-coded query and replace with the definition in .\Appleseed.Services.Search.Console\config\appleseed_schema.cql
            session.Execute("CREATE TABLE IF NOT EXISTS appleseed_search_engines.config (\"config_type\" text,\"config_name\" text,\"config_values\" map<text, frozen<map<text, text>>>,PRIMARY KEY((\"config_type\"), \"config_name\"))");


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
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{type?}");
            });
        }
    }
}
