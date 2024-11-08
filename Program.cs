using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Models;
using MyStore.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
#if DEBUG
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
#endif
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

//Параметри для паролів
builder.Services.Configure<IdentityOptions>(option =>
{
    // Мінімальна довжина пароля — 5 символів.
    option.Password.RequiredLength = 5;
    // Вимога мати принаймні 1 унікальний символ у паролі (символ, який не повторюється).
    option.Password.RequiredUniqueChars = 1;
    // Не обов'язково, щоб пароль містив цифру.
    option.Password.RequireDigit = false;
    // Не обов'язково, щоб пароль містив літери нижнього регістру (малі літери).
    option.Password.RequireLowercase = false;
    // Не обов'язково, щоб пароль містив літери верхнього регістру (великі літери).
    option.Password.RequireUppercase = false;
    // Не обов'язково, щоб пароль містив спеціальні символи (напр. !,@,#).
    option.Password.RequireNonAlphanumeric = false;
    //Тільки підтверджені Email
    option.SignIn.RequireConfirmedEmail = true;
    //Тривалість блокування користувача
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    //Максимальна кількість спроб введення пароля
    option.Lockout.MaxFailedAccessAttempts = 5;

});
// Токен дійсним протягом 5 хвилин. Після цього токен стає недійсним і більше не може бути використаний для виконання відповідної операції(ідтвердження електронної пошти чи скидання пароля)
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(5);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Додаємо підтримку маршрутів для Areas
app.UseEndpoints(endpoints =>
{
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    
    endpoints.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
});
app.MapRazorPages();

app.Run();
