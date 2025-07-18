using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElCriolloAPI.Models
{
    public class Pedido
    {

     [Key]
     [Column ("PedidoID")] 
     public int PedidoID { get; set; } 
     [Required]
     [Column ("MESAID")]
     public int MESAID { get; set; }
     [Required]
     [Column("UsuarioID")]  
     public int UsuarioId { get; set; }
     [Required]
     [Column("ProductosID")] 
     public int ProductosID { get; set; }

        [Required]
     [StringLength(20)]
     public string Estado { get; set; } = "Facturado";
     public DateTime FechaPedido { get; set; } = DateTime.Now;
     [Required]
     [Column(TypeName = "decimal(10,2)")]
     public decimal Total { get; set; } = 0.00m;
     public DateTime FechaCreacion { get; set; } = DateTime.Now;

     [ForeignKey("MESAID")]
     public virtual Mesa Mesa { get; set; } = null!;

     [ForeignKey("UsuarioId")] 
     public virtual Usuario Usuario { get; set; } = null!;

     [ForeignKey("ProductosID")] 
     public virtual Producto Producto { get; set; } = null!;
     public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

     public virtual Factura? Factura { get; set; }


    }
}
