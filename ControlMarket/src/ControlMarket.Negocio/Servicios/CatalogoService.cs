using ControlMarket.Datos;
using ControlMarket.Entidades;
using ControlMarket.Negocio.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace ControlMarket.Negocio.Servicios;

/// <summary>RF Gestión de categorías, proveedores y productos.</summary>
public class CatalogoService : ServicioBase
{
    public CatalogoService(IContextFactory factory, Sesion sesion) : base(factory, sesion) { }

    // ---------- Categorías ----------
    public List<Categoria> ListarCategorias()
    {
        using var db = Factory.Crear();
        return db.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToList();
    }

    public Categoria GuardarCategoria(int idCategoria, string? nombre, string? descripcion)
    {
        Sesion.Exigir(Modulo.CategoriasProveedores);
        var nom = Requerido(nombre, "Nombre", 50);
        using var db = Factory.Crear();
        if (db.Categorias.Any(c => c.Nombre == nom && c.IdCategoria != idCategoria))
            throw new NegocioException($"La categoría '{nom}' ya existe.");

        // IdCategoria no es IDENTITY en la BD: se asigna el siguiente número.
        var c = idCategoria == 0
            ? db.Categorias.Add(new Categoria { IdCategoria = (db.Categorias.Max(x => (int?)x.IdCategoria) ?? 0) + 1 }).Entity
            : db.Categorias.Find(idCategoria) ?? throw new NegocioException("La categoría no existe.");
        c.Nombre = nom;
        c.Descripcion = Opcional(descripcion, "Descripción", 150);
        db.SaveChanges();
        return c;
    }

    // ---------- Proveedores ----------
    public List<Proveedor> ListarProveedores()
    {
        using var db = Factory.Crear();
        return db.Proveedores.AsNoTracking().OrderBy(p => p.RazonSocial).ToList();
    }

    public Proveedor GuardarProveedor(int idProveedor, string? ruc, string? razonSocial, string? telefono)
    {
        Sesion.Exigir(Modulo.CategoriasProveedores);
        var r = Digitos(ruc, "RUC", 11);
        var rs = Requerido(razonSocial, "Razón social", 150);
        using var db = Factory.Crear();
        if (db.Proveedores.Any(p => p.Ruc == r && p.IdProveedor != idProveedor))
            throw new NegocioException($"Ya existe un proveedor con RUC {r}.");

        var p = idProveedor == 0 ? db.Proveedores.Add(new Proveedor()).Entity
            : db.Proveedores.Find(idProveedor) ?? throw new NegocioException("El proveedor no existe.");
        p.Ruc = r;
        p.RazonSocial = rs;
        p.Telefono = Opcional(telefono, "Teléfono", 15);
        db.SaveChanges();
        return p;
    }

    // ---------- Productos ----------
    public List<Producto> ListarProductos(string? filtro = null, bool soloActivos = false)
    {
        using var db = Factory.Crear();
        var q = db.Productos.AsNoTracking().Include(p => p.Categoria).AsQueryable();
        if (soloActivos) q = q.Where(p => p.Activo);
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var f = filtro.Trim();
            q = q.Where(p => p.Nombre.Contains(f) || p.CodigoBarras.Contains(f) || p.Categoria!.Nombre.Contains(f));
        }
        return q.OrderBy(p => p.Nombre).ToList();
    }

    public Producto GuardarProducto(int idProducto, string? codigoBarras, string? nombre, int idCategoria,
        decimal precioCompra, decimal precioVenta, int stockMinimo, bool activo)
    {
        Sesion.Exigir(Modulo.Productos);
        var codigo = Requerido(codigoBarras, "Código de barras", 50);
        var nom = Requerido(nombre, "Nombre", 100);
        if (precioCompra <= 0 || precioVenta <= 0)
            throw new NegocioException("Los precios deben ser mayores a cero.");
        if (precioVenta < precioCompra)
            throw new NegocioException("El precio de venta no puede ser menor que el precio de compra.");
        if (stockMinimo < 0)
            throw new NegocioException("El stock mínimo no puede ser negativo.");

        using var db = Factory.Crear();
        if (!db.Categorias.Any(c => c.IdCategoria == idCategoria))
            throw new NegocioException("Seleccione una categoría válida.");
        if (db.Productos.Any(p => p.CodigoBarras == codigo && p.IdProducto != idProducto))
            throw new NegocioException($"Ya existe un producto con el código de barras {codigo}.");

        var p = idProducto == 0 ? db.Productos.Add(new Producto()).Entity
            : db.Productos.Find(idProducto) ?? throw new NegocioException("El producto no existe.");
        p.CodigoBarras = codigo;
        p.Nombre = nom;
        p.IdCategoria = idCategoria;
        p.PrecioCompra = Math.Round(precioCompra, 2);
        p.PrecioVenta = Math.Round(precioVenta, 2);
        p.StockMinimo = stockMinimo;
        p.Activo = activo;
        db.SaveChanges();
        return p;
    }
}
