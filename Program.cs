using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ElCriolloAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();

// ✅ CONFIGURAR ENTITY FRAMEWORK
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ CONFIGURAR CORS (Para que Swagger funcione)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ CONFIGURAR SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "El Criollo API",
        Version = "v1",
        Description = "API para el restaurante El Criollo - Sabor Dominicano Auténtico"
    });
});

var app = builder.Build();

// Configurar el pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "El Criollo API v1");
        c.RoutePrefix = "swagger";
    });
}

// ✅ USAR CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ✅ PROBAR CONEXIÓN A BASE DE DATOS
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.CanConnectAsync();
        Console.WriteLine("✅ Conexión a base de datos exitosa!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error conectando a base de datos: {ex.Message}");
}

Console.WriteLine("🍽️ El Criollo API iniciada exitosamente!");
Console.WriteLine($"🌐 Swagger disponible en: https://localhost:7122/swagger");

app.Run();