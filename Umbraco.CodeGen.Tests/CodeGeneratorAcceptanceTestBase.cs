using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Tests.TestHelpers;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Tests
{
    public abstract class CodeGeneratorAcceptanceTestBase
    {
        protected void TestBuildCode(string fileName, string contentTypeName)
        {
            TestBuildCode(fileName, fileName, contentTypeName);
        }

        protected void TestBuildCode(string classFileName, string xmlFileName, string contentTypeName)
        {
            ContentType contentType;
            var expectedOutput = "";
            using (var inputReader = File.OpenText(@"..\..\TestFiles\" + xmlFileName + ".xml"))
            {
                contentType = new ContentTypeSerializer().Deserialize(inputReader);
            }
            using (var goldReader = File.OpenText(@"..\..\TestFiles\" + classFileName + ".cs"))
            {
                expectedOutput = goldReader.ReadToEnd();
            }

            var configuration = CodeGeneratorConfiguration.Create();
            var typeConfig = configuration.Get(contentTypeName);
            typeConfig.BaseClass = "Umbraco.Core.Models.TypedModelBase";
            typeConfig.Namespace = "Umbraco.CodeGen.Models";

            configuration.TypeMappings.Add(new TypeMapping("Umbraco.Integer", "Int32"));

            OnConfiguring(configuration, contentTypeName);

            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            var dataTypeProvider = new TestDataTypeProvider();
            var generator = new CodeGenerator(typeConfig, dataTypeProvider, CreateGeneratorFactory());

            throw new Exception("Aint passing type model here yet, since serialization disappears");
            generator.Generate(null, writer);

            writer.Flush();
            Console.WriteLine(sb.ToString());

            Assert.AreEqual(expectedOutput, sb.ToString());
        }

        protected abstract CodeGeneratorFactory CreateGeneratorFactory();

        protected virtual void OnConfiguring(CodeGeneratorConfiguration configuration, string contentTypeName)
        {
            
        }

        protected void TestBuildCode(string classFileName, TypeModel contentType, string contentTypeName)
        {
            string expectedOutput;
            using (var goldReader = File.OpenText(@"..\..\TestFiles\" + classFileName + ".cs"))
            {
                expectedOutput = goldReader.ReadToEnd();
            }

            var configuration = CodeGeneratorConfiguration.Create();
            var typeConfig = configuration.Get(contentTypeName);
            typeConfig.BaseClass = "Umbraco.Core.Models.TypedModelBase";
            typeConfig.Namespace = "Umbraco.CodeGen.Models";

            configuration.TypeMappings.Add(new TypeMapping("Umbraco.Integer", "Int32"));

            OnConfiguring(configuration, contentTypeName);

            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            var dataTypeProvider = new TestDataTypeProvider();
            var generator = new CodeGenerator(typeConfig, dataTypeProvider, CreateGeneratorFactory());

            generator.Generate(contentType, writer);

            writer.Flush();

            //Debug.Write("\n\n------\n");
            //Console.Write(sb.ToString().Replace("\r\n", "\n"));

            Assert.AreEqual(expectedOutput, sb.ToString());
        }
    }
}