using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal class PolygonEditorUtility
	{
		private const float k_HandlePointSnap = 0.2f;

		private const float k_HandlePickDistance = 50f;

		private Collider2D m_ActiveCollider;

		private bool m_LoopingCollider = false;

		private int m_MinPathPoints = 3;

		private int m_SelectedPath = -1;

		private int m_SelectedVertex = -1;

		private float m_SelectedDistance = 0f;

		private int m_SelectedEdgePath = -1;

		private int m_SelectedEdgeVertex0 = -1;

		private int m_SelectedEdgeVertex1 = -1;

		private float m_SelectedEdgeDistance = 0f;

		private bool m_LeftIntersect = false;

		private bool m_RightIntersect = false;

		private bool m_DeleteMode = false;

		private bool m_FirstOnSceneGUIAfterReset;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache0;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache1;

		public void Reset()
		{
			this.m_SelectedPath = -1;
			this.m_SelectedVertex = -1;
			this.m_SelectedEdgePath = -1;
			this.m_SelectedEdgeVertex0 = -1;
			this.m_SelectedEdgeVertex1 = -1;
			this.m_LeftIntersect = false;
			this.m_RightIntersect = false;
			this.m_FirstOnSceneGUIAfterReset = true;
		}

		private void UndoRedoPerformed()
		{
			if (this.m_ActiveCollider != null)
			{
				Collider2D activeCollider = this.m_ActiveCollider;
				this.StopEditing();
				this.StartEditing(activeCollider);
			}
		}

		public void StartEditing(Collider2D collider)
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			this.Reset();
			PolygonCollider2D polygonCollider2D = collider as PolygonCollider2D;
			if (polygonCollider2D)
			{
				this.m_ActiveCollider = collider;
				this.m_LoopingCollider = true;
				this.m_MinPathPoints = 3;
				PolygonEditor.StartEditing(polygonCollider2D);
			}
			else
			{
				EdgeCollider2D edgeCollider2D = collider as EdgeCollider2D;
				if (!edgeCollider2D)
				{
					throw new NotImplementedException(string.Format("PolygonEditorUtility does not support {0}", collider));
				}
				this.m_ActiveCollider = collider;
				this.m_LoopingCollider = false;
				this.m_MinPathPoints = 2;
				PolygonEditor.StartEditing(edgeCollider2D);
			}
		}

		public void StopEditing()
		{
			PolygonEditor.StopEditing();
			this.m_ActiveCollider = null;
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		private void ApplyEditing(Collider2D collider)
		{
			PolygonCollider2D polygonCollider2D = collider as PolygonCollider2D;
			if (polygonCollider2D)
			{
				PolygonEditor.ApplyEditing(polygonCollider2D);
			}
			else
			{
				EdgeCollider2D edgeCollider2D = collider as EdgeCollider2D;
				if (!edgeCollider2D)
				{
					throw new NotImplementedException(string.Format("PolygonEditorUtility does not support {0}", collider));
				}
				PolygonEditor.ApplyEditing(edgeCollider2D);
			}
		}

		public void OnSceneGUI()
		{
			if (!(this.m_ActiveCollider == null))
			{
				if (!Tools.viewToolActive)
				{
					Vector2 offset = this.m_ActiveCollider.offset;
					Event current = Event.current;
					this.m_DeleteMode = (current.command || current.control);
					Transform transform = this.m_ActiveCollider.transform;
					GUIUtility.keyboardControl = 0;
					HandleUtility.s_CustomPickDistance = 50f;
					Plane plane = new Plane(-transform.forward, Vector3.zero);
					Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
					float distance;
					plane.Raycast(ray, out distance);
					Vector3 point = ray.GetPoint(distance);
					Vector2 vector = transform.InverseTransformPoint(point);
					if (current.type == EventType.MouseMove || this.m_FirstOnSceneGUIAfterReset)
					{
						int num;
						int num2;
						float num3;
						if (PolygonEditor.GetNearestPoint(vector - offset, out num, out num2, out num3))
						{
							this.m_SelectedPath = num;
							this.m_SelectedVertex = num2;
							this.m_SelectedDistance = num3;
						}
						else
						{
							this.m_SelectedPath = -1;
						}
						int selectedEdgeVertex;
						if (PolygonEditor.GetNearestEdge(vector - offset, out num, out num2, out selectedEdgeVertex, out num3, this.m_LoopingCollider))
						{
							this.m_SelectedEdgePath = num;
							this.m_SelectedEdgeVertex0 = num2;
							this.m_SelectedEdgeVertex1 = selectedEdgeVertex;
							this.m_SelectedEdgeDistance = num3;
						}
						else
						{
							this.m_SelectedEdgePath = -1;
						}
						if (current.type == EventType.MouseMove)
						{
							current.Use();
						}
					}
					else if (current.type == EventType.MouseUp)
					{
						this.m_LeftIntersect = false;
						this.m_RightIntersect = false;
					}
					bool flag = false;
					bool flag2 = false;
					if (this.m_SelectedPath != -1 && this.m_SelectedEdgePath != -1)
					{
						Vector2 vector2;
						PolygonEditor.GetPoint(this.m_SelectedPath, this.m_SelectedVertex, out vector2);
						vector2 += offset;
						Vector3 position = transform.TransformPoint(vector2);
						float num4 = HandleUtility.GetHandleSize(position) * 0.2f;
						flag2 = (this.m_SelectedEdgeDistance < this.m_SelectedDistance - num4);
						flag = !flag2;
					}
					else if (this.m_SelectedPath != -1)
					{
						flag = true;
					}
					else if (this.m_SelectedEdgePath != -1)
					{
						flag2 = true;
					}
					if (this.m_DeleteMode && flag2)
					{
						flag2 = false;
						flag = true;
					}
					bool flag3 = false;
					if (flag2 && !this.m_DeleteMode)
					{
						Vector2 vector3;
						PolygonEditor.GetPoint(this.m_SelectedEdgePath, this.m_SelectedEdgeVertex0, out vector3);
						Vector2 vector4;
						PolygonEditor.GetPoint(this.m_SelectedEdgePath, this.m_SelectedEdgeVertex1, out vector4);
						vector3 += offset;
						vector4 += offset;
						Vector3 vector5 = transform.TransformPoint(vector3);
						Vector3 vector6 = transform.TransformPoint(vector4);
						vector5.z = (vector6.z = 0f);
						Handles.color = Color.green;
						Handles.DrawAAPolyLine(4f, new Vector3[]
						{
							vector5,
							vector6
						});
						Handles.color = Color.white;
						Vector2 v = this.GetNearestPointOnEdge(transform.TransformPoint(vector), vector5, vector6);
						EditorGUI.BeginChangeCheck();
						float num5 = HandleUtility.GetHandleSize(v) * 0.04f;
						Handles.color = Color.green;
						Vector3 arg_3CB_0 = v;
						Vector3 arg_3CB_1 = new Vector3(0f, 0f, 1f);
						Vector3 arg_3CB_2 = new Vector3(1f, 0f, 0f);
						Vector3 arg_3CB_3 = new Vector3(0f, 1f, 0f);
						float arg_3CB_4 = num5;
						if (PolygonEditorUtility.<>f__mg$cache0 == null)
						{
							PolygonEditorUtility.<>f__mg$cache0 = new Handles.CapFunction(Handles.DotHandleCap);
						}
						v = Handles.Slider2D(arg_3CB_0, arg_3CB_1, arg_3CB_2, arg_3CB_3, arg_3CB_4, PolygonEditorUtility.<>f__mg$cache0, Vector3.zero);
						Handles.color = Color.white;
						if (EditorGUI.EndChangeCheck())
						{
							PolygonEditor.InsertPoint(this.m_SelectedEdgePath, this.m_SelectedEdgeVertex1, (vector3 + vector4) / 2f - offset);
							this.m_SelectedPath = this.m_SelectedEdgePath;
							this.m_SelectedVertex = this.m_SelectedEdgeVertex1;
							this.m_SelectedDistance = 0f;
							flag = true;
							flag3 = true;
						}
					}
					if (flag)
					{
						Vector2 vector7;
						PolygonEditor.GetPoint(this.m_SelectedPath, this.m_SelectedVertex, out vector7);
						vector7 += offset;
						Vector3 vector8 = transform.TransformPoint(vector7);
						vector8.z = 0f;
						Vector2 a = HandleUtility.WorldToGUIPoint(vector8);
						float num6 = HandleUtility.GetHandleSize(vector8) * 0.04f;
						if ((this.m_DeleteMode && current.type == EventType.MouseDown && Vector2.Distance(a, Event.current.mousePosition) < 50f) || this.DeleteCommandEvent(current))
						{
							if (current.type != EventType.ValidateCommand)
							{
								int pointCount = PolygonEditor.GetPointCount(this.m_SelectedPath);
								if (pointCount > this.m_MinPathPoints)
								{
									PolygonEditor.RemovePoint(this.m_SelectedPath, this.m_SelectedVertex);
									this.Reset();
									flag3 = true;
								}
							}
							current.Use();
						}
						EditorGUI.BeginChangeCheck();
						Handles.color = ((!this.m_DeleteMode) ? Color.green : Color.red);
						Vector3 arg_5AF_0 = vector8;
						Vector3 arg_5AF_1 = new Vector3(0f, 0f, 1f);
						Vector3 arg_5AF_2 = new Vector3(1f, 0f, 0f);
						Vector3 arg_5AF_3 = new Vector3(0f, 1f, 0f);
						float arg_5AF_4 = num6;
						if (PolygonEditorUtility.<>f__mg$cache1 == null)
						{
							PolygonEditorUtility.<>f__mg$cache1 = new Handles.CapFunction(Handles.DotHandleCap);
						}
						Vector3 vector9 = Handles.Slider2D(arg_5AF_0, arg_5AF_1, arg_5AF_2, arg_5AF_3, arg_5AF_4, PolygonEditorUtility.<>f__mg$cache1, Vector3.zero);
						Handles.color = Color.white;
						if (EditorGUI.EndChangeCheck() && !this.m_DeleteMode)
						{
							vector7 = transform.InverseTransformPoint(vector9);
							vector7 -= offset;
							PolygonEditor.TestPointMove(this.m_SelectedPath, this.m_SelectedVertex, vector7, out this.m_LeftIntersect, out this.m_RightIntersect, this.m_LoopingCollider);
							PolygonEditor.SetPoint(this.m_SelectedPath, this.m_SelectedVertex, vector7);
							flag3 = true;
						}
						if (!flag3)
						{
							this.DrawEdgesForSelectedPoint(vector9, transform, this.m_LeftIntersect, this.m_RightIntersect, this.m_LoopingCollider);
						}
					}
					if (flag3)
					{
						Undo.RecordObject(this.m_ActiveCollider, "Edit Collider");
						PolygonEditor.ApplyEditing(this.m_ActiveCollider);
					}
					if (this.DeleteCommandEvent(current))
					{
						Event.current.Use();
					}
					this.m_FirstOnSceneGUIAfterReset = false;
				}
			}
		}

		private bool DeleteCommandEvent(Event evt)
		{
			return (evt.type == EventType.ExecuteCommand || evt.type == EventType.ValidateCommand) && (evt.commandName == "Delete" || evt.commandName == "SoftDelete");
		}

		private void DrawEdgesForSelectedPoint(Vector3 worldPos, Transform transform, bool leftIntersect, bool rightIntersect, bool loop)
		{
			bool flag = true;
			bool flag2 = true;
			int pointCount = PolygonEditor.GetPointCount(this.m_SelectedPath);
			int num = this.m_SelectedVertex - 1;
			if (num == -1)
			{
				num = pointCount - 1;
				flag = loop;
			}
			int num2 = this.m_SelectedVertex + 1;
			if (num2 == pointCount)
			{
				num2 = 0;
				flag2 = loop;
			}
			Vector2 offset = this.m_ActiveCollider.offset;
			Vector2 vector;
			PolygonEditor.GetPoint(this.m_SelectedPath, num, out vector);
			Vector2 vector2;
			PolygonEditor.GetPoint(this.m_SelectedPath, num2, out vector2);
			vector += offset;
			vector2 += offset;
			Vector3 vector3 = transform.TransformPoint(vector);
			Vector3 vector4 = transform.TransformPoint(vector2);
			vector3.z = (vector4.z = worldPos.z);
			float width = 4f;
			if (flag)
			{
				Handles.color = ((!leftIntersect && !this.m_DeleteMode) ? Color.green : Color.red);
				Handles.DrawAAPolyLine(width, new Vector3[]
				{
					worldPos,
					vector3
				});
			}
			if (flag2)
			{
				Handles.color = ((!rightIntersect && !this.m_DeleteMode) ? Color.green : Color.red);
				Handles.DrawAAPolyLine(width, new Vector3[]
				{
					worldPos,
					vector4
				});
			}
			Handles.color = Color.white;
		}

		private Vector2 GetNearestPointOnEdge(Vector2 point, Vector2 start, Vector2 end)
		{
			Vector2 rhs = point - start;
			Vector2 normalized = (end - start).normalized;
			float num = Vector2.Dot(normalized, rhs);
			Vector2 result;
			if (num <= 0f)
			{
				result = start;
			}
			else if (num >= Vector2.Distance(start, end))
			{
				result = end;
			}
			else
			{
				Vector2 b = normalized * num;
				result = start + b;
			}
			return result;
		}
	}
}
