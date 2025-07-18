namespace ElCriolloAPI.DTOS
{
    public class PedidoResponseDto
    {
        public int PedidoId { get; set; }
        public int MesaId { get; set; }
        public int NumeroMesa { get; set; } 
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public string? NotasEspeciales { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public List<DetallePedidoResponseDto> Productos { get; set; } = new List<DetallePedidoResponseDto>();
    }

    public class DetallePedidoResponseDto
    {
        public int DetallePedidoId { get; set; }
        public int ProductosId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string CategoriaProducto { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public string? NotasEspeciales { get; set; }
    }
}
