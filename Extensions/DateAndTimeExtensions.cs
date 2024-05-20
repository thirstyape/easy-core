namespace easy_core;

/// <summary>
/// Extension methods related to <see cref="DateTime"/>, <see cref="DateOnly"/>, <see cref="TimeSpan"/>, and <see cref="TimeOnly"/> values.
/// </summary>
public static class DateAndTimeExtensions
{
	/// <inheritdoc cref="DateOnly.ToDateTime(TimeOnly)" />
	public static DateTime ToDateTime(this DateOnly value)
	{
		return value.ToDateTime(TimeOnly.MinValue);
	}

	/// <summary>
	/// Returns a <see cref="DateTime"/> for each day in the provided month.
	/// </summary>
	/// <param name="date">The date to return days in month for.</param>
	/// <param name="preserveTime">Specifies whether to keep the original time value.</param>
	public static IEnumerable<DateTime> GetDaysInMonth(this DateTime date, bool preserveTime = false)
	{
		var days = DateTime.DaysInMonth(date.Year, date.Month);

		if (preserveTime)
			for (var day = 1; day <= days; day++)
				yield return new DateTime(date.Year, date.Month, day).Add(date.TimeOfDay);
		else
			for (var day = 1; day <= days; day++)
				yield return new DateTime(date.Year, date.Month, day);
	}

	/// <summary>
	/// Returns a <see cref="DateOnly"/> for each day in the provided month.
	/// </summary>
	/// <param name="date">The date to return days in month for.</param>
	public static IEnumerable<DateOnly> GetDaysInMonth(this DateOnly date)
	{
		var days = DateTime.DaysInMonth(date.Year, date.Month);

		for (var day = 1; day <= days; day++)
			yield return new DateOnly(date.Year, date.Month, day);
	}

	/// <summary>
	/// Returns the Xth instance of the specified day in the month.
	/// </summary>
	/// <param name="dayInMonth">The source date to use, any day in the desired month.</param>
	/// <param name="dayOfWeek">The day of the week to find.</param>
	/// <param name="xth">The sequential instance of the day to find (valid options are 1 to 5).</param>
	/// <param name="preserveTime">Specifies whether to keep the original time value.</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <remarks>
	/// As an example, to find the last Sunday in a month, set dayOfWeek to <see cref="DayOfWeek.Sunday"/> and xth to 5. If the month has 5 Sundays it will return the 5th, otherwise the 4th.
	/// </remarks>
	public static DateTime GetXthDayOfWeekInMonth(this DateTime dayInMonth, DayOfWeek dayOfWeek, int xth, bool preserveTime = false)
	{
		if (xth < 1 || xth > 5)
			throw new ArgumentOutOfRangeException(nameof(xth), "Must specify value between 1 and 5.");

		var possibilities = dayInMonth
			.GetDaysInMonth(preserveTime)
			.Where(x => x.DayOfWeek == dayOfWeek)
			.Select((x, i) => new { Date = x, Index = i })
			.ToList();

		var match = possibilities.SingleOrDefault(x => (x.Index + 1) == xth);

		if (match != null)
			return match.Date;
		else
			return possibilities.Single(x => (x.Index + 1) == 4).Date;
	}

	/// <summary>
	/// Returns the Xth instance of the specified day in the month.
	/// </summary>
	/// <param name="dayInMonth">The source date to use, any day in the desired month.</param>
	/// <param name="dayOfWeek">The day of the week to find.</param>
	/// <param name="xth">The sequential instance of the day to find (valid options are 1 to 5).</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <remarks>
	/// As an example, to find the last Sunday in a month, set dayOfWeek to <see cref="DayOfWeek.Sunday"/> and xth to 5. If the month has 5 Sundays it will return the 5th, otherwise the 4th.
	/// </remarks>
	public static DateOnly GetXthDayOfWeekInMonth(this DateOnly dayInMonth, DayOfWeek dayOfWeek, int xth)
	{
		if (xth < 1 || xth > 5)
			throw new ArgumentOutOfRangeException(nameof(xth), "Must specify value between 1 and 5.");

		var possibilities = dayInMonth
			.GetDaysInMonth()
			.Where(x => x.DayOfWeek == dayOfWeek)
			.Select((x, i) => new { Date = x, Index = i })
			.ToList();

		var match = possibilities.SingleOrDefault(x => (x.Index + 1) == xth);

		if (match != null)
			return match.Date;
		else
			return possibilities.Single(x => (x.Index + 1) == 4).Date;
	}

	/// <summary>
	/// Returns the Xth day of the specified month.
	/// </summary>
	/// <param name="dayInMonth">The source date to use, any day in the desired month.</param>
	/// <param name="xth">The day of the month to find (valid options are 1 to 31).</param>
	/// <param name="preserveTime">Specifies whether to keep the original time value.</param>
	/// <param name="tryNextYear">Specifies whether to try the next year when requested date is not found (useful for leap years).</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <remarks>
	/// Will return highest day of month if out of range xth is provided (ex. returns 30th in November when 31st is requested).
	/// </remarks>
	public static DateTime GetXthDayInMonth(this DateTime dayInMonth, int xth, bool preserveTime = false, bool tryNextYear = false)
	{
		if (xth < 1 || xth > 31)
			throw new ArgumentOutOfRangeException(nameof(xth), "Must specify value between 1 and 31.");

		if (tryNextYear)
		{
			int days;
			var i = 0;
			var check = dayInMonth;

			do
			{
				days = DateTime.DaysInMonth(check.Year, check.Month);
				i++;

				if (xth > days)
					check = check.AddYears(1);
			} while (xth > days && i < 4);

			if (xth <= days && check != dayInMonth)
				dayInMonth = check;
		}

		return dayInMonth.GetDaysInMonth(preserveTime).GetXthDayInMonth(xth);
	}

	/// <summary>
	/// Returns the Xth day of the specified month.
	/// </summary>
	/// <param name="dayInMonth">The source date to use, any day in the desired month.</param>
	/// <param name="xth">The day of the month to find (valid options are 1 to 31).</param>
	/// <param name="tryNextYear">Specifies whether to try the next year when requested date is not found (useful for leap years).</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <remarks>
	/// Will return highest day of month if out of range xth is provided (ex. returns 30th in November when 31st is requested).
	/// </remarks>
	public static DateOnly GetXthDayInMonth(this DateOnly dayInMonth, int xth, bool tryNextYear = false)
	{
		if (xth < 1 || xth > 31)
			throw new ArgumentOutOfRangeException(nameof(xth), "Must specify value between 1 and 31.");

		if (tryNextYear)
		{
			int days;
			var i = 0;
			var check = dayInMonth;

			do
			{
				days = DateTime.DaysInMonth(check.Year, check.Month);
				i++;

				if (xth > days)
					check = check.AddYears(1);
			} while (xth > days && i < 4);

			if (xth <= days && check != dayInMonth)
				dayInMonth = check;
		}

		return dayInMonth.GetDaysInMonth().GetXthDayInMonth(xth);
	}

	/// <summary>
	/// Returns the Xth day from the provided range.
	/// </summary>
	private static DateTime GetXthDayInMonth(this IEnumerable<DateTime> days, int xth)
	{
		var match = days.FirstOrNull(x => x.Day == xth);

		if (match != null)
			return match.Value;
		else if (xth > 28)
			return days.GetXthDayInMonth(--xth);
		else
			return DateTime.MaxValue;
	}

	/// <summary>
	/// Returns the Xth day from the provided range.
	/// </summary>
	private static DateOnly GetXthDayInMonth(this IEnumerable<DateOnly> days, int xth)
	{
		var match = days.FirstOrNull(x => x.Day == xth);

		if (match != null)
			return match.Value;
		else if (xth > 28)
			return days.GetXthDayInMonth(--xth);
		else
			return DateOnly.MaxValue;
	}

	/// <summary>
	/// Returns the next occurrence of the specified day of week or the current date if already on the specified day of week.
	/// </summary>
	/// <param name="date">The date to use as a base.</param>
	/// <param name="day">The day of the week requested.</param>
	public static DateTime GetNextWeekday(this DateTime date, DayOfWeek day) => date.AddDays((day - date.DayOfWeek + 7) % 7);

	/// <summary>
	/// Returns the next occurrence of the specified day of week or the current date if already on the specified day of week.
	/// </summary>
	/// <param name="date">The date to use as a base.</param>
	/// <param name="day">The day of the week requested.</param>
	public static DateOnly GetNextWeekday(this DateOnly date, DayOfWeek day) => date.AddDays((day - date.DayOfWeek + 7) % 7);

	/// <summary>
	/// Returns the previous occurrence of the specified day of week or the current date if already on the specified day of week.
	/// </summary>
	/// <param name="date">The date to use as a base.</param>
	/// <param name="day">The day of the week requested.</param>
	public static DateTime GetPreviousWeekday(this DateTime date, DayOfWeek day) => date.AddDays(-((date.DayOfWeek - day + 7) % 7));

	/// <summary>
	/// Returns the previous occurrence of the specified day of week or the current date if already on the specified day of week.
	/// </summary>
	/// <param name="date">The date to use as a base.</param>
	/// <param name="day">The day of the week requested.</param>
	public static DateOnly GetPreviousWeekday(this DateOnly date, DayOfWeek day) => date.AddDays(-((date.DayOfWeek - day + 7) % 7));

	/// <summary>
	/// Provides an enumerable collection of <see cref="DateTime"/> for each day between the start and end.
	/// </summary>
	/// <param name="start">The date to start at.</param>
	/// <param name="end">The date to end at.</param>
	/// <param name="step">The number of days use as an interval.</param>
	public static IEnumerable<DateTime> ListDaysTo(this DateTime start, DateTime end, int step = 1)
	{
		for (var day = start.Date; day.Date <= end.Date; day = day.AddDays(step))
			yield return day;
	}

	/// <summary>
	/// Provides an enumerable collection of <see cref="DateTime"/> for each day between the start and end.
	/// </summary>
	/// <param name="start">The date to start at.</param>
	/// <param name="end">The date to end at.</param>
	/// <param name="step">The number of days use as an interval.</param>
	public static IEnumerable<DateOnly> ListDaysTo(this DateOnly start, DateOnly end, int step = 1)
	{
		for (var day = start; day <= end; day = day.AddDays(step))
			yield return day;
	}

	/// <summary>
	/// Formats the provided <see cref="TimeSpan"/> into a custom string.
	/// </summary>
	/// <param name="timeSpan">The timespan to format.</param>
	/// <param name="showDays">Specifies whether to display the days component.</param>
	/// <param name="showHours">Specifies whether to display the hours component.</param>
	/// <param name="showMinutes">Specifies whether to display the hours component.</param>
	/// <param name="showSeconds">Specifies whether to display the minutes component.</param>
	/// <remarks>
	/// Typical usage is to return hours with values greater than 24 or minutes with values greater than 60.
	/// </remarks>
	public static string ToCustomString(this TimeSpan timeSpan, bool showDays = false, bool showHours = true, bool showMinutes = true, bool showSeconds = true)
	{
		var result = "";

		// Days
		if (showDays)
			result += (int)Math.Floor(timeSpan.TotalDays);

		// Hours
		if (showDays == false && showHours)
			result += (int)Math.Floor(timeSpan.TotalHours);
		else if (showDays && showHours)
			result += $".{((int)Math.Floor(timeSpan.TotalHours - (Math.Floor(timeSpan.TotalDays) * 24))).ToString().PadLeft(2, '0')}";

		// Minutes
		if (showHours == false && showMinutes)
			result += (int)Math.Floor(timeSpan.TotalMinutes);
		else if (showHours && showMinutes)
			result += $":{((int)Math.Floor(timeSpan.TotalMinutes - (Math.Floor(timeSpan.TotalHours) * 60))).ToString().PadLeft(2, '0')}";

		// Seconds
		if (showMinutes == false && showSeconds)
			result += (int)Math.Floor(timeSpan.TotalSeconds);
		else if (showMinutes && showSeconds)
			result += $":{((int)Math.Floor(timeSpan.TotalSeconds - (Math.Floor(timeSpan.TotalMinutes) * 60))).ToString().PadLeft(2, '0')}";

		return result;
	}

	/// <summary>
	/// Returns the dates in the specified month.
	/// </summary>
	/// <param name="year">The year to list dates for.</param>
	/// <param name="month">The month in the year to list dates for.</param>
	public static IEnumerable<DateTime> ListDatesInMonth(int year, int month)
	{
		var start = new DateTime(year, month, 1);
		var end = start.AddMonths(1).AddDays(-1);

		return start.ListDaysTo(end);
	}

	/// <summary>
	/// Rounds the provided timespan according to the selected rounding option.
	/// </summary>
	/// <param name="value">The timespan to round.</param>
	/// <param name="roundToDay">Rounds the provided value to the nearest day.</param>
	/// <param name="roundToHour">Rounds the provided value to the nearest hour.</param>
	/// <param name="roundToMinute">Rounds the provided value to the nearest minute.</param>
	/// <param name="roundToSecond">Rounds the provided value to the nearest second.</param>
	/// <param name="roundToMillisecond">Rounds the provided value to the nearest millisecond.</param>
	/// <param name="roundToNearest">Specifies whether to apply <see cref="Math.Round(double, int)"/> or <see cref="Math.Floor(double)"/>.</param>
	/// <exception cref="ArgumentException"></exception>
	/// <remarks>
	/// If more than one rounding interval is selected, the largest one will be returned (i.e. if roundToDay and roundToSecond are both <see langword="true"/>, the result will be rounded to the nearest day).
	/// </remarks>
	public static TimeSpan Round(this TimeSpan value, bool roundToDay = false, bool roundToHour = false, bool roundToMinute = false, bool roundToSecond = true, bool roundToMillisecond = false, bool roundToNearest = true)
	{
		if (roundToDay)
			return TimeSpan.FromDays(roundToNearest ? Math.Round(value.TotalDays, 0) : Math.Floor(value.TotalDays));
		else if (roundToHour)
			return TimeSpan.FromHours(roundToNearest ? Math.Round(value.TotalHours, 0) : Math.Floor(value.TotalHours));
		else if (roundToMinute)
			return TimeSpan.FromMinutes(roundToNearest ? Math.Round(value.TotalMinutes, 0) : Math.Floor(value.TotalMinutes));
		else if (roundToSecond)
			return TimeSpan.FromSeconds(roundToNearest ? Math.Round(value.TotalSeconds, 0) : Math.Floor(value.TotalSeconds));
		else if (roundToMillisecond)
			return TimeSpan.FromMilliseconds(roundToNearest ? Math.Round(value.TotalMilliseconds, 0) : Math.Floor(value.TotalMilliseconds));
		else
			throw new ArgumentException("Must select one rounding mode.");
	}

	/// <inheritdoc cref="Enumerable.Sum(IEnumerable{long})"/>
	public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan> selector)
	{
		var sum = 0L;

		checked
		{
			foreach (TSource item in source)
				sum += selector(item).Ticks;
		}

		return TimeSpan.FromTicks(sum);
	}

	/// <summary>
	/// Returns the timespan in HH:mm format, with any days converted into hours.
	/// </summary>
	/// <param name="value">The value to format.</param>
	public static string ToHhMm(this TimeSpan value)
	{
		if (value >= TimeSpan.Zero)
			return $"{((int)value.TotalHours).ToString().PadLeft(2, '0')}:{value:mm}";
		else
			return $"-{((int)value.TotalHours).ToString().PadLeft(2, '0')}:{value:mm}";
	}

	/// <summary>
	/// Returns the timeonly in HH:mm format, with any days converted into hours.
	/// </summary>
	/// <param name="value">The value to format.</param>
	public static string ToHhMm(this TimeOnly value)
	{
		return DateTime.Today.Add(value.ToTimeSpan()).ToString("HH:mm");
	}

	/// <summary>
	/// Returns the timespan in hh:mm format, with the AM or PM specifier at the end.
	/// </summary>
	/// <param name="value">The value to format.</param>
	/// <exception cref="ArgumentException"></exception>
	public static string ToAmPm(this TimeSpan value)
	{
		if (value >= TimeSpan.Zero && value < TimeSpan.FromDays(1))
			return DateTime.Today.Add(value).ToString("t");
		else
			throw new ArgumentException("Value must be between zero and 24 hours.", nameof(value));
	}

	/// <summary>
	/// Returns the timeonly in hh:mm format, with the AM or PM specifier at the end.
	/// </summary>
	/// <param name="value">The value to format.</param>
	public static string ToAmPm(this TimeOnly value)
	{
		return DateTime.Today.Add(value.ToTimeSpan()).ToString("t");
	}
}
