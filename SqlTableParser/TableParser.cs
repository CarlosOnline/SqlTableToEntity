using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using SqlTableParser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SqlTableParser;

/// <summary>
/// Table parser.
/// </summary>
public class TableParser
{
    private string FilePath { get; set; }

    /// <summary>
    /// Diagnostic mode.
    /// </summary>
    public static bool DiagnosticMode { get; set; }

    /// <summary>
    /// Constructs table parser.
    /// </summary>
    /// <param name="filePath">Sql file path.</param>
    public TableParser(string filePath)
    {
        this.FilePath = filePath;
    }

    /// <summary>
    /// Parse sql file.
    /// </summary>
    /// <returns>Collection of tables if any found.</returns>
    public IEnumerable<Table> ParseSqlForTable()
    {
        foreach (var statement in GetCreateTableStatements())
        {
            var table = ProcessCreateTable(statement);
            table.SourceFilePath = FilePath;

            yield return table;
        }
    }

    private IEnumerable<SqlCreateTableStatement> GetCreateTableStatements()
    {
        var parserResult = Parser.Parse(File.ReadAllText(FilePath));

        foreach (var batch in parserResult.Script.Batches)
        {
            foreach (var statement in batch.Statements)
            {
                switch (statement)
                {
                    case SqlCreateTableStatement createTableStatement:
                        yield return createTableStatement;
                        break;
                }
            }
        }
    }

    private Table ProcessCreateTable(SqlCreateTableStatement statement)
    {
        var table = new Table
        {
            Schema = statement.Name.SchemaName.Value,
            Name = statement.Name.ObjectName.Value,
            Sql = statement.Sql
        };

        DebugLog($"{table.Schema} {table.Name}");

        ProcessTableDefinition(statement.Definition, table);

        UpdateInterColumnInformation(table);

        ValidateTable(table);

        return table;
    }

    private void ValidateTable(Table table)
    {
        var missing = table.Columns.Where(item => string.IsNullOrWhiteSpace(item.Name)).ToArray();
        if (missing.Length > 0)
        {
            throw new Exception($"Missing names for {string.Join(",", missing.Select(item => item.Name))} in {table.DiagnosticString}");
        }

        missing= table.Columns.Where(item => string.IsNullOrWhiteSpace(item.DataType)).ToArray();
        if (missing.Length > 0)
        {
            throw new Exception($"Missing types for {string.Join(",", missing.Select(item => item.Name))} in {table.DiagnosticString}");
        }
    }

    private void UpdateInterColumnInformation(Table table)
    {
        foreach (var columnName in table.PrimaryKeyColumns)
        {
            var column = table.Columns.FirstOrDefault(item => string.Equals(item.Name, columnName, StringComparison.OrdinalIgnoreCase));
            if (column != null)
            {
                column.IsPrimaryKeyExplicit = true;
            }
        }

        foreach (var column in table.Columns)
        {
            if (!string.IsNullOrWhiteSpace(column.ReferenceColumn))
            {
                var referenceColumn = table.Columns.FirstOrDefault(item => string.Equals(item.Name, column.ReferenceColumn, StringComparison.OrdinalIgnoreCase));
                if (referenceColumn != null)
                {
                    column.DataType = referenceColumn.DataType;
                }
                else
                {
                    column.DataType = column.ReferenceColumn;
                }
            }
        }
    }

    private void ProcessTableDefinition(SqlTableDefinition statement, Table table)
    {
        foreach (var child in statement.Children)
        {
            switch (child)
            {
                case SqlComputedColumnDefinition definition:
                    {
                        var column = ProcessComputedColumn(definition);
                        table.Columns.Add(column);
                    }
                    break;

                case SqlColumnDefinition definition:
                    {
                        var column = ProcessColumn(definition, table);
                        table.Columns.Add(column);
                    }
                    break;

                case SqlPrimaryKeyConstraint constraint:
                    table.PrimaryKeyColumns.AddRange(GetPrimaryKeyColumns(constraint));
                    table.PrimaryKeyColumns = table.PrimaryKeyColumns.OrderBy(item => item).Distinct().ToList();
                    break;

                case SqlConstraint _:
                    break;

                default:
                    LogUnknownObject(child);
                    break;
            }
        }
    }

    private List<string> GetPrimaryKeyColumns(SqlPrimaryKeyConstraint constraint)
    {
        var results = new List<string>();
        foreach (var child in constraint.Children)
        {
            switch (child)
            {
                case SqlIndexedColumn indexedColumn:
                    results.Add(indexedColumn.Name.Value);
                    break;

                case SqlIdentifier _:
                case SqlIndexOption _:
                    break;

                default:
                    LogUnknownObject(child);
                    break;
            }
        }

        return results;
    }

    private TableColumn ProcessColumn(SqlColumnDefinition definition, Table table)
    {
        DebugLog($"{definition.DataType.DataType.ObjectIdentifier.GetValue()} {definition.Name} {definition.Sql}");

        var column = new TableColumn
        {
            Sql = definition.Sql,
        };

        foreach (var child in definition.Children)
        {
            switch (child)
            {
                case SqlIdentifier identifier:
                    column.Name = identifier.Value;
                    break;

                case SqlDataTypeSpecification specification:
                    column.DataType = specification.GetValue();
                    column.DataTypeExtraInfo1 = specification.Argument1;
                    column.DataTypeExtraInfo2 = specification.Argument2;
                    break;

                case SqlColumnIdentity _:
                    column.IsIdentityColumnExplicit = true;
                    break;

                case SqlPrimaryKeyConstraint constraint:
                    if (constraint.IndexedColumns?.Count > 0)
                    {
                        table.PrimaryKeyColumns.AddRange(constraint.IndexedColumns.Select(item => item.Name.Value));
                        table.PrimaryKeyColumns = table.PrimaryKeyColumns.OrderBy(item => item).Distinct().ToList();
                    }
                    else if (constraint.Children.Count() == 0)
                    {
                        column.IsPrimaryKeyExplicit = true;
                        table.PrimaryKeyColumns.Add(column.Name);
                        table.PrimaryKeyColumns = table.PrimaryKeyColumns.OrderBy(item => item).Distinct().ToList();
                    }
                    break;

                case SqlConstraint constraint:
                    column.SqlContraintType = constraint.Type;
                    break;

                case SqlCollation _:
                    break;

                default:
                    LogUnknownObject(child);
                    break;
            }
        }

        return column;
    }

    private TableColumn ProcessComputedColumn(SqlComputedColumnDefinition definition)
    {
        DebugLog($"{definition.Name} {definition.Sql}");

        var column = new TableColumn
        {
            Computed = true,
            Sql = definition.Sql,
        };

        foreach (var child in definition.Children)
        {
            switch (child)
            {
                case SqlIdentifier identifier:
                    column.Name = identifier.Value;
                    break;

                case SqlSimpleCaseExpression expression:
                    column.DataType = ProcessCaseStatement(expression);
                    break;

                case SqlBuiltinScalarFunctionCallExpression expression:
                    column.ReferenceColumn = ProcessFunctionCallStatement(expression);
                    break;

                case SqlColumnRefExpression expression:
                    column.ReferenceColumn = expression.ColumnName.Value;
                    break;

                case SqlConstraint _:
                    break;

                default:
                    LogUnknownObject(child);
                    column.DataType = "VARCHAR";
                    break;
            }
        }

        return column;
    }

    private string ProcessFunctionCallStatement(SqlBuiltinScalarFunctionCallExpression expression)
    {
        switch (expression.FunctionName.ToUpper())
        {
            case "ISNULL":
                return GetColumnName(expression.Children.FirstOrDefault());

            case "CONVERT":
                // Returns a type
                return GetColumnName(expression.Children.FirstOrDefault());

            default:
                var columnName = GetColumnName(expression.Children.FirstOrDefault());
                if (!string.IsNullOrWhiteSpace(columnName))
                {
                    return columnName;
                }

                LogUnknownObject(expression);
                return null;
        }
    }

    private string ProcessCaseStatement(SqlSimpleCaseExpression element)
    {
        switch (element.WhenClauses.FirstOrDefault())
        {
            case SqlSimpleWhenClause clause:
                return clause.GetDataType();

            default:
                LogUnknownObject(element);
                return "VARCHAR";
        }
    }

    private string GetColumnName(SqlCodeObject codeObject)
    {
        switch (codeObject)
        {
            case SqlColumnRefExpression expression:
                return expression.ColumnName.Value;

            case SqlDataTypeSpecification specification:
                return specification.GetValue();

            default:
                LogUnknownObject(codeObject);
                return null;
        }
    }

    private void DebugLog(string message)
    {
        if (DiagnosticMode)
        {
            Console.WriteLine(message);
        }

        Debug.WriteLine(message);
    }

    private void LogUnknownObject(SqlCodeObject codeObject)
    {
        Console.WriteLine($"UNKNOWN {codeObject.GetType().GetTypeName()} {codeObject.Sql} in {FilePath}");
        if (Debugger.IsAttached)
        {
            Debugger.Break();
        }
    }
}