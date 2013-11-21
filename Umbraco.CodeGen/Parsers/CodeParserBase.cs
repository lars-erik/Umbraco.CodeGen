using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;
using Attribute = ICSharpCode.NRefactory.CSharp.Attribute;

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
            return WithVariableInitializer(fieldVariable, ex => ex.GetText().Trim('"'), defaultValue);
        }

        protected static IEnumerable<string> StringArrayValue(TypeDeclaration type, string fieldName)
        {
            var fieldVariable = FindFieldVariable(type, fieldName);
            return WithVariableInitializer(fieldVariable, StringArrayInitializerToArray, new string[0]);
        }

        protected static IEnumerable<string> TypeArrayValue(TypeDeclaration type, string fieldName)
        {
            var fieldVariable = FindFieldVariable(type, fieldName);
            return WithVariableInitializer(fieldVariable, TypeArrayInitializerToArray, new string[0]);
        }

        protected static IEnumerable<string> StringArrayInitializerToArray(Expression ex)
        {
            return ((ArrayCreateExpression)ex).Initializer.Elements
                .OfType<PrimitiveExpression>()
                .Select(e => e.Value as string)
                .ToArray();
        }

        protected static IEnumerable<string> TypeArrayInitializerToArray(Expression ex)
        {
            return ((ArrayCreateExpression) ex).Initializer.Elements
                .OfType<TypeOfExpression>()
                .Where(e => e.Type is SimpleType)
                .Select(e => ((SimpleType) e.Type).Identifier)
                .ToArray();
        }

        protected static bool BoolFieldValue(TypeDeclaration type, string fieldName)
        {
            var fieldVariable = FindFieldVariable(type, fieldName);
            return WithVariableInitializer(fieldVariable, ex => Convert.ToBoolean(ex.GetText()), false);
        }

        protected static T WithVariableInitializer<T>(VariableInitializer variable, Func<Expression, T> valueGetter, T defaultValue)
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

        protected static string AttributeValue(EntityDeclaration entity, string attributeName, string defaultValue = null)
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

        protected T AttributeArgumentValue<T>(Attribute attribute, string argumentName, T defaultValue)
        {
            var argument = FindAttributeArgument(attribute, argumentName);
            return PrimitiveAttributeArgumentValueOrDefault(argument, defaultValue);
        }

        // TODO: Dry these two
        protected static IEnumerable<string> StringArrayValue(Attribute attribute, string argumentName)
        {
            var argument = FindAttributeArgument(attribute, argumentName);
            if (argument == null) return new string[0];
            var array = argument.Expression as ArrayCreateExpression;
            if (array != null)
                return StringArrayInitializerToArray(array);
            return new string[0];
        }

        protected static IEnumerable<string> TypeArrayValue(Attribute attribute, string argumentName)
        {
            var argument = FindAttributeArgument(attribute, argumentName);
            if (argument == null) return new string[0];
            var array = argument.Expression as ArrayCreateExpression;
            if (array != null)
                return TypeArrayInitializerToArray(array);
            return new string[0];
        }

        private T PrimitiveAttributeArgumentValueOrDefault<T>(NamedExpression argument, T defaultValue)
        {
            if (argument == null)
                return defaultValue;
            var primitive = argument.Expression as PrimitiveExpression;
            if (primitive != null)
                return (T)primitive.Value;
            return defaultValue; // (T)argument.Va
        }

        private static NamedExpression FindAttributeArgument(Attribute attribute, string argumentName)
        {
            if (attribute == null)
                return null;
            return attribute.Arguments.OfType<NamedExpression>().FirstOrDefault(expr => expr.Name == argumentName);
        }
    }
}
