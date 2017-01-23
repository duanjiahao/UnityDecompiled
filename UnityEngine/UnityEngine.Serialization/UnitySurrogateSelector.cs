using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnityEngine.Serialization
{
	public class UnitySurrogateSelector : ISurrogateSelector
	{
		public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
		{
			ISerializationSurrogate result;
			if (type.IsGenericType)
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(List<>))
				{
					selector = this;
					result = ListSerializationSurrogate.Default;
					return result;
				}
				if (genericTypeDefinition == typeof(Dictionary<, >))
				{
					selector = this;
					Type type2 = typeof(DictionarySerializationSurrogate<, >).MakeGenericType(type.GetGenericArguments());
					result = (ISerializationSurrogate)Activator.CreateInstance(type2);
					return result;
				}
			}
			selector = null;
			result = null;
			return result;
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
