# Umbraco uSync Code Generator

Synchronizes C# classes with uSync DocumentType files.

# Umbraco 7 usage

Build USync development branch. :)  

Build CodeGen against it and U7.  

New DataTypeIds etc. in U7, so need new config below.  
Invalid Descriptions in this config, but they aren't used.  

    <?xml version="1.0" encoding="utf-8" ?>
    <CodeGenerator 
      OverwriteReadOnly="true"
      GeneratorFactory="Umbraco.CodeGen.Generators.AnnotatedCodeGeneratorFactory, Umbraco.CodeGen"
      ParserFactory="Umbraco.CodeGen.Parsers.AnnotatedParserFactory, Umbraco.CodeGen"
      >
      <DocumentTypes ModelPath="Models/DocumentTypes" Namespace="MP.Standard.Web.Models" BaseClass="PublishedContentModel" GenerateClasses="false" GenerateXml="true" RemovePrefix=""/>
      <MediaTypes ModelPath="Models/MediaTypes" Namespace="MP.Standard.Web.Models" BaseClass="PublishedContentModel" GenerateClasses="false" GenerateXml="false" RemovePrefix=""/>
      <TypeMappings Default="String" DefaultDefinitionId="0cc0eba1-9960-42c9-bf9b-60e150b429ae">
        <TypeMapping DataTypeId="92897bc6-a5f3-4ffe-ae27-f2e7e33dda49" Type="Boolean" Description="True/false"/>
        <TypeMapping DataTypeId="2e6d3631-066e-44b8-aec4-96f09099b2b5" Type="Int32" Description="Numeric"/>
        <TypeMapping DataTypeId="84c6b441-31df-4ffe-b67e-67d5bc3ae65a" Type="Int32" Description="Upload"/>
        <TypeMapping DataTypeId="e4d66c0f-b935-4200-81f0-025f7256b89a" Type="DateTime" Description="Date Picker with time"/>
        <TypeMapping DataTypeId="0225af17-b302-49cb-9176-b9f35cab9c17" Type="System.Drawing.Color" Description="Approved Color"/>
        <TypeMapping DataTypeId="fd9f1447-6c61-4a7c-9595-5aa39147d318" Type="Object" Description="Folder Browser"/>
        <TypeMapping DataTypeId="5046194e-4237-453c-a547-15db3a07c4e1" Type="DateTime" Description="Date Picker"/>
        <TypeMapping DataTypeId="a6857c73-d6e9-480c-b6e6-f15f6ad11125" Type="Int32" Description="Content Picker"/>
      </TypeMappings>
    </CodeGenerator>

Reference Umbraco.CodeGen.WaitForSixTwo in addition to core/integration.
Umbraco.CodeGen.WaitForSixTwo is built as Umbraco.Tests.dll for Typed model factory support.  
Create the following class in your web ~/Models, then derive your models from it.  
Models will be fed to `UmbracoViewPage<YourModel>` automatically. Just waiting for Stephane to be happy with the factory stuff.  
In the mean time tearing a rift in the space time continuum by faking trusted assembly. :)  

    public class PublishedContentModel : Umbraco.CodeGen.WaitForSixTwo.PublishedContentModel
    {
        public PublishedContentModel(IPublishedContent content)
            : base(content)
        {
        }
    }


# None of the following confirmed for U7

Install the [package](http://our.umbraco.org/projects/developer-tools/umbraco-codegen) or add all assemblies to ~/Bin and you're good to go.
Config is read from uSync and ~/config/CodeGen.config.

Depends on [NRefactory](https://github.com/icsharpcode/NRefactory) and [USync](https://github.com/KevinJump/jumps.umbraco.usync)

Binaries available at the [project page](http://lars-erik.github.io/Umbraco.CodeGen/), and as a [package](http://our.umbraco.org/projects/developer-tools/umbraco-codegen)

Documentation is all in the [wiki](https://github.com/lars-erik/Umbraco.CodeGen/wiki)

###Gotchas:
* Generated classes are partial
    * DO NOT write code other than properties in the generated classes!
* If document type and property has the same name,
    the class is suffixed with "Class"