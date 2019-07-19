using System.Web.Http;
using System.Web.Mvc;
using HMT.Web.Api.Tracking.Areas.HelpPage.ModelDescriptions;
using HMT.Web.Api.Tracking.Areas.HelpPage.Models;

namespace HMT.Web.Api.Tracking.Areas.HelpPage.Controllers {
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller {
        /// <summary>
        /// 
        /// </summary>
        private const string ErrorViewName = "Error";

        /// <summary>
        /// 
        /// </summary>
        public HelpController()
            : this(GlobalConfiguration.Configuration) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public HelpController(HttpConfiguration config) {
            Configuration = config;
        }

        /// <summary>
        /// 
        /// </summary>
        public HttpConfiguration Configuration { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index() {
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiId"></param>
        /// <returns></returns>
        public ActionResult Api(string apiId) {
            if (!string.IsNullOrEmpty(apiId)) {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null) {
                    return View(apiModel);
                }
            }

            return View(ErrorViewName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public ActionResult ResourceModel(string modelName) {
            if (!string.IsNullOrEmpty(modelName)) {
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out ModelDescription modelDescription)) {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }
    }
}