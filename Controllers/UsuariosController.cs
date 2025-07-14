using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElCriolloAPI.Data;
using ElCriolloAPI.Models;

namespace ElCriolloAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // Endpoint especial para probar la conexión a base de datos
        [HttpGet("test-connection")]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                // Intentar conectar a la base de datos
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    // Contar usuarios en la base de datos
                    var userCount = await _context.Usuarios.CountAsync();

                    return Ok(new
                    {
                        message = "✅ Conexión exitosa a El Criollo Database",
                        connected = true,
                        totalUsuarios = userCount,
                        database = "ElCriolloRestaurante",
                        timestamp = DateTime.Now
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "❌ No se pudo conectar a la base de datos",
                        connected = false
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error de conexión",
                    error = ex.Message,
                    connected = false
                });

            }
        }
    }
}
