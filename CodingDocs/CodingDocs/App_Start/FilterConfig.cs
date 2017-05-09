using System.Web;
using System.Web.Mvc;
using CodingDocs.Utilities;

namespace CodingDocs
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
        }
    }
}
