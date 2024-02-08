using SqlTableParser.Models;

namespace SqlTableToEntityApp.Generators;

internal class EntityGenerator : BaseGenerator<Table>
{
    public EntityGenerator(string templateFilePath, IEnumerable<Table> tables, string databaseName, string outputFilePath) :
        base(templateFilePath, tables, databaseName, outputFilePath)
    {
    }

    public void GenerateEntityFiles()
    {
        foreach (var table in this.tables)
        {
            var outputFilePath = GetOutputFilePath(table);
            table.OutputFilePath = outputFilePath;

            var contents = entityGenerator.Generate(table, this.databaseName);
            Logger.Debug(contents);

            GeneratorUtility.WriteOutputFile(contents, databaseName, outputFilePath);
        }
    }
}
