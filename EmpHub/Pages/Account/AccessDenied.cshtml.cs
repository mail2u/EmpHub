using EmpHub.Extension;
using EmpHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace EmpHub.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}
