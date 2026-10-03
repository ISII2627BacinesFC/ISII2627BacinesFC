namespace AppForSEII.API.Models;

public class LineaCompraModelo
{
    public LineaCompraModelo()
    {
    }

    public LineaCompraModelo(int cantidadLicencias, decimal precioUnidad, Modelo3D modelo, CompraModelo3D compra)
    {
        CantidadLicencias = cantidadLicencias;
        PrecioUnidad = precioUnidad;
        Subtotal = cantidadLicencias * precioUnidad;
        Modelo = modelo;
        Compra = compra;
    }

    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad de licencias debe ser al menos 1.")]
    public int CantidadLicencias { get; set; }

    [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
    [Precision(10, 2)]
    public decimal PrecioUnidad { get; set; }

    [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
    [Precision(10, 2)]
    public decimal Subtotal { get; set; }

    [Required]
    public Modelo3D Modelo { get; set; } = null!;

    [Required]
    [PlantUmlIgnoreAssociation]
    public CompraModelo3D Compra { get; set; } = null!;
}