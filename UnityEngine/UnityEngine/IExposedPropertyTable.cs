using System;

namespace UnityEngine
{
	internal interface IExposedPropertyTable
	{
		void SetReferenceValue(PropertyName id, Object value);

		Object GetReferenceValue(PropertyName id, out bool idValid);

		void ClearReferenceValue(PropertyName id);
	}
}
