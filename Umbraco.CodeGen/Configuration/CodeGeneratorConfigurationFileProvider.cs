using System.IO;
using System.Text;
using System.Xml;

namespace Umbraco.CodeGen.Configuration
{
    public class CodeGeneratorConfigurationFileProvider : IConfigurationProvider, IConfigurationPersister
	{
	    private readonly string path;
	    private readonly IConfigurationProvider provider;

		public CodeGeneratorConfigurationFileProvider(string path)
		{
		    this.path = path;
		    using (var reader = File.OpenText(path))
			{
				provider = new CodeGeneratorConfigurationProvider(reader.ReadToEnd());
			}
		}

	    public CodeGeneratorConfiguration GetConfiguration()
		{
			return provider.GetConfiguration();
		}

	    public void SaveConfiguration(CodeGeneratorConfiguration configuration)
	    {
	        using (var writer = File.CreateText(path))
	        {
                var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = Encoding.UTF8, Indent = true });
                CodeGeneratorConfigurationProvider.SerializeConfiguration(configuration, xmlWriter);
	        }
	    }
	}
}
