using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmpHub.Extension
{
    public static class HtmlExtensions
    {
        public static string IsSelected(this IHtmlHelper html, string folder = null)
        {
            if (html == null) { return string.Empty; }

            string currentPage = (string)html.ViewContext.RouteData.Values["page"];

            return currentPage.Equals(folder) ? "active" : currentPage.StartsWith(folder + "/") ? "active" : String.Empty;
        }
    }
}
