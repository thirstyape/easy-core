using System.Net;

namespace easy_core;

/// <summary>
/// Extension methods related to IP addresses.
/// </summary>
public static class IpAddressExtensions
{
	/// <summary>
	/// Checks to see whether the provided IP address is in the specified range.
	/// </summary>
	/// <param name="address">The address to check.</param>
	/// <param name="first">The lowest address in the range.</param>
	/// <param name="last">The highest address in the range.</param>
	public static bool IsInRange(this IPAddress address, IPAddress first, IPAddress last)
	{
		var ip = address.ToLong();
		return ip >= first.ToLong() && ip <= last.ToLong();
	}

	/// <summary>
	/// Returns whether the provided IP address is contained within a local network.
	/// </summary>
	/// <param name="address">The address to check.</param>
	/// <returns>
	/// True when the provided address is part of a class A, class B, or class C network.
	/// </returns>
	public static bool IsPrivate(this IPAddress address)
	{
		return address.IsInRange(IPAddress.Parse("10.0.0.0"), IPAddress.Parse("10.255.255.255")) ||
			address.IsInRange(IPAddress.Parse("172.16.0.0"), IPAddress.Parse("172.31.255.255")) ||
			address.IsInRange(IPAddress.Parse("192.168.0.0"), IPAddress.Parse("192.168.255.255"));
	}

	/// <summary>
	/// Returns whether the provided address is less than another.
	/// </summary>
	/// <param name="address">The address to check.</param>
	/// <param name="other">Another address to check against.</param>
	public static bool IsLessThan(this IPAddress address, IPAddress other) => address.ToLong() < other.ToLong();

	/// <summary>
	/// Returns whether the provided address is greater than another.
	/// </summary>
	/// <param name="address">The address to check.</param>
	/// <param name="other">Another address to check against.</param>
	public static bool IsGreaterThan(this IPAddress address, IPAddress other) => address.ToLong() > other.ToLong();

	/// <summary>
	/// Returns the last IP address with the provided CIDR range.
	/// </summary>
	/// <param name="address">The first address in the range.</param>
	/// <param name="cidr">The mask to add to the first address.</param>
	/// <exception cref="ArgumentException"></exception>
	/// <remarks>
	/// This method simply adds the quantity of addresses in the CIDR to the provided IP address.
	/// </remarks>
	public static IPAddress LastInRange(this IPAddress address, int cidr)
	{
		if (cidr < 0 || cidr > 32)
			throw new ArgumentException("The CIDR value must be in the range 0 to 32.", nameof(cidr));

		return new IPAddress(address.ToLong() + (long)Math.Pow(2, 32 - cidr));
	}

	/// <summary>
	/// Converts the provided IP address into a 64-bit integer.
	/// </summary>
	/// <param name="address">The address to convert.</param>
	public static long ToLong(this IPAddress address) => BitConverter.ToUInt32(address.GetAddressBytes().Reverse().ToArray(), 0);
}
