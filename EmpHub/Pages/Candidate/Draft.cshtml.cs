using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Candidate
{
    public class DraftModel : PageModel
    {
        public string id { get; set; }
        public void OnGet(string id)
        {
            this.id = id;
        }
    }
}
