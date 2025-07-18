using System.ComponentModel.DataAnnotations;

namespace ElCriolloAPI.DTOS
{
    public class CreatePedidoDto
    {
        [Required(ErrorMessage = "El ID de la mesa es requerido")]
        public int MesaId { get; set; }

        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public int UsuarioId { get; set; }

        public string? NotasEspeciales { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos un producto")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
        public List<DetallePedidoDto> Productos { get; set; } = new List<DetallePedidoDto>();
    }

    public class DetallePedidoDto
    {
        [Required(ErrorMessage = "El ID del producto es requerido")]
        public int ProductosId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, 20, ErrorMessage = "La cantidad debe estar entre 1 y 20")]
        public int Cantidad { get; set; }

        public string? NotasEspeciales { get; set; }


    }
}
