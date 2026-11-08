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
    }

}