
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AppForSEII.API.Models; 
using AppForSEII.API.Models.ComprarAccesorios; 
using AppForSEII.API.Models.UC_Reservar;

namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }

            try {
                SeedPiezasYMateriales(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Piezas3D and Materiales in the Database.");
            }

            try {
                SeedLicenciasYModelos3D(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Licencias and Modelos3D in the Database.");
            }

            try {
                SeedAccesorios(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Accesorios in the Database.");
            }

            try {
                SeedImpresoras3D(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the 3D Printers in the Database.");
            }

            try {
                var client = dbContext.Users.OfType<Client>().FirstOrDefault(u => u.UserName == "peter@uclm.es");
                if (client != null) {
                    SeedReservaImpresora(dbContext, client);
                }
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding a Reservation in the Database.");
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {
            foreach (string roleName in roles) {
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }

            if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
                Client user = new Client("3", "Peter", "Jackson", "peter@uclm.es", "Campus Universitario s/n, Albacete 02071");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager.AddToRoleAsync(user, roles[2]).Wait();
                }
            }
        }

        public static void SeedPiezasYMateriales(ApplicationDbContext dbContext) {
            // Aqui se pueblan materiales si no existen.
            if (!dbContext.Materiales.Any())
            {
                var materiales = new List<Material>
                {
                    new Material("PLA", 0.05m, 5000m),
                    new Material("Resina Blanca", 0.08m, 1200m),
                    new Material("PETG", 0.06m, 3000m),
                };

                dbContext.Materiales.AddRange(materiales);
                dbContext.SaveChanges();
            }

            // Poblar piezas3D asociadas a materiales validos
            if (!dbContext.Piezas3D.Any())
            {
                var pla = dbContext.Materiales.FirstOrDefault(m => m.Nombre == "PLA")!;
                var petg = dbContext.Materiales.FirstOrDefault(m => m.Nombre == "PETG")!;
                var resinaBlanca = dbContext.Materiales.FirstOrDefault(m => m.Nombre == "Resina Blanca")!;

                var pieza1 = new Pieza3D("Engranaje pequeño", 45.5m, CategoriaPieza.Repuestos);
                pieza1.MaterialesValidos.Add(petg);

                var pieza2 = new Pieza3D("Soporte de camara", 120.0m, CategoriaPieza.HerramientasYAccesorios);
                pieza2.MaterialesValidos.Add(pla);
                pieza2.MaterialesValidos.Add(resinaBlanca);

                var pieza3 = new Pieza3D("Miniatura de Pegaso", 85.0m, CategoriaPieza.MiniaturasYMaquetas);
                pieza3.MaterialesValidos.Add(pla);
                pieza3.MaterialesValidos.Add(petg);
                pieza3.MaterialesValidos.Add(resinaBlanca);

                dbContext.Piezas3D.AddRange(pieza1, pieza2, pieza3);
                dbContext.SaveChanges();
            }
        }

        public static void SeedLicenciasYModelos3D(ApplicationDbContext dbContext) {
            // Se pueblan las licencias si no existen
            if (!dbContext.LicenciasModelo3D.Any()) {
                var licencias = new List<LicenciaModelo3D> {
                    new LicenciaModelo3D("Uso personal", DateTime.Today.AddYears(1)),
                    new LicenciaModelo3D("Uso comercial", DateTime.Today.AddYears(2)),
                    // Caduca el próximo mes, para probar el flujo alternativo 7.4
                    new LicenciaModelo3D("Creative Commons", DateTime.Today.AddDays(20)),
                };

                dbContext.LicenciasModelo3D.AddRange(licencias);
                dbContext.SaveChanges();
            }

            // Se pueblan los modelos 3D asociados a una licencia
            if (!dbContext.Modelos3D.Any()) {
                var personal = dbContext.LicenciasModelo3D.First(l => l.Nombre == "Uso personal");
                var comercial = dbContext.LicenciasModelo3D.First(l => l.Nombre == "Uso comercial");
                var creativeCommons = dbContext.LicenciasModelo3D.First(l => l.Nombre == "Creative Commons");

                var modelos = new List<Modelo3D> {
                    new Modelo3D("Dragón articulado", "Miniaturas", FormatoModelo3D.STL, 12.99m, personal),
                    new Modelo3D("Soporte para móvil", "Accesorios", FormatoModelo3D.OBJ, 4.50m, comercial),
                    new Modelo3D("Maceta geométrica", "Decoración", FormatoModelo3D.TresMF, 6.75m, creativeCommons),
                    new Modelo3D("Engranaje de repuesto", "Repuestos", FormatoModelo3D.STL, 3.20m, comercial),
                };

                dbContext.Modelos3D.AddRange(modelos);
                dbContext.SaveChanges();
            }
        }

        public static void SeedAccesorios(ApplicationDbContext context) {
            if (context.Accesorios.Any()) {
                return; 
            }

            context.Accesorios.AddRange(
                new Accesorio {
                    Nombre = "Boquilla de Latón 0.4mm",
                    Categoria = CategoriaAccesorio.Boquillas,
                    Compatibilidad = "Ender 3, Prusa i3",
                    CantidadDisponible = 50,
                    Precio = 5.99m
                },
                new Accesorio {
                    Nombre = "Base PEI Magnética",
                    Categoria = CategoriaAccesorio.BasesDeImpresion,
                    Compatibilidad = "Artillery Genius",
                    CantidadDisponible = 15,
                    Precio = 25.50m
                },
                new Accesorio {
                    Nombre = "Alicates de Corte de Precisión",
                    Categoria = CategoriaAccesorio.Herramientas,
                    Compatibilidad = "Universal",
                    CantidadDisponible = 20,
                    Precio = 8.50m
                },
                new Accesorio {
                    Nombre = "Kit de Agujas Desatascadoras",
                    Categoria = CategoriaAccesorio.KitsDeLimpieza,
                    Compatibilidad = "Boquillas 0.4mm",
                    CantidadDisponible = 30,
                    Precio = 12.00m
                },
                new Accesorio {
                    Nombre = "Ventilador de Capa 5015",
                    Categoria = CategoriaAccesorio.Repuestos,
                    Compatibilidad = "Creality CR-10",
                    CantidadDisponible = 10,
                    Precio = 14.99m
                }
            );

            context.SaveChanges();
        }

        public static void SeedImpresoras3D(ApplicationDbContext dbContext) {
            if (!dbContext.Impresoras3D.Any(i => i.Nombre == "Prusa i3 MK3S+")) {
                var prusa = new Impresora3D {
                    Nombre = "Prusa i3 MK3S+",
                    Modelo = "MK3S+",
                    Tipo = TipoImpresora.Filamento,
                    Descripcion = "Impresora FDM de alta precisión para prototipado rápido.",
                    PrecioKilovatioHora = 0.25m,
                    PrecioReserva = 15.00m
                };
                dbContext.Impresoras3D.Add(prusa);
            }

            if (!dbContext.Impresoras3D.Any(i => i.Nombre == "Elegoo Mars 4")) {
                var elegoo = new Impresora3D {
                    Nombre = "Elegoo Mars 4",
                    Modelo = "Ultra 9K",
                    Tipo = TipoImpresora.Resina,
                    Descripcion = "Impresora de resina MSLA con resolución 9K para miniaturas detalladas.",
                    PrecioKilovatioHora = 0.30m,
                    PrecioReserva = 25.50m
                };
                dbContext.Impresoras3D.Add(elegoo);
            }

            dbContext.SaveChanges();
        }

        public static void SeedReservaImpresora(ApplicationDbContext dbContext, Client client) {
            if (!dbContext.ReservasImpresora.Any()) {
                var impresora = dbContext.Impresoras3D.FirstOrDefault(i => i.Nombre == "Prusa i3 MK3S+");

                if (impresora != null) {
                    var reserva = new ReservaImpresora {
                        FechaReserva = DateTime.Now,
                        NombreCliente = client.Name ?? "Peter",
                        ApellidosCliente = client.Surname ?? "Jackson",
                        DireccionFacturacion = client.DireccionFacturacion ?? "Campus Universitario s/n, Albacete 02071",
                        MetodoPago = MetodoPago.TarjetaCredito,
                        ClienteId = client.Id,
                        Cliente = client,
                        LineasReserva = new List<LineaReserva>()
                    };

                    var linea = new LineaReserva {
                        TiempoReserva = TiempoReserva.DosHoras,
                        PrecioSubtotal = 30.00m,
                        ImpresoraId = impresora.Id,
                        Impresora = impresora,
                        Reserva = reserva
                    };

                    reserva.LineasReserva.Add(linea);
                    reserva.PrecioTotal = linea.PrecioSubtotal;

                    dbContext.ReservasImpresora.Add(reserva);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}

