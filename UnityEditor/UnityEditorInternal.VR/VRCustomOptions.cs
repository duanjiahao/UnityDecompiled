using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	internal abstract class VRCustomOptions
	{
		private SerializedProperty settings;

		public bool IsExpanded
		{
			get;
			set;
		}

		internal SerializedProperty FindPropertyAssert(string name)
		{
			SerializedProperty serializedProperty = null;
			if (this.settings == null)
			{
				Debug.LogError("No existing VR settings. Failed to find:" + name);
			}
			else
			{
				serializedProperty = this.settings.FindPropertyRelative(name);
				if (serializedProperty == null)
				{
					Debug.LogError("Failed to find:" + name);
				}
			}
			return serializedProperty;
		}

		public virtual void Initialize(SerializedProperty vrEditorSettings)
		{
			this.settings = vrEditorSettings;
		}

		public abstract void Draw(Rect rect);

		public abstract float GetHeight();
	}
}
