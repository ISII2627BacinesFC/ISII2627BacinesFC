namespace AppForSEII.API.DTOs.Modelo3DDTOs;

public class Modelo3DParaComprarDTO
{
    public Modelo3DParaComprarDTO()
    {
    }

    public Modelo3DParaComprarDTO(int id, string nombre, string categoria, string licencia,
        FormatoModelo3D formato, decimal precio)
    {
        Id = id;
        Nombre = nombre;
        Categoria = categoria;
        Licencia = licencia;
        Formato = formato;
        Precio = precio;
    }

    public int Id { get; set; }

    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = "La categoría no puede tener más de 30 caracteres.")]
    public string Categoria { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "La licencia no puede tener más de 50 caracteres.")]
    public string Licencia { get; set; } = string.Empty;

    public FormatoModelo3D Formato { get; set; }

    public decimal Precio { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Modelo3DParaComprarDTO dto &&
               Id == dto.Id &&
               Nombre == dto.Nombre &&
               Categoria == dto.Categoria &&
               Licencia == dto.Licencia &&
               Formato == dto.Formato &&
               Precio == dto.Precio;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Nombre, Categoria, Licencia, Formato, Precio);
    }
}