using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace SqlTableParser.Models;

/// <summary>
/// Sql Table.
/// </summary>
[DebuggerDisplay("{DiagnosticString}")]
public class Table
{
    /// <summary>
    /// Schema name.
    /// </summary>
    public string Schema { get; set; }

    /// <summary>
    /// Name of table.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Table columns.
    /// </summary>
    public List<TableColumn> Columns { get; set; } = new List<TableColumn>();

    /// <summary>
    /// Primary key columns.
    /// </summary>
    public List<string> PrimaryKeyColumns { get; set; } = new List<string>();

    /// <summary>
    /// Has multiple primary keys.
    /// </summary>
    public bool MultiplePrimaryKeys => PrimaryKeyColumns.Count > 1;

    /// <summary>
    /// Has identity column.
    /// </summary>
    public bool HasIdentityColumn => Columns.Any(item => item.IsIdentity);

    /// <summary>
    /// Source file path.
    /// </summary>
    public string SourceFilePath { get; set; }

    /// <summary>
    /// Source folder path.
    /// </summary>
    public string SourceFolder => Path.GetDirectoryName(SourceFilePath);

    /// <summary>
    /// Source file name.
    /// </summary>
    public string SourceFileName => Path.GetFileName(SourceFilePath);

    /// <summary>
    /// Source file name without extension.
    /// </summary>
    public string SourceBaseFileName => Path.GetFileNameWithoutExtension(SourceFilePath);

    /// <summary>
    /// Output file path.
    /// </summary>
    public string OutputFilePath { get; set; }

    /// <summary>
    /// Output folder path.
    /// </summary>
    public string OutputFolder => Path.GetDirectoryName(OutputFilePath);

    /// <summary>
    /// Output file name.
    /// </summary>
    public string OutputFileName => Path.GetFileName(OutputFilePath);

    /// <summary>
    /// Output file name without extension.
    /// </summary>
    public string OutputBaseFileName => Path.GetFileNameWithoutExtension(OutputFilePath);

    /// <summary>
    /// Original table sql.
    /// </summary>
    public string Sql { get; set; }

    /// <summary>
    /// Tells whether or not the column is a primary key.
    /// </summary>
    /// <param name="columnName">Column name.</param>
    /// <returns>True if column is a primary key.</returns>
    public bool IsPrimaryKey(string columnName)
    {
        return PrimaryKeyColumns.Any(item => item.Equals(columnName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Diagnostic string.
    /// </summary>
    [JsonIgnore]
    public string DiagnosticString
    {
        get
        {
            var primaryKeyColumns = string.Join(",", PrimaryKeyColumns);
            return $"[{Schema}].[{Name}] Columns: {Columns.Count:N0} {primaryKeyColumns}";
        }
    }

    /// <summary>
    /// Tells whether or not the table contains a column with the specified data type.
    /// </summary>
    /// <param name="type">Sql data type.</param>
    /// <returns></returns>
    public bool ContainsDataType(string type)
    {
        return Columns.Any(item => string.Equals(item.DataType, type, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Tells whether or not the table contains any not null columns.
    /// </summary>
    /// <returns>True if any not null columns found.</returns>
    public bool ContainsNotNull()
    {
        return Columns.Any(item => item.IsNotNull);
    }

    /// <summary>
    /// Tells whether or not the table contains a primary key column.
    /// </summary>
    /// <returns>True if any primary columns found.</returns>
    public bool ContainsPrimaryKey()
    {
        return Columns.Any(item => item.IsPrimaryKey);
    }

    /// <summary>
    /// Check if column is an Id column.
    /// </summary>
    /// <param name="name">Name of column.</param>
    /// <returns>True if column is the Id column.</returns>
    public bool IsIdColumn(string name)
    {
        var column = Columns.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        if (MultiplePrimaryKeys && HasIdentityColumn)
        {
            return column.IsIdentity;
        }
        else if (MultiplePrimaryKeys && !HasIdentityColumn)
        {
            return string.Equals(column.Name, PrimaryKeyColumns[0], StringComparison.OrdinalIgnoreCase);
        }

        return column.IsPrimaryKey;
    }

    /// <summary>
    /// Tells whether or not the table contains an identity column.
    /// </summary>
    /// <returns>True if any identity columns found.</returns>
    public bool ContainsIdentity()
    {
        return Columns.Any(item => item.IsIdentity);
    }
}
