using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	public class ArcHandle
	{
		private static readonly float s_DefaultAngleHandleSize = 0.08f;

		private static readonly float s_DefaultAngleHandleSizeRatio = 1.25f;

		private static readonly float s_DefaultRadiusHandleSize = 0.03f;

		private bool m_ControlIDsReserved = false;

		private int m_AngleHandleControlID;

		private int[] m_RadiusHandleControlIDs = new int[4];

		private Quaternion m_MostRecentValidAngleHandleOrientation = Quaternion.identity;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache0;

		public float angle
		{
			get;
			set;
		}

		public float radius
		{
			get;
			set;
		}

		public Color angleHandleColor
		{
			get;
			set;
		}

		public Color radiusHandleColor
		{
			get;
			set;
		}

		public Color fillColor
		{
			get;
			set;
		}

		public Color wireframeColor
		{
			get;
			set;
		}

		public Handles.CapFunction angleHandleDrawFunction
		{
			get;
			set;
		}

		public Handles.SizeFunction angleHandleSizeFunction
		{
			get;
			set;
		}

		public Handles.CapFunction radiusHandleDrawFunction
		{
			get;
			set;
		}

		public Handles.SizeFunction radiusHandleSizeFunction
		{
			get;
			set;
		}

		public ArcHandle()
		{
			this.radius = 1f;
			this.SetColorWithoutRadiusHandle(Color.white, 0.1f);
		}

		private static float DefaultAngleHandleSizeFunction(Vector3 position)
		{
			return HandleUtility.GetHandleSize(position) * ArcHandle.s_DefaultAngleHandleSize;
		}

		private static float DefaultRadiusHandleSizeFunction(Vector3 position)
		{
			return HandleUtility.GetHandleSize(position) * ArcHandle.s_DefaultRadiusHandleSize;
		}

		private static void DefaultRadiusHandleDrawFunction(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			Handles.DotHandleCap(controlID, position, rotation, size, eventType);
		}

		public void SetColorWithoutRadiusHandle(Color color, float fillColorAlpha)
		{
			this.SetColorWithRadiusHandle(color, fillColorAlpha);
			this.radiusHandleColor = Color.clear;
			this.wireframeColor = color;
		}

		public void SetColorWithRadiusHandle(Color color, float fillColorAlpha)
		{
			this.fillColor = color * new Color(1f, 1f, 1f, fillColorAlpha);
			this.angleHandleColor = color;
			this.radiusHandleColor = color;
			this.wireframeColor = color;
		}

		public void DrawHandle()
		{
			if (!this.m_ControlIDsReserved)
			{
				this.GetControlIDs();
			}
			this.m_ControlIDsReserved = false;
			if (Handles.color.a != 0f)
			{
				Vector3 vector = Handles.matrix.MultiplyPoint3x4(Vector3.one) - Handles.matrix.MultiplyPoint3x4(Vector3.zero);
				if (vector.x != 0f || vector.z != 0f)
				{
					Vector3 vector2 = Quaternion.AngleAxis(this.angle, Vector3.up) * Vector3.forward * this.radius;
					float num = Mathf.Abs(this.angle);
					float num2 = this.angle % 360f;
					using (new Handles.DrawingScope(Handles.color * this.fillColor))
					{
						if (Handles.color.a > 0f)
						{
							int i = 0;
							int num3 = (int)num / 360;
							while (i < num3)
							{
								Handles.DrawSolidArc(Vector3.zero, Vector3.up, Vector3.forward, 360f, this.radius);
								i++;
							}
							Handles.DrawSolidArc(Vector3.zero, Vector3.up, Vector3.forward, num2, this.radius);
						}
					}
					using (new Handles.DrawingScope(Handles.color * this.wireframeColor))
					{
						if (Handles.color.a > 0f)
						{
							Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, (num < 360f) ? num2 : 360f, this.radius);
						}
					}
					if (Event.current.alt)
					{
						bool flag = true;
						int[] radiusHandleControlIDs = this.m_RadiusHandleControlIDs;
						for (int j = 0; j < radiusHandleControlIDs.Length; j++)
						{
							int num4 = radiusHandleControlIDs[j];
							if (num4 == GUIUtility.hotControl)
							{
								flag = false;
								break;
							}
						}
						if (flag && GUIUtility.hotControl != this.m_AngleHandleControlID)
						{
							return;
						}
					}
					using (new Handles.DrawingScope(Handles.color * this.radiusHandleColor))
					{
						if (Handles.color.a > 0f)
						{
							float num5 = Mathf.Sign(this.angle);
							int num6 = Mathf.Min(1 + (int)(Mathf.Min(360f, num) * 0.0111111114f), 4);
							for (int k = 0; k < num6; k++)
							{
								Quaternion q = Quaternion.AngleAxis((float)k * 90f * num5, Vector3.up);
								using (new Handles.DrawingScope(Handles.matrix * Matrix4x4.TRS(Vector3.zero, q, Vector3.one)))
								{
									Vector3 vector3 = Vector3.forward * this.radius;
									EditorGUI.BeginChangeCheck();
									float num7 = (this.radiusHandleSizeFunction != null) ? this.radiusHandleSizeFunction(vector3) : ArcHandle.DefaultRadiusHandleSizeFunction(vector3);
									int arg_37A_0 = this.m_RadiusHandleControlIDs[k];
									Vector3 arg_37A_1 = vector3;
									Vector3 arg_37A_2 = Vector3.forward;
									float arg_37A_3 = num7;
									Handles.CapFunction arg_37A_4;
									if ((arg_37A_4 = this.radiusHandleDrawFunction) == null)
									{
										if (ArcHandle.<>f__mg$cache0 == null)
										{
											ArcHandle.<>f__mg$cache0 = new Handles.CapFunction(ArcHandle.DefaultRadiusHandleDrawFunction);
										}
										arg_37A_4 = ArcHandle.<>f__mg$cache0;
									}
									Vector3 a = Handles.Slider(arg_37A_0, arg_37A_1, arg_37A_2, arg_37A_3, arg_37A_4, SnapSettings.move.z);
									if (EditorGUI.EndChangeCheck())
									{
										this.radius += (a - vector3).z;
									}
								}
							}
						}
					}
					using (new Handles.DrawingScope(Handles.color * this.angleHandleColor))
					{
						if (Handles.color.a > 0f)
						{
							EditorGUI.BeginChangeCheck();
							float handleSize = (this.angleHandleSizeFunction != null) ? this.angleHandleSizeFunction(vector2) : ArcHandle.DefaultAngleHandleSizeFunction(vector2);
							vector2 = Handles.Slider2D(this.m_AngleHandleControlID, vector2, Vector3.up, Vector3.forward, Vector3.right, handleSize, this.angleHandleDrawFunction ?? new Handles.CapFunction(this.DefaultAngleHandleDrawFunction), Vector2.zero);
							if (EditorGUI.EndChangeCheck())
							{
								float target = Vector3.Angle(Vector3.forward, vector2) * Mathf.Sign(Vector3.Dot(Vector3.right, vector2));
								this.angle += Mathf.DeltaAngle(this.angle, target);
								this.angle = Handles.SnapValue(this.angle, SnapSettings.rotation);
							}
						}
					}
				}
			}
		}

		internal void GetControlIDs()
		{
			this.m_AngleHandleControlID = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);
			for (int i = 0; i < this.m_RadiusHandleControlIDs.Length; i++)
			{
				this.m_RadiusHandleControlIDs[i] = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);
			}
			this.m_ControlIDsReserved = true;
		}

		private void DefaultAngleHandleDrawFunction(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			Handles.DrawLine(Vector3.zero, position);
			Vector3 vector = Handles.matrix.MultiplyPoint3x4(position);
			Vector3 upwards = vector - Handles.matrix.MultiplyPoint3x4(Vector3.zero);
			Vector3 forward = Handles.matrix.MultiplyVector(Quaternion.AngleAxis(90f, Vector3.up) * position);
			Matrix4x4 matrix = Matrix4x4.TRS(vector, this.m_MostRecentValidAngleHandleOrientation = ((forward.sqrMagnitude != 0f) ? Quaternion.LookRotation(forward, upwards) : this.m_MostRecentValidAngleHandleOrientation), Vector3.one + Vector3.forward * ArcHandle.s_DefaultAngleHandleSizeRatio);
			using (new Handles.DrawingScope(matrix))
			{
				Handles.CylinderHandleCap(controlID, Vector3.zero, Quaternion.identity, size, eventType);
			}
		}
	}
}
