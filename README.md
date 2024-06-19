# ktsu.io.ToStringJsonConverter

## Overview
`ToStringJsonConverter` is a JSON converter factory that simplifies serialization and deserialization of custom types in .NET by leveraging the `ToString` and `FromString` methods.

## Features
- Automatically identifies types with a `FromString` method for custom serialization.
- Converts objects to and from JSON using their string representation.
- Supports custom types with static `FromString` methods.

## Installation
Install via NuGet:
```sh
dotnet add package ktsu.io.ToStringJsonConverter
```

## Usage
Configure the converter in your `JsonSerializerOptions`:
```csharp
var options = new JsonSerializerOptions();
options.Converters.Add(new ToStringJsonConverterFactory());
```

## Example
```csharp
public class MyType
{
    public string Value { get; set; }
    public static MyType FromString(string value) => new() { Value = value };
    public override string ToString() => Value;
}

var myType = new MyType { Value = "example" };
string json = JsonSerializer.Serialize(myType, options); // "example"
MyType deserialized = JsonSerializer.Deserialize<MyType>(json, options);
```

## License
This project is licensed under the MIT License. See the LICENSE file for more details.
