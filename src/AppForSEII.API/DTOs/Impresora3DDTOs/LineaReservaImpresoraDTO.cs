using System;
using AppForSEII.API.Models;
using AppForSEII.API.Models.UC_Reservar;

namespace AppForSEII.API.DTOs.Impresora3DDTOs
{
    public class LineaReservaImpresoraDTO
    {
        public LineaReservaImpresoraDTO()
        {
        }

        public LineaReservaImpresoraDTO(
            int impresoraId,
            string nombre,
            string modelo,
            TipoImpresora tipo,
            string? descripcion,
            decimal precioKilovatioHora,
            decimal precioReserva,
            TiempoReserva tiempoReserva,
            decimal precioSubtotal)
        {
            ImpresoraId = impresoraId;
            Nombre = nombre;
            Modelo = modelo;
            Tipo = tipo;
            Descripcion = descripcion;
            PrecioKilovatioHora = precioKilovatioHora;
            PrecioReserva = precioReserva;
            TiempoReserva = tiempoReserva;
            PrecioSubtotal = precioSubtotal;
        }

        public int ImpresoraId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public TipoImpresora Tipo { get; set; }

        public string? Descripcion { get; set; }

        public decimal PrecioKilovatioHora { get; set; }

        public decimal PrecioReserva { get; set; }

        public TiempoReserva TiempoReserva { get; set; }

        public decimal PrecioSubtotal { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is LineaReservaImpresoraDTO dto &&
                   ImpresoraId == dto.ImpresoraId &&
                   Nombre == dto.Nombre &&
                   Modelo == dto.Modelo &&
                   Tipo == dto.Tipo &&
                   Descripcion == dto.Descripcion &&
                   PrecioKilovatioHora == dto.PrecioKilovatioHora &&
                   PrecioReserva == dto.PrecioReserva &&
                   TiempoReserva == dto.TiempoReserva &&
                   PrecioSubtotal == dto.PrecioSubtotal;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(ImpresoraId);
            hash.Add(Nombre);
            hash.Add(Modelo);
            hash.Add(Tipo);
            hash.Add(Descripcion);
            hash.Add(PrecioKilovatioHora);
            hash.Add(PrecioReserva);
            hash.Add(TiempoReserva);
            hash.Add(PrecioSubtotal);
            return hash.ToHashCode();
        }
    }
}