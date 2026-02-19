using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Employee
{
    public class DetailModel : PageModel
    {
        public string id { get; set; }
        public string tab { get; set; }
        public void OnGet(string id, string tab)
        {
            this.id = id;
            this.tab = tab;
        }
    }
}
