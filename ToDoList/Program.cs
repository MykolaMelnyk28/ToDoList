using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoList.DB;
using ToDoList.Servicies;
using ToDoList.Shared.Entity;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllersWithViews();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
string dbVersion = builder.Configuration["DatabaseVersion"];

builder.Services.AddDbContext<ApplicationContext>(options => {
	options.UseMySql(connectionString, new MySqlServerVersion(Version.Parse(dbVersion)));
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
	options.LoginPath = "/Home/Index";
	options.AccessDeniedPath = "/Home/Error";
});
builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<UserEntity>(options =>
{
	options.User.RequireUniqueEmail = true;

	options.Password.RequiredLength = 8;
	options.Password.RequireDigit = true;
	options.Password.RequireNonAlphanumeric = true;
})
	.AddEntityFrameworkStores<ApplicationContext>()
	.AddDefaultTokenProviders();

builder.Services.AddTransient<TaskManager>();

var app = builder.Build();

if(!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
