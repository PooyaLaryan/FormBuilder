using System.Text.Json;
using System.Text.Json.Serialization;

namespace FormBuilder;

public class EmptyStringGuidConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrWhiteSpace(str))
            return Guid.NewGuid();

        if (Guid.TryParse(str, out var g))
            return g;

        throw new JsonException($"'{str}' یک Guid معتبر نیست.");
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        => writer.WriteStringValue(value);
}
