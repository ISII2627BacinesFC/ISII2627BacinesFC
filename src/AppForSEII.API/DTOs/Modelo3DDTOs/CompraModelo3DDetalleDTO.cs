namespace AppForSEII.API.DTOs.Modelo3DDTOs;

public class CompraModelo3DDetalleDTO
{
    public CompraModelo3DDetalleDTO()
    {
    }

    public CompraModelo3DDetalleDTO(int id, string nombreCliente, string apellidosCliente, string correoElectronico,
        string direccionFacturacion, DateTime fechaCompra, decimal precioTotal, MetodoPago metodoPago,
        string? descripcion, IList<LineaCompraModeloDTO> lineas)
    {
        Id = id;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        CorreoElectronico = correoElectronico;
        DireccionFacturacion = direccionFacturacion;
        FechaCompra = fechaCompra;
        PrecioTotal = precioTotal;
        MetodoPago = metodoPago;
        Descripcion = descripcion;
        Lineas = lineas;
    }

    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
    public string NombreCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100, ErrorMessage = "Los apellidos no pueden tener más de 100 caracteres.")]
    public string ApellidosCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección de facturación es obligatoria.")]
    [StringLength(150, ErrorMessage = "La dirección no puede tener más de 150 caracteres.")]
    public string DireccionFacturacion { get; set; } = string.Empty;

    public DateTime FechaCompra { get; set; }

    public decimal PrecioTotal { get; set; }

    public MetodoPago MetodoPago { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres.")]
    public string? Descripcion { get; set; }

    public IList<LineaCompraModeloDTO> Lineas { get; set; } = new List<LineaCompraModeloDTO>();

    public override bool Equals(object? obj)
    {
        return obj is CompraModelo3DDetalleDTO dto &&
               Id == dto.Id &&
               NombreCliente == dto.NombreCliente &&
               ApellidosCliente == dto.ApellidosCliente &&
               CorreoElectronico == dto.CorreoElectronico &&
               DireccionFacturacion == dto.DireccionFacturacion &&
               FechaCompra == dto.FechaCompra &&
               PrecioTotal == dto.PrecioTotal &&
               MetodoPago == dto.MetodoPago &&
               Descripcion == dto.Descripcion &&
               Lineas.SequenceEqual(dto.Lineas);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id);
        hash.Add(NombreCliente);
        hash.Add(ApellidosCliente);
        hash.Add(CorreoElectronico);
        hash.Add(DireccionFacturacion);
        hash.Add(FechaCompra);
        hash.Add(PrecioTotal);
        hash.Add(MetodoPago);
        hash.Add(Descripcion);
        return hash.ToHashCode();
    }
}