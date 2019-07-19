using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMT.Web.Api.Tracking.Models {

    /// <summary>
    /// Printer Job
    /// </summary>
    public class PrinterJob : PrinterJobEvent {

        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
    }
}