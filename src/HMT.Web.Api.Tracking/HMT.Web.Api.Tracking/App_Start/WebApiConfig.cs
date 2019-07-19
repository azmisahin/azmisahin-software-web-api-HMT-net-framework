namespace HMT.Web.Api.Tracking {
    using System.Web.Http;

    /// <summary>
    /// Web Api Config
    /// </summary>
    public static class WebApiConfig {
        /// <summary>
        /// Register
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config) {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
