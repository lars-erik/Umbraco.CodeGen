using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Parsers.Bcl
{
    public class CommonInfoParser : InfoParserBase
    {
        public CommonInfoParser(ContentTypeConfiguration configuration) : base(configuration)
        {
        }

        protected override void OnParseInfo(TypeDeclaration type, Definitions.ContentType definition)
        {
            var info = definition.Info;
            info.Alias = type.Name.PascalCase();
            info.Master = FindMaster(type, Configuration).PascalCase();

            info.Name = AttributeValue(type, "DisplayName", type.Name.SplitPascalCase());
            info.Description = AttributeValue(type, "Description", null);
            info.AllowAtRoot = BoolFieldValue(type, "AllowAtRoot");
            info.Icon = StringFieldValue(type, "icon", "folder.gif");
            info.Thumbnail = StringFieldValue(type, "thumbnail", "folder.png");
        }
    }
}
