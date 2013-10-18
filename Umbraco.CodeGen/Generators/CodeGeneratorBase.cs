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

        public abstract void Generate(CodeObject codeObject, Entity entity);

        protected static void AddAttribute(CodeTypeMember type, string attributeName, string value)
        {
            var attribute = CreatePrimitiveAttribute(attributeName, value);
            AddAttribute(type, attribute);
        }

        protected void AddAttribute(CodeMemberProperty propNode, string attributeName)
        {
            AddAttribute(propNode, CreateAttribute(attributeName));
        }

        private static void AddAttribute(CodeTypeMember type, CodeAttributeDeclaration attribute)
        {
            type.CustomAttributes.Add(attribute);
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
    }
}