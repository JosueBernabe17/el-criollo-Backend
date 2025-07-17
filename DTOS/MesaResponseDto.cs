namespace ElCriolloAPI.DTOS
{
    public class MesaResponseDto
    {
        public int MesaId { get; set; }
        public int NumeroMesas { get; set; }
        public int Capacidad { get; set; }
        public string? Ubicacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }

        // Opcional: Información adicional sin relaciones complejas
        public int PedidosActivos { get; set; }
        public bool TieneReservas { get; set; }
    

    }
}
