namespace AppForSEII.API.Models.ComprarAccesorios
{
    public class LineaCompraAccesorio
    {
        public LineaCompraAccesorio()
        {
        }

        public LineaCompraAccesorio(int cantidad, decimal precioUnidad, Accesorio accesorio, CompraAccesorios compraAccesorios)
        {
            Cantidad = cantidad;
            PrecioUnidad = precioUnidad;
            Accesorio = accesorio;
            CompraAccesorios = compraAccesorios;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio por unidad es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio por unidad debe ser mayor que 0.")]
        [Precision(10, 2)]
        public decimal PrecioUnidad { get; set; }

        [Precision(10, 2)]
        public decimal Subtotal => Cantidad * PrecioUnidad;


        [PlantUmlIgnoreAssociation]
        public CompraAccesorios CompraAccesorios { get; set; } = null!;

        public Accesorio Accesorio { get; set; } = null!;
    }
}