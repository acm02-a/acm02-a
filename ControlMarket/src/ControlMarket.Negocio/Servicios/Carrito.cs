using ControlMarket.Entidades.Dtos;

namespace ControlMarket.Negocio.Servicios;

/// <summary>Venta en curso en la caja (en memoria hasta que se cobra).</summary>
public class Carrito
{
    private readonly List<ItemCarritoDto> _items = new();

    public IReadOnlyList<ItemCarritoDto> Items => _items;
    public bool EstaVacio => _items.Count == 0;
    public decimal Total => Math.Round(_items.Sum(i => i.Importe), 2);

    public int CantidadDe(int idProducto) => _items.Where(i => i.IdProducto == idProducto).Sum(i => i.Cantidad);

    internal void Agregar(ItemCarritoDto nuevo)
    {
        var idx = _items.FindIndex(i => i.IdProducto == nuevo.IdProducto);
        if (idx >= 0)
            _items[idx] = _items[idx] with { Cantidad = _items[idx].Cantidad + nuevo.Cantidad };
        else
            _items.Add(nuevo);
    }

    public void Quitar(int idProducto) => _items.RemoveAll(i => i.IdProducto == idProducto);

    public void Vaciar() => _items.Clear();

    /// <summary>Subtotal (base imponible) e IGV, con precios que ya incluyen IGV.</summary>
    public (decimal Subtotal, decimal Igv, decimal Total) Totales(decimal porcentajeIgv)
    {
        var total = Total;
        var subtotal = Math.Round(total / (1 + porcentajeIgv / 100m), 2);
        return (subtotal, total - subtotal, total);
    }
}
