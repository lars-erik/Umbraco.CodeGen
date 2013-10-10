using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Tests.Definitions
{
    [TestFixture]
    public class ContentTypeSerializerTests
    {
        [Test]
        public void Deserialize_DocumentType_ReturnsDocumentType()
        {
            TestDeserialize("SomeDocumentType", CreateExpectedDocumentType());
        }

        [Test]
        public void Deserialize_MediaType_ReturnsMediaType()
        {
            TestDeserialize("SomeMediaType", CreateExpectedMediaType());
        }

        [Test]
        public void Serialize_DocumentType_WritesDocumentType()
        {
            TestSerialize(CreateExpectedDocumentType(), "SomeDocumentType");
        }

        [Test]
        public void Serialize_MediaType_WritesDocumentType()
        {
            TestSerialize(CreateExpectedMediaType(), "SomeMediaType");
        }

        private static void TestDeserialize<T>(string filename, T expectedContentType)
            where T : ContentType
        {
            T actual;
            var actualBuilder = new StringBuilder();
            var expectedBuilder = new StringBuilder();
            using (var reader = File.OpenText(string.Format(@"..\..\TestFiles\{0}.xml", filename)))
            {
                var serializer = new ContentTypeSerializer();
                var content = serializer.Deserialize(reader);
                actual = (T) content;
            }
            BclSerialize(actualBuilder, actual);
            BclSerialize(expectedBuilder, expectedContentType);
            Assert.AreEqual(expectedBuilder.ToString(), actualBuilder.ToString());
        }

        private void TestSerialize(ContentType contentType, string goldFileName)
        {
            string expectedXml;
            using (var reader = File.OpenText(string.Format(@"..\..\TestFiles\{0}.xml", goldFileName)))
            {
                expectedXml = reader.ReadToEnd();
            }

            var serializer = new ContentTypeSerializer();
            var actualXml = serializer.Serialize(contentType);
            Assert.AreEqual(expectedXml, actualXml);
        }

        private static void BclSerialize<T>(StringBuilder builder, T contentType)
        {
            var writer = new StringWriter(builder);
            var xmlSerializer = new XmlSerializer(typeof (T));
            xmlSerializer.Serialize(writer, contentType);
        }

        private DocumentType CreateExpectedDocumentType()
        {
            return new DocumentType
            {
                Info = new DocumentTypeInfo
                {
                    Name = "Some document type",
                    Alias = "someDocumentType",
                    Icon = "privateMemberIcon.gif",
                    Thumbnail = "privateMemberThumb.png",
                    Description = "A description of some document type",
                    AllowAtRoot = true,
                    Master = null,
                    AllowedTemplates = new List<string>
                    {
                        "ATemplate",
                        "AnotherTemplate"
                    },
                    DefaultTemplate = "ATemplate"
                },
                Structure = new List<string>
                {
                    "SomeOtherDocType"
                },
                GenericProperties = new List<GenericProperty>
                {
                    new GenericProperty
                    {
                        Name = "Some property",
                        Alias = "someProperty",
                        Type = "5e9b75ae-face-41c8-b47e-5f4b0fd82f83",
                        Definition = "ca90c950-0aff-4e72-b976-a30b1ac57dad",
                        Tab = "A tab",
                        Mandatory = true,
                        Validation = "[a-z]",
                        Description = "A description"
                    },
                    new GenericProperty
                    {
                        Name = "Another property",
                        Alias = "anotherProperty",
                        Type = "5e9b75ae-face-41c8-b47e-5f4b0fd82f83",
                        Definition = "ca90c950-0aff-4e72-b976-a30b1ac57dad",
                        Tab = "A tab",
                        Description = "Another description"
                    },
                    new GenericProperty
                    {
                        Name = "Tabless property",
                        Alias = "tablessProperty",
                        Type = "1413afcb-d19a-4173-8e9a-68288d2a73b8",
                        Definition = "2e6d3631-066e-44b8-aec4-96f09099b2b5"
                    }
                },
                Tabs = new List<Tab>
                {
                    new Tab
                    {
                        Id = 0,
                        Caption = "A tab"
                    }
                }
            };
        }

        private MediaType CreateExpectedMediaType()
        {
            return new MediaType
            {
                Info = new Info
                {
                    Name = "Inherited Media Folder",
                    Alias = "inheritedMediaFolder",
                    Icon = "folder.gif",
                    Thumbnail = "folder.png",
                    AllowAtRoot = true,
                    Master = "Folder"
                },
                Structure = new List<string>
                {
                    "Folder",
                    "Image",
                    "File",
                    "InheritedMediaFolder"
                },
                GenericProperties = new List<GenericProperty>
                {
                    new GenericProperty
                    {
                        Name = "LetsHaveAProperty",
                        Alias = "letsHaveAProperty",
                        Type = "ec15c1e5-9d90-422a-aa52-4f7622c63bea",
                        Definition = "0cc0eba1-9960-42c9-bf9b-60e150b429ae",
                        Tab = "A tab"
                    },
                    new GenericProperty
                    {
                        Name = "And a tabless property",
                        Alias = "andATablessProperty",
                        Type = "ec15c1e5-9d90-422a-aa52-4f7622c63bea",
                        Definition = "0cc0eba1-9960-42c9-bf9b-60e150b429ae",
                    }
                },
                Tabs = new List<Tab>
                {
                    new Tab
                    {
                        Id = 0,
                        Caption = "A tab"
                    }
                }
            };
        }
    }
}
