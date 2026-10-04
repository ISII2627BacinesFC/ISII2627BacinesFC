using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs.ApplicationUserDTO;

namespace AppForSEII.API.Data;
using AppForSEII.API.Models.ComprarAccesorios;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }


    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public DbSet<Client> Clientes { get; set; }


    // DbSets del caso de uso Comprar Modelos 3D
    public DbSet<LicenciaModelo3D> LicenciasModelo3D { get; set; }
    public DbSet<Modelo3D> Modelos3D { get; set; }
    public DbSet<CompraModelo3D> ComprasModelo3D { get; set; }
    public DbSet<LineaCompraModelo> LineasCompraModelo { get; set; }

    // DbSets del caso de uso Comprar Accesorios
    public DbSet<Accesorio> Accesorios { get; set; }
    public DbSet<CompraAccesorios> ComprasAccesorios { get; set; }
    public DbSet<LineaCompraAccesorio> LineasCompraAccesorio { get; set; }

}