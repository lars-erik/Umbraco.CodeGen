using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Umbraco.CodeGen.Integration.Api
{
    [PluginController("CodeGen")]
    public class ConfigurationController : UmbracoApiController
    {
        private const StringComparison IgnoreCase = StringComparison.InvariantCultureIgnoreCase;

        public CodeGeneratorConfiguration Get()
        {
            return Integration.Configuration.CodeGen;
        }

        public CodeGeneratorConfiguration Post(CodeGeneratorConfiguration configuration)
        {
            Integration.Configuration.SaveConfiguration(configuration);
            return Get();
        }

        public IEnumerable<FactoryDto> GetFactories()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies
                .SelectMany(a => a.GetTypes());
            var generators = types
                .Where(t => t.BaseType == typeof (CodeGeneratorFactory))
                .Select(t =>
                    new FactoryDto
                    {
                        GeneratorFactory = t.FullName,
                        ParserFactory = ((Type)t.CustomAttributes.Single(a => a.AttributeType == typeof(ParserAttribute)).ConstructorArguments[0].Value).FullName,
                        Description = (string)t.CustomAttributes.Single(a => a.AttributeType == typeof(DescriptionAttribute)).ConstructorArguments[0].Value
                    }
                );
            return generators;
        }

        public IEnumerable<PropertyEditorBasic> GetDataTypes()
        {
            return PropertyEditorResolver.Current.PropertyEditors
                .OrderBy(x => x.Name)
                .Select(Mapper.Map<PropertyEditorBasic>);
        }

        public IEnumerable<string> GetTypeProposal(string input)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.Name.StartsWith(input, IgnoreCase) || t.FullName.StartsWith(input, IgnoreCase))
                .Select(t => t.Namespace == "System" ? t.Name : t.FullName)
                .OrderBy(s => s.Length)
                .Take(10);
        }
    }
}
