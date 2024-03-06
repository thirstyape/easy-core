namespace easy_core;

/// <summary>
/// Provides services to create and validate One-Time Passcode (OTP) codes.
/// </summary>
public static class OtpService
{
	private const int SecretLength = 15;

	/// <summary>
	/// Returns the current TOTP iteration (based on Unix epoch).
	/// </summary>
	public static long CurrentIteration => (long)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds / 30L);

	/// <summary>
	/// Checks that the provided secret and code are equal for the current iteration.
	/// </summary>
	/// <param name="secret">The user secret key.</param>
	/// <param name="code">The current code to check.</param>
	/// <param name="additionalIterations">A number of iterations to check outside of the current iteration (to ease time sync).</param>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <remarks>
	/// Implements <see href="https://www.ietf.org/rfc/rfc6238.txt">Request for Comments: 6238</see>.
	/// </remarks>
	public static bool CheckOtpCode(string secret, string code, int additionalIterations = 1)
	{
		if (secret.Length != SecretLength)
			throw new ArgumentException("The provided secret was an invalid length", nameof(secret));

		if (additionalIterations < 0 || additionalIterations > 10)
			throw new ArgumentOutOfRangeException(nameof(additionalIterations), "The range of iterations is 0 to 10");

		if (code == GetOtpCode(secret))
			return true;

		for (int i = 1; i < additionalIterations + 1; i++)
			if (code == GetOtpCode(secret, CurrentIteration + i) || code == GetOtpCode(secret, CurrentIteration - i))
				return true;

		return false;
	}

	/// <summary>
	/// Returns the current One-Time Passcode (OTP) for the provided inputs (TOTP).
	/// </summary>
	/// <param name="secret">The user secret key.</param>
	/// <exception cref="ArgumentException"></exception>
	/// <remarks>
	/// Implements <see href="https://www.ietf.org/rfc/rfc6238.txt">Request for Comments: 6238</see>.
	/// </remarks>
	public static string GetOtpCode(string secret)
	{
		return GetOtpCode(secret, CurrentIteration, 6);
	}

	/// <summary>
	/// Returns the current One-Time Passcode (OTP) for the provided inputs (COTP).
	/// </summary>
	/// <param name="secret">The user secret key.</param>
	/// <param name="iterationNumber">The iteration number to create the code for.</param>
	/// <exception cref="ArgumentException"></exception>
	/// <remarks>
	/// Implements <see href="https://www.ietf.org/rfc/rfc4226.txt">Request for Comments: 4226</see>.
	/// </remarks>
	public static string GetOtpCode(string secret, long iterationNumber)
	{
		return GetOtpCode(secret, iterationNumber, 6);
	}

	/// <summary>
	/// Returns the current One-Time Passcode (OTP) for the provided inputs (COTP).
	/// </summary>
	/// <param name="secret">The user secret key.</param>
	/// <param name="iterationNumber">The iteration number to create the code for.</param>
	/// <param name="digits">The length of the code to return.</param>
	/// <exception cref="ArgumentException"></exception>
	/// <remarks>
	/// Implements <see href="https://www.ietf.org/rfc/rfc4226.txt">Request for Comments: 4226</see>.
	/// </remarks>
	public static string GetOtpCode(string secret, long iterationNumber, int digits)
	{
		if (secret.Length != SecretLength)
			throw new ArgumentException("The provided secret was an invalid length", nameof(secret));

		if (iterationNumber < 0)
			throw new ArgumentException("The current iteration cannot be below 0", nameof(iterationNumber));

		if (digits < 0 || digits > 24)
			throw new ArgumentException("The range of return value lengths is 1 to 24", nameof(digits));

		var counter = BitConverter.GetBytes(iterationNumber);

		if (BitConverter.IsLittleEndian)
			Array.Reverse(counter);

		var hash = HashingService.CreateHash(secret, counter);
		var offset = hash[^1] & 0xf;

		var binary =
			((hash[offset] & 0x7f) << 24) |
			((hash[offset + 1] & 0xff) << 16) |
			((hash[offset + 2] & 0xff) << 8) |
			(hash[offset + 3] & 0xff);

		var password = binary % (int)Math.Pow(10, digits);

		return password.ToString(new string('0', digits));
	}

	/// <summary>
	/// Generates a random key to be used as the OTP secret for a user.
	/// </summary>
	/// <param name="useBase32">Specifies whether the return value is encoded in base-32.</param>
	public static string GetOtpSecret(bool useBase32 = false)
	{
		var key = EncryptionService.NewKey(SecretLength, CharacterSetGroups.Uppercase | CharacterSetGroups.Numeric);

		if (useBase32)
			return Base32Converter.EncodeBase32String(key);
		else
			return key;
	}
}
