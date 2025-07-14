using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElCriolloAPI.Models
{
    public class Reserva
    {



        [Key]
        [Column("ReservaID")]
        public int ReservaId { get; set; }

        [Required]
        public int MesaId { get; set; }

        [StringLength(100)]
        public string? NombreCliente { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required]
        public int CantidadPersonas { get; set; }

        [Required]
        public DateTime FechaReserva { get; set; }

        [Required]
        public TimeSpan HoraReserva { get; set; }

        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        public bool CorreoEnviado { get; set; } = false;

        public DateTime? FechaEnvioCorreo { get; set; }

        public string? ObservacionesReserva { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
