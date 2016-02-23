using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen
{
    public class CodeGenerator
    {
        private readonly ContentTypeConfiguration contentTypeConfiguration;
        private readonly IDataTypeProvider dataTypeProvider;
        private readonly CodeGeneratorFactory factory;
        private IList<DataTypeDefinition> dataTypes;
        private CodeGeneratorBase generator;

        // TODO: Move to CodeGeneratorConfiguration
        private readonly CodeGeneratorOptions options = new CodeGeneratorOptions
        {
            BlankLinesBetweenMembers = false,
            BracingStyle = "C"
        };

        private static readonly CSharpCodeProvider CodeProvider = new CSharpCodeProvider();

        public CodeGenerator(
            ContentTypeConfiguration contentTypeConfiguration, 
            IDataTypeProvider dataTypeProvider, 
            CodeGeneratorFactory factory)
        {
            this.contentTypeConfiguration = contentTypeConfiguration;
            this.dataTypeProvider = dataTypeProvider;
            this.factory = factory;
        }

        public void Generate(TypeModel contentType, TextWriter writer)
        {
            EnsureGenerator();

            //var compileUnit = new CodeCompileUnit();
            //generator.Generate(compileUnit, contentType);
            //CodeProvider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
            var ns = new CodeNamespace();
            generator.Generate(ns, contentType);
            CodeProvider.GenerateCodeFromNamespace(ns, writer, options);

            writer.Flush();
        }

        private void EnsureGenerator()
        {
            if (dataTypes == null)
                dataTypes = dataTypeProvider.GetDataTypes().ToList();

            if (generator == null)
                generator = factory.Create(contentTypeConfiguration, dataTypes);
        }
    }
}
