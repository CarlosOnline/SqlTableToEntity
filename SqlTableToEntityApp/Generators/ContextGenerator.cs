using SqlTableParser.Models;

namespace SqlTableToEntityApp.Generators;

internal class ContextGenerator : BaseGenerator<List<Table>>
{
    public ContextGenerator(string templateFilePath, IEnumerable<Table> tables, string databaseName, string outputFilePath) :
        base(templateFilePath, tables, databaseName, outputFilePath)
    {
    }

    public void GenerateDatabaseContextFile()
    {
        var contents = entityGenerator.Generate(this.tables.ToList(), this.databaseName);
        Logger.Debug(contents);

        GeneratorUtility.WriteOutputFile(contents, databaseName, outputFilePath);
    }
}
