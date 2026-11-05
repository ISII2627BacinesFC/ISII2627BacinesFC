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

        // POST: api/ComprasModelo3D/CreateCompraModelo3D
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraModelo3DDetalleDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateCompraModelo3D(CompraModelo3DDetalleDTO compraParaCrear)
        {
            if (compraParaCrear.Lineas.Count == 0)
                ModelState.AddModelError("Lineas", "Error! Debes incluir al menos un modelo 3D para comprar");

            if (compraParaCrear.MetodoPago == MetodoPago.Bizum)
                ModelState.AddModelError("MetodoPago", "Error! El método de pago debe ser tarjeta de crédito o PayPal");

            Client? cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.UserName == compraParaCrear.CorreoElectronico);

            if (cliente == null)
                ModelState.AddModelError("CorreoElectronico", "Error! No existe ningún cliente con ese correo electrónico");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var compra = new CompraModelo3D(DateTime.Now, compraParaCrear.NombreCliente, compraParaCrear.ApellidosCliente,
                compraParaCrear.CorreoElectronico, compraParaCrear.DireccionFacturacion, compraParaCrear.Descripcion,
                0, compraParaCrear.MetodoPago, cliente!);

            foreach (var lineaParaCrear in compraParaCrear.Lineas)
            {
                Modelo3D? modelo = await _context.Modelos3D
                    .Include(m => m.Licencia)
                    .FirstOrDefaultAsync(m => m.Id == lineaParaCrear.ModeloId);

                if (modelo == null)
                    ModelState.AddModelError("Lineas", $"Error! El modelo 3D con id {lineaParaCrear.ModeloId} no existe");
                else if (lineaParaCrear.CantidadLicencias < 1)
                    ModelState.AddModelError("Lineas", $"Error! La cantidad de licencias del modelo {modelo.Nombre} debe ser al menos 1");
                else
                    compra.Lineas.Add(new LineaCompraModelo(lineaParaCrear.CantidadLicencias, modelo.Precio, modelo, compra));
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            compra.PrecioTotal = compra.Lineas.Sum(l => l.Subtotal);

            _context.Add(compra);
            await _context.SaveChangesAsync();

            var compraDetalle = new CompraModelo3DDetalleDTO(compra.Id, compra.NombreCliente, compra.ApellidosCliente,
                compra.CorreoElectronico, compra.DireccionFacturacion, compra.FechaCompra, compra.PrecioTotal,
                compra.MetodoPago, compra.Descripcion,
                compra.Lineas.Select(l => new LineaCompraModeloDTO(l.Modelo.Id, l.Modelo.Nombre, l.Modelo.Categoria,
                    l.Modelo.Formato, l.Modelo.Licencia.Nombre, l.Modelo.Licencia.FechaExpiracion,
                    l.CantidadLicencias, l.PrecioUnidad, l.Subtotal)).ToList());

            return CreatedAtAction(nameof(GetCompraModelo3D), new { id = compra.Id }, compraDetalle);
        }
    }
}