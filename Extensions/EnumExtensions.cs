using System.ComponentModel.DataAnnotations;

namespace easy_core;

/// <summary>
/// Extension methods related to enumerations.
/// </summary>
public static class EnumExtensions
{
	/// <summary>
	/// Iterates over a bitmasked enum value and returns each of the flags that is active.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enum to iterate.</typeparam>
	/// <param name="value">The value to iterate over.</param>
	/// <remarks>
	/// Does not support ulong as underlying type for Enum.
	/// </remarks>
	public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum value) where TEnum : Enum
	{
		foreach (var flag in Enum.GetValues(value.GetType()).Cast<TEnum>())
			if ((Convert.ToInt64(value) & Convert.ToInt64(flag)) != 0)
				yield return flag;
	}

	/// <summary>
	/// Checks to see the provided value has at least one of the acceptable flags.
	/// </summary>
	/// <param name="value">The value to check.</param>
	/// <param name="acceptableFlags">The flags to check against (input as TEnum.Option1 | TEnum.Option2).</param>
	public static bool HasAnyFlag<TEnum>(this TEnum value, TEnum acceptableFlags) where TEnum : Enum
	{
		if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(ulong))
			return (Convert.ToUInt64(value) & Convert.ToUInt64(acceptableFlags)) != 0;
		else
			return (Convert.ToInt64(value) & Convert.ToInt64(acceptableFlags)) != 0;
	}

	/// <summary>
	/// Checks to see the provided value has all of the required flags.
	/// </summary>
	/// <param name="value">The value to check.</param>
	/// <param name="requiredFlags">The flags to check against (input as TEnum.Option1 | TEnum.Option2).</param>
	/// <remarks>
	/// Does not support ulong as underlying type for Enum.
	/// </remarks>
	public static bool HasAllFlags<TEnum>(this TEnum value, TEnum requiredFlags) where TEnum : Enum
	{
		foreach (var requirement in requiredFlags.GetFlags().Select(x => Convert.ToInt64(x)).Where(x => x > 0))
			if ((Convert.ToInt64(value) & requirement) == 0)
				return false;

		return true;
	}

	/// <summary>
	/// Returns the <see cref="DisplayAttribute.Name"/> value of the selected flags in a CSV string.
	/// </summary>
	/// <typeparam name="TEnum">The type of the property.</typeparam>
	/// <param name="value">The value to find the name of.</param>
	/// <param name="ignoreZero">Specifies whether to ignore the flag with a zero value.</param>
	/// <remarks>
	/// Does not support ulong as underlying type for Enum.
	/// </remarks>
	public static string GetFlaggedEnumDisplay<TEnum>(this TEnum value, bool ignoreZero = true) where TEnum : Enum
	{
		var display = new List<string>();

		foreach (var flag in value.GetFlags())
		{
			if (ignoreZero && Convert.ToInt64(flag) == 0)
				continue;

			var current = flag.GetValueDisplayName();

			if (current.Length > 0)
				display.Add(current);
		}

		if (display.Count > 0)
			return string.Join(", ", display);
		else
			return value.ToString();
	}

	/// <summary>
	/// Adds the provided flag to the value.
	/// </summary>
	/// <typeparam name="TEnum"></typeparam>
	/// <param name="value">The value to add a flag to.</param>
	/// <param name="flag">The option to add.</param>
	public static TEnum SetFlag<TEnum>(this TEnum? value, TEnum flag) where TEnum : Enum
	{
		if (value == null)
			return flag;
		else if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(ulong))
			return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToUInt64(value) | Convert.ToUInt64(flag));
		else
			return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToInt64(value) | Convert.ToInt64(flag));
	}

	/// <summary>
	/// Removes the provided flag to the value.
	/// </summary>
	/// <typeparam name="TEnum"></typeparam>
	/// <param name="value">The value to remove a flag from.</param>
	/// <param name="flag">he option to remove.</param>
	public static TEnum? UnsetFlag<TEnum>(this TEnum? value, TEnum flag) where TEnum : Enum
	{
		if (value == null)
			return value;
		else if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(ulong))
			return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToUInt64(value) & ~Convert.ToUInt64(flag));
		else
			return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToInt64(value) & ~Convert.ToInt64(flag));
	}

	/// <summary>
	/// Combines a list of flagged enums into a single result.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enum to combine.</typeparam>
	/// <param name="flags">A collection of flagged enum values to combine.</param>
	public static TEnum? CombineFlags<TEnum>(this IEnumerable<TEnum> flags) where TEnum : Enum
	{
		if (flags.Any() == false)
			return default;

		var value = flags.First();

		foreach (var flag in flags.Skip(1))
			value = value.SetFlag(flag);

		return value;
	}
}
