using RazorEngineCore;
using System.Collections.Generic;

namespace SqlRazorGenerator
{
    public class TemplateModel<T> : RazorEngineTemplateBase
    {
        /// <summary>
        /// Environment variables.
        /// </summary>
        public Dictionary<string, string> EnvironmentVariables { get; set; }
    }
}
