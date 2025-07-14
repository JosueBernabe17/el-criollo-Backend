using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElCriolloAPI.Models
{
    public class Factura
    {
        [Key]
        [Column("FacturaID")]
        public int FacturaId { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ITBIS { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalFinal { get; set; } = 0.00m;

        [Required]
        [StringLength(200)]
        public string MetodoPago { get; set; } = string.Empty;

        public DateTime FechaFacturacion { get; set; } = DateTime.Now;
    }
}
