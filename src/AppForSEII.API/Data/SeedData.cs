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

            try
            {
                SeedPiezasYMateriales(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the Piezas3D and Materiales in the Database.");
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

        public static void SeedPiezasYMateriales (ApplicationDbContext dbContext) {
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





    }
}