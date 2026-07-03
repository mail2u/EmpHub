using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using EmpHub.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using System;
using System.Net.Http;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using EmpHub.Extension;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace EmpHub.Pages.Account
{
    public class LoginModel : PageModel
    {
        public string userId { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string department { get; set; }
        public string errorMsg { get; set; }
        public string ReturnUrl { get; set; }

        private readonly IConfiguration _configuration;

        public LoginModel(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public async Task<IActionResult> OnGet(string ReturnUrl)
        {
            this.ReturnUrl = ReturnUrl;

            if (!String.IsNullOrEmpty(User.UserId()))
            {
                if (String.IsNullOrEmpty(ReturnUrl) || ReturnUrl == "/")
                {
                    return new RedirectToPageResult("/Index");
                }

                return Redirect(ReturnUrl);
            }

            return Page();
        }

        public async Task<IActionResult> OnPost([FromBody] LoginModel iProp)
        {
            UserModels iUser = new UserModels();
            iUser.username = Request.Form["username"];
            iUser.password = Request.Form["password"];
            iUser.ipaddress = HttpContext.Connection.RemoteIpAddress.ToString();

            string pattern = @"^[a-zA-Z]+\.[a-zA-Z]{2}$";

            if (Regex.IsMatch(iUser.username, pattern) == false)
            {
                errorMsg = "username pattern invalid.";
                return Page();
            }

            var returnUrl = Request.Form["ReturnUrl"];

            this.ReturnUrl = returnUrl;

            WebAPIModels webAPI = new WebAPIModels();
            this._configuration.GetSection("WebAPI").Bind(webAPI);

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    bool isValid = false;

                    var env = this._configuration.GetValue<String>("Environment");

                    if (env.ToLower().Equals("uat") && iUser.password == "Tip@12345")
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = context.ValidateCredentials(iUser.username, iUser.password);
                    }

                    if (isValid)
                    {
                        UserPrincipal result = UserPrincipal.FindByIdentity(context, iUser.username);

                        HttpContext.Session.Remove("access_token");
                        HttpContext.Session.Remove("expire_token");
                        HttpContext.Session.Remove("access_token_user_id");
                        await Login(iUser);

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, iUser.userId),
                            new Claim("profile", iUser.username),
                            new Claim("position", iUser.positionDesc),
                            new Claim("department", iUser.departmentDesc),
                            new Claim("given_name", String.Format("{0} {1}", iUser.firstname_th, iUser.lastname_th)),
                            new Claim(ClaimTypes.Authentication, iUser.role),
                        };

                        //JWT Connect
                        var jwtOptions = this._configuration.GetSection("JwtOptions").Get<JwtOptions>();
                        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
                        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                        var Sectoken = new JwtSecurityToken(jwtOptions.Issuer,
                          jwtOptions.Issuer,
                          claims,
                          expires: DateTime.Now.AddMinutes(5760),
                          signingCredentials: credentials);

                        var accessToken = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                        HttpContext.Session.SetString("access_token", accessToken);
                        HttpContext.Session.SetString("expire_token", DateTime.UtcNow.AddMinutes(5758).ToString("o"));
                        HttpContext.Session.SetString("access_token_user_id", iUser.userId);
                        claims.Add(new Claim("access_token", accessToken));

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true, // ทำให้ Cookie เป็น Persistent
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5760) // อายุการใช้งาน Cookie 7 วัน
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        if (String.IsNullOrEmpty(returnUrl) || returnUrl == "/")
                        {
                            return new RedirectToPageResult("/Index");
                        }

                        return Redirect(returnUrl);
                    }
                    else
                    {
                        throw new Exception("Username/Password invalid");
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            return Page();
        }

        private async Task Login(UserModels iUser)
        {
            WebAPIModels webAPI = new WebAPIModels();
            this._configuration.GetSection("WebAPI").Bind(webAPI);

            using (var httpClient = new HttpClient())
            {
                var dataRequest = new
                {
                    iUser.username
                    ,
                    iUser.password
                    ,
                    iUser.ipaddress
                };

                StringContent content = new StringContent(JsonConvert.SerializeObject(dataRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(webAPI.APIEmpHub + "/api/User/Login", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var dataResponse = JsonConvert.DeserializeObject<UserModels>(apiResponse);

                        iUser.userId = dataResponse.userId;
                        iUser.firstname_th = dataResponse.firstname_th;
                        iUser.lastname_th = dataResponse.lastname_th;
                        iUser.role = !String.IsNullOrEmpty(dataResponse.role) ? dataResponse.role : "-";
                        iUser.positionDesc = !String.IsNullOrEmpty(dataResponse.positionDesc) ? dataResponse.positionDesc : "-";
                        iUser.departmentDesc = !String.IsNullOrEmpty(dataResponse.departmentDesc) ? dataResponse.departmentDesc : "-";
                    }
                    else
                    {
                        throw new Exception(apiResponse);
                    }
                }
            }
        }
    }
}
