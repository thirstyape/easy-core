using System.Text;

namespace easy_core;

/// <summary>
/// Extension methods related to strings.
/// </summary>
public static class StringExtensions
{
	/// <summary>
	/// Encodes the provided string to a base 64 string.
	/// </summary>
	/// <param name="value">The value to encode.</param>
	public static string? Base64Encode(this string? value)
	{
		if (value == null)
			return null;

		var bytes = Encoding.UTF8.GetBytes(value);

		return Convert.ToBase64String(bytes);
	}

	/// <summary>
	/// Encodes the provided string from a base 64 string.
	/// </summary>
	/// <param name="value">The value to decode.</param>
	public static string? Base64Decode(this string? value)
	{
		if (value == null)
			return null;

		var bytes = Convert.FromBase64String(value);

		return Encoding.UTF8.GetString(bytes);
	}

	/// <summary>
	/// Formats the provided string for use in a CSV file.
	/// </summary>
	/// <param name="value">The value to format.</param>
	/// <param name="separator">The type of separator that will be used in the CSV file.</param>
	/// <param name="quoted">Additional characters that should be encapsulated in quotation marks when present.</param>
	public static string ToCsvString(this string? value, char separator = ',', params char[] quoted)
	{
		if (string.IsNullOrWhiteSpace(value))
			return string.Empty;

		if (value.Contains('"'))
			value = value.Replace("\"", "\"\"");

		if (value.Contains(separator) || value.Contains('"') || value.StartsWith(' ') || value.EndsWith(' ') || quoted.Any(x => value.Contains(x)))
			value = $"\"{value}\"";

		return value;
	}

	/// <summary>
	/// Splits the provided string into lines at either \r\n or \n.
	/// </summary>
	/// <param name="value">The value to split.</param>
	public static string[] ToLines(this string value) => value.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

	/// <summary>
	/// Converts an Excel style column string into an integer.
	/// </summary>
	/// <param name="column">The Excel column to convert.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public static int FromExcelColumn(this string column)
	{
		if (string.IsNullOrWhiteSpace(column))
			throw new ArgumentNullException(nameof(column));

		column = column.ToUpperInvariant();

		var index = 0;

		for (int i = 0; i < column.Length; i++)
		{
			index *= 26;
			index += column[i] - 'A' + 1;
		}

		return index;
	}

	/// <summary>
	/// Converts an integer into an Excel style column string (such as A, B, AA, AB, etc.).
	/// </summary>
	/// <param name="index">The number to convert.</param>
	public static string ToExcelColumn(this int index)
	{
		var column = "";

		while (index > 0)
		{
			var modulo = (index - 1) % 26;
			column = Convert.ToChar('A' + modulo) + column;
			index = (index - modulo) / 26;
		}

		return column;
	}

	/// <summary>
	/// Compares two strings using the default string comparison functions as per the selected mode.
	/// </summary>
	/// <param name="value">The source value.</param>
	/// <param name="compareTo">The value to compare with the source.</param>
	/// <param name="mode">The comparison function to perform.</param>
	/// <param name="comparisonType">The culture type to compare the strings with.</param>
	/// <remarks>
	/// Will use one of the following functions for comparison:
	/// 
	/// <see cref="string.Equals(string?, StringComparison)"/>,
	/// <see cref="string.Contains(string, StringComparison)"/>,
	/// <see cref="string.StartsWith(string, StringComparison)"/>,
	/// <see cref="string.EndsWith(string, StringComparison)"/>
	/// </remarks>
	public static bool IsMatch(this string value, string compareTo, StringMatchMode mode = StringMatchMode.Equals, StringComparison comparisonType = StringComparison.CurrentCulture)
	{
		if (mode == StringMatchMode.Equals)
			return value.Equals(compareTo, comparisonType);
		else if (mode == StringMatchMode.Contains)
			return value.Contains(compareTo, comparisonType);
		else if (mode == StringMatchMode.StartsWith)
			return value.StartsWith(compareTo, comparisonType);
		else if (mode == StringMatchMode.EndsWith)
			return value.EndsWith(compareTo, comparisonType);
		else
			return false;
	}
}
