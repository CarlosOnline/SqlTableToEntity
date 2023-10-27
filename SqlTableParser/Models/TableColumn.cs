using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Newtonsoft.Json;
using System.Diagnostics;

namespace SqlTableParser.Models;

/// <summary>
/// Table column.
/// </summary>
[DebuggerDisplay("{DiagnosticString}")]
public class TableColumn
{
    /// <summary>
    /// Column name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Sql data type.
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// Sql data type extra info 1.
    /// </summary>
    public int? DataTypeExtraInfo1 { get; set; }

    /// <summary>
    /// Sql data type extra info 2.
    /// </summary>
    public int? DataTypeExtraInfo2 { get; set; }

    /// <summary>
    /// Tells whether or not the column is computed.
    /// </summary>
    public bool Computed { get; set; }

    /// <summary>
    /// Tells whether or not the column is an identity column.
    /// </summary>
    public bool IsIdentity => IsIdentityColumnExplicit || IsIdentityConstraint;

    /// <summary>
    /// Tells whether or not the column is a primary key.
    /// </summary>
    public bool IsPrimaryKey => IsPrimaryKeyExplicit || IsPrimaryKeyConstraint;

    /// <summary>
    /// References column that contains the data type for functions like ISNULL(...).
    /// Can also be the data type for CONVERT([date], ...)
    /// </summary>
    public string ReferenceColumn { get; set; }

    /// <summary>
    /// Original sql for the column.
    /// </summary>
    public string Sql { get; set; }

    /// <summary>
    /// Is primary key explicit.
    /// </summary>
    public bool IsPrimaryKeyExplicit { get; set; }

    /// <summary>
    /// Is identity column explicit.
    /// </summary>
    public bool IsIdentityColumnExplicit { get; set; }

    /// <summary>
    /// Sql constraint.
    /// </summary>
    public SqlConstraintType SqlContraintType { get; set; }

    /// <summary>
    /// Tells whether or not the column has an identity constraint.
    /// </summary>
    public bool IsIdentityConstraint => SqlContraintType == SqlConstraintType.Identity;

    /// <summary>
    /// Tells whether or not the column is nullable.
    /// </summary>
    public bool IsNull => SqlContraintType == SqlConstraintType.Null;

    /// <summary>
    /// Tells whether or not the column is not nullable.
    /// </summary>
    public bool IsNotNull => SqlContraintType == SqlConstraintType.NotNull;

    /// <summary>
    /// Tells whether or not the column has a default value.
    /// </summary>
    public bool IsDefault => SqlContraintType == SqlConstraintType.Default;

    /// <summary>
    /// Tells whether or not the column has a primary key constraint.
    /// </summary>
    public bool IsPrimaryKeyConstraint => SqlContraintType == SqlConstraintType.PrimaryKey;

    /// <summary>
    /// Tells whether or not the column has a foreign key constraint.
    /// </summary>
    public bool IsForeignKey => SqlContraintType == SqlConstraintType.ForeignKey;

    /// <summary>
    /// Tells whether or not the column is a row guid column.
    /// </summary>
    public bool IsRowGuidCol => SqlContraintType == SqlConstraintType.RowGuidCol;

    /// <summary>
    /// Tells whether or not the column is unique.
    /// </summary>
    public bool IsUnique => SqlContraintType == SqlConstraintType.Unique;

    /// <summary>
    /// Diagnostic string.
    /// </summary>
    [JsonIgnore]
    public string DiagnosticString
    {
        get
        {
            var identity = IsIdentity ? "IDENTITY" : string.Empty;
            var readOnly = Computed ? "READONLY" : string.Empty;
            var primaryKey = IsPrimaryKey ? "PRIMARY" : string.Empty;
            var nullable = IsNull ? "NULL" : string.Empty;
            var notNullable = IsNotNull ? "NOT NULL" : string.Empty;

            return $"{DataType,-20} {Name,-20} {primaryKey} {identity} {readOnly} {nullable} {notNullable}";
        }
    }
}
