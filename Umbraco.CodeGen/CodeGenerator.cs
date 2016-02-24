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
using Umbraco.CodeGen.Factories;
using Umbraco.CodeGen.Generators;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen
{
    public class CodeGenerator
    {
        private readonly Configuration.GeneratorConfig configuration;
        private readonly CodeGeneratorFactory factory;
        private CodeGeneratorBase generator;

        // TODO: Move to CodeGeneratorConfiguration
        private readonly CodeGeneratorOptions options = new CodeGeneratorOptions
        {
            BlankLinesBetweenMembers = false,
            BracingStyle = "C"
        };

        private static readonly CSharpCodeProvider CodeProvider = new CSharpCodeProvider();

        public CodeGenerator(
            Configuration.GeneratorConfig configuration, 
            CodeGeneratorFactory factory)
        {
            this.configuration = configuration;
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
            if (generator == null)
                generator = factory.Create(configuration);
        }
    }
}
