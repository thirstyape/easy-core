namespace easy_core;

/// <summary>
/// Extension methods related to streams.
/// </summary>
public static class StreamExtensions
{
	/// <summary>
	/// Copies the source stream into the destination stream and seeks the streams to 0 if possible.
	/// </summary>
	/// <param name="source">The stream to copy data from.</param>
	/// <param name="destination">The stream to copy data into.</param>
	public static void CopyAndReset<TStream>(this TStream source, Stream destination) where TStream : Stream
	{
		source.CopyTo(destination);

		if (source.CanSeek)
			source.Seek(0, SeekOrigin.Begin);

		if (destination.CanSeek)
			source.Seek(0, SeekOrigin.Begin);
	}

	/// <summary>
	/// Copies the source stream into the destination stream and seeks the streams to 0 if possible.
	/// </summary>
	/// <param name="source">The stream to copy data from.</param>
	/// <param name="destination">The stream to copy data into.</param>
	/// <param name="token">A token to cancel the operation.</param>
	public static async Task CopyAndResetAsync<TStream>(this TStream source, Stream destination, CancellationToken? token = null) where TStream : Stream
	{
		await source.CopyToAsync(destination, token ?? CancellationToken.None);

		if (source.CanSeek)
			source.Seek(0, SeekOrigin.Begin);

		if (destination.CanSeek)
			source.Seek(0, SeekOrigin.Begin);
	}

	/// <summary>
	/// Copies the source stream into a new memory stream and seeks the source to 0 if possible.
	/// </summary>
	/// <param name="stream">The stream to copy data from.</param>
	public static MemoryStream CopyAndReset<TStream>(this TStream stream) where TStream : Stream
	{
		var destination = new MemoryStream();

		stream.CopyTo(destination);

		if (stream.CanSeek)
			stream.Seek(0, SeekOrigin.Begin);

		destination.Seek(0, SeekOrigin.Begin);

		return destination;
	}

	/// <summary>
	/// Copies the source stream into a new memory stream and seeks the source to 0 if possible.
	/// </summary>
	/// <param name="stream">The stream to copy data from.</param>
	/// <param name="token">A token to cancel the operation.</param>
	public static async Task<MemoryStream> CopyAndResetAsync<TStream>(this TStream stream, CancellationToken? token = null) where TStream : Stream
	{
		var destination = new MemoryStream();

		await stream.CopyToAsync(destination, token ?? CancellationToken.None);

		if (stream.CanSeek)
			stream.Seek(0, SeekOrigin.Begin);

		destination.Seek(0, SeekOrigin.Begin);

		return destination;
	}
}
