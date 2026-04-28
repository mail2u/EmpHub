using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Service
{
    public class FormUpdateDataDocumentModel : PageModel
    {
        public string id { get; set; }
        public string mode { get; set; }
        public void OnGet(string mode, string id)
        {
            this.mode = mode;
            this.id = id;
        }
    }
}
