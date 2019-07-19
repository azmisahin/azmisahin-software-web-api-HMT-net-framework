using System;

namespace HMT.Web.Api.Tracking.Areas.HelpPage.ModelDescriptions {
    /// <summary>
    /// Describes a type model.
    /// </summary>
    public abstract class ModelDescription {
        /// <summary>
        /// Documentation
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// Model Type
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
    }
}