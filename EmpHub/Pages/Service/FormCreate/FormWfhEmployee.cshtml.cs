using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Service.FormCreate
{
    public class FormWfhEmployeeModel : PageModel
    {
        public string mode { get; set; }
        public string id { get; set; }
        public void OnGet(string mode, string id)
        {
            this.mode = mode;
            this.id = id;
        }
    }
}
