using RazorEngineCore;
using System.Collections.Generic;

namespace SqlRazorGenerator
{
    /// <summary>
    /// Razor template model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TemplateModel<T> : RazorEngineTemplateBase
    {
        /// <summary>
        /// Database name.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Environment variables.
        /// </summary>
        public Dictionary<string, string> EnvironmentVariables { get; set; }
    }
}
