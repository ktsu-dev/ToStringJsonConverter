# ktsu.ToStringJsonConverter

> A JSON converter factory that serializes objects using their ToString and FromString methods.

[![License](https://img.shields.io/github/license/ktsu-dev/ToStringJsonConverter)](https://github.com/ktsu-dev/ToStringJsonConverter/blob/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/ktsu.ToStringJsonConverter.svg)](https://www.nuget.org/packages/ktsu.ToStringJsonConverter/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.ToStringJsonConverter.svg)](https://www.nuget.org/packages/ktsu.ToStringJsonConverter/)
[![Build Status](https://github.com/ktsu-dev/ToStringJsonConverter/workflows/build/badge.svg)](https://github.com/ktsu-dev/ToStringJsonConverter/actions)
[![GitHub Stars](https://img.shields.io/github/stars/ktsu-dev/ToStringJsonConverter?style=social)](https://github.com/ktsu-dev/ToStringJsonConverter/stargazers)

## Introduction

`ToStringJsonConverter` is a JSON converter factory for System.Text.Json that simplifies serialization and deserialization of custom types by leveraging their `ToString` and `FromString` methods. This approach is particularly useful for value types, strong types, and any other types where a string representation makes logical sense.

## Features

- **Automatic Type Detection**: Automatically identifies types with compatible `FromString` methods.
- **String-Based Serialization**: Converts objects to and from JSON using their string representation.
- **Property Name Support**: Works with both JSON values and property names.
- **Reflection Optimization**: Uses cached reflection for improved performance.
- **Generic Method Support**: Handles both generic and non-generic `FromString` methods.

## Installation

### Package Manager Console

```powershell
Install-Package ktsu.ToStringJsonConverter
```

### .NET CLI

```bash
dotnet add package ktsu.ToStringJsonConverter
```

### Package Reference

```xml
<PackageReference Include="ktsu.ToStringJsonConverter" Version="x.y.z" />
```

## Usage Examples

### Basic Example

```csharp
using System.Text.Json;
using ktsu.ToStringJsonConverter;

// Configure the converter in your JsonSerializerOptions
var options = new JsonSerializerOptions();
options.Converters.Add(new ToStringJsonConverterFactory());

// Example custom type with ToString and FromString
public class CustomId
{
    public string Value { get; set; }
    
    public static CustomId FromString(string value) => new() { Value = value };
    
    public override string ToString() => Value;
}

// Serialization
var id = new CustomId { Value = "12345" };
string json = JsonSerializer.Serialize(id, options);
// json is now: "12345"

// Deserialization
CustomId deserialized = JsonSerializer.Deserialize<CustomId>(json, options);
// deserialized.Value is now: "12345"
```

### Integration with Other Converters

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using ktsu.ToStringJsonConverter;

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters =
    {
        new ToStringJsonConverterFactory(),
        new JsonStringEnumConverter()
    }
};

// Now both enum values and custom types with FromString will be handled appropriately
```

## Advanced Usage

### Working with Collections of Custom Types

```csharp
using System.Text.Json;
using ktsu.ToStringJsonConverter;

// Setup serializer options with the converter
var options = new JsonSerializerOptions();
options.Converters.Add(new ToStringJsonConverterFactory());

// A collection of custom types
List<CustomId> ids = new()
{
    new CustomId { Value = "A001" },
    new CustomId { Value = "B002" },
    new CustomId { Value = "C003" }
};

// Serialize the collection
string json = JsonSerializer.Serialize(ids, options);
// json is now: ["A001","B002","C003"]

// Deserialize back to a collection
List<CustomId> deserializedIds = JsonSerializer.Deserialize<List<CustomId>>(json, options);
```

### Using with Dictionaries as Keys

```csharp
// Custom type can be used as dictionary keys
var dictionary = new Dictionary<CustomId, string>
{
    { new CustomId { Value = "key1" }, "value1" },
    { new CustomId { Value = "key2" }, "value2" }
};

string json = JsonSerializer.Serialize(dictionary, options);
// Serializes as a dictionary with string keys

var deserialized = JsonSerializer.Deserialize<Dictionary<CustomId, string>>(json, options);
// Keys are properly deserialized back to CustomId objects
```

## API Reference

### ToStringJsonConverterFactory

The primary class for integrating with System.Text.Json serialization.

#### Methods

| Name | Return Type | Description |
|------|-------------|-------------|
| `CanConvert(Type typeToConvert)` | `bool` | Determines if a type can be converted by checking for a static FromString method |
| `CreateConverter(Type typeToConvert, JsonSerializerOptions options)` | `JsonConverter` | Creates a type-specific converter instance |

### Compatibility Requirements

For a type to work with ToStringJsonConverter, it must meet these requirements:

1. Have a public static `FromString(string)` method that returns an instance of the type
2. Override `ToString()` to provide a string representation that can be reversed by `FromString`

## Contributing

Contributions are welcome! Here's how you can help:

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please make sure to update tests as appropriate.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
