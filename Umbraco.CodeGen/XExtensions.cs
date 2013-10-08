using System.Linq;
using System.Xml.Linq;

namespace Umbraco.CodeGen
{
	public static class XExtensions
	{
		public static string ElementValue(this XContainer element, string elementName)
		{
			var subElement = element.Element(elementName);
			if (subElement == null)
				return "";
			return subElement.Value;
		}

		public static string AttributeValue(this XElement element, string attributeName, string defaultValue = "")
		{
			return element.Attributes(attributeName).Select(a => a.Value).DefaultIfEmpty(defaultValue).SingleOrDefault();
		}
	}
}