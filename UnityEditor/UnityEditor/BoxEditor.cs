using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class BoxEditor
	{
		private const float kViewAngleThreshold = 0.05235988f;

		private int m_ControlIdHint;

		private int m_HandleControlID;

		private bool m_UseLossyScale;

		private bool m_AlwaysDisplayHandles;

		private bool m_DisableZaxis;

		private bool m_AllowNegativeSize = true;

		public Handles.DrawCapFunction drawMethodForHandles;

		public Func<Vector3, float> getHandleSizeMethod;

		public bool allowNegativeSize
		{
			get
			{
				return this.m_AllowNegativeSize;
			}
			set
			{
				this.m_AllowNegativeSize = value;
			}
		}

		public float backfaceAlphaMultiplier
		{
			get;
			set;
		}

		public BoxEditor(bool useLossyScale, int controlIdHint)
		{
			this.m_UseLossyScale = useLossyScale;
			this.m_ControlIdHint = controlIdHint;
			this.backfaceAlphaMultiplier = Handles.backfaceAlphaMultiplier;
		}

		public BoxEditor(bool useLossyScale, int controlIdHint, bool disableZaxis)
		{
			this.m_UseLossyScale = useLossyScale;
			this.m_ControlIdHint = controlIdHint;
			this.m_DisableZaxis = disableZaxis;
			this.backfaceAlphaMultiplier = Handles.backfaceAlphaMultiplier;
		}

		public void OnEnable()
		{
			this.m_HandleControlID = -1;
		}

		public void OnDisable()
		{
		}

		public void SetAlwaysDisplayHandles(bool enable)
		{
			this.m_AlwaysDisplayHandles = enable;
		}

		public bool OnSceneGUI(Transform transform, Color color, ref Vector3 center, ref Vector3 size)
		{
			return this.OnSceneGUI(transform, color, true, ref center, ref size);
		}

		public bool OnSceneGUI(Transform transform, Color color, bool handlesOnly, ref Vector3 center, ref Vector3 size)
		{
			if (this.m_UseLossyScale)
			{
				Matrix4x4 transform2 = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
				size.Scale(transform.lossyScale);
				center = transform.TransformPoint(center);
				center = transform2.inverse.MultiplyPoint(center);
				bool result = this.OnSceneGUI(transform2, color, handlesOnly, ref center, ref size);
				center = transform2.MultiplyPoint(center);
				center = transform.InverseTransformPoint(center);
				size.Scale(new Vector3(1f / transform.lossyScale.x, 1f / transform.lossyScale.y, 1f / transform.lossyScale.z));
				return result;
			}
			return this.OnSceneGUI(transform.localToWorldMatrix, color, handlesOnly, ref center, ref size);
		}

		public bool OnSceneGUI(Matrix4x4 transform, Color color, bool handlesOnly, ref Vector3 center, ref Vector3 size)
		{
			return this.OnSceneGUI(transform, color, color, handlesOnly, ref center, ref size);
		}

		public bool OnSceneGUI(Matrix4x4 transform, Color boxColor, Color midPointHandleColor, bool handlesOnly, ref Vector3 center, ref Vector3 size)
		{
			bool flag = GUIUtility.hotControl == this.m_HandleControlID;
			if (!this.m_AlwaysDisplayHandles && !flag)
			{
				for (int i = 0; i < 6; i++)
				{
					GUIUtility.GetControlID(this.m_ControlIdHint, FocusType.Keyboard);
				}
				return false;
			}
			if (Tools.viewToolActive)
			{
				return false;
			}
			Color color = Handles.color;
			Handles.color = boxColor;
			Vector3 b = center - size * 0.5f;
			Vector3 a = center + size * 0.5f;
			Matrix4x4 matrix = Handles.matrix;
			Handles.matrix = transform;
			int hotControl = GUIUtility.hotControl;
			if (!handlesOnly)
			{
				Handles.DrawWireCube(center, size);
			}
			Vector3 point = transform.inverse.MultiplyPoint(Camera.current.transform.position);
			Bounds bounds = new Bounds(center, size);
			bool isCameraInsideBox = bounds.Contains(point);
			Handles.color = midPointHandleColor;
			this.MidpointHandles(ref b, ref a, Handles.matrix, isCameraInsideBox);
			if (hotControl != GUIUtility.hotControl && GUIUtility.hotControl != 0)
			{
				this.m_HandleControlID = GUIUtility.hotControl;
			}
			bool changed = GUI.changed;
			if (changed)
			{
				center = (a + b) * 0.5f;
				size = a - b;
			}
			Handles.color = color;
			Handles.matrix = matrix;
			return changed;
		}

		private void MidpointHandles(ref Vector3 minPos, ref Vector3 maxPos, Matrix4x4 transform, bool isCameraInsideBox)
		{
			Vector3 vector = new Vector3(1f, 0f, 0f);
			Vector3 localTangent = new Vector3(0f, 1f, 0f);
			Vector3 vector2 = new Vector3(0f, 0f, 1f);
			Vector3 vector3 = (maxPos + minPos) * 0.5f;
			Vector3 localPos = new Vector3(maxPos.x, vector3.y, vector3.z);
			Vector3 vector4 = this.MidpointHandle(localPos, localTangent, vector2, transform, isCameraInsideBox);
			maxPos.x = ((!this.m_AllowNegativeSize) ? Math.Max(vector4.x, minPos.x) : vector4.x);
			localPos = new Vector3(minPos.x, vector3.y, vector3.z);
			vector4 = this.MidpointHandle(localPos, localTangent, -vector2, transform, isCameraInsideBox);
			minPos.x = ((!this.m_AllowNegativeSize) ? Math.Min(vector4.x, maxPos.x) : vector4.x);
			localPos = new Vector3(vector3.x, maxPos.y, vector3.z);
			vector4 = this.MidpointHandle(localPos, vector, -vector2, transform, isCameraInsideBox);
			maxPos.y = ((!this.m_AllowNegativeSize) ? Math.Max(vector4.y, minPos.y) : vector4.y);
			localPos = new Vector3(vector3.x, minPos.y, vector3.z);
			vector4 = this.MidpointHandle(localPos, vector, vector2, transform, isCameraInsideBox);
			minPos.y = ((!this.m_AllowNegativeSize) ? Math.Min(vector4.y, maxPos.y) : vector4.y);
			if (!this.m_DisableZaxis)
			{
				localPos = new Vector3(vector3.x, vector3.y, maxPos.z);
				vector4 = this.MidpointHandle(localPos, localTangent, -vector, transform, isCameraInsideBox);
				maxPos.z = ((!this.m_AllowNegativeSize) ? Math.Max(vector4.z, minPos.z) : vector4.z);
				localPos = new Vector3(vector3.x, vector3.y, minPos.z);
				vector4 = this.MidpointHandle(localPos, localTangent, vector, transform, isCameraInsideBox);
				minPos.z = ((!this.m_AllowNegativeSize) ? Math.Min(vector4.z, maxPos.z) : vector4.z);
			}
		}

		private static void DefaultMidPointDrawFunc(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			Handles.DotCap(controlID, position, rotation, size);
		}

		private static float DefaultMidpointGetSizeFunc(Vector3 localPos)
		{
			return HandleUtility.GetHandleSize(localPos) * 0.03f;
		}

		private Vector3 MidpointHandle(Vector3 localPos, Vector3 localTangent, Vector3 localBinormal, Matrix4x4 transform, bool isCameraInsideBox)
		{
			Color color = Handles.color;
			float num = 1f;
			this.AdjustMidpointHandleColor(localPos, localTangent, localBinormal, transform, num, isCameraInsideBox);
			int controlID = GUIUtility.GetControlID(this.m_ControlIdHint, FocusType.Keyboard);
			if (num > 0f)
			{
				Vector3 normalized = Vector3.Cross(localTangent, localBinormal).normalized;
				Handles.DrawCapFunction drawFunc = this.drawMethodForHandles ?? new Handles.DrawCapFunction(BoxEditor.DefaultMidPointDrawFunc);
				Func<Vector3, float> func = this.getHandleSizeMethod ?? new Func<Vector3, float>(BoxEditor.DefaultMidpointGetSizeFunc);
				localPos = Slider1D.Do(controlID, localPos, normalized, func(localPos), drawFunc, SnapSettings.scale);
			}
			Handles.color = color;
			return localPos;
		}

		private void AdjustMidpointHandleColor(Vector3 localPos, Vector3 localTangent, Vector3 localBinormal, Matrix4x4 transform, float alphaFactor, bool isCameraInsideBox)
		{
			if (!isCameraInsideBox)
			{
				Vector3 b = transform.MultiplyPoint(localPos);
				Vector3 lhs = transform.MultiplyVector(localTangent);
				Vector3 rhs = transform.MultiplyVector(localBinormal);
				Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
				float num;
				if (Camera.current.orthographic)
				{
					num = Vector3.Dot(-Camera.current.transform.forward, normalized);
				}
				else
				{
					num = Vector3.Dot((Camera.current.transform.position - b).normalized, normalized);
				}
				if (num < -0.0001f)
				{
					alphaFactor *= this.backfaceAlphaMultiplier;
				}
			}
			if (alphaFactor < 1f)
			{
				Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, Handles.color.a * alphaFactor);
			}
		}

		private void EdgeHandles(ref Vector3 minPos, ref Vector3 maxPos, Matrix4x4 transform)
		{
			Vector3 vector = new Vector3(1f, 0f, 0f);
			Vector3 vector2 = new Vector3(0f, 1f, 0f);
			Vector3 vector3 = new Vector3(0f, 0f, 1f);
			float z = (minPos.z + maxPos.z) * 0.5f;
			Vector3 handlePos = new Vector3(minPos.x, minPos.y, z);
			Vector3 vector4 = this.EdgeHandle(handlePos, vector, -vector, -vector2, transform);
			minPos.x = vector4.x;
			minPos.y = vector4.y;
			handlePos = new Vector3(minPos.x, maxPos.y, z);
			vector4 = this.EdgeHandle(handlePos, vector, -vector, vector2, transform);
			minPos.x = vector4.x;
			maxPos.y = vector4.y;
			handlePos = new Vector3(maxPos.x, maxPos.y, z);
			vector4 = this.EdgeHandle(handlePos, vector, vector, vector2, transform);
			maxPos.x = vector4.x;
			maxPos.y = vector4.y;
			handlePos = new Vector3(maxPos.x, minPos.y, z);
			vector4 = this.EdgeHandle(handlePos, vector, vector, -vector2, transform);
			maxPos.x = vector4.x;
			minPos.y = vector4.y;
			float y = (minPos.y + maxPos.y) * 0.5f;
			Vector3 handlePos2 = new Vector3(minPos.x, y, minPos.z);
			Vector3 vector5 = this.EdgeHandle(handlePos2, vector2, -vector, -vector3, transform);
			minPos.x = vector5.x;
			minPos.z = vector5.z;
			handlePos2 = new Vector3(minPos.x, y, maxPos.z);
			vector5 = this.EdgeHandle(handlePos2, vector2, -vector, vector3, transform);
			minPos.x = vector5.x;
			maxPos.z = vector5.z;
			handlePos2 = new Vector3(maxPos.x, y, maxPos.z);
			vector5 = this.EdgeHandle(handlePos2, vector2, vector, vector3, transform);
			maxPos.x = vector5.x;
			maxPos.z = vector5.z;
			handlePos2 = new Vector3(maxPos.x, y, minPos.z);
			vector5 = this.EdgeHandle(handlePos2, vector2, vector, -vector3, transform);
			maxPos.x = vector5.x;
			minPos.z = vector5.z;
			float x = (minPos.x + maxPos.x) * 0.5f;
			Vector3 handlePos3 = new Vector3(x, minPos.y, minPos.z);
			Vector3 vector6 = this.EdgeHandle(handlePos3, vector2, -vector2, -vector3, transform);
			minPos.y = vector6.y;
			minPos.z = vector6.z;
			handlePos3 = new Vector3(x, minPos.y, maxPos.z);
			vector6 = this.EdgeHandle(handlePos3, vector2, -vector2, vector3, transform);
			minPos.y = vector6.y;
			maxPos.z = vector6.z;
			handlePos3 = new Vector3(x, maxPos.y, maxPos.z);
			vector6 = this.EdgeHandle(handlePos3, vector2, vector2, vector3, transform);
			maxPos.y = vector6.y;
			maxPos.z = vector6.z;
			handlePos3 = new Vector3(x, maxPos.y, minPos.z);
			vector6 = this.EdgeHandle(handlePos3, vector2, vector2, -vector3, transform);
			maxPos.y = vector6.y;
			minPos.z = vector6.z;
		}

		private Vector3 EdgeHandle(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, Matrix4x4 transform)
		{
			Color color = Handles.color;
			bool flag = true;
			if (Camera.current)
			{
				Vector3 b = Handles.matrix.inverse.MultiplyPoint(Camera.current.transform.position);
				Vector3 normalized = (handlePos - b).normalized;
				Vector3 lhs = Vector3.Cross(slideDir1, slideDir2);
				float f = Vector3.Dot(lhs, normalized);
				if (Mathf.Acos(Mathf.Abs(f)) > 1.51843643f)
				{
					flag = false;
				}
			}
			float num = (!flag) ? 0f : 1f;
			this.AdjustEdgeHandleColor(handlePos, slideDir1, slideDir2, transform, num);
			int controlID = GUIUtility.GetControlID(this.m_ControlIdHint, FocusType.Keyboard);
			if (num > 0f)
			{
				handlePos = Slider2D.Do(controlID, handlePos, handleDir, slideDir1, slideDir2, HandleUtility.GetHandleSize(handlePos) * 0.03f, new Handles.DrawCapFunction(Handles.DotCap), SnapSettings.scale, true);
			}
			Handles.color = color;
			return handlePos;
		}

		private void AdjustEdgeHandleColor(Vector3 handlePos, Vector3 slideDir1, Vector3 slideDir2, Matrix4x4 transform, float alphaFactor)
		{
			Vector3 inPoint = transform.MultiplyPoint(handlePos);
			Vector3 normalized = transform.MultiplyVector(slideDir1).normalized;
			Vector3 normalized2 = transform.MultiplyVector(slideDir2).normalized;
			bool flag;
			if (Camera.current.orthographic)
			{
				flag = (Vector3.Dot(-Camera.current.transform.forward, normalized) < 0f && Vector3.Dot(-Camera.current.transform.forward, normalized2) < 0f);
			}
			else
			{
				Plane plane = new Plane(normalized, inPoint);
				Plane plane2 = new Plane(normalized2, inPoint);
				flag = (!plane.GetSide(Camera.current.transform.position) && !plane2.GetSide(Camera.current.transform.position));
			}
			if (flag)
			{
				alphaFactor *= this.backfaceAlphaMultiplier;
			}
			if (alphaFactor < 1f)
			{
				Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, Handles.color.a * alphaFactor);
			}
		}
	}
}
