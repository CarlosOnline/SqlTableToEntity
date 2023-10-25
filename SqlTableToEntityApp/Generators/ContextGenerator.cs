using SqlTableParser.Models;

namespace SqlTableToEntityApp.Generators;

internal class ContextGenerator : BaseGenerator<List<Table>>
{
    public ContextGenerator(string templateFilePath, IEnumerable<Table> tables, string outputFilePath) :
        base(templateFilePath, tables, outputFilePath)
    {
    }

    public void GenerateDatabaseContextFile()
    {
        var contents = entityGenerator.Generate(this.tables.ToList());
        Logger.Debug(contents);

        GeneratorUtility.WriteOutputFile(contents, outputFilePath);
    }
}
