using HorsesForCourses.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register repositories and services
builder.Services.AddScoped<ICoachRepository, CoachRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<ICoachService, CoachService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IAccountService, AccountService>();


// Configure EF Core with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(@"Data Source=C:\Users\becod\horses-for-courses\HorsesForCourses.WebApi\app.db"));

builder.Services.AddAuthentication("Cookies")
.AddCookie("Cookies", o => { o.LoginPath = "/Account/Login"; });
builder.Services.AddAuthorization();

var app = builder.Build();

// Apply migrations automatically at startup (development only)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
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

// Standard MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
