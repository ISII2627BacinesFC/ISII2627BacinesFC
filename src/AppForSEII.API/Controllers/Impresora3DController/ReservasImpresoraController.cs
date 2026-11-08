using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.Impresora3DDTOs;
using AppForSEII.API.Models;
using AppForSEII.API.Models.UC_Reservar;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasImpresoraController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReservasImpresoraController> _logger;

        public ReservasImpresoraController(ApplicationDbContext context, ILogger<ReservasImpresoraController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ReservasImpresora/5
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ReservaImpresoraDetalleDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetReservaImpresora3D(int id)
        {
            ReservaImpresora? reserva = await _context.ReservasImpresora
                .Include(r => r.LineasReserva)
                    .ThenInclude(l => l.Impresora)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
            {
                _logger.LogError($"Error: la reserva de impresora 3D con id {id} no existe");
                return NotFound();
            }

            var reservaDTO = new ReservaImpresoraDetalleDTO(
                reserva.Id,
                reserva.NombreCliente,
                reserva.ApellidosCliente,
                reserva.DireccionFacturacion,
                reserva.FechaReserva,
                reserva.PrecioTotal,
                reserva.MetodoPago,
                reserva.LineasReserva.Select(l => new LineaReservaImpresoraDTO(
                    l.Impresora.Id,
                    l.Impresora.Nombre,
                    l.Impresora.Modelo,
                    l.Impresora.Tipo,
                    l.Impresora.Descripcion,
                    l.Impresora.PrecioKilovatioHora,
                    l.Impresora.PrecioReserva,
                    l.TiempoReserva,
                    l.PrecioSubtotal
                )).ToList()
            );

            return Ok(reservaDTO);
        }
    }
}