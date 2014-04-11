using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Parsers
{
    public abstract class ParserFactory
    {
        protected ContentTypeConfiguration Configuration;
        protected IList<DataTypeDefinition> DataTypes;

        public virtual ContentTypeCodeParser Create(
            ContentTypeConfiguration configuration,
            IEnumerable<DataTypeDefinition> dataTypes 
            )
        {
            Configuration = configuration;
            DataTypes = dataTypes as List<DataTypeDefinition> ?? dataTypes.ToList();
            var typedParser = configuration.ContentTypeName == "DocumentType" 
                                  ? CreateDocumentTypeParser() 
                                  : CreateMediaTypeParser();
            return typedParser;
        }

        public abstract ContentTypeCodeParser CreateMediaTypeParser();
        public abstract ContentTypeCodeParser CreateDocumentTypeParser();
    }
}