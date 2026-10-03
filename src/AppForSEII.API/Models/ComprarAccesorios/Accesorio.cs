namespace AppForSEII.API.Models.ComprarAccesorios
{
    public class Accesorio
    {
        public Accesorio()
        {
        }

        public Accesorio(string nombre, CategoriaAccesorio categoria, string compatibilidad, int cantidadDisponible, decimal precio)
        {
            Nombre = nombre;
            Categoria = categoria;
            Compatibilidad = compatibilidad;
            CantidadDisponible = cantidadDisponible;
            Precio = precio;
        }

        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "El nombre no puede tener mas de 50 caracteres.")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }  = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public CategoriaAccesorio Categoria { get; set; }

        [StringLength(100, ErrorMessage = "La compatibilidad no puede tener más de 100 caracteres.")]
        [Required(ErrorMessage = "La compatibilidad es obligatoria.")]
        public string Compatibilidad { get; set; }  = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad disponible no puede ser negativa.")]
        public int CantidadDisponible { get; set; }


        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0.")]
        [Precision(10, 2)]
        public decimal Precio { get; set; }
    }
}