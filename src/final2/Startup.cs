using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Toys.DAL;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Toys.HelperClasses;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.OptionsModel;

namespace Toys
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            string baseDir = appEnv.ApplicationBasePath;
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(baseDir, "App_Data"));
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<FinalContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<Toys.Models.User, IdentityRole>()
                .AddEntityFrameworkStores<FinalContext>();

            // Add framework services.
            services.AddMvc();

            services.Configure<MvcOptions>(options =>
            {
                options.ModelBinders.Insert(0, new DateTimeModelBinder());
            });

            services.AddCaching();
            services.AddSession(configure: s => s.IdleTimeout = TimeSpan.FromMinutes(30));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ConfiguteAuth(app);

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        private void ConfiguteAuth(IApplicationBuilder app)
        {
            app.UseIdentity();

            var options = app.ApplicationServices.GetRequiredService<IOptions<IdentityOptions>>().Value;

            options.Cookies.ApplicationCookie.LoginPath = new PathString("/auth/login");

            app.UseCookieAuthentication(options.Cookies.ApplicationCookie);

            var roleManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();

            //app.UseCookieAuthentication(options.Cookies.ExternalCookie);

            //app.UseGoogleAuthentication(new Microsoft.AspNet.Authentication.Google.GoogleOptions
            //{
            //    ClientId = "1063540966140-570hdvnhdf92d4ebq4rv7chl81epghoa.apps.googleusercontent.com",
            //    ClientSecret = "5x9QBOt-yt9OCdL-Tnld-lH4"
            //});
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
