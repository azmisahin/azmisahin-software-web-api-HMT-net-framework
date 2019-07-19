namespace HMT.Web.Api.Tracking {
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Route Config
    /// </summary>
    public class RouteConfig {
        /// <summary>
        /// Register Routers
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
