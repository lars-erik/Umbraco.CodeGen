using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;

namespace Umbraco.CodeGen.Parsers
{
    public abstract class CodeParserBase
    {
        protected const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

        protected static string FindMaster(TypeDeclaration type, ContentTypeConfiguration configuration)
        {
            var baseType = type.BaseTypes.FirstOrDefault() as SimpleType;
            if (baseType == null || baseType.Identifier == configuration.BaseClass)
                return null;
            return baseType.Identifier;
        }

        protected static string FindDataTypeDefinitionId(EntityDeclaration prop, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return dataTypes.Where(dt => dt.DefinitionId == AttributeValue(prop, "DataType", Guid.Empty.ToString())).Select(dt => dt.DataTypeId).SingleOrDefault();
        }
        
        protected static IEnumerable<PropertyDeclaration> FindProperties(TypeDeclaration type)
        {
            return type.Descendants.OfType<PropertyDeclaration>();
        }

        protected static string StringFieldValue(TypeDeclaration type, string fieldName, string defaultValue = null)
        {
            var fieldVariable = FindFieldVariable(type, fieldName);
            return WithInitializer(fieldVariable, ex => ex.GetText().Trim('"'), defaultValue);
        }

        protected static IEnumerable<string> StringArrayValue(TypeDeclaration type, string fieldName)
        {
            var fieldVariable = FindFieldVariable(type, fieldName);
            return WithInitializer(fieldVariable, ex =>
                ((ArrayCreateExpression)ex).Initializer.Elements
                    .OfType<PrimitiveExpression>()
                    .Select(e => e.Value as string)
                    .ToArray(),
                new string[0]
            );
        }

        protected static IEnumerable<string> TypeArrayValue(TypeDeclaration type, string fieldName)
        {
            var fieldVariable = FindFieldVariable(type, fieldName);
            return WithInitializer(fieldVariable, ex =>
                ((ArrayCreateExpression)ex).Initializer.Elements
                    .OfType<TypeOfExpression>()
                    .Where(e => e.Type is SimpleType)
                    .Select(e => ((SimpleType)e.Type).Identifier)
                    .ToArray(),
                new string[0]
            );
        }

        protected static bool BoolFieldValue(TypeDeclaration type, string fieldName)
        {
            var fieldVariable = FindFieldVariable(type, fieldName);
            return WithInitializer(fieldVariable, ex => Convert.ToBoolean(ex.GetText()), false);
        }

        protected static T WithInitializer<T>(VariableInitializer variable, Func<Expression, T> valueGetter, T defaultValue)
        {
            if (variable == null || variable.Initializer == null || variable.Initializer.GetText() == "null")
                return defaultValue;
            return valueGetter(variable.Initializer);
        }

        protected static VariableInitializer FindFieldVariable(TypeDeclaration type, string fieldName)
        {
            var fieldVariable =
                type.Members.OfType<FieldDeclaration>()
                    .SelectMany(f => f.Variables)
                    .SingleOrDefault(m => String.Compare(m.Name, fieldName, IgnoreCase) == 0);
            return fieldVariable;
        }

        protected static string AttributeValue(EntityDeclaration entity, string attributeName, string defaultValue = "")
        {
            var attribute = FindAttribute(entity.Attributes, attributeName);
            return AttributeValueOrDefault(attribute, defaultValue);
        }

        protected static string AttributeValueOrDefault(ICSharpCode.NRefactory.CSharp.Attribute attribute, string defaultValue)
        {
            var value = attribute != null
                ? attribute.Arguments.Select(arg => arg.GetText().Trim('"')).FirstOrDefault()
                : defaultValue;
            return value;
        }

        protected static ICSharpCode.NRefactory.CSharp.Attribute FindAttribute(IEnumerable<AttributeSection> attributeSections, string attributeName)
        {
            return attributeSections
                .SelectMany(att => att.Attributes)
                .Where(att => att.Type is SimpleType)
                .SingleOrDefault(att => ((SimpleType)att.Type).Identifier == attributeName);
        }
    }
}
