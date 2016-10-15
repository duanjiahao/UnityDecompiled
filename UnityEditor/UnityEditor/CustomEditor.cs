using System;
using UnityEngine;

namespace UnityEditor
{
	public sealed class CustomEditor : Attribute
	{
		internal Type m_InspectedType;

		internal bool m_EditorForChildClasses;

		public bool isFallback
		{
			get;
			set;
		}

		public CustomEditor(Type inspectedType)
		{
			if (inspectedType == null)
			{
				Debug.LogError("Failed to load CustomEditor inspected type");
			}
			this.m_InspectedType = inspectedType;
			this.m_EditorForChildClasses = false;
		}

		public CustomEditor(Type inspectedType, bool editorForChildClasses)
		{
			if (inspectedType == null)
			{
				Debug.LogError("Failed to load CustomEditor inspected type");
			}
			this.m_InspectedType = inspectedType;
			this.m_EditorForChildClasses = editorForChildClasses;
		}
	}
}
