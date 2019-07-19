using System.Collections.ObjectModel;

namespace HMT.Web.Api.Tracking.Areas.HelpPage.ModelDescriptions {
    /// <summary>
    /// 
    /// </summary>
    public class EnumTypeModelDescription : ModelDescription {

        /// <summary>
        /// 
        /// </summary>
        public EnumTypeModelDescription() {
            Values = new Collection<EnumValueDescription>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Collection<EnumValueDescription> Values { get; private set; }
    }
}