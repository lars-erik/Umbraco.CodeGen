using System.Collections.Generic;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Tests.TestHelpers
{
    public class TestDataTypeProvider : IDataTypeProvider
    {
        public static readonly DataTypeDefinition Rte = new DataTypeDefinition("RTE", "5e9b75ae-face-41c8-b47e-5f4b0fd82f83", "ca90c950-0aff-4e72-b976-a30b1ac57dad");
        public static readonly DataTypeDefinition TextString = new DataTypeDefinition("Textstring", "ec15c1e5-9d90-422a-aa52-4f7622c63bea", "0cc0eba1-9960-42c9-bf9b-60e150b429ae");
        public static readonly DataTypeDefinition Numeric = new DataTypeDefinition("Numeric", "1413afcb-d19a-4173-8e9a-68288d2a73b8", "2e6d3631-066e-44b8-aec4-96f09099b2b5");
        public static readonly List<DataTypeDefinition> All = new List<DataTypeDefinition>{Rte, TextString, Numeric};

        public IEnumerable<DataTypeDefinition> GetDataTypes()
        {
            return All;
        }
    }
}