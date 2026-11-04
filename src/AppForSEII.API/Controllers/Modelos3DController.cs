using AppForSEII.API.DTOs.Modelo3DDTOs;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Modelos3DController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Modelos3DController> _logger;

        public Modelos3DController(ApplicationDbContext context, ILogger<Modelos3DController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Modelos3D/GetModelos3DParaComprar?nombre=x&categoria=y
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<Modelo3DParaComprarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetModelos3DParaComprar(string? nombre, string? categoria)
        {
            IList<Modelo3DParaComprarDTO> modelos = await _context.Modelos3D
                .Include(m => m.Licencia)
                .Where(m => (nombre == null || m.Nombre.Contains(nombre))
                         && (categoria == null || m.Categoria == categoria))
                .OrderBy(m => m.Nombre)
                .Select(m => new Modelo3DParaComprarDTO(m.Id, m.Nombre, m.Categoria,
                    m.Licencia.Nombre, m.Formato, m.Precio))
                .ToListAsync();

            return Ok(modelos);
        }
    }
}