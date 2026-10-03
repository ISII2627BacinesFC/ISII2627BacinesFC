namespace AppForSEII.API.Models;

public class CompraModelo3D
{
    public CompraModelo3D()
    {
    }

    public CompraModelo3D(DateTime fechaCompra, string nombreCliente, string apellidosCliente, string correoElectronico,
        string direccionFacturacion, string? descripcion, decimal precioTotal, MetodoPago metodoPago, Client cliente)
    {
        FechaCompra = fechaCompra;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        CorreoElectronico = correoElectronico;
        DireccionFacturacion = direccionFacturacion;
        Descripcion = descripcion;
        PrecioTotal = precioTotal;
        MetodoPago = metodoPago;
        Cliente = cliente;
    }

    public int Id { get; set; }

    [PlantUmlIgnoreAssociation]
    [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
    [System.ComponentModel.DataAnnotations.Display(Name = "Fecha de compra")]
    public DateTime FechaCompra { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.", MinimumLength = 1)]
    public string NombreCliente { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "Los apellidos no pueden tener más de 100 caracteres.", MinimumLength = 1)]
    public string ApellidosCliente { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required]
    [StringLength(150, ErrorMessage = "La dirección no puede tener más de 150 caracteres.", MinimumLength = 1)]
    public string DireccionFacturacion { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres.")]
    public string? Descripcion { get; set; }

    [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
    [Precision(10, 2)]
    public decimal PrecioTotal { get; set; }

    [Required]
    [PlantUmlIgnoreAssociation]
    public MetodoPago MetodoPago { get; set; }

    [Required]
    public Client Cliente { get; set; } = null!;
}