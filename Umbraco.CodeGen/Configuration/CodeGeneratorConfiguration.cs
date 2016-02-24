using System;
using Umbraco.CodeGen.Factories;
using Umbraco.CodeGen.Generators;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.ModelsBuilder.Configuration;

namespace Umbraco.CodeGen.Configuration
{
	public class GeneratorConfig
	{
        public Type BaseClass { get; set; }
        public string Namespace { get; set; }
        
        public string GeneratorFactory { get; set; }
        public string InterfaceFactory { get; set; }

	    public string ModelsPath { get; set; }

	    public GeneratorConfig()
	    {
	        GeneratorFactory = typeof(SimpleModelGeneratorFactory).FullName;
	        InterfaceFactory = typeof(InterfaceGeneratorFactory).FullName;
	    }

	    public static GeneratorConfig FromModelsBuilder()
	    {
	        var config = new GeneratorConfig();
	        config.Namespace = UmbracoConfig.For.ModelsBuilder().ModelsNamespace;
	        config.BaseClass = typeof (PublishedContentModel);
	        config.ModelsPath = UmbracoConfig.For.ModelsBuilder().ModelsPath;
	        return config;
	    }
	}

}