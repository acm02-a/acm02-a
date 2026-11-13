using ControlMarket.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ControlMarket.Datos;

/// <summary>
/// Inserta los datos mínimos para que el sistema arranque (roles, métodos de pago, parámetros
/// y un administrador). Es idempotente: solo inserta lo que falta.
/// </summary>
public static class DatosIniciales
{
    public const string AdminUsername = "admin";
    public const string AdminPasswordInicial = "Admin123*";

    public static void Asegurar(ControlMarketContext db)
    {
        if (!db.Roles.Any())
        {
            // Uno por uno para que el IDENTITY asigne 1, 2, 3 en este orden.
            foreach (var nombre in new[] { "Administrador", "JefeAlmacen", "Cajero" })
            {
                db.Roles.Add(new Rol { NombreRol = nombre });
                db.SaveChanges();
            }
        }

        if (!db.MetodosPago.Any())
        {
            foreach (var nombre in new[] { "Efectivo", "Tarjeta", "BilleteraDigital" })
            {
                db.MetodosPago.Add(new MetodoPago { Nombre = nombre });
                db.SaveChanges();
            }
        }

        if (!db.Parametros.Any())
        {
            db.Parametros.Add(new Parametro
            {
                RazonSocial = "Supermercado San Martín S.A.C.",
                Ruc = "20601234567",
                Direccion = "Av. Los Próceres 1250, Surco",
                PorcentajeIgv = 18m,
                DiasAlertaVencimiento = 7
            });
        }

        db.SaveChanges();

        if (!db.Usuarios.Any(u => u.IdRol == Rol.Administrador))
        {
            db.Usuarios.Add(new Usuario
            {
                IdRol = Rol.Administrador,
                Nombre = "Administrador del sistema",
                Username = AdminUsername,
                PasswordHash = PasswordHasher.Hash(AdminPasswordInicial),
                Activo = true
            });
            db.SaveChanges();
        }
    }

    /// <summary>Aplica migraciones pendientes (crea la BD si no existe) y carga los datos mínimos.</summary>
    public static void Inicializar(ControlMarketContext db, bool aplicarMigraciones)
    {
        if (aplicarMigraciones && db.Database.IsRelational())
            db.Database.Migrate();
        else if (!db.Database.IsRelational())
            db.Database.EnsureCreated();
        Asegurar(db);
    }
}
