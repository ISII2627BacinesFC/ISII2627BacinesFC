using AppForSEII.API.Models.ComprarAccesorios;

namespace AppForSEII.API.DTOs.Accesorios3DDTOs;

// Una línea de la compra: se usa tanto para enviar la compra (POST)
// como para mostrarla (DETAILS): nombre, categoría, compatibilidad,
// cantidad, precio por unidad y precio subtotal.
public class LineaCompraAccesorioDTO
{
    public LineaCompraAccesorioDTO()
    {
    }

    public LineaCompraAccesorioDTO(int accesorioId, string nombre, CategoriaAccesorio categoria,
        string compatibilidad, int cantidad, decimal precioUnidad)
    {
        AccesorioId = accesorioId;
        Nombre = nombre;
        Categoria = categoria;
        Compatibilidad = compatibilidad;
        Cantidad = cantidad;
        PrecioUnidad = precioUnidad;
    }

    public int AccesorioId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public CategoriaAccesorio Categoria { get; set; }

    public string Compatibilidad { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; }

    public decimal PrecioUnidad { get; set; }

    public decimal Subtotal => Cantidad * PrecioUnidad;

    public override bool Equals(object? obj)
    {
        return obj is LineaCompraAccesorioDTO dto &&
               AccesorioId == dto.AccesorioId &&
               Nombre == dto.Nombre &&
               Categoria == dto.Categoria &&
               Compatibilidad == dto.Compatibilidad &&
               Cantidad == dto.Cantidad &&
               PrecioUnidad == dto.PrecioUnidad;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(AccesorioId, Nombre, Categoria, Compatibilidad, Cantidad, PrecioUnidad);
    }
}
