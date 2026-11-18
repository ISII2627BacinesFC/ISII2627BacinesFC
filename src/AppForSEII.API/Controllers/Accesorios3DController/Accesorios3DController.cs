using AppForSEII.API.DTOs.Accesorios3DDTOs;
using AppForSEII.API.Models.ComprarAccesorios;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Accesorios3DController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Accesorios3DController> _logger;

        public Accesorios3DController(ApplicationDbContext context, ILogger<Accesorios3DController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Accesorios3D/GetAccesorios3DParaComprar?nombre=x&categoria=y
        // Paso 2: lista de accesorios con stock (FA 2.1) filtrados por nombre y categoría (FA 2.2)
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<Accesorio3DParaComprarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetAccesorios3DParaComprar(string? nombre, CategoriaAccesorio? categoria)
        {
            IList<Accesorio3DParaComprarDTO> accesorios = await _context.Accesorios
                .Where(a => a.CantidadDisponible > 0
                         && (nombre == null || a.Nombre.Contains(nombre))
                         && (categoria == null || a.Categoria == categoria))
                .OrderBy(a => a.Nombre)
                .Select(a => new Accesorio3DParaComprarDTO(a.Id, a.Nombre, a.Categoria,
                    a.Compatibilidad, a.CantidadDisponible, a.Precio))
                .ToListAsync();

            return Ok(accesorios);
        }
    }
}
