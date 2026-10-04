using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models.UC_Reservar
{
    public class Impresora3D
    {
        public Impresora3D()
        {
            LineasReserva = new List<LineaReserva>();
        }

        public Impresora3D(string nombre, string modelo, TipoImpresora tipo, string? descripcion, decimal precioKilovatioHora, decimal precioReserva) : this()
        {
            Nombre = nombre;
            Modelo = modelo;
            Tipo = tipo;
            Descripcion = descripcion;
            PrecioKilovatioHora = precioKilovatioHora;
            PrecioReserva = precioReserva;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres ni menos de 1.", MinimumLength = 1)]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "El modelo no puede tener más de 50 caracteres ni menos de 1.", MinimumLength = 1)]
        public string Modelo { get; set; } = null!;

        [Required]
        public TipoImpresora Tipo { get; set; }

        // Opcional en el modelo (puede ser nulo)
        [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres.")]
        public string? Descripcion { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(5, 2)]
        [Range(0.01, 999.99, ErrorMessage = "El precio por kilovatio/hora debe ser mayor que 0.")]
        public decimal PrecioKilovatioHora { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(5, 2)]
        [Range(0.01, 999.99, ErrorMessage = "El precio de reserva debe ser mayor que 0.")]
        public decimal PrecioReserva { get; set; }

        public virtual ICollection<LineaReserva> LineasReserva { get; set; }
    }
}