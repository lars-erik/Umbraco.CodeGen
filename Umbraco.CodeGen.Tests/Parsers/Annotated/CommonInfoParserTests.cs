using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers.Annotated;

namespace Umbraco.CodeGen.Tests.Parsers.Annotated
{
    [TestFixture]
    public class CommonInfoParserTests : ContentTypeCodeParserTestBase
    {
        [SetUp]
        public void SetUp()
        {
            Configuration = CodeGeneratorConfiguration.Create().MediaTypes;
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
        public void Parse_Name_WhenDisplayNameArgument_IsAttributeValue()
        {
            const string code = @"
                [MediaType(DisplayName=""It's another name"")]
                public class ItsAName {}
            ";
            Parse(code);
            Assert.AreEqual("It's another name", Info.Name);
        }

        [Test]
        public void Parse_Description_WhenDescriptionArgument_IsAttributeValue()
        {
            const string code = @"
                [MediaType(Description=""It's a description"")]
                public class AClass {}
            ";
            Parse(code);
            Assert.AreEqual("It's a description", Info.Description);
        }

        [Test]
        public void Parse_Description_WhenArgumentMissing_IsNull()
        {
            Parse(EmptyClass);
            Assert.IsNull(Info.Description);
        }

        [Test]
        public void Parse_Alias_IsPascalCase()
        {
            const string code = @"
                public class ItShouldBePascal{}
            ";
            Parse(code);
            Assert.AreEqual("ItShouldBePascal", Info.Alias);
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
        public void Parse_Master_WhenNonRootBaseClass_IsBaseClassPascalCased()
        {
            const string expectedBaseClass = "ExpectedBaseClass";
            const string code = @"
                public class AClass : ExpectedBaseClass {}
            ";
            Parse(code);
            Assert.AreEqual(expectedBaseClass.PascalCase(), Info.Master);
        }

        [Test]
        public void Parse_AllowAtRoot_WhenTrueArgument_IsTrue()
        {
            const string code = @"
                [MediaType(AllowAtRoot=true)]
                public class AClass {
                }
            ";
            Parse(code);
            Assert.IsTrue(Info.AllowAtRoot);
        }

        [Test]
        public void Parse_AllowAtRoot_WhenFalseArgument_IsFalse()
        {
            const string code = @"
                [MediaType(AllowAtRoot=false)]
                public class AClass {
                }
            ";
            Parse(code);
            Assert.IsFalse(Info.AllowAtRoot);
        }

        [Test]
        public void Parse_AllowAtRoot_WhenMissing_IsFalse()
        {
            Parse(EmptyClass);
            Assert.IsFalse(Info.AllowAtRoot);
        }

        [Test]
        [TestCase("Icon", "anIcon.gif")]
        [TestCase("Thumbnail", "folder.png")]
        public void Parse_Argument_HasValue(
            string memberName,
            string memberValue
        )
        {
            var code = @"
                [MediaType(" + memberName + @" = """ + memberValue + @""")]
                public class AClass {
                }
            ";
            Parse(code);
            var value = typeof(DocumentTypeInfo).GetProperty(memberName).GetValue(Info, null);
            Assert.AreEqual(memberValue, value);
        }

        [Test]
        [TestCase("Icon", "folder.gif")]
        [TestCase("Thumbnail", "folder.png")]
        public void Parse_Argument_IsNullOrMissing_HasDefaultValue(
            string memberName,
            object expectedValue
        )
        {
            var code = new[]{
                EmptyClass
                , @"
                [MediaType(" + memberName + @" = null)]
                public class AClass {
                }
            "};
            foreach (var snippet in code)
            {
                Parse(snippet);
                var value = PropertyValue(memberName);
                Assert.AreEqual(expectedValue, value);
            }
        }
    }
}
