namespace AppForSEII.API.DTOs.Accesorios3DDTOs;

// DTO que devuelven el DETAILS y el POST (paso 7 del caso de uso): los datos de la
// compra más su Id y la fecha de compra.
public class CompraAccesoriosDetalleDTO : CompraAccesoriosParaCrearDTO
{
    public CompraAccesoriosDetalleDTO()
    {
    }

    public CompraAccesoriosDetalleDTO(int id, DateTime fechaCompra, string userNameCliente, string nombreCliente,
        string apellidosCliente, string direccionEnvio, string numeroTelefono, MetodoPago metodoPago,
        IList<LineaCompraAccesorioDTO> lineas)
        : base(userNameCliente, nombreCliente, apellidosCliente, direccionEnvio, numeroTelefono, metodoPago, lineas)
    {
        Id = id;
        FechaCompra = fechaCompra;
    }

    public int Id { get; set; }

    public DateTime FechaCompra { get; set; }

    // La fecha del POST se genera con DateTime.Now, así que en los tests
    // se considera igual si la diferencia es menor de un minuto.
    private static bool CompararFecha(DateTime fecha1, DateTime fecha2)
    {
        return (fecha1 - fecha2).Duration() < TimeSpan.FromMinutes(1);
    }

    public override bool Equals(object? obj)
    {
        return obj is CompraAccesoriosDetalleDTO dto &&
               base.Equals(obj) &&
               Id == dto.Id &&
               CompararFecha(FechaCompra, dto.FechaCompra);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Id);
    }
}
