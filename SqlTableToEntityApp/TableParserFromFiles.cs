using SqlTableParser.Models;
using SqlTableParser;
using Serilog;
using System.Collections;

namespace SqlTableToEntityApp
{
    internal class TableParserFromFiles : IEnumerable<Table>
    {
        protected static bool DiagnosticMode { get; set; }

        protected static ILogger Logger => Log.Logger;

        private string[] paths { get; set; }

        public TableParserFromFiles(string[] paths)
        {
            this.paths = paths;
        }

        /// <summary>
        /// Parse files.
        /// </summary>
        /// <returns>Parsed tables.</returns>
        public IEnumerable<Table> ParseFiles()
        {
            foreach (var path in this.paths)
            {
                if (Directory.Exists(path))
                {
                    var files = Directory.EnumerateFiles(path, "*.sql", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var filePath = Path.Combine(path, file);

                        foreach (var table in ParseFile(filePath))
                        {
                            yield return table;
                        }
                    }
                }
                else if (File.Exists(path))
                {
                    foreach (var table in ParseFile(path))
                    {
                        yield return table;
                    }
                }
                else
                {
                    Logger.Error($"Missing directory {path}");
                }
            }
        }

        private IEnumerable<Table> ParseFile(string filePath)
        {
            if (DiagnosticMode)
            {
                Logger.Debug(string.Empty);
                Logger.Debug("**************************");
                Logger.Debug($"Parsing {filePath}");
                Logger.Debug("**************************");
            }

            var parser = new TableParser(filePath);
            foreach (var table in parser.ParseSqlForTable())
            {
                if (!DiagnosticMode)
                {
                    Logger.Debug(string.Empty);
                    Logger.Debug("**************************");
                    Logger.Debug($"Parsing {filePath}");
                    Logger.Debug("**************************");
                }

                Logger.Debug(table.DiagnosticString);
                foreach (var column in table.Columns)
                {
                    Logger.Debug($"{column.DiagnosticString,-60} {column.Sql}");
                }

                yield return table;
            }
        }

        public IEnumerator<Table> GetEnumerator() {
            return this.ParseFiles().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.ParseFiles().GetEnumerator();
        }
    }
}
