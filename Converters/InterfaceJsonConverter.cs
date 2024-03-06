using System.Text.Json;
using System.Text.Json.Serialization;

namespace easy_core;

/// <summary>
/// Provides capabilities to register and convert basic interface types to and from JSON.
/// </summary>
/// <typeparam name="TClass">The concrete type to deserialize to.</typeparam>
/// <typeparam name="TInterface">The interface type that the concrete type implements.</typeparam>
public class InterfaceJsonConverter<TClass, TInterface> : JsonConverter<TInterface> where TClass : class, TInterface, new()
{
	/// <inheritdoc/>
	public override TInterface Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return JsonSerializer.Deserialize<TClass>(ref reader, options.Clone(false)) ?? new TClass();
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, value, options.Clone(false));
	}
}
