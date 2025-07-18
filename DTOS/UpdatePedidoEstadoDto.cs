using System.ComponentModel.DataAnnotations;

namespace ElCriolloAPI.DTOS
{
    public class UpdatePedidoEstadoDto
    {
        [Required(ErrorMessage = "El estado es requerido")]
        [RegularExpression("^(Facturado|Cancelado)$",
              ErrorMessage = "El estado debe ser: Facturado o Cancelado")]
        public string Estado { get; set; } = string.Empty;

        public string? NotasActualizacion { get; set; }

    }
}
