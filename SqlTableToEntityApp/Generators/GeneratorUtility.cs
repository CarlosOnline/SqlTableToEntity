using Serilog;
using SqlTableParser.Models;

namespace SqlTableToEntityApp.Generators;

internal class GeneratorUtility
{
    static ILogger Logger => Log.Logger;

    public static string GetOutputFilePath(Table table, string databaseName, string outputPath)
    {
        var outputFilePath = outputPath
            .Replace("{Database}", databaseName)
            .Replace("{Schema}", table.Schema)
            .Replace("{Table}", table.Name);

        if (Directory.Exists(outputFilePath))
        {
            outputFilePath = Path.Combine(outputFilePath, table.Name + ".txt");
        }

        return outputFilePath;
    }

    public static void WriteOutputFile(string contents, string databaseName, string outputFilePath)
    {
        outputFilePath = outputFilePath.Replace("{Database}", databaseName);

        contents = contents.Replace("\r\n}", "}");
        Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
        File.WriteAllText(outputFilePath, contents);

        Logger.Information($"Generated {outputFilePath}");
    }
}
