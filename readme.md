# Umbraco uSync Code Generator

Synchronizes C# classes with uSync DocumentType files.

Add all assemblies to Umbraco/Bin and you're good to go. (Yes, yes, package is coming)
Config is read from uSync and ~/config/CodeGen.config.

##Recommended uSync setup:
    <usync read="true" write="false" attach="true" .../>

*Having `write="true"` will overwrite your code, so don't use it unless you don't want to edit generated code.*

*DO NOT CODE FIRST BEFORE FIRST RUN OF USYNC!*
*YOU'LL LOSE YOUR CODE!*

##Recommended CodeGen config (~/config/CodeGen.config):
    <?xml version="1.0" encoding="utf-8" ?>
    <CodeGenerator
      ModelPath="~/Models/Synchronized"
      BaseClass="DocumentTypeBase"
      Namespace="Umbraco.CodeGen.Web.Models"
      GenerateClasses="true"
      GenerateXml="true"
      RemovePrefix="pfx"
      OverwriteReadOnly="false"
      >
      <TypeMappings Default="String">
	    <TypeMapping DataTypeId="38B352C1-E9F8-4FD8-9324-9A2EAB06D97A" Type="Boolean" Description="True/false"/>
	    <TypeMapping DataTypeId="1413AFCB-D19A-4173-8E9A-68288D2A73B8" Type="Int32" Description="Numeric"/>
	    <TypeMapping DataTypeId="5032A6E6-69E3-491D-BB28-CD31CD11086C" Type="Int32" Description="Upload"/>
	    <TypeMapping DataTypeId="B6FB1622-AFA5-4BBF-A3CC-D9672A442222" Type="DateTime" Description="Date Picker with time"/>
	    <TypeMapping DataTypeId="F8D60F68-EC59-4974-B43B-C46EB5677985" Type="System.Drawing.Color" Description="Approved Color"/>
	    <TypeMapping DataTypeId="CCCD4AE9-F399-4ED2-8038-2E88D19E810C" Type="Object" Description="Folder Browser"/>
	    <TypeMapping DataTypeId="23E93522-3200-44E2-9F29-E61A6FCBB79A" Type="DateTime" Description="Date Picker"/>
	    <TypeMapping DataTypeId="158AA029-24ED-4948-939E-C3DA209E5FBA" Type="Int32" Description="Content Picker"/>
	    <TypeMapping DataTypeId="EAD69342-F06D-4253-83AC-28000225583B" Type="Int32" Description="Media Picker"/>
	    <TypeMapping DataTypeId="474FCFF8-9D2D-11DE-ABC6-AD7A56D89593" Type="Object" Description="Macro Container"/>
      </TypeMappings>
    </CodeGenerator>

###CodeGenerator element

*ModelPath*
If you have ReSharper, set the Models\Synchronized folder as Namespace Provider=False.
Otherwise, make a folder that has the namespace structure you want.
The code generation will attemt to create DocumentType XML for ALL classes found in the specified folder.

*BaseClass*
A class in the same namespace as the generated/code first classes with at least the following structure:
    public class DocumentTypeBase
    {
        protected IPublishedContent Content; // Can be field or property, must be named Content

        protected DocumentTypeBase(IPublishedContent content) // Must implement ctor with one IPublishedContent arg
        {
            Content = content;
        }
    }
The class itself can be named whatever.

*Namespace*
Preferably the correct namespace for your model folder. :)

*GenerateClasses*
Whether to generate class for document types when saved. (Works with uSync attach=true)

*GenerateXml*
Whether to generate XML for document types when Umbraco starts. (Works with uSync read=true)

*RemovePrefix*
Partially implemented and not at all tested.
Should remove prefixes from class and property names if you have them in your aliases.

*OverwriteReadOnly*
For everyone lucky enough to have a SCS locking the files.

*TypeMappings*
Pretty self explanatory if you should use this tool at all. ;)
For some reason the CSharpCodeDomProvider adds @ to intristic type aliases. (string, int etc.)
Use the class names of the types to avoid it.

Gotchas:
* If document type and property has the same name,
    the class is suffixed with "Class"