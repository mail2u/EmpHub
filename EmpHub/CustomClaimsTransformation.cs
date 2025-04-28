using EmpHub.Models;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using EmpHub.Extension;
using Duende.IdentityModel;

namespace EmpHub
{
    public class CustomClaimsTransformation : IClaimsTransformation
    {
        private readonly IConfiguration _configuration;

        public CustomClaimsTransformation(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {

            var identity = (ClaimsIdentity)principal.Identity;
            string username = identity.FindFirst("profile").Value;

            if (!String.IsNullOrEmpty(username))
            {
                WebAPIModels webAPI = new WebAPIModels();
                this._configuration.GetSection("WebAPI").Bind(webAPI);

                using (var httpClient = new HttpClient())
                {
                    var dataRequest = new
                    {
                        username = username
                    };

                    StringContent content = new StringContent(JsonConvert.SerializeObject(dataRequest), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(webAPI.APIEmpHub + "/api/User/Login", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var dataResponse = JsonConvert.DeserializeObject<UserModels>(apiResponse);

                            #region Sub
                            var existingClaimSub = identity.FindFirst(JwtClaimTypes.Subject);
                            if (existingClaimSub != null)
                            {
                                identity.RemoveClaim(existingClaimSub); // ลบ Claim เดิม
                            }

                            identity.AddClaim(new Claim(JwtClaimTypes.Subject, dataResponse.userId));
                            #endregion
                            #region Name
                            var existingClaimName = identity.FindFirst(JwtClaimTypes.Name);
                            if (existingClaimName != null)
                            {
                                identity.RemoveClaim(existingClaimName); // ลบ Claim เดิม
                            }

                            identity.AddClaim(new Claim(JwtClaimTypes.Name, $"{dataResponse.firstname_th} {dataResponse.lastname_th}"));
                            #endregion
                            #region Role
                            var existingClaimRole = identity.FindFirst(JwtClaimTypes.Role);
                            if (existingClaimRole != null)
                            {
                                identity.RemoveClaim(existingClaimRole); // ลบ Claim เดิม
                            }

                            identity.AddClaim(new Claim(JwtClaimTypes.Role, dataResponse.role));
                            #endregion
                        }
                    }
                }

            }

            return await Task.FromResult(principal);
        }
    }
}
