// 🍽️ MESAS CONTROLLER CON DTOs - EL CRIOLLO
// Author: Josué Bernabé
// Description: API endpoints for restaurant table management with clean DTOs

using ElCriolloAPI.Data;
using ElCriolloAPI.DTOS;
using ElCriolloAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElCriolloAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MesasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MesasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/mesas
        // Devuelve DTOs limpios sin relaciones complejas
        [HttpGet]
        [Authorize(Roles = "Administrador,Mesero,Recepcionista,Cajero")]
        public async Task<ActionResult<IEnumerable<MesaResponseDto>>> GetMesas()
        {
            try
            {
                var mesas = await _context.Mesas.ToListAsync();

                var mesasDto = mesas.Select(m => new MesaResponseDto
                {
                    MesaId = m.MesaId,
                    NumeroMesas = m.NumeroMesas,
                    Capacidad = m.Capacidad,
                    Ubicacion = m.Ubicacion,
                    Estado = m.Estado,
                    FechaCreacion = m.FechaCreacion,
                    PedidosActivos = 0, // Por ahora 0, después calcularemos
                    TieneReservas = false // Por ahora false, después calcularemos
                }).ToList();

                return Ok(mesasDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al obtener las mesas",
                    error = ex.Message
                });
            }
        }

        // GET: api/mesas/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Mesero,Recepcionista,Cajero")]
        public async Task<ActionResult<MesaResponseDto>> GetMesa(int id)
        {
            try
            {
                var mesa = await _context.Mesas.FindAsync(id);
                if (mesa == null)
                {
                    return NotFound(new
                    {
                        message = $"❌ Mesa con ID {id} no encontrada",
                        mesaId = id
                    });
                }

                var mesaDto = new MesaResponseDto
                {
                    MesaId = mesa.MesaId,
                    NumeroMesas = mesa.NumeroMesas,
                    Capacidad = mesa.Capacidad,
                    Ubicacion = mesa.Ubicacion,
                    Estado = mesa.Estado,
                    FechaCreacion = mesa.FechaCreacion,
                    PedidosActivos = 0,
                    TieneReservas = false
                };

                return Ok(mesaDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al obtener la mesa",
                    error = ex.Message
                });
            }
        }

        // GET: api/mesas/test-connection
        [HttpGet("test-connection")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                var totalMesas = await _context.Mesas.CountAsync();
                var mesasLibres = await _context.Mesas.CountAsync(m => m.Estado == "Libre");
                var mesasOcupadas = await _context.Mesas.CountAsync(m => m.Estado == "Ocupada");
                var mesasReservadas = await _context.Mesas.CountAsync(m => m.Estado == "Reservada");

                return Ok(new
                {
                    message = "✅ Conexión exitosa a Mesas (Solo Administrador)",
                    estadisticas = new
                    {
                        totalMesas,
                        mesasLibres,
                        mesasOcupadas,
                        mesasReservadas
                    },
                    usuario = User.Identity!.Name,
                    rol = User.FindFirst("role")?.Value,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error de conexión",
                    error = ex.Message
                });
            }
        }

        // POST: api/mesas
        // Ahora usa CreateMesaDto - JSON limpio y simple
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<MesaResponseDto>> CreateMesa(CreateMesaDto createMesaDto)
        {
            try
            {
                // Verificar si el número de mesa ya existe
                var mesaExistente = await _context.Mesas
                    .FirstOrDefaultAsync(m => m.NumeroMesas == createMesaDto.NumeroMesas);

                if (mesaExistente != null)
                {
                    return BadRequest(new
                    {
                        message = "❌ Ya existe una mesa con ese número",
                        numeroMesa = createMesaDto.NumeroMesas,
                        mesaExistente = mesaExistente.MesaId
                    });
                }

                // Convertir DTO a Model
                var mesa = new Mesa
                {
                    NumeroMesas = createMesaDto.NumeroMesas,
                    Capacidad = createMesaDto.Capacidad,
                    Ubicacion = createMesaDto.Ubicacion,
                    Estado = createMesaDto.Estado,
                    FechaCreacion = DateTime.Now
                };

                _context.Mesas.Add(mesa);
                await _context.SaveChangesAsync();

                // Devolver DTO en respuesta
                var mesaResponse = new MesaResponseDto
                {
                    MesaId = mesa.MesaId,
                    NumeroMesas = mesa.NumeroMesas,
                    Capacidad = mesa.Capacidad,
                    Ubicacion = mesa.Ubicacion,
                    Estado = mesa.Estado,
                    FechaCreacion = mesa.FechaCreacion,
                    PedidosActivos = 0,
                    TieneReservas = false
                };

                return Ok(new
                {
                    message = "✅ Mesa creada exitosamente por Administrador",
                    mesa = mesaResponse,
                    creadaPor = User.Identity!.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al crear la mesa",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // PUT: api/mesas/5
        // Ahora usa UpdateMesaDto - JSON limpio y simple
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Mesero")]
        public async Task<ActionResult> UpdateMesa(int id, UpdateMesaDto updateMesaDto)
        {
            try
            {
                var mesaExistente = await _context.Mesas.FindAsync(id);

                if (mesaExistente == null)
                {
                    return NotFound(new
                    {
                        message = $"❌ Mesa con ID {id} no encontrada",
                        mesaId = id
                    });
                }

                var userRole = User.FindFirst("role")?.Value;

                // Si es MESERO, solo puede cambiar el estado
                if (userRole == "Mesero")
                {
                    if (mesaExistente.Estado != updateMesaDto.Estado)
                    {
                        mesaExistente.Estado = updateMesaDto.Estado;
                        await _context.SaveChangesAsync();

                        var mesaResponse = new MesaResponseDto
                        {
                            MesaId = mesaExistente.MesaId,
                            NumeroMesas = mesaExistente.NumeroMesas,
                            Capacidad = mesaExistente.Capacidad,
                            Ubicacion = mesaExistente.Ubicacion,
                            Estado = mesaExistente.Estado,
                            FechaCreacion = mesaExistente.FechaCreacion,
                            PedidosActivos = 0,
                            TieneReservas = false
                        };

                        return Ok(new
                        {
                            message = "✅ Estado de mesa actualizado por Mesero",
                            mesa = mesaResponse,
                            modificadoPor = User.Identity!.Name,
                            rol = userRole
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            message = "❌ Meseros solo pueden cambiar el estado de la mesa",
                            permisos = "Para cambiar otros datos necesitas ser Administrador"
                        });
                    }
                }

                // Si es ADMINISTRADOR, puede cambiar todo
                if (userRole == "Administrador")
                {
                    // Validar número duplicado
                    if (mesaExistente.NumeroMesas != updateMesaDto.NumeroMesas)
                    {
                        var numeroExiste = await _context.Mesas
                            .AnyAsync(m => m.NumeroMesas == updateMesaDto.NumeroMesas && m.MesaId != id);

                        if (numeroExiste)
                        {
                            return BadRequest(new
                            {
                                message = "❌ Ya existe otra mesa con ese número",
                                numeroMesa = updateMesaDto.NumeroMesas
                            });
                        }
                    }

                    // Actualizar todas las propiedades
                    mesaExistente.NumeroMesas = updateMesaDto.NumeroMesas;
                    mesaExistente.Capacidad = updateMesaDto.Capacidad;
                    mesaExistente.Ubicacion = updateMesaDto.Ubicacion;
                    mesaExistente.Estado = updateMesaDto.Estado;

                    await _context.SaveChangesAsync();

                    var mesaResponse = new MesaResponseDto
                    {
                        MesaId = mesaExistente.MesaId,
                        NumeroMesas = mesaExistente.NumeroMesas,
                        Capacidad = mesaExistente.Capacidad,
                        Ubicacion = mesaExistente.Ubicacion,
                        Estado = mesaExistente.Estado,
                        FechaCreacion = mesaExistente.FechaCreacion,
                        PedidosActivos = 0,
                        TieneReservas = false
                    };

                    return Ok(new
                    {
                        message = "✅ Mesa actualizada completamente por Administrador",
                        mesa = mesaResponse,
                        modificadoPor = User.Identity!.Name,
                        rol = userRole
                    });
                }

                return Forbid("❌ Rol no autorizado para esta operación");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al actualizar la mesa",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // DELETE: api/mesas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> DeleteMesa(int id)
        {
            try
            {
                var mesa = await _context.Mesas.FindAsync(id);

                if (mesa == null)
                {
                    return NotFound(new
                    {
                        message = $"❌ Mesa con ID {id} no encontrada",
                        mesaId = id
                    });
                }

                var tienePedidosActivos = await _context.Pedidos
                    .AnyAsync(p => p.MESAID == id && p.Estado == "Activo");

                if (tienePedidosActivos)
                {
                    return BadRequest(new
                    {
                        message = "❌ No se puede eliminar la mesa porque tiene pedidos activos",
                        mesaId = id,
                        numeroMesa = mesa.NumeroMesas
                    });
                }

                if (mesa.Estado == "Ocupada" || mesa.Estado == "Reservada")
                {
                    return BadRequest(new
                    {
                        message = $"❌ No se puede eliminar la mesa porque está {mesa.Estado.ToLower()}",
                        mesaId = id,
                        numeroMesa = mesa.NumeroMesas,
                        estado = mesa.Estado
                    });
                }

                _context.Mesas.Remove(mesa);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "✅ Mesa eliminada exitosamente por Administrador",
                    mesaEliminada = new
                    {
                        mesaId = mesa.MesaId,
                        numeroMesa = mesa.NumeroMesas,
                        capacidad = mesa.Capacidad,
                        ubicacion = mesa.Ubicacion
                    },
                    eliminadaPor = User.Identity!.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al eliminar la mesa",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }
    }
}