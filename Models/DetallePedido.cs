using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElCriolloAPI.Models
{
    public class DetallePedido
    {

        [Key]
        [Column("DetalleID")]
        public int DetalleId { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        public int ProductosId { get; set; }

        [Required]
        public int Cantidad { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        public string? ObservacionesDetalle { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
