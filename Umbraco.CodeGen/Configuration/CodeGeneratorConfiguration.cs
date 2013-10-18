using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Umbraco.CodeGen.Configuration
{
	public class ContentTypeConfiguration
	{
	    internal CodeGeneratorConfiguration config;

        [XmlAttribute]
		public string ModelPath { get; set; }
        [XmlAttribute]
        public string BaseClass { get; set; }
        [XmlAttribute]
        public bool GenerateClasses { get; set; }
        [XmlAttribute]
        public bool GenerateXml { get; set; }
        [XmlAttribute]
        public string Namespace { get; set; }
        
        [XmlIgnore]
        public string ContentTypeName { get; set; }

        [XmlIgnore]
        public string DefaultTypeMapping { get { return config.TypeMappings.DefaultType; } }
        [XmlIgnore]
        public string DefaultDefinitionId { get { return config.TypeMappings.DefaultDefinitionId; } }
        [XmlIgnore]
        public TypeMappings TypeMappings { get { return config.TypeMappings; } }

        [XmlIgnore]
	    public CodeGeneratorConfiguration Config
	    {
	        get { return config; }
	    }

	    public ContentTypeConfiguration(CodeGeneratorConfiguration config, string contentTypeName)
	    {
	        ContentTypeName = contentTypeName;
	        this.config = config;
	    }

	    public ContentTypeConfiguration()
	    {
	        
	    }
	}

    [XmlRoot("CodeGenerator")]
	public class CodeGeneratorConfiguration
	{
        public static class Keys
        {
            public const string DocumentType = "DocumentType";
            public const string MediaType = "MediaType";
        }

        public static class Defaults
        {
            public const string GeneratorFactory = "Umbraco.CodeGen.Generators.DefaultCodeGeneratorFactory, Umbraco.CodeGen";
            public const string ParserFactory = "Umbraco.CodeGen.Parsers.DefaultParserFactory, Umbraco.CodeGen";
        }

        private Dictionary<string, ContentTypeConfiguration> configs;
        private string generatorFactory = Defaults.GeneratorFactory;
        private string parserFactory = Defaults.ParserFactory;

        public ContentTypeConfiguration DocumentTypes
	    {
	        get { return configs[Keys.DocumentType]; }
	        set
	        {
	            value.config = this;
	            value.ContentTypeName = Keys.DocumentType;
	            configs[Keys.DocumentType] = value;
	        }
	    }

	    public ContentTypeConfiguration MediaTypes
	    {
	        get { return configs[Keys.MediaType]; }
	        set
	        {
	            value.config = this;
	            value.ContentTypeName = Keys.MediaType;
	            configs[Keys.MediaType] = value;
	        }
	    }

        [XmlIgnore]
        public string DefaultTypeMapping
        {
            get { return TypeMappings.DefaultType; }
            set { TypeMappings.DefaultType = value; }
        }

        [XmlIgnore]
        public string DefaultDefinitionId
        {
            get { return TypeMappings.DefaultDefinitionId; }
            set { TypeMappings.DefaultDefinitionId = value; }
        }

        public TypeMappings TypeMappings { get; set; } 
        
        [XmlAttribute]
        public bool OverwriteReadOnly { get; set; }

        [XmlAttribute, DefaultValue(Defaults.GeneratorFactory)]
        public string GeneratorFactory
        {
            get { return generatorFactory; }
            set { generatorFactory = value; }
        }

        [XmlAttribute, DefaultValue(Defaults.ParserFactory)]
        public string ParserFactory
        {
            get { return parserFactory; }
            set { parserFactory = value; }
        }

        public ContentTypeConfiguration Get(string contentTypeName)
        {
            return configs[contentTypeName];
        }

	    public CodeGeneratorConfiguration()
	    {
	        configs = new Dictionary<string, ContentTypeConfiguration>
	        {
	            {Keys.DocumentType, new ContentTypeConfiguration(this, Keys.DocumentType)},
	            {Keys.MediaType, new ContentTypeConfiguration(this, Keys.MediaType)}
	        };
            TypeMappings = new TypeMappings(); //new Dictionary<string, string>());
	    }
	}

    [XmlRoot("TypeMappings", Namespace = "")]
    public class TypeMappings
    {
        public static class Defaults
        {
            public const string DefaultDefinitionId = "Textstring";
            public const string DefaultType = "String";
        }

        private string defaultDefinitionId = Defaults.DefaultDefinitionId;
        private string defaultType = Defaults.DefaultType;

        [XmlAttribute, DefaultValue(Defaults.DefaultType)]
        public string DefaultType
        {
            get { return defaultType; }
            set { defaultType = value; }
        }

        [XmlAttribute, DefaultValue(Defaults.DefaultDefinitionId)]
        public string DefaultDefinitionId
        {
            get { return defaultDefinitionId; }
            set { defaultDefinitionId = value; }
        }

        [XmlElement("TypeMapping")]
        public List<TypeMapping> Items { get; set; }
            
        [XmlIgnore]
        public string this[string typeId]
        {
            get
            {
                var mapping = Items.SingleOrDefault(HasDataTypeId(typeId));
                return mapping != null ? mapping.Type : null;
            }
        }

        [XmlIgnore]
        public int Count
        {
            get { return Items.Count; }
        }

        public bool ContainsKey(string typeId)
        {
            return Items.Any(HasDataTypeId(typeId));
        }

        public TypeMappings()
        {
            Items = new List<TypeMapping>();
        }

        public TypeMappings(IEnumerable<TypeMapping> typeMappings)
        {
            Items = new List<TypeMapping>(typeMappings);   
        }

        private static Func<TypeMapping, bool> HasDataTypeId(string typeId)
        {
            return tm => String.Compare(tm.DataTypeId, typeId, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public void Add(TypeMapping typeMapping)
        {
            Items.Add(typeMapping);
        }
    }

    public class TypeMapping
    {
        [XmlAttribute]
        public string DataTypeId { get; set; }
        [XmlAttribute]
        public string Type { get; set; }
        [XmlAttribute]
        public string Description { get; set; }

        public TypeMapping()
        {
        }

        public TypeMapping(string dataTypeId, string type)
        {
            DataTypeId = dataTypeId;
            Type = type;
        }
    }
}