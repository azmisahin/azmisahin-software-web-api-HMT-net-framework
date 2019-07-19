namespace HMT.Web.Api.Tracking.Models {
    using System.Data.Entity;

    /// <summary>
    /// Data Context
    /// </summary>
    public class DataContext : DbContext {

        /// <summary>
        /// Data Context
        /// </summary>
        public DataContext()
            : base("name=DataContext") {
        }

        /// <summary>
        /// Print Jobs
        /// </summary>
        public virtual DbSet<PrinterJob> PrintJobs { get; set; }
    }
}