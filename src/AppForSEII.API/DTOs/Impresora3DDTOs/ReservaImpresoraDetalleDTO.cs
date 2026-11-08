using System;
using System.Collections.Generic;
using System.Linq;
using AppForSEII.API.Models;
using AppForSEII.API.Models.UC_Reservar;

namespace AppForSEII.API.DTOs.Impresora3DDTOs
{
    public class ReservaImpresoraDetalleDTO
    {
        public ReservaImpresoraDetalleDTO()
        {
        }

        public ReservaImpresoraDetalleDTO(
            int id,
            string nombreCliente,
            string apellidosCliente,
            string direccionFacturacion,
            DateTime fechaReserva,
            decimal precioTotal,
            MetodoPago metodoPago,
            IList<LineaReservaImpresoraDTO> lineas)
        {
            Id = id;
            NombreCliente = nombreCliente;
            ApellidosCliente = apellidosCliente;
            DireccionFacturacion = direccionFacturacion;
            FechaReserva = fechaReserva;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            Lineas = lineas;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Error! El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "Error! El nombre no puede tener más de 50 caracteres")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Error! Los apellidos son obligatorios")]
        [StringLength(50, ErrorMessage = "Error! Los apellidos no pueden tener más de 50 caracteres")]
        public string ApellidosCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Error! La dirección de facturación es obligatoria")]
        [StringLength(100, ErrorMessage = "Error! La dirección de facturación no puede tener más de 100 caracteres")]
        public string DireccionFacturacion { get; set; } = string.Empty;

        public DateTime FechaReserva { get; set; }

        public decimal PrecioTotal { get; set; }

        [Required(ErrorMessage = "Error! El método de pago no es válido")]
        public MetodoPago MetodoPago { get; set; }

        [Required(ErrorMessage = "Error! Debes incluir al menos una impresora para reservar")]
        public IList<LineaReservaImpresoraDTO> Lineas { get; set; } = new List<LineaReservaImpresoraDTO>();

        public override bool Equals(object? obj)
        {
            return obj is ReservaImpresoraDetalleDTO dto &&
                   Id == dto.Id &&
                   NombreCliente == dto.NombreCliente &&
                   ApellidosCliente == dto.ApellidosCliente &&
                   DireccionFacturacion == dto.DireccionFacturacion &&
                   FechaReserva == dto.FechaReserva &&
                   PrecioTotal == dto.PrecioTotal &&
                   MetodoPago == dto.MetodoPago &&
                   Lineas.SequenceEqual(dto.Lineas);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Id);
            hash.Add(NombreCliente);
            hash.Add(ApellidosCliente);
            hash.Add(DireccionFacturacion);
            hash.Add(FechaReserva);
            hash.Add(PrecioTotal);
            hash.Add(MetodoPago);
            return hash.ToHashCode();
        }
    }
}