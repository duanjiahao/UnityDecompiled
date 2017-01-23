using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(OcclusionPortal))]
	internal class OcclusionPortalInspector : Editor
	{
		private static readonly int s_BoxHash = "BoxColliderEditor".GetHashCode();

		private readonly BoxEditor m_BoxEditor = new BoxEditor(true, OcclusionPortalInspector.s_BoxHash);

		private SerializedProperty m_Center;

		private SerializedProperty m_Size;

		private SerializedObject m_Object;

		public void OnEnable()
		{
			this.m_Object = new SerializedObject(base.targets);
			this.m_Center = this.m_Object.FindProperty("m_Center");
			this.m_Size = this.m_Object.FindProperty("m_Size");
			this.m_BoxEditor.OnEnable();
			this.m_BoxEditor.SetAlwaysDisplayHandles(true);
		}

		private void OnSceneGUI()
		{
			OcclusionPortal occlusionPortal = base.target as OcclusionPortal;
			Vector3 vector3Value = this.m_Center.vector3Value;
			Vector3 vector3Value2 = this.m_Size.vector3Value;
			Color s_ColliderHandleColor = Handles.s_ColliderHandleColor;
			if (this.m_BoxEditor.OnSceneGUI(occlusionPortal.transform, s_ColliderHandleColor, ref vector3Value, ref vector3Value2))
			{
				this.m_Center.vector3Value = vector3Value;
				this.m_Size.vector3Value = vector3Value2;
				this.m_Object.ApplyModifiedProperties();
			}
		}
	}
}
