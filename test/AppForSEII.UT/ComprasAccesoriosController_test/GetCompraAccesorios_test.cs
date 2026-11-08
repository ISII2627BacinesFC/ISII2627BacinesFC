using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.Accesorios3DDTOs;
using AppForSEII.API.Models.ComprarAccesorios;

namespace AppForSEII.UT.ComprasAccesoriosController_test
{
    public class GetCompraAccesorios_test : AppForSEII4SqliteUT
    {
        private readonly DateTime _fechaCompra = new DateTime(2026, 11, 4, 10, 30, 0);

        public GetCompraAccesorios_test()
        {
            var cliente = new Client("1", "Peter", "Jackson", "peter@uclm.es", "Campus Universitario s/n, Albacete");

            var boquilla = new Accesorio("Boquilla de latón 0.4mm", CategoriaAccesorio.Boquillas, "Ender 3, Prusa i3", 50, 5.99m);
            var alicates = new Accesorio("Alicates de corte", CategoriaAccesorio.Herramientas, "Universal", 20, 8.50m);

            _context.AddRange(boquilla, alicates);
            _context.SaveChanges();

            var compra = new CompraAccesorios(_fechaCompra, "Peter", "Jackson", "Calle Mayor 1, Albacete",
                "600123456", 20.48m, MetodoPago.Bizum, cliente);

            compra.LineasCompraAccesorio.Add(new LineaCompraAccesorio(2, 5.99m, boquilla, compra));
            compra.LineasCompraAccesorio.Add(new LineaCompraAccesorio(1, 8.50m, alicates, compra));

            _context.Add(compra);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompraAccesorios_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasAccesoriosController>>();
            ILogger<ComprasAccesoriosController> logger = mock.Object;
            var controller = new ComprasAccesoriosController(_context, logger);

            // Act
            var result = await controller.GetCompraAccesorios(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompraAccesorios_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasAccesoriosController>>();
            ILogger<ComprasAccesoriosController> logger = mock.Object;
            var controller = new ComprasAccesoriosController(_context, logger);

            var expectedCompra = new CompraAccesoriosDetalleDTO(1, _fechaCompra, "peter@uclm.es", "Peter", "Jackson",
                "Calle Mayor 1, Albacete", "600123456", MetodoPago.Bizum,
                new List<LineaCompraAccesorioDTO>() {
                    new LineaCompraAccesorioDTO(1, "Boquilla de latón 0.4mm", CategoriaAccesorio.Boquillas, "Ender 3, Prusa i3", 2, 5.99m),
                    new LineaCompraAccesorioDTO(2, "Alicates de corte", CategoriaAccesorio.Herramientas, "Universal", 1, 8.50m)
                });

            // Act
            var result = await controller.GetCompraAccesorios(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var compraDTOActual = Assert.IsType<CompraAccesoriosDetalleDTO>(okResult.Value);
            Assert.Equal(expectedCompra, compraDTOActual);
        }
    }
}
