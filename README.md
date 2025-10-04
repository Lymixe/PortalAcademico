# Portal Académico - Gestión de Cursos y Matrículas

Proyecto de examen parcial desarrollado con ASP.NET Core 8, Entity Framework Core y Redis.

**URL de la aplicación desplegada:**
*Añadir la URL aquí cuando el despliegue esté completo*

---

## Cómo ejecutar el proyecto localmente

1.  Clona el repositorio.
2.  Asegúrate de tener instalado el SDK de .NET 8.
3.  Configura las variables en `appsettings.Development.json` o mediante secretos de usuario.
4.  Ejecuta las migraciones de la base de datos: `dotnet ef database update`
5.  Ejecuta la aplicación: `dotnet run`

---

## Credenciales de prueba

-   **Rol Coordinador:**
    -   **Usuario:** `coordinador@test.com`
    -   **Contraseña:** `Coordinador123!`

-   **Rol Estudiante:**
    -   **Usuario:** `emy3@gmail.com`
    -   **Contraseña:** `Andrew2.`
---

## Variables de Entorno Requeridas

Estas son las variables necesarias para que la aplicación funcione en producción.

-   `ASPNETCORE_ENVIRONMENT`: `Production`
-   `ASPNETCORE_URLS`: `http://0.0.0.0:${PORT}`
-   `ConnectionStrings__DefaultConnection`: `DataSource=/var/data/app.db;Cache=Shared` (Ruta para el disco persistente de Render)
-   `Redis__ConnectionString`: La cadena de conexión de tu instancia de Redis Cloud.