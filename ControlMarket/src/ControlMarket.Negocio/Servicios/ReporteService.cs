using ControlMarket.Datos;
using ControlMarket.Entidades.Dtos;
using ControlMarket.Negocio.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace ControlMarket.Negocio.Servicios;

/// <summary>RF Reportes (Administrador).</summary>
public class ReporteService : ServicioBase
{
    private readonly InventarioService _inventario;

    public ReporteService(IContextFactory factory, Sesion sesion) : base(factory, sesion)
    {
        _inventario = new InventarioService(factory, sesion) { Reloj = () => Reloj() };
    }

    private static void ValidarRango(DateTime desde, DateTime hasta)
    {
        if (desde.Date > hasta.Date) throw new NegocioException("La fecha 'Desde' no puede ser mayor que 'Hasta'.");
    }

    /// <summary>Reporte de ventas por fechas, filtrado por rango y cajero (null = todos).</summary>
    public List<VentaPorFechaDto> VentasPorFecha(DateTime desde, DateTime hasta, int? idCajero = null)
    {
        Sesion.Exigir(Modulo.Reportes);
        ValidarRango(desde, hasta);
        var ini = desde.Date;
        var fin = hasta.Date.AddDays(1);
        using var db = Factory.Crear();
        var q = db.Ventas.AsNoTracking().Where(v => v.Fecha >= ini && v.Fecha < fin);
        if (idCajero.HasValue) q = q.Where(v => v.IdCajero == idCajero.Value);

        return q.GroupBy(v => new { Dia = v.Fecha.Date, v.Cajero!.Username })
            .Select(g => new { g.Key.Dia, g.Key.Username, N = g.Count(), Total = g.Sum(v => v.Total) })
            .OrderBy(x => x.Dia).ThenBy(x => x.Username)
            .AsEnumerable()
            .Select(x => new VentaPorFechaDto(x.Dia, x.Username, x.N, x.Total))
            .ToList();
    }

    /// <summary>Ganancia neta (precio de venta − precio de compra congelados) por producto o categoría.</summary>
    public List<RentabilidadDto> Rentabilidad(DateTime desde, DateTime hasta, bool porCategoria, int? idCajero = null)
    {
        Sesion.Exigir(Modulo.Reportes);
        ValidarRango(desde, hasta);
        var ini = desde.Date;
        var fin = hasta.Date.AddDays(1);
        using var db = Factory.Crear();
        var q = db.DetallesVenta.AsNoTracking().Where(d => d.Venta!.Fecha >= ini && d.Venta.Fecha < fin);
        if (idCajero.HasValue) q = q.Where(d => d.Venta!.IdCajero == idCajero.Value);

        var filas = porCategoria
            ? q.GroupBy(d => d.Producto!.Categoria!.Nombre)
                .Select(g => new { Grupo = g.Key, Ingreso = g.Sum(d => d.Cantidad * d.PrecioUnitario), Costo = g.Sum(d => d.Cantidad * d.CostoUnitario) })
                .ToList()
            : q.GroupBy(d => d.Producto!.Nombre)
                .Select(g => new { Grupo = g.Key, Ingreso = g.Sum(d => d.Cantidad * d.PrecioUnitario), Costo = g.Sum(d => d.Cantidad * d.CostoUnitario) })
                .ToList();

        return filas.Select(f => new RentabilidadDto(f.Grupo, f.Ingreso, f.Costo, f.Ingreso - f.Costo))
            .OrderByDescending(f => f.GananciaNeta).ToList();
    }

    /// <summary>Productos bajo stock mínimo y lotes próximos a vencer.</summary>
    public List<ProductoCriticoDto> ProductosCriticos()
    {
        Sesion.Exigir(Modulo.Reportes);
        var bajos = _inventario.AlertasStockMinimo()
            .Select(a => new ProductoCriticoDto(a.Producto, "Bajo stock mínimo", a.StockActual, null));
        var porVencer = _inventario.AlertasVencimiento()
            .Select(a => new ProductoCriticoDto(a.Producto, "Próximo a vencer", a.Cantidad, a.FechaVencimiento));
        return bajos.Concat(porVencer).ToList();
    }

    /// <summary>Top N de productos más vendidos (por unidades).</summary>
    public List<TopProductoDto> TopProductos(int top = 5, DateTime? desde = null, DateTime? hasta = null)
    {
        Sesion.Exigir(Modulo.Reportes);
        using var db = Factory.Crear();
        var q = db.DetallesVenta.AsNoTracking().AsQueryable();
        if (desde.HasValue) q = q.Where(d => d.Venta!.Fecha >= desde.Value.Date);
        if (hasta.HasValue) q = q.Where(d => d.Venta!.Fecha < hasta.Value.Date.AddDays(1));
        return q.GroupBy(d => d.Producto!.Nombre)
            .Select(g => new { Producto = g.Key, Unidades = g.Sum(d => d.Cantidad) })
            .OrderByDescending(x => x.Unidades)
            .Take(top)
            .AsEnumerable()
            .Select(x => new TopProductoDto(x.Producto, x.Unidades))
            .ToList();
    }
}
