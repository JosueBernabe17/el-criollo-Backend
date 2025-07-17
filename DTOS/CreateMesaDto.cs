using System.ComponentModel.DataAnnotations;

namespace ElCriolloAPI.DTOS
{
    public class CreateMesaDto
    {


        [Required(ErrorMessage = "El número de mesa es requerido")]
        [Range(1, 999, ErrorMessage = "El número de mesa debe estar entre 1 y 999")]
        public int NumeroMesas { get; set; }

        [Required(ErrorMessage = "La capacidad es requerida")]
        [Range(1, 20, ErrorMessage = "La capacidad debe estar entre 1 y 20 personas")]
        public int Capacidad { get; set; }

        [StringLength(50, ErrorMessage = "La ubicación no puede exceder 50 caracteres")]
        public string? Ubicacion { get; set; }

        [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
        public string Estado { get; set; } = "Libre";
    



    }
}
