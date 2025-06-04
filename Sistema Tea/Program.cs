using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Models.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar conexión a SQL Server con EF
builder.Services.AddDbContext<TeaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbTea")));

// Add controllers with views
builder.Services.AddControllersWithViews();

// Habilitar uso de sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Habilitar uso de cookies (necesario para sesión)
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Importante: esto antes de UseAuthorization si vas a usar autenticación personalizada

app.UseAuthorization();

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TestInicio}/{action=Index}/{id?}");

app.Run();
