namespace AppForSEII.API.Models
{
    public class EncargoImpresion
    {
        public int Id { get; set; }
        public DateTime FechaEncargo { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidosCliente { get; set; } = string.Empty;
        public string DireccionEnvio { get; set; } = string.Empty;
        public string NumeroTelefono { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal PrecioTotal { get; set; }

        // Atributo enum MetodoPago (TarjetaCredito, PayPal, Bizum)
        public MetodoPago MetodoPago { get; set; }

        // Relación 0..* a 1 con Cliente ("realiza")
        public string ClienteId { get; set; } = string.Empty;
        public Client Cliente { get; set; } = null!;

        // Relación 1 a 1..* con LineaEncargo ("contiene")
        public IList<LineaEncargo> LineasEncargo { get; set; } = new List<LineaEncargo>();
    }
}