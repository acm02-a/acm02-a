using System.Security.Cryptography;

namespace ControlMarket.Datos;

/// <summary>Hash de contraseñas con PBKDF2 (SHA-256 + sal aleatoria). Formato: iteraciones.sal.hash</summary>
public static class PasswordHasher
{
    private const int Iteraciones = 100_000;

    public static string Hash(string password)
    {
        var sal = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, sal, Iteraciones, HashAlgorithmName.SHA256, 32);
        return $"{Iteraciones}.{Convert.ToBase64String(sal)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verificar(string password, string almacenado)
    {
        var partes = almacenado.Split('.');
        if (partes.Length != 3 || !int.TryParse(partes[0], out var iter)) return false;
        var sal = Convert.FromBase64String(partes[1]);
        var esperado = Convert.FromBase64String(partes[2]);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, sal, iter, HashAlgorithmName.SHA256, esperado.Length);
        return CryptographicOperations.FixedTimeEquals(hash, esperado);
    }
}
