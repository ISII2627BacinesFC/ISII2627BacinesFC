using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using AppForSEII.API.Controllers;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.Pieza3DDTOs;


namespace AppForSEII.UT.Piezas3DController_test
{
    public class GetPiezas3DForEncargo_test : AppForSEII4SqliteUT
    {
        Mock<ILogger<Piezas3DController>> _mockLogger;
        public Piezas3DController controller;

        public GetPiezas3DForEncargo_test()
        {
            _mockLogger = new Mock<ILogger<Piezas3DController>>();
            controller = new Piezas3DController(_context, _mockLogger.Object);
        }

        private async Task MetodoSeed(){

            // Creo materiales:
            var material1 = new Material{ ID = 1, Nombre = "PLA Blanco", PrecioPorGramo = 0.05m};
            var material2 = new Material{ ID = 2, Nombre = "Resina Premium", PrecioPorGramo = 0.12m};
            await _context.Materiales.AddRangeAsync(material1, material2);

            // Creo piezas3D:
            var pieza1 = new Pieza3D{ Id = 1, Nombre = "Engranaje especial", Categoria = CategoriaPieza.Repuestos, Peso = 50m};
            var pieza2 = new Pieza3D{ Id = 2, Nombre = "Jarron Pequeño", Categoria = CategoriaPieza.Decoracion, Peso = 100m};
            await _context.Piezas3D.AddRangeAsync(pieza1, pieza2);

            // Creo la relacion entre piezas3D y materiales:
            var pm1 = new PiezaMaterial{ PiezaId = 1, MaterialId = 1, };
            var pm2 = new PiezaMaterial{ PiezaId = 2, MaterialId = 2, };
            await _context.Set<PiezaMaterial>().AddRangeAsync(pm1, pm2);

            //Guardo cambios
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetPiezas3DForEncargo_SinFiltros_ReturnsOk()
        {
            await MetodoSeed();
            var expectedDTOs = new List<Pieza3DForEncargoDTO>
            {
                new Pieza3DForEncargoDTO(1, "Engranaje especial", CategoriaPieza.Repuestos, 50m,"PLA Blanco", 2.5m),
                new Pieza3DForEncargoDTO(2, "Jarron Pequeño", CategoriaPieza.Decoracion, 100m,"Resina Premium", 12m)
            };

            // Invocar al endpoint sin filtros
            var actionResult = await controller.GetPiezas3DForEncargo(null,null);

            // Verificar que el resultado sea OK
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var actualDTOs = Assert.IsType<List<Pieza3DForEncargoDTO>>(okResult.Value);
            Assert.Equal(expectedDTOs.Count, actualDTOs.Count);
            Assert.Equal(expectedDTOs, actualDTOs);
        }
        
    }
}