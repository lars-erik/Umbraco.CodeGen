using System.CodeDom;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class CommonInfoGeneratorTests : TypeCodeGeneratorTestBase
    {
        private Info info;

        [SetUp]
        public void SetUp()
        {
            Configuration = new CodeGeneratorConfiguration().MediaTypes;
            Generator = new CommonInfoGenerator(Configuration);
            ContentType = new MediaType { Info = { Alias = "anEntity" } };
            Candidate = Type = new CodeTypeDeclaration();
            info = ContentType.Info;
        }

        [Test]
        public void Generate_Icon_WhenHasValue_IsFieldWithValue()
        {
            info.Icon = "icon.gif";
            Generate();
            Assert.AreEqual("icon.gif", PrimitiveFieldValue("icon"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Icon_WhenNullOrEmpty_IsIgnored(string value)
        {
            Generate();
            Assert.IsNull(FindField("icon"));
        }

        [Test]
        public void Generate_Thumbnail_WhenHasValue_IsFieldWithValue()
        {
            info.Thumbnail = "thumb.png";
            Generate();
            Assert.AreEqual("thumb.png", PrimitiveFieldValue("thumbnail"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Thumbnail_WhenNullOrEmpty_IsIgnored(string value)
        {
            Generate();
            Assert.IsNull(FindField("thumbnail"));
        }

        [Test]
        public void Generate_AllowAtRoot_WhenTrue_IsFieldValue()
        {
            info.AllowAtRoot = true;
            Generate();
            Assert.IsTrue((bool)PrimitiveFieldValue("allowAtRoot"));
        }

        [Test]
        public void Generate_AllowAtRoot_WhenTrue_IsOmitted()
        {
            Generate();
            Assert.IsNull(FindField("allowAtRoot"));
        }

        private void Generate()
        {
            Generator.Generate(Type, ContentType);
        }
    }
}
