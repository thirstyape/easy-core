using System.Text;

namespace easy_core;

/// <summary>
/// Converts data to and from base 32 format.
/// </summary>
/// <remarks>
/// Based on Shane's answer from <see href="https://stackoverflow.com/questions/641361/base32-decoding">Stack Overflow</see>.
/// </remarks>
public static class Base32Converter
{
	/// <summary>
	/// Converts the specified string, which encodes binary data as base-32 digits, to an plain text string.
	/// </summary>
	/// <param name="s">The string to convert.</param>
	/// <remarks>
	/// Assumes ASCII encoding is used on input string.
	/// </remarks>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	public static string DecodeBase32String(string s)
	{
		return Encoding.ASCII.GetString(FromBase32String(s));
	}

	/// <summary>
	/// Converts the specified string, which encodes binary data as base-32 digits, to an equivalent 8-bit unsigned integer array.
	/// </summary>
	/// <param name="s">The string to convert.</param>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	public static byte[] FromBase32String(string s)
	{
		if (string.IsNullOrEmpty(s))
			throw new ArgumentNullException(nameof(s));

		s = s.TrimEnd('=');

		int mask;
		byte currentByte = 0, bitsRemaining = 8;

		var byteCount = s.Length * 5 / 8;
		var returnArray = new byte[byteCount];
		var arrayIndex = 0;

		foreach (char c in s)
		{
			var currentValue = CharToValue(c);

			if (bitsRemaining > 5)
			{
				mask = currentValue << (bitsRemaining - 5);
				currentByte = (byte)(currentByte | mask);
				bitsRemaining -= 5;
			}
			else
			{
				mask = currentValue >> (5 - bitsRemaining);
				currentByte = (byte)(currentByte | mask);
				returnArray[arrayIndex++] = currentByte;
				currentByte = (byte)(currentValue << (3 + bitsRemaining));
				bitsRemaining += 3;
			}
		}

		// Check whether ending in full byte
		if (arrayIndex != byteCount)
			returnArray[arrayIndex] = currentByte;

		return returnArray;
	}

	/// <summary>
	/// Converts a string to its equivalent string representation that is encoded with base-32 digits.
	/// </summary>
	/// <param name="inString">A string to convert.</param>
	/// <remarks>
	/// Assumes ASCII encoding is used on input string.
	/// </remarks>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	public static string EncodeBase32String(string inString)
	{
		return ToBase32String(Encoding.ASCII.GetBytes(inString));
	}

	/// <summary>
	/// Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-32 digits.
	/// </summary>
	/// <param name="inArray">An array of 8-bit unsigned integers.</param>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	public static string ToBase32String(byte[] inArray)
	{
		if (inArray == null || inArray.Length == 0)
			throw new ArgumentNullException(nameof(inArray));

		byte nextChar = 0, bitsRemaining = 5;

		var arrayIndex = 0;
		var charCount = (int)Math.Ceiling(inArray.Length / 5d) * 8;
		var returnArray = new char[charCount];

		foreach (byte b in inArray)
		{
			nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
			returnArray[arrayIndex++] = ValueToChar(nextChar);

			if (bitsRemaining < 4)
			{
				nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
				returnArray[arrayIndex++] = ValueToChar(nextChar);
				bitsRemaining += 5;
			}

			bitsRemaining -= 3;
			nextChar = (byte)((b << bitsRemaining) & 31);
		}

		// Check whether ending in full char
		if (arrayIndex != charCount)
		{
			returnArray[arrayIndex++] = ValueToChar(nextChar);
			while (arrayIndex != charCount) returnArray[arrayIndex++] = '=';
		}

		return new string(returnArray);
	}

	/// <summary>
	/// Converts a char into its equivalent base-32 byte.
	/// </summary>
	/// <param name="c">The char to convert.</param>
	/// <exception cref="ArgumentException"></exception>
	private static int CharToValue(char c)
	{
		var value = (int)c;

		if (value < 91 && value > 64) // Uppercase letters
			return value - 65;
		else if (value < 56 && value > 49) // Numbers 2-7
			return value - 24;
		else if (value < 123 && value > 96) // Lowercase letters
			return value - 97;
		else
			throw new ArgumentException("Character is not a Base32 character.", nameof(c));
	}

	/// <summary>
	/// Converts a base-32 byte into its equivalent char.
	/// </summary>
	/// <param name="b">The byte to convert.</param>
	/// <exception cref="ArgumentException"></exception>
	private static char ValueToChar(byte b)
	{
		if (b < 26)
			return (char)(b + 65);
		else if (b < 32)
			return (char)(b + 24);
		else
			throw new ArgumentException("Byte is not a base-32 value.", nameof(b));
	}
}
