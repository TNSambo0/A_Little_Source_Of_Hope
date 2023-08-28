using Microsoft.EntityFrameworkCore;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AccountDbContextConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(Options =>
{
    Options.Cookie.Name = ".A_Little_Source_Of_Hope.Session";
    Options.IdleTimeout = TimeSpan.FromMinutes(10);
    Options.Cookie.IsEssential = true;
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
});
// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Customers", policy => policy.RequireRole("Customer", "Orphanage Manager"));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator", "Orphanage Administrator",
        "Volunteer Administrator", "Category Administrator", "Product Administrator"));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Users", policy => policy.RequireRole("Administrator", "Orphanage Administrator",
        "Volunteer Administrator", "Category Administrator", "Product Administrator", "Customer", "Orphanage Manager"));
});
// Authorization handlers
builder.Services.AddSingleton<IAuthorizationHandler, AdministratorAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CategoryAdministratorAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, OrphanageAdministratorAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, PaymentAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ProductAdministratorAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ShoppingCartCustomerAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, VolunteerAdministratorAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, VolunteerCustomerAuthorizationHandler>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.ConfigureApplicationCookie(o =>
{
    o.ExpireTimeSpan = TimeSpan.FromDays(5);
    o.SlidingExpiration = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

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
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
