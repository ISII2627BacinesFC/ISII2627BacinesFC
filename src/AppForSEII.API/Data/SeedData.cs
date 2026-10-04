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
                SeedLicenciasYModelos3D(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Licencias and Modelos3D in the Database.");
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
                //A customer class has been defined because it has different attributes (purchase, rental, etc.)
                ApplicationUser user = new ApplicationUser("3", "Peter", "Jackson", "peter@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //customer role
                    userManager.AddToRoleAsync(user, roles[2]).Wait();

                }
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

    }
}