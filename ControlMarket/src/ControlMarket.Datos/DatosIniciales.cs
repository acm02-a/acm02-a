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
            db.Roles.AddRange(
                new Rol { IdRol = Rol.Administrador, NombreRol = "Administrador" },
                new Rol { IdRol = Rol.JefeAlmacen, NombreRol = "JefeAlmacen" },
                new Rol { IdRol = Rol.Cajero, NombreRol = "Cajero" });
        }

        if (!db.MetodosPago.Any())
        {
            db.MetodosPago.AddRange(
                new MetodoPago { IdMetodoPago = MetodoPago.Efectivo, Nombre = "Efectivo" },
                new MetodoPago { IdMetodoPago = 2, Nombre = "Tarjeta" },
                new MetodoPago { IdMetodoPago = 3, Nombre = "BilleteraDigital" });
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
