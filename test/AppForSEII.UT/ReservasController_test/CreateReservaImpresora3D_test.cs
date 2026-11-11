using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.Impresora3DDTOs;
using AppForSEII.API.Models;
using AppForSEII.API.Models.UC_Reservar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.ReservasController_test
{
    public class CreateReservaImpresora3D_test : AppForSEII4SqliteUT
    {
        public CreateReservaImpresora3D_test()
        {
            var cliente = new Client("1", "Peter", "Jackson", "peter@uclm.es", "Campus Universitario s/n, Albacete");

            var impresoras = new List<Impresora3D>()
            {
                new Impresora3D("Prusa i3 MK3S+", "MK3S+", TipoImpresora.Filamento, "Impresora FDM de alta precisión", 0.25m, 15.00m),
                new Impresora3D("Elegoo Mars 4", "Ultra 9K", TipoImpresora.Resina, "Impresora de resina de alta resolución", 0.30m, 25.50m)
            };

            _context.Add(cliente);
            _context.AddRange(impresoras);
            _context.SaveChanges();
        }

        // Simula la línea que envía el cliente desde la vista: solo son relevantes ImpresoraId y TiempoReserva
        private static LineaReservaImpresoraDTO LineaParaCrear(int impresoraId, TiempoReserva tiempoReserva)
        {
            return new LineaReservaImpresoraDTO(
                impresoraId,
                string.Empty,
                string.Empty,
                TipoImpresora.Filamento,
                null,
                0m,
                0m,
                tiempoReserva,
                0m);
        }

        // Método auxiliar para componer la reserva de prueba con valores por defecto modificables
        private static ReservaImpresoraDetalleDTO ReservaParaCrear(
            string nombre = "Peter",
            string apellidos = "Jackson",
            string direccionFacturacion = "Campus Universitario s/n, Albacete",
            MetodoPago metodoPago = MetodoPago.PayPal,
            IList<LineaReservaImpresoraDTO>? lineas = null)
        {
            return new ReservaImpresoraDetalleDTO(
                0,
                nombre,
                apellidos,
                direccionFacturacion,
                DateTime.Today,
                0m,
                metodoPago,
                lineas ?? new List<LineaReservaImpresoraDTO>() { LineaParaCrear(1, TiempoReserva.UnaHora) });
        }

        public static IEnumerable<object[]> TestCasesFor_CreateReservaImpresora3D_Error()
        {
            var allTests = new List<object[]>
            {
                // Flujo Alternativo 5: Lista de impresoras vacía
                new object[]
                {
                    ReservaParaCrear(lineas: new List<LineaReservaImpresoraDTO>()),
                    "Error! Debes incluir al menos una impresora para reservar"
                },

                // Flujo Alternativo 7.1: Nombre obligatorio
                new object[]
                {
                    ReservaParaCrear(nombre: ""),
                    "Error! El nombre es obligatorio"
                },

                // Flujo Alternativo 7.1: Apellidos obligatorios
                new object[]
                {
                    ReservaParaCrear(apellidos: ""),
                    "Error! Los apellidos son obligatorios"
                },

                // Flujo Alternativo 7.1: Dirección de facturación obligatoria
                new object[]
                {
                    ReservaParaCrear(direccionFacturacion: ""),
                    "Error! La dirección de facturación es obligatoria"
                },

                new object[]
                {
                    ReservaParaCrear(direccionFacturacion: "Calle Mayor 5, Madrid"),
                    "¡Error! Solo se admiten reservas con dirección de facturación en Albacete"
                },

                // Flujo Alternativo 7.1: Método de pago no admitido/inválido
                new object[]
                {
                    ReservaParaCrear(metodoPago: (MetodoPago)99),
                    "Error! El método de pago no es válido"
                },

                // Precondición: Cliente no registrado en el sistema
                new object[]
                {
                    ReservaParaCrear(nombre: "NoExiste", apellidos: "Usuario"),
                    "Error! El cliente 'NoExiste Usuario' no existe en la base de datos."
                },

                // Flujo Alternativo 7.4: Impresora inexistente en catálogo
                new object[]
                {
                    ReservaParaCrear(lineas: new List<LineaReservaImpresoraDTO> { LineaParaCrear(99, TiempoReserva.UnaHora) }),
                    "Error! La impresora con id 99 no existe"
                },

                // Flujo Alternativo 7.3: Tiempo de reserva fuera de catálogo
                new object[]
                {
                    ReservaParaCrear(lineas: new List<LineaReservaImpresoraDTO> { LineaParaCrear(1, (TiempoReserva)99) }),
                    "Error! El tiempo de reserva para la impresora Prusa i3 MK3S+ no es válido"
                }
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_CreateReservaImpresora3D_Error))]
        public async Task CreateReservaImpresora3D_Error_test(ReservaImpresoraDetalleDTO reservaParaCrear, string errorEsperado)
        {
            // Arrange
            var mock = new Mock<ILogger<ReservasImpresoraController>>();
            var controller = new ReservasImpresoraController(_context, mock.Object);

            // Act
            var result = await controller.CreateReservaImpresora3D(reservaParaCrear);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            var errorActual = problemDetails.Errors.First().Value[0];
            Assert.Equal(errorEsperado, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateReservaImpresora3D_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReservasImpresoraController>>();
            var controller = new ReservasImpresoraController(_context, mock.Object);

            // Prusa i3 MK3S+: 15.00€/h x 2 horas (DosHoras) = 30.00€
            // Elegoo Mars 4: 25.50€/h x 1 hora (UnaHora) = 25.50€
            // Precio Total esperado = 30.00 + 25.50 = 55.50€
            var reservaParaCrear = ReservaParaCrear(
                nombre: "Peter",
                apellidos: "Jackson",
                direccionFacturacion: "Campus Universitario s/n, Albacete",
                metodoPago: MetodoPago.TarjetaCredito,
                lineas: new List<LineaReservaImpresoraDTO>()
                {
                    LineaParaCrear(1, TiempoReserva.DosHoras),
                    LineaParaCrear(2, TiempoReserva.UnaHora)
                });

            var expectedReserva = new ReservaImpresoraDetalleDTO(
                1,
                "Peter",
                "Jackson",
                "Campus Universitario s/n, Albacete",
                DateTime.Today,
                55.50m,
                MetodoPago.TarjetaCredito,
                new List<LineaReservaImpresoraDTO>()
                {
                    new LineaReservaImpresoraDTO(1, "Prusa i3 MK3S+", "MK3S+", TipoImpresora.Filamento, "Impresora FDM de alta precisión", 0.25m, 15.00m, TiempoReserva.DosHoras, 30.00m),
                    new LineaReservaImpresoraDTO(2, "Elegoo Mars 4", "Ultra 9K", TipoImpresora.Resina, "Impresora de resina de alta resolución", 0.30m, 25.50m, TiempoReserva.UnaHora, 25.50m)
                });

            // Act
            var result = await controller.CreateReservaImpresora3D(reservaParaCrear);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var reservaDTOActual = Assert.IsType<ReservaImpresoraDetalleDTO>(createdResult.Value);

            // Sincronizar fecha generada en servidor con DateTime.Now antes de validar igualdad de objetos
            expectedReserva.FechaReserva = reservaDTOActual.FechaReserva;
            Assert.Equal(expectedReserva, reservaDTOActual);
        }
    }
}