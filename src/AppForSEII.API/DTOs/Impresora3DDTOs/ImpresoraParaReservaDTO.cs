using System;
using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models.UC_Reservar;

namespace AppForSEII.API.DTOs.Impresora3DDTOs;

public class ImpresoraParaReservaDTO
{
    public ImpresoraParaReservaDTO()
    {
    }

    public ImpresoraParaReservaDTO(int id, string nombre, string modelo, TipoImpresora tipo,
        string? descripcion, decimal precioKilovatioHora, decimal precioReserva)
    {
        Id = id;
        Nombre = nombre;
        Modelo = modelo;
        Tipo = tipo;
        Descripcion = descripcion;
        PrecioKilovatioHora = precioKilovatioHora;
        PrecioReserva = precioReserva;
    }

    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Modelo { get; set; } = null!;
    public TipoImpresora Tipo { get; set; }
    public string? Descripcion { get; set; }
    public decimal PrecioKilovatioHora { get; set; }
    public decimal PrecioReserva { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is ImpresoraParaReservaDTO dto &&
               Id == dto.Id &&
               Nombre == dto.Nombre &&
               Modelo == dto.Modelo &&
               Tipo == dto.Tipo &&
               Descripcion == dto.Descripcion &&
               PrecioKilovatioHora == dto.PrecioKilovatioHora &&
               PrecioReserva == dto.PrecioReserva;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Nombre, Modelo, Tipo, Descripcion, PrecioKilovatioHora, PrecioReserva);
    }
}