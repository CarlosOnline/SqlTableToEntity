using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using System;

namespace SqlTableParser
{
    internal static class Extensions
    {
        public static string GetTypeName(this Type type)
        {
            return type.Name.Replace("Microsoft.SqlServer.Management.SqlParser.SqlCodeDom", string.Empty);
        }

        public static string GetDataType(this SqlSimpleWhenClause clause)
        {
            switch (clause.ThenExpression)
            {
                case SqlLiteralExpression expression:
                    return expression.Type.ToString();

                default:
                    return "VARCHAR";
            }
        }

        public static string GetValue(this SqlObjectIdentifier identifier)
        {
            return identifier.ObjectName.Value;
        }

        public static string GetValue(this SqlDataTypeSpecification specification)
        {
            return specification.DataType.ObjectIdentifier.GetValue();
        }
    }
}
