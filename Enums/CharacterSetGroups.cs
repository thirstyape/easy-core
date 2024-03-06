using System.ComponentModel.DataAnnotations;

namespace easy_core;

/// <summary>
/// Defines various groups of ASCII characters.
/// </summary>
[Flags]
public enum CharacterSetGroups
{
	/// <summary>
	/// Contains 0 - 9.
	/// </summary>
	/// <remarks>
	/// 0123456789
	/// </remarks>
	[Display(Name = "Numeric", Description = "Adds 0 - 9")]
	Numeric = 0b_00000000_00000000_00000000_00000001,

	/// <summary>
	/// Contains A - Z in lower case.
	/// </summary>
	/// <remarks>
	/// abcdefghijklmnopqrstuvwxyz
	/// </remarks>
	[Display(Name = "Lower Case", Description = "Adds A - Z in lower case")]
	Lowercase = 0b_00000000_00000000_00000000_00000010,

	/// <summary>
	/// Contains A - Z in upper case.
	/// </summary>
	/// <remarks>
	/// ABCDEFGHIJKLMNOPQRSTUVWXYZ
	/// </remarks>
	[Display(Name = "Upper Case", Description = "Adds A - Z in upper case")]
	Uppercase = 0b_00000000_00000000_00000000_00000100,

	/// <summary>
	/// Contains various punctuation marks.
	/// </summary>
	/// <remarks>
	/// `~!@#$%^&amp;*()_-=+[]{}|;:,.&lt;&gt;?
	/// </remarks>
	[Display(Name = "Punctuation", Description = "Adds various punctuation marks")]
	Punctuation = 0b_00000000_00000000_00000000_00001000
}
