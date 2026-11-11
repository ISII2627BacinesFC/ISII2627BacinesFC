using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.Accesorios3DDTOs;
using AppForSEII.API.Models.ComprarAccesorios;

namespace AppForSEII.UT.ComprasAccesoriosController_test
{
    public class CreateCompraAccesorios_test : AppForSEII4SqliteUT
    {
        private const string _userName = "peter@uclm.es";
        private const string _nombre = "Peter";
        private const string _apellidos = "Jackson";
        private const string _direccionEnvio = "Calle Mayor 1, Albacete, Castilla-La Mancha";
        private const string _telefono = "600123456";

        private const string _boquillaNombre = "Boquilla de latón 0.4mm";
        private const string _boquillaCompatibilidad = "Ender 3, Prusa i3";
        private const string _baseNombre = "Base PEI magnética";
        private const string _baseCompatibilidad = "Artillery Genius";

        public CreateCompraAccesorios_test()
        {
            var cliente = new Client("1", _nombre, _apellidos, _userName, "Campus Universitario s/n, Albacete");

            var accesorios = new List<Accesorio>() {
                new Accesorio(_boquillaNombre, CategoriaAccesorio.Boquillas, _boquillaCompatibilidad, 50, 5.99m),
                // solo quedan 2 unidades
                new Accesorio(_baseNombre, CategoriaAccesorio.BasesDeImpresion, _baseCompatibilidad, 102, 25.50m),
            };

            _context.Add(cliente);
            _context.AddRange(accesorios);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateCompraAccesorios_Error()
        {
            var lineaValida = new List<LineaCompraAccesorioDTO>() {
                new LineaCompraAccesorioDTO(1, _boquillaNombre, CategoriaAccesorio.Boquillas, _boquillaCompatibilidad, 1, 5.99m)
            };



            // FA 4: sin accesorios
            var compraSinLineas = new CompraAccesoriosParaCrearDTO(_userName, _nombre, _apellidos,
                _direccionEnvio, _telefono, MetodoPago.TarjetaCredito, new List<LineaCompraAccesorioDTO>());

            // Usuario que no está registrado
            var compraUsuarioNoRegistrado = new CompraAccesoriosParaCrearDTO("nadie@uclm.es", _nombre, _apellidos,
                _direccionEnvio, _telefono, MetodoPago.TarjetaCredito, lineaValida);


                    

            // Accesorio que no existe en la base de datos
            //var compraAccesorioNoExiste = new CompraAccesoriosParaCrearDTO(_userName, _nombre, _apellidos,
            //    _direccionEnvio, _telefono, MetodoPago.TarjetaCredito,
            //    new List<LineaCompraAccesorioDTO>() {
            //        new LineaCompraAccesorioDTO(99, "Accesorio inventado", CategoriaAccesorio.Repuestos, "Ninguna", 1, 1.00m)
            //    });

            // FA 7.3: no hay stock suficiente (se piden 5 y quedan 2)
            //var compraSinStock = new CompraAccesoriosParaCrearDTO(_userName, _nombre, _apellidos,
            //    _direccionEnvio, _telefono, MetodoPago.TarjetaCredito,
            //    new List<LineaCompraAccesorioDTO>() {
            //        new LineaCompraAccesorioDTO(2, _baseNombre, CategoriaAccesorio.BasesDeImpresion, _baseCompatibilidad, 5, 25.50m)
            //    });

            //si la dirección no termina por Castilla-La Mancha
            var compraDireccionNoValida = new CompraAccesoriosParaCrearDTO(_userName, _nombre, _apellidos,
                  "Calle Mayor 1, Albacete", _telefono, MetodoPago.TarjetaCredito, lineaValida); 

            var allTests = new List<object[]>
            {
                new object[] { compraSinLineas, "¡Error! Debes incluir al menos un accesorio en la compra" },
                new object[] { compraUsuarioNoRegistrado, "¡Error! El usuario no está registrado" },
                //new object[] { compraAccesorioNoExiste, "¡Error! El accesorio 'Accesorio inventado' no existe" },               
                //new object[] { compraSinStock, "¡Error! No hay stock suficiente del accesorio 'Base PEI magnética'" },
                new object[] { compraDireccionNoValida, "¡Error! Solo se realizan envios dentro de Castilla-La Mancha"},
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_CreateCompraAccesorios_Error))]
        public async Task CreateCompraAccesorios_Error_test(CompraAccesoriosParaCrearDTO compraDTO, string errorEsperado)
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasAccesoriosController>>();
            ILogger<ComprasAccesoriosController> logger = mock.Object;
            var controller = new ComprasAccesoriosController(_context, logger);

            // Act
            var result = await controller.CreateCompraAccesorios(compraDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            var errorActual = problemDetails.Errors.First().Value[0];
            Assert.StartsWith(errorEsperado, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateCompraAccesorios_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasAccesoriosController>>();
            ILogger<ComprasAccesoriosController> logger = mock.Object;
            var controller = new ComprasAccesoriosController(_context, logger);

            var lineas = new List<LineaCompraAccesorioDTO>() {
                new LineaCompraAccesorioDTO(1, _boquillaNombre, CategoriaAccesorio.Boquillas, _boquillaCompatibilidad, 2, 5.99m),
                new LineaCompraAccesorioDTO(2, _baseNombre, CategoriaAccesorio.BasesDeImpresion, _baseCompatibilidad, 1, 25.50m)
            };

            var compraDTO = new CompraAccesoriosParaCrearDTO(_userName, _nombre, _apellidos,
                _direccionEnvio, _telefono, MetodoPago.PayPal, lineas);

            var expectedCompraDetalle = new CompraAccesoriosDetalleDTO(1, DateTime.Now, _userName, _nombre, _apellidos,
                _direccionEnvio, _telefono, MetodoPago.PayPal,
                new List<LineaCompraAccesorioDTO>() {
                    new LineaCompraAccesorioDTO(1, _boquillaNombre, CategoriaAccesorio.Boquillas, _boquillaCompatibilidad, 2, 5.99m),
                    new LineaCompraAccesorioDTO(2, _baseNombre, CategoriaAccesorio.BasesDeImpresion, _baseCompatibilidad, 1, 25.50m)
                });

            // Act
            var result = await controller.CreateCompraAccesorios(compraDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var compraDetalleActual = Assert.IsType<CompraAccesoriosDetalleDTO>(createdResult.Value);
            Assert.Equal(expectedCompraDetalle, compraDetalleActual);
        }
    }
}
