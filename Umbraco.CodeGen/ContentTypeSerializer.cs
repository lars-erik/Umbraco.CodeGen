using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen
{
    /// <summary>
    /// Serializes or deserializes Document- and MediaTypes.
    /// Absolutely not threadsafe, don't event think about it.
    /// </summary>
    public class ContentTypeSerializer
    {
        // Well, I wish I could really do this with XmlSerializer
        // ... and I really didn't try with DataContractSerializer
        // ... and it's only really a matter of selecting root types
        // ... but it's not the ugliest stuff in the world :)

        private static readonly DocumentTypeSerializer DTypeSerializer = new DocumentTypeSerializer();
        private static readonly MediaTypeSerializer MTypeSerializer = new MediaTypeSerializer();

        readonly Dictionary<string, TypedContentTypeSerializer> deserializeFactory = new Dictionary<string, TypedContentTypeSerializer>
        {
            {"DocumentType", DTypeSerializer},
            {"MediaType", MTypeSerializer}
        };

        readonly Dictionary<Type, TypedContentTypeSerializer> serializeFactory = new Dictionary<Type, TypedContentTypeSerializer>
        {
            {typeof(DocumentType), DTypeSerializer },
            {typeof(MediaType), MTypeSerializer }
        };

        private TypedContentTypeSerializer typedSerializer;
        private ContentType type;
        private XElement root;

        public ContentType Deserialize(StreamReader reader)
        {
            var doc = XDocument.Load(reader);
            root = doc.Root;
            if (root == null) throw new Exception("There's no root");
            typedSerializer = CreateTypedSerializer(root);
            type = typedSerializer.Create(root);
            typedSerializer.DeserializeInfo(root.Element("Info"), type);
            DeserializeStructure();
            DeserializeProperties();
            DeserializeTabs();
            return type;
        }

        public string Serialize(ContentType contentType)
        {
            var doc = new XDocument();
            var infoElement = new XElement("Info");
            type = contentType;
            typedSerializer = CreateTypedSerializer();
            root = typedSerializer.CreateRoot();
            doc.Add(root);
            root.Add(infoElement);
            typedSerializer.SerializeInfo(infoElement, contentType);
            SerializeStructure();
            SerializeProperties();
            SerializeTabs();
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            doc.Save(writer);
            writer.Flush();
            return sb.ToString();
        }

        #region Serialization

        private TypedContentTypeSerializer CreateTypedSerializer()
        {
            return serializeFactory[type.GetType()];
        }

        private void SerializeStructure()
        {
            var structure = new XElement("Structure",
                typedSerializer.CreateStructureElements(type)
                );
            root.Add(structure);
        }

        private void SerializeProperties()
        {
            var propsElement = new XElement("GenericProperties");
            foreach (var property in type.GenericProperties)
                SerializeProperty(propsElement, property);
            root.Add(propsElement);
        }

        private void SerializeProperty(XElement propsElement, GenericProperty property)
        {
            var prop = new XElement("GenericProperty",
                CreateElement("Name", property.Name),
                CreateElement("Alias", property.Alias),
                CreateElement("Type", property.Type),
                CreateElement("Definition", property.Definition),
                CreateElement("Tab", property.Tab),
                CreateElement("Mandatory", property.Mandatory.ToString()),
                CreateElement("Validation", property.Validation),
                new XElement("Description", new XCData(property.Description ?? ""))
                );
            propsElement.Add(prop);
        }

        private static XElement CreateElement(string name, string value)
        {
            return String.IsNullOrEmpty(value)
                       ? new XElement(name)
                       : new XElement(name, value);
        }

        private void SerializeTabs()
        {
            var tabsElement = new XElement("Tabs");
            foreach(var tab in type.Tabs)
                SerializeTab(tabsElement, tab);
            root.Add(tabsElement);
        }

        private void SerializeTab(XElement tabsElement, Tab tab)
        {
            var tabElement = new XElement("Tab",
                new XElement("Id", tab.Id),
                CreateElement("Caption", tab.Caption)
                );
            if (tab.Order.HasValue)
                tabElement.Add(new XElement("Order", tab.Order.Value));
            tabsElement.Add(tabElement);
        }
        #endregion

        #region Deserialization
        private TypedContentTypeSerializer CreateTypedSerializer(XElement root)
        {
            var rootName = root.Name.LocalName;
            if (!deserializeFactory.ContainsKey(rootName))
                throw new Exception("Didn't expect root of type " + rootName);
            return deserializeFactory[rootName];
        }

        private void DeserializeStructure()
        {
            var structure = root.Element("Structure");
            if (structure == null)
                throw new Exception("Expected Structure element");
            foreach(var contentType in structure.Elements())
                type.Structure.Add(contentType.Value);
        }

        private void DeserializeProperties()
        {
            var props = root.Element("GenericProperties");
            if (props == null)
                throw new Exception("Expected GenericProperties element");
            foreach (var propElement in props.Elements())
                DeserializeProperty(propElement);
        }

        private void DeserializeProperty(XContainer propElement)
        {
            var prop = new GenericProperty
            {
                Name = propElement.ElementValue("Name"),
                Alias = propElement.ElementValue("Alias"),
                Type = propElement.ElementValue("Type"),
                Definition = propElement.ElementValue("Definition"),
                Tab = propElement.ElementValue("Tab"),
                Mandatory = Convert.ToBoolean(propElement.ElementValue("Mandatory")),
                Validation = propElement.ElementValue("Validation"),
                Description = propElement.ElementValue("Description")
            };
            type.GenericProperties.Add(prop);
        }

        private void DeserializeTabs()
        {
            var tabs = root.Element("Tabs");
            if (tabs == null)
                throw new Exception("Expected Tabs element");
            foreach (var tabElement in tabs.Elements())
                DeserializeTab(tabElement);
        }

        private void DeserializeTab(XContainer tabElement)
        {
            var tab = new Tab
            {
                Caption = tabElement.ElementValue("Caption"),
                Id = Convert.ToInt32(tabElement.ElementValue("Id")),
                Order = !String.IsNullOrEmpty(tabElement.ElementValue("Order"))
                            ? (int?)Convert.ToInt32(tabElement.ElementValue("Order"))
                            : null
            };
            type.Tabs.Add(tab);
        }

        #endregion

        private abstract class TypedContentTypeSerializer
        {
            protected abstract ContentType CreateType();

            public abstract void DeserializeInfo(XElement infoElement, ContentType type);

            public ContentType Create(XContainer root)
            {
                return CreateType();
            }

            protected void DeserializeCommonInfo(XElement infoElement, Info info)
            {
                info.Name = infoElement.ElementValue("Name");
                info.Alias = infoElement.ElementValue("Alias");
                info.Icon = infoElement.ElementValue("Icon");
                info.Thumbnail = infoElement.ElementValue("Thumbnail");
                info.Description = infoElement.ElementValue("Description");
                info.AllowAtRoot = Convert.ToBoolean(infoElement.ElementValue("AllowAtRoot"));
                info.Master = infoElement.ElementValue("Master");
            }

            public abstract XElement CreateRoot();
            
            public virtual void OnSerializingInfo(XElement infoElement, Info info)
            {
            }

            public void SerializeInfo(XElement infoElement, ContentType type)
            {
                var info = GetInfo(type);
                infoElement.Add(CreateElement("Name", info.Name));
                infoElement.Add(CreateElement("Alias", info.Alias));
                infoElement.Add(CreateElement("Icon", info.Icon));
                infoElement.Add(CreateElement("Thumbnail", info.Thumbnail));
                infoElement.Add(CreateElement("Description", info.Description));
                infoElement.Add(CreateElement("AllowAtRoot", info.AllowAtRoot.ToString()));
                infoElement.Add(CreateElement("Master", info.Master));
                OnSerializingInfo(infoElement, info);
            }

            protected abstract Info GetInfo(ContentType type);

            public abstract IEnumerable<XElement> CreateStructureElements(ContentType contentType);
        }

        private class DocumentTypeSerializer : TypedContentTypeSerializer
        {
            protected override ContentType CreateType()
            {
                return new DocumentType();
            }

            public override void DeserializeInfo(XElement infoElement, ContentType type)
            {
                var info = (DocumentTypeInfo) type.Info;
                DeserializeCommonInfo(infoElement, info);

                foreach (var allowedTemplate in infoElement.Descendants("Template"))
                    info.AllowedTemplates.Add(allowedTemplate.Value);
                info.DefaultTemplate = infoElement.ElementValue("DefaultTemplate");
            }

            public override XElement CreateRoot()
            {
                return new XElement("DocumentType");
            }

            public override void OnSerializingInfo(XElement infoElement, Info info)
            {
                var docInfo = (DocumentTypeInfo) info;
                var allowedTemplates = new XElement("AllowedTemplates",
                    docInfo.AllowedTemplates
                           .Where(t => !String.IsNullOrEmpty(t))
                           .Select(t => new XElement("Template", t))
                    );
                infoElement.Add(allowedTemplates);
                infoElement.Add(CreateElement("DefaultTemplate", docInfo.DefaultTemplate));
            }

            protected override Info GetInfo(ContentType type)
            {
                return ((DocumentType) type).Info;
            }

            public override IEnumerable<XElement> CreateStructureElements(ContentType contentType)
            {
                return contentType.Structure
                                  .Where(s => !String.IsNullOrEmpty(s))
                                  .Select(s => new XElement("DocumentType", s));
            }
        }

        private class MediaTypeSerializer : TypedContentTypeSerializer
        {
            protected override ContentType CreateType()
            {
                return new MediaType();
            }

            public override void DeserializeInfo(XElement infoElement, ContentType type)
            {
                DeserializeCommonInfo(infoElement, ((MediaType)type).Info);
            }

            public override XElement CreateRoot()
            {
                return new XElement("MediaType");
            }

            protected override Info GetInfo(ContentType type)
            {
                return ((MediaType) type).Info;
            }

            public override IEnumerable<XElement> CreateStructureElements(ContentType contentType)
            {
                return contentType.Structure
                                  .Where(s => !String.IsNullOrEmpty(s))
                                  .Select(s => new XElement("MediaType", s));
            }
        }
    }
}