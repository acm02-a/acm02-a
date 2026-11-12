namespace ControlMarket.Entidades.Dtos;

public record StockProductoDto(
    int IdProducto, string CodigoBarras, string Producto, int StockActual,
    int StockMinimo, DateTime? ProximoVencimiento, string Estado);

public record AlertaStockDto(string CodigoBarras, string Producto, int StockActual, int StockMinimo);

public record AlertaVencimientoDto(int IdLote, string Lote, string Producto, int Cantidad, DateTime FechaVencimiento);

public record VentaPorFechaDto(DateTime Fecha, string Cajero, int NumeroVentas, decimal Total);

public record RentabilidadDto(string Grupo, decimal Ingreso, decimal Costo, decimal GananciaNeta);

public record ProductoCriticoDto(string Producto, string Motivo, int Stock, DateTime? Vence);

public record TopProductoDto(string Producto, int Unidades);

public record ItemCarritoDto(int IdProducto, string CodigoBarras, string Producto, int Cantidad, decimal PrecioUnitario)
{
    public decimal Importe => Cantidad * PrecioUnitario;
}
