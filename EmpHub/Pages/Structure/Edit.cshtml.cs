using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Structure
{
    public class EditModel : PageModel
    {
        public string id { get; set; }
        public void OnGet(string id)
        {
            this.id = id;
        }
    }
}
