namespace AppForSEII.API.DTOs.Accesorios3DDTOs;

// DTO que recibe el POST (pasos 5 y 6 del caso de uso): datos obligatorios del
// cliente (nombre, apellidos, dirección de envío, teléfono, método de pago)
// y la lista de accesorios seleccionados.
public class CompraAccesoriosParaCrearDTO
{
    public CompraAccesoriosParaCrearDTO()
    {
    }

    public CompraAccesoriosParaCrearDTO(string userNameCliente, string nombreCliente, string apellidosCliente,
        string direccionEnvio, string numeroTelefono, MetodoPago metodoPago, IList<LineaCompraAccesorioDTO> lineas)
    {
        UserNameCliente = userNameCliente;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionEnvio = direccionEnvio;
        NumeroTelefono = numeroTelefono;
        MetodoPago = metodoPago;
        Lineas = lineas;
    }

    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [EmailAddress]
    public string UserNameCliente { get; set; } = string.Empty;

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

    [Required(ErrorMessage = "El método de pago es obligatorio.")]
    public MetodoPago MetodoPago { get; set; }

    public IList<LineaCompraAccesorioDTO> Lineas { get; set; } = new List<LineaCompraAccesorioDTO>();

    public decimal PrecioTotal => Lineas.Sum(l => l.Subtotal);

    public override bool Equals(object? obj)
    {
        return obj is CompraAccesoriosParaCrearDTO dto &&
               UserNameCliente == dto.UserNameCliente &&
               NombreCliente == dto.NombreCliente &&
               ApellidosCliente == dto.ApellidosCliente &&
               DireccionEnvio == dto.DireccionEnvio &&
               NumeroTelefono == dto.NumeroTelefono &&
               MetodoPago == dto.MetodoPago &&
               PrecioTotal == dto.PrecioTotal &&
               Lineas.SequenceEqual(dto.Lineas);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserNameCliente, NombreCliente, ApellidosCliente, DireccionEnvio,
            NumeroTelefono, MetodoPago, PrecioTotal);
    }
}
