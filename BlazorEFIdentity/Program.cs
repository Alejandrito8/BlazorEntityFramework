using BlazorEFIdentity.Components;
using BlazorEFIdentity.Components.Account;
using BlazorEFIdentity.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
 
namespace BlazorEFIdentity
{
public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
 
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
 
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
 
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
                .AddIdentityCookies();
 
            builder.Services.AddScoped<BankServices>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=sysdb.db"));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
 
            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
 
 
            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
 
            var app = builder.Build();
 
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
 
            app.UseHttpsRedirection();
 
            app.UseStaticFiles();
            app.UseAntiforgery();
 
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
 
 
            app.MapAdditionalIdentityEndpoints();
 
            using (var scope = app.Services.CreateScope())
            {
                var roleManager =
                    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "User" };
 
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }
 
            using (var scope = app.Services.CreateScope())
            {
 
                var userManager =
                    scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                string email = "admin@admin.se";
                string password = "Abc123!";
 
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new ApplicationUser();
                    user.UserName = email;
                    user.Email = email;
 
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            app.Run();
        }
    }
}