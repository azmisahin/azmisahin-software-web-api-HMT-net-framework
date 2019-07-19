namespace automation {

    using System;
    using System.Reflection;

    /// <summary>
    /// Application
    /// </summary>
    public class App {

        /// <summary>
        /// Version Number
        /// </summary>
        public string Version {
            get {
                // The version of the currenty executing assembly
                return $"{typeof(string).Assembly.GetName().Version}";
            }
        }
    }
}
