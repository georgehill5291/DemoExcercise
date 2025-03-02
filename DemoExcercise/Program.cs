using DemoExcercise.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Context;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        options.EnableSensitiveDataLogging(); // Helps identify conflicting entity IDs
    }
);

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => { options.User.RequireUniqueEmail = true; })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

builder.Services.AddScoped<IBlogService, BlogService>();

// Add MVC services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // ✅ Important for Identity pages

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // ✅ Correct Path
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // ✅ Correct Path
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.MapRazorPages();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// app.UseSession();

// Ensure roles and admin user exist
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await Helper.SeedRolesAndAdminAsync(services);
}

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    Console.WriteLine($"User: {context.User.Identity?.Name}, Authenticated: {context.User.Identity?.IsAuthenticated}");
    await next();
});

app.MapControllerRoute(
    "default",
    "{controller=Blog}/{action=Index}/{id?}");

app.Run();