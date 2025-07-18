using ElCriolloAPI.Data;
using ElCriolloAPI.DTOS;
using ElCriolloAPI.Models;
using ElCriolloAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElCriolloAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public PedidosController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: api/pedidos
        [HttpGet]
        [Authorize(Roles = "Administrador,Mesero,Recepcionista,Cajero")]
        public async Task<ActionResult> GetPedidos(
            [FromQuery] string? estado = null,
            [FromQuery] int? mesaId = null,
            [FromQuery] int? usuarioId = null)
        {
            try
            {
                var query = _context.Pedidos
                    .Include(p => p.Mesa)
                    .Include(p => p.Usuario)
                    .Include(p => p.DetallePedidos)
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(estado))
                {
                    query = query.Where(p => p.Estado == estado);
                }

                if (mesaId.HasValue)
                {
                    query = query.Where(p => p.MESAID == mesaId.Value);
                }

                if (usuarioId.HasValue)
                {
                    query = query.Where(p => p.UsuarioId == usuarioId.Value);
                }

                var pedidos = await query
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToListAsync();

                var pedidosSimplificados = pedidos.Select(p => new
                {
                    pedidoId = p.PedidoID,
                    mesaId = p.MESAID,
                    numeroMesa = p.Mesa?.NumeroMesas ?? 0,
                    usuarioId = p.UsuarioId,
                    nombreUsuario = p.Usuario?.NombreCompleto ?? "Usuario no encontrado",
                    estado = p.Estado,
                    fechaPedido = p.FechaPedido,
                    fechaCreacion = p.FechaCreacion,
                    totalProductos = p.DetallePedidos.Count
                }).ToList();

                return Ok(new
                {
                    message = "✅ Pedidos obtenidos exitosamente",
                    totalPedidos = pedidosSimplificados.Count,
                    filtros = new
                    {
                        estado = estado ?? "Todos",
                        mesaId = mesaId?.ToString() ?? "Todas",
                        usuarioId = usuarioId?.ToString() ?? "Todos"
                    },
                    pedidos = pedidosSimplificados,
                    consultadoPor = User.Identity!.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al obtener pedidos",
                    error = ex.Message
                });
            }
        }

        // GET: api/pedidos/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Mesero,Recepcionista,Cajero")]
        public async Task<ActionResult> GetPedido(int id)
        {
            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.Mesa)
                    .Include(p => p.Usuario)
                    .Include(p => p.DetallePedidos)
                    .FirstOrDefaultAsync(p => p.PedidoID == id);

                if (pedido == null)
                {
                    return NotFound(new
                    {
                        message = $"❌ Pedido con ID {id} no encontrado",
                        pedidoId = id
                    });
                }

                var pedidoDetallado = new
                {
                    pedidoId = pedido.PedidoID,
                    mesaId = pedido.MESAID,
                    numeroMesa = pedido.Mesa?.NumeroMesas ?? 0,
                    usuarioId = pedido.UsuarioId,
                    nombreUsuario = pedido.Usuario?.NombreCompleto ?? "Usuario no encontrado",
                    estado = pedido.Estado,
                    fechaPedido = pedido.FechaPedido,
                    fechaCreacion = pedido.FechaCreacion,
                    totalProductos = pedido.DetallePedidos.Count,
                    mesa = new
                    {
                        mesaId = pedido.Mesa?.MesaId ?? 0,
                        numeroMesa = pedido.Mesa?.NumeroMesas ?? 0,
                        estadoMesa = pedido.Mesa?.Estado ?? ""
                    },
                    usuario = new
                    {
                        usuarioId = pedido.Usuario?.UsuarioId ?? 0,
                        nombreCompleto = pedido.Usuario?.NombreCompleto ?? "",
                        email = pedido.Usuario?.Email ?? "",
                        rol = pedido.Usuario?.Rol ?? ""
                    }
                };

                return Ok(pedidoDetallado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al obtener el pedido",
                    error = ex.Message
                });
            }
        }

        // GET: api/pedidos/mesa/5
        [HttpGet("mesa/{mesaId}")]
        [Authorize(Roles = "Administrador,Mesero,Recepcionista")]
        public async Task<ActionResult> GetPedidosPorMesa(int mesaId)
        {
            try
            {
                var mesa = await _context.Mesas.FindAsync(mesaId);
                if (mesa == null)
                {
                    return NotFound(new
                    {
                        message = $"❌ Mesa con ID {mesaId} no encontrada",
                        mesaId = mesaId
                    });
                }

                var pedidos = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .Include(p => p.DetallePedidos)
                    .Where(p => p.MESAID == mesaId)
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToListAsync();

                var pedidosMesa = pedidos.Select(p => new
                {
                    pedidoId = p.PedidoID,
                    usuarioId = p.UsuarioId,
                    nombreUsuario = p.Usuario?.NombreCompleto ?? "",
                    estado = p.Estado,
                    fechaPedido = p.FechaPedido,
                    totalProductos = p.DetallePedidos.Count
                }).ToList();

                return Ok(new
                {
                    message = $"✅ Pedidos de la mesa #{mesa.NumeroMesas} obtenidos",
                    mesa = new
                    {
                        mesaId = mesa.MesaId,
                        numeroMesa = mesa.NumeroMesas,
                        estado = mesa.Estado
                    },
                    totalPedidos = pedidosMesa.Count,
                    pedidos = pedidosMesa
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al obtener pedidos de la mesa",
                    error = ex.Message
                });
            }
        }

        // POST: api/pedidos
        [HttpPost]
        [Authorize(Roles = "Administrador,Mesero,Recepcionista")]
        public async Task<ActionResult> CreatePedido(CreatePedidoDto createPedidoDto)
        {
            try
            {
                // Validar que la mesa existe
                var mesa = await _context.Mesas.FindAsync(createPedidoDto.MesaId);
                if (mesa == null)
                {
                    return NotFound(new
                    {
                        message = "❌ Mesa no encontrada",
                        mesaId = createPedidoDto.MesaId
                    });
                }

                // Validar que el usuario existe
                var usuario = await _context.Usuarios.FindAsync(createPedidoDto.UsuarioId);
                if (usuario == null)
                {
                    return NotFound(new
                    {
                        message = "❌ Usuario no encontrado",
                        usuarioId = createPedidoDto.UsuarioId
                    });
                }

                // Crear el pedido (sin productos por ahora, enfoque simplificado)
                var nuevoPedido = new Pedido
                {
                    MESAID = createPedidoDto.MesaId,
                    UsuarioId = createPedidoDto.UsuarioId,
                    ProductosID = createPedidoDto.Productos.First().ProductosId,
                    Estado = "Facturado",
                    Total = 0.00m,
                    FechaPedido = DateTime.Now,
                    FechaCreacion = DateTime.Now
                };

                _context.Pedidos.Add(nuevoPedido);
                await _context.SaveChangesAsync();

                // Cambiar estado de la mesa a Ocupada
                mesa.Estado = "Ocupada";
                await _context.SaveChangesAsync();

                // 📧 ENVIAR EMAIL AUTOMÁTICO
                var emailEnviado = await _emailService.SendPedidoConfirmationAsync(
                    usuario.Email,
                    usuario.NombreCompleto,
                    nuevoPedido.PedidoID
                );

                return Ok(new
                {
                    message = "✅ Pedido creado exitosamente",
                    pedido = new
                    {
                        pedidoId = nuevoPedido.PedidoID,
                        mesaId = nuevoPedido.MESAID,
                        numeroMesa = mesa.NumeroMesas,
                        usuarioId = nuevoPedido.UsuarioId,
                        nombreUsuario = usuario.NombreCompleto,
                        estado = nuevoPedido.Estado,
                        fechaPedido = nuevoPedido.FechaPedido
                    },
                    emailNotificacion = new
                    {
                        enviado = emailEnviado,
                        destinatario = usuario.Email,
                        mensaje = emailEnviado ?
                            "📧 Email de confirmación enviado al cliente" :
                            "⚠️ Pedido creado pero email no se pudo enviar"
                    },
                    mesaActualizada = new
                    {
                        mesaId = mesa.MesaId,
                        numeroMesa = mesa.NumeroMesas,
                        nuevoEstado = mesa.Estado
                    },
                    creadoPor = User.Identity!.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al crear el pedido",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // PUT: api/pedidos/5/estado
        [HttpPut("{id}/estado")]
        [Authorize(Roles = "Administrador,Mesero")]
        public async Task<ActionResult> UpdatePedidoEstado(int id, UpdatePedidoEstadoDto updateEstadoDto)
        {
            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .Include(p => p.Mesa)
                    .FirstOrDefaultAsync(p => p.PedidoID == id);

                if (pedido == null)
                {
                    return NotFound(new
                    {
                        message = $"❌ Pedido con ID {id} no encontrado",
                        pedidoId = id
                    });
                }

                var estadoAnterior = pedido.Estado;
                pedido.Estado = updateEstadoDto.Estado;

                // Si el pedido se marca como Entregado, liberar la mesa
                if (updateEstadoDto.Estado == "Entregado" && pedido.Mesa != null)
                {
                    pedido.Mesa.Estado = "Libre";
                }

                await _context.SaveChangesAsync();

                // 📧 ENVIAR EMAIL AUTOMÁTICO
                var emailEnviado = false;
                if (pedido.Usuario != null)
                {
                    var mensajeEmail = GetMensajeSegunEstado(updateEstadoDto.Estado, pedido.PedidoID);
                    emailEnviado = await _emailService.SendAdminNotificationAsync(
                        mensajeEmail,
                        $"Pedido #{pedido.PedidoID} - {updateEstadoDto.Estado}"
                    );
                }

                return Ok(new
                {
                    message = $"✅ Estado del pedido actualizado a '{updateEstadoDto.Estado}'",
                    pedidoId = pedido.PedidoID,
                    estadoAnterior = estadoAnterior,
                    nuevoEstado = pedido.Estado,
                    emailNotificacion = new
                    {
                        enviado = emailEnviado,
                        destinatario = pedido.Usuario?.Email ?? "",
                        tipoNotificacion = "Cambio de estado de pedido"
                    },
                    mesaActualizada = updateEstadoDto.Estado == "Entregado" && pedido.Mesa != null ? new
                    {
                        mesaId = pedido.Mesa.MesaId,
                        numeroMesa = pedido.Mesa.NumeroMesas,
                        nuevoEstado = pedido.Mesa.Estado
                    } : null,
                    actualizadoPor = User.Identity!.Name,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al actualizar estado del pedido",
                    error = ex.Message
                });
            }
        }

        // DELETE: api/pedidos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> CancelarPedido(int id)
        {
            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.Mesa)
                    .Include(p => p.Usuario)
                    .Include(p => p.DetallePedidos)
                    .FirstOrDefaultAsync(p => p.PedidoID == id);

                if (pedido == null)
                {
                    return NotFound(new
                    {
                        message = $"❌ Pedido con ID {id} no encontrado",
                        pedidoId = id
                    });
                }

                if (pedido.Estado == "Entregado")
                {
                    return BadRequest(new
                    {
                        message = "❌ No se puede cancelar un pedido ya entregado",
                        pedidoId = id,
                        estadoActual = pedido.Estado
                    });
                }

                // Cambiar estado a Cancelado
                pedido.Estado = "Cancelado";

                // Liberar la mesa si estaba ocupada
                if (pedido.Mesa != null && pedido.Mesa.Estado == "Ocupada")
                {
                    var otrosPedidosActivos = await _context.Pedidos
                        .AnyAsync(p => p.MESAID == pedido.MESAID &&
                                      p.PedidoID != id &&
                                      p.Estado != "Cancelado" &&
                                      p.Estado != "Entregado");

                    if (!otrosPedidosActivos)
                    {
                        pedido.Mesa.Estado = "Libre";
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "✅ Pedido cancelado exitosamente",
                    pedidoCancelado = new
                    {
                        pedidoId = pedido.PedidoID,
                        totalProductos = pedido.DetallePedidos.Count,
                        cliente = pedido.Usuario?.NombreCompleto ?? "",
                        mesa = pedido.Mesa?.NumeroMesas ?? 0,
                        estadoMesa = pedido.Mesa?.Estado ?? ""
                    },
                    canceladoPor = User.Identity!.Name,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al cancelar el pedido",
                    error = ex.Message
                });
            }
        }

        // GET: api/pedidos/estadisticas
        [HttpGet("estadisticas")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> GetEstadisticas()
        {
            try
            {
                var totalPedidos = await _context.Pedidos.CountAsync();
                var pedidosPendientes = await _context.Pedidos.CountAsync(p => p.Estado == "Pendiente");
                var pedidosActivos = await _context.Pedidos.CountAsync(p => p.Estado == "Activo");
                var pedidosEntregados = await _context.Pedidos.CountAsync(p => p.Estado == "Entregado");
                var pedidosCancelados = await _context.Pedidos.CountAsync(p => p.Estado == "Cancelado");

                var pedidosHoy = await _context.Pedidos
                    .CountAsync(p => p.FechaCreacion.Date == DateTime.Today);

                return Ok(new
                {
                    message = "✅ Estadísticas de pedidos El Criollo",
                    resumenGeneral = new
                    {
                        totalPedidos,
                        pedidosHoy
                    },
                    estadosPedidos = new
                    {
                        pendientes = pedidosPendientes,
                        activos = pedidosActivos,
                        entregados = pedidosEntregados,
                        cancelados = pedidosCancelados
                    },
                    usuario = User.Identity!.Name,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al obtener estadísticas",
                    error = ex.Message
                });
            }
        }

        // Método auxiliar
        private static string GetMensajeSegunEstado(string estado, int pedidoId)
        {
            return estado switch
            {
                "Preparando" => $"El pedido #{pedidoId} está siendo preparado en la cocina.",
                "Listo" => $"¡El pedido #{pedidoId} está listo para servir!",
                "Entregado" => $"El pedido #{pedidoId} ha sido entregado exitosamente.",
                "Cancelado" => $"El pedido #{pedidoId} ha sido cancelado.",
                _ => $"El pedido #{pedidoId} ha cambiado de estado a: {estado}"
            };
        }
    }
}