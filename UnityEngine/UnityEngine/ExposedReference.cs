using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode(Name = "ExposedReference")]
	[Serializable]
	public struct ExposedReference<T> where T : Object
	{
		[SerializeField]
		public PropertyName exposedName;

		[SerializeField]
		public Object defaultValue;

		public T Resolve(ExposedPropertyResolver resolver)
		{
			bool flag;
			Object @object = ExposedPropertyResolver.ResolveReferenceInternal(resolver.table, this.exposedName, out flag);
			T result;
			if (flag)
			{
				result = (@object as T);
			}
			else
			{
				result = (this.defaultValue as T);
			}
			return result;
		}
	}
}
