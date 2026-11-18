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

        // POST: api/ComprasAccesorios/CreateCompraAccesorios
        // Pasos 5 y 6: el cliente confirma y paga la compra
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraAccesoriosDetalleDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateCompraAccesorios(CompraAccesoriosParaCrearDTO compraParaCrear)
        {
            // Las validaciones de los campos obligatorios (FA 7.1) las hace [ApiController]
            // con los atributos del DTO antes de entrar en el método.

            // FA 4: no se puede continuar sin ningún accesorio en el carrito
            if (compraParaCrear.Lineas.Count == 0)
                ModelState.AddModelError("Lineas", "¡Error! Debes incluir al menos un accesorio en la compra");

            // Precondición: el cliente debe estar registrado
            Client? cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.UserName == compraParaCrear.UserNameCliente);
            if (cliente == null)
                ModelState.AddModelError("Cliente", "¡Error! El usuario no está registrado");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var idsAccesorios = compraParaCrear.Lineas.Select(l => l.AccesorioId).ToList();
            var accesorios = await _context.Accesorios
                .Where(a => idsAccesorios.Contains(a.Id))
                .ToListAsync();

            var compra = new CompraAccesorios(DateTime.Now, compraParaCrear.NombreCliente,
                compraParaCrear.ApellidosCliente, compraParaCrear.DireccionEnvio,
                compraParaCrear.NumeroTelefono, 0, compraParaCrear.MetodoPago, cliente!);

            foreach (var linea in compraParaCrear.Lineas)
            {
                var accesorio = accesorios.FirstOrDefault(a => a.Id == linea.AccesorioId);

                if (accesorio == null)
                {
                    ModelState.AddModelError("Lineas", $"¡Error! El accesorio '{linea.Nombre}' no existe");
                }
                // FA 7.3: no hay stock suficiente del accesorio
                else if (accesorio.CantidadDisponible < linea.Cantidad)
                {
                    ModelState.AddModelError("Lineas", $"¡Error! No hay stock suficiente del accesorio '{accesorio.Nombre}'. Cantidad máxima disponible: {accesorio.CantidadDisponible}");
                }
                else
                {
                    // la compra aún no está en la base de datos, así que relacionamos la línea con el objeto compra
                    compra.LineasCompraAccesorio.Add(new LineaCompraAccesorio(linea.Cantidad, accesorio.Precio, accesorio, compra));
                    accesorio.CantidadDisponible -= linea.Cantidad;
                }
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            compra.PrecioTotal = compra.LineasCompraAccesorio.Sum(l => l.Subtotal);

            _context.Add(compra);

            try
            {
                // guarda la compra, sus líneas y el nuevo stock de los accesorios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Conflict("Error! Ha habido un problema al guardar la compra, inténtalo más tarde. " + ex.Message);
            }

            var compraDetalle = new CompraAccesoriosDetalleDTO(compra.Id, compra.FechaCompra,
                compraParaCrear.UserNameCliente, compra.NombreCliente, compra.ApellidosCliente,
                compra.DireccionEnvio, compra.NumeroTelefono, compra.MetodoPago,
                compra.LineasCompraAccesorio.Select(l => new LineaCompraAccesorioDTO(l.Accesorio.Id,
                    l.Accesorio.Nombre, l.Accesorio.Categoria, l.Accesorio.Compatibilidad,
                    l.Cantidad, l.PrecioUnidad)).ToList());

            return CreatedAtAction(nameof(GetCompraAccesorios), new { id = compra.Id }, compraDetalle);
        }
    }
}
