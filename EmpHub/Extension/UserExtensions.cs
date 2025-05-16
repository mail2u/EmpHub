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

        public static string Role(this IPrincipal principal)
        {
            return GetClaimsValue(principal, "role");
        }


        public static string AccessToken(this IPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity;
            var accessTokenClaim = identity.FindFirst("access_token");
            var expireTokenClaim = identity.FindFirst("expire_token");

            if (accessTokenClaim == null || expireTokenClaim == null
                || String.IsNullOrEmpty(accessTokenClaim.Value)
                || String.IsNullOrEmpty(expireTokenClaim.Value)
                || DateTime.UtcNow >= DateTime.Parse(expireTokenClaim.Value))
            {
                var userId = principal.UserId();
                var authen = principal.Authen();

                return RefreshToken(identity, userId, authen);
            }

            return accessTokenClaim.Value;
        }

        private static string RefreshToken(ClaimsIdentity identity, string userId, string authen)
        {
            if (!String.IsNullOrEmpty(userId) && !String.IsNullOrEmpty(authen))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Authentication, authen),
                };
                var jwtOptions = _config.GetSection("JwtOptions").Get<JwtOptions>();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var Sectoken = new JwtSecurityToken(jwtOptions.Issuer,
                  jwtOptions.Issuer,
                  claims,
                  expires: DateTime.Now.AddMinutes(60),
                  signingCredentials: credentials);

                var accessToken = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                #region AccessToken
                var accessTokenClaim = identity.FindFirst("access_token");
                if (accessTokenClaim != null)
                {
                    identity.RemoveClaim(accessTokenClaim);
                }

                identity.AddClaim(new Claim("access_token", accessToken));
                #endregion
                #region ExpireToken
                var expireTokenClaim = identity.FindFirst("expire_token");
                if (expireTokenClaim != null)
                {
                    identity.RemoveClaim(accessTokenClaim);
                }

                identity.AddClaim(new Claim("expire_token", DateTime.UtcNow.AddMinutes(58).ToString("o")));
                #endregion

                return accessToken;
            }

            return null;
        }
    }
}
