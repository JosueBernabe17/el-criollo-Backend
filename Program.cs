using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ElCriolloAPI.Data;
using ElCriolloAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();

// ✅ CONFIGURAR ENTITY FRAMEWORK
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//CONFIGURAR EMAIL SERVICE
builder.Services.AddScoped<IEmailService, EmailService>();
// ✅ CONFIGURAR JWT AUTHENTICATION
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Solo para desarrollo
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ✅ CONFIGURAR CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ CONFIGURAR SWAGGER CON JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "El Criollo API",
        Version = "v1",
        Description = "API para el restaurante El Criollo con autenticación JWT"
    });

    // Configuración para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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

// ✅ ORDEN IMPORTANTE DEL MIDDLEWARE
app.UseCors("AllowAll");
app.UseHttpsRedirection();

// ✅ AGREGAR AUTHENTICATION Y AUTHORIZATION
app.UseAuthentication(); // ← Debe ir ANTES de UseAuthorization
app.UseAuthorization();

app.MapControllers();

// Probar conexión a base de datos
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        await context.Database.CanConnectAsync();
        Console.WriteLine("✅ Conexión a base de datos exitosa!");
        Console.WriteLine("📧 EmailService configurado y listo!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error conectando a base de datos: {ex.Message}");
}

Console.WriteLine(" El Criollo API iniciada exitosamente!");
Console.WriteLine(" JWT Authentication configurado");
Console.WriteLine($"🌐 Swagger disponible en: https://localhost:7122/swagger");

app.Run();