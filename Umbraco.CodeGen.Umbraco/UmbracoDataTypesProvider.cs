using System;
using System.Collections.Generic;
using System.Linq;
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
            var fakePropType = new PublishedPropertyType(null, new PropertyType(dataTypeDefinition.PropertyEditorAlias, dataTypeDefinition.DatabaseType));
            return fakePropType.ClrType;
        }
    }
}
