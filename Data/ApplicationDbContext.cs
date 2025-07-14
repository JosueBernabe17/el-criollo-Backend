using Microsoft.EntityFrameworkCore;
using ElCriolloAPI.Models;

namespace ElCriolloAPI.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets - Representan las tablas de tu base de datos
        // Cada DbSet<T> = Una tabla en SQL Server

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedidos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        // Configuración adicional de las tablas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar nombres de tablas para que coincidan con tu base de datos
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Mesa>().ToTable("MESAS");
            modelBuilder.Entity<Producto>().ToTable("PRODUCTOS");
            modelBuilder.Entity<Pedido>().ToTable("Pedidos");
            modelBuilder.Entity<DetallePedido>().ToTable("DETALLE_PEDIDOS");
            modelBuilder.Entity<Factura>().ToTable("FACTURAS");
            modelBuilder.Entity<Reserva>().ToTable("RESERVAS");

            // Configuración especial para columnas calculadas
            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.Subtotal)
                .HasComputedColumnSql("[Cantidad] * [PrecioUnitario]");
            
          
        }
    }
}