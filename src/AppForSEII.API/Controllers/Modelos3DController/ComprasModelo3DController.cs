using AppForSEII.API.DTOs.Modelo3DDTOs;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasModelo3DController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ComprasModelo3DController> _logger;

        public ComprasModelo3DController(ApplicationDbContext context, ILogger<ComprasModelo3DController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ComprasModelo3D/5
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CompraModelo3DDetalleDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetCompraModelo3D(int id)
        {
            CompraModelo3D? compra = await _context.ComprasModelo3D
                .Include(c => c.Lineas)
                    .ThenInclude(l => l.Modelo)
                        .ThenInclude(m => m.Licencia)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
            {
                _logger.LogError($"Error: la compra de modelos 3D con id {id} no existe");
                return NotFound();
            }

            var compraDTO = new CompraModelo3DDetalleDTO(compra.Id, compra.NombreCliente, compra.ApellidosCliente,
                compra.CorreoElectronico, compra.DireccionFacturacion, compra.FechaCompra, compra.PrecioTotal,
                compra.MetodoPago, compra.Descripcion,
                compra.Lineas.Select(l => new LineaCompraModeloDTO(l.Modelo.Id, l.Modelo.Nombre, l.Modelo.Categoria,
                    l.Modelo.Formato, l.Modelo.Licencia.Nombre, l.Modelo.Licencia.FechaExpiracion,
                    l.CantidadLicencias, l.PrecioUnidad, l.Subtotal)).ToList());

            return Ok(compraDTO);
        }
    }
}