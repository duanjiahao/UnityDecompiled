using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[Serializable]
	internal class SerializableDelayedCallback : ScriptableObject
	{
		[SerializeField]
		private long m_CallbackTicks;

		[SerializeField]
		private UnityEvent m_Callback;

		protected SerializableDelayedCallback()
		{
			this.m_Callback = new UnityEvent();
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
		}

		public static SerializableDelayedCallback SubscribeCallback(UnityAction action, TimeSpan delayUntilCallback)
		{
			SerializableDelayedCallback serializableDelayedCallback = ScriptableObject.CreateInstance<SerializableDelayedCallback>();
			serializableDelayedCallback.m_CallbackTicks = DateTime.UtcNow.Add(delayUntilCallback).Ticks;
			serializableDelayedCallback.m_Callback.AddPersistentListener(action, UnityEventCallState.EditorAndRuntime);
			return serializableDelayedCallback;
		}

		public void Cancel()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
		}

		private void Update()
		{
			long ticks = DateTime.UtcNow.Ticks;
			if (ticks >= this.m_CallbackTicks)
			{
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
				this.m_Callback.Invoke();
			}
		}
	}
}
