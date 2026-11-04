using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using AppForSEII.API.Data;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.Pieza3DDTOs;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class Piezas3DController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Piezas3DController> _logger;
        public Piezas3DController(ApplicationDbContext appDbContext, ILogger<Piezas3DController> Logger)
        {
            _context = appDbContext;
            _logger = Logger;
        }

        // Método GET

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<Pieza3DForEncargoDTO>), (int)HttpStatusCode.OK)]

        public async Task<ActionResult<IList<Pieza3DForEncargoDTO>>> GetPiezas3DForEncargo(string? nombre, CategoriaPieza? categoria)
        {
           _logger.LogInformation("Consultando piezas 3D disponibles para encargo.");
            var piezas = await _context.Piezas3D
                .Include(p => p.MaterialesValidos)
                .ThenInclude(pm => pm.Material)
                .Where(p => (nombre == null || p.Nombre.Contains(nombre)) && (categoria == null || p.Categoria == categoria))
                .OrderBy(p => p.Nombre)
                .Select(p => new Pieza3DForEncargoDTO(
                    p.Id,
                    p.Nombre,
                    p.Categoria,
                    p.Peso,
                    p.MaterialesValidos.OrderBy(pm => pm.Material.PrecioPorGramo).Select(pm => pm.Material.Nombre).FirstOrDefault() ?? "Sin material",
                    p.Peso * p.MaterialesValidos.OrderBy(pm => pm.Material.PrecioPorGramo).Select(pm => pm.Material.PrecioPorGramo).FirstOrDefault()
                ))
                .ToListAsync();

            return Ok(piezas);
        }
        
    }
}
