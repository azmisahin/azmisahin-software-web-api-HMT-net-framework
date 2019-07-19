namespace HMT.Web.Api.Tracking {
    using System.Web.Optimization;

    /// <summary>
    /// Bundle Config
    /// </summary>
    public class BundleConfig {
        /// <summary>
        /// Register Bundles
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/assets/lib/jquery/js/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/assets/lib/jquery/js/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/assets/lib/bootstrap/js/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/assets/lib/bootstrap/css/bootstrap.min.css",
                      "~/assets/site/css/shared.css"));
        }
    }
}
