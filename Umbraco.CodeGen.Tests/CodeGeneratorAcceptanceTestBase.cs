using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Factories;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Tests.TestHelpers;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Tests
{
    public abstract class CodeGeneratorAcceptanceTestBase
    {
        protected abstract CodeGeneratorFactory CreateGeneratorFactory();

        protected virtual void OnConfiguring(GeneratorConfig configuration, string contentTypeName)
        {
            
        }

        protected void TestBuildCode(string classFileName, TypeModel contentType, string contentTypeName)
        {
            string expectedOutput;
            using (var goldReader = File.OpenText(@"..\..\TestFiles\" + classFileName + ".cs"))
            {
                expectedOutput = goldReader.ReadToEnd();
            }

            var config = new GeneratorConfig
            {
                BaseClass = typeof (PublishedContentModel),
                Namespace = "Umbraco.CodeGen.Models"
            };

            OnConfiguring(config, contentTypeName);

            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            var generator = new CodeGenerator(config, CreateGeneratorFactory());

            generator.Generate(contentType, writer);

            writer.Flush();

            Assert.AreEqual(expectedOutput, sb.ToString());
        }
    }
}