using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography.X509Certificates;

namespace EmpHub.Pages.Organization
{
    public class ManageModel : PageModel
    {
        public string id { get; set; }
        public string chartId { get; set; }
        public void OnGet(string id, string chartId)
        {
            this.chartId = chartId;
            if (String.IsNullOrEmpty(id))
            {
                this.id = "CEO";
            }
            else { this.id = id; }
        }
    }
}
