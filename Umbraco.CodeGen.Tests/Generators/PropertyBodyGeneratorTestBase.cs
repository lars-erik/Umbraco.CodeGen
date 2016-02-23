using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Tests.Generators
{
    public abstract class PropertyBodyGeneratorTestBase
    {
        protected string GeneratePropertyAndGetBodyText()
        {
            var property = new PropertyModel {Alias = "aProperty", ClrName = "AProperty"};
            var propNode = new CodeMemberProperty {Type = new CodeTypeReference("String")};
            var generator = CreateGenerator();

            generator.Generate(propNode, property);

            var ns = CodeGenerationHelper.CreateNamespaceWithTypeAndProperty(propNode);
            var builder = CodeGenerationHelper.GenerateCode(ns);

            var code = builder.ToString();
            var returnIndex = code.IndexOf("return");
            var endIndex = code.IndexOf(";", returnIndex);
            var body = code.Substring(returnIndex, endIndex - returnIndex + 1);
            return body;
        }

        protected abstract CodeGeneratorBase CreateGenerator();
    }
}