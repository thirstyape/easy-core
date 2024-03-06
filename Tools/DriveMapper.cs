using System.Diagnostics;
using System.Runtime.InteropServices;

namespace easy_core;

/// <summary>
/// Connects and disconnects network locations as mapped drives.
/// </summary>
public static class DriveMapper
{
	/// <summary>
	/// Unmounts and removes an existing network drive then creates and mounts the network drive.
	/// </summary>
	/// <param name="path">The network path to mount.</param>
	/// <param name="username">The username to use to log into the share.</param>
	/// <param name="password">The password to use to log into the share.</param>
	/// <param name="letter">An optional letter to assign to the share (Windows only).</param>
	/// <param name="mountPoint">The local system path to connect the network share to (macOS and Linux only).</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="PlatformNotSupportedException"></exception>
	/// <remarks>
	/// Equivalent to running <see cref="RemoveDriveMap(string, char?, string?)"/> then <see cref="AddDriveMap(string, string, string, char?, string?)"/>.
	/// </remarks>
	public static bool RecreateDriveMap(string path, string username, string password, char? letter = null, string? mountPoint = null)
	{
		RemoveDriveMap(path, letter, mountPoint);
		return AddDriveMap(path, username, password, letter, mountPoint);
	}

	/// <summary>
	/// Creates and mounts a new network drive.
	/// </summary>
	/// <param name="path">The network path to mount.</param>
	/// <param name="username">The username to use to log into the share.</param>
	/// <param name="password">The password to use to log into the share.</param>
	/// <param name="letter">An optional letter to assign to the share (Windows only).</param>
	/// <param name="mountPoint">The local system path to connect the network share to (macOS and Linux only).</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="PlatformNotSupportedException"></exception>
	public static bool AddDriveMap(string path, string username, string password, char? letter = null, string? mountPoint = null)
	{
		if (string.IsNullOrWhiteSpace(path))
			throw new ArgumentNullException(nameof(path));

		ProcessStartInfo startInfo;

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			startInfo = new ProcessStartInfo
			{
				FileName = "net.exe",
				Arguments = letter == null ? $"use \"{path.Replace('/', '\\')}\" /user:{username} {password}" : $"use {letter}: \"{path.Replace('/', '\\')}\" /user:{username} {password}",
				UseShellExecute = false,
				CreateNoWindow = true
			};
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			if (string.IsNullOrWhiteSpace(mountPoint))
				throw new ArgumentNullException(nameof(mountPoint));

			startInfo = new ProcessStartInfo
			{
				FileName = "sudo",
				Arguments = $"mount -t cifs -o rw,username={username},password={password},iocharset=utf8 \"{path.Replace('\\', '/')}\" \"{mountPoint.Replace('\\', '/')}\"",
				UseShellExecute = false,
				CreateNoWindow = true
			};
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			if (string.IsNullOrWhiteSpace(mountPoint))
				throw new ArgumentNullException(nameof(mountPoint));

			startInfo = new ProcessStartInfo
			{
				FileName = "sudo",
				Arguments = $"mount -t smbfs //{username}:{password}@\"{path.Replace('\\', '/').TrimStart('/')}\" \"{mountPoint.Replace('\\', '/')}\"",
				UseShellExecute = false,
				CreateNoWindow = true
			};
		}
		else
		{
			throw new PlatformNotSupportedException("Mapping drives only supported on Windows, macOS, and Linux.");
		}

		try
		{
			return RunProcess(startInfo);
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Unmounts and removes an existing network drive.
	/// </summary>
	/// <param name="path">The network path to unmount.</param>
	/// <param name="letter">An optional letter assinged to the share (Windows only).</param>
	/// <param name="mountPoint">The local system path to disconnect the network share from (macOS and Linux only).</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="PlatformNotSupportedException"></exception>
	public static bool RemoveDriveMap(string path, char? letter = null, string? mountPoint = null)
	{
		ProcessStartInfo startInfo;

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			startInfo = new ProcessStartInfo
			{
				FileName = "net.exe",
				Arguments = letter == null ? $"use \"{path.Replace('/', '\\')}\" /delete" : $"use {letter}: \"{path.Replace('/', '\\')}\" /delete",
				UseShellExecute = false,
				CreateNoWindow = true
			};
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			if (string.IsNullOrWhiteSpace(mountPoint))
				throw new ArgumentNullException(nameof(mountPoint));

			startInfo = new ProcessStartInfo
			{
				FileName = "sudo",
				Arguments = $"umount \"{mountPoint.Replace('\\', '/')}\"",
				UseShellExecute = false,
				CreateNoWindow = true
			};
		}
		else
		{
			throw new PlatformNotSupportedException("Mapping drives only supported on Windows, macOS, and Linux.");
		}

		try
		{
			return RunProcess(startInfo);
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Runs a process using the provided start info.
	/// </summary>
	/// <param name="startInfo">The details for the process to run.</param>
	private static bool RunProcess(ProcessStartInfo startInfo)
	{
		var process = new Process()
		{
			StartInfo = startInfo
		};

		process.Start();

		if (process.WaitForExit(5_000))
		{
			process.Dispose();
			return true;
		}
		else
		{
			process.Kill();
			return false;
		}
	}
}
