using Newtonsoft.Json;
using SqlTableParser.Models;

namespace SqlTableToEntityApp.Generators;

internal class JsonGenerator
{
    readonly IEnumerable<Table> tables;
    private readonly string outputFilePath;

    public JsonGenerator(IEnumerable<Table> tables, string outputFilePath)
    {
        this.tables = tables;
        this.outputFilePath = outputFilePath;
    }

    public void SaveTableJsonFiles()
    {
        foreach (var table in this.tables)
        {
            var json = JsonConvert.SerializeObject(table, Formatting.Indented);

            var jsonFilePath = Path.ChangeExtension(GeneratorUtility.GetOutputFilePath(table, outputFilePath), "json");
            GeneratorUtility.WriteOutputFile(json, jsonFilePath);
        }
    }
}
