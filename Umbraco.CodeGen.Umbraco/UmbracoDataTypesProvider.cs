using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.CodeGen.Configuration;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;
using DataTypeDefinition = Umbraco.CodeGen.Configuration.DataTypeDefinition;

namespace Umbraco.CodeGen.Umbraco
{
    public class UmbracoDataTypesProvider : IDataTypeProvider
    {
        private readonly object lockObj = new object();
        private IEnumerable<DataTypeDefinition> dataTypes;

        public IEnumerable<DataTypeDefinition> GetDataTypes()
        {
            lock(lockObj)
            { 
                if (dataTypes == null)
                { 
                    var umbracoDefinitions = UmbracoContext.Current.Application.Services.DataTypeService.GetAllDataTypeDefinitions();
                    dataTypes = umbracoDefinitions.Select(d => new DataTypeDefinition(d.Name, d.PropertyEditorAlias, ResolveType(d)));
                }
            }
            return dataTypes;
        }

        private Type ResolveType(IDataTypeDefinition dataTypeDefinition)
        {

            var ctor = typeof(PublishedPropertyType).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, 
                new[] {typeof(string), typeof(int), typeof(string), typeof(bool) }, 
                null);

            var fakePropType = (PublishedPropertyType)ctor.Invoke(new object[] {dataTypeDefinition.Name, dataTypeDefinition.Id, dataTypeDefinition.PropertyEditorAlias, false});

            var converter = PropertyValueConvertersResolver.Current.Converters.FirstOrDefault(x => x.IsConverter(fakePropType));
            var converterMeta = converter as IPropertyValueConverterMeta;
            var type = converterMeta?.GetPropertyValueType(fakePropType);
            if (converter != null && type == null)
            {
                var attr = converter.GetType().GetCustomAttribute<PropertyValueTypeAttribute>();
                if (attr != null)
                {
                    type = attr.Type;
                }
            }
            return type ?? typeof(object);
        }
    }
}
