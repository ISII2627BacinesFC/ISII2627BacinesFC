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

        public string NombreCliente { get; set; } = string.Empty;

        public string ApellidosCliente { get; set; } = string.Empty;

        public string DireccionFacturacion { get; set; } = string.Empty;

        public DateTime FechaReserva { get; set; }

        public decimal PrecioTotal { get; set; }

        public MetodoPago MetodoPago { get; set; }

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