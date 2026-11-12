using ControlMarket.Datos;
using ControlMarket.Entidades;
using ControlMarket.Negocio.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace ControlMarket.Negocio.Servicios;

public class ParametroService : ServicioBase
{
    public ParametroService(IContextFactory factory, Sesion sesion) : base(factory, sesion) { }

    public Parametro Obtener()
    {
        using var db = Factory.Crear();
        return db.Parametros.AsNoTracking().OrderBy(p => p.IdParametro).FirstOrDefault()
               ?? new Parametro { RazonSocial = "ControlMarket", Ruc = "00000000000" };
    }

    /// <summary>RF Parámetros generales.</summary>
    public Parametro Guardar(string? razonSocial, string? ruc, string? direccion, decimal igv, int diasAlerta)
    {
        Sesion.Exigir(Modulo.Parametros);
        if (igv < 0 || igv > 100) throw new NegocioException("El IGV debe estar entre 0 y 100.");
        if (diasAlerta < 0 || diasAlerta > 365) throw new NegocioException("Los días de aviso deben estar entre 0 y 365.");

        using var db = Factory.Crear();
        var p = db.Parametros.OrderBy(x => x.IdParametro).FirstOrDefault();
        if (p == null)
        {
            p = new Parametro();
            db.Parametros.Add(p);
        }
        p.RazonSocial = Requerido(razonSocial, "Razón social", 150);
        p.Ruc = Digitos(ruc, "RUC", 11);
        p.Direccion = Opcional(direccion, "Dirección", 200);
        p.PorcentajeIgv = igv;
        p.DiasAlertaVencimiento = diasAlerta;
        db.SaveChanges();
        return p;
    }
}
