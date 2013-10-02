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
			var provider = new USyncDataTypeProvider(Path.Combine(Environment.CurrentDirectory, @"..\..\uSync"));
			var dataTypes = provider.GetDataTypes().ToList();
			Assert.AreNotEqual(0, dataTypes.Count());
			Assert.IsNotNullOrEmpty(dataTypes.First().DataTypeName);
			Assert.AreNotEqual(Guid.Empty, dataTypes.First().DataTypeId);
			Assert.AreNotEqual(Guid.Empty, dataTypes.First().DefinitionId);
		}
	}
}
