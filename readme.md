# Umbraco uSync Code Generator

Synchronizes C# classes with uSync DocumentType files.

Install the [package](http://our.umbraco.org/projects/developer-tools/umbraco-codegen) or add all assemblies to ~/Bin and you're good to go.
Config is read from uSync and ~/config/CodeGen.config.

Depends on [NRefactory](https://github.com/icsharpcode/NRefactory) and [USync](https://github.com/KevinJump/jumps.umbraco.usync)

Binaries available at the [project page](http://lars-erik.github.io/Umbraco.CodeGen/), and as a [package](http://our.umbraco.org/projects/developer-tools/umbraco-codegen)

Documentation is all in the [wiki](https://github.com/lars-erik/Umbraco.CodeGen/wiki)

###Gotchas:
* Generated classes are partial
    DO NOT write code other than properties in the generated classes!
* If document type and property has the same name,
    the class is suffixed with "Class"