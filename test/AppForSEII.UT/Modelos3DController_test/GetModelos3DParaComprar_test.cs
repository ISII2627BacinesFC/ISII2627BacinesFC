using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.Modelo3DDTOs;

namespace AppForSEII.UT.Modelos3DController_test
{
    public class GetModelos3DParaComprar_test : AppForSEII4SqliteUT
    {
        public GetModelos3DParaComprar_test()
        {
            var licenciaPersonal = new LicenciaModelo3D("Uso personal", new DateTime(2027, 12, 31));
            var licenciaComercial = new LicenciaModelo3D("Uso comercial", new DateTime(2028, 6, 30));

            var modelos = new List<Modelo3D>() {
                new Modelo3D("Dragón articulado", "Miniaturas", FormatoModelo3D.STL, 12.99m, licenciaPersonal),
                new Modelo3D("Engranaje de repuesto", "Repuestos", FormatoModelo3D.OBJ, 3.20m, licenciaComercial),
                new Modelo3D("Maceta geométrica", "Decoración", FormatoModelo3D.TresMF, 6.75m, licenciaPersonal),
            };

            _context.AddRange(modelos);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetModelos3DParaComprar()
        {
            var dragonDTO = new Modelo3DParaComprarDTO(1, "Dragón articulado", "Miniaturas", "Uso personal", FormatoModelo3D.STL, 12.99m);
            var engranajeDTO = new Modelo3DParaComprarDTO(2, "Engranaje de repuesto", "Repuestos", "Uso comercial", FormatoModelo3D.OBJ, 3.20m);
            var macetaDTO = new Modelo3DParaComprarDTO(3, "Maceta geométrica", "Decoración", "Uso personal", FormatoModelo3D.TresMF, 6.75m);

            var allTests = new List<object[]>
            {
                // Sin filtros: devuelve todos ordenados por nombre
                new object[] { null, null, new List<Modelo3DParaComprarDTO>() { dragonDTO, engranajeDTO, macetaDTO } },
                // Filtro por nombre
                new object[] { "Dragón", null, new List<Modelo3DParaComprarDTO>() { dragonDTO } },
                // Filtro por categoría
                new object[] { null, "Repuestos", new List<Modelo3DParaComprarDTO>() { engranajeDTO } },
                // Filtro por nombre y categoría
                new object[] { "Maceta", "Decoración", new List<Modelo3DParaComprarDTO>() { macetaDTO } },
                // Ningún modelo cumple los filtros (FA 2.2): lista vacía
                new object[] { "Dragón", "Repuestos", new List<Modelo3DParaComprarDTO>() },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_GetModelos3DParaComprar))]
        public async Task GetModelos3DParaComprar_OK_test(string? nombre, string? categoria,
            List<Modelo3DParaComprarDTO> expectedModelos)
        {
            // Arrange
            var mock = new Mock<ILogger<Modelos3DController>>();
            ILogger<Modelos3DController> logger = mock.Object;
            var controller = new Modelos3DController(_context, logger);

            // Act
            var result = await controller.GetModelos3DParaComprar(nombre, categoria);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var modelosDTOActual = Assert.IsType<List<Modelo3DParaComprarDTO>>(okResult.Value);
            Assert.Equal(expectedModelos, modelosDTOActual);
        }
    }
}