using System.ComponentModel.DataAnnotations;

namespace ElCriolloAPI.DTOS
{
    public class UpdateProductoDto
    {

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string NombreProducto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es requerida")]
        [RegularExpression("^(Entradas|Plato Principal|Acompañante|Bebidas|Bebidas Alcoholica|Postres)$",
            ErrorMessage = "Categoría debe ser: Entradas, Plato Principal, Acompañante, Bebidas, Bebidas Alcoholica, o Postres")]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(1, 9999.99, ErrorMessage = "El precio debe estar entre $1.00 y $9999.99")]
        public decimal Precio { get; set; }

        public bool Disponible { get; set; } = true;
    }
}
