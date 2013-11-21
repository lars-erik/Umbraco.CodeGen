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
                        Type = "5e9b75ae-face-41c8-b47e-5f4b0fd82f83",
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
                        Type = "5e9b75ae-face-41c8-b47e-5f4b0fd82f83",
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
                        Type = "1413afcb-d19a-4173-8e9a-68288d2a73b8",
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
                        Type = "ec15c1e5-9d90-422a-aa52-4f7622c63bea",
                        Definition = "0cc0eba1-9960-42c9-bf9b-60e150b429ae",
                        Tab = "A tab"
                    },
                    new GenericProperty
                    {
                        Name = "And A Tabless Property",
                        Alias = "andATablessProperty",
                        Type = "ec15c1e5-9d90-422a-aa52-4f7622c63bea",
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