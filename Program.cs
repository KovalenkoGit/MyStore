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

//��������� ��� ������
builder.Services.Configure<IdentityOptions>(option =>
{
    // ̳������� ������� ������ � 5 �������.
    option.Password.RequiredLength = 5;
    // ������ ���� �������� 1 ��������� ������ � ����� (������, ���� �� ������������).
    option.Password.RequiredUniqueChars = 1;
    // �� ����'������, ��� ������ ����� �����.
    option.Password.RequireDigit = false;
    // �� ����'������, ��� ������ ����� ����� �������� ������� (��� �����).
    option.Password.RequireLowercase = false;
    // �� ����'������, ��� ������ ����� ����� ��������� ������� (����� �����).
    option.Password.RequireUppercase = false;
    // �� ����'������, ��� ������ ����� ��������� ������� (����. !,@,#).
    option.Password.RequireNonAlphanumeric = false;
    //ҳ���� ���������� Email
    option.SignIn.RequireConfirmedEmail = true;
    //��������� ���������� �����������
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    //����������� ������� ����� �������� ������
    option.Lockout.MaxFailedAccessAttempts = 5;

});
// ����� ������ �������� 5 ������. ϳ��� ����� ����� ��� �������� � ����� �� ���� ���� ������������ ��� ��������� �������� ��������(������������ ���������� ����� �� �������� ������)
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

// ������ �������� �������� ��� Areas
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
