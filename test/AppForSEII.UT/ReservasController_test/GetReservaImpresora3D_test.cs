using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.Impresora3DDTOs;
using AppForSEII.API.Models;
using AppForSEII.API.Models.UC_Reservar;

namespace AppForSEII.UT.ReservasController_test
{
    public class GetReservaImpresora3D_test : AppForSEII4SqliteUT
    {
        private readonly DateTime _fechaReserva = new DateTime(2026, 11, 4, 10, 30, 0);

        public GetReservaImpresora3D_test()
        {
            var cliente = new Client("1", "Peter", "Jackson", "peter@uclm.es", "Campus Universitario s/n, Albacete");

            // Impresora 1: Prusa i3 MK3S+ (Tipo 0 = Filamento)
            var impresora1 = new Impresora3D(
                "Prusa i3 MK3S+",
                "MK3S+",
                TipoImpresora.Filamento,
                "Impresora FDM de alta precisión",
                0.25m,
                15.00m);

            // Impresora 2: Elegoo Mars 4 (Tipo 1 = Resina)
            var impresora2 = new Impresora3D(
                "Elegoo Mars 4",
                "Ultra 9K",
                TipoImpresora.Resina,
                "Impresora de resina de alta resolución",
                0.30m,
                25.50m);

            // Reserva con 2 líneas:
            var reserva = new ReservaImpresora(
                _fechaReserva,
                "Peter",
                "Jackson",
                "Campus Universitario s/n, Albacete",
                81.00m,
                MetodoPago.PayPal,
                cliente);

            reserva.LineasReserva.Add(new LineaReserva(TiempoReserva.DosHoras, 30.00m, impresora1));
            reserva.LineasReserva.Add(new LineaReserva(TiempoReserva.DosHoras, 51.00m, impresora2));

            _context.Add(reserva);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReservaImpresora3D_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReservasImpresoraController>>();
            ILogger<ReservasImpresoraController> logger = mock.Object;
            var controller = new ReservasImpresoraController(_context, logger);

            // Act
            var result = await controller.GetReservaImpresora3D(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReservaImpresora3D_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReservasImpresoraController>>();
            ILogger<ReservasImpresoraController> logger = mock.Object;
            var controller = new ReservasImpresoraController(_context, logger);

            var expectedReserva = new ReservaImpresoraDetalleDTO(
                1,
                "Peter",
                "Jackson",
                "Campus Universitario s/n, Albacete",
                _fechaReserva,
                81.00m,
                MetodoPago.PayPal,
                new List<LineaReservaImpresoraDTO>()
                {
                    new LineaReservaImpresoraDTO(
                        1,
                        "Prusa i3 MK3S+",
                        "MK3S+",
                        TipoImpresora.Filamento,
                        "Impresora FDM de alta precisión",
                        0.25m,
                        15.00m,
                        TiempoReserva.DosHoras,
                        30.00m),
                    new LineaReservaImpresoraDTO(
                        2,
                        "Elegoo Mars 4",
                        "Ultra 9K",
                        TipoImpresora.Resina,
                        "Impresora de resina de alta resolución",
                        0.30m,
                        25.50m,
                        TiempoReserva.DosHoras,
                        51.00m)
                });

            // Act
            var result = await controller.GetReservaImpresora3D(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var reservaDTOActual = Assert.IsType<ReservaImpresoraDetalleDTO>(okResult.Value);
            Assert.Equal(expectedReserva, reservaDTOActual);
        }
    }
}