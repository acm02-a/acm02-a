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
│   └── ControlMarket.Pruebas       → Pruebas automáticas xUnit (23 pruebas)
├── database/
│   └── ControlMarket_EFCore.sql    → Script SQL generado desde el modelo de EF Core
└── docs/
    └── Informe_Integrante3.md      → Texto para el informe del trabajo
```

Dependencias entre capas (cada capa solo conoce a la de abajo):

```
Presentacion ──► Negocio ──► Datos ──► Entidades
                    └──────────────────► Entidades
```

## Cómo ejecutar (Windows)

1. Instalar **Visual Studio 2022** (carga de trabajo “Desarrollo de escritorio de .NET”) y **SQL Server** (Express o Developer).
2. Abrir `ControlMarket.sln`.
3. Revisar la cadena de conexión en `src/ControlMarket.Presentacion/appsettings.json`:
   ```json
   "ControlMarket": "Server=localhost;Database=ControlMarketDB;Trusted_Connection=True;TrustServerCertificate=True;"
   ```
   - SQL Server Express: `Server=localhost\\SQLEXPRESS;...`
   - LocalDB: `Server=(localdb)\\MSSQLLocalDB;...`
4. Establecer **ControlMarket.Presentacion** como proyecto de inicio y presionar **F5**.
   - Si la base de datos no existe, se crea sola (migraciones de EF Core) y se cargan los datos
     mínimos: roles, métodos de pago, parámetros y el usuario administrador.
5. Iniciar sesión con **admin / Admin123\*** (cambiar la contraseña desde *Gestión de usuarios*).

### Si la BD ya fue creada con el script del Integrante 2

Poner `"AplicarMigraciones": false` en `appsettings.json`. La aplicación usará la BD existente
y solo insertará los datos mínimos que falten. Para eso, el script debe respetar los nombres
de tablas/columnas del modelo (ver `database/ControlMarket_EFCore.sql` como referencia).

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
