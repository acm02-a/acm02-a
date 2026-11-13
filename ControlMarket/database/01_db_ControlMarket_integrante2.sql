USE [master]
GO
/****** Object:  Database [db_ControlMarket]    Script Date: 12/11/2026 18:48:04 ******/
CREATE DATABASE [db_ControlMarket]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'db_ControlMarket', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQL2026\MSSQL\DATA\db_ControlMarket.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'db_ControlMarket_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQL2026\MSSQL\DATA\db_ControlMarket_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [db_ControlMarket] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [db_ControlMarket].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [db_ControlMarket] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [db_ControlMarket] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [db_ControlMarket] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [db_ControlMarket] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [db_ControlMarket] SET ARITHABORT OFF 
GO
ALTER DATABASE [db_ControlMarket] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [db_ControlMarket] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [db_ControlMarket] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [db_ControlMarket] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [db_ControlMarket] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [db_ControlMarket] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [db_ControlMarket] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [db_ControlMarket] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [db_ControlMarket] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [db_ControlMarket] SET  DISABLE_BROKER 
GO
ALTER DATABASE [db_ControlMarket] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [db_ControlMarket] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [db_ControlMarket] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [db_ControlMarket] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [db_ControlMarket] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [db_ControlMarket] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [db_ControlMarket] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [db_ControlMarket] SET RECOVERY FULL 
GO
ALTER DATABASE [db_ControlMarket] SET  MULTI_USER 
GO
ALTER DATABASE [db_ControlMarket] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [db_ControlMarket] SET DB_CHAINING OFF 
GO
ALTER DATABASE [db_ControlMarket] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [db_ControlMarket] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [db_ControlMarket] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [db_ControlMarket] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'db_ControlMarket', N'ON'
GO
ALTER DATABASE [db_ControlMarket] SET QUERY_STORE = ON
GO
ALTER DATABASE [db_ControlMarket] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [db_ControlMarket]
GO
/****** Object:  Table [dbo].[Categoria]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categoria](
	[IdCategoria] [int] NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_Categoria] PRIMARY KEY CLUSTERED 
(
	[IdCategoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Comprobante]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comprobante](
	[IdComprobante] [int] IDENTITY(1,1) NOT NULL,
	[IdVenta] [int] NOT NULL,
	[Tipo] [varchar](20) NOT NULL,
	[Serie] [varchar](10) NOT NULL,
	[Numero] [int] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_Comprobante] PRIMARY KEY CLUSTERED 
(
	[IdComprobante] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleVenta]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleVenta](
	[IdDetalleVenta] [int] IDENTITY(1,1) NOT NULL,
	[IdVenta] [int] NOT NULL,
	[IdProducto] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[PrecioUnitario] [decimal](10, 2) NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_DetalleVenta] PRIMARY KEY CLUSTERED 
(
	[IdDetalleVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IngresoMercaderia]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IngresoMercaderia](
	[IdIngreso] [int] IDENTITY(1,1) NOT NULL,
	[IdProveedor] [int] NOT NULL,
	[IdJefeAlmacen] [int] NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_IngresoMercaderia] PRIMARY KEY CLUSTERED 
(
	[IdIngreso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Lote]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Lote](
	[IdLote] [int] IDENTITY(1,1) NOT NULL,
	[IdProducto] [int] NOT NULL,
	[IdIngreso] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[FechaIngreso] [datetime] NOT NULL,
	[FechaVencimiento] [datetime] NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_Lote] PRIMARY KEY CLUSTERED 
(
	[IdLote] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MetodoPago]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetodoPago](
	[IdMetodoPago] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_MetodoPago] PRIMARY KEY CLUSTERED 
(
	[IdMetodoPago] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Producto]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Producto](
	[IdProducto] [int] IDENTITY(1,1) NOT NULL,
	[IdCategoria] [int] NOT NULL,
	[CodigoBarras] [varchar](50) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[PrecioCompra] [decimal](10, 2) NOT NULL,
	[PrecioVenta] [decimal](10, 2) NOT NULL,
	[StockMinimo] [int] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_Producto] PRIMARY KEY CLUSTERED 
(
	[IdProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Proveedor]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Proveedor](
	[IdProveedor] [int] IDENTITY(1,1) NOT NULL,
	[Ruc] [varchar](11) NOT NULL,
	[RazonSocial] [varchar](150) NOT NULL,
	[Telefono] [varchar](15) NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_Proveedor] PRIMARY KEY CLUSTERED 
(
	[IdProveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rol]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rol](
	[IdRol] [int] IDENTITY(1,1) NOT NULL,
	[NombreRol] [varchar](50) NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_Rol] PRIMARY KEY CLUSTERED 
(
	[IdRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuario]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuario](
	[IdUsuario] [int] IDENTITY(1,1) NOT NULL,
	[IdRol] [int] NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[PasswordHash] [varchar](256) NOT NULL,
	[Activo] [bit] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Venta]    Script Date: 12/11/2026 18:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Venta](
	[IdVenta] [int] IDENTITY(1,1) NOT NULL,
	[IdCajero] [int] NOT NULL,
	[IdMetodoPago] [int] NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Subtotal] [decimal](10, 2) NOT NULL,
	[Igv] [decimal](10, 2) NOT NULL,
	[Total] [decimal](10, 2) NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_Venta] PRIMARY KEY CLUSTERED 
(
	[IdVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Comprobante]  WITH CHECK ADD  CONSTRAINT [FK_Comprobante_Venta] FOREIGN KEY([IdVenta])
REFERENCES [dbo].[Venta] ([IdVenta])
GO
ALTER TABLE [dbo].[Comprobante] CHECK CONSTRAINT [FK_Comprobante_Venta]
GO
ALTER TABLE [dbo].[DetalleVenta]  WITH CHECK ADD  CONSTRAINT [FK_DetalleVenta_Producto] FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Producto] ([IdProducto])
GO
ALTER TABLE [dbo].[DetalleVenta] CHECK CONSTRAINT [FK_DetalleVenta_Producto]
GO
ALTER TABLE [dbo].[DetalleVenta]  WITH CHECK ADD  CONSTRAINT [FK_DetalleVenta_Venta] FOREIGN KEY([IdVenta])
REFERENCES [dbo].[Venta] ([IdVenta])
GO
ALTER TABLE [dbo].[DetalleVenta] CHECK CONSTRAINT [FK_DetalleVenta_Venta]
GO
ALTER TABLE [dbo].[IngresoMercaderia]  WITH CHECK ADD  CONSTRAINT [FK_IngresoMercaderia_Proveedor] FOREIGN KEY([IdProveedor])
REFERENCES [dbo].[Proveedor] ([IdProveedor])
GO
ALTER TABLE [dbo].[IngresoMercaderia] CHECK CONSTRAINT [FK_IngresoMercaderia_Proveedor]
GO
ALTER TABLE [dbo].[IngresoMercaderia]  WITH CHECK ADD  CONSTRAINT [FK_IngresoMercaderia_Usuario] FOREIGN KEY([IdJefeAlmacen])
REFERENCES [dbo].[Usuario] ([IdUsuario])
GO
ALTER TABLE [dbo].[IngresoMercaderia] CHECK CONSTRAINT [FK_IngresoMercaderia_Usuario]
GO
ALTER TABLE [dbo].[Lote]  WITH CHECK ADD  CONSTRAINT [FK_Lote_IngresoMercaderia] FOREIGN KEY([IdIngreso])
REFERENCES [dbo].[IngresoMercaderia] ([IdIngreso])
GO
ALTER TABLE [dbo].[Lote] CHECK CONSTRAINT [FK_Lote_IngresoMercaderia]
GO
ALTER TABLE [dbo].[Lote]  WITH CHECK ADD  CONSTRAINT [FK_Lote_Producto] FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Producto] ([IdProducto])
GO
ALTER TABLE [dbo].[Lote] CHECK CONSTRAINT [FK_Lote_Producto]
GO
ALTER TABLE [dbo].[Producto]  WITH CHECK ADD  CONSTRAINT [FK_Producto_Categoria] FOREIGN KEY([IdCategoria])
REFERENCES [dbo].[Categoria] ([IdCategoria])
GO
ALTER TABLE [dbo].[Producto] CHECK CONSTRAINT [FK_Producto_Categoria]
GO
ALTER TABLE [dbo].[Usuario]  WITH CHECK ADD  CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY([IdRol])
REFERENCES [dbo].[Rol] ([IdRol])
GO
ALTER TABLE [dbo].[Usuario] CHECK CONSTRAINT [FK_Usuario_Rol]
GO
ALTER TABLE [dbo].[Venta]  WITH CHECK ADD  CONSTRAINT [FK_Venta_MetodoPago] FOREIGN KEY([IdMetodoPago])
REFERENCES [dbo].[MetodoPago] ([IdMetodoPago])
GO
ALTER TABLE [dbo].[Venta] CHECK CONSTRAINT [FK_Venta_MetodoPago]
GO
ALTER TABLE [dbo].[Venta]  WITH CHECK ADD  CONSTRAINT [FK_Venta_Usuario] FOREIGN KEY([IdCajero])
REFERENCES [dbo].[Usuario] ([IdUsuario])
GO
ALTER TABLE [dbo].[Venta] CHECK CONSTRAINT [FK_Venta_Usuario]
GO
USE [master]
GO
ALTER DATABASE [db_ControlMarket] SET  READ_WRITE 
GO
