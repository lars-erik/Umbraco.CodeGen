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

Example generated class with mixin

```c#
namespace WebApplication1.Models
{
    using global::System;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;
    
    public partial class UmbTextPage : UmbMaster, ISomeMixin
    {
        public UmbTextPage(IPublishedContent content) : 
                base(content)
        {
        }
        public virtual bool FeaturedPage
        {
            get
            {
                return Content.GetPropertyValue<bool>("featuredPage");
            }
        }
        public virtual object Image
        {
            get
            {
                return Content.GetPropertyValue<object>("image");
            }
        }
        public virtual System.Web.IHtmlString BodyText
        {
            get
            {
                return Content.GetPropertyValue<System.Web.IHtmlString>("bodyText");
            }
        }
        public virtual System.DateTime MixinProperty
        {
            get
            {
                return Content.GetPropertyValue<System.DateTime>("mixinProperty");
            }
        }
    }
}
```

Example code generator for property declarations (used from `SimplePropertyGenerator`)

```c#
public class PropertyDeclarationGenerator : CompositeCodeGenerator
{
    protected PropertyDeclarationGenerator(GeneratorConfig config, params CodeGeneratorBase[] childGenerators) 
        : base(config, childGenerators)
    {
    }

    // Gets a CodeDom `CodeMemberProperty` created by `PropertiesGenerator`
    public override void Generate(object codeObject, object typeOrPropertyModel)
    {
        var umbracoProperty = (PropertyModel)typeOrPropertyModel;
        var codeDomProperty = (CodeMemberProperty)codeObject;

        codeDomProperty.Attributes = MemberAttributes.Public;
        codeDomProperty.Type = new CodeTypeReference(umbracoProperty.ClrType);
        codeDomProperty.Name = umbracoProperty.ClrName;

        // Delegate rest of this to children, 
        // passing the CodeDom property and the Umbraco property def.
        base.Generate(codeDomProperty, umbracoProperty)
    }

}
```
