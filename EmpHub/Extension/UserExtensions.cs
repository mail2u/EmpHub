using System.Security.Claims;
using System.Security.Principal;

namespace EmpHub.Extension
{
    public static class UserExtensions
    {
        public static IConfiguration config;
        public static void Initialize(IConfiguration Configuration)
        {
            config = Configuration;
        }

        public static string GetClaimsValue(this IPrincipal principal, string claimType)
        {
            if (principal != null)
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                if (claimsIdentity.HasClaim(x => x.Type == claimType))
                {
                    var claim = claimsIdentity.FindFirst(claimType);
                    return claim.Value;
                }
            }
            return string.Empty;
        }

        public static string Authen(this IPrincipal principal)
        {
            return GetClaimsValue(principal, ClaimTypes.Authentication);
        }

        public static string UserId(this IPrincipal principal)
        {
            return GetClaimsValue(principal, "sub");
        }

        public static string ProfileId(this IPrincipal principal)
        {
            return GetClaimsValue(principal, "profile");
        }

        public static string UserName(this IPrincipal principal)
        {
            return GetClaimsValue(principal, "name");
        }

        public static string Email(this IPrincipal principal)
        {
            return GetClaimsValue(principal, "email");
        }

        public static string Department(this IPrincipal principal)
        {
            return GetClaimsValue(principal, "family_name");
        }

        public static string AccessToken(this IPrincipal principal)
        {
            return GetClaimsValue(principal, "access_token");
        }

        public static string Role(this IPrincipal principal)
        {
            return GetClaimsValue(principal, "role");
        }

    }
}
