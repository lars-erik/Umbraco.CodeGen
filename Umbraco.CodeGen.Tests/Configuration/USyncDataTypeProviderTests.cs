using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Integration;

namespace Umbraco.CodeGen.Tests.Configuration
{
	[TestFixture]
	public class USyncDataTypeProviderTests
	{
		[Test]
		public void GetDataTypes_ReturnsTypesFromDiskConfig()
		{
			var provider = new USyncDataTypeProvider(@"..\..\uSync");
			var dataTypes = provider.GetDataTypes().ToList();
			Assert.AreNotEqual(0, dataTypes.Count());
			Assert.IsNotNullOrEmpty(dataTypes.First().DataTypeName);
			Assert.AreNotEqual(String.Empty, dataTypes.First().DataTypeId);
			Assert.AreNotEqual(Guid.Empty, dataTypes.First().DefinitionId);
            foreach (var type in provider.GetDataTypes())
            {
                Console.WriteLine("{0,-25}{1,-30}{2,-20}", type.DataTypeName, type.DataTypeId, type.DefinitionId);
            }
        }
	}
}
