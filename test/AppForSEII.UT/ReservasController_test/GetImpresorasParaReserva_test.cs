using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AppForSEII.API.Controllers;
using AppForSEII.API.Models.UC_Reservar;
using AppForSEII.API.DTOs.Impresora3DDTOs;

namespace AppForSEII.UT.ReservasController_test
{
    public class GetImpresorasParaReserva_test : AppForSEII4SqliteUT
    {
        public GetImpresorasParaReserva_test()
        {
            var impresoras = new List<Impresora3D>()
            {
                new Impresora3D
                {
                    Id = 1,
                    Nombre = "Prusa i3 MK3S+",
                    Modelo = "MK3S+",
                    Tipo = TipoImpresora.Filamento,
                    Descripcion = "Impresora FDM de alta precisión para prototipado rápido.",
                    PrecioKilovatioHora = 0.25m,
                    PrecioReserva = 15.00m
                },
                new Impresora3D
                {
                    Id = 2,
                    Nombre = "Elegoo Mars 4",
                    Modelo = "Ultra 9K",
                    Tipo = TipoImpresora.Resina,
                    Descripcion = "Impresora de resina MSLA con resolución 9K para miniaturas detalladas.",
                    PrecioKilovatioHora = 0.30m,
                    PrecioReserva = 25.50m
                },
                new Impresora3D
                {
                    Id = 3,
                    Nombre = "Anycubic Kobra 2",
                    Modelo = "Kobra 2 Neo",
                    Tipo = TipoImpresora.Filamento,
                    Descripcion = "Impresora FDM rápida para principiantes.",
                    PrecioKilovatioHora = 0.20m,
                    PrecioReserva = 12.00m
                }
            };

            _context.AddRange(impresoras);
            _context.SaveChanges();
        }

        public static IEnumerable<object?[]> TestCasesFor_GetImpresorasParaReserva()
        {
           
            var anycubicDTO = new ImpresoraParaReservaDTO(
                3,
                "Anycubic Kobra 2",
                "Kobra 2 Neo",
                TipoImpresora.Filamento,
                "Impresora FDM rápida para principiantes.",
                0.20m,
                12.00m
            );

            var elegooDTO = new ImpresoraParaReservaDTO(
                2,
                "Elegoo Mars 4",
                "Ultra 9K",
                TipoImpresora.Resina,
                "Impresora de resina MSLA con resolución 9K para miniaturas detalladas.",
                0.30m,
                25.50m
            );

            var prusaDTO = new ImpresoraParaReservaDTO(
                1,
                "Prusa i3 MK3S+",
                "MK3S+",
                TipoImpresora.Filamento,
                "Impresora FDM de alta precisión para prototipado rápido.",
                0.25m,
                15.00m
            );

            var allTests = new List<object?[]>
            {
                // 1. Sin filtros: devuelve todas ordenadas por nombre (Anycubic, Elegoo, Prusa)
                new object?[] { null, null, new List<ImpresoraParaReservaDTO> { anycubicDTO, elegooDTO, prusaDTO } },

                // 2. Filtro por nombre (búsqueda parcial)
                new object?[] { "Mars", null, new List<ImpresoraParaReservaDTO> { elegooDTO } },

                // 3. Filtro por tipo de impresora (Filamento)
                new object?[] { null, TipoImpresora.Filamento, new List<ImpresoraParaReservaDTO> { anycubicDTO, prusaDTO } },

                // 4. Filtro por nombre y tipo combinados
                new object?[] { "Prusa", TipoImpresora.Filamento, new List<ImpresoraParaReservaDTO> { prusaDTO } },

                // 5. Filtro sin coincidencias: lista vacía
                new object?[] { "Elegoo", TipoImpresora.Filamento, new List<ImpresoraParaReservaDTO>() }
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_GetImpresorasParaReserva))]
        public async Task GetImpresorasParaReserva_OK_test(string? nombre, TipoImpresora? tipo,
            List<ImpresoraParaReservaDTO> expectedImpresoras)
        {
        
            var mock = new Mock<ILogger<ReservasController>>();
            ILogger<ReservasController> logger = mock.Object;
            var controller = new ReservasController(_context, logger);

    
            var result = await controller.GetImpresorasParaReserva(nombre, tipo);


            var okResult = Assert.IsType<OkObjectResult>(result);
            var impresorasDTOActual = Assert.IsType<List<ImpresoraParaReservaDTO>>(okResult.Value);
            Assert.Equal(expectedImpresoras, impresorasDTOActual);
        }
    }
}