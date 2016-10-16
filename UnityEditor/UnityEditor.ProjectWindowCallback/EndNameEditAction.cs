using System;
using UnityEngine;

namespace UnityEditor.ProjectWindowCallback
{
	public abstract class EndNameEditAction : ScriptableObject
	{
		public virtual void OnEnable()
		{
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		public abstract void Action(int instanceId, string pathName, string resourceFile);

		public virtual void CleanUp()
		{
			UnityEngine.Object.DestroyImmediate(this);
		}
	}
}
