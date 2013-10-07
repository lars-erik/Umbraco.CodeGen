using System.Xml.Linq;

static internal class XExtensions
{
	public static string ElementValue(this XContainer element, string elementName)
	{
		var subElement = element.Element(elementName);
		if (subElement == null)
			return "";
		return subElement.Value;
	}
}