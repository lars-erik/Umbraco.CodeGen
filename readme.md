# Umbraco ModelsBuilder Contrib

CodeDom based generation of types with ModelsBuilder.  
Completely extendable and modifyable on all levels.

Builtin generated classes are clean, partial, all virtual for painless extension.

For now ignores all ModelsBuilder logic for parsing existing partials.

Example appSettings for configuring ModelsBuilder to use CodeDom:  
(and save to ~/Models)

```xml
<add key="Umbraco.ModelsBuilder.Enable" value="true" />
<add key="Umbraco.ModelsBuilder.ModelsMode" value="AppData" />
<add key="Umbraco.ModelsBuilder.ModelsPath" value="~/Models" />
<add key="Umbraco.ModelsBuilder.ModelsNamespace" value="WebApplication1.Models" />
<add key="Umbraco.ModelsBuilder.BuilderType" value="Umbraco.CodeGen.CodeDomTextBuilder, Umbraco.CodeGen" />

<!-- optional, to override codedom builder -->
<add key="Umbraco.ModelsBuilder.GeneratorFactory" value="Umbraco.CodeGen.Factories.SimpleModelGeneratorFactory, Umbraco.CodeGen" />
```

Example plain model generator configuration:

```c#
public class SimpleModelGeneratorFactory : CodeGeneratorFactory
{
    public override CodeGeneratorBase Create(Configuration.GeneratorConfig config)
    {
        return new NamespaceGenerator(config,
            new ImportsGenerator(config),
            new SimpleClassGenerator(config,
                new PropertiesGenerator(config,
                    new SimplePropertyGenerator(config)
                )
            )
        );
    }
}
```

Example generator with bundled interfaces for mixin types

```c#
public class SimpleModelAndInterfaceGeneratorFactory : CodeGeneratorFactory
{
    public override CodeGeneratorBase Create(GeneratorConfig config)
    {
        return new NamespaceGenerator(config,
                
            new ImportsGenerator(config),
                
            new SimpleInterfaceGenerator(config,
                new PropertiesGenerator(config,
                    new InterfacePropertyGenerator(config)
                )
            ),
                
            new SimpleClassGenerator(config,
                new PropertiesGenerator(config,
                    new SimplePropertyGenerator(config)
                )
            )
        );

    }
}
```