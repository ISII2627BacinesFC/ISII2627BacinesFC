using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CreateEncargoImpresion_test : AppForSEII4SqliteUT
    {
        public CreateEncargoImpresion_test()
        {
            var cliente = new Client("1", "Ricky", "Bobby", "ricky.bobby@alu.uclm.es", "Campus Universitario s/n, Albacete");
            _context.Users.Add(cliente);
            // 1. Sembrado de Materiales
            var pla = new Material("PLA Blanco", 0.05m, 1000m);
            var resina = new Material("Resina Gris", 0.08m, 500m);

            // 2. Sembrado de Pieza compatible solo con PLA
            var pieza = new Pieza3D("Engranaje especial", 50.0m, CategoriaPieza.Repuestos);
            pieza.MaterialesValidos.Add(new PiezaMaterial { Pieza = pieza, Material = pla });

            _context.Materiales.AddRange(pla, resina);
            _context.Piezas3D.Add(pieza);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateEncargoImpresion_Success_test()
        {
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            var controller = new EncargosImpresionController(_context, mock.Object);

            var pieza = _context.Piezas3D.First();
            var material = _context.Materiales.First(m => m.Nombre == "PLA Blanco");

            var dto = new EncargoImpresionCreateDTO(
                "Ricky",
                "Bobby",
                "Campus Universitario s/n, Albacete",
                "600123456",
                "TarjetaCredito",
                "Prueba de encargo exitoso",
                new List<LineaEncargoImpresionCreateDTO>
                {
                    new LineaEncargoImpresionCreateDTO(pieza.Id, material.ID, 2)
                }
            );

            // Fase Act:
            var result = await controller.CreateEncargoImpresion(dto);

            // Fase Assert;
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var actualDTO = Assert.IsType<EncargoImpresionDetalleDTO>(createdResult.Value);

            Assert.Equal("Ricky", actualDTO.NombreCliente);
            Assert.Equal("Bobby", actualDTO.ApellidosCliente);
            Assert.Equal("TarjetaCredito", actualDTO.MetodoPago);
            Assert.Single(actualDTO.Lineas);

            decimal precioEsperado = 5.00m;
            Assert.Equal(precioEsperado, actualDTO.PrecioTotal);
            Assert.Equal(precioEsperado, actualDTO.Lineas[0].Subtotal);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateEncargoImpresion_InvalidPayment_BadRequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            var controller = new EncargosImpresionController(_context, mock.Object);

            var pieza = _context.Piezas3D.First();
            var material = _context.Materiales.First(m => m.Nombre == "PLA Blanco");

            var dto = new EncargoImpresionCreateDTO(
                "Ricky",
                "Bobby",
                "Campus Universitario s/n, Albacete",
                "600123456",
                "MetodoInexistente",
                "Pago inválido",
                new List<LineaEncargoImpresionCreateDTO>
                {
                    new LineaEncargoImpresionCreateDTO(pieza.Id, material.ID, 1)
                }
            );

            // Act
            var result = await controller.CreateEncargoImpresion(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Método de pago no válido: MetodoInexistente", badRequest.Value);
        }
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateEncargoImpresion_PiezaNotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            var controller = new EncargosImpresionController(_context, mock.Object);

            var material = _context.Materiales.First();

            var dto = new EncargoImpresionCreateDTO(
                "Ricky",
                "Bobby",
                "Campus Universitario s/n, Albacete",
                "600123456",
                "TarjetaCredito",
                "Pieza no existe",
                new List<LineaEncargoImpresionCreateDTO>
                {
                    new LineaEncargoImpresionCreateDTO(999, material.ID, 1) // ID 999 no existe
                }
            );

            // Act
            var result = await controller.CreateEncargoImpresion(dto);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Pieza3D con ID 999 no encontrada.", notFound.Value);
        }
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateEncargoImpresion_MaterialNotValid_BadRequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            var controller = new EncargosImpresionController(_context, mock.Object);

            var pieza = _context.Piezas3D.First();
            // Resina Gris existe en BD pero no está en los MaterialesValidos de esta pieza
            var materialIncompatible = _context.Materiales.First(m => m.Nombre == "Resina Gris");

            var dto = new EncargoImpresionCreateDTO(
                "Ricky",
                "Bobby",
                "Campus Universitario s/n, Albacete",
                "600123456",
                "TarjetaCredito",
                "Material incompatible",
                new List<LineaEncargoImpresionCreateDTO>
                {
                    new LineaEncargoImpresionCreateDTO(pieza.Id, materialIncompatible.ID, 1)
                }
            );

            // Act
            var result = await controller.CreateEncargoImpresion(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal($"El material con ID {materialIncompatible.ID} no es válido para la pieza '{pieza.Nombre}'.", badRequest.Value);
        }

        // Prueba unitaria sprint2
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateEncargoImpresion_ErrorLongitudTelefono_test()
        {
            //Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            var controller = new EncargosImpresionController(_context, mock.Object);

            var pieza = _context.Piezas3D.First();
            var material = _context.Materiales.First(m => m.Nombre == "PLA Blanco");

            var dto = new EncargoImpresionCreateDTO(
                "Ricky",
                "Bobby",
                "Campus Universitario s/n, Albacete",
                "600123456357",
                "TarjetaCredito",
                "Prueba de encargo exitoso",
                new List<LineaEncargoImpresionCreateDTO>
                {
                    new LineaEncargoImpresionCreateDTO(pieza.Id, material.ID, 2)
                }
            );

            // Act
            var result = await controller.CreateEncargoImpresion(dto);

            //Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("¡Error! El teléfono debe tener 9 dígitos.", badRequest.Value);

        }
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateEncargoImpresion_LongitudTelefonoCorrecta_test()
        {
            //Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            var controller = new EncargosImpresionController(_context, mock.Object);

            var pieza = _context.Piezas3D.First();
            var material = _context.Materiales.First(m => m.Nombre == "PLA Blanco");

            var dto = new EncargoImpresionCreateDTO(
                "Ricky",
                "Bobby",
                "Campus Universitario s/n, Albacete",
                "600123456",
                "TarjetaCredito",
                "Prueba de encargo exitoso",
                new List<LineaEncargoImpresionCreateDTO>
                {
                    new LineaEncargoImpresionCreateDTO(pieza.Id, material.ID, 2)
                }
            );

            // Act
            var result = await controller.CreateEncargoImpresion(dto);

            //Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var actualDTO = Assert.IsType<EncargoImpresionDetalleDTO>(createdResult.Value);

            Assert.Equal("Ricky", actualDTO.NombreCliente);
            Assert.Equal("Bobby", actualDTO.ApellidosCliente);
            Assert.Equal("TarjetaCredito", actualDTO.MetodoPago);
            Assert.Single(actualDTO.Lineas);
            decimal precioEsperado = 5.00m;
            Assert.Equal(precioEsperado, actualDTO.PrecioTotal);
            Assert.Equal(precioEsperado, actualDTO.Lineas[0].Subtotal);

        }
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateEncargoImpresion_ErrorLongitudMenorTelefono_test()
        {
            //Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            var controller = new EncargosImpresionController(_context, mock.Object);

            var pieza = _context.Piezas3D.First();
            var material = _context.Materiales.First(m => m.Nombre == "PLA Blanco");

            var dto = new EncargoImpresionCreateDTO(
                "Ricky",
                "Bobby",
                "Campus Universitario s/n, Albacete",
                "6001",
                "TarjetaCredito",
                "Prueba de encargo exitoso",
                new List<LineaEncargoImpresionCreateDTO>
                {
                    new LineaEncargoImpresionCreateDTO(pieza.Id, material.ID, 2)
                }
            );

            // Act
            var result = await controller.CreateEncargoImpresion(dto);

            //Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("¡Error! El teléfono debe tener 9 dígitos.", badRequest.Value);

        }
    }
}