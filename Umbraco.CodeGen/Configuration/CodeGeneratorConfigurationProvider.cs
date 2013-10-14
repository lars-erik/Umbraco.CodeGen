using System.IO;
using System.Xml.Serialization;

namespace Umbraco.CodeGen.Configuration
{
	public class CodeGeneratorConfigurationProvider : IConfigurationProvider
	{
		private readonly string inputFileContent;
		private CodeGeneratorConfiguration configuration;


		public CodeGeneratorConfigurationProvider(string inputFileContent)
		{
			this.inputFileContent = inputFileContent;
		}

		public CodeGeneratorConfiguration GetConfiguration()
		{
		    return configuration ?? (configuration = LoadConfiguration());
		}

	    private CodeGeneratorConfiguration LoadConfiguration()
		{
		    var serializer = new XmlSerializer(typeof (CodeGeneratorConfiguration));
		    return (CodeGeneratorConfiguration) serializer.Deserialize(new StringReader(inputFileContent));
		}
	}
}