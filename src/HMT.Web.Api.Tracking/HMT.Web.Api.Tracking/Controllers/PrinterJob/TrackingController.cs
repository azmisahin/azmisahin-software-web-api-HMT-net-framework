namespace HMT.Web.Api.Tracking.Controllers.PrinterJob {
    using System.Collections.Generic;
    using System.Web.Http;
    using HMT.Web.Api.Tracking.Models;

    /// <summary>
    /// Tracking
    /// </summary>
    public class TrackingController : ApiController {
        /// <summary>
        /// Get All Tracking
        /// </summary>
        /// <returns></returns>
        public List<PrinterJobEvent> Get() {
            return new List<PrinterJobEvent>() { };
        }

        /// <summary>
        /// Get Tracking By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Get(int id) {
            return "value";
        }

        /// <summary>
        /// Tracking
        /// </summary>
        /// <param name="value"></param>
        public void Post(PrinterJobEvent value) {
        }
    }
}