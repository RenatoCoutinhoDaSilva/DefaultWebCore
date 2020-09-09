using System.Security.Claims;
using System.Security.Principal;

namespace Gestor.Extensions
{
    public static class IdentityExtensions
    {
        public static long Id(this IIdentity identity) {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Sid);

            if (claim == null)
                return 0;

            return long.Parse(claim.Value);
        }

        public static string Nome(this IIdentity identity) {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst("Nome");

            if (claim == null)
                return null;

            return claim.Value;
        }

        public static string Foto(this IIdentity identity) {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst("Foto");

            if (claim == null)
                return "";

            return $"{Entity.Models.Usuario.CaminhoArquivo}/{claim.Value}";
        }
    }
}
