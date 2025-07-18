using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElCriolloAPI.Models
{
    public class Producto
    {
        [Key]
        [Column("ProductosID")]
        public int ProductosId { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreProducto { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; } 

        public bool Disponible { get; set; } = true;

        [Column("FechasCreacion")]
        public DateTime FechasCreacion { get; set; } = DateTime.Now;

        public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();



    }
}
