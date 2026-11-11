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

        //GET encargo impresion
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

        //Mi post:
        [HttpPost]
        public async Task<ActionResult<EncargoImpresionDetalleDTO>> CreateEncargoImpresion(EncargoImpresionCreateDTO dto)
        {
            _logger.LogInformation("Creando un nuevo encargo de impresión para {Nombre} {Apellidos}", dto.NombreCliente, dto.ApellidosCliente);
            if (dto.Lineas == null || !dto.Lineas.Any())
            {
                return BadRequest("El encargo debe contener al menos una línea.");
            }

            if (!Enum.TryParse<MetodoPago>(dto.MetodoPago, true, out var metodoPagoEnum))
            {
                return BadRequest($"Método de pago no válido: {dto.MetodoPago}");
            }

            var cliente = await _context.Users.OfType<Client>().FirstOrDefaultAsync();
            if (cliente == null)
            {
                return BadRequest("No existe ningún cliente registrado para asociar al encargo.");
            }
            // Prueba autoria Sprint2
            if (dto.NumeroTelefono.Length != 9)
            {
                return BadRequest("¡Error! El teléfono debe tener 9 dígitos.");
            }

            var encargo = new EncargoImpresion
            {
                FechaEncargo = DateTime.Now,
                NombreCliente = dto.NombreCliente,
                ApellidosCliente = dto.ApellidosCliente,
                DireccionEnvio = dto.DireccionEnvio,
                NumeroTelefono = dto.NumeroTelefono,
                Descripcion = dto.Descripcion,
                MetodoPago = metodoPagoEnum,
                ClienteId = cliente.Id,
                Cliente = cliente,
                LineasEncargo = new List<LineaEncargo>()
            };

            decimal precioTotalEncargo = 0m;

            foreach (var item in dto.Lineas)
            {
                var pieza = await _context.Piezas3D
                    .Include(p => p.MaterialesValidos)
                        .ThenInclude(pm => pm.Material)
                    .FirstOrDefaultAsync(p => p.Id == item.PiezaId);

                if (pieza == null)
                {
                    return NotFound($"Pieza3D con ID {item.PiezaId} no encontrada.");
                }

                // Verifico que el material elegido sea apto para la pieza
                var relacionMaterial = pieza.MaterialesValidos
                    .FirstOrDefault(pm => pm.MaterialId == item.MaterialId);

                if (relacionMaterial == null || relacionMaterial.Material == null)
                {
                    return BadRequest($"El material con ID {item.MaterialId} no es válido para la pieza '{pieza.Nombre}'.");
                }

                var material = relacionMaterial.Material;

                // Calculo el precio unitario y subtotal según peso y material
                decimal precioUnidad = pieza.Peso * material.PrecioPorGramo;
                decimal subtotal = precioUnidad * item.Cantidad;
                precioTotalEncargo += subtotal;

                var linea = new LineaEncargo
                {
                    Pieza3DId = pieza.Id,
                    Pieza = pieza,
                    MaterialId = material.ID,
                    MaterialSeleccionado = material,
                    Cantidad = item.Cantidad,
                    PrecioUnidad = precioUnidad,
                    Subtotal = subtotal,
                    EncargoImpresion = encargo
                };

                encargo.LineasEncargo.Add(linea);
            }

            encargo.PrecioTotal = precioTotalEncargo;

            //Guardo en la base de datos
            _context.EncargosImpresion.Add(encargo);
            await _context.SaveChangesAsync();
            //Proyecto el DTO
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
            return CreatedAtAction(nameof(GetEncargoImpresion), new { id = encargo.Id }, detalleDTO);
        }
    }
}