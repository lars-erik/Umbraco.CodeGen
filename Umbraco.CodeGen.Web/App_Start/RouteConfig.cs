using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Umbraco.CodeGen.Web.App_Start
{
    public class RouteConfig
    {
        public static void ApplyRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                "codegen",
                "CodeGen/{controller}/{action}/{id}",
                new { id = UrlParameter.Optional }
                );

            routes.MapRoute(
                "default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}