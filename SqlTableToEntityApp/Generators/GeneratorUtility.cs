using Serilog;
using SqlTableParser.Models;

namespace SqlTableToEntityApp.Generators;

internal class GeneratorUtility
{
    static ILogger Logger => Log.Logger;

    public static string GetOutputFilePath(Table table, string outputPath)
    {
        var outputFilePath = outputPath.Replace("{Schema}", table.Schema)
            .Replace("{Table}", table.Name);

        if (Directory.Exists(outputFilePath))
        {
            outputFilePath = Path.Combine(outputFilePath, table.Name + ".txt");
        }

        return outputFilePath;
    }

    public static void WriteOutputFile(string contents, string outputFilePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
        File.WriteAllText(outputFilePath, contents);

        Logger.Information($"Generated {outputFilePath}");
    }
}
