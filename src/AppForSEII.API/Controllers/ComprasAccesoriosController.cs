using AppForSEII.API.DTOs.Accesorios3DDTOs;
using AppForSEII.API.Models.ComprarAccesorios;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasAccesoriosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ComprasAccesoriosController> _logger;

        public ComprasAccesoriosController(ApplicationDbContext context, ILogger<ComprasAccesoriosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ComprasAccesorios/5
        // Paso 7: muestra la compra realizada con los datos del cliente y los accesorios comprados
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CompraAccesoriosDetalleDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetCompraAccesorios(int id)
        {
            CompraAccesorios? compra = await _context.ComprasAccesorios
                .Include(c => c.Cliente)
                .Include(c => c.LineasCompraAccesorio)
                    .ThenInclude(l => l.Accesorio)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
            {
                _logger.LogError($"Error: la compra de accesorios con id {id} no existe");
                return NotFound();
            }

            var compraDTO = new CompraAccesoriosDetalleDTO(compra.Id, compra.FechaCompra,
                compra.Cliente.UserName ?? string.Empty, compra.NombreCliente, compra.ApellidosCliente,
                compra.DireccionEnvio, compra.NumeroTelefono, compra.MetodoPago,
                compra.LineasCompraAccesorio.Select(l => new LineaCompraAccesorioDTO(l.Accesorio.Id,
                    l.Accesorio.Nombre, l.Accesorio.Categoria, l.Accesorio.Compatibilidad,
                    l.Cantidad, l.PrecioUnidad)).ToList());

            return Ok(compraDTO);
        }
    }
}
