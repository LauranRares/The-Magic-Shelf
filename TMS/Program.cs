using Microsoft.EntityFrameworkCore;
using TMS.Data;
using TMS.Models;
using Microsoft.AspNetCore.Identity;
using TMS.DbInitialize;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TMSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

//builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<TMSDbContext>();

builder.Services.AddIdentity<UserDB, IdentityRole>().AddDefaultTokenProviders()
   .AddEntityFrameworkStores<TMSDbContext>();

builder.Services.AddScoped<DbInitialize>();

builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

SeedDataBase();
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Guest}/{controller=GuestViews}/{action=Home}/{id?}");

app.Run();

void SeedDataBase()
{
    using(var scope =app.Services.CreateScope())
    {
        var init=scope.ServiceProvider.GetRequiredService<DbInitialize>();
        init.Initialize();
    }
}