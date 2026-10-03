namespace AppForSEII.API.Models;

public class Modelo3D
{
    public Modelo3D()
    {
    }

    public Modelo3D(string nombre, string categoria, FormatoModelo3D formato, decimal precio, LicenciaModelo3D licencia)
    {
        Nombre = nombre;
        Categoria = categoria;
        Formato = formato;
        Precio = precio;
        Licencia = licencia;
    }

    public int Id { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.", MinimumLength = 1)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(30, ErrorMessage = "La categoría no puede tener más de 30 caracteres.", MinimumLength = 1)]
    public string Categoria { get; set; } = string.Empty;

    [Required]
    [PlantUmlIgnoreAssociation]
    public FormatoModelo3D Formato { get; set; }

    [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0.")]
    [Precision(10, 2)]
    public decimal Precio { get; set; }

    [Required]
    public LicenciaModelo3D Licencia { get; set; } = null!;
}