using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.Impresora3DDTOs;
using AppForSEII.API.Models;
using AppForSEII.API.Models.UC_Reservar;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasImpresoraController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReservasImpresoraController> _logger;

        public ReservasImpresoraController(ApplicationDbContext context, ILogger<ReservasImpresoraController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ReservasImpresora/5
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ReservaImpresoraDetalleDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetReservaImpresora3D(int id)
        {
            ReservaImpresora? reserva = await _context.ReservasImpresora
                .Include(r => r.LineasReserva)
                    .ThenInclude(l => l.Impresora)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
            {
                _logger.LogError($"Error: la reserva de impresora 3D con id {id} no existe");
                return NotFound();
            }

            var reservaDTO = new ReservaImpresoraDetalleDTO(
                reserva.Id,
                reserva.NombreCliente,
                reserva.ApellidosCliente,
                reserva.DireccionFacturacion,
                reserva.FechaReserva,
                reserva.PrecioTotal,
                reserva.MetodoPago,
                reserva.LineasReserva.Select(l => new LineaReservaImpresoraDTO(
                    l.Impresora.Id,
                    l.Impresora.Nombre,
                    l.Impresora.Modelo,
                    l.Impresora.Tipo,
                    l.Impresora.Descripcion,
                    l.Impresora.PrecioKilovatioHora,
                    l.Impresora.PrecioReserva,
                    l.TiempoReserva,
                    l.PrecioSubtotal
                )).ToList()
            );

            return Ok(reservaDTO);
        }
       // POST: api/ReservasImpresora/CreateReservaImpresora3D
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ReservaImpresoraDetalleDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateReservaImpresora3D(ReservaImpresoraDetalleDTO reservaParaCrear)
        {
            // Flujo Alternativo 5: Validar que se incluye al menos una impresora
            if (reservaParaCrear.Lineas == null || reservaParaCrear.Lineas.Count == 0)
                ModelState.AddModelError("Lineas", "Error! Debes incluir al menos una impresora para reservar");

            // Flujo Alternativo 7.1: Validación de campos obligatorios del cliente
            if (string.IsNullOrWhiteSpace(reservaParaCrear.NombreCliente))
                ModelState.AddModelError("NombreCliente", "Error! El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(reservaParaCrear.ApellidosCliente))
                ModelState.AddModelError("ApellidosCliente", "Error! Los apellidos son obligatorios");

            if (string.IsNullOrWhiteSpace(reservaParaCrear.DireccionFacturacion))
                ModelState.AddModelError("DireccionFacturacion", "Error! La dirección de facturación es obligatoria");

            // Flujo Alternativo 7.1: Validación de método de pago válido en el enum
            if (!Enum.IsDefined(typeof(MetodoPago), reservaParaCrear.MetodoPago))
                ModelState.AddModelError("MetodoPago", "Error! El método de pago no es válido");

            // Precondición: Búsqueda del cliente por Name y Surname (ApplicationUser)
            Client? cliente = null;
            if (!string.IsNullOrWhiteSpace(reservaParaCrear.NombreCliente) && !string.IsNullOrWhiteSpace(reservaParaCrear.ApellidosCliente))
            {
                cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Name == reservaParaCrear.NombreCliente && c.Surname == reservaParaCrear.ApellidosCliente);

                if (cliente == null)
                    ModelState.AddModelError("Cliente", $"Error! El cliente '{reservaParaCrear.NombreCliente} {reservaParaCrear.ApellidosCliente}' no existe en la base de datos.");
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Instancia de ReservaImpresora con total inicial a 0
            var reserva = new ReservaImpresora(
                DateTime.Now,
                reservaParaCrear.NombreCliente,
                reservaParaCrear.ApellidosCliente,
                reservaParaCrear.DireccionFacturacion,
                0m,
                reservaParaCrear.MetodoPago,
                cliente!);

            foreach (var lineaParaCrear in reservaParaCrear.Lineas)
            {
                // Búsqueda usando ImpresoraId de LineaReservaImpresoraDTO
                Impresora3D? impresora = await _context.Impresoras3D
                    .FirstOrDefaultAsync(i => i.Id == lineaParaCrear.ImpresoraId);

                // Flujo Alternativo 7.4: Comprobar existencia de la impresora
                if (impresora == null)
                {
                    ModelState.AddModelError("Lineas", $"Error! La impresora con id {lineaParaCrear.ImpresoraId} no existe");
                    continue;
                }

                // Flujo Alternativo 7.3: Obtener número de horas según el enum
                int horas = ObtenerHoras(lineaParaCrear.TiempoReserva);
                if (horas <= 0)
                {
                    ModelState.AddModelError("Lineas", $"Error! El tiempo de reserva para la impresora {impresora.Nombre} no es válido");
                    continue;
                }

                // Subtotal = horas * PrecioReserva
                decimal subtotal = horas * impresora.PrecioReserva;
                reserva.LineasReserva.Add(new LineaReserva(lineaParaCrear.TiempoReserva, subtotal, impresora));
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Suma total calculada desde los subtotales de las líneas
            reserva.PrecioTotal = reserva.LineasReserva.Sum(l => l.PrecioSubtotal);

            _context.Add(reserva);
            await _context.SaveChangesAsync();

            // Retorno en DTO ReservaImpresoraDetalleDTO
            var reservaCreadaDTO = new ReservaImpresoraDetalleDTO(
                reserva.Id,
                reserva.NombreCliente,
                reserva.ApellidosCliente,
                reserva.DireccionFacturacion,
                reserva.FechaReserva,
                reserva.PrecioTotal,
                reserva.MetodoPago,
                reserva.LineasReserva.Select(l => new LineaReservaImpresoraDTO(
                    l.Impresora.Id,
                    l.Impresora.Nombre,
                    l.Impresora.Modelo,
                    l.Impresora.Tipo,
                    l.Impresora.Descripcion,
                    l.Impresora.PrecioKilovatioHora,
                    l.Impresora.PrecioReserva,
                    l.TiempoReserva,
                    l.PrecioSubtotal)).ToList());

            return CreatedAtAction(nameof(GetReservaImpresora3D), new { id = reserva.Id }, reservaCreadaDTO);
        }

        private static int ObtenerHoras(TiempoReserva tiempo) => tiempo switch
        {
            TiempoReserva.UnaHora => 1,
            TiempoReserva.DosHoras => 2,
            TiempoReserva.TresHoras => 3,
            TiempoReserva.CuatroHoras => 4,
            TiempoReserva.CincoHoras => 5,
            TiempoReserva.UnDia => 24,
            TiempoReserva.DosDias => 48,
            _ => 0
        };
}
}