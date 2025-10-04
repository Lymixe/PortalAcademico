using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;

var builder = WebApplication.CreateBuilder(args);

// --- SECCIÓN DE SERVICIOS ---

// 1. Conexión a la Base de Datos SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// 2. Configuración de Redis para Caché
builder.Services.AddStackExchangeRedisCache(options =>
{
    // VOLVEMOS AL MÉTODO ORIGINAL Y MÁS SEGURO DE LEER LA CONFIGURACIÓN DE REDIS
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "PortalAcademico_";
});

// 3. Configuración de Sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Configuración de Identity con soporte para Roles y Tokens
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); 

// 5. Añadir servicios para controladores MVC y Razor Pages (para Identity)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// --- SECCIÓN DE MIDDLEWARE (PIPELINE DE PETICIONES) ---

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); 

app.Run();