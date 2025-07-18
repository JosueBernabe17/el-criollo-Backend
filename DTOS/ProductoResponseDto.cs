namespace ElCriolloAPI.DTOS
{
    public class ProductoResponseDto
    {
        public int ProductosId { get; set; } 
        public string NombreProducto { get; set; } = string.Empty; 
        public string Descripcion { get; set; } = string.Empty; 
        public string Categoria { get; set; } = string.Empty; 
        public decimal Precio { get; set; } 
        public bool Disponible { get; set; } 
        public DateTime FechasCreacion { get; set; } 
    }
}
