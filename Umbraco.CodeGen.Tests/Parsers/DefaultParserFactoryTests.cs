using System.Collections.Generic;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    [TestFixture]
    public class DefaultParserFactoryTests
    {
        [Test]
        public void Create_DelegatesConfiguration()
        {
            var factory = new DefaultParserFactory();
            var configuration = new ContentTypeConfiguration(new CodeGeneratorConfiguration(), "DocumentType");
            var dataTypeDefinitions = new List<DataTypeDefinition>();
            var parser = factory.Create(configuration, dataTypeDefinitions);
            Assert.IsInstanceOf<DocumentTypeCodeParser>(parser);
        }
    }
}
