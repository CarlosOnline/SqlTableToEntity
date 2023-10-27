using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlTableParser.Models;

namespace SqlTableToEntityApp.Generators;

internal class JsonGenerator
{
    readonly IEnumerable<Table> tables;
    private readonly string databaseName;
    private readonly string outputFilePath;

    private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = true,
                ProcessExtensionDataNames = true,
            }
        },
        Converters = new JsonConverter[] { new Newtonsoft.Json.Converters.StringEnumConverter() },
    };

    public JsonGenerator(IEnumerable<Table> tables, string databaseName, string outputFilePath)
    {
        this.tables = tables;
        this.outputFilePath = outputFilePath;
        this.databaseName = databaseName;
    }

    public void SaveTableJsonFiles()
    {
        foreach (var table in this.tables)
        {
            var json = JsonConvert.SerializeObject(table, jsonSerializerSettings);

            var jsonFilePath = Path.ChangeExtension(GeneratorUtility.GetOutputFilePath(table, databaseName, outputFilePath), "json");
            GeneratorUtility.WriteOutputFile(json, databaseName, jsonFilePath);
        }
    }
}
