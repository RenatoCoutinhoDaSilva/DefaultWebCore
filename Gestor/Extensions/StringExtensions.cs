using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gestor.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmail(this string email) {
            return Regex.IsMatch(email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }

        public static int[] ToIntArray(this string value) {
            if (value != null)
                return new int[0];

            var result = value.Split(",");
            return result.Select(i => int.Parse(i)).ToArray();
        }

        public static long[] ToLongArray(this string value) {
            if (value != null)
                return new long[0];

            var result = value.Split(",");
            return result.Select(i => long.Parse(i)).ToArray();
        }

        /// <summary>
        /// Converte uma string de data no padrão brasileiro para um valor DateTime.
        /// </summary>
        /// <param name="value">Ex.: 12/05/2020 (dd/MM/yyyy)</param>
        public static DateTime ToDate(this string value) {
            if (value == null)
                return DateTime.MinValue;

            var result = new string[0];

            if (value.Contains("-"))
                result = value.Split("-");
            else
                result = value.Split("/");

            if (result[0].Length == 4)
                return new DateTime(int.Parse(result[0]), int.Parse(result[1]), int.Parse(result[2]));
            else 
                return new DateTime(int.Parse(result[2]), int.Parse(result[1]), int.Parse(result[0]));
        }
    }
}
