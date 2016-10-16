using System;
using UnityEngine;

namespace UnityEditor
{
	internal class CallbackController
	{
		private readonly Action m_Callback;

		private readonly float m_CallbacksPerSecond;

		private double m_NextCallback;

		public bool active
		{
			get;
			private set;
		}

		public CallbackController(Action callback, float callbacksPerSecond)
		{
			this.m_Callback = callback;
			this.m_CallbacksPerSecond = Mathf.Max(callbacksPerSecond, 1f);
		}

		public void Start()
		{
			this.m_NextCallback = 0.0;
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			this.active = true;
		}

		public void Stop()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			this.active = false;
		}

		private void Update()
		{
			double timeSinceStartup = EditorApplication.timeSinceStartup;
			if (timeSinceStartup > this.m_NextCallback)
			{
				this.m_NextCallback = timeSinceStartup + (double)(1f / this.m_CallbacksPerSecond);
				if (this.m_Callback != null)
				{
					this.m_Callback();
				}
			}
		}
	}
}
