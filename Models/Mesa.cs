using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElCriolloAPI.Models
{
    public class Mesa
    {
        [Key]
        [Column("MESAID")]
        public int MesaId { get; set; }

        [Required]
        public int NumeroMesas { get; set; }

        [Required]
        public int Capacidad { get; set; }

        [StringLength(50)]
        public string? Ubicacion { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Libre";

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();



    }
}
