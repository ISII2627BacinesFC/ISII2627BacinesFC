namespace AppForSEII.API.Models;

public class LicenciaModelo3D
{
    public LicenciaModelo3D()
    {
    }

    public LicenciaModelo3D(string nombre, DateTime fechaExpiracion)
    {
        Nombre = nombre;
        FechaExpiracion = fechaExpiracion;
    }

    public int Id { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "El nombre de la licencia no puede tener más de 50 caracteres.", MinimumLength = 1)]
    public string Nombre { get; set; } = string.Empty;

            [PlantUmlIgnoreAssociation]
    [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    [System.ComponentModel.DataAnnotations.Display(Name = "Fecha de expiración")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime FechaExpiracion { get; set; }
}