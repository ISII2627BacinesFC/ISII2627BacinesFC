using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.Modelo3DDTOs;

namespace AppForSEII.UT.Modelos3DController_test
{
    public class GetCompraModelo3D_test : AppForSEII4SqliteUT
    {
        private readonly DateTime _fechaCompra = new DateTime(2026, 11, 4, 10, 30, 0);
        private readonly DateTime _fechaExpiracionPersonal = new DateTime(2027, 12, 31);
        private readonly DateTime _fechaExpiracionComercial = new DateTime(2028, 6, 30);

        public GetCompraModelo3D_test()
        {
            var cliente = new Client("1", "Peter", "Jackson", "peter@uclm.es", "Campus Universitario s/n, Albacete");

            var licenciaPersonal = new LicenciaModelo3D("Uso personal", _fechaExpiracionPersonal);
            var licenciaComercial = new LicenciaModelo3D("Uso comercial", _fechaExpiracionComercial);

            var dragon = new Modelo3D("Dragón articulado", "Miniaturas", FormatoModelo3D.STL, 12.99m, licenciaPersonal);
            var engranaje = new Modelo3D("Engranaje de repuesto", "Repuestos", FormatoModelo3D.OBJ, 3.20m, licenciaComercial);

            var compra = new CompraModelo3D(_fechaCompra, "Peter", "Jackson", "peter@uclm.es",
                "Campus Universitario s/n, Albacete", "Para imprimir en clase", 32.38m, MetodoPago.PayPal, cliente);

            compra.Lineas.Add(new LineaCompraModelo(2, 12.99m, dragon, compra));
            compra.Lineas.Add(new LineaCompraModelo(2, 3.20m, engranaje, compra));

            _context.Add(compra);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompraModelo3D_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasModelo3DController>>();
            ILogger<ComprasModelo3DController> logger = mock.Object;
            var controller = new ComprasModelo3DController(_context, logger);

            // Act
            var result = await controller.GetCompraModelo3D(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompraModelo3D_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasModelo3DController>>();
            ILogger<ComprasModelo3DController> logger = mock.Object;
            var controller = new ComprasModelo3DController(_context, logger);

            var expectedCompra = new CompraModelo3DDetalleDTO(1, "Peter", "Jackson", "peter@uclm.es",
                "Campus Universitario s/n, Albacete", _fechaCompra, 32.38m, MetodoPago.PayPal, "Para imprimir en clase",
                new List<LineaCompraModeloDTO>() {
                    new LineaCompraModeloDTO(1, "Dragón articulado", "Miniaturas", FormatoModelo3D.STL,
                        "Uso personal", _fechaExpiracionPersonal, 2, 12.99m, 25.98m),
                    new LineaCompraModeloDTO(2, "Engranaje de repuesto", "Repuestos", FormatoModelo3D.OBJ,
                        "Uso comercial", _fechaExpiracionComercial, 2, 3.20m, 6.40m)
                });

            // Act
            var result = await controller.GetCompraModelo3D(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var compraDTOActual = Assert.IsType<CompraModelo3DDetalleDTO>(okResult.Value);
            Assert.Equal(expectedCompra, compraDTOActual);
        }
    }
}