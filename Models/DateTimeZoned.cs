namespace easy_core;

/// <summary>
/// This value type represents a date and time with a specific time zone applied. If no time zone is provided, the local system time zone will be used.
/// </summary>
public readonly struct DateTimeZoned : IComparable, IComparable<DateTimeZoned>, IEquatable<DateTimeZoned>
{
	/// <summary>
	/// Creates a new zoned <see cref="DateTime"/> with the system time zone.
	/// </summary>
	/// <param name="dateTime">The local <see cref="DateTime"/> to apply a time zone to.</param>
	public DateTimeZoned(DateTime dateTime)
	{
		var local = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);

		UniversalTime = TimeZoneInfo.ConvertTimeToUtc(local, TimeZoneInfo.Local);
		TimeZone = TimeZoneInfo.Local;
	}

	/// <summary>
	/// Creates a new zoned <see cref="DateTime"/> with the specified time zone.
	/// </summary>
	/// <param name="dateTime">The <see cref="DateTime"/> to apply a time zone to.</param>
	/// <param name="timeZone">The time zone to apply.</param>
	/// <remarks>
	/// Assumes the provided <see cref="DateTime"/> is from the specified time zone.
	/// </remarks>
	public DateTimeZoned(DateTime dateTime, TimeZoneInfo timeZone)
	{
		var unspecified = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);

		UniversalTime = TimeZoneInfo.ConvertTimeToUtc(unspecified, timeZone);
		TimeZone = timeZone;
	}

	/// <summary>
	/// Creates a new zoned <see cref="DateTime"/> with the specified time zone.
	/// </summary>
	/// <param name="dateTime">The <see cref="DateTime"/> to apply a time zone to.</param>
	/// <param name="timeZone">The time zone to apply.</param>
	/// <remarks>
	/// Assumes the provided <see cref="DateTime"/> is from the specified time zone.
	/// </remarks>
	public DateTimeZoned(DateTime dateTime, string timeZone)
	{
		var unspecified = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
		var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

		UniversalTime = TimeZoneInfo.ConvertTimeToUtc(unspecified, timeZoneInfo);
		TimeZone = timeZoneInfo;
	}

	/// <summary>
	/// The UTC <see cref="DateTime"/> for the stored value.
	/// </summary>
	public DateTime UniversalTime { get; init; }

	/// <summary>
	/// The selected time zone.
	/// </summary>
	public TimeZoneInfo TimeZone { get; init; }

	/// <summary>
	/// The localized <see cref="DateTime"/> for the stored value.
	/// </summary>
	public DateTime LocalTime => TimeZoneInfo.ConvertTime(UniversalTime, TimeZone);

	/// <summary>
	/// Specifies whether UTC and localized values are the same.
	/// </summary>
	public bool IsUtc => UniversalTime == LocalTime;

	/// <summary>
	/// Returns a new <see cref="DateTimeZoned"/> with the current <see cref="LocalTime"/> converted to the target time zone.
	/// </summary>
	/// <param name="timeZone">The time zone to convert to.</param>
	public DateTimeZoned ConvertTo(string timeZone)
	{
		var converted = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(LocalTime, TimeZone.Id, timeZone);
		return new DateTimeZoned(converted, timeZone);
	}

	/// <summary>
	/// Returns a new <see cref="DateTimeZoned"/> with the current <see cref="LocalTime"/> converted to the target time zone.
	/// </summary>
	/// <param name="timeZone">The time zone to convert to.</param>
	public DateTimeZoned ConvertTo(TimeZoneInfo timeZone)
	{
		var converted = TimeZoneInfo.ConvertTime(LocalTime, TimeZone, timeZone);
		return new DateTimeZoned(converted, timeZone.Id);
	}

	/// <summary>
	/// Returns the value as a string in the round-trip date/time pattern.
	/// </summary>
	/// <param name="format">The <see cref="DateTime"/> format to apply to the resulting string (default "o").</param>
	public string ToLocalString(string? format = "o")
	{
		return new DateTimeOffset(LocalTime, TimeZone.BaseUtcOffset).ToString(format);
	}

	/// <summary>
	/// Returns the value as a string in the universal sortable date/time pattern.
	/// </summary>
	/// <param name="format">The <see cref="DateTime"/> format to apply to the resulting string (default "u").</param>
	public string ToUniversalString(string? format = "u")
	{
		return UniversalTime.ToString(format);
	}

	/// <summary>
	/// Returns a <see cref="DateTime"/> representing the current date and time adjusted to the system time zone.
	/// </summary>
	/// <remarks>
	/// This is functionally equivalent to <see cref="DateTime.Now"/> and has been added for completeness.
	/// </remarks>
	public static DateTime Now() => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Local);

	/// <summary>
	/// Returns a <see cref="DateTime"/> representing the current date and time adjusted to the specified time zone.
	/// </summary>
	/// <param name="timeZone">The time zone to apply.</param>
	public static DateTime Now(TimeZoneInfo timeZone) => TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);

	/// <summary>
	/// Returns a <see cref="DateTime"/> representing the current date and time adjusted to the specified time zone.
	/// </summary>
	/// <param name="timeZone">The time zone to apply.</param>
	public static DateTime Now(string timeZone)
	{
		var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
		return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo);
	}

	/// <summary>
	/// Returns a <see cref="DateTime"/> representing the current date adjusted to the system time zone with the time component set to 00:00:00.
	/// </summary>
	/// <remarks>
	/// This is functionally equivalent to <see cref="DateTime.Today"/> and has been added for completeness.
	/// </remarks>
	public static DateTime Today() => Now().Date;

	/// <summary>
	/// Returns a <see cref="DateTime"/> representing the current date adjusted to the specified time zone with the time component set to 00:00:00.
	/// </summary>
	/// <param name="timeZone">The time zone to apply.</param>
	public static DateTime Today(TimeZoneInfo timeZone) => Now(timeZone).Date;

	/// <summary>
	/// Returns a <see cref="DateTime"/> representing the current date adjusted to the specified time zone with the time component set to 00:00:00.
	/// </summary>
	/// <param name="timeZone">The time zone to apply.</param>
	public static DateTime Today(string timeZone) => Now(timeZone).Date;

	/// <inheritdoc/>
	public override bool Equals(object? value)
	{
		return value is DateTimeZoned d2 && this == d2;
	}

	/// <inheritdoc/>
	public bool Equals(DateTimeZoned value)
	{
		return this == value;
	}

	/// <summary>
	/// Compares two <see cref="DateTimeZoned"/> values for equality.
	/// </summary>
	/// <param name="d1">The first value to compare.</param>
	/// <param name="d2">The second value to compare.</param>
	/// <returns>
	/// Returns <see langword="true"/> if the two <see cref="DateTimeZoned"/> values are equal, or <see langword="false"/> if they are not equal.
	/// </returns>
	public static bool Equals(DateTimeZoned d1, DateTimeZoned d2)
	{
		return d1 == d2;
	}

	/// <summary>
	/// Compares two <see cref="DateTimeZoned"/> values, returning an integer that indicates their relationship.
	/// </summary>
	/// <param name="d1">The first value to compare.</param>
	/// <param name="d2">The second value to compare.</param>
	/// <returns>
	/// Returns 1 if the first value is greater than the second, -1 if the second value is greater than the first, or 0 if the two values are equal.
	/// </returns>
	public static int Compare(DateTimeZoned d1, DateTimeZoned d2)
	{
		var ticks1 = d1.UniversalTime.Ticks;
		var ticks2 = d2.UniversalTime.Ticks;

		if (ticks1 > ticks2)
			return 1;
		else if (ticks1 < ticks2)
			return -1;
		else
			return 0;
	}

	/// <inheritdoc/>
	public int CompareTo(object? value)
	{
		if (value == null)
			return 1;

		if (value is not DateTimeZoned)
			throw new ArgumentException(null, nameof(value));

		return Compare(this, (DateTimeZoned)value);
	}

	/// <inheritdoc/>
	public int CompareTo(DateTimeZoned value)
	{
		return Compare(this, value);
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var ticks = UniversalTime.Ticks;
		return unchecked((int)ticks) ^ (int)(ticks >> 32);
	}

	public static TimeSpan operator -(DateTimeZoned d1, DateTimeZoned d2) => new(d1.UniversalTime.Ticks - d2.UniversalTime.Ticks);

	public static bool operator ==(DateTimeZoned d1, DateTimeZoned d2) => d1.UniversalTime.Ticks == d2.UniversalTime.Ticks;

	public static bool operator !=(DateTimeZoned d1, DateTimeZoned d2) => d1.UniversalTime.Ticks != d2.UniversalTime.Ticks;

	public static bool operator <(DateTimeZoned d1, DateTimeZoned d2) => d1.UniversalTime.Ticks < d2.UniversalTime.Ticks;

	public static bool operator <=(DateTimeZoned d1, DateTimeZoned d2) => d1.UniversalTime.Ticks <= d2.UniversalTime.Ticks;

	public static bool operator >(DateTimeZoned d1, DateTimeZoned d2) => d1.UniversalTime.Ticks > d2.UniversalTime.Ticks;

	public static bool operator >=(DateTimeZoned d1, DateTimeZoned d2) => d1.UniversalTime.Ticks >= d2.UniversalTime.Ticks;
}
