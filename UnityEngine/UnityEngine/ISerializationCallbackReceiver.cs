using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public interface ISerializationCallbackReceiver
	{
		void OnBeforeSerialize();

		void OnAfterDeserialize();
	}
}
