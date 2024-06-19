// Ignore Spelling: Deserialize

namespace ktsu.io.ToStringJsonConverter.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ktsu.io.ToStringJsonConverter;

[TestClass]
public class ToStringJsonConverterFactoryTests
{
	public class TestClass
	{
		public string Value { get; set; } = string.Empty;

		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
		public static TestClass FromString(string value) => new() { Value = value };

		public override string ToString() => Value;
	}

	public sealed class TestClass<TNumber>
	{
		public string Value { get; set; } = string.Empty;

		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
		public static TestClass<TNumber> FromString<TSelf>(string value) => new() { Value = value };

		public override string ToString() => Value;

		public TNumber? Number { get; set; }
	}

	private static JsonSerializerOptions GetOptions()
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add(new ToStringJsonConverterFactory());
		return options;
	}

	[TestMethod]
	public void CanConvert_ShouldReturnTrueForValidType()
	{
		var factory = new ToStringJsonConverterFactory();
		Assert.IsTrue(factory.CanConvert(typeof(TestClass)));
		Assert.IsTrue(factory.CanConvert(typeof(TestClass<int>)));
	}

	[TestMethod]
	public void CanConvert_ShouldReturnFalseForInvalidType()
	{
		var factory = new ToStringJsonConverterFactory();
		Assert.IsFalse(factory.CanConvert(typeof(string)));
	}

	[TestMethod]
	public void CreateConverter_ShouldReturnConverterForValidType()
	{
		var factory = new ToStringJsonConverterFactory();
		var converter = factory.CreateConverter(typeof(TestClass), GetOptions());
		Assert.IsNotNull(converter);
		converter = factory.CreateConverter(typeof(TestClass<int>), GetOptions());
		Assert.IsNotNull(converter);
	}

	[TestMethod]
	public void Serialize_ShouldUseToString()
	{
		var options = GetOptions();
		var testInstance = new TestClass { Value = "test value" };
		string json = JsonSerializer.Serialize(testInstance, options);
		Assert.AreEqual("\"test value\"", json);
	}

	[TestMethod]
	public void SerializeGeneric_ShouldUseToString()
	{
		var options = GetOptions();
		var testInstance = new TestClass<int> { Value = "test value" };
		string json = JsonSerializer.Serialize(testInstance, options);
		Assert.AreEqual("\"test value\"", json);
	}

	[TestMethod]
	public void Deserialize_ShouldUseFromString()
	{
		var options = GetOptions();
		string json = "\"test value\"";
		var testInstance = JsonSerializer.Deserialize<TestClass>(json, options);
		Assert.IsNotNull(testInstance);
		Assert.AreEqual("test value", testInstance.Value);
	}

	[TestMethod]
	public void DeserializeGeneric_ShouldUseFromString()
	{
		var options = GetOptions();
		string json = "\"test value\"";
		var testInstance = JsonSerializer.Deserialize<TestClass<int>>(json, options);
		Assert.IsNotNull(testInstance);
		Assert.AreEqual("test value", testInstance.Value);
	}

	[TestMethod]
	public void Deserialize_ShouldThrowJsonExceptionForInvalidToken()
	{
		var options = GetOptions();
		string json = "123";
		Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<TestClass>(json, options));
	}
}
