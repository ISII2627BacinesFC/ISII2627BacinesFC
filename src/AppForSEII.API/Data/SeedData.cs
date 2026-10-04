using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using AppForSEII.API.Models;
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
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }
                if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
                Client user = new Client("3", "Peter", "Jackson", "peter@uclm.es", "Campus Universitario s/n, Albacete 02071") {
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, "OtherPass12$");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager.AddToRoleAsync(user, roles[2]).Wait();
                }
            }
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
            if (!dbContext.ReservasImpresora.Any(r => r.Id == 1)) {
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



