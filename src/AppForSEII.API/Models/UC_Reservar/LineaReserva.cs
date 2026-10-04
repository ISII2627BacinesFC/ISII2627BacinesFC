using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Display = System.ComponentModel.DataAnnotations.DisplayAttribute;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII.API.Models.UC_Reservar
{
    public class LineaReserva
    {
        public LineaReserva()
        {
        }

        public LineaReserva(TiempoReserva tiempoReserva, decimal precioSubtotal, Impresora3D impresora)
        {
            TiempoReserva = tiempoReserva;
            PrecioSubtotal = precioSubtotal;
            Impresora = impresora;
            ImpresoraId = impresora.Id;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tiempo de reserva")]
        public TiempoReserva TiempoReserva { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(6,2)")]
        [Range(0.01, 9999.99, ErrorMessage = "El precio subtotal debe ser positivo.")]
        [Display(Name = "Precio subtotal")]
        public decimal PrecioSubtotal { get; set; }

        [Required]
        public int ImpresoraId { get; set; }

        [ForeignKey("ImpresoraId")]
        public virtual Impresora3D Impresora { get; set; } = null!;

        [Required]
        public int ReservaId { get; set; }

        [ForeignKey("ReservaId")]
        public virtual ReservaImpresora Reserva { get; set; } = null!;
    }
}