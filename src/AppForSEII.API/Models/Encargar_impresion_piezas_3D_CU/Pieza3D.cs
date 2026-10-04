using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    public class Pieza3D
    {
        public Pieza3D() {}

        public Pieza3D(string nombre, decimal peso, CategoriaPieza categoria)
        {
            Nombre = nombre;
            Peso = peso;
            Categoria = categoria;
        }

        [Key]
        public int Id {get;set;}
        
        [Required]
        [StringLength(100, ErrorMessage = "El nombre de la pieza no puede superar los 100 caracteres.")]
        public string Nombre {get;set;} = string.Empty;

        [Required]
        [Precision(10,2)]
        [Range(0.1, 50000.0, ErrorMessage = "El peso debe ser mayor a 0.")]
        public decimal Peso {get;set;}

        [Required]
        public CategoriaPieza Categoria {get;set;}

        public ICollection<Material> MaterialesValidos {get;set;} = new HashSet<Material>();

    }      
}
