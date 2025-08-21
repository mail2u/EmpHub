using EmpHub.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace EmpHub.Extension
{
    public static class UserExtensions
    {
        public static IConfiguration _config;
        public static void Initialize(IConfiguration Configuration)
        {
            _config = Configuration;
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
            return GetClaimsValue(principal, ClaimTypes.NameIdentifier);
        }

        public static string UserName(this IPrincipal principal)
        {
            return GetClaimsValue(principal, "name");
        }

        public static string Department(this IPrincipal principal)
        {
            return GetClaimsValue(principal, "department");
        }

        public static string AccessToken(this IPrincipal principal, HttpContext context)
        {
            var identity = (ClaimsIdentity)principal.Identity;
            var accessTokenClaim = context.Session.GetString("access_token");
            var expireTokenClaim = context.Session.GetString("expire_token");

            if (accessTokenClaim == null || expireTokenClaim == null
                || String.IsNullOrEmpty(accessTokenClaim)
                || String.IsNullOrEmpty(expireTokenClaim)
                || DateTime.Now >= DateTime.Parse(expireTokenClaim))
            {
                return RefreshToken((ClaimsPrincipal)principal, context);
            }

            return accessTokenClaim;
        }

        private static string RefreshToken(ClaimsPrincipal principal, HttpContext context)
        {
            var userId = principal.UserId();
            var authen = principal.Authen();

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

                // เก็บลง Session
                context.Session.SetString("access_token", accessToken);
                context.Session.SetString("expire_token", DateTime.UtcNow.AddMinutes(58).ToString("o"));

                return accessToken;
            }

            return null;
        }
    }
}
