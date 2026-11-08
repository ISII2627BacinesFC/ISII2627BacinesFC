using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.Accesorios3DDTOs;
using AppForSEII.API.Models.ComprarAccesorios;

namespace AppForSEII.UT.Accesorios3DController_test
{
    public class GetAccesorios3DParaComprar_test : AppForSEII4SqliteUT
    {
        public GetAccesorios3DParaComprar_test()
        {
            var accesorios = new List<Accesorio>() {
                new Accesorio("Boquilla de latón 0.4mm", CategoriaAccesorio.Boquillas, "Ender 3, Prusa i3", 50, 5.99m),
                new Accesorio("Base PEI magnética", CategoriaAccesorio.BasesDeImpresion, "Artillery Genius", 15, 25.50m),
                new Accesorio("Alicates de corte", CategoriaAccesorio.Herramientas, "Universal", 20, 8.50m),
                // sin stock: nunca debe aparecer en la lista (FA 2.1)
                new Accesorio("Ventilador de capa 5015", CategoriaAccesorio.Repuestos, "Creality CR-10", 0, 14.99m),
            };

            _context.AddRange(accesorios);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetAccesorios3DParaComprar()
        {
            var boquillaDTO = new Accesorio3DParaComprarDTO(1, "Boquilla de latón 0.4mm", CategoriaAccesorio.Boquillas, "Ender 3, Prusa i3", 50, 5.99m);
            var baseDTO = new Accesorio3DParaComprarDTO(2, "Base PEI magnética", CategoriaAccesorio.BasesDeImpresion, "Artillery Genius", 15, 25.50m);
            var alicatesDTO = new Accesorio3DParaComprarDTO(3, "Alicates de corte", CategoriaAccesorio.Herramientas, "Universal", 20, 8.50m);

            var allTests = new List<object[]>
            {
                // Sin filtros: devuelve los que tienen stock ordenados por nombre
                new object[] { null, null, new List<Accesorio3DParaComprarDTO>() { alicatesDTO, baseDTO, boquillaDTO } },
                // Filtro por nombre
                new object[] { "Boquilla", null, new List<Accesorio3DParaComprarDTO>() { boquillaDTO } },
                // Filtro por categoría
                new object[] { null, CategoriaAccesorio.Herramientas, new List<Accesorio3DParaComprarDTO>() { alicatesDTO } },
                // Filtro por nombre y categoría
                new object[] { "Base", CategoriaAccesorio.BasesDeImpresion, new List<Accesorio3DParaComprarDTO>() { baseDTO } },
                // Accesorio sin stock: no se muestra (FA 2.1)
                new object[] { "Ventilador", null, new List<Accesorio3DParaComprarDTO>() },
                // Ningún accesorio cumple los filtros: lista vacía
                new object[] { "Boquilla", CategoriaAccesorio.Repuestos, new List<Accesorio3DParaComprarDTO>() },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_GetAccesorios3DParaComprar))]
        public async Task GetAccesorios3DParaComprar_OK_test(string? nombre, CategoriaAccesorio? categoria,
            List<Accesorio3DParaComprarDTO> expectedAccesorios)
        {
            // Arrange
            var mock = new Mock<ILogger<Accesorios3DController>>();
            ILogger<Accesorios3DController> logger = mock.Object;
            var controller = new Accesorios3DController(_context, logger);

            // Act
            var result = await controller.GetAccesorios3DParaComprar(nombre, categoria);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var accesoriosDTOActual = Assert.IsType<List<Accesorio3DParaComprarDTO>>(okResult.Value);
            Assert.Equal(expectedAccesorios, accesoriosDTOActual);
        }
    }
}
