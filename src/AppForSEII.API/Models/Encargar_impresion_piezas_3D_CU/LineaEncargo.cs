using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    public class LineaEncargo
    {
        public LineaEncargo() {}

        public LineaEncargo(int cantidad, decimal precioUnidad, decimal subtotal, Pieza3D pieza, Material materialSeleccionado)
        {
            Cantidad = cantidad;
            PrecioUnidad = precioUnidad;
            Subtotal = subtotal;
            Pieza = pieza;
            MaterialSeleccionado = materialSeleccionado;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad minima debe ser 1.")]
        public int Cantidad { get; set; }

        [Required]
        [Precision(10,2)]
        public decimal PrecioUnidad { get; set; }

        [Required]
        [Precision(10,2)]
        public decimal Subtotal { get; set; }

        // Relación con Pieza3D (rol: pieza)
        [Required]
        public int Pieza3DId { get; set; }

        [ForeignKey(nameof(Pieza3DId))]
        public Pieza3D Pieza { get; set; } = null!;

        // Relación con Material (rol: material seleccionado)
        [Required]
        public int MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material MaterialSeleccionado { get; set; } = null!;

        // Relación con EncargoImpresion
        public int EncargoImpresionId { get; set; }

        [ForeignKey(nameof(EncargoImpresionId))]
        public EncargoImpresion EncargoImpresion { get; set; } = null!;
    }
}