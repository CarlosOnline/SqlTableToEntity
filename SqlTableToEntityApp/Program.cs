using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using SqlRazorGenerator;
using SqlTableParser.Models;
using SqlTableToEntityApp.Generators;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SqlTableToEntityApp;

internal class Program
{
    static ILogger Logger => Log.Logger;

    static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
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

    /// <summary>
    /// Generate entity models from Sql Table files.  Entity models are specified using .net Razor template files.  This allows the user to generate any type of file desired.  See README.md for example Razor templates.
    ///
    /// POCO generation example:
    /// ------------------------
    /// Example to generate POCO entities.
    ///
    /// --files points to database folders with tables under AppleSchema and OrangeSchema folders.
    ///
    /// --output-file-path stores entities under respective MyCodeProject\MyDatabase\AppleSchema and MyCodeProject\MyDatabase\OrangeSchema folders.
    ///
    /// SqlTableToEntityApp.exe --generate Entity
    ///     --template-file "MyTemplates\PocoEntity.cs"
    ///     --output-file-path "MyCodeProject\{Database}\{Schema}\{Table}.cs"
    ///     --files "MyDatabase\AppleSchema\Tables","MyDatabase\Contoso\OrangeSchema\Tables"
    ///
    /// Database context generation example:
    /// ------------------------------------
    /// Example to generate database context file for all tables.
    ///
    /// --files points to database folders with tables under AppleSchema and OrangeSchema folders.
    ///
    /// SqlTableToEntityApp.exe --generate Context
    ///     --template-file "MyTemplates\PocoEntity.cs"
    ///     --output-file-path "MyCodeProject\database\DatabaseContext.cs"
    ///     --files "MyDatabase\AppleSchema\Tables","MyDatabase\Contoso\OrangeSchema\Tables"
    ///
    /// Validate template example:
    /// --------------------------
    /// Example to validate template in a loop.  Helful for developing templates.
    ///
    /// SqlTableToEntityApp.exe --generate Validate
    ///     --template-file "MyTemplates\PocoEntity.cs"
    /// </summary>
    /// <param name="generate">Generate type: entities, database context, json files, or validate the template.</param>
    /// <param name="databaseName">Database name.</param>
    /// <param name="templateFile">Razor template file used to generate C# or Java or whatever models.</param>
    /// <param name="outputFilePath">Output file path pattern.  Example folder\{Database}\{Schema}\{Table}.ext.</param>
    /// <param name="files">Folders or files to be processed comma seperated.</param>
    static int Main(
        ActionType generate,
        string databaseName,
        string templateFile,
        string outputFilePath,
        string files)
    {
        JsonConvert.DefaultSettings = () => JsonSerializerSettings;

        var logFileName = $"{Assembly.GetEntryAssembly().GetName().Name}.log";
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.File(logFileName, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Logger.Information($"**************************************************************************");
        Logger.Information($"Starting SqlTableToEntity");

        if (generate == ActionType.Json)
        {
            SaveTableJsonFiles(databaseName, outputFilePath, files);
            return 0;
        }

        if (generate == ActionType.Validate)
        {
            ValidateTemplate(templateFile);
            return 0;
        }

        var runner = new Runner
        {
            Action = generate,
            DatabaseName = databaseName,
            TemplateFile = templateFile,
            OutputFilePath = outputFilePath,
            Files = files,
        };

        return runner.Generate();
    }

    private static void SaveTableJsonFiles(string databaseName, string outputFilePath, string files)
    {
        var paths = files.Split(",").Select(item => item.Trim()).ToArray();
        var tableParser = new TableParserFromFiles(paths);
        var generator = new JsonGenerator(tableParser, databaseName, outputFilePath);
        generator.SaveTableJsonFiles();
    }

    private static void ValidateTemplate(string templateFile)
    {
        while (true)
        {
            try
            {
                Logger.Information($"Validating {templateFile}");
                var razorGenerator = new RazorGenerator<Table>(templateFile);
                Logger.Information("SUCCESS");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed validation of {templateFile}");
                var errorMessage = Regex.Replace(ex.Message, "  +", " ");
                foreach (var line in errorMessage.Split('\n').Take(20))
                {
                    Console.WriteLine(line);
                }
            }

            Console.WriteLine("Press any key to redo validation");
            Console.ReadKey();
        }
    }
}
