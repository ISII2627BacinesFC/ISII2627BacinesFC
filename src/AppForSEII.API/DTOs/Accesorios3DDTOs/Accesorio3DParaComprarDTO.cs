using AppForSEII.API.Models.ComprarAccesorios;

namespace AppForSEII.API.DTOs.Accesorios3DDTOs;

// DTO que devuelve el SELECT (paso 2 del caso de uso): nombre, categoría,
// compatibilidad, cantidad disponible y precio de cada accesorio.
public class Accesorio3DParaComprarDTO
{
    public Accesorio3DParaComprarDTO()
    {
    }

    public Accesorio3DParaComprarDTO(int id, string nombre, CategoriaAccesorio categoria, string compatibilidad,
        int cantidadDisponible, decimal precio)
    {
        Id = id;
        Nombre = nombre;
        Categoria = categoria;
        Compatibilidad = compatibilidad;
        CantidadDisponible = cantidadDisponible;
        Precio = precio;
    }

    public int Id { get; set; }

    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    public CategoriaAccesorio Categoria { get; set; }

    [StringLength(100, ErrorMessage = "La compatibilidad no puede tener más de 100 caracteres.")]
    public string Compatibilidad { get; set; } = string.Empty;

    public int CantidadDisponible { get; set; }

    public decimal Precio { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Accesorio3DParaComprarDTO dto &&
               Id == dto.Id &&
               Nombre == dto.Nombre &&
               Categoria == dto.Categoria &&
               Compatibilidad == dto.Compatibilidad &&
               CantidadDisponible == dto.CantidadDisponible &&
               Precio == dto.Precio;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Nombre, Categoria, Compatibilidad, CantidadDisponible, Precio);
    }
}
