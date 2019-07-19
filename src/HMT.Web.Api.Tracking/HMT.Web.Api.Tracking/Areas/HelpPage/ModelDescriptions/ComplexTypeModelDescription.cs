using System.Collections.ObjectModel;

namespace HMT.Web.Api.Tracking.Areas.HelpPage.ModelDescriptions {
    /// <summary>
    /// 
    /// </summary>
    public class ComplexTypeModelDescription : ModelDescription {
        /// <summary>
        /// 
        /// </summary>
        public ComplexTypeModelDescription() {
            Properties = new Collection<ParameterDescription>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Collection<ParameterDescription> Properties { get; private set; }
    }
}