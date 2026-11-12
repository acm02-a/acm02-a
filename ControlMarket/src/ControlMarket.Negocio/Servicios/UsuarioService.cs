using ControlMarket.Datos;
using ControlMarket.Entidades;
using ControlMarket.Negocio.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace ControlMarket.Negocio.Servicios;

public class UsuarioService : ServicioBase
{
    public UsuarioService(IContextFactory factory, Sesion sesion) : base(factory, sesion) { }

    /// <summary>RF Autenticación por roles.</summary>
    public Perfil IniciarSesion(string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            throw new NegocioException("Ingrese usuario y contraseña.");

        using var db = Factory.Crear();
        var usuario = db.Usuarios.AsNoTracking().Include(u => u.Rol)
            .FirstOrDefault(u => u.Username == username.Trim());

        if (usuario == null || !PasswordHasher.Verificar(password, usuario.PasswordHash))
            throw new NegocioException("Usuario o contraseña incorrectos.");
        if (!usuario.Activo)
            throw new NegocioException("El usuario está desactivado. Contacte al administrador.");

        Sesion.Iniciar(usuario);
        return Sesion.Perfil!;
    }

    public void CerrarSesion() => Sesion.Cerrar();

    public List<Rol> ListarRoles()
    {
        using var db = Factory.Crear();
        return db.Roles.AsNoTracking().OrderBy(r => r.IdRol).ToList();
    }

    public List<Usuario> Listar(string? filtro = null)
    {
        Sesion.Exigir(Modulo.Usuarios);
        using var db = Factory.Crear();
        var q = db.Usuarios.AsNoTracking().Include(u => u.Rol).AsQueryable();
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var f = filtro.Trim();
            q = q.Where(u => u.Nombre.Contains(f) || u.Username.Contains(f));
        }
        return q.OrderBy(u => u.IdUsuario).ToList();
    }

    public List<Usuario> ListarPorRol(int idRol)
    {
        using var db = Factory.Crear();
        return db.Usuarios.AsNoTracking().Where(u => u.IdRol == idRol).OrderBy(u => u.Nombre).ToList();
    }

    /// <summary>
    /// RF Gestión de usuarios: alta (id = 0) o edición. Si la contraseña viene vacía
    /// al editar, se conserva la anterior.
    /// </summary>
    public Usuario Guardar(int idUsuario, string? nombre, string? username, string? password, int idRol, bool activo)
    {
        Sesion.Exigir(Modulo.Usuarios);
        var nom = Requerido(nombre, "Nombre", 100);
        var user = Requerido(username, "Usuario", 50);

        using var db = Factory.Crear();
        if (!db.Roles.Any(r => r.IdRol == idRol))
            throw new NegocioException("Seleccione un rol válido.");
        if (db.Usuarios.Any(u => u.Username == user && u.IdUsuario != idUsuario))
            throw new NegocioException($"El nombre de usuario '{user}' ya existe.");

        Usuario entidad;
        if (idUsuario == 0)
        {
            ValidarPassword(password);
            entidad = new Usuario { PasswordHash = PasswordHasher.Hash(password!) };
            db.Usuarios.Add(entidad);
        }
        else
        {
            entidad = db.Usuarios.Find(idUsuario) ?? throw new NegocioException("El usuario no existe.");
            if (!string.IsNullOrEmpty(password))
            {
                ValidarPassword(password);
                entidad.PasswordHash = PasswordHasher.Hash(password);
            }
            if (Sesion.Usuario?.IdUsuario == idUsuario && (!activo || idRol != Rol.Administrador))
                throw new NegocioException("No puede desactivarse ni quitarse el rol de administrador a sí mismo.");
        }

        entidad.Nombre = nom;
        entidad.Username = user;
        entidad.IdRol = idRol;
        entidad.Activo = activo;
        db.SaveChanges();
        return entidad;
    }

    private static void ValidarPassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6)
            throw new NegocioException("La contraseña debe tener al menos 6 caracteres.");
    }
}
