using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Service
{
    public class FormNewEmployeeModel : PageModel
    {
        public string mode { get; set; }
        public void OnGet(string mode)
        {
            this.mode = mode;
        }
    }
}
