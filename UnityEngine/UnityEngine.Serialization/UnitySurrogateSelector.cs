using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnityEngine.Serialization
{
	public class UnitySurrogateSelector : ISurrogateSelector
	{
		public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
		{
			if (type.IsGenericType)
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(List<>))
				{
					selector = this;
					return ListSerializationSurrogate.Default;
				}
				if (genericTypeDefinition == typeof(Dictionary<, >))
				{
					selector = this;
					Type type2 = typeof(DictionarySerializationSurrogate<, >).MakeGenericType(type.GetGenericArguments());
					return (ISerializationSurrogate)Activator.CreateInstance(type2);
				}
			}
			selector = null;
			return null;
		}

		public void ChainSelector(ISurrogateSelector selector)
		{
			throw new NotImplementedException();
		}

		public ISurrogateSelector GetNextSelector()
		{
			throw new NotImplementedException();
		}
	}
}
