# SqlTableToEntity
- Generate entity model files from SQL Table sql files.
- Parses SQL files, and then uses Razor template file template to generate entity file.
- Can generate C# POCO, Java POJO, or any type of entity files.

Contents:
- Console application: `SqlTableToEntityApp`

# Build
```
dotnet build SqlTableToEntity.sln
dotnet publish SqlTableToEntity.sln
```

# SqlTableToEntityApp Console Application

## Usage
`SqlTableToEntityApp.exe --help`

```
Description:
  Generate entity models from Sql Table files.  Entity models are specified using .net Razor template files.  This allows the user
  to generate any type of file desired.  See README.md for example Razor templates.

               POCO generation example:
               ------------------------
               Example to generate POCO entities.

               --files points to database folders with tables under AppleSchema and OrangeSchema folders.

               --output-file-path stores entities under respective MyCodeProject\MyDatabase\AppleSchema and
  MyCodeProject\MyDatabase\OrangeSchema folders.

               SqlTableToEntityApp.exe --generate Entity
                   --template-file "MyTemplates\PocoEntity.cs"
                   --output-file-path "MyCodeProject\{Database}\{Schema}\{Table}.cs"
                   --files "MyDatabase\AppleSchema\Tables","MyDatabase\Contoso\OrangeSchema\Tables"

               Database context generation example:
               ------------------------------------
               Example to generate database context file for all tables.

               --files points to database folders with tables under AppleSchema and OrangeSchema folders.

               SqlTableToEntityApp.exe --generate Context
                   --template-file "MyTemplates\PocoEntity.cs"
                   --output-file-path "MyCodeProject\database\DatabaseContext.cs"
                   --files "MyDatabase\AppleSchema\Tables","MyDatabase\Contoso\OrangeSchema\Tables"

               Validate template example:
               --------------------------
               Example to validate template in a loop.  Helful for developing templates.

               SqlTableToEntityApp.exe --generate Validate
                   --template-file "MyTemplates\PocoEntity.cs"

Usage:
  SqlTableToEntityApp [options]

Options:
  --generate <Context|Entity|Json|Validate>  Generate type: entities, database context, json files, or validate the template.
  --database-name <database-name>            Database name.
  --template-file <template-file>            Razor template file used to generate C# or Java or whatever models.
  --output-file-path <output-file-path>      Output file path pattern.  Example folder\{Database}\{Schema}\{Table}.ext.
  --files <files>                            Folders or files to be processed comma seperated.
  --version                                  Show version information
  -?, -h, --help                             Show help and usage information
```

---

# Entity Razor Model Info
- The `Entity` razor template Model variable `@Model` is of type: [Table](#T-SqlTableParser-Models-Table 'SqlTableParser.Models.Table').
- The `Context` razor template Model variable `@Model` is of type: [List\<Table\>](#T-SqlTableParser-Models-Table 'SqlTableParser.Models.Table').

For example to render the table name use `@Model.Name`, use `@Model.Schema` to render the table schema name.  Use `@Database` to render the database name which comes from `-database-name` command line parameter.

---

# Sample C# POCO Razor template
```
@functions {
    string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.Length == 1)
        {
            return value.ToLowerInvariant();
        }

        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }

    string ToPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.Length == 1)
        {
            return value.ToUpperInvariant();
        }

        return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }

    public string GetNativeType(string sqlType)
    {
        return sqlType.ToLower() switch
        {
            "bigint" => "long",
            "binary" => "byte[]",
            "bit" => "bool",
            "char" => "string",
            "datetime" => "DateTime",
            "datetime2" => "DateTime",
            "datetimeoffset" => "DateTime",
            "decimal" => "decimal",
            "float" => "double",
            "image" => "byte[]",
            "int" => "int",
            "money" => "decimal",
            "nchar" => "string",
            "ntext" => "string",
            "numeric" => "decimal",
            "nvarchar" => "string",
            "real" => "Single",
            "rowversion" => "byte[]",
            "smalldatetime" => "DateTime",
            "smallint" => "Int16",
            "smallmoney" => "decimal",
            "sql_variant" => "object",
            "text" => "string",
            "timestamp" => "byte[]",
            "tinyint" => "byte",
            "uniqueidentifier" => "Guid",
            "varbinary" => "byte[]",
            "varchar" => "string",
            "xml" => "Xml",
            _ => "string",
        };
    }
}
namespace JoyOfPlaying.Database.Tables;

using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text.Json.Serialization;

/// <summary>
/// @(@Model.Name).
/// </summary>
[Table("@(@Model.Name)", Schema = "@(@Model.Schema)")]
public class @(@Model.Name)
{
    @foreach(var column in @Model.Columns)
    {
        @if(column.IsIdentity)
        {
@:    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        }

@:    public @(@GetNativeType(column.DataType)) @(@ToPascalCase(column.Name)) {get; set;)
@:
    }
}
```

---

# Sample Java POJO Template
```
@functions {
    public string GetNativeType(string sqlType)
    {
        return sqlType.ToLower() switch
        {
            "bigint" => "Long",
            "binary" => "byte[]",
            "bit" => "Boolean",
            "char" => "String",
            "date" => "Date",
            "datetime" => "Timestamp",
            "datetime2" => "Timestamp",
            "decimal" => "BigDecimal",
            "float" => "double",
            "int" => "Integer",
            "money" => "BigDecimal",
            "nchar" => "String",
            "numeric" => "BigDecimal",
            "nvarchar" => "String",
            "real" => "float",
            "smalldatetime" => "Timestamp",
            "smallint" => "short",
            "smallmoney" => "BigDecimal",
            "sysname" => "String",
            "tinyint" => "short",
            "uniqueidentifier" => "String",
            "varbinary" => "byte[]",
            "varchar" => "String",
            _ => "String",
        };
    }

    string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.Length <= 3)
        {
            return value.ToLowerInvariant();
        }

        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }

    string ToPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.Length == 1)
        {
            return value.ToUpperInvariant();
        }

        return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }

    string GetSizeAttribute(string sqlType, int? size)
    {
        if (size == null || size <= 0 ||
            (!string.Equals(sqlType, "nvarchar", System.StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(sqlType, "varchar", System.StringComparison.OrdinalIgnoreCase)))
        {
            return null;
        }

        return $"@Size(max = {size})";
    }

    bool RequiresSizeAttribute(SqlTableParser.Models.Table table)
    {
        return table.Columns.Any(item => GetSizeAttribute(item.DataType, item.DataTypeExtraInfo1) != null);
    }
}
package com.joyofplaying.database.models.@(@Model.Schema.ToLower());

import jakarta.persistence.*;
@if (@Model.ContainsNotNull())
{
@:import jakarta.validation.constraints.NotNull;
}
@if (@RequiresSizeAttribute(@Model))
{
@:import jakarta.validation.constraints.Size;
}
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@if (@Model.ContainsDataType("decimal"))
{
@:import java.math.BigDecimal;
}
@if (@Model.ContainsDataType("date"))
{
@:import java.sql.Date;
}
@if (@Model.ContainsDataType("datetime") || @Model.ContainsDataType("datetime2") || @Model.ContainsDataType("smalldatetime"))
{
@:import java.sql.Timestamp;
}

@@Model(schema = "[@(@Model.Schema)]", name = "[@(@Model.Name)]")
@@Entity
@@Getter
@@Setter
@@Builder
@@AllArgsConstructor
@@NoArgsConstructor
public class @(@Model.Name)
{
    @foreach(var column in @Model.Columns)
    {
        @if(column.Computed)
        {
            continue;
        }

        @if(column.IsPrimaryKey)
        {
@:    @@Id
        }

        @if(column.IsIdentity)
        {
@:    @@GeneratedValue(strategy = GenerationType.IDENTITY)
        }

        @if(column.IsNotNull && !column.IsIdentity)
        {
@:    @@NotNull
        }

        @if(@GetSizeAttribute(column.DataType, column.DataTypeExtraInfo1) != null)
        {
@:    @GetSizeAttribute(column.DataType, column.DataTypeExtraInfo1)
        }

@:    @@Column(name = "[@(@column.Name)]")
@:    private @(@GetNativeType(column.DataType)) @(@ToCamelCase(column.Name));
@:
    }
}
```
---

# Sample C# Database Context Template
```
namespace JoyOfPlaying.Application.Database;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

@foreach(var table in @Model)
{
@:using JoyOfPlaying.Application.Database.Models.@(@table.Schema).@(table.Name);
}

public class DatabaseContext : DbContext {
    @foreach(var table in @Model)
    {
@:    public virtual DbSet<@(table.Name)> @(@table.Name) {get; set;}
@:
    }
}
```

---

# Sample Java Database Context Template
```
@functions {
    string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.Length == 1)
        {
            return value.ToLowerInvariant();
        }

        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }
}

package com.joyofplaying.application.database;

import lombok.Data;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@foreach(var table in @Model)
{
@:import com.joyofplaying.application.database.models.@(@table.Schema).@(table.Name);
}

@@Service
@@Data
public class DatabaseContext {
    @foreach(var table in @Model)
    {
@:    @@AutoWired
@:    @(table.Name)Repository @ToCamelCase(@table.Name)Repository;
@:
    }
}
```

## API Documentation

- [TemplateModel](#T-SqlRazorGenerator-TemplateModel 'SqlRazorGenerator.TemplateModel')
  - [Table](#P-SqlRazorGenerator-TemplateModel-Table 'SqlRazorGenerator.TemplateModel.Table')
  - [Tables](#P-SqlRazorGenerator-TemplateModel-Tables 'SqlRazorGenerator.TemplateModel.Tables')
  - [Database](#P-SqlRazorGenerator-TemplateModel-Database 'SqlRazorGenerator.TemplateModel.Database')
  - [EnvironmentVariables](#P-SqlRazorGenerator-TemplateModel-EnvironmentVariables 'SqlRazorGenerator.TemplateModel.EnvironmentVariables')

- [Table](#T-SqlTableParser-Models-Table 'SqlTableParser.Models.Table')
  - [Columns](#P-SqlTableParser-Models-Table-Columns 'SqlTableParser.Models.Table.Columns')
  - [DiagnosticString](#P-SqlTableParser-Models-Table-DiagnosticString 'SqlTableParser.Models.Table.DiagnosticString')
  - [HasIdentityColumn](#P-SqlTableParser-Models-Table-HasIdentityColumn 'SqlTableParser.Models.Table.HasIdentityColumn')
  - [MultiplePrimaryKeys](#P-SqlTableParser-Models-Table-MultiplePrimaryKeys 'SqlTableParser.Models.Table.MultiplePrimaryKeys')
  - [Name](#P-SqlTableParser-Models-Table-Name 'SqlTableParser.Models.Table.Name')
  - [OutputBaseFileName](#P-SqlTableParser-Models-Table-OutputBaseFileName 'SqlTableParser.Models.Table.OutputBaseFileName')
  - [OutputFileName](#P-SqlTableParser-Models-Table-OutputFileName 'SqlTableParser.Models.Table.OutputFileName')
  - [OutputFilePath](#P-SqlTableParser-Models-Table-OutputFilePath 'SqlTableParser.Models.Table.OutputFilePath')
  - [OutputFolder](#P-SqlTableParser-Models-Table-OutputFolder 'SqlTableParser.Models.Table.OutputFolder')
  - [PrimaryKeyColumns](#P-SqlTableParser-Models-Table-PrimaryKeyColumns 'SqlTableParser.Models.Table.PrimaryKeyColumns')
  - [Schema](#P-SqlTableParser-Models-Table-Schema 'SqlTableParser.Models.Table.Schema')
  - [SourceFileName](#P-SqlTableParser-Models-Table-SourceFileName 'SqlTableParser.Models.Table.SourceFileName')
  - [SourceFilePath](#P-SqlTableParser-Models-Table-SourceFilePath 'SqlTableParser.Models.Table.SourceFilePath')
  - [SourceFolder](#P-SqlTableParser-Models-Table-SourceFolder 'SqlTableParser.Models.Table.SourceFolder')
  - [Sql](#P-SqlTableParser-Models-Table-Sql 'SqlTableParser.Models.Table.Sql')
  - [ContainsDataType(type)](#M-SqlTableParser-Models-Table-ContainsDataType-System-String- 'SqlTableParser.Models.Table.ContainsDataType(System.String)')
  - [ContainsIdentity()](#M-SqlTableParser-Models-Table-ContainsIdentity 'SqlTableParser.Models.Table.ContainsIdentity')
  - [ContainsNotNull()](#M-SqlTableParser-Models-Table-ContainsNotNull 'SqlTableParser.Models.Table.ContainsNotNull')
  - [ContainsPrimaryKey()](#M-SqlTableParser-Models-Table-ContainsPrimaryKey 'SqlTableParser.Models.Table.ContainsPrimaryKey')
  - [IsIdColumn(name)](#M-SqlTableParser-Models-Table-IsIdColumn-System-String- 'SqlTableParser.Models.Table.IsIdColumn(System.String)')
  - [IsPrimaryKey(columnName)](#M-SqlTableParser-Models-Table-IsPrimaryKey-System-String- 'SqlTableParser.Models.Table.IsPrimaryKey(System.String)')
- [TableColumn](#T-SqlTableParser-Models-TableColumn 'SqlTableParser.Models.TableColumn')
  - [Computed](#P-SqlTableParser-Models-TableColumn-Computed 'SqlTableParser.Models.TableColumn.Computed')
  - [DataType](#P-SqlTableParser-Models-TableColumn-DataType 'SqlTableParser.Models.TableColumn.DataType')
  - [DataTypeExtraInfo1](#P-SqlTableParser-Models-TableColumn-DataTypeExtraInfo1 'SqlTableParser.Models.TableColumn.DataTypeExtraInfo1')
  - [DataTypeExtraInfo2](#P-SqlTableParser-Models-TableColumn-DataTypeExtraInfo2 'SqlTableParser.Models.TableColumn.DataTypeExtraInfo2')
  - [DiagnosticString](#P-SqlTableParser-Models-TableColumn-DiagnosticString 'SqlTableParser.Models.TableColumn.DiagnosticString')
  - [IsDefault](#P-SqlTableParser-Models-TableColumn-IsDefault 'SqlTableParser.Models.TableColumn.IsDefault')
  - [IsForeignKey](#P-SqlTableParser-Models-TableColumn-IsForeignKey 'SqlTableParser.Models.TableColumn.IsForeignKey')
  - [IsIdentity](#P-SqlTableParser-Models-TableColumn-IsIdentity 'SqlTableParser.Models.TableColumn.IsIdentity')
  - [IsIdentityColumnExplicit](#P-SqlTableParser-Models-TableColumn-IsIdentityColumnExplicit 'SqlTableParser.Models.TableColumn.IsIdentityColumnExplicit')
  - [IsIdentityConstraint](#P-SqlTableParser-Models-TableColumn-IsIdentityConstraint 'SqlTableParser.Models.TableColumn.IsIdentityConstraint')
  - [IsNotNull](#P-SqlTableParser-Models-TableColumn-IsNotNull 'SqlTableParser.Models.TableColumn.IsNotNull')
  - [IsNull](#P-SqlTableParser-Models-TableColumn-IsNull 'SqlTableParser.Models.TableColumn.IsNull')
  - [IsPrimaryKey](#P-SqlTableParser-Models-TableColumn-IsPrimaryKey 'SqlTableParser.Models.TableColumn.IsPrimaryKey')
  - [IsPrimaryKeyConstraint](#P-SqlTableParser-Models-TableColumn-IsPrimaryKeyConstraint 'SqlTableParser.Models.TableColumn.IsPrimaryKeyConstraint')
  - [IsPrimaryKeyExplicit](#P-SqlTableParser-Models-TableColumn-IsPrimaryKeyExplicit 'SqlTableParser.Models.TableColumn.IsPrimaryKeyExplicit')
  - [IsRowGuidCol](#P-SqlTableParser-Models-TableColumn-IsRowGuidCol 'SqlTableParser.Models.TableColumn.IsRowGuidCol')
  - [IsUnique](#P-SqlTableParser-Models-TableColumn-IsUnique 'SqlTableParser.Models.TableColumn.IsUnique')
  - [Name](#P-SqlTableParser-Models-TableColumn-Name 'SqlTableParser.Models.TableColumn.Name')
  - [ReferenceColumn](#P-SqlTableParser-Models-TableColumn-ReferenceColumn 'SqlTableParser.Models.TableColumn.ReferenceColumn')
  - [Sql](#P-SqlTableParser-Models-TableColumn-Sql 'SqlTableParser.Models.TableColumn.Sql')
  - [SqlContraintType](#P-SqlTableParser-Models-TableColumn-SqlContraintType 'SqlTableParser.Models.TableColumn.SqlContraintType')

<a name='P-SqlRazorGenerator-TemplateModel-Table'></a>
### Table `property`

##### Summary

Table data see [Table](#T-SqlTableParser-Models-Table 'SqlTableParser.Models.Table').

<a name='P-SqlRazorGenerator-TemplateModel-Tables'></a>
### Tables `property`
Value is only present when generating entity: `--generate Entity`

##### Summary

List of Tables see [Table](#T-SqlTableParser-Models-Table 'SqlTableParser.Models.Table').
Value is only present when generating context: `--generate Context`

<a name='P-SqlRazorGenerator-TemplateModel-Database'></a>
### Database `property`

##### Summary

Database name. Value comes `--database-name MyDatabase`

<a name='P-SqlRazorGenerator-TemplateModel-EnvironmentVariables'></a>
### EnvironmentVariables `property`

##### Summary

Environment variables of type `Dictionary<string, string>`.

<a name='T-SqlTableParser-Models-Table'></a>
## Table `type`

##### Namespace

SqlTableParser.Models

##### Summary

Sql Table.

<a name='P-SqlTableParser-Models-Table-Columns'></a>
### Columns `property`

##### Summary

Table columns.

<a name='P-SqlTableParser-Models-Table-DiagnosticString'></a>
### DiagnosticString `property`

##### Summary

Diagnostic string.

<a name='P-SqlTableParser-Models-Table-HasIdentityColumn'></a>
### HasIdentityColumn `property`

##### Summary

Has identity column.

<a name='P-SqlTableParser-Models-Table-MultiplePrimaryKeys'></a>
### MultiplePrimaryKeys `property`

##### Summary

Has multiple primary keys.

<a name='P-SqlTableParser-Models-Table-Name'></a>
### Name `property`

##### Summary

Name of table.

<a name='P-SqlTableParser-Models-Table-PrimaryKeyColumns'></a>
### PrimaryKeyColumns `property`

##### Summary

Primary key columns.

<a name='P-SqlTableParser-Models-Table-Schema'></a>
### Schema `property`

##### Summary

Schema name.

<a name='P-SqlTableParser-Models-Table-SourceFileName'></a>
### SourceFileName `property`

##### Summary

Source file name.

<a name='P-SqlTableParser-Models-Table-SourceFilePath'></a>
### SourceFilePath `property`

##### Summary

Source file path.

<a name='P-SqlTableParser-Models-Table-SourceFolder'></a>
### SourceFolder `property`

##### Summary

Source folder path.

<a name='P-SqlTableParser-Models-Table-Sql'></a>
### Sql `property`

##### Summary

Original table sql.

<a name='M-SqlTableParser-Models-Table-ContainsDataType-System-String-'></a>
### ContainsDataType(type) `method`

##### Summary

Tells whether or not the table contains a column with the specified data type.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| type | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Sql data type. |

<a name='M-SqlTableParser-Models-Table-ContainsIdentity'></a>
### ContainsIdentity() `method`

##### Summary

Tells whether or not the table contains an identity column.

##### Returns

True if any identity columns found.

##### Parameters

This method has no parameters.

<a name='M-SqlTableParser-Models-Table-ContainsNotNull'></a>
### ContainsNotNull() `method`

##### Summary

Tells whether or not the table contains any not null columns.

##### Returns

True if any not null columns found.

##### Parameters

This method has no parameters.

<a name='M-SqlTableParser-Models-Table-ContainsPrimaryKey'></a>
### ContainsPrimaryKey() `method`

##### Summary

Tells whether or not the table contains a primary key column.

##### Returns

True if any primary columns found.

##### Parameters

This method has no parameters.

<a name='M-SqlTableParser-Models-Table-IsIdColumn-System-String-'></a>
### IsIdColumn(name) `method`

##### Summary

Check if column is an Id column.

##### Returns

True if column is the Id column.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Name of column. |

<a name='M-SqlTableParser-Models-Table-IsPrimaryKey-System-String-'></a>
### IsPrimaryKey(columnName) `method`

##### Summary

Tells whether or not the column is a primary key.

##### Returns

True if column is a primary key.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| columnName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Column name. |

<a name='T-SqlTableParser-Models-TableColumn'></a>
## TableColumn `type`

##### Namespace

SqlTableParser.Models

##### Summary

Table column.

<a name='P-SqlTableParser-Models-TableColumn-Computed'></a>
### Computed `property`

##### Summary

Tells whether or not the column is computed.

<a name='P-SqlTableParser-Models-TableColumn-DataType'></a>
### DataType `property`

##### Summary

Sql data type.

<a name='P-SqlTableParser-Models-TableColumn-DataTypeExtraInfo1'></a>
### DataTypeExtraInfo1 `property`

##### Summary

Sql data type extra info 1.

<a name='P-SqlTableParser-Models-TableColumn-DataTypeExtraInfo2'></a>
### DataTypeExtraInfo2 `property`

##### Summary

Sql data type extra info 2.

<a name='P-SqlTableParser-Models-TableColumn-DiagnosticString'></a>
### DiagnosticString `property`

##### Summary

Diagnostic string.

<a name='P-SqlTableParser-Models-TableColumn-IsDefault'></a>
### IsDefault `property`

##### Summary

Tells whether or not the column has a default value.

<a name='P-SqlTableParser-Models-TableColumn-IsForeignKey'></a>
### IsForeignKey `property`

##### Summary

Tells whether or not the column has a foreign key constraint.

<a name='P-SqlTableParser-Models-TableColumn-IsIdentity'></a>
### IsIdentity `property`

##### Summary

Tells whether or not the column is an identity column.

<a name='P-SqlTableParser-Models-TableColumn-IsIdentityColumnExplicit'></a>
### IsIdentityColumnExplicit `property`

##### Summary

Is identity column explicit.

<a name='P-SqlTableParser-Models-TableColumn-IsIdentityConstraint'></a>
### IsIdentityConstraint `property`

##### Summary

Tells whether or not the column has an identity constraint.

<a name='P-SqlTableParser-Models-TableColumn-IsNotNull'></a>
### IsNotNull `property`

##### Summary

Tells whether or not the column is not nullable.

<a name='P-SqlTableParser-Models-TableColumn-IsNull'></a>
### IsNull `property`

##### Summary

Tells whether or not the column is nullable.

<a name='P-SqlTableParser-Models-TableColumn-IsPrimaryKey'></a>
### IsPrimaryKey `property`

##### Summary

Tells whether or not the column is a primary key.

<a name='P-SqlTableParser-Models-TableColumn-IsPrimaryKeyConstraint'></a>
### IsPrimaryKeyConstraint `property`

##### Summary

Tells whether or not the column has a primary key constraint.

<a name='P-SqlTableParser-Models-TableColumn-IsPrimaryKeyExplicit'></a>
### IsPrimaryKeyExplicit `property`

##### Summary

Is primary key explicit.

<a name='P-SqlTableParser-Models-TableColumn-IsRowGuidCol'></a>
### IsRowGuidCol `property`

##### Summary

Tells whether or not the column is a row guid column.

<a name='P-SqlTableParser-Models-TableColumn-IsUnique'></a>
### IsUnique `property`

##### Summary

Tells whether or not the column is unique.

<a name='P-SqlTableParser-Models-TableColumn-Name'></a>
### Name `property`

##### Summary

Column name.

<a name='P-SqlTableParser-Models-TableColumn-ReferenceColumn'></a>
### ReferenceColumn `property`

##### Summary

References column that contains the data type for functions like ISNULL(...).
Can also be the data type for CONVERT([date], ...)

<a name='P-SqlTableParser-Models-TableColumn-Sql'></a>
### Sql `property`

##### Summary

Original sql for the column.

<a name='P-SqlTableParser-Models-TableColumn-SqlContraintType'></a>
### SqlContraintType `property`

##### Summary

Sql constraint.

