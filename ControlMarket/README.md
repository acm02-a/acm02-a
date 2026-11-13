# ControlMarket – Proyecto C# en capas (Integrante 3)

Sistema de control de inventario y ventas para supermercados.
**C# · .NET 8 · Windows Forms · Entity Framework Core 8 · SQL Server**

## Estructura de la solución

```
ControlMarket.sln
├── src/
│   ├── ControlMarket.Entidades     → Capa de entidades (modelos POCO + DTOs de reportes)
│   ├── ControlMarket.Datos         → Capa de acceso a datos (DbContext, Fluent API, migraciones, auditoría)
│   ├── ControlMarket.Negocio       → Capa de lógica de negocio (servicios, reglas, sesión y perfiles)
│   └── ControlMarket.Presentacion  → Capa de presentación (formularios Windows Forms)
├── tests/
│   └── ControlMarket.Pruebas       → Pruebas automáticas xUnit (26 pruebas)
├── database/
│   ├── 01_db_ControlMarket_integrante2.sql → Script de la BD (Integrante 2)
│   ├── 02_ajustes_integrante3.sql          → Ajustes que necesita la aplicación (ejecutar después del 01)
│   └── ControlMarket_EFCore.sql            → Script equivalente generado por EF Core (solo referencia)
└── docs/
    └── Informe_Integrante3.md      → Texto para el informe del trabajo
```

Dependencias entre capas (cada capa solo conoce a la de abajo):

```
Presentacion ──► Negocio ──► Datos ──► Entidades
                    └──────────────────► Entidades
```

## Cómo ejecutar (Windows)

1. Instalar **Visual Studio 2022** (carga de trabajo “Desarrollo de escritorio de .NET”) y **SQL Server**.
2. En SQL Server Management Studio ejecutar, en este orden:
   1. `database/01_db_ControlMarket_integrante2.sql` → crea `db_ControlMarket` (Integrante 2).
      Si la carpeta de datos de su PC es distinta, cambiar las rutas `FILENAME` del `CREATE DATABASE`
      (o crear la BD vacía a mano y ejecutar el script desde `USE [db_ControlMarket]`).
   2. `database/02_ajustes_integrante3.sql` → agrega lo que necesita la aplicación.
3. Abrir `ControlMarket.sln` y revisar la cadena de conexión en
   `src/ControlMarket.Presentacion/appsettings.json`:
   ```json
   "ControlMarket": "Server=localhost\\SQL2026;Database=db_ControlMarket;Trusted_Connection=True;TrustServerCertificate=True;"
   ```
   `SQL2026` es el nombre de la instancia que usó el Integrante 2. En otra PC puede ser
   `localhost\\SQLEXPRESS`, `localhost` o `(localdb)\\MSSQLLocalDB`.
4. Establecer **ControlMarket.Presentacion** como proyecto de inicio y presionar **F5**.
   Al iniciar, la aplicación inserta lo mínimo que falte: roles, métodos de pago, parámetros y el usuario admin.
5. Iniciar sesión con **admin / Admin123\*** (cambiar la contraseña desde *Gestión de usuarios*).

> **Importante para los datos de prueba:** el código usa estos Id fijos:
> `Rol` 1 = Administrador, 2 = JefeAlmacen, 3 = Cajero · `MetodoPago` 1 = Efectivo.
> Si se insertan roles o métodos de pago a mano, respetar ese orden. Las contraseñas de `Usuario`
> deben crearse desde la aplicación (se guardan con hash PBKDF2, no en texto plano).

### Alternativa sin los scripts

Con `"AplicarMigraciones": true` y una BD que no exista, EF Core crea toda la base de datos
con la migración `Inicial` (mismo esquema que 01 + 02).

### Pruebas

```bash
dotnet test
```

## Comandos de Entity Framework Core

```bash
dotnet tool restore
# nueva migración después de cambiar el modelo
dotnet ef migrations add NombreCambio -p src/ControlMarket.Datos -s src/ControlMarket.Datos -o Migraciones
# regenerar el script SQL
dotnet ef migrations script -p src/ControlMarket.Datos -s src/ControlMarket.Datos -o database/ControlMarket_EFCore.sql --idempotent
```
