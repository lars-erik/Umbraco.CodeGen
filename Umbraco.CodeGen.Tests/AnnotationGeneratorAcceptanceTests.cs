using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Generators.Annotated;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class AnnotationGeneratorAcceptanceTests : CodeGeneratorFactory
    {
        private Func<ContentTypeConfiguration, IEnumerable<DataTypeDefinition>, CodeGeneratorBase> factory;
            
        [Test]
        public void BuildCode_GeneratesCodeForDocumentType()
        {
            var contentType = TestFactory.CreateExpectedDocumentType();
            factory = CreateDocTypeGenerator;

            Generate(contentType);
        }

        [Test]
        public void BuildCode_GeneratesCodeForMediaType()
        {
            var contentType = TestFactory.CreateExpectedMediaType();
            factory = CreateMediaTypeGenerator;
            contentType.Info.Description = "Oy, need a description to boot!";

            Generate(contentType);
        }

        private void Generate(ContentType contentType)
        {
            var configuration = new CodeGeneratorConfiguration().DocumentTypes;
            var dataTypeProvider = new TestDataTypeProvider();

            var generator = new CodeGenerator(configuration, dataTypeProvider, this);

            var stringBuilder = new StringBuilder();
            var writer = new StringWriter(stringBuilder);

            configuration.Namespace = "A.Namespace";
            configuration.BaseClass = "ABaseClass";

            generator.Generate(contentType, writer);

            Console.WriteLine(stringBuilder.ToString());
            Assert.Inconclusive("Not finished yet");
        }

        public override CodeGeneratorBase Create(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return factory(configuration, dataTypes);
        }

        public CodeGeneratorBase CreateDocTypeGenerator(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return new NamespaceGenerator(
                configuration,
                new ImportsGenerator(configuration),
                new ClassGenerator(configuration,
                    new CompositeCodeGenerator(
                        configuration,
                        new EntityNameGenerator(configuration),
                        new AttributeCodeGenerator(
                            "DocumentType",
                            configuration,
                            new EntityDescriptionGenerator(configuration),
                            new CommonInfoGenerator(configuration),
                            new DocumentTypeInfoGenerator(configuration)
                        )
                    ),
                    new CtorGenerator(configuration),
                    new PropertiesGenerator(
                        configuration,
                        new PropertyDeclarationGenerator(
                            configuration,
                            dataTypes.ToList(),
                            new EntityNameGenerator(configuration),
                            new AttributeCodeGenerator(
                                "GenericProperty",
                                configuration,
                                new EntityDescriptionGenerator(configuration)
                                ),
                            new PropertyBodyGenerator(configuration)
                           )
                        )
                    )
                );
        }
        public CodeGeneratorBase CreateMediaTypeGenerator(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return new NamespaceGenerator(
                configuration,
                new ImportsGenerator(configuration),
                new ClassGenerator(configuration,
                    new CompositeCodeGenerator(
                        configuration,
                        new EntityNameGenerator(configuration),
                        new AttributeCodeGenerator(
                            "MediaType",
                            configuration,
                            new EntityDescriptionGenerator(configuration),
                            new CommonInfoGenerator(configuration)
                        )
                    ),
                    new CtorGenerator(configuration),
                    new PropertiesGenerator(
                        configuration,
                        new PropertyDeclarationGenerator(
                            configuration,
                            dataTypes.ToList(),
                            new EntityNameGenerator(configuration),
                            new AttributeCodeGenerator(
                                "GenericProperty",
                                configuration,
                                new EntityDescriptionGenerator(configuration)
                                ),
                            new PropertyBodyGenerator(configuration)
                           )
                        )
                    )
                );
        }
    }
}
