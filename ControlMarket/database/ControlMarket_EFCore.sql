IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [Categoria] (
        [IdCategoria] int NOT NULL IDENTITY,
        [Nombre] varchar(50) NOT NULL,
        [Descripcion] varchar(150) NULL,
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_Categoria] PRIMARY KEY ([IdCategoria])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [MetodoPago] (
        [IdMetodoPago] int NOT NULL,
        [Nombre] varchar(50) NOT NULL,
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_MetodoPago] PRIMARY KEY ([IdMetodoPago])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [Parametro] (
        [IdParametro] int NOT NULL IDENTITY,
        [RazonSocial] varchar(150) NOT NULL,
        [Ruc] char(11) NOT NULL,
        [Direccion] varchar(200) NULL,
        [PorcentajeIgv] decimal(5,2) NOT NULL,
        [DiasAlertaVencimiento] int NOT NULL,
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_Parametro] PRIMARY KEY ([IdParametro]),
        CONSTRAINT [CK_Parametro_Dias] CHECK (DiasAlertaVencimiento >= 0),
        CONSTRAINT [CK_Parametro_Igv] CHECK (PorcentajeIgv BETWEEN 0 AND 100)
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [Proveedor] (
        [IdProveedor] int NOT NULL IDENTITY,
        [Ruc] char(11) NOT NULL,
        [RazonSocial] varchar(150) NOT NULL,
        [Telefono] varchar(15) NULL,
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_Proveedor] PRIMARY KEY ([IdProveedor])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [Rol] (
        [IdRol] int NOT NULL,
        [NombreRol] varchar(50) NOT NULL,
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_Rol] PRIMARY KEY ([IdRol])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [Producto] (
        [IdProducto] int NOT NULL IDENTITY,
        [IdCategoria] int NOT NULL,
        [CodigoBarras] varchar(50) NOT NULL,
        [Nombre] varchar(100) NOT NULL,
        [PrecioCompra] decimal(10,2) NOT NULL,
        [PrecioVenta] decimal(10,2) NOT NULL,
        [StockMinimo] int NOT NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_Producto] PRIMARY KEY ([IdProducto]),
        CONSTRAINT [CK_Producto_Precios] CHECK (PrecioCompra >= 0 AND PrecioVenta >= 0),
        CONSTRAINT [CK_Producto_StockMinimo] CHECK (StockMinimo >= 0),
        CONSTRAINT [FK_Producto_Categoria_IdCategoria] FOREIGN KEY ([IdCategoria]) REFERENCES [Categoria] ([IdCategoria]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [Usuario] (
        [IdUsuario] int NOT NULL IDENTITY,
        [IdRol] int NOT NULL,
        [Nombre] varchar(100) NOT NULL,
        [Username] varchar(50) NOT NULL,
        [PasswordHash] varchar(256) NOT NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_Usuario] PRIMARY KEY ([IdUsuario]),
        CONSTRAINT [FK_Usuario_Rol_IdRol] FOREIGN KEY ([IdRol]) REFERENCES [Rol] ([IdRol]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [IngresoMercaderia] (
        [IdIngreso] int NOT NULL IDENTITY,
        [IdProveedor] int NOT NULL,
        [IdJefeAlmacen] int NOT NULL,
        [Fecha] datetime NOT NULL,
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_IngresoMercaderia] PRIMARY KEY ([IdIngreso]),
        CONSTRAINT [FK_IngresoMercaderia_Proveedor_IdProveedor] FOREIGN KEY ([IdProveedor]) REFERENCES [Proveedor] ([IdProveedor]) ON DELETE NO ACTION,
        CONSTRAINT [FK_IngresoMercaderia_Usuario_IdJefeAlmacen] FOREIGN KEY ([IdJefeAlmacen]) REFERENCES [Usuario] ([IdUsuario]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [Venta] (
        [IdVenta] int NOT NULL IDENTITY,
        [IdCajero] int NOT NULL,
        [IdMetodoPago] int NOT NULL,
        [Fecha] datetime NOT NULL,
        [Subtotal] decimal(10,2) NOT NULL,
        [Igv] decimal(10,2) NOT NULL,
        [Total] decimal(10,2) NOT NULL,
        [MontoRecibido] decimal(10,2) NOT NULL,
        [Vuelto] decimal(10,2) NOT NULL,
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_Venta] PRIMARY KEY ([IdVenta]),
        CONSTRAINT [CK_Venta_Montos] CHECK (Subtotal >= 0 AND Igv >= 0 AND Total >= 0),
        CONSTRAINT [FK_Venta_MetodoPago_IdMetodoPago] FOREIGN KEY ([IdMetodoPago]) REFERENCES [MetodoPago] ([IdMetodoPago]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Venta_Usuario_IdCajero] FOREIGN KEY ([IdCajero]) REFERENCES [Usuario] ([IdUsuario]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [Lote] (
        [IdLote] int NOT NULL IDENTITY,
        [IdProducto] int NOT NULL,
        [IdIngreso] int NOT NULL,
        [Cantidad] int NOT NULL,
        [CantidadInicial] int NOT NULL,
        [FechaIngreso] datetime NOT NULL,
        [FechaVencimiento] datetime NOT NULL,
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_Lote] PRIMARY KEY ([IdLote]),
        CONSTRAINT [CK_Lote_Cantidad] CHECK (Cantidad >= 0 AND Cantidad <= CantidadInicial),
        CONSTRAINT [CK_Lote_Fechas] CHECK (FechaVencimiento >= FechaIngreso),
        CONSTRAINT [FK_Lote_IngresoMercaderia_IdIngreso] FOREIGN KEY ([IdIngreso]) REFERENCES [IngresoMercaderia] ([IdIngreso]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Lote_Producto_IdProducto] FOREIGN KEY ([IdProducto]) REFERENCES [Producto] ([IdProducto]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [Comprobante] (
        [IdComprobante] int NOT NULL IDENTITY,
        [IdVenta] int NOT NULL,
        [Tipo] varchar(20) NOT NULL,
        [Serie] varchar(10) NOT NULL,
        [Numero] int NOT NULL,
        [DocumentoCliente] varchar(11) NULL,
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_Comprobante] PRIMARY KEY ([IdComprobante]),
        CONSTRAINT [CK_Comprobante_Tipo] CHECK (Tipo IN ('Boleta','Factura')),
        CONSTRAINT [FK_Comprobante_Venta_IdVenta] FOREIGN KEY ([IdVenta]) REFERENCES [Venta] ([IdVenta]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE TABLE [DetalleVenta] (
        [IdDetalleVenta] int NOT NULL IDENTITY,
        [IdVenta] int NOT NULL,
        [IdProducto] int NOT NULL,
        [Cantidad] int NOT NULL,
        [PrecioUnitario] decimal(10,2) NOT NULL,
        [CostoUnitario] decimal(10,2) NOT NULL,
        [CreatedBy] varchar(50) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [ModifiedBy] varchar(50) NULL,
        [ModifiedAt] datetime NULL,
        CONSTRAINT [PK_DetalleVenta] PRIMARY KEY ([IdDetalleVenta]),
        CONSTRAINT [CK_DetalleVenta_Cantidad] CHECK (Cantidad > 0),
        CONSTRAINT [FK_DetalleVenta_Producto_IdProducto] FOREIGN KEY ([IdProducto]) REFERENCES [Producto] ([IdProducto]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DetalleVenta_Venta_IdVenta] FOREIGN KEY ([IdVenta]) REFERENCES [Venta] ([IdVenta]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Categoria_Nombre] ON [Categoria] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Comprobante_IdVenta] ON [Comprobante] ([IdVenta]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Comprobante_Serie_Numero] ON [Comprobante] ([Serie], [Numero]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_DetalleVenta_IdProducto] ON [DetalleVenta] ([IdProducto]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_DetalleVenta_IdVenta] ON [DetalleVenta] ([IdVenta]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_IngresoMercaderia_IdJefeAlmacen] ON [IngresoMercaderia] ([IdJefeAlmacen]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_IngresoMercaderia_IdProveedor] ON [IngresoMercaderia] ([IdProveedor]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_Lote_IdIngreso] ON [Lote] ([IdIngreso]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_Lote_IdProducto_FechaVencimiento] ON [Lote] ([IdProducto], [FechaVencimiento]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MetodoPago_Nombre] ON [MetodoPago] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Producto_CodigoBarras] ON [Producto] ([CodigoBarras]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_Producto_IdCategoria] ON [Producto] ([IdCategoria]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Proveedor_Ruc] ON [Proveedor] ([Ruc]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Rol_NombreRol] ON [Rol] ([NombreRol]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_Usuario_IdRol] ON [Usuario] ([IdRol]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuario_Username] ON [Usuario] ([Username]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_Venta_Fecha] ON [Venta] ([Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_Venta_IdCajero] ON [Venta] ([IdCajero]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    CREATE INDEX [IX_Venta_IdMetodoPago] ON [Venta] ([IdMetodoPago]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112210517_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261112210517_Inicial', N'8.0.11');
END;
GO

COMMIT;
GO

