/* =====================================================================
   ControlMarket - Ajustes sobre db_ControlMarket (script del Integrante 2)
   Ejecutar DESPUÉS de 01_db_ControlMarket_integrante2.sql.
   Es idempotente: se puede ejecutar varias veces sin error y no borra datos.

   Agrega lo que necesitan las pantallas y la capa de negocio:
     - Columnas nuevas (Activo, Descripcion, CantidadInicial, MontoRecibido,
       Vuelto, CostoUnitario, DocumentoCliente)
     - Tabla Parametro (RF Parámetros generales)
     - Restricciones UNIQUE, CHECK y DEFAULT del diccionario de datos
   ===================================================================== */
USE [db_ControlMarket];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ---------- 1. Columnas nuevas ---------- */
IF COL_LENGTH('dbo.Categoria', 'Descripcion') IS NULL
    ALTER TABLE dbo.Categoria ADD Descripcion varchar(150) NULL;

IF COL_LENGTH('dbo.Producto', 'Activo') IS NULL
    ALTER TABLE dbo.Producto ADD Activo bit NOT NULL CONSTRAINT DF_Producto_Activo DEFAULT (1);

IF COL_LENGTH('dbo.Lote', 'CantidadInicial') IS NULL
    ALTER TABLE dbo.Lote ADD CantidadInicial int NOT NULL CONSTRAINT DF_Lote_CantidadInicial DEFAULT (0);

IF COL_LENGTH('dbo.Venta', 'MontoRecibido') IS NULL
    ALTER TABLE dbo.Venta ADD MontoRecibido decimal(10,2) NOT NULL CONSTRAINT DF_Venta_MontoRecibido DEFAULT (0);

IF COL_LENGTH('dbo.Venta', 'Vuelto') IS NULL
    ALTER TABLE dbo.Venta ADD Vuelto decimal(10,2) NOT NULL CONSTRAINT DF_Venta_Vuelto DEFAULT (0);

IF COL_LENGTH('dbo.DetalleVenta', 'CostoUnitario') IS NULL
    ALTER TABLE dbo.DetalleVenta ADD CostoUnitario decimal(10,2) NOT NULL CONSTRAINT DF_DetalleVenta_CostoUnitario DEFAULT (0);

IF COL_LENGTH('dbo.Comprobante', 'DocumentoCliente') IS NULL
    ALTER TABLE dbo.Comprobante ADD DocumentoCliente varchar(11) NULL;
GO

/* ---------- 2. Datos existentes y tipos corregidos ---------- */
-- Lotes ya cargados: lo recibido = lo disponible
UPDATE dbo.Lote SET CantidadInicial = Cantidad WHERE CantidadInicial < Cantidad;

-- Ventas ya cargadas: costo histórico = precio de compra actual
UPDATE d SET d.CostoUnitario = p.PrecioCompra
FROM dbo.DetalleVenta d JOIN dbo.Producto p ON p.IdProducto = d.IdProducto
WHERE d.CostoUnitario = 0;

-- Diccionario de datos: Producto.Nombre es VARCHAR(100)
ALTER TABLE dbo.Producto ALTER COLUMN Nombre varchar(100) NOT NULL;

-- Todo lote debe tener fecha de vencimiento (alertas y FEFO)
UPDATE dbo.Lote SET FechaVencimiento = DATEADD(YEAR, 1, FechaIngreso) WHERE FechaVencimiento IS NULL;
ALTER TABLE dbo.Lote ALTER COLUMN FechaVencimiento datetime NOT NULL;
GO

/* ---------- 3. Tabla Parametro ---------- */
IF OBJECT_ID('dbo.Parametro', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Parametro (
        IdParametro           int IDENTITY(1,1) NOT NULL,
        RazonSocial           varchar(150) NOT NULL,
        Ruc                   varchar(11)  NOT NULL,
        Direccion             varchar(200) NULL,
        PorcentajeIgv         decimal(5,2) NOT NULL CONSTRAINT DF_Parametro_Igv DEFAULT (18),
        DiasAlertaVencimiento int          NOT NULL CONSTRAINT DF_Parametro_Dias DEFAULT (7),
        CreatedBy             varchar(50)  NOT NULL,
        CreatedAt             datetime     NOT NULL CONSTRAINT DF_Parametro_CreatedAt DEFAULT (GETDATE()),
        ModifiedBy            varchar(50)  NULL,
        ModifiedAt            datetime     NULL,
        CONSTRAINT PK_Parametro PRIMARY KEY (IdParametro),
        CONSTRAINT CK_Parametro_Igv  CHECK (PorcentajeIgv BETWEEN 0 AND 100),
        CONSTRAINT CK_Parametro_Dias CHECK (DiasAlertaVencimiento >= 0)
    );
END
GO

/* ---------- 4. UNIQUE ---------- */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Rol_NombreRol')
    CREATE UNIQUE INDEX UQ_Rol_NombreRol ON dbo.Rol(NombreRol);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Usuario_Username')
    CREATE UNIQUE INDEX UQ_Usuario_Username ON dbo.Usuario(Username);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Categoria_Nombre')
    CREATE UNIQUE INDEX UQ_Categoria_Nombre ON dbo.Categoria(Nombre);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Producto_CodigoBarras')
    CREATE UNIQUE INDEX UQ_Producto_CodigoBarras ON dbo.Producto(CodigoBarras);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Proveedor_Ruc')
    CREATE UNIQUE INDEX UQ_Proveedor_Ruc ON dbo.Proveedor(Ruc);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_MetodoPago_Nombre')
    CREATE UNIQUE INDEX UQ_MetodoPago_Nombre ON dbo.MetodoPago(Nombre);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Comprobante_IdVenta')        -- relación 1 a 1
    CREATE UNIQUE INDEX UQ_Comprobante_IdVenta ON dbo.Comprobante(IdVenta);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Comprobante_Serie_Numero')
    CREATE UNIQUE INDEX UQ_Comprobante_Serie_Numero ON dbo.Comprobante(Serie, Numero);
GO

/* ---------- 5. Índices de consulta ---------- */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Lote_Producto_Vencimiento')
    CREATE INDEX IX_Lote_Producto_Vencimiento ON dbo.Lote(IdProducto, FechaVencimiento);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Venta_Fecha')
    CREATE INDEX IX_Venta_Fecha ON dbo.Venta(Fecha);
GO

/* ---------- 6. CHECK ---------- */
IF OBJECT_ID('CK_Producto_Precios') IS NULL
    ALTER TABLE dbo.Producto ADD CONSTRAINT CK_Producto_Precios CHECK (PrecioCompra >= 0 AND PrecioVenta >= 0);
IF OBJECT_ID('CK_Producto_StockMinimo') IS NULL
    ALTER TABLE dbo.Producto ADD CONSTRAINT CK_Producto_StockMinimo CHECK (StockMinimo >= 0);
IF OBJECT_ID('CK_Lote_Cantidad') IS NULL
    ALTER TABLE dbo.Lote ADD CONSTRAINT CK_Lote_Cantidad CHECK (Cantidad >= 0 AND Cantidad <= CantidadInicial);
IF OBJECT_ID('CK_Lote_Fechas') IS NULL
    ALTER TABLE dbo.Lote ADD CONSTRAINT CK_Lote_Fechas CHECK (FechaVencimiento >= FechaIngreso);
IF OBJECT_ID('CK_DetalleVenta_Cantidad') IS NULL
    ALTER TABLE dbo.DetalleVenta ADD CONSTRAINT CK_DetalleVenta_Cantidad CHECK (Cantidad > 0);
IF OBJECT_ID('CK_Venta_Montos') IS NULL
    ALTER TABLE dbo.Venta ADD CONSTRAINT CK_Venta_Montos CHECK (Subtotal >= 0 AND Igv >= 0 AND Total >= 0);
IF OBJECT_ID('CK_Comprobante_Tipo') IS NULL
    ALTER TABLE dbo.Comprobante ADD CONSTRAINT CK_Comprobante_Tipo CHECK (Tipo IN ('Boleta','Factura'));
GO

/* ---------- 7. DEFAULT de auditoría (CreatedAt = GETDATE()) ---------- */
DECLARE @t sysname, @sql nvarchar(400);
DECLARE c CURSOR LOCAL FAST_FORWARD FOR
    SELECT t.name FROM sys.tables t
    JOIN sys.columns col ON col.object_id = t.object_id AND col.name = 'CreatedAt'
    WHERE col.default_object_id = 0;
OPEN c;
FETCH NEXT FROM c INTO @t;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'ALTER TABLE dbo.' + QUOTENAME(@t) + N' ADD CONSTRAINT ' + QUOTENAME('DF_' + @t + '_CreatedAt') + N' DEFAULT (GETDATE()) FOR CreatedAt;';
    EXEC sp_executesql @sql;
    FETCH NEXT FROM c INTO @t;
END
CLOSE c; DEALLOCATE c;
GO

PRINT 'Ajustes aplicados. Los roles, métodos de pago, parámetros y el usuario admin los crea la aplicación al iniciar.';
GO
