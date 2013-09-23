# Umbraco uSync Code Generator

*Note: This is not my main source control. Uploaded here for sharing purposes.*

Build and run vsix file from vsix project bin folder to install.

Create an xml file in your project with the following minimal content:

    <?xml version="1.0" encoding="utf-8" ?>
    <CodeGen>
      <USync path="[relative path to usync folder]" removePrefix="" />
      <TypeMappings default="String"/>
    </CodeGen>

To have properties generated as other types than string,
add TypeMapping elements to the TypeMappings element:

    <TypeMappings default="String">
      <TypeMapping dataType="ead69342-f06d-4253-83ac-28000225583b" type="Int32"/>
    </TypeMappings>

Where dataType equals the Id of the datatype you want to map.

If you've prefixed your aliases with something you don't want in your code,
set removePrefix to whatever you want to remove.

Set custom tool for the xml file to DocumentTypeClassGen.
Save the file, or later right click an select "Run Custom Tool"

Gotchas:
* If document type and property has the same name,
    the class is suffixed with "Class"