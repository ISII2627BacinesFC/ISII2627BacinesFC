using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    public class Material
    {
        public Material(){}

        public Material(string nombre, decimal precioPorGramo, decimal stockGramos)
        {
            Nombre = nombre;
            PrecioPorGramo = precioPorGramo;
            StockGramos = stockGramos;
        }

        [Key]
        public int ID {get;set;}

        [Required]
        [StringLength(50, ErrorMessage = "El nombre del material no debe superar los 50 caracteres.")]
        public string Nombre {get;set;} = string.Empty;

        [Required]
        [Precision(10,2)]
        [Range(0.01, 1000.0, ErrorMessage = "El precio por gramo debe de ser positivo.")]
        public decimal PrecioPorGramo {get;set;}

        [Required]
        [Precision(12,2)]
        public decimal StockGramos{get;set;}
    }
}