using SqlRazorGenerator;
using SqlTableParser.Models;
using Serilog;

namespace SqlTableToEntityApp.Generators
{
    internal class BaseGenerator<T> where T : class, new()
    {
        protected RazorGenerator<T> entityGenerator;

        protected static bool DiagnosticMode { get; set; }

        protected static ILogger Logger => Log.Logger;

        protected string outputFilePath { get; set; }

        protected IEnumerable<Table> tables;

        public BaseGenerator(string templateFilePath, IEnumerable<Table> tables, string outputFilePath)
        {
            this.tables = tables;
            this.outputFilePath = outputFilePath;
            this.entityGenerator = new RazorGenerator<T>(templateFilePath);
        }

        protected string GetOutputFilePath(Table table)
        {
            return GeneratorUtility.GetOutputFilePath(table, this.outputFilePath);
        }
    }
}
