using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Service
{
    public class CreateModel : PageModel
    {
        string cateCode { get; set; }
        public void OnGet(string cateCode)
        {
            this.cateCode = cateCode;
        }
    }
}
