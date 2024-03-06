using System.Text.Json;
using System.Text.Json.Serialization;

namespace easy_core;

/// <summary>
/// Converts grouped values to and from their JSON representation.
/// </summary>
/// <remarks>
/// Deserialization requires values that have been serialized with this converter.
/// </remarks>
public class GroupingJsonConverter<TKey, TElement> : JsonConverter<IGrouping<TKey, TElement>>
{
	/// <inheritdoc/>
	public override IGrouping<TKey, TElement>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var result = new GenericGrouping<TKey, TElement>();

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.StartObject)
				continue;
			else if (reader.TokenType == JsonTokenType.EndObject)
				break;

			var propertyName = reader.GetString();

			if (propertyName == nameof(result.Key))
			{
				var valueConverter = (JsonConverter<TKey>)options.GetConverter(typeof(TKey));

				TKey key;

				if (valueConverter != null)
				{
					reader.Read();
					key = valueConverter.Read(ref reader, typeof(TKey), options)!;
				}
				else
				{
					key = JsonSerializer.Deserialize<TKey>(ref reader, options)!;
				}

				result.Key = key;
			}
			else if (propertyName == nameof(result.Elements))
			{
				var valueConverter = (JsonConverter<IEnumerable<TElement>>)options.GetConverter(typeof(IEnumerable<TElement>));

				IEnumerable<TElement> elements;

				if (valueConverter != null)
				{
					reader.Read();
					elements = valueConverter.Read(ref reader, typeof(IEnumerable<TElement>), options)!;
				}
				else
				{
					elements = JsonSerializer.Deserialize<IEnumerable<TElement>>(ref reader, options)!;
				}

				result.Elements = elements.ToList();
			}
		}

		return result;
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, IGrouping<TKey, TElement> value, JsonSerializerOptions options)
	{
		var generic = new GenericGrouping<TKey, TElement>(value);

		writer.WriteStartObject();

		writer.WritePropertyName(nameof(generic.Key));
		JsonSerializer.Serialize(writer, generic.Key, options);

		writer.WritePropertyName(nameof(generic.Elements));
		writer.WriteStartArray();

		foreach (var element in generic.Elements)
			JsonSerializer.Serialize(writer, element, options);

		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
