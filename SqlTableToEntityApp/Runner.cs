using SqlTableToEntityApp.Generators;

namespace SqlTableToEntityApp;

internal class Runner
{
    public ActionType Action {  get; set; }

    public string DatabaseName { get; set; }

    public string TemplateFile { get;set; }

    public string OutputFilePath { get; set; }

    public string Files { get; set; }

    public string[] Paths => Files?.Split(",").Select(item => item.Trim()).ToArray();

    public void SaveTableJsonFiles()
    {
        if (Paths == null || Paths.Length == 0)
        {
            return;
        }

        var tableParser = new TableParserFromFiles(Paths);
        var generator = new JsonGenerator(tableParser, DatabaseName, OutputFilePath);
        generator.SaveTableJsonFiles();
    }

    public int Generate()
    {
        if (string.IsNullOrWhiteSpace(TemplateFile))
        {
            Console.WriteLine($"Missing --template-file [{TemplateFile}]");
            return -1;
        }

        if (string.IsNullOrWhiteSpace(OutputFilePath))
        {
            Console.WriteLine($"Missing --output-file-path argument [{OutputFilePath}]");
            return -1;
        }

        if (Paths == null || Paths.Length == 0)
        {
            Console.WriteLine($"Missing --Files to be processed.");
            return -1;
        }

        var tableParser = new TableParserFromFiles(Paths);

        switch (Action)
        {
            case ActionType.Validate:
                break;

            case ActionType.Entity:
                {
                    var generator = new EntityGenerator(TemplateFile, tableParser, DatabaseName, OutputFilePath);
                    generator.GenerateEntityFiles();
                }
                break;

            case ActionType.Json:
                {
                    var generator = new JsonGenerator(tableParser, DatabaseName, OutputFilePath);
                    generator.SaveTableJsonFiles();
                }
                break;

            case ActionType.Context:
                {
                    var generator = new ContextGenerator(TemplateFile, tableParser, DatabaseName, OutputFilePath);
                    generator.GenerateDatabaseContextFile();
                }
                break;
        }
        return 0;
    }
}
