using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace UnityEditor
{
	internal class PointEditor
	{
		private static Vector2 s_StartMouseDragPosition;
		private static List<int> s_StartDragSelection;
		private static bool s_DidDrag;
		private static Mesh s_Mesh;
		private static Vector3 s_EditingScale = Vector3.one;
		private static Quaternion s_EditingRotation = Quaternion.identity;
		private static Mesh GetMesh()
		{
			PointEditor.s_Mesh = (PointEditor.s_Mesh ?? (Resources.GetBuiltinResource(typeof(Mesh), "New-Sphere.fbx") as Mesh));
			return PointEditor.s_Mesh;
		}
		public static bool MovePoints(IEditablePoint points, Transform cloudTransform, List<int> selection)
		{
			if (selection.Count == 0)
			{
				return false;
			}
			if (Event.current.type == EventType.MouseUp)
			{
				PointEditor.s_EditingScale = Vector3.one;
				PointEditor.s_EditingRotation = Quaternion.identity;
			}
			if (Camera.current)
			{
				Vector3 vector = Vector3.zero;
				vector = ((Tools.pivotMode != PivotMode.Pivot) ? (selection.Aggregate(vector, (Vector3 current, int index) => current + points.GetPosition(index)) / (float)selection.Count) : points.GetPosition(selection[0]));
				vector = cloudTransform.TransformPoint(vector);
				switch (Tools.current)
				{
				case Tool.Move:
				{
					Vector3 position = Handles.PositionHandle(vector, (Tools.pivotRotation != PivotRotation.Local) ? Quaternion.identity : cloudTransform.rotation);
					if (GUI.changed)
					{
						Vector3 b = cloudTransform.InverseTransformPoint(position) - cloudTransform.InverseTransformPoint(vector);
						foreach (int current4 in selection)
						{
							points.SetPosition(current4, points.GetPosition(current4) + b);
						}
						return true;
					}
					break;
				}
				case Tool.Rotate:
				{
					Quaternion rotation = Handles.RotationHandle(PointEditor.s_EditingRotation, vector);
					if (GUI.changed)
					{
						Vector3 b2 = cloudTransform.InverseTransformPoint(vector);
						foreach (int current2 in selection)
						{
							Vector3 vector2 = points.GetPosition(current2) - b2;
							vector2 = Quaternion.Inverse(PointEditor.s_EditingRotation) * vector2;
							vector2 = rotation * vector2;
							vector2 += b2;
							points.SetPosition(current2, vector2);
						}
						PointEditor.s_EditingRotation = rotation;
						return true;
					}
					break;
				}
				case Tool.Scale:
				{
					Vector3 vector3 = Handles.ScaleHandle(PointEditor.s_EditingScale, vector, Quaternion.identity, HandleUtility.GetHandleSize(vector));
					if (GUI.changed)
					{
						Vector3 b3 = cloudTransform.InverseTransformPoint(vector);
						foreach (int current3 in selection)
						{
							Vector3 vector4 = points.GetPosition(current3) - b3;
							vector4.x /= PointEditor.s_EditingScale.x;
							vector4.y /= PointEditor.s_EditingScale.y;
							vector4.z /= PointEditor.s_EditingScale.z;
							vector4.x *= vector3.x;
							vector4.y *= vector3.y;
							vector4.z *= vector3.z;
							vector4 += b3;
							points.SetPosition(current3, vector4);
						}
						PointEditor.s_EditingScale = vector3;
						return true;
					}
					break;
				}
				}
			}
			return false;
		}
		public static int FindNearest(Vector2 point, Transform cloudTransform, IEditablePoint points)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(point);
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			for (int i = 0; i < points.Count; i++)
			{
				float num = 0f;
				Vector3 zero = Vector3.zero;
				if (MathUtils.IntersectRaySphere(ray, cloudTransform.TransformPoint(points.GetPosition(i)), points.GetPointScale() * 0.5f, ref num, ref zero) && num > 0f)
				{
					dictionary.Add(i, num);
				}
			}
			if (dictionary.Count <= 0)
			{
				return -1;
			}
			IOrderedEnumerable<KeyValuePair<int, float>> source = 
				from x in dictionary
				orderby x.Value
				select x;
			return source.First<KeyValuePair<int, float>>().Key;
		}
		public static bool SelectPoints(IEditablePoint points, Transform cloudTransform, ref List<int> selection, bool firstSelect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			if (Event.current.alt && Event.current.type != EventType.Repaint)
			{
				return false;
			}
			bool result = false;
			Event current = Event.current;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if ((HandleUtility.nearestControl == controlID || firstSelect) && current.button == 0)
				{
					if (!current.shift && !EditorGUI.actionKey)
					{
						selection.Clear();
						result = true;
					}
					GUIUtility.hotControl = controlID;
					PointEditor.s_StartMouseDragPosition = current.mousePosition;
					PointEditor.s_StartDragSelection = new List<int>(selection);
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID && current.button == 0)
				{
					if (!PointEditor.s_DidDrag)
					{
						int num = PointEditor.FindNearest(PointEditor.s_StartMouseDragPosition, cloudTransform, points);
						if (num != -1)
						{
							if (!current.shift && !EditorGUI.actionKey)
							{
								selection.Add(num);
							}
							else
							{
								int num2 = selection.IndexOf(num);
								if (num2 != -1)
								{
									selection.RemoveAt(num2);
								}
								else
								{
									selection.Add(num);
								}
							}
						}
						GUI.changed = true;
						result = true;
					}
					PointEditor.s_StartDragSelection = null;
					PointEditor.s_StartMouseDragPosition = Vector2.zero;
					PointEditor.s_DidDrag = false;
					GUIUtility.hotControl = 0;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID && current.button == 0)
				{
					PointEditor.s_DidDrag = true;
					selection.Clear();
					selection.AddRange(PointEditor.s_StartDragSelection);
					Rect rect = PointEditor.FromToRect(PointEditor.s_StartMouseDragPosition, current.mousePosition);
					Matrix4x4 matrix = Handles.matrix;
					Handles.matrix = cloudTransform.localToWorldMatrix;
					for (int i = 0; i < points.Count; i++)
					{
						Vector2 point = HandleUtility.WorldToGUIPoint(points.GetPosition(i));
						if (rect.Contains(point))
						{
							selection.Add(i);
						}
					}
					Handles.matrix = matrix;
					GUI.changed = true;
					current.Use();
				}
				break;
			case EventType.Repaint:
				if (GUIUtility.hotControl == controlID && current.mousePosition != PointEditor.s_StartMouseDragPosition)
				{
					GUIStyle gUIStyle = "SelectionRect";
					Handles.BeginGUI();
					gUIStyle.Draw(PointEditor.FromToRect(PointEditor.s_StartMouseDragPosition, current.mousePosition), false, false, false, false);
					Handles.EndGUI();
				}
				break;
			case EventType.Layout:
				HandleUtility.AddDefaultControl(controlID);
				break;
			}
			selection = selection.Distinct<int>().ToList<int>();
			return result;
		}
		public static void Draw(IEditablePoint points, Transform cloudTransform, List<int> selection, bool twoPassDrawing)
		{
			LightmapVisualization.DrawPointCloud(points.GetUnselectedPositions(), points.GetSelectedPositions(), points.GetDefaultColor(), points.GetSelectedColor(), points.GetPointScale(), cloudTransform);
		}
		private static Rect FromToRect(Vector2 from, Vector2 to)
		{
			Rect result = new Rect(from.x, from.y, to.x - from.x, to.y - from.y);
			if (result.width < 0f)
			{
				result.x += result.width;
				result.width = -result.width;
			}
			if (result.height < 0f)
			{
				result.y += result.height;
				result.height = -result.height;
			}
			return result;
		}
	}
}
