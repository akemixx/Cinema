using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using CinemaA.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CinemaA.Settings;
using CinemaA.Services;
using CinemaA.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace CinemaA
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                })
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    );
            services.AddRazorPages();
            // Token for getting AJAX requests.
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            // Configure settings for email sending.
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<User>(Configuration.GetSection("AdminUserForTesting"));
            // Add service for email senging.
            services.AddTransient<IMailService, MailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IOptions<User> _adminUser)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Create an "Admin" role if it doesn't exist in the DB.
            try
            {
                if (!roleManager.RoleExistsAsync("Admin").Result)
                    roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
            }
            catch { }

            // Create a user with admin role using data from configuration if it doesn't exist.
            try
            {
                User adminUser = _adminUser.Value;
                string password = Configuration.GetValue<string>("DefaultPassword");
                if (userManager.FindByEmailAsync(adminUser.Email).Result == null)
                {
                    var user = new User { UserName = adminUser.Email, Email = adminUser.Email, RealName = adminUser.RealName };
                    userManager.CreateAsync(user, password).Wait();
                    userManager.AddToRoleAsync(user, "Admin");
                }
            }
            catch { }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
