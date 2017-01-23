using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(OcclusionArea))]
	internal class OcclusionAreaEditor : Editor
	{
		private SerializedObject m_Object;

		private SerializedProperty m_Size;

		private SerializedProperty m_Center;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache0;

		private void OnEnable()
		{
			this.m_Object = new SerializedObject(base.target);
			this.m_Size = base.serializedObject.FindProperty("m_Size");
			this.m_Center = base.serializedObject.FindProperty("m_Center");
		}

		private void OnDisable()
		{
			this.m_Object.Dispose();
			this.m_Object = null;
		}

		private void OnSceneGUI()
		{
			this.m_Object.Update();
			OcclusionArea occlusionArea = (OcclusionArea)base.target;
			Color color = Handles.color;
			Handles.color = new Color(145f, 244f, 139f, 255f) / 255f;
			Vector3 p = occlusionArea.transform.TransformPoint(this.m_Center.vector3Value);
			Vector3 vector = this.m_Size.vector3Value * 0.5f;
			Vector3 a = this.m_Size.vector3Value * 0.5f;
			Vector3 lossyScale = occlusionArea.transform.lossyScale;
			Vector3 b = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
			vector = Vector3.Scale(vector, lossyScale);
			a = Vector3.Scale(a, lossyScale);
			bool changed = GUI.changed;
			vector.x = this.SizeSlider(p, -Vector3.right, vector.x);
			vector.y = this.SizeSlider(p, -Vector3.up, vector.y);
			vector.z = this.SizeSlider(p, -Vector3.forward, vector.z);
			a.x = this.SizeSlider(p, Vector3.right, a.x);
			a.y = this.SizeSlider(p, Vector3.up, a.y);
			a.z = this.SizeSlider(p, Vector3.forward, a.z);
			if (GUI.changed)
			{
				this.m_Center.vector3Value = this.m_Center.vector3Value + Vector3.Scale(Quaternion.Inverse(occlusionArea.transform.rotation) * (a - vector) * 0.5f, b);
				vector = Vector3.Scale(vector, b);
				a = Vector3.Scale(a, b);
				this.m_Size.vector3Value = a + vector;
				base.serializedObject.ApplyModifiedProperties();
			}
			GUI.changed |= changed;
			Handles.color = color;
		}

		private float SizeSlider(Vector3 p, Vector3 d, float r)
		{
			Vector3 vector = p + d * r;
			Color color = Handles.color;
			if (Vector3.Dot(vector - Camera.current.transform.position, d) >= 0f)
			{
				Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, Handles.color.a * Handles.backfaceAlphaMultiplier);
			}
			float handleSize = HandleUtility.GetHandleSize(vector);
			bool changed = GUI.changed;
			GUI.changed = false;
			Vector3 arg_C1_0 = vector;
			float arg_C1_2 = handleSize * 0.1f;
			if (OcclusionAreaEditor.<>f__mg$cache0 == null)
			{
				OcclusionAreaEditor.<>f__mg$cache0 = new Handles.CapFunction(Handles.CylinderHandleCap);
			}
			vector = Handles.Slider(arg_C1_0, d, arg_C1_2, OcclusionAreaEditor.<>f__mg$cache0, 0f);
			if (GUI.changed)
			{
				r = Vector3.Dot(vector - p, d);
			}
			GUI.changed |= changed;
			Handles.color = color;
			return r;
		}
	}
}
