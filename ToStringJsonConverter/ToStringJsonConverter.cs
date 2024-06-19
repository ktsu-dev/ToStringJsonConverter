// Ignore Spelling: Stringify

namespace ktsu.io.ToStringJsonConverter;

using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ktsu.io.Extensions;

public class ToStringJsonConverterFactory : JsonConverterFactory
{
	public override bool CanConvert(Type typeToConvert)
	{
		ArgumentNullException.ThrowIfNull(typeToConvert);
		return typeToConvert.TryFindMethod("FromString", BindingFlags.Static | BindingFlags.Public, out var method) &&
			method is not null && method.GetParameters().Length != 0 &&
			method.GetParameters()[0].ParameterType == typeof(string);
	}

	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		ArgumentNullException.ThrowIfNull(typeToConvert);
		var converterType = typeof(ToStringJsonConverter<>).MakeGenericType(typeToConvert);
		return (JsonConverter)Activator.CreateInstance(converterType, BindingFlags.Instance | BindingFlags.Public, binder: null, args: null, culture: null)!;
	}

	private sealed class ToStringJsonConverter<T> : JsonConverter<T>
	{
		private static readonly MethodInfo? FromStringMethod;

		static ToStringJsonConverter()
		{
			if (typeof(T).TryFindMethod("FromString", BindingFlags.Static | BindingFlags.Public, out FromStringMethod))
			{
				Debug.Assert(FromStringMethod is not null);
				if (FromStringMethod.ContainsGenericParameters)
				{
					FromStringMethod = FromStringMethod.MakeGenericMethod(typeof(T));
				}
			}
		}

		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			ArgumentNullException.ThrowIfNull(typeToConvert);

			return reader.TokenType == JsonTokenType.String
				? (T)FromStringMethod!.Invoke(null, [reader.GetString()!])!
				: throw new JsonException();
		}

		public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			ArgumentNullException.ThrowIfNull(typeToConvert);

			return (T)FromStringMethod!.Invoke(null, [reader.GetString()!])!;
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		{
			ArgumentNullException.ThrowIfNull(value);
			ArgumentNullException.ThrowIfNull(writer);

			writer.WriteStringValue(value.ToString());
		}

		public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		{
			ArgumentNullException.ThrowIfNull(value);
			ArgumentNullException.ThrowIfNull(writer);

			writer.WritePropertyName(value.ToString()!);
		}
	}
}
