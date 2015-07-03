using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.Core.Models;

namespace Umbraco.CodeGen.Umbraco.Tests
{
    [TestFixture]
    public class Mapping_IContentType_To_ContentType
    {
        [Test]
        public void Maps_Metadata()
        {
            var umbct = new Core.Models.ContentType(-1)
            {
                Alias = "SomeDocumentType",
                Name = "Some document type",
                AllowedAsRoot = true,
                AllowedContentTypes = new[] { new ContentTypeSort(new Lazy<int>(() => 1), 1, "SomeOtherDocType"), },
                AllowedTemplates = new[] { new Template("", "", "ATemplate"), new Template("", "", "AnotherTemplate"), },
                PropertyGroups = new PropertyGroupCollection(new[]{
                    new PropertyGroup
                    {
                      Name  = "A tab",
                      PropertyTypes = new PropertyTypeCollection(new []
                      {
                          new PropertyType(new DataTypeDefinition(-1, "Umbraco.Richtext"), "Richtext editor")
                          {
                              Alias = "someProperty",
                              Description = "A description"
                          }, 
                          new PropertyType(new DataTypeDefinition(-1, "Umbraco.Richtext"), "Richtext editor")
                          {
                              Alias = "anotherProperty",
                              Description = "Another description"
                          } 
                      })
                    },
                    new PropertyGroup(new PropertyTypeCollection(new []
                    {
                          new PropertyType(new DataTypeDefinition(-1, "Umbraco.Number"), "Numeric")
                          {
                              Alias = "tablessProperty"
                          },
                    }))
                }),
                Description = "A description of some document type",
                Icon = "privateMemberIcon.gif",
                Thumbnail = "privateMemberThumb.png",
            };
            umbct.SetDefaultTemplate(umbct.AllowedTemplates.First());

            var expected = new Definitions.DocumentType
            {
                Info = new DocumentTypeInfo
                {
                    Alias = "SomeDocumentType",
                    AllowAtRoot = true,
                    AllowedTemplates = new List<string> { "ATemplate", "AnotherTemplate" },
                    DefaultTemplate = "ATemplate",
                    Description = "A description of some document type",
                    Icon = "privateMemberIcon.gif",
                    Master = "",
                    Name = "Some document type",
                    Thumbnail = "privateMemberThumb.png"
                },
                Tabs = new List<Tab>
                {
                    new Tab{Caption="A tab"}
                },
                Structure = new List<string>
                {
                    "SomeOtherDocType"
                },
                GenericProperties = new List<GenericProperty>
                {
                    new GenericProperty
                    {
                        Alias = "someProperty",
                        Definition = "Richtext editor",
                        Description = "A description",
                        Name = "Some property",
                        Tab = "A tab",
                        Type = "Umbraco.Richtext"
                    },
                    new GenericProperty
                    {
                        Alias = "anotherProperty",
                        Definition = "Richtext editor",
                        Description = "Another description",
                        Name = "Another property",
                        Tab = "A tab",
                        Type = "Umbraco.Richtext"
                    },
                    new GenericProperty
                    {
                        Alias = "tablessProperty",
                        Definition = "Numeric",
                        Name = "Tabless property",
                        Tab = "",
                        Type = "Umbraco.Number"
                    },
                }
            };

            var actual = ContentTypeMapping.Map(umbct);

            Assert.AreEqual(expected, actual, JsonConvert.SerializeObject(actual));

        }
    }
}
