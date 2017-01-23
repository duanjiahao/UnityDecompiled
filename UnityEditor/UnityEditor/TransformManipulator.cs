using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class TransformManipulator
	{
		private struct TransformData
		{
			public static Quaternion[] s_Alignments = new Quaternion[]
			{
				Quaternion.LookRotation(Vector3.right, Vector3.up),
				Quaternion.LookRotation(Vector3.right, Vector3.forward),
				Quaternion.LookRotation(Vector3.up, Vector3.forward),
				Quaternion.LookRotation(Vector3.up, Vector3.right),
				Quaternion.LookRotation(Vector3.forward, Vector3.right),
				Quaternion.LookRotation(Vector3.forward, Vector3.up)
			};

			public Transform transform;

			public Vector3 position;

			public Vector3 localPosition;

			public Quaternion rotation;

			public Vector3 scale;

			public RectTransform rectTransform;

			public Rect rect;

			public Vector2 anchoredPosition;

			public Vector2 sizeDelta;

			public static TransformManipulator.TransformData GetData(Transform t)
			{
				TransformManipulator.TransformData result = default(TransformManipulator.TransformData);
				result.SetupTransformValues(t);
				result.rectTransform = t.GetComponent<RectTransform>();
				if (result.rectTransform != null)
				{
					result.sizeDelta = result.rectTransform.sizeDelta;
					result.rect = result.rectTransform.rect;
					result.anchoredPosition = result.rectTransform.anchoredPosition;
				}
				return result;
			}

			private Quaternion GetRefAlignment(Quaternion targetRotation, Quaternion ownRotation)
			{
				float num = float.NegativeInfinity;
				Quaternion result = Quaternion.identity;
				for (int i = 0; i < TransformManipulator.TransformData.s_Alignments.Length; i++)
				{
					float num2 = Mathf.Min(new float[]
					{
						Mathf.Abs(Vector3.Dot(targetRotation * Vector3.right, ownRotation * TransformManipulator.TransformData.s_Alignments[i] * Vector3.right)),
						Mathf.Abs(Vector3.Dot(targetRotation * Vector3.up, ownRotation * TransformManipulator.TransformData.s_Alignments[i] * Vector3.up)),
						Mathf.Abs(Vector3.Dot(targetRotation * Vector3.forward, ownRotation * TransformManipulator.TransformData.s_Alignments[i] * Vector3.forward))
					});
					if (num2 > num)
					{
						num = num2;
						result = TransformManipulator.TransformData.s_Alignments[i];
					}
				}
				return result;
			}

			private void SetupTransformValues(Transform t)
			{
				this.transform = t;
				this.position = t.position;
				this.localPosition = t.localPosition;
				this.rotation = t.rotation;
				this.scale = t.localScale;
			}

			private void SetScaleValue(Vector3 scale)
			{
				this.transform.localScale = scale;
			}

			public void SetScaleDelta(Vector3 scaleDelta, Vector3 scalePivot, Quaternion scaleRotation, bool preferRectResize)
			{
				this.SetPosition(scaleRotation * Vector3.Scale(Quaternion.Inverse(scaleRotation) * (this.position - scalePivot), scaleDelta) + scalePivot);
				Vector3 minDragDifference = ManipulationToolUtility.minDragDifference;
				if (this.transform.parent != null)
				{
					minDragDifference.x /= this.transform.parent.lossyScale.x;
					minDragDifference.y /= this.transform.parent.lossyScale.y;
					minDragDifference.z /= this.transform.parent.lossyScale.z;
				}
				Quaternion ownRotation = (!Tools.rectBlueprintMode || !InternalEditorUtility.SupportsRectLayout(this.transform)) ? this.rotation : this.transform.parent.rotation;
				Quaternion refAlignment = this.GetRefAlignment(scaleRotation, ownRotation);
				scaleDelta = refAlignment * scaleDelta;
				scaleDelta = Vector3.Scale(scaleDelta, refAlignment * Vector3.one);
				if (preferRectResize && this.rectTransform != null)
				{
					Vector2 vector = this.sizeDelta + Vector2.Scale(this.rect.size, scaleDelta) - this.rect.size;
					vector.x = MathUtils.RoundBasedOnMinimumDifference(vector.x, minDragDifference.x);
					vector.y = MathUtils.RoundBasedOnMinimumDifference(vector.y, minDragDifference.y);
					this.rectTransform.sizeDelta = vector;
					if (this.rectTransform.drivenByObject != null)
					{
						RectTransform.SendReapplyDrivenProperties(this.rectTransform);
					}
				}
				else
				{
					this.SetScaleValue(Vector3.Scale(this.scale, scaleDelta));
				}
			}

			private void SetPosition(Vector3 newPosition)
			{
				this.SetPositionDelta(newPosition - this.position);
			}

			public void SetPositionDelta(Vector3 positionDelta)
			{
				Vector3 vector = positionDelta;
				Vector3 minDragDifference = ManipulationToolUtility.minDragDifference;
				if (this.transform.parent != null)
				{
					vector = this.transform.parent.InverseTransformVector(vector);
					minDragDifference.x /= this.transform.parent.lossyScale.x;
					minDragDifference.y /= this.transform.parent.lossyScale.y;
					minDragDifference.z /= this.transform.parent.lossyScale.z;
				}
				bool flag = Mathf.Approximately(vector.x, 0f);
				bool flag2 = Mathf.Approximately(vector.y, 0f);
				bool flag3 = Mathf.Approximately(vector.z, 0f);
				if (this.rectTransform == null)
				{
					Vector3 vector2 = this.localPosition + vector;
					vector2.x = ((!flag) ? MathUtils.RoundBasedOnMinimumDifference(vector2.x, minDragDifference.x) : this.localPosition.x);
					vector2.y = ((!flag2) ? MathUtils.RoundBasedOnMinimumDifference(vector2.y, minDragDifference.y) : this.localPosition.y);
					vector2.z = ((!flag3) ? MathUtils.RoundBasedOnMinimumDifference(vector2.z, minDragDifference.z) : this.localPosition.z);
					this.transform.localPosition = vector2;
				}
				else
				{
					Vector3 vector3 = this.localPosition + vector;
					vector3.z = ((!flag3) ? MathUtils.RoundBasedOnMinimumDifference(vector3.z, minDragDifference.z) : this.localPosition.z);
					this.transform.localPosition = vector3;
					Vector2 vector4 = this.anchoredPosition + vector;
					vector4.x = ((!flag) ? MathUtils.RoundBasedOnMinimumDifference(vector4.x, minDragDifference.x) : this.anchoredPosition.x);
					vector4.y = ((!flag2) ? MathUtils.RoundBasedOnMinimumDifference(vector4.y, minDragDifference.y) : this.anchoredPosition.y);
					this.rectTransform.anchoredPosition = vector4;
					if (this.rectTransform.drivenByObject != null)
					{
						RectTransform.SendReapplyDrivenProperties(this.rectTransform);
					}
				}
			}

			public void DebugAlignment(Quaternion targetRotation)
			{
				Quaternion rhs = Quaternion.identity;
				if (!TransformManipulator.individualSpace)
				{
					rhs = this.GetRefAlignment(targetRotation, this.rotation);
				}
				Vector3 a = this.transform.position;
				float d = HandleUtility.GetHandleSize(a) * 0.25f;
				Color color = Handles.color;
				Handles.color = Color.red;
				Vector3 b = this.rotation * rhs * Vector3.right * d;
				Handles.DrawLine(a - b, a + b);
				Handles.color = Color.green;
				b = this.rotation * rhs * Vector3.up * d;
				Handles.DrawLine(a - b, a + b);
				Handles.color = Color.blue;
				b = this.rotation * rhs * Vector3.forward * d;
				Handles.DrawLine(a - b, a + b);
				Handles.color = color;
			}
		}

		private static EventType s_EventTypeBefore = EventType.Ignore;

		private static TransformManipulator.TransformData[] s_MouseDownState = null;

		private static Vector3 s_StartHandlePosition = Vector3.zero;

		private static Vector3 s_StartLocalHandleOffset = Vector3.zero;

		private static int s_HotControl = 0;

		private static bool s_LockHandle = false;

		public static Vector3 mouseDownHandlePosition
		{
			get
			{
				return TransformManipulator.s_StartHandlePosition;
			}
		}

		public static bool active
		{
			get
			{
				return TransformManipulator.s_MouseDownState != null;
			}
		}

		public static bool individualSpace
		{
			get
			{
				return Tools.pivotRotation == PivotRotation.Local && Tools.pivotMode == PivotMode.Pivot;
			}
		}

		private static void BeginEventCheck()
		{
			TransformManipulator.s_EventTypeBefore = Event.current.GetTypeForControl(TransformManipulator.s_HotControl);
		}

		private static EventType EndEventCheck()
		{
			EventType eventType = (TransformManipulator.s_EventTypeBefore == Event.current.GetTypeForControl(TransformManipulator.s_HotControl)) ? EventType.Ignore : TransformManipulator.s_EventTypeBefore;
			TransformManipulator.s_EventTypeBefore = EventType.Ignore;
			if (eventType == EventType.MouseDown)
			{
				TransformManipulator.s_HotControl = GUIUtility.hotControl;
			}
			else if (eventType == EventType.MouseUp)
			{
				TransformManipulator.s_HotControl = 0;
			}
			return eventType;
		}

		public static void BeginManipulationHandling(bool lockHandleWhileDragging)
		{
			TransformManipulator.BeginEventCheck();
			TransformManipulator.s_LockHandle = lockHandleWhileDragging;
		}

		public static EventType EndManipulationHandling()
		{
			EventType eventType = TransformManipulator.EndEventCheck();
			if (eventType == EventType.MouseDown)
			{
				TransformManipulator.RecordMouseDownState(Selection.transforms);
				TransformManipulator.s_StartHandlePosition = Tools.handlePosition;
				TransformManipulator.s_StartLocalHandleOffset = Tools.localHandleOffset;
				if (TransformManipulator.s_LockHandle)
				{
					Tools.LockHandlePosition();
				}
				Tools.LockHandleRectRotation();
			}
			else if (TransformManipulator.s_MouseDownState != null && (eventType == EventType.MouseUp || GUIUtility.hotControl != TransformManipulator.s_HotControl))
			{
				TransformManipulator.s_MouseDownState = null;
				if (TransformManipulator.s_LockHandle)
				{
					Tools.UnlockHandlePosition();
				}
				Tools.UnlockHandleRectRotation();
				ManipulationToolUtility.DisableMinDragDifference();
			}
			return eventType;
		}

		private static void RecordMouseDownState(Transform[] transforms)
		{
			TransformManipulator.s_MouseDownState = new TransformManipulator.TransformData[transforms.Length];
			for (int i = 0; i < transforms.Length; i++)
			{
				TransformManipulator.s_MouseDownState[i] = TransformManipulator.TransformData.GetData(transforms[i]);
			}
		}

		private static void SetLocalHandleOffsetScaleDelta(Vector3 scaleDelta, Quaternion pivotRotation)
		{
			Quaternion rotation = Quaternion.Inverse(Tools.handleRotation) * pivotRotation;
			Tools.localHandleOffset = Vector3.Scale(Vector3.Scale(TransformManipulator.s_StartLocalHandleOffset, rotation * scaleDelta), rotation * Vector3.one);
		}

		public static void SetScaleDelta(Vector3 scaleDelta, Quaternion pivotRotation)
		{
			if (TransformManipulator.s_MouseDownState != null)
			{
				TransformManipulator.SetLocalHandleOffsetScaleDelta(scaleDelta, pivotRotation);
				for (int i = 0; i < TransformManipulator.s_MouseDownState.Length; i++)
				{
					TransformManipulator.TransformData transformData = TransformManipulator.s_MouseDownState[i];
					Undo.RecordObject(transformData.transform, "Scale");
				}
				Vector3 scalePivot = Tools.handlePosition;
				for (int j = 0; j < TransformManipulator.s_MouseDownState.Length; j++)
				{
					if (Tools.pivotMode == PivotMode.Pivot)
					{
						scalePivot = TransformManipulator.s_MouseDownState[j].position;
					}
					if (TransformManipulator.individualSpace)
					{
						pivotRotation = TransformManipulator.s_MouseDownState[j].rotation;
					}
					TransformManipulator.s_MouseDownState[j].SetScaleDelta(scaleDelta, scalePivot, pivotRotation, false);
				}
			}
		}

		public static void SetResizeDelta(Vector3 scaleDelta, Vector3 pivotPosition, Quaternion pivotRotation)
		{
			if (TransformManipulator.s_MouseDownState != null)
			{
				TransformManipulator.SetLocalHandleOffsetScaleDelta(scaleDelta, pivotRotation);
				for (int i = 0; i < TransformManipulator.s_MouseDownState.Length; i++)
				{
					TransformManipulator.TransformData transformData = TransformManipulator.s_MouseDownState[i];
					Undo.RecordObject((!(transformData.rectTransform != null)) ? transformData.transform : transformData.rectTransform, "Resize");
				}
				for (int j = 0; j < TransformManipulator.s_MouseDownState.Length; j++)
				{
					TransformManipulator.s_MouseDownState[j].SetScaleDelta(scaleDelta, pivotPosition, pivotRotation, true);
				}
			}
		}

		public static void SetPositionDelta(Vector3 positionDelta)
		{
			if (TransformManipulator.s_MouseDownState != null)
			{
				for (int i = 0; i < TransformManipulator.s_MouseDownState.Length; i++)
				{
					TransformManipulator.TransformData transformData = TransformManipulator.s_MouseDownState[i];
					Undo.RecordObject((!(transformData.rectTransform != null)) ? transformData.transform : transformData.rectTransform, "Move");
				}
				for (int j = 0; j < TransformManipulator.s_MouseDownState.Length; j++)
				{
					TransformManipulator.s_MouseDownState[j].SetPositionDelta(positionDelta);
				}
			}
		}

		public static void DebugAlignment(Quaternion targetRotation)
		{
			if (TransformManipulator.s_MouseDownState != null)
			{
				for (int i = 0; i < TransformManipulator.s_MouseDownState.Length; i++)
				{
					TransformManipulator.s_MouseDownState[i].DebugAlignment(targetRotation);
				}
			}
		}
	}
}
