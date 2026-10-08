using System;
using System.Security.Cryptography;

namespace negocio
{
    // Hash de contrasenas con PBKDF2 + sal aleatoria.
    // Formato guardado en la base: pbkdf2$iteraciones$sal$hash   (cabe de sobra en NVARCHAR(200))
    internal static class PasswordHelper
    {
        private const int Iteraciones = 100000;
        private const int LargoSal = 16;
        private const int LargoHash = 32;

        public static string Hash(string password)
        {
            byte[] sal = new byte[LargoSal];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(sal);
            }
            byte[] hash = Derivar(password, sal, Iteraciones);
            return string.Format("pbkdf2${0}${1}${2}", Iteraciones, Convert.ToBase64String(sal), Convert.ToBase64String(hash));
        }

        public static bool Verificar(string password, string guardado)
        {
            if (string.IsNullOrEmpty(guardado))
                return false;

            string[] partes = guardado.Split('$');
            int iteraciones;
            if (partes.Length != 4 || partes[0] != "pbkdf2" || !int.TryParse(partes[1], out iteraciones))
                return false;

            try
            {
                byte[] sal = Convert.FromBase64String(partes[2]);
                byte[] esperado = Convert.FromBase64String(partes[3]);
                byte[] calculado = Derivar(password, sal, iteraciones);
                return SonIguales(esperado, calculado);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static byte[] Derivar(string password, byte[] sal, int iteraciones)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, sal, iteraciones, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(LargoHash);
            }
        }

        // Compara sin cortar en el primer byte distinto (evita ataques por tiempo de respuesta)
        private static bool SonIguales(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int diferencia = 0;
            for (int i = 0; i < a.Length; i++)
                diferencia |= a[i] ^ b[i];
            return diferencia == 0;
        }
    }
}
