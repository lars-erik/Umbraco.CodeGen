using ICSharpCode.NRefactory.CSharp;

namespace Umbraco.CodeGen.Parsers
{
    public class CommonInfoParser : InfoCodeParser
    {
        public CommonInfoParser(ContentTypeConfiguration configuration) : base(configuration)
        {
        }

        protected override void OnParseInfo(TypeDeclaration type, Definitions.ContentType definition)
        {
            var info = definition.Info;
            info.Alias = type.Name.CamelCase();
            info.Name = AttributeValue(type, "DisplayName", type.Name.SplitPascalCase());
            info.Description = AttributeValue(type, "Description", null);
            info.Master = FindMaster(type, Configuration).CamelCase();
            info.AllowAtRoot = BoolFieldValue(type, "AllowAtRoot");
            info.Icon = StringFieldValue(type, "icon", "folder.gif");
            info.Thumbnail = StringFieldValue(type, "thumbnail", "folder.png");
        }
    }
}
