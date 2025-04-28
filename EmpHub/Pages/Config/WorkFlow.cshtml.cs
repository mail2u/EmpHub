using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmpHub.Pages.Config
{
    public class WorkFlowModel : PageModel
    {
        public string sysId { get; set; }
        public void OnGet(string sysId)
        {
            this.sysId = sysId;
        }
    }
}
