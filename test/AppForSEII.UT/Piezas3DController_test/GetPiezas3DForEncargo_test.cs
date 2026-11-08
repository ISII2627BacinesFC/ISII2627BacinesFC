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
        public GetPiezas3DForEncargo_test()
        {
            var material1 = new Material{ ID = 1, Nombre = "PLA Blanco", PrecioPorGramo = 0.05m};
            var material2 = new Material{ ID = 2, Nombre = "Resina Premium", PrecioPorGramo = 0.12m};
            _context.Materiales.AddRangeAsync(material1, material2);

            // Creo piezas3D:
            var pieza1 = new Pieza3D{ Id = 1, Nombre = "Engranaje especial", Categoria = CategoriaPieza.Repuestos, Peso = 50m};
            var pieza2 = new Pieza3D{ Id = 2, Nombre = "Jarron Pequeño", Categoria = CategoriaPieza.Decoracion, Peso = 100m};
            _context.Piezas3D.AddRangeAsync(pieza1, pieza2);

            // Creo la relacion entre piezas3D y materiales:
            var pm1 = new PiezaMaterial{ PiezaId = 1, MaterialId = 1, };
            var pm2 = new PiezaMaterial{ PiezaId = 2, MaterialId = 2, };
            _context.Set<PiezaMaterial>().AddRangeAsync(pm1, pm2);

            //Guardo cambios
            _context.SaveChangesAsync();
        }

        public static IEnumerable<object[]> TestCasesFor_GetPiezas3DForEncargo()
        {
            var engranajeDTO = new Pieza3DForEncargoDTO(1, "Engranaje especial", CategoriaPieza.Repuestos, 50m, "PLA Blanco", 2.5m);
            var jarronDTO = new Pieza3DForEncargoDTO(2, "Jarron Pequeño", CategoriaPieza.Decoracion, 100m, "Resina Premium", 12.0m);

            var allTests = new List<object[]>
            {
                // 1. Sin filtros: devuelve todas ordenadas por nombre
                new object[] { null, null, new List<Pieza3DForEncargoDTO> { engranajeDTO, jarronDTO } },

                // 2. Filtro por nombre
                new object[] { "Engranaje", null, new List<Pieza3DForEncargoDTO> { engranajeDTO } },

                // 3. Filtro por categoría
                new object[] { null, CategoriaPieza.Decoracion, new List<Pieza3DForEncargoDTO> { jarronDTO } },

                // 4. Filtro por nombre y categoría
                new object[] { "Engranaje", CategoriaPieza.Repuestos, new List<Pieza3DForEncargoDTO> { engranajeDTO } },

                // 5. Sin coincidencias: lista vacía
                new object[] { "Inexistente", null, new List<Pieza3DForEncargoDTO>() }
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_GetPiezas3DForEncargo))]
        public async Task GetPiezas3DForEncargo_OK_test(string? nombre, CategoriaPieza? categoria, List<Pieza3DForEncargoDTO> expectedPiezas)
        {
            // Arrange
            var mock = new Mock<ILogger<Piezas3DController>>();
            ILogger<Piezas3DController> logger = mock.Object;
            var controller = new Piezas3DController(_context, logger);

            // Act
            var result = await controller.GetPiezas3DForEncargo(nombre, categoria);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var piezasactualDTOs = Assert.IsType<List<Pieza3DForEncargoDTO>>(okResult.Value);
            Assert.Equal(expectedPiezas, piezasactualDTOs);
        }
    }
}