using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Service
{
    public class DetailModel : PageModel
    {
        public string id { get; set; }
        public void OnGet(string id)
        {
            this.id = id;
        }
    }
}
