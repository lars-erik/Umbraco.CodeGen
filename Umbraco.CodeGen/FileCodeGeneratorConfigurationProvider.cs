using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.CodeGen.Integration
{
	public class FileCodeGeneratorConfigurationProvider : IConfigurationProvider
	{
		private readonly IConfigurationProvider provider;

		public FileCodeGeneratorConfigurationProvider(string path)
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
