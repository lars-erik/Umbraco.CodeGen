using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

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
        [JsonIgnore]
        public string ContentTypeName { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public TypeMappings TypeMappings { get { return config.TypeMappings; } }

        [XmlIgnore]
        [JsonIgnore]
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
	        get { return Configs.ContainsKey(Keys.DocumentType) ? Configs[Keys.DocumentType] : null; }
	        set { SetConfig(value, Keys.DocumentType); }
	    }

        public ContentTypeConfiguration MediaTypes
	    {
	        get { return Configs.ContainsKey(Keys.MediaType) ? Configs[Keys.MediaType] : null; }
            set { SetConfig(value, Keys.MediaType); }
	    }

        private void SetConfig(ContentTypeConfiguration value, string contentTypeName)
        {
            value.config = this;
            value.ContentTypeName = contentTypeName;
            if (!Configs.ContainsKey(contentTypeName))
                Configs.Add(contentTypeName, value);
            else
                Configs[contentTypeName] = value;
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

        protected Dictionary<string, ContentTypeConfiguration> Configs
        {
            get { return configs ?? (configs = new Dictionary<string, ContentTypeConfiguration>()); }
        }

        public ContentTypeConfiguration Get(string contentTypeName)
        {
            return Configs[contentTypeName];
        }

        public CodeGeneratorConfiguration()
        {
            configs = new Dictionary<string, ContentTypeConfiguration>
	        {
	            {Keys.DocumentType, new ContentTypeConfiguration(this, Keys.DocumentType)},
	            {Keys.MediaType, new ContentTypeConfiguration(this, Keys.MediaType)}
	        };
        }

        public static CodeGeneratorConfiguration Create()
        {
            var configuration = new CodeGeneratorConfiguration
            {
                TypeMappings = new TypeMappings()
            };
            configuration.configs = new Dictionary<string, ContentTypeConfiguration>
	        {
	            {Keys.DocumentType, new ContentTypeConfiguration(configuration, Keys.DocumentType)},
	            {Keys.MediaType, new ContentTypeConfiguration(configuration, Keys.MediaType)}
	        };
            return configuration;
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