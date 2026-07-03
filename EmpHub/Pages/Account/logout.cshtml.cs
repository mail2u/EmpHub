using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Account
{
    public class logoutModel : PageModel
    {
        public async Task OnGetAsync()
        {
            HttpContext.Session.Remove("access_token");
            HttpContext.Session.Remove("expire_token");
            HttpContext.Session.Remove("access_token_user_id");
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
