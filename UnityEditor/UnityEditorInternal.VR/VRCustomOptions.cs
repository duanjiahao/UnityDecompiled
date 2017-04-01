using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	internal abstract class VRCustomOptions
	{
		private SerializedProperty editorSettings;

		private SerializedProperty playerSettings;

		public bool IsExpanded
		{
			get;
			set;
		}

		internal SerializedProperty FindPropertyAssert(string name)
		{
			SerializedProperty serializedProperty = null;
			if (this.editorSettings == null && this.playerSettings == null)
			{
				Debug.LogError("No existing VR settings. Failed to find:" + name);
			}
			else
			{
				bool flag = false;
				if (this.editorSettings != null)
				{
					serializedProperty = this.editorSettings.FindPropertyRelative(name);
					if (serializedProperty != null)
					{
						flag = true;
					}
				}
				if (!flag && this.playerSettings != null)
				{
					serializedProperty = this.playerSettings.FindPropertyRelative(name);
					if (serializedProperty != null)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					Debug.LogError("Failed to find property:" + name);
				}
			}
			return serializedProperty;
		}

		public virtual void Initialize(SerializedObject settings)
		{
			this.Initialize(settings, "");
		}

		public virtual void Initialize(SerializedObject settings, string propertyName)
		{
			this.editorSettings = settings.FindProperty("vrEditorSettings");
			if (this.editorSettings != null && !string.IsNullOrEmpty(propertyName))
			{
				this.editorSettings = this.editorSettings.FindPropertyRelative(propertyName);
			}
			this.playerSettings = settings.FindProperty("vrSettings");
			if (this.playerSettings != null && !string.IsNullOrEmpty(propertyName))
			{
				this.playerSettings = this.playerSettings.FindPropertyRelative(propertyName);
			}
		}

		public abstract Rect Draw(Rect rect);

		public abstract float GetHeight();
	}
}
