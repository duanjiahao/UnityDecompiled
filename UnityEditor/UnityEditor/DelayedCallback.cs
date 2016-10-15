using System;

namespace UnityEditor
{
	internal class DelayedCallback
	{
		private Action m_Callback;

		private double m_CallbackTime;

		public DelayedCallback(Action function, double timeFromNow)
		{
			this.m_Callback = function;
			this.m_CallbackTime = EditorApplication.timeSinceStartup + timeFromNow;
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
		}

		public void Clear()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			this.m_CallbackTime = 0.0;
			this.m_Callback = null;
		}

		private void Update()
		{
			if (EditorApplication.timeSinceStartup > this.m_CallbackTime)
			{
				Action callback = this.m_Callback;
				this.Clear();
				callback();
			}
		}
	}
}
