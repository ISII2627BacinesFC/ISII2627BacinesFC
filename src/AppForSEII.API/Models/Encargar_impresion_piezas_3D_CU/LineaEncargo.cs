namespace AppForSEII.API.Models
{
    public class LineaEncargo
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnidad { get; set; }
        public decimal Subtotal { get; set; }

        // Relación con Pieza3D (rol: pieza)
        public int Pieza3DId { get; set; }
        public Pieza3D Pieza { get; set; } = null!;

        // Relación con Material (rol: material seleccionado)
        public int MaterialId { get; set; }
        public Material MaterialSeleccionado { get; set; } = null!;

        // Relación con EncargoImpresion
        //public int EncargoImpresionId { get; set; }
        //public EncargoImpresion EncargoImpresion { get; set; } = null!;
    }
}