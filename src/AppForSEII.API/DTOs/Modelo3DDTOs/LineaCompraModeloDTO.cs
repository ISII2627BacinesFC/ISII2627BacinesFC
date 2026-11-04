namespace AppForSEII.API.DTOs.Modelo3DDTOs;

public class LineaCompraModeloDTO
{
    public LineaCompraModeloDTO()
    {
    }

    public LineaCompraModeloDTO(int modeloId, string nombre, string categoria, FormatoModelo3D formato,
        string licencia, DateTime fechaExpiracionLicencia, int cantidadLicencias, decimal precioUnidad, decimal subtotal)
    {
        ModeloId = modeloId;
        Nombre = nombre;
        Categoria = categoria;
        Formato = formato;
        Licencia = licencia;
        FechaExpiracionLicencia = fechaExpiracionLicencia;
        CantidadLicencias = cantidadLicencias;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
    }

    public int ModeloId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public FormatoModelo3D Formato { get; set; }

    public string Licencia { get; set; } = string.Empty;

    public DateTime FechaExpiracionLicencia { get; set; }

    public int CantidadLicencias { get; set; }

    public decimal PrecioUnidad { get; set; }

    public decimal Subtotal { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is LineaCompraModeloDTO dto &&
               ModeloId == dto.ModeloId &&
               Nombre == dto.Nombre &&
               Categoria == dto.Categoria &&
               Formato == dto.Formato &&
               Licencia == dto.Licencia &&
               FechaExpiracionLicencia == dto.FechaExpiracionLicencia &&
               CantidadLicencias == dto.CantidadLicencias &&
               PrecioUnidad == dto.PrecioUnidad &&
               Subtotal == dto.Subtotal;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(ModeloId);
        hash.Add(Nombre);
        hash.Add(Categoria);
        hash.Add(Formato);
        hash.Add(Licencia);
        hash.Add(FechaExpiracionLicencia);
        hash.Add(CantidadLicencias);
        hash.Add(PrecioUnidad);
        hash.Add(Subtotal);
        return hash.ToHashCode();
    }
}