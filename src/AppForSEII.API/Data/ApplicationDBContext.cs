using AppForSEII.API.Models;
using AppForSEII.API.Models.UC_Reservar;
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
    public DbSet<Impresora3D> Impresoras3D { get; set; }
    public DbSet<LineaReserva> LineasReserva { get; set; }
    public DbSet<ReservaImpresora> ReservasImpresora { get; set; }

    public DbSet<Client> Clientes { get; set; }


    // DbSets del caso de uso Encargar impresión de piezas 3D
    public DbSet<Material> Materiales { get; set; }
    public DbSet<Pieza3D> Piezas3D { get; set; }
    public DbSet<LineaEncargo> LineasEncargo { get; set; }
    public DbSet<EncargoImpresion> EncargosImpresion { get; set; }

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