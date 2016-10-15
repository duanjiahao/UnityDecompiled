using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SimpleJson
{
	[GeneratedCode("simple-json", "1.0.0")]
	internal interface IJsonSerializerStrategy
	{
		[SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Need to support .NET 2")]
		bool TrySerializeNonPrimitiveObject(object input, out object output);

		object DeserializeObject(object value, Type type);
	}
}
