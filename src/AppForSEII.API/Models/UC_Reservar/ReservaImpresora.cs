using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using DataType = System.ComponentModel.DataAnnotations.DataType;
using Display = System.ComponentModel.DataAnnotations.DisplayAttribute;

namespace AppForSEII.API.Models.UC_Reservar
{
    public class ReservaImpresora
    {
        public ReservaImpresora()
        {
            LineasReserva = new List<LineaReserva>();
        }

        public ReservaImpresora(
            DateTime fechaReserva,
            string nombreCliente,
            string apellidosCliente,
            string direccionFacturacion,
            decimal precioTotal,
            MetodoPago metodoPago,
            Client client) : this()
        {
            FechaReserva = fechaReserva;
            NombreCliente = nombreCliente;
            ApellidosCliente = apellidosCliente;
            DireccionFacturacion = direccionFacturacion;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            Cliente = client;
            ClienteId = client.Id;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de reserva")]
        public DateTime FechaReserva { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El nombre del cliente no puede tener más de 50 caracteres ni menos de 1.", MinimumLength = 1)]
        [Display(Name = "Nombre del cliente")]
        public string NombreCliente { get; set; }= null!;

        [Required]
        [StringLength(100, ErrorMessage = "Los apellidos del cliente no pueden tener más de 100 caracteres ni menos de 1.", MinimumLength = 1)]
        [Display(Name = "Apellidos del cliente")]
        public string ApellidosCliente { get; set; }= null!;

        [Required]
        [StringLength(150, ErrorMessage = "La dirección de facturación no puede superar los 150 caracteres.")]
        [Display(Name = "Dirección de facturación")]
        public string DireccionFacturacion { get; set; }= null!;

        [Required]
        [DataType(DataType.Currency)]
        [Precision(7, 2)]
        [Range(0.01, 99999.99, ErrorMessage = "El precio total debe ser mayor que cero.")]
        [Display(Name = "Precio total")]
        public decimal PrecioTotal { get; set; }

        [Required]
        [Display(Name = "Método de pago")]
        public MetodoPago MetodoPago { get; set; }

        public string ClienteId { get; set; }= null!;

        [ForeignKey("ClienteId")]
        public virtual Client Cliente { get; set; } = null!;

        public virtual ICollection<LineaReserva> LineasReserva { get; set; }
    }
}