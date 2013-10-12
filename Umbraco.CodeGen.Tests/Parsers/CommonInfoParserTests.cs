using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    [TestFixture]
    public class CommonInfoParserTests : InfoParserTestBase
    {
        [SetUp]
        public void SetUp()
        {
            Configuration = new ContentTypeConfiguration(null);
            Parser = new CommonInfoParser(Configuration);
            ContentType = new MediaType();
            Info = ContentType.Info;
        }

        [Test]
        public void Parse_Name_WhenPureClassName_IsSplitPascal()
        {
            const string code = @"
                public class ItsAName {}
            ";
            Parse(code);
            Assert.AreEqual("Its A Name", Info.Name);
        }

        [Test]
        public void Parse_Name_WhenDisplayNameAttribute_IsAttributeValue()
        {
            const string code = @"
                [DisplayName(""It's another name"")]
                public class ItsAName {}
            ";
            Parse(code);
            Assert.AreEqual("It's another name", Info.Name);
        }

        [Test]
        public void Parse_Description_WhenDescriptionAttribute_IsAttributeValue()
        {
            const string code = @"
                [Description(""It's a description"")]
                public class AClass {}
            ";
            Parse(code);
            Assert.AreEqual("It's a description", Info.Description);
        }

        [Test]
        public void Parse_Description_WhenAttributeMissing_IsNull()
        {
            Parse(EmptyClass);
            Assert.IsNull(Info.Description);
        }

        [Test]
        public void Parse_Alias_IsCamelCase()
        {
            const string code = @"
                public class ItShouldBeCamel{}
            ";
            Parse(code);
            Assert.AreEqual("itShouldBeCamel", Info.Alias);
        }

        [Test]
        public void Parse_Master_WhenNoBaseClass_IsNull()
        {
            Parse(EmptyClass);
            Assert.IsNull(Info.Master);
        }

        [Test]
        public void Parse_Master_WhenRootBaseClass_IsNull()
        {
            const string rootBaseClass = "RootBaseClass";
            const string code = @"
                public class AClass : RootBaseClass {}
            ";
            Configuration.BaseClass = rootBaseClass;
            Parse(code);
            Assert.IsNull(Info.Master);
        }

        [Test]
        public void Parse_Master_WhenNonRootBaseClass_IsBaseClassCascalCased()
        {
            const string expectedBaseClass = "ExpectedBaseClass";
            const string code = @"
                public class AClass : ExpectedBaseClass {}
            ";
            Parse(code);
            Assert.AreEqual(expectedBaseClass.CamelCase(), Info.Master);
        }

        [Test]
        public void AllowAtRoot_WhenTrueMember_IsTrue()
        {
            const string code = @"
                public class AClass {
                    bool allowAtRoot = true;
                }
            ";
            Parse(code);
            Assert.IsTrue(Info.AllowAtRoot);
        }

        [Test]
        public void AllowAtRoot_WhenFalse_IsFalse()
        {
            const string code = @"
                public class AClass {
                    bool allowAtRoot = false;
                }
            ";
            Parse(code);
            Assert.IsFalse(Info.AllowAtRoot);
        }

        [Test]
        public void AllowAtRoot_WhenMissing_IsFalse()
        {
            Parse(EmptyClass);
            Assert.IsFalse(Info.AllowAtRoot);
        }

        [Test]
        [TestCase("Icon", "anIcon.gif")]
        [TestCase("Thumbnail", "folder.png")]
        public void Parse_StringMember_HasValue(
            string memberName, 
            string memberValue
        )
        {
            var code = @"
                public class AClass {
                    const string " + memberName + @" = """ + memberValue + @""";
                }
            ";
            Parse(code);
            var value = typeof (DocumentTypeInfo).GetProperty(memberName).GetValue(Info, null);
            Assert.AreEqual(memberValue, value);
        }

        [Test]
        [TestCase("Icon", "folder.gif")]
        [TestCase("Thumbnail", "folder.png")]
        public void Parse_Member_IsNullOrMissing_HasDefaultValue(
            string memberName, 
            object expectedValue
        )
        {
            var code = new[]{
                EmptyClass
                , @"
                public class AClass {
                    const string " + memberName + @" = null;
                }
            "};
            foreach(var snippet in code)
            {
                Parse(snippet);
                var value = PropertyValue(memberName);
                Assert.AreEqual(expectedValue, value);
            }
        }
    }
}
