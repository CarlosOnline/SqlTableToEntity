using Serilog;
using SqlRazorGenerator;
using SqlTableParser.Models;
using SqlTableToEntityApp.Generators;
using System.Reflection;

namespace SqlTableToEntityApp;

internal class Program
{
    static ILogger Logger => Log.Logger;

    /// <summary>
    /// Generate entity models from Sql Table files.  Entity models are specified using .net Razor template files.  This allows the user to generate any type of file desired.  See README.md for example Razor templates.
    /// 
    /// POCO generation example:
    /// ------------------------
    /// Example to generate POCO entities.
    /// 
    /// --files points to database folders with tables under AppleSchema and OrangeSchema folders.
    /// 
    /// --output-file-path stores entities under respective MyCodeProject\AppleSchema and MyCodeProject\OrangeSchema folders.
    /// 
    /// SqlTableToEntityApp.exe --generate Entity
    ///     --template-file "MyTemplates\PocoEntity.cs"
    ///     --output-file-path "MyCodeProject\database\{Schema}\{Table}.cs" 
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
    /// <param name="templateFile">Razor template file used to generate C# or Java or whatever models.</param>
    /// <param name="outputFilePath">Output file path pattern.  Example folder\{Schema}\{Table}.ext.</param>
    /// <param name="files">Folders or files to be processed comma seperated.</param>
    static int Main(
        ActionType generate,
        string templateFile,
        string outputFilePath,
        string files)
    {
        var logFileName = $"{Assembly.GetEntryAssembly().GetName().Name}.log";
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.File(logFileName, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Logger.Information($"**************************************************************************");
        Logger.Information($"Starting SqlTableToEntity");

        if (string.IsNullOrWhiteSpace(templateFile) || !File.Exists(templateFile))
        {
            Console.WriteLine($"Missing template-file: [ {templateFile} ]");
            return -1;
        }

        if (generate == ActionType.Validate)
        {
            ValidateTemplate(templateFile);
            return 0;
        }

        if (string.IsNullOrWhiteSpace(outputFilePath))
        {
            Console.WriteLine($"Missing output-folder argument");
            return -1;
        }

        if (files.Length == 0)
        {
            Console.WriteLine($"Missing files to be processed.");
            return -1;
        }

        var paths = files.Split(",").Select(item => item.Trim()).ToArray();
        var tableParser = new TableParserFromFiles(paths);

        switch (generate)
        {
            case ActionType.Validate:
                break;

            case ActionType.Entity:
                {
                    var generator = new EntityGenerator(templateFile, tableParser, outputFilePath);
                    generator.GenerateEntityFiles();
                }
                break;

            case ActionType.Json:
                {
                    var generator = new JsonGenerator(tableParser, outputFilePath);
                    generator.SaveTableJsonFiles();
                }
                break;

            case ActionType.Context:
                {
                    var generator = new ContextGenerator(templateFile, tableParser, outputFilePath);
                    generator.GenerateDatabaseContextFile();
                }
                break;
        }
        return 0;
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
            catch
            {
                Logger.Error($"Failed validation of {templateFile}");
            }

            Console.WriteLine("Press any key to redo validation");
            Console.ReadKey();
        }
    }
}
