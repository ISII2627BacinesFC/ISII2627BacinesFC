namespace AppForSEII.API.Models.ComprarAccesorios
{
    public class CompraAccesorios
    {
        public CompraAccesorios()
        {
        }

        public CompraAccesorios(DateTime fechaCompra, string nombreCliente, string apellidosCliente, string direccionEnvio, string numeroTelefono, decimal precioTotal, MetodoPago metodoPago)
        {
            FechaCompra = fechaCompra;
            NombreCliente = nombreCliente;
            ApellidosCliente = apellidosCliente;
            DireccionEnvio = direccionEnvio;
            NumeroTelefono = numeroTelefono;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
        public DateTime FechaCompra { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos del cliente son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden tener más de 100 caracteres.")]
        public string ApellidosCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección de envío es obligatoria.")]
        [StringLength(150, ErrorMessage = "La dirección de envío no puede tener más de 150 caracteres.")]
        public string DireccionEnvio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El formato del número de teléfono no es válido.")]
        [StringLength(15, ErrorMessage = "El número de teléfono no puede tener más de 15 caracteres.")]
        public string NumeroTelefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio total es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio total debe ser mayor que 0.")]
        [Precision(10, 2)]
        public decimal PrecioTotal { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        public MetodoPago MetodoPago { get; set; }

        public Client Cliente { get; set; }

        public IList<LineaCompraAccesorio> LineasCompraAccesorio { get; set; } = new List<LineaCompraAccesorio>();
    }
}