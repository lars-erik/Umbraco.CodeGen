using System.CodeDom;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class CtorGeneratorTests
    {
        [Test]
        public void Generate_AddsConstructorWithBaseCall_PassingIPublishedContentParameter()
        {
            var type = new CodeTypeDeclaration{Name="AName"};
            type.BaseTypes.Add("ABaseType");
            var generator = new CtorGenerator(null);
            generator.Generate(type, new MediaType());

            var ns = CodeGenerationHelper.CreateNamespaceWithType(type);
            var code = CodeGenerationHelper.GenerateCode(ns);

            Assert.AreEqual(
            @"namespace ANamespace {
    
    
    public class AName : ABaseType {
        
        public AName(IPublishedContent content) : 
                base(content) {
        }
    }
}
", code.ToString());
        }
    }
}
