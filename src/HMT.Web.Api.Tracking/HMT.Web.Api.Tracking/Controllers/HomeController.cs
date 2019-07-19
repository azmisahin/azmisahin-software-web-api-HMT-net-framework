namespace HMT.Web.Api.Tracking.Controllers {
    using System.Web.Mvc;

    /// <summary>
    /// Home
    /// </summary>
    public class HomeController : Controller {
        /// <summary>
        /// Home
        /// </summary>
        /// <returns></returns>
        public ActionResult Index() {
            ViewBag.Title = "Tracking";

            return View();
        }
    }
}
