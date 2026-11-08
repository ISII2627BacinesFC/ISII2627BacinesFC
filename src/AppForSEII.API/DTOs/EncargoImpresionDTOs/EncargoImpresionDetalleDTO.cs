using System;
using System.Collections.Generic;
using System.Linq;

namespace AppForSEII.API.DTOs.EncargoImpresionDTOs
{
    public class EncargoImpresionDetalleDTO
    {
        public EncargoImpresionDetalleDTO()
        {
            Lineas = new List<LineaEncargoImpresionDTO>();
        }
        public EncargoImpresionDetalleDTO(int id, string nombreCliente, string apellidosCliente, string direccionEnvio, string numeroTelefono, string? descripcion, decimal precioTotal, DateTime fechaEncargo, string metodoPago, IList<LineaEncargoImpresionDTO> lineas)
        {
            Id = id;
            NombreCliente = nombreCliente;
            ApellidosCliente = apellidosCliente;
            DireccionEnvio = direccionEnvio;
            NumeroTelefono = numeroTelefono;
            Descripcion = descripcion;
            PrecioTotal = precioTotal;
            FechaEncargo = fechaEncargo;
            MetodoPago = metodoPago;
            Lineas = lineas ?? new List<LineaEncargoImpresionDTO>();
        }

        public int Id { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidosCliente { get; set; } = string.Empty;
        public string DireccionEnvio { get; set; } = string.Empty;
        public string NumeroTelefono { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal PrecioTotal { get; set; }
        public DateTime FechaEncargo { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public IList<LineaEncargoImpresionDTO> Lineas { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is EncargoImpresionDetalleDTO dto &&
                Id == dto.Id &&
                NombreCliente == dto.NombreCliente &&
                ApellidosCliente == dto.ApellidosCliente &&
                DireccionEnvio == dto.DireccionEnvio &&
                NumeroTelefono == dto.NumeroTelefono &&
                Descripcion == dto.Descripcion &&
                PrecioTotal == dto.PrecioTotal &&
                FechaEncargo == dto.FechaEncargo &&
                MetodoPago == dto.MetodoPago &&
                Lineas.SequenceEqual(dto.Lineas);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Id);
            hash.Add(NombreCliente);
            hash.Add(ApellidosCliente);
            hash.Add(DireccionEnvio);
            hash.Add(NumeroTelefono);
            hash.Add(Descripcion);
            hash.Add(PrecioTotal);
            hash.Add(FechaEncargo);
            hash.Add(MetodoPago);
            return hash.ToHashCode();
        }
    }
}