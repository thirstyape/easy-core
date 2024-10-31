using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace easy_core;

/// <summary>
/// Provides services to create cryptography certificates.
/// </summary>
public static class CertificateBuilder
{
	/// <summary>
	/// Generates a new X509 certificate using RSA, SHA256, and Pkcs1.
	/// </summary>
	/// <param name="name">The name to assign the certificate.</param>
	/// <param name="expiry">The duration until the certificate expires.</param>
	public static X509Certificate2 CreateCertificate(string name, TimeSpan expiry)
	{
		var now = DateTimeOffset.Now;
		var request = new CertificateRequest($"CN={name}", RSA.Create(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
		return request.CreateSelfSigned(now, now.Add(expiry));
	}

	/// <summary>
	/// Checks whether the specified certificate is still valid or has expired.
	/// </summary>
	/// <param name="path">The full path to the certificate file.</param>
	/// <param name="days">The number of days to adjust the expiry check by.</param>
	public static bool CheckCertificateExpiry(string path, int days = 0)
	{
		if (File.Exists(path) == false)
			return false;

		var certificate = X509Certificate.CreateFromCertFile(path);

		if (certificate == null)
			return false;

		var date = days == 0 ? DateTime.Now : DateTime.Now.AddDays(days);
		return DateTime.TryParse(certificate.GetExpirationDateString(), out var expiry) && expiry > date;
	}

	/// <summary>
	/// Saves the provided certificate to a PFX file (PKCS #12) with private key.
	/// </summary>
	/// <param name="path">The full path to save the certificate at.</param>
	/// <param name="password">The password to protect the certificate with.</param>
	/// <param name="certificate">The certificate to save.</param>
	public static void SaveCertificateToPfx(string path, string password, X509Certificate2 certificate)
	{
		File.WriteAllBytes(path, certificate.Export(X509ContentType.Pfx, password));
	}

	/// <summary>
	/// Saves the public portion of the provided certificate to a CER file.
	/// </summary>
	/// <param name="path">The full path to save the certificate at.</param>
	/// <param name="certificate">The certificate to save.</param>
	public static void SaveCertificatePublicKey(string path, X509Certificate2 certificate)
	{
		// Create certificate text
		var builder = new StringBuilder();

		builder.AppendLine("-----BEGIN CERTIFICATE-----");
		builder.AppendLine(Convert.ToBase64String(certificate.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
		builder.AppendLine("-----END CERTIFICATE-----");

		// Create Base 64 encoded CER (public key only)
		File.WriteAllText(path, builder.ToString());
	}
}
