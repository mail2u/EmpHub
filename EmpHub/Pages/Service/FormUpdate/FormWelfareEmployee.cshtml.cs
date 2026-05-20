using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Service.FormUpdate
{
    public class FormWelfareEmployeeModel : PageModel
    {
        public string refId { get; set; }
        public void OnGet(string refId)
        {
            this.refId = refId;
        }
    }
}
