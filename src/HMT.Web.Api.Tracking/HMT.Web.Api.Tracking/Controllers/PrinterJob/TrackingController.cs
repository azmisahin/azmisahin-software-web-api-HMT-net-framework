namespace HMT.Web.Api.Tracking.Controllers.PrinterJob {
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using HMT.Web.Api.Tracking.Models;
    using System.Linq;

    /// <summary>
    /// Tracking
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/PrinterJob/Tracking")]
    public class TrackingController : ApiController {

        /// <summary>
        /// Data Context
        /// </summary>
        private DataContext db;

        /// <summary>
        /// Initialize
        /// </summary>
        public TrackingController() {

            // Set Data
            db = new DataContext();
        }

        /// <summary>
        /// Get All Tracking
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public IEnumerable<PrinterJob> Get() => (from item in db.PrintJobs
                                                 orderby item.ID descending
                                                 select item).Take(100);

        /// <summary>
        /// Get Tracking By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("")]
        public PrinterJob Get(int id) => (from item in db.PrintJobs
                                          where item.ID == id
                                          select item).FirstOrDefault();

        /// <summary>
        /// Tracking
        /// </summary>
        /// <param name="value"></param>
        [Route("")]
        public void Post(PrinterJob value) {

            // Add Item
            _ = db
                .PrintJobs
                .Add(value);

            // Save Item
            _ = db.SaveChangesAsync();
        }
    }
}