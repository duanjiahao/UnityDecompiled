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

		public T Resolve(IExposedPropertyTable resolver)
		{
			T result;
			if (resolver != null)
			{
				bool flag;
				Object referenceValue = resolver.GetReferenceValue(this.exposedName, out flag);
				if (flag)
				{
					result = (referenceValue as T);
					return result;
				}
			}
			result = (this.defaultValue as T);
			return result;
		}
	}
}
