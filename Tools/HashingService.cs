using System.Security.Cryptography;
using System.Text;

namespace easy_core;

/// <summary>
/// Provides services to hash and verify hashes of messages.
/// </summary>
public static class HashingService
{
	// Hashing and Key Derivation settings
	private const int MinimumIterations = 10_000;
	private const int HashSize = 160;
	private const int SaltSize = 128;

	private static int HashByteSize => HashSize / 8;
	private static int SaltByteSize => SaltSize / 8;

	/// <summary>
	/// Generates a new hash value for the provided message using <see cref="HMACSHA256"/>.
	/// </summary>
	/// <param name="message">The message to hash.</param>
	/// <param name="buffer">Additional data to hash.</param>
	public static byte[] CreateHash(string message, byte[] buffer)
	{
		var key = Encoding.ASCII.GetBytes(message);
		var hmac = new HMACSHA1(key);

		return hmac.ComputeHash(buffer);
	}

	/// <summary>
	/// Generates a new hash value for the provided message using PBKDF2.
	/// </summary>
	/// <param name="message">The message to hash.</param>
	/// <param name="derivationIterations">The number of iterations to run while generating the hash (default <see cref="MinimumIterations"/>).</param>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	public static string CreateHash(string message, int? derivationIterations = null)
	{
		var hash = CreateHash(message, derivationIterations ?? MinimumIterations, HashAlgorithmName.SHA512);

		return Convert.ToBase64String(hash);
	}

	/// <summary>
	/// Generates a new hash value for the provided message using PBKDF2.
	/// </summary>
	/// <param name="message">The message to hash.</param>
	/// <param name="derivationIterations">The number of iterations to run while generating the hash.</param>
	/// <param name="hashAlgorithmName">The algorithm to use when hashing.</param>
	/// <param name="secure">Specifies whether to allow only secure hash algorithms.</param>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	public static byte[] CreateHash(string message, int derivationIterations, HashAlgorithmName hashAlgorithmName, bool secure = true)
	{
		// Validate inputs
		if (string.IsNullOrWhiteSpace(message))
			throw new ArgumentNullException(nameof(message), "Must provide text to hash.");

		if (derivationIterations < MinimumIterations)
			throw new ArgumentException($"Must have at least {MinimumIterations} derivation iterations.", nameof(derivationIterations));

		if (hashAlgorithmName == HashAlgorithmName.MD5 && secure)
			throw new ArgumentException($"Must select a hash algorithm other than {nameof(HashAlgorithmName.MD5)} or set {nameof(secure)} to false.", nameof(hashAlgorithmName));

		// Create salt
		var salt = new byte[SaltByteSize];

		using var service = RandomNumberGenerator.Create();
		service.GetBytes(salt);

		// Generate hash and copy to output array
		using var pbkdf2 = new Rfc2898DeriveBytes(message, salt, derivationIterations, hashAlgorithmName);

		var hash = pbkdf2.GetBytes(HashByteSize);
		var hashBytes = new byte[SaltByteSize + HashByteSize];

		Array.Copy(salt, 0, hashBytes, 0, SaltByteSize);
		Array.Copy(hash, 0, hashBytes, SaltByteSize, HashByteSize);

		return hashBytes;
	}

	/// <summary>
	/// Compares the provided text against the provided hash to verify whether they match.
	/// </summary>
	/// <param name="message">The message to verify.</param>
	/// <param name="hash">The hash to verify against.</param>
	/// <param name="derivationIterations">The number of iterations to run while checking the hash (must be the same as on generation).</param>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	public static bool CheckHash(string message, string hash, int? derivationIterations = null)
	{
		if (string.IsNullOrWhiteSpace(hash) || hash.Length % 4 > 0)
			throw new ArgumentException("Must provide a valid hash to check against.", nameof(hash));

		var result = CheckHash(message, Convert.FromBase64String(hash), derivationIterations ?? MinimumIterations, HashAlgorithmName.SHA512);

		return result;
	}

	/// <summary>
	/// Compares the provided text against the provided hash to verify whether they match.
	/// </summary>
	/// <param name="message">The message to verify.</param>
	/// <param name="hash">The hash to verify against.</param>
	/// <param name="derivationIterations">The number of iterations to run while checking the hash (must be the same as on generation).</param>
	/// <param name="hashAlgorithmName">The algorithm to use when checking the hash(must be the same as on generation).</param>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	public static bool CheckHash(string message, byte[] hash, int derivationIterations, HashAlgorithmName hashAlgorithmName)
	{
		// Validate inputs
		if (string.IsNullOrWhiteSpace(message))
			throw new ArgumentNullException(nameof(message), "Must provide text to verify.");

		if (hash == null || hash.Length == 0)
			throw new ArgumentException("Must provide hash to check against.", nameof(hash));

		if (derivationIterations < MinimumIterations)
			throw new ArgumentException($"Must have at least {MinimumIterations} derivation iterations.", nameof(derivationIterations));

		// Extract salt
		var salt = new byte[SaltByteSize];

		Array.Copy(hash, 0, salt, 0, SaltByteSize);

		// Create check hash and compare to provided
		using var pbkdf2 = new Rfc2898DeriveBytes(message, salt, derivationIterations, hashAlgorithmName);

		var checkHash = pbkdf2.GetBytes(HashByteSize);

		for (var i = 0; i < HashByteSize; i++)
			if (hash[i + SaltByteSize] != checkHash[i])
				return false;

		return true;
	}
}
