using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.CodeGen.Integration
{
	public class UmbracoCodeGeneratorConfigurationProvider : IConfigurationProvider
	{
		private readonly IConfigurationProvider provider;

		public UmbracoCodeGeneratorConfigurationProvider(string path)
		{
			using (var reader = File.OpenText(path))
			{
				provider = new CodeGeneratorConfigurationProvider(reader.ReadToEnd());
			}
		}

		public CodeGeneratorConfiguration GetConfiguration()
		{
			return provider.GetConfiguration();
		}
	}
}
