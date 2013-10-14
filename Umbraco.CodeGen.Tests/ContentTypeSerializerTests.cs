using System.IO;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class ContentTypeSerializerTests
    {
        [Test]
        public void Deserialize_DocumentType_ReturnsDocumentType()
        {
            TestDeserialize("SomeDocumentType", TestFactory.CreateExpectedDocumentType());
        }

        [Test]
        public void Deserialize_MediaType_ReturnsMediaType()
        {
            TestDeserialize("SomeMediaType", TestFactory.CreateExpectedMediaType());
        }

        [Test]
        public void Serialize_DocumentType_WritesDocumentType()
        {
            TestSerialize(TestFactory.CreateExpectedDocumentType(), "SomeDocumentType");
        }

        [Test]
        public void Serialize_MediaType_WritesDocumentType()
        {
            TestSerialize(TestFactory.CreateExpectedMediaType(), "SomeMediaType");
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
            SerializationHelper.BclSerialize(actualBuilder, actual);
            SerializationHelper.BclSerialize(expectedBuilder, expectedContentType);
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
    }
}
