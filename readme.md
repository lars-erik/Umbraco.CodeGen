# Umbraco ModelsBuilder Contrib

CodeDom based generation of types with ModelsBuilder.  
Completely extendable and modifyable on all levels.

Generated classes are clean, partial, all virtual for painless extension.

For now ignores all ModelsBuilder logic for parsing existing partials.

For now not possible to swap factory, but configuration imminent.

Example appSettings for configuring ModelsBuilder to use CodeDom:  
(and save to ~/Models)

```xml
<add key="Umbraco.ModelsBuilder.Enable" value="true" />
<add key="Umbraco.ModelsBuilder.ModelsMode" value="AppData" />
<add key="Umbraco.ModelsBuilder.ModelsPath" value="~/Models" />
<add key="Umbraco.ModelsBuilder.ModelsNamespace" value="WebApplication1.Models" />
<add key="Umbraco.ModelsBuilder.BuilderType" value="Umbraco.CodeGen.CodeDomTextBuilder, Umbraco.CodeGen" />
```

Example generator configuration:

```c#
public class SimpleModelGeneratorFactory : CodeGeneratorFactory
{
    public override CodeGeneratorBase Create(Configuration.GeneratorConfig configuration)
    {
        return new NamespaceGenerator(
            configuration,
            new ImportsGenerator(configuration),
            new ClassGenerator(configuration,
                new EntityNameGenerator(configuration),
                new CtorGenerator(configuration),
                new PropertiesGenerator(
                    configuration,
                    new PublicPropertyDeclarationGenerator(
                        configuration,
                        new EntityNameGenerator(configuration),
                        new PropertyBodyGenerator(configuration)
                        )
                    )
                )
            );
    }
}
```
