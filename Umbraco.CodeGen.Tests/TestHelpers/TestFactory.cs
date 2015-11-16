using System.Collections.Generic;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Tests.TestHelpers
{
    static internal class TestFactory
    {
        public static DocumentType CreateExpectedDocumentType()
        {
            return new DocumentType
            {
                Info = new DocumentTypeInfo
                {
                    Name = "Some Document Type",
                    Alias = "SomeDocumentType",
                    Icon = "privateMemberIcon.gif",
                    Thumbnail = "privateMemberThumb.png",
                    Description = "A description of some document type",
                    AllowAtRoot = true,
                    Master = null,
                    AllowedTemplates = new List<string>
                    {
                        "ATemplate",
                        "AnotherTemplate"
                    },
                    DefaultTemplate = "ATemplate"
                },
                Structure = new List<string>
                {
                    "SomeOtherDocType"
                },
                GenericProperties = new List<GenericProperty>
                {
                    new GenericProperty
                    {
                        Name = "Some Property",
                        Alias = "someProperty",
                        PropertyEditorAlias = "Umbraco.TinyMCEv3",
                        Definition = "ca90c950-0aff-4e72-b976-a30b1ac57dad",
                        Tab = "A tab",
                        Mandatory = true,
                        Validation = "[a-z]",
                        Description = "A description"
                    },
                    new GenericProperty
                    {
                        Name = "Another Property",
                        Alias = "anotherProperty",
                        PropertyEditorAlias = "Umbraco.TinyMCEv3",
                        Definition = "ca90c950-0aff-4e72-b976-a30b1ac57dad",
                        Tab = "A tab",
                        Description = "Another description"
                    },
                    new GenericProperty
                    {
                        Name = "Tabless Property",
                        Alias = "tablessProperty",
                        Description = null,
                        Tab = null,
                        PropertyEditorAlias = "Umbraco.Integer",
                        Definition = "2e6d3631-066e-44b8-aec4-96f09099b2b5"
                    }
                },
                Tabs = new List<Tab>
                {
                    new Tab
                    {
                        Id = 0,
                        Caption = "A tab"
                    }
                }
            };
        }

        public static MediaType CreateExpectedMediaType()
        {
            return new MediaType
            {
                Info = new Info
                {
                    Name = "Inherited Media Folder",
                    Alias = "InheritedMediaFolder",
                    Icon = "folder.gif",
                    Thumbnail = "folder.png",
                    AllowAtRoot = true,
                    Master = "Folder"
                },
                Structure = new List<string>
                {
                    "Folder",
                    "Image",
                    "File",
                    "InheritedMediaFolder"
                },
                GenericProperties = new List<GenericProperty>
                {
                    new GenericProperty
                    {
                        Name = "Lets Have A Property",
                        Alias = "letsHaveAProperty",
                        PropertyEditorAlias = "76FD82CF-8AC3-4FF7-9E88-D0A8539AE109",
                        Definition = "0cc0eba1-9960-42c9-bf9b-60e150b429ae",
                        Tab = "A tab"
                    },
                    new GenericProperty
                    {
                        Name = "And A Tabless Property",
                        Alias = "andATablessProperty",
                        PropertyEditorAlias = "76FD82CF-8AC3-4FF7-9E88-D0A8539AE109",
                        Definition = "0cc0eba1-9960-42c9-bf9b-60e150b429ae",
                    }
                },
                Tabs = new List<Tab>
                {
                    new Tab
                    {
                        Id = 0,
                        Caption = "A tab"
                    }
                }
            };
        }
    }
}