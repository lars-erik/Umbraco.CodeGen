using System;

static public class StringExtensions
{
	private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

	public static string PascalCase(this string value)
	{
		return value.Substring(0, 1).ToUpper() + value.Substring(1);
	}

	public static string RemovePrefix(this string name, string removePrefix)
	{
		if (removePrefix != null && name.StartsWith(removePrefix, IgnoreCase))
			name = name.Substring(removePrefix.Length);
		return name;
	}

	public static string CamelCase(this string value)
	{
		return value.Substring(0, 1).ToLower() + value.Substring(1);
	}
}