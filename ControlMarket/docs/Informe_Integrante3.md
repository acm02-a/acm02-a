# Integrante 3 – Desarrollo C#, Entity Framework Core y arquitectura en capas

## 1. Tecnologías utilizadas

| Elemento | Tecnología |
|---|---|
| Lenguaje | C# 12 |
| Plataforma | .NET 8 (`net8.0-windows` para la interfaz) |
| Interfaz | Windows Forms (formularios del prototipo) |
| ORM | Entity Framework Core 8 (`Microsoft.EntityFrameworkCore.SqlServer`) |
| Base de datos | Microsoft SQL Server |
| Gráficos | `WinForms.DataVisualization` (control Chart) |
| Pruebas | xUnit + proveedor EF Core InMemory |

> Se migró el proyecto original (.NET Framework 4.7.2) a .NET 8 porque **EF Core 8 requiere .NET 8**.
> Los formularios del prototipo se conservaron y se conectaron a la lógica real.

## 2. Arquitectura en capas

La solución `ControlMarket.sln` está dividida en cuatro proyectos (una capa por proyecto) más un proyecto de pruebas:

| Capa | Proyecto | Responsabilidad |
|---|---|---|
| Entidades | `ControlMarket.Entidades` | Clases del modelo (`Usuario`, `Producto`, `Lote`, `Venta`, …), clase base `EntidadAuditable` y DTOs de reportes. No depende de ninguna otra capa. |
| Datos | `ControlMarket.Datos` | `ControlMarketContext` (DbContext), configuración Fluent API de cada tabla, migraciones, auditoría automática, hash de contraseñas y datos iniciales. |
| Negocio | `ControlMarket.Negocio` | Servicios con las reglas del sistema: `UsuarioService`, `ParametroService`, `CatalogoService`, `InventarioService`, `VentaService`, `ReporteService`; sesión y perfiles por rol. |
| Presentación | `ControlMarket.Presentacion` | Formularios Windows Forms. Solo llaman a los servicios de negocio; nunca acceden a la BD directamente. |
| Pruebas | `ControlMarket.Pruebas` | 23 pruebas automáticas de la lógica de negocio. |

Flujo de una operación (ejemplo, registrar un producto):

```
FrmProductos (btnGuardar) → CatalogoService.GuardarProducto() → valida reglas
   → ControlMarketContext.SaveChanges() → llena auditoría → SQL Server (INSERT/UPDATE)
```

`App.cs` (en Presentación) es la **raíz de composición**: lee `appsettings.json`, crea la fábrica
de contextos con la cadena de conexión y crea los servicios que usan los formularios.

## 3. Configuración de Entity Framework Core

- **DbContext:** `ControlMarketContext` expone un `DbSet` por tabla: `Rol`, `Usuario`, `Categoria`,
  `Producto`, `Proveedor`, `IngresoMercaderia`, `Lote`, `MetodoPago`, `Venta`, `DetalleVenta`,
  `Comprobante`, `Parametro`.
- **Fluent API** (`Datos/Configuraciones`): cada tabla tiene su clase `IEntityTypeConfiguration<T>`
  donde se definen nombre de tabla, PK, FK, `UNIQUE`, `NOT NULL`, tipos (`varchar(n)`, `decimal(10,2)`,
  `datetime`) y restricciones `CHECK`, siguiendo el diccionario de datos.
- **Conexión con SQL Server:** `ContextFactory.ParaSqlServer(cadena)` usa `UseSqlServer` con reintentos
  automáticos ante fallos transitorios (`EnableRetryOnFailure`). La cadena está en `appsettings.json`.
- **Contextos de vida corta:** cada operación crea y libera su propio `DbContext` (recomendado en
  aplicaciones de escritorio).
- **Migraciones:** la migración `Inicial` crea toda la BD; al iniciar la aplicación se aplica
  automáticamente (`Database.Migrate()`). El script equivalente está en `database/ControlMarket_EFCore.sql`.
- **Auditoría automática (RNF06):** todas las entidades heredan de `EntidadAuditable`
  (`CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`). `SaveChanges()` se sobrescribe para llenarlos
  con el usuario logueado y la fecha/hora, sin que los formularios tengan que hacerlo.

## 4. Relación con el diagrama de clases

| Diagrama de clases | Implementación |
|---|---|
| `CUsuario` (abstracta) y `ObtenerMenu()` | `Perfil` (abstracta) con `Modulos` y `ObtenerMenu()` |
| `CAdministrador`, `CJefeAlmacen`, `CCajero` | `PerfilAdministrador`, `PerfilJefeAlmacen`, `PerfilCajero` (polimorfismo: cada uno define sus módulos) |
| `CProducto.ObtenerStock()`, `EstaBajoMinimo()` | `Producto.ObtenerStock()`, `Producto.EstaBajoMinimo()` |
| `CLote.EstaPorVencer()`, `Descontar()` | `Lote.EstaPorVencer()`, `Lote.Descontar()` |
| `CVenta.AgregarProducto()`, `CalcularTotal()` | `VentaService.AgregarProducto()`, `Venta.CalcularTotal()` |
| `CDetalleVenta.CalcularSubtotal()` | `DetalleVenta.CalcularSubtotal()` |
| `CGestora` (controladora) | Servicios de la capa de negocio (se dividió por módulo para no tener una sola clase gigante) |

## 5. Lógica de negocio implementada (por requerimiento)

| Requerimiento | Dónde | Regla implementada |
|---|---|---|
| Autenticación por roles | `UsuarioService.IniciarSesion` | Contraseña verificada con hash PBKDF2-SHA256; usuarios inactivos no ingresan; el menú solo muestra los módulos del rol y además cada servicio verifica el permiso (`Sesion.Exigir`). |
| Gestión de usuarios | `UsuarioService.Guardar` | Username único, contraseña mínima de 6 caracteres guardada como hash, el admin no puede desactivarse a sí mismo. |
| Parámetros generales | `ParametroService` | IGV entre 0 y 100, días de aviso 0–365, RUC de 11 dígitos. |
| Campos de auditoría | `ControlMarketContext.SaveChanges` | Automático en todas las tablas y mostrado en cada pantalla. |
| Productos / Categorías / Proveedores | `CatalogoService` | Código de barras, nombre de categoría y RUC únicos; precio de venta ≥ precio de compra. |
| Ingreso por lotes | `InventarioService.RegistrarIngreso` | Crea `IngresoMercaderia` + `Lote` registrando al Jefe de Almacén; vencimiento posterior al ingreso. |
| Consulta de stock | `InventarioService.ConsultarStock` | Stock = suma de lotes **no vencidos**; estado Normal / Bajo mínimo / Por vencer. |
| Alertas | `AlertasStockMinimo`, `AlertasVencimiento` | Menú principal: filas rojas (stock ≤ mínimo) y amarillas (vencen dentro de los días configurados). |
| Registro en caja + restricción de stock | `VentaService.AgregarProducto` | No permite agregar más unidades de las disponibles (considerando lo que ya está en el carrito). |
| Cálculo de importes | `Carrito.Totales`, `Venta.CalcularTotal` | Precios incluyen IGV: Subtotal = Total / (1 + IGV); IGV = Total − Subtotal. |
| Métodos de pago y vuelto | `VentaService.Cobrar` | En efectivo el monto recibido debe cubrir el total; se calcula el vuelto. |
| Emisión de comprobantes | `VentaService.Cobrar` | Boleta (B001, DNI opcional de 8 dígitos) o Factura (F001, RUC obligatorio); número correlativo por serie. |
| Descuento de stock (FEFO) | `VentaService.Cobrar` | Descuenta primero del lote que vence antes. |
| Integridad de los datos (RNF) | `VentaService.Cobrar` | Venta, detalles, comprobante y descuento de stock en **una sola transacción**: si algo falla no se guarda nada. |
| Reportes | `ReporteService` | Ventas por fecha y cajero, rentabilidad por producto/categoría (con el costo congelado en la venta), productos críticos y Top 5 (gráfico). |
| Seguridad de acceso (RNF) | `MonitorInactividad` + `FrmMenuPrincipal` | Cierre automático de sesión tras N minutos sin actividad (configurable, 15 por defecto). |
| Usabilidad (RNF) | `FrmCaja` | Atajos F2 buscar, F4 quitar, F8 cobrar, F10 cancelar; Enter agrega el producto. |

## 6. Ajustes al modelo de datos (coordinar con Integrantes 1 y 2)

Para que los formularios del prototipo funcionen se necesitaron estos campos/tabla que no estaban en el
diagrama de BD original. Están incluidos en la migración y en `database/ControlMarket_EFCore.sql`:

| Tabla | Cambio | Motivo |
|---|---|---|
| `Categoria` | + `Descripcion varchar(150) NULL` | El mockup de Categorías muestra la descripción. |
| `Producto` | + `Activo bit NOT NULL DEFAULT 1` | RF “desactivar productos”. |
| `Lote` | + `CantidadInicial int NOT NULL` | Guardar lo recibido aunque `Cantidad` baje con las ventas. |
| `Venta` | + `MontoRecibido`, `Vuelto decimal(10,2)` | Mostrar efectivo y vuelto en el comprobante. |
| `DetalleVenta` | + `CostoUnitario decimal(10,2)` | Congelar el precio de compra para el reporte de rentabilidad. |
| `Comprobante` | + `DocumentoCliente varchar(11) NULL`; `UNIQUE(Serie, Numero)` | DNI/RUC del cliente; evitar números repetidos. |
| **`Parametro`** (nueva) | `RazonSocial`, `Ruc`, `Direccion`, `PorcentajeIgv`, `DiasAlertaVencimiento` | RF Parámetros generales. |
| Todas | `ModifiedBy` / `ModifiedAt` admiten NULL | Un registro recién creado aún no tiene modificación. |

## 7. Pruebas realizadas

Se implementaron **23 pruebas automáticas** (xUnit) que se ejecutan con `dotnet test`; todas pasan:

- **Seguridad:** login correcto/incorrecto, usuario desactivado, permisos por rol, username duplicado, hash de contraseña.
- **Auditoría y catálogo:** llenado de `CreatedBy/ModifiedBy`, códigos únicos, validación de precios y RUC, parámetros.
- **Inventario:** stock por lotes, validación de fechas, alertas de vencimiento y de stock mínimo.
- **Ventas:** cálculo de IGV, restricción de stock, descuento FEFO, correlativo de boletas/facturas,
  venta cancelada sin modificar stock, comprobante.
- **Reportes:** ventas por fecha/cajero, rentabilidad, productos críticos y Top 5.

Además se verificó que todas las consultas de reportes se traducen correctamente a T-SQL de SQL Server
y que la migración genera el script completo de creación de la base de datos.
