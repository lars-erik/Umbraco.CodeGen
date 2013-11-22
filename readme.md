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
        <TypeMapping DataTypeId="Umbraco.TrueFalse" Type="Boolean" Description="True/false"/>
        <TypeMapping DataTypeId="Umbraco.Integer" Type="Int32" Description="Numeric"/>
        <TypeMapping DataTypeId="Umbraco.UploadField" Type="Int32" Description="Upload"/>
        <TypeMapping DataTypeId="Umbraco.DateTime" Type="DateTime" Description="Date Picker with time"/>
        <TypeMapping DataTypeId="Umbraco.ColorPickerAlias" Type="String" Description="Approved Color"/><!-- System.Drawing.Color <- I Wish.. -->
        <TypeMapping DataTypeId="Umbraco.FolderBrowser" Type="Object" Description="Folder Browser"/>
        <TypeMapping DataTypeId="Umbraco.Date" Type="DateTime" Description="Date Picker"/>
        <TypeMapping DataTypeId="Umbraco.ContentPickerAlias" Type="Int32" Description="Content Picker"/>
        <TypeMapping DataTypeId="Umbraco.MediaPicker" Type="Int32" Description="Media Picker"/>
        <TypeMapping DataTypeId="Umbraco.TinyMCEv3" Type="System.Web.IHtmlString" Description="Richtext editor"/>
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