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
            var contents = entityGenerator.Generate(table, this.databaseName);
            Logger.Debug(contents);

            var outputFilePath = GetOutputFilePath(table);
            GeneratorUtility.WriteOutputFile(contents, databaseName, outputFilePath);
        }
    }
}
