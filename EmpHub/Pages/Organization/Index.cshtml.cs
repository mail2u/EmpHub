using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Organization
{
    public class IndexModel : PageModel
    {
        public string id { get; set; }
        public void OnGet(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                this.id = "CEO";
            }
            else { this.id = id; }
        }
    }
}
