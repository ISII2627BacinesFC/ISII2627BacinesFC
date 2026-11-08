using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.Data;
using AppForSEII.API.Models.UC_Reservar;
using AppForSEII.API.DTOs.Impresora3DDTOs;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReservasController> _logger;

        public ReservasController(ApplicationDbContext context, ILogger<ReservasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Reservas/GetImpresorasParaReserva?nombre=x&tipo=Filamento
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ImpresoraParaReservaDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetImpresorasParaReserva(string? nombre, TipoImpresora? tipo)
        {
            IList<ImpresoraParaReservaDTO> impresoras = await _context.Impresoras3D
                .Where(i => (nombre == null || i.Nombre.Contains(nombre))
                         && (tipo == null || i.Tipo == tipo))
                .OrderBy(i => i.Nombre)
                .Select(i => new ImpresoraParaReservaDTO(
                    i.Id,
                    i.Nombre,
                    i.Modelo,
                    i.Tipo,
                    i.Descripcion,
                    i.PrecioKilovatioHora,
                    i.PrecioReserva))
                .ToListAsync();

            return Ok(impresoras);
        }
    }
}