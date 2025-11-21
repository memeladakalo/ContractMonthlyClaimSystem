using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Services;
using ContractMonthlyClaimSystem.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);






    
// ------------------------
//  DATABASE + IDENTITY
// ------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddTransient<IEmailService, MailKitEmailService>();

builder.Services.AddTransient<IEmailService, EmailService>();
// Redirect unauthenticated users
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Email Service
builder.Services.AddTransient<IEmailService, EmailService>();

// MVC + Validation
builder.Services.AddControllersWithViews()
       .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

// ------------------------
//  BUILD APP
// ------------------------
var app = builder.Build();


// ------------------------
//  SEED DEFAULT ROLES/USERS
// ------------------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await IdentitySeeder.SeedRolesAndAdmin(userManager, roleManager);
    string[] roles = { "Lecturer", "Coordinator", "Manager", "HR" };

    // Create system roles
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Seed default Manager user
    var managerEmail = "manager@college.com";
    var manager = await userManager.FindByEmailAsync(managerEmail);

    if (manager == null)
    {
        var defaultManager = new ApplicationUser
        {
            UserName = managerEmail,
            Email = managerEmail,
            FullName = "System Manager"
        };

        await userManager.CreateAsync(defaultManager, "Password123!");
        await userManager.AddToRoleAsync(defaultManager, "Manager");
    }
}

// ------------------------
//  MIDDLEWARE
// ------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// DEFAULT ROUTE → LOGIN
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
