using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Service.FormCreate
{
    public class FormPdpaEmployeeModel : PageModel
    {
        public string mode { get; set; }
        public void OnGet(string mode)
        {
            this.mode = mode;
        }
    }
}
