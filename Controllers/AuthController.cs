using ElCriolloAPI.Data;
using ElCriolloAPI.Models;
using ElCriolloAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
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
        private readonly IEmailService _emailService;

        // Constructor - recibe contexto y configuración
        public AuthController(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        // Probar email de bienvenida
        [HttpPost("test-email-welcome")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> TestWelcomeEmail([FromBody] TestEmailRequest request)
        {
            try
            {
                var emailSent = await _emailService.SendWelcomeEmailAsync(
                    request.Email,
                    request.NombreCompleto
                );

                if (emailSent)
                {
                    return Ok(new
                    {
                        message = "✅ Email de bienvenida enviado exitosamente",
                        emailDestinatario = request.Email,
                        nombreCompleto = request.NombreCompleto,
                        tipoEmail = "Bienvenida",
                        timestamp = DateTime.Now,
                        enviadorPor = User.Identity!.Name
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "❌ Error enviando email de bienvenida",
                        emailDestinatario = request.Email
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error interno enviando email",
                    error = ex.Message
                });
            }
        }

        // POST: api/auth/test-email-pedido
        // Probar email de confirmación de pedido
        [HttpPost("test-email-pedido")]
        [Authorize(Roles = "Administrador,Mesero")]
        public async Task<ActionResult> TestPedidoEmail([FromBody] TestPedidoEmailRequest request)
        {
            try
            {
                var emailSent = await _emailService.SendPedidoConfirmationAsync(
                    request.Email,
                    request.NombreCliente,
                    request.PedidoId
                );

                if (emailSent)
                {
                    return Ok(new
                    {
                        message = "✅ Email de confirmación de pedido enviado",
                        emailDestinatario = request.Email,
                        nombreCliente = request.NombreCliente,
                        pedidoId = request.PedidoId,
                        tipoEmail = "Confirmación Pedido",
                        timestamp = DateTime.Now
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "❌ Error enviando confirmación de pedido"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error interno",
                    error = ex.Message
                });
            }
        }

        // POST: api/auth/test-email-admin
        // Probar notificación para administradores
        [HttpPost("test-email-admin")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> TestAdminNotification([FromBody] TestAdminEmailRequest request)
        {
            try
            {
                var emailSent = await _emailService.SendAdminNotificationAsync(
                    request.Message,
                    request.Subject
                );

                if (emailSent)
                {
                    return Ok(new
                    {
                        message = "✅ Notificación admin enviada exitosamente",
                        subject = request.Subject,
                        contenido = request.Message,
                        tipoEmail = "Notificación Admin",
                        timestamp = DateTime.Now,
                        enviadoPor = User.Identity!.Name
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "❌ Error enviando notificación admin"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error interno",
                    error = ex.Message
                });
            }
        }

        // POST: api/auth/test-email-reserva
        // Probar email de confirmación de reserva
        [HttpPost("test-email-reserva")]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<ActionResult> TestReservaEmail([FromBody] TestReservaEmailRequest request)
        {
            try
            {
                var emailSent = await _emailService.SendReservaConfirmationAsync(
                    request.Email,
                    request.NombreCliente,
                    request.FechaReserva
                );

                if (emailSent)
                {
                    return Ok(new
                    {
                        message = "✅ Email de confirmación de reserva enviado",
                        emailDestinatario = request.Email,
                        nombreCliente = request.NombreCliente,
                        fechaReserva = request.FechaReserva,
                        tipoEmail = "Confirmación Reserva",
                        timestamp = DateTime.Now
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "❌ Error enviando confirmación de reserva"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error interno",
                    error = ex.Message
                });
            }
        }

        // POST: api/auth/test-email-mesa
        // Probar notificación de cambio de estado de mesa
        [HttpPost("test-email-mesa")]
        [Authorize(Roles = "Administrador,Mesero")]
        public async Task<ActionResult> TestMesaStatusEmail([FromBody] TestMesaEmailRequest request)
        {
            try
            {
                var emailSent = await _emailService.SendMesaStatusChangeAsync(
                    request.MesaNumero,
                    request.NuevoEstado
                );

                if (emailSent)
                {
                    return Ok(new
                    {
                        message = "✅ Notificación de cambio de mesa enviada",
                        mesaNumero = request.MesaNumero,
                        nuevoEstado = request.NuevoEstado,
                        tipoEmail = "Cambio Estado Mesa",
                        timestamp = DateTime.Now,
                        enviadoPor = User.Identity!.Name
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "❌ Error enviando notificación de mesa"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error interno",
                    error = ex.Message
                });
            }
        }

        // 📧 DTOs para los endpoints de email
        public class TestEmailRequest
        {
            public string Email { get; set; } = string.Empty;
            public string NombreCompleto { get; set; } = string.Empty;
        }

        public class TestPedidoEmailRequest
        {
            public string Email { get; set; } = string.Empty;
            public string NombreCliente { get; set; } = string.Empty;
            public int PedidoId { get; set; }
        }

        public class TestAdminEmailRequest
        {
            public string Message { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
        }

        public class TestReservaEmailRequest
        {
            public string Email { get; set; } = string.Empty;
            public string NombreCliente { get; set; } = string.Empty;
            public DateTime FechaReserva { get; set; }
        }

        public class TestMesaEmailRequest
        {
            public int MesaNumero { get; set; }
            public string NuevoEstado { get; set; } = string.Empty;
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


        // POST: api/auth/register
        // Registro de usuario con email automático de bienvenida
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                //  Validaciones básicas
                if (string.IsNullOrEmpty(request.NombreCompleto) ||
                    string.IsNullOrEmpty(request.Email) ||
                    string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new
                    {
                        message = "❌ Todos los campos son requeridos",
                        success = false
                    });
                }

                //  Validar formato de email
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest(new
                    {
                        message = "❌ Formato de email inválido",
                        success = false
                    });
                }

                //  Verificar si el email ya existe
                var usuarioExistente = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (usuarioExistente != null)
                {
                    return BadRequest(new
                    {
                        message = "❌ Ya existe un usuario con este email",
                        email = request.Email,
                        success = false
                    });
                }

                //  Crear nuevo usuario
                var nuevoUsuario = new Usuario
                {
                    NombreCompleto = request.NombreCompleto.Trim(),
                    Email = request.Email.ToLower().Trim(),
                    Contraseña = request.Password, // En producción: hash la contraseña
                    Rol = request.Rol ?? "Mesero", // Rol por defecto
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                // Guardar en base de datos
                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                //  ENVIAR EMAIL DE BIENVENIDA AUTOMÁTICAMENTE
                var emailEnviado = await _emailService.SendWelcomeEmailAsync(
                    nuevoUsuario.Email,
                    nuevoUsuario.NombreCompleto
                );

                //  Generar token JWT para login automático
                var token = GenerateJwtToken(nuevoUsuario);

                //  Respuesta exitosa con información del email
                return Ok(new
                {
                    message = "✅ Usuario registrado exitosamente",
                    success = true,
                    usuario = new
                    {
                        id = nuevoUsuario.UsuarioId,
                        nombreCompleto = nuevoUsuario.NombreCompleto,
                        email = nuevoUsuario.Email,
                        rol = nuevoUsuario.Rol,
                        activo = nuevoUsuario.Activo,
                        fechaCreacion = nuevoUsuario.FechaCreacion
                    },
                    token = token,
                    emailNotificacion = new
                    {
                        enviado = emailEnviado,
                        tipoEmail = "Bienvenida",
                        destinatario = nuevoUsuario.Email,
                        mensaje = emailEnviado ?
                            "📧 Email de bienvenida enviado exitosamente" :
                            "⚠️ Usuario creado pero email no se pudo enviar"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error interno durante el registro",
                    error = ex.Message,
                    success = false
                });
            }
        }

        // Método auxiliar para validar email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // DTO para el registro
        // DTO para el registro CON ROL CLIENTE
        public class RegisterRequest
        {
            public string NombreCompleto { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;

            [RegularExpression("^(Administrador|Mesero|Recepcionista|Cajero|Cliente)$",
                ErrorMessage = "El rol debe ser: Administrador, Mesero, Recepcionista, Cajero o Cliente")]
            public string? Rol { get; set; } = "Cliente"; 
        }

    }

}
