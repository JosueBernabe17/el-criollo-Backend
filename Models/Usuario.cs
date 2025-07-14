using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElCriolloAPI.Models
{
    public class Usuario
    {

        [Key]
        [Column("UsuarioID")]
        public int UsuarioId { get; set; }
        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Contraseña { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Column("Role")]
        public string Rol { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
