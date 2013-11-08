using System;
using System.CodeDom;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public abstract class CodeGeneratorBase
    {
        protected const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
        protected ContentTypeConfiguration Config;

        protected CodeGeneratorBase(ContentTypeConfiguration config)
        {
            Config = config;
        }

        public abstract void Generate(object codeObject, Entity entity);

        protected static void AddAttribute(CodeTypeMember type, string attributeName, string value)
        {
            var attribute = CreatePrimitiveAttribute(attributeName, value);
            AddAttribute(type, attribute);
        }

        protected CodeAttributeDeclaration AddAttribute(CodeTypeMember codeObject, string attributeName)
        {
            var attribute = CreateAttribute(attributeName);
            AddAttribute(codeObject, attribute);
            return attribute;
        }

        protected static void AddAttribute(CodeTypeMember codeObject, CodeAttributeDeclaration attribute)
        {
            codeObject.CustomAttributes.Add(attribute);
        }

        private static CodeAttributeDeclaration CreatePrimitiveAttribute<T>(string attributeName, T value)
        {
            var arguments = new CodeAttributeArgument(new CodePrimitiveExpression(value));
            return CreateAttribute(attributeName, arguments);
        }

        private static CodeAttributeDeclaration CreateAttribute(string attributeName, params CodeAttributeArgument[] arguments)
        {
            return new CodeAttributeDeclaration(
                attributeName,
                arguments
            );
        }

        protected void AddFieldIfNotEmpty(CodeTypeDeclaration type, string fieldName, string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return;
            AddField(type, fieldName, value);
        }

        protected void AddFieldIfTrue(CodeTypeDeclaration type, string fieldName, bool value)
        {
            if (value)
                AddField(type, fieldName, true);
        }

        private static void AddField<T>(CodeTypeDeclaration type, string fieldName, T value)
        {
            var field = new CodeMemberField(typeof (T), fieldName)
            {
                InitExpression = new CodePrimitiveExpression(value)
            };
            type.Members.Add(field);
        }

        protected static void AddAttributeArgumentIfValue(CodeAttributeDeclaration attribute, string argumentName, string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
                AddAttributePrimitiveArgument(attribute, argumentName, value);
        }

        protected static void AddAttributePrimitiveArgument(CodeAttributeDeclaration attribute, string argumentName, object value)
        {
            var argumentValue = new CodePrimitiveExpression(value);
            AddAttributeArgument(attribute, argumentName, argumentValue);
        }

        protected static void AddAttributeArgument(CodeAttributeDeclaration attribute, string argumentName,
            CodeExpression argumentValue)
        {
            attribute.Arguments.Add(
                new CodeAttributeArgument(
                    argumentName,
                    argumentValue
                    )
                );
        }
    }
}