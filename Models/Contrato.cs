namespace MawdyAsistenciaApp.Models
{
    public class Contrato
    {
        public int Id { get; set; }
        public required string NombreContrato { get; set; }
        public required bool Activo { get; set; }
        public required int Suplemento { get; set; }
        public required int ClienteId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
