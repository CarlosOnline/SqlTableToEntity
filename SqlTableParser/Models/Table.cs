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
    /// Tells whether or not the table contains an identity column.
    /// </summary>
    /// <returns>True if any identity columns found.</returns>
    public bool ContainsIdentity()
    {
        return Columns.Any(item => item.IsIdentity);
    }
}
