namespace easy_core;

/// <summary>
/// Defines properties to customize encrypting and decrypting values.
/// </summary>
public class EncryptionSettings
{
	/// <summary>
	/// The block size to use in encryption operations.
	/// </summary>
	public int AesBlockSize { get; set; } = 128;

	/// <summary>
	/// The length in bits for the encryption key.
	/// </summary>
	/// <remarks>
	/// Accepted values are 128, 192, and 256 (default).
	/// </remarks>
	public int AesKeySize { get; set; } = 256;

	/// <summary>
	/// The length in bits for the encryption salt.
	/// </summary>
	/// <remarks>
	/// Only applies to password based encryption.
	/// </remarks>
	public int SaltSize { get; set; } = 64;

	/// <summary>
	/// The numver of iterations to use when generating encryption keys.
	/// </summary>
	/// <remarks>
	/// Only applies to password based encryption.
	/// </remarks>
	public int KeyGenerationIterations { get; set; } = 10_000;

	/// <summary>
	/// The minimum length for a valid encryption password.
	/// </summary>
	/// <remarks>
	/// Only applies to password based encryption.
	/// </remarks>
	public int MinPasswordLength { get; set; } = 12;

	/// <summary>
	/// The <see cref="AesKeySize"/> converted from bits to bytes.
	/// </summary>
	public int AesKeyByteSize => AesKeySize / 8;

	/// <summary>
	/// The <see cref="SaltSize"/> converted from bits to bytes.
	/// </summary>
	public int SaltByteSize => SaltSize / 8;
}
