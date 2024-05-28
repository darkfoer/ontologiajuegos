using System.IO;
using System.Text.RegularExpressions;

namespace MVCJuegos.Utilities
{
    public static class FormatnameUtilities
    {
        public static string FormatearNombreArchivo(string nombre)
        {
            // Lista de caracteres no permitidos en los nombres de archivo de Windows
            char[] caracteresNoPermitidos = Path.GetInvalidFileNameChars();

            // Eliminar caracteres no permitidos
            foreach (char caracter in caracteresNoPermitidos)
            {
                nombre = nombre.Replace(caracter.ToString(), string.Empty);
            }

            // Eliminar cualquier secuencia de tipo HTML especial como "&#xBF;" o "&#xED;"
            nombre = Regex.Replace(nombre, @"&[#\w]+;", string.Empty);

            // Reemplazar múltiples espacios en blanco con uno solo
            nombre = Regex.Replace(nombre, @"\s+", " ");

            // Eliminar espacios adicionales al inicio y al final
            nombre = nombre.Trim();

            return nombre;
        }
    }
}
