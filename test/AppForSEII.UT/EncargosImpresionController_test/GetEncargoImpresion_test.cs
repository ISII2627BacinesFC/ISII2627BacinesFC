using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AppForSEII.API.Controllers.EncargarImpresionPiezas3D_Controller;
using AppForSEII.API.DTOs.EncargoImpresionDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.UT.EncargosImpresionController_test
{
    public class GetEncargoImpresion_test : AppForSEII4SqliteUT
    {
        private readonly DateTime _fechaEncargo = new DateTime(2026, 11, 8, 12, 0, 0);

        public GetEncargoImpresion_test()
        {
            // 1. Cliente
            var cliente = new Client("1", "Peter", "Jackson", "peter@uclm.es", "Campus Universitario s/n, Albacete");

            // 2. Material y Pieza
            var pla = new Material("PLA Blanco", 0.05m, 1000m);
            var pieza = new Pieza3D("Engranaje especial", 50.0m, CategoriaPieza.Repuestos);
            
            // Relación con MaterialesValidos
            pieza.MaterialesValidos.Add(new PiezaMaterial { Pieza = pieza, Material = pla });

            // 3. Encargo de impresión
            var encargo = new EncargoImpresion(
                _fechaEncargo,
                "Peter",
                "Jackson",
                "Campus Universitario s/n, Albacete",
                "600123456",
                25.00m,
                MetodoPago.TarjetaCredito,
                cliente,
                "Prueba de encargo"
            );

            // 4. Línea de encargo asociada
            var linea = new LineaEncargo
            {
                Id = 1,
                Cantidad = 2,
                PrecioUnidad = 12.50m,
                Subtotal = 25.00m,
                Pieza = pieza,
                MaterialSeleccionado = pla,
                EncargoImpresion = encargo
            };

            encargo.LineasEncargo.Add(linea);

            _context.Users.Add(cliente);
            _context.Materiales.Add(pla);
            _context.Piezas3D.Add(pieza);
            _context.EncargosImpresion.Add(encargo);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetEncargoImpresion_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            var controller = new EncargosImpresionController(_context, mock.Object);

            // Act
            var result = await controller.GetEncargoImpresion(0);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetEncargoImpresion_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            var controller = new EncargosImpresionController(_context, mock.Object);

            // DTO esperado con el orden exacto de los 10 parámetros
            var expectedDTO = new EncargoImpresionDetalleDTO(
                1,
                "Peter",
                "Jackson",
                "Campus Universitario s/n, Albacete",
                "600123456",
                "Prueba de encargo",
                25.00m,
                _fechaEncargo,
                "TarjetaCredito",
                new List<LineaEncargoImpresionDTO>
                {
                    new LineaEncargoImpresionDTO(
                        1,
                        "Engranaje especial",
                        "PLA Blanco",
                        2,
                        12.50m,
                        25.00m
                    )
                }
            );

            // Act
            var result = await controller.GetEncargoImpresion(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDTO = Assert.IsType<EncargoImpresionDetalleDTO>(okResult.Value);
            Assert.Equal(expectedDTO, actualDTO);
        }
    }
}