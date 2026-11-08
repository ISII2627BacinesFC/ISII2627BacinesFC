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

    [Required]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres ni menos de 1.", MinimumLength = 1)]
    public string Nombre { get; set; } = null!;

    [Required]
    [StringLength(50, ErrorMessage = "El modelo no puede tener más de 50 caracteres ni menos de 1.", MinimumLength = 1)]
    public string Modelo { get; set; } = null!;

    [Required]
    public TipoImpresora Tipo { get; set; }

    [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres.")]
    public string? Descripcion { get; set; }

    [Required]
    [Range(0.01, 999.99, ErrorMessage = "El precio por kilovatio/hora debe ser mayor que 0.")]
    public decimal PrecioKilovatioHora { get; set; }

    [Required]
    [Range(0.01, 999.99, ErrorMessage = "El precio de reserva debe ser mayor que 0.")]
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