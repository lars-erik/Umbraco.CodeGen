using System.Collections.Generic;

namespace Umbraco.CodeGen.Parsers
{
    public class DefaultParserFactory : ParserFactoryBase
    {
        protected override ContentTypeCodeParser CreateMediaTypeParser()
        {
            var parsers = CreateParsers(new CommonInfoParser(Configuration));
            return new MediaTypeCodeParser(Configuration, parsers.ToArray());
        }

        protected override ContentTypeCodeParser CreateDocumentTypeParser()
        {
            var parsers = CreateParsers(new DocumentTypeInfoParser(Configuration));
            return new DocumentTypeCodeParser(Configuration, parsers.ToArray());
        }

        protected List<ContentTypeCodeParserBase> CreateParsers(InfoParserBase infoParser)
        {
            var parsers = CreateDefaultParsers();
            parsers.Insert(0, infoParser);
            return parsers;
        }

        protected List<ContentTypeCodeParserBase> CreateDefaultParsers()
        {
            return new List<ContentTypeCodeParserBase>
            {
                new StructureParser(Configuration),
                new PropertiesParser(Configuration,
                    new PropertyParser(Configuration, DataTypes)
                ),
                new TabsParser(Configuration)
            };
        }
    }
}
