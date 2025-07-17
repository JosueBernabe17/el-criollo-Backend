using System.ComponentModel.DataAnnotations;

namespace ElCriolloAPI.DTOS
{
    public class UpdateMesaDto
    {

        [Required(ErrorMessage = "El número de mesa es requerido")]
        [Range(1, 999, ErrorMessage = "El número de mesa debe estar entre 1 y 999")]
        public int NumeroMesas { get; set; }

        [Required(ErrorMessage = "La capacidad es requerida")]
        [Range(1, 20, ErrorMessage = "La capacidad debe estar entre 1 y 20 personas")]
        public int Capacidad { get; set; }

        [StringLength(50, ErrorMessage = "La ubicación no puede exceder 50 caracteres")]
        public string? Ubicacion { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [RegularExpression("^(Libre|Ocupada|Reservada)$", ErrorMessage = "El estado debe ser: Libre, Ocupada o Reservada")]
        public string Estado { get; set; } = "Libre";
    

    }
}
