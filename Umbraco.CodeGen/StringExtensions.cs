using System;
using System.Globalization;

static public class StringExtensions
{
	private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;


	public static string PascalCase(this string value)
	{
		if (String.IsNullOrEmpty(value)) return value;
		return value.Substring(0, 1).ToUpper() + value.Substring(1);
	}

    public static string SplitPascalCase(this string value)
    {
        if (String.IsNullOrEmpty(value)) return value;
        var cc = CultureInfo.CurrentCulture;
        var newValue = PascalCase(value);
        var splitValue = "";
        for (var i = 0; i < newValue.Length; i++)
        {
            if (i > 0 && newValue[i].ToString(cc) == newValue[i].ToString(cc).ToUpper())
                splitValue += " ";
            splitValue += newValue[i];
        }
        return splitValue;
    }

	public static string RemovePrefix(this string name, string removePrefix)
	{
		if (removePrefix != null && name.StartsWith(removePrefix, IgnoreCase))
			name = name.Substring(removePrefix.Length);
		return name;
	}

	public static string CamelCase(this string value)
	{
		if (String.IsNullOrEmpty(value)) return value;
		return value.Substring(0, 1).ToLower() + value.Substring(1);
	}
}