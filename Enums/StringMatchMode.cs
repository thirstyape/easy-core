namespace easy_core;

/// <summary>
/// A list of options to compare strings.
/// </summary>
public enum StringMatchMode
{
	/// <summary>
	/// The string must contain the query.
	/// </summary>
	Contains,

	/// <summary>
	/// The string must start with the query.
	/// </summary>
	StartsWith,

	/// <summary>
	/// The string must end with the query.
	/// </summary>
	EndsWith,

	/// <summary>
	/// The string must be the same as the query.
	/// </summary>
	Equals
}
