using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace voiceapi
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
            // 1. Add Authentication Services
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = "https://slovoio.eu.auth0.com/";
                options.Audience = "http://slovo.io/api";
            });
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors("AllowAll");
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "link",
                    template: "link/{id?}", 
                    defaults: new { controller = "Home", action = "Link" });
                routes.MapRoute(
                    name: "shortshowlink",
                    template: "s/{id?}",
                    defaults: new { controller = "Home", action = "Show" });
                routes.MapRoute(
                    name: "shortlink",
                    template: "l/{id?}", 
                    defaults: new { controller = "Home", action = "Link" });
                routes.MapRoute(
                    name: "audioredirect",
                    template: "a/{fileName}.{fileExtensionWithoutDot}", 
                    defaults: new { controller = "Home", action = "Audio" });
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "all",
                    template: "{*url}",
                    defaults: new { controller = "Home", action = "All" });
    
            });
        }
    }
}
