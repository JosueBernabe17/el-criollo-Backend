using ElCriolloAPI.Data;
using ElCriolloAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElCriolloAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Constructor - recibe contexto y configuración
        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/auth/test-connection
        // Endpoint para probar que el controller funciona
        [HttpPost("test-connection")]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                var totalUsuarios = await _context.Usuarios.CountAsync();

                return Ok(new
                {
                    message = "✅ AuthController funcionando correctamente",
                    totalUsuarios = totalUsuarios,
                    jwtConfigured = !string.IsNullOrEmpty(_configuration["Jwt:Key"]),
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error en AuthController",
                    error = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                //  Valida que lleguen email y contraseña
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new
                    {
                        message = "❌ Email y contraseña son requeridos",
                        success = false
                    });
                }

                // Buscar usuario por email
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (usuario == null)
                {
                    return BadRequest(new
                    {
                        message = "❌ Email no encontrado",
                        success = false
                    });
                }

                //  Validar contraseña 
                if (usuario.Contraseña != request.Password)
                {
                    return BadRequest(new
                    {
                        message = "❌ Contraseña incorrecta",
                        success = false
                    });
                }

                //  Validar que el usuario esté activo
                if (!usuario.Activo)
                {
                    return BadRequest(new
                    {
                        message = "❌ Usuario inactivo",
                        success = false
                    });
                }

                //  Generar token JWT
                var token = GenerateJwtToken(usuario);

                //  Devolver respuesta exitosa
                return Ok(new
                {
                    message = "✅ Login exitoso",
                    success = true,
                    token = token,
                    user = new
                    {
                        id = usuario.UsuarioId,
                        nombreCompleto = usuario.NombreCompleto,
                        email = usuario.Email,
                        rol = usuario.Rol
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error interno en login",
                    error = ex.Message,
                    success = false
                });
            }
        }

        // Método privado para generar token JWT
        private string GenerateJwtToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("userId", usuario.UsuarioId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpiryInHours"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Clase para recibir datos de login
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }

}
