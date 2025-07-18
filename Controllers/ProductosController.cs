using ElCriolloAPI.Data;
using ElCriolloAPI.DTOS;
using ElCriolloAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElCriolloAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/productos
        // Menú completo - TODOS los roles pueden ver
        [HttpGet]
        [Authorize(Roles = "Administrador,Mesero,Recepcionista,Cajero")]
        public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetProductos()
        {
            try
            {
                var productos = await _context.Productos
                    .OrderBy(p => p.Categoria)
                    .ThenBy(p => p.NombreProducto)
                    .ToListAsync();

                var productosDto = productos.Select(p => new ProductoResponseDto
                {
                    ProductosId = p.ProductosId,
                    NombreProducto = p.NombreProducto,
                    Descripcion = p.Descripcion,
                    Categoria = p.Categoria,
                    Precio = p.Precio,
                    Disponible = p.Disponible,
                    FechasCreacion = p.FechasCreacion,

                }).ToList();

                return Ok(new
                {
                    message = "✅ Menú de El Criollo obtenido exitosamente",
                    totalProductos = productosDto.Count,
                    productos = productosDto,
                    consultadoPor = User.Identity!.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al obtener el menú",
                    error = ex.Message
                });
            }
        }

        // GET: api/productos/5
        // Producto específico
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Mesero,Recepcionista,Cajero")]
        public async Task<ActionResult<ProductoResponseDto>> GetProducto(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    return NotFound(new
                    {
                        message = $"❌ Producto con ID {id} no encontrado",
                        productoId = id
                    });
                }

                var productoDto = new ProductoResponseDto
                {
                    ProductosId = producto.ProductosId,
                    NombreProducto = producto.NombreProducto,
                    Descripcion = producto.Descripcion,
                    Categoria = producto.Categoria,
                    Precio = producto.Precio,
                    Disponible = producto.Disponible,
                    FechasCreacion = producto.FechasCreacion,
                    
                };

                return Ok(productoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al obtener el producto",
                    error = ex.Message
                });
            }
        }

        [HttpGet("categoria/{categoria}")]
        [Authorize(Roles = "Administrador,Mesero,Recepcionista,Cajero")]
        public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetProductosPorCategoria(string categoria)
        {
            try
            {
                var productos = await _context.Productos
                    .Where(p => p.Categoria == categoria)
                    .OrderBy(p => p.NombreProducto)
                    .ToListAsync();

                if (!productos.Any())
                {
                    return NotFound(new
                    {
                        message = $"❌ No se encontraron productos en la categoría '{categoria}'",
                        categoria = categoria,
                        categoriasDisponibles = new[] { "Entradas", "Plato Principal", "Acompañante", "Bebidas", "Bebidas Alcoholica", "Postres" }
                    });
                }

                var productosDto = productos.Select(p => new ProductoResponseDto
                {
                    ProductosId = p.ProductosId,
                    NombreProducto = p.NombreProducto,
                    Descripcion = p.Descripcion,
                    Categoria = p.Categoria,
                    Precio = p.Precio,
                    Disponible = p.Disponible,
                    FechasCreacion = p.FechasCreacion
                }).ToList();

                return Ok(new
                {
                    message = $"✅ Productos de categoría '{categoria}' obtenidos exitosamente",
                    categoria = categoria,
                    totalProductos = productosDto.Count,
                    productos = productosDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al obtener productos por categoría",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // GET: api/productos/disponibles
        // Solo productos disponibles - ENDPOINT ESPECIALIZADO
        [HttpGet("disponibles")]
        [Authorize(Roles = "Administrador,Mesero,Recepcionista,Cajero")]
        public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetProductosDisponibles()
        {
            try
            {
                var productos = await _context.Productos
                    .Where(p => p.Disponible == true)
                    .OrderBy(p => p.Categoria)
                    .ThenBy(p => p.NombreProducto)
                    .ToListAsync();

                var productosDto = productos.Select(p => new ProductoResponseDto
                {
                    ProductosId = p.ProductosId,
                    NombreProducto = p.NombreProducto,
                    Descripcion = p.Descripcion,
                    Categoria = p.Categoria,
                    Precio = p.Precio,
                    Disponible = p.Disponible,
                    FechasCreacion = p.FechasCreacion,
                    
                }).ToList();

                return Ok(new
                {
                    message = "✅ Productos disponibles obtenidos exitosamente",
                    totalDisponibles = productosDto.Count,
                    productos = productosDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al obtener productos disponibles",
                    error = ex.Message
                });
            }
        }

        // POST: api/productos
        // Crear nuevo producto - Solo ADMINISTRADORES
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ProductoResponseDto>> CreateProducto(CreateProductoDto createProductoDto)
        {
            try
            {
                // Verificar si el producto ya existe
                var productoExistente = await _context.Productos
                    .FirstOrDefaultAsync(p => p.NombreProducto.ToLower() == createProductoDto.NombreProducto.ToLower());

                if (productoExistente != null)
                {
                    return BadRequest(new
                    {
                        message = "❌ Ya existe un producto con ese nombre",
                        nombreProducto = createProductoDto.NombreProducto,
                        productoExistente = productoExistente.ProductosId
                    });
                }

                // Crear nuevo producto
                var producto = new Producto
                {
                    NombreProducto = createProductoDto.NombreProducto,
                    Descripcion = createProductoDto.Descripcion,
                    Categoria = createProductoDto.Categoria,
                    Precio = createProductoDto.Precio,
                    Disponible = createProductoDto.Disponible,
                    FechasCreacion = DateTime.Now,
                   
                };

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                var productoResponse = new ProductoResponseDto
                {
                    ProductosId = producto.ProductosId,
                    NombreProducto = producto.NombreProducto,
                    Descripcion = producto.Descripcion,
                    Categoria = producto.Categoria,
                    Precio = producto.Precio,
                    Disponible = producto.Disponible,
                    FechasCreacion = producto.FechasCreacion,
                   
                };

                return Ok(new
                {
                    message = "✅ Producto creado exitosamente por Administrador",
                    producto = productoResponse,
                    creadoPor = User.Identity!.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al crear el producto",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // PUT: api/productos/5
        // Actualizar producto - Solo ADMINISTRADORES
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> UpdateProducto(int id, UpdateProductoDto updateProductoDto)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);

                if (producto == null)
                {
                    return NotFound(new
                    {
                        message = $"❌ Producto con ID {id} no encontrado",
                        productoId = id
                    });
                }

                // Verificar nombre duplicado (si cambió)
                if (producto.NombreProducto.ToLower() != updateProductoDto.NombreProducto.ToLower())
                {
                    var nombreExiste = await _context.Productos
                        .AnyAsync(p => p.NombreProducto.ToLower() == updateProductoDto.NombreProducto.ToLower() && p.ProductosId != id);

                    if (nombreExiste)
                    {
                        return BadRequest(new
                        {
                            message = "❌ Ya existe otro producto con ese nombre",
                            nombreProducto = updateProductoDto.NombreProducto
                        });
                    }
                }

                // Actualizar producto
                producto.NombreProducto = updateProductoDto.NombreProducto;
                producto.Descripcion = updateProductoDto.Descripcion;
                producto.Categoria = updateProductoDto.Categoria;
                producto.Precio = updateProductoDto.Precio;
                producto.Disponible = updateProductoDto.Disponible;
                

                await _context.SaveChangesAsync();

                var productoResponse = new ProductoResponseDto
                {
                    ProductosId = producto.ProductosId,
                    NombreProducto = producto.NombreProducto,
                    Descripcion = producto.Descripcion,
                    Categoria = producto.Categoria,
                    Precio = producto.Precio,
                    Disponible = producto.Disponible,
                    FechasCreacion = producto.FechasCreacion,
                    
                };

                return Ok(new
                {
                    message = "✅ Producto actualizado exitosamente por Administrador",
                    producto = productoResponse,
                    modificadoPor = User.Identity!.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al actualizar el producto",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // DELETE: api/productos/5
        // Eliminar producto - Solo ADMINISTRADORES
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> DeleteProducto(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);

                if (producto == null)
                {
                    return NotFound(new
                    {
                        message = $"❌ Producto con ID {id} no encontrado",
                        productoId = id
                    });
                }

                // Verificar si el producto está en pedidos activos
                var tieneDetallePedidos = await _context.DetallePedidos
                    .AnyAsync(dp => dp.ProductosId == id);

                if (tieneDetallePedidos)
                {
                    return BadRequest(new
                    {
                        message = "❌ No se puede eliminar el producto porque está en pedidos existentes",
                        productoId = id,
                        nombreProducto = producto.NombreProducto,
                        recomendacion = "Marca el producto como no disponible en su lugar"
                    });
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "✅ Producto eliminado exitosamente por Administrador",
                    productoEliminado = new
                    {
                        productoId = producto.ProductosId,
                        nombreProducto = producto.NombreProducto,
                        categoria = producto.Categoria,
                        precio = producto.Precio
                    },
                    eliminadoPor = User.Identity!.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Error al eliminar el producto",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // GET: api/productos/test-connection
        // Estadísticas del menú - Solo ADMINISTRADORES
        [HttpGet("test-connection")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                var totalProductos = await _context.Productos.CountAsync();
                var productosDisponibles = await _context.Productos.CountAsync(p => p.Disponible);
                var productosNoDisponibles = totalProductos - productosDisponibles;

                // Estadísticas por categoría
                var estadisticasPorCategoria = await _context.Productos
                    .GroupBy(p => p.Categoria)
                    .Select(g => new
                    {
                        categoria = g.Key,
                        total = g.Count(),
                        disponibles = g.Count(p => p.Disponible),
                    })
                    .ToListAsync();

                return Ok(new
                {
                    message = "✅ Estadísticas del menú El Criollo (Solo Administrador)",
                    resumen = new
                    {
                        totalProductos,
                        productosDisponibles,
                        productosNoDisponibles
                    },
                    estadisticasPorCategoria,
                    usuario = User.Identity!.Name,
                    rol = User.FindFirst("role")?.Value,
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
    }
}
    

