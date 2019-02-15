using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ZL.CommandCore;
using ZL.CommandCore.Abs;

namespace Demo.Web1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddCommand(opt => {
                opt.ConnectionString = "server=192.168.2.64;userid=administrator;password=zlan2017;database=zlan_core;";
                opt.ServiceKey = "DemoWeb1";
            });
            //services.AddCommand(new CommandOptions() {
            //    ConnectionString = "server=192.168.2.64;userid=administrator;password=zlan2017;database=zlan_core;",
            //    ServiceKey = "DemoWeb1"
            //});

            //
            //动态添加当前项目所有接口的依赖注入
            //
            var assembly = typeof(Startup).Assembly;

            //实现了接口ICommandBase接口的添加依赖注入
            var types = assembly.ExportedTypes.Where(x => x.IsClass && x.IsPublic && x.GetInterface("ICommandBase") != null);

            foreach (var type in types)
            {
                services.AddScoped(type, type);
            }


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("api", new Swashbuckle.AspNetCore.Swagger.Info
                {
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "docs";
                c.DefaultModelsExpandDepth(-1);
                c.DocExpansion(DocExpansion.List);
                c.EnableDeepLinking();
                c.ShowExtensions();
                c.EnableValidator();
                c.SwaggerEndpoint("/swagger/api/swagger.json", "api v1.1");
            });
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
    }
}
