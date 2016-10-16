using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnityEngine.Serialization
{
	internal class DictionarySerializationSurrogate<TKey, TValue> : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			Dictionary<TKey, TValue> dictionary = (Dictionary<TKey, TValue>)obj;
			dictionary.GetObjectData(info, context);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			IEqualityComparer<TKey> comparer = (IEqualityComparer<TKey>)info.GetValue("Comparer", typeof(IEqualityComparer<TKey>));
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(comparer);
			if (info.MemberCount > 3)
			{
				KeyValuePair<TKey, TValue>[] array = (KeyValuePair<TKey, TValue>[])info.GetValue("KeyValuePairs", typeof(KeyValuePair<TKey, TValue>[]));
				if (array != null)
				{
					KeyValuePair<TKey, TValue>[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						KeyValuePair<TKey, TValue> keyValuePair = array2[i];
						dictionary.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			return dictionary;
		}
	}
}
