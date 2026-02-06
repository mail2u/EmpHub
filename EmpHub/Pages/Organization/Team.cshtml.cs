using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Organization
{
    public class TeamModel : PageModel
    {
        public string code { get; set; }
        public void OnGet(string code)
        {
            this.code = code;
        }
    }
}
