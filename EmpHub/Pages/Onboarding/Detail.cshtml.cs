using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Onboarding
{
    public class DetailModel : PageModel
    {
        public string userId { get; set; }
        public void OnGet(string userId)
        {
            this.userId = userId;
        }
    }
}
