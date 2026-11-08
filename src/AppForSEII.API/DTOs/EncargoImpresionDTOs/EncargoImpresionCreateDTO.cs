using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.EncargoImpresionDTOs
{
    public class EncargoImpresionCreateDTO
    {
        public EncargoImpresionCreateDTO()
        {
            Lineas = new List<LineaEncargoImpresionCreateDTO>();
        }

        public EncargoImpresionCreateDTO(string nombreCliente, string apellidosCliente, 
            string direccionEnvio, string numeroTelefono, string metodoPago, 
            string? descripcion, IList<LineaEncargoImpresionCreateDTO> lineas)
        {
            NombreCliente = nombreCliente;
            ApellidosCliente = apellidosCliente;
            DireccionEnvio = direccionEnvio;
            NumeroTelefono = numeroTelefono;
            MetodoPago = metodoPago;
            Descripcion = descripcion;
            Lineas = lineas;
        }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(50)]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos del cliente son obligatorios.")]
        [StringLength(100)]
        public string ApellidosCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección de envío es obligatoria.")]
        [StringLength(200)]
        public string DireccionEnvio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        public string NumeroTelefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        public string MetodoPago { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "El encargo debe contener al menos una línea.")]
        public IList<LineaEncargoImpresionCreateDTO> Lineas { get; set; }
    }
}