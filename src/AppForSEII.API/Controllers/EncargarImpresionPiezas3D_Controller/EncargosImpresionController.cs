using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.EncargoImpresionDTOs;

namespace AppForSEII.API.Controllers.EncargarImpresionPiezas3D_Controller
{

    [Route("api/[controller]")]
    [ApiController]
    public class EncargosImpresionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EncargosImpresionController> _logger;
        public EncargosImpresionController(ApplicationDbContext context, ILogger<EncargosImpresionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //GET encargo impresión
        [HttpGet("{id}")]
        public async Task<ActionResult<EncargoImpresionDetalleDTO>> GetEncargoImpresion(int id)
        {
            _logger.LogInformation($"Consultando detalle del encargo de impresion con ID: {id}");
            var encargo = await _context.EncargosImpresion
                .Include(e => e.LineasEncargo)
                    .ThenInclude(l => l.Pieza)
                .Include(e => e.LineasEncargo)
                    .ThenInclude(l => l.MaterialSeleccionado)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (encargo == null)
            {
                _logger.LogWarning("Encargo de impresión con ID {Id} no encontrado", id);
                return NotFound();
            }

            var detalleDTO = new EncargoImpresionDetalleDTO(
            encargo.Id,
            encargo.NombreCliente,
            encargo.ApellidosCliente,
            encargo.DireccionEnvio,
            encargo.NumeroTelefono,
            encargo.Descripcion,
            encargo.PrecioTotal,
            encargo.FechaEncargo,
            encargo.MetodoPago.ToString(),
            encargo.LineasEncargo.Select(l => new LineaEncargoImpresionDTO(
                l.Id,
                l.Pieza.Nombre,
                l.MaterialSeleccionado.Nombre,
                l.Cantidad,
                l.PrecioUnidad,
                l.Subtotal
            )).ToList()
        );
        return Ok(detalleDTO);

        }
    }
}