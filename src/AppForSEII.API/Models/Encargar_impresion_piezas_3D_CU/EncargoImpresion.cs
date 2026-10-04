using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    public class EncargoImpresion
    {
        public EncargoImpresion() {}

        public EncargoImpresion(DateTime fechaEncargo, string nombreCliente, string apellidosCliente, string direccionEnvio, string numeroTelefono, decimal precioTotal, MetodoPago metodoPago, Client cliente, string? descripcion = null)
        {
            FechaEncargo = fechaEncargo;
            NombreCliente = nombreCliente;
            ApellidosCliente = apellidosCliente;
            DireccionEnvio = direccionEnvio;
            NumeroTelefono = numeroTelefono;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            Cliente = cliente;
            Descripcion = descripcion;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
        public DateTime FechaEncargo { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCliente { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ApellidosCliente { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string DireccionEnvio { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string NumeroTelefono { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Descripcion { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal PrecioTotal { get; set; }

        [Required]
        // Atributo enum MetodoPago (TarjetaCredito, PayPal, Bizum)
        public MetodoPago MetodoPago { get; set; }

        [Required]
        // Relación 0..* a 1 con Cliente ("realiza")
        public string ClienteId { get; set; } = string.Empty;

        [ForeignKey(nameof(ClienteId))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Client Cliente { get; set; } = null!;

        // Relación 1 a 1..* con LineaEncargo ("contiene")
        public IList<LineaEncargo> LineasEncargo { get; set; } = new List<LineaEncargo>();
    }
}