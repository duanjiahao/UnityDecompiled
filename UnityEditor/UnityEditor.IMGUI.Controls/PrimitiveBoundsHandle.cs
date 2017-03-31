using System;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	public abstract class PrimitiveBoundsHandle
	{
		[Flags]
		public enum Axes
		{
			None = 0,
			X = 1,
			Y = 2,
			Z = 4,
			All = 7
		}

		protected enum HandleDirection
		{
			PositiveX,
			NegativeX,
			PositiveY,
			NegativeY,
			PositiveZ,
			NegativeZ
		}

		private static readonly float s_DefaultMidpointHandleSize = 0.03f;

		private static readonly int[] s_NextAxis = new int[]
		{
			1,
			2,
			0
		};

		private static GUIContent s_EditModeButton;

		private int m_ControlIDHint;

		private int[] m_ControlIDs = new int[6];

		private Bounds m_Bounds;

		private Bounds m_BoundsOnClick;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache0;

		[CompilerGenerated]
		private static Handles.SizeFunction <>f__mg$cache1;

		internal static GUIContent editModeButton
		{
			get
			{
				if (PrimitiveBoundsHandle.s_EditModeButton == null)
				{
					PrimitiveBoundsHandle.s_EditModeButton = new GUIContent(EditorGUIUtility.IconContent("EditCollider").image, EditorGUIUtility.TextContent("Edit bounding volume.\n\n - Hold Alt after clicking control handle to pin center in place.\n - Hold Shift after clicking control handle to scale uniformly.").text);
				}
				return PrimitiveBoundsHandle.s_EditModeButton;
			}
		}

		public Vector3 center
		{
			get
			{
				return this.m_Bounds.center;
			}
			set
			{
				this.m_Bounds.center = value;
			}
		}

		public PrimitiveBoundsHandle.Axes axes
		{
			get;
			set;
		}

		public Color handleColor
		{
			get;
			set;
		}

		public Color wireframeColor
		{
			get;
			set;
		}

		public Handles.CapFunction midpointHandleDrawFunction
		{
			get;
			set;
		}

		public Handles.SizeFunction midpointHandleSizeFunction
		{
			get;
			set;
		}

		public PrimitiveBoundsHandle(int controlIDHint)
		{
			this.m_ControlIDHint = controlIDHint;
			this.handleColor = Color.white;
			this.wireframeColor = Color.white;
			this.axes = PrimitiveBoundsHandle.Axes.All;
		}

		private static float DefaultMidpointHandleSizeFunction(Vector3 position)
		{
			return HandleUtility.GetHandleSize(position) * PrimitiveBoundsHandle.s_DefaultMidpointHandleSize;
		}

		public void SetColor(Color color)
		{
			this.handleColor = color;
			this.wireframeColor = color;
		}

		public void DrawHandle()
		{
			for (int i = 0; i < this.m_ControlIDs.Length; i++)
			{
				this.m_ControlIDs[i] = GUIUtility.GetControlID(this.m_ControlIDHint, FocusType.Keyboard);
			}
			Color color = Handles.color;
			Handles.color *= this.wireframeColor;
			if (Handles.color.a > 0f)
			{
				this.DrawWireframe();
			}
			if (!Event.current.alt)
			{
				Vector3 min = this.m_Bounds.min;
				Vector3 max = this.m_Bounds.max;
				int hotControl = GUIUtility.hotControl;
				Handles.color = color * this.handleColor;
				Vector3 point = Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
				bool isCameraInsideBox = this.m_Bounds.Contains(point);
				EditorGUI.BeginChangeCheck();
				this.MidpointHandles(ref min, ref max, isCameraInsideBox);
				bool flag = EditorGUI.EndChangeCheck();
				if (hotControl != GUIUtility.hotControl && GUIUtility.hotControl != 0)
				{
					this.m_BoundsOnClick = this.m_Bounds;
				}
				if (flag)
				{
					this.m_Bounds.center = (max + min) * 0.5f;
					this.m_Bounds.size = max - min;
					int j = 0;
					int num = this.m_ControlIDs.Length;
					while (j < num)
					{
						if (GUIUtility.hotControl == this.m_ControlIDs[j])
						{
							this.m_Bounds = this.OnHandleChanged((PrimitiveBoundsHandle.HandleDirection)j, this.m_BoundsOnClick, this.m_Bounds);
						}
						j++;
					}
					if (Event.current.shift)
					{
						int hotControl2 = GUIUtility.hotControl;
						Vector3 size = this.m_Bounds.size;
						int num2 = 0;
						if (hotControl2 == this.m_ControlIDs[2] || hotControl2 == this.m_ControlIDs[3])
						{
							num2 = 1;
						}
						if (hotControl2 == this.m_ControlIDs[4] || hotControl2 == this.m_ControlIDs[5])
						{
							num2 = 2;
						}
						float num3 = (!Mathf.Approximately(this.m_BoundsOnClick.size[num2], 0f)) ? (size[num2] / this.m_BoundsOnClick.size[num2]) : 1f;
						int num4 = PrimitiveBoundsHandle.s_NextAxis[num2];
						size[num4] = num3 * this.m_BoundsOnClick.size[num4];
						num4 = PrimitiveBoundsHandle.s_NextAxis[num4];
						size[num4] = num3 * this.m_BoundsOnClick.size[num4];
						this.m_Bounds.size = size;
					}
					if (Event.current.alt)
					{
						this.m_Bounds.center = this.m_BoundsOnClick.center;
					}
				}
				Handles.color = color;
			}
		}

		protected abstract void DrawWireframe();

		protected virtual Bounds OnHandleChanged(PrimitiveBoundsHandle.HandleDirection handle, Bounds boundsOnClick, Bounds newBounds)
		{
			return newBounds;
		}

		protected Vector3 GetSize()
		{
			Vector3 size = this.m_Bounds.size;
			for (int i = 0; i < 3; i++)
			{
				if (!this.IsAxisEnabled(i))
				{
					size[i] = 0f;
				}
			}
			return size;
		}

		protected void SetSize(Vector3 size)
		{
			this.m_Bounds.size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
		}

		protected bool IsAxisEnabled(PrimitiveBoundsHandle.Axes axis)
		{
			return (this.axes & axis) == axis;
		}

		protected bool IsAxisEnabled(int vector3Axis)
		{
			bool result;
			switch (vector3Axis)
			{
			case 0:
				result = this.IsAxisEnabled(PrimitiveBoundsHandle.Axes.X);
				break;
			case 1:
				result = this.IsAxisEnabled(PrimitiveBoundsHandle.Axes.Y);
				break;
			case 2:
				result = this.IsAxisEnabled(PrimitiveBoundsHandle.Axes.Z);
				break;
			default:
				throw new ArgumentOutOfRangeException("vector3Axis", "Must be 0, 1, or 2");
			}
			return result;
		}

		private void MidpointHandles(ref Vector3 minPos, ref Vector3 maxPos, bool isCameraInsideBox)
		{
			Vector3 right = Vector3.right;
			Vector3 up = Vector3.up;
			Vector3 forward = Vector3.forward;
			Vector3 vector = (maxPos + minPos) * 0.5f;
			if (this.IsAxisEnabled(PrimitiveBoundsHandle.Axes.X))
			{
				Vector3 localPos = new Vector3(maxPos.x, vector.y, vector.z);
				maxPos.x = Mathf.Max(this.MidpointHandle(this.m_ControlIDs[0], localPos, up, forward, isCameraInsideBox).x, minPos.x);
				localPos = new Vector3(minPos.x, vector.y, vector.z);
				minPos.x = Mathf.Min(this.MidpointHandle(this.m_ControlIDs[1], localPos, up, -forward, isCameraInsideBox).x, maxPos.x);
			}
			if (this.IsAxisEnabled(PrimitiveBoundsHandle.Axes.Y))
			{
				Vector3 localPos = new Vector3(vector.x, maxPos.y, vector.z);
				maxPos.y = Mathf.Max(this.MidpointHandle(this.m_ControlIDs[2], localPos, right, -forward, isCameraInsideBox).y, minPos.y);
				localPos = new Vector3(vector.x, minPos.y, vector.z);
				minPos.y = Mathf.Min(this.MidpointHandle(this.m_ControlIDs[3], localPos, right, forward, isCameraInsideBox).y, maxPos.y);
			}
			if (this.IsAxisEnabled(PrimitiveBoundsHandle.Axes.Z))
			{
				Vector3 localPos = new Vector3(vector.x, vector.y, maxPos.z);
				maxPos.z = Mathf.Max(this.MidpointHandle(this.m_ControlIDs[4], localPos, up, -right, isCameraInsideBox).z, minPos.z);
				localPos = new Vector3(vector.x, vector.y, minPos.z);
				minPos.z = Mathf.Min(this.MidpointHandle(this.m_ControlIDs[5], localPos, up, right, isCameraInsideBox).z, maxPos.z);
			}
		}

		private Vector3 MidpointHandle(int id, Vector3 localPos, Vector3 localTangent, Vector3 localBinormal, bool isCameraInsideBox)
		{
			Color color = Handles.color;
			this.AdjustMidpointHandleColor(localPos, localTangent, localBinormal, isCameraInsideBox);
			if (Handles.color.a > 0f)
			{
				Vector3 normalized = Vector3.Cross(localTangent, localBinormal).normalized;
				Handles.CapFunction arg_66_0;
				if ((arg_66_0 = this.midpointHandleDrawFunction) == null)
				{
					if (PrimitiveBoundsHandle.<>f__mg$cache0 == null)
					{
						PrimitiveBoundsHandle.<>f__mg$cache0 = new Handles.CapFunction(Handles.DotHandleCap);
					}
					arg_66_0 = PrimitiveBoundsHandle.<>f__mg$cache0;
				}
				Handles.CapFunction capFunction = arg_66_0;
				Handles.SizeFunction arg_92_0;
				if ((arg_92_0 = this.midpointHandleSizeFunction) == null)
				{
					if (PrimitiveBoundsHandle.<>f__mg$cache1 == null)
					{
						PrimitiveBoundsHandle.<>f__mg$cache1 = new Handles.SizeFunction(PrimitiveBoundsHandle.DefaultMidpointHandleSizeFunction);
					}
					arg_92_0 = PrimitiveBoundsHandle.<>f__mg$cache1;
				}
				Handles.SizeFunction sizeFunction = arg_92_0;
				localPos = Slider1D.Do(id, localPos, normalized, sizeFunction(localPos), capFunction, SnapSettings.scale);
			}
			Handles.color = color;
			return localPos;
		}

		private void AdjustMidpointHandleColor(Vector3 localPos, Vector3 localTangent, Vector3 localBinormal, bool isCameraInsideBox)
		{
			float num = 1f;
			if (!isCameraInsideBox && this.axes == PrimitiveBoundsHandle.Axes.All)
			{
				Vector3 lhs = Handles.matrix.MultiplyVector(localTangent);
				Vector3 rhs = Handles.matrix.MultiplyVector(localBinormal);
				Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
				float num2;
				if (Camera.current.orthographic)
				{
					num2 = Vector3.Dot(-Camera.current.transform.forward, normalized);
				}
				else
				{
					num2 = Vector3.Dot((Camera.current.transform.position - Handles.matrix.MultiplyPoint(localPos)).normalized, normalized);
				}
				if (num2 < -0.0001f)
				{
					num *= Handles.backfaceAlphaMultiplier;
				}
			}
			Handles.color *= new Color(1f, 1f, 1f, num);
		}
	}
}
