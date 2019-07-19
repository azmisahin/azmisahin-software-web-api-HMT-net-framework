namespace HMT.Web.Api.Tracking.Controllers.PrinterJob {
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using HMT.Web.Api.Tracking.Models;

    /// <summary>
    /// Tracking
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/PrinterJob/Tracking")]
    public class TrackingController : ApiController {
        /// <summary>
        /// Get All Tracking
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public List<PrinterJobEvent> Get() {
            return new List<PrinterJobEvent>() { };
        }

        /// <summary>
        /// Get Tracking By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("")]
        public PrinterJobEvent Get(int id) {
            return new PrinterJobEvent { };
        }

        /// <summary>
        /// Tracking
        /// </summary>
        /// <param name="value"></param>
        [Route("")]
        public void Post(PrinterJobEvent value) {
        }
    }
}