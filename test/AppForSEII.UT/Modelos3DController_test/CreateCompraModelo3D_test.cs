using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.Modelo3DDTOs;

namespace AppForSEII.UT.Modelos3DController_test
{
    public class CreateCompraModelo3D_test : AppForSEII4SqliteUT
    {
        private static readonly DateTime _fechaExpiracionPersonal = new DateTime(2027, 12, 31);
        private static readonly DateTime _fechaExpiracionComercial = new DateTime(2028, 6, 30);

        public CreateCompraModelo3D_test()
        {
            var cliente = new Client("1", "Peter", "Jackson", "peter@uclm.es", "Campus Universitario s/n, Albacete");

            var licenciaPersonal = new LicenciaModelo3D("Uso personal", _fechaExpiracionPersonal);
            var licenciaComercial = new LicenciaModelo3D("Uso comercial", _fechaExpiracionComercial);

            var modelos = new List<Modelo3D>() {
                new Modelo3D("Dragón articulado", "Miniaturas", FormatoModelo3D.STL, 12.99m, licenciaPersonal),
                new Modelo3D("Engranaje de repuesto", "Repuestos", FormatoModelo3D.OBJ, 3.20m, licenciaComercial),
            };

            _context.Add(cliente);
            _context.AddRange(modelos);
            _context.SaveChanges();
        }

        // Crea la línea tal y como la envía el cliente: solo importan el id del modelo y la cantidad
        private static LineaCompraModeloDTO LineaParaCrear(int modeloId, int cantidadLicencias)
        {
            return new LineaCompraModeloDTO(modeloId, "", "", FormatoModelo3D.STL, "", DateTime.Today,
                cantidadLicencias, 0, 0);
        }

        private static CompraModelo3DDetalleDTO CompraParaCrear(string correo, MetodoPago metodoPago,
            IList<LineaCompraModeloDTO> lineas)
        {
            return new CompraModelo3DDetalleDTO(0, "Peter", "Jackson", correo, "Campus Universitario s/n, Albacete",
                DateTime.Today, 0, metodoPago, "Para imprimir en clase", lineas);
        }

        private static CompraModelo3DDetalleDTO CompraParaCrearPrueba(string correo, MetodoPago metodoPago,
            IList<LineaCompraModeloDTO> lineas)
        {
            return new CompraModelo3DDetalleDTO(0, "Peter", "Jackson", correo, "Campus Universitario s/n, Albacete",
                DateTime.Today, 0, metodoPago, "No se que escribir como descripción, espero q pase los 50 caracteres todo esto que estoy poniendo, si no igual lo puedo alargar un poco más, espero sacar un 10 y si no pues nada", lineas);
        }

        public static IEnumerable<object[]> TestCasesFor_CreateCompraModelo3D_Error()
        {
            var allTests = new List<object[]>
            {
                // Sin modelos 3D (FA 5 + Prueba)
                new object[] { CompraParaCrear("peter@uclm.es", MetodoPago.PayPal,
                    new List<LineaCompraModeloDTO>()),
                    "Error! Debes incluir al menos un modelo 3D para comprar" },
                // Método de pago no permitido
                new object[] { CompraParaCrear("peter@uclm.es", MetodoPago.Bizum,
                    new List<LineaCompraModeloDTO>() { LineaParaCrear(1, 1) }),
                    "Error! El método de pago debe ser tarjeta de crédito o PayPal" },
                // Cliente inexistente
                new object[] { CompraParaCrear("noexiste@uclm.es", MetodoPago.PayPal,
                    new List<LineaCompraModeloDTO>() { LineaParaCrear(1, 1) }),
                    "Error! No existe ningún cliente con ese correo electrónico" },
                // Modelo 3D inexistente
                new object[] { CompraParaCrear("peter@uclm.es", MetodoPago.PayPal,
                    new List<LineaCompraModeloDTO>() { LineaParaCrear(99, 1) }),
                    "Error! El modelo 3D con id 99 no existe" },
                // Cantidad de licencias inválida (FA 7.3)
                new object[] { CompraParaCrear("peter@uclm.es", MetodoPago.PayPal,
                    new List<LineaCompraModeloDTO>() { LineaParaCrear(1, 0) }),
                    "Error! La cantidad de licencias del modelo Dragón articulado debe ser al menos 1" },
                // Método de pago no permitido
                new object[] { CompraParaCrearPrueba("peter@uclm.es", MetodoPago.PayPal,
                    new List<LineaCompraModeloDTO>() { LineaParaCrear(1, 1) }),
                    "Error! La descripción no puede superar los 50 caracteres" },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_CreateCompraModelo3D_Error))]
        public async Task CreateCompraModelo3D_Error_test(CompraModelo3DDetalleDTO compraParaCrear, string errorEsperado)
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasModelo3DController>>();
            ILogger<ComprasModelo3DController> logger = mock.Object;
            var controller = new ComprasModelo3DController(_context, logger);

            // Act
            var result = await controller.CreateCompraModelo3D(compraParaCrear);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            var errorActual = problemDetails.Errors.First().Value[0];
            Assert.Equal(errorEsperado, errorActual);
        }


        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateCompraModelo3D_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasModelo3DController>>();
            ILogger<ComprasModelo3DController> logger = mock.Object;
            var controller = new ComprasModelo3DController(_context, logger);

            var compraParaCrear = CompraParaCrear("peter@uclm.es", MetodoPago.PayPal,
                new List<LineaCompraModeloDTO>() { LineaParaCrear(1, 2), LineaParaCrear(2, 1) });

            var expectedCompra = new CompraModelo3DDetalleDTO(1, "Peter", "Jackson", "peter@uclm.es",
                "Campus Universitario s/n, Albacete", DateTime.Today, 29.18m, MetodoPago.PayPal, "Para imprimir en clase",
                new List<LineaCompraModeloDTO>() {
                    new LineaCompraModeloDTO(1, "Dragón articulado", "Miniaturas", FormatoModelo3D.STL,
                        "Uso personal", _fechaExpiracionPersonal, 2, 12.99m, 25.98m),
                    new LineaCompraModeloDTO(2, "Engranaje de repuesto", "Repuestos", FormatoModelo3D.OBJ,
                        "Uso comercial", _fechaExpiracionComercial, 1, 3.20m, 3.20m)
                });

            // Act
            var result = await controller.CreateCompraModelo3D(compraParaCrear);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var compraDTOActual = Assert.IsType<CompraModelo3DDetalleDTO>(createdResult.Value);
            // La fecha de compra la pone el servidor con DateTime.Now, así que se copia antes de comparar
            expectedCompra.FechaCompra = compraDTOActual.FechaCompra;
            Assert.Equal(expectedCompra, compraDTOActual);
        }
    }
}