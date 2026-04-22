using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Onboarding
{
    public class UpdateModel : PageModel
    {
        public string userId { get; set; }
        public void OnGet(string userId)
        {
            this.userId = userId;
        }
    }
}
