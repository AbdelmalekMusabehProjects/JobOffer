using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using JobOffer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobOffer.GeneralComponent;
using AspNetCoreHero.ToastNotification;

namespace JobOffer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddDbContext<ModelContext>(options => options.UseOracle(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(60); });
            services.AddNotyf(config =>
            {
                config.DurationInSeconds = 10;
                config.IsDismissable = true;
                config.Position = NotyfPosition.BottomRight;
            }
);
            services.AddSignalR();
            services.AddRazorPages();

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=GuestUser}/{action=Home}/{id?}");

                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
