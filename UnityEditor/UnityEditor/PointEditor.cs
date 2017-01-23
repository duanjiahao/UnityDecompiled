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

		private static List<int> s_SelectionStart;

		public static bool MovePoints(IEditablePoint points, Transform cloudTransform, List<int> selection)
		{
			bool result;
			if (selection.Count == 0)
			{
				result = false;
			}
			else
			{
				if (Camera.current)
				{
					Vector3 vector = Vector3.zero;
					vector = ((Tools.pivotMode != PivotMode.Pivot) ? (selection.Aggregate(vector, (Vector3 current, int index) => current + points.GetPosition(index)) / (float)selection.Count) : points.GetPosition(selection[0]));
					vector = cloudTransform.TransformPoint(vector);
					Vector3 position = Handles.PositionHandle(vector, (Tools.pivotRotation != PivotRotation.Local) ? Quaternion.identity : cloudTransform.rotation);
					if (GUI.changed)
					{
						Vector3 b = cloudTransform.InverseTransformPoint(position) - cloudTransform.InverseTransformPoint(vector);
						foreach (int current2 in selection)
						{
							points.SetPosition(current2, points.GetPosition(current2) + b);
						}
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		public static int FindNearest(Vector2 point, Transform cloudTransform, IEditablePoint points)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(point);
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			for (int i = 0; i < points.Count; i++)
			{
				float num = 0f;
				Vector3 zero = Vector3.zero;
				if (MathUtils.IntersectRaySphere(ray, cloudTransform.TransformPoint(points.GetPosition(i)), points.GetPointScale() * 0.5f, ref num, ref zero))
				{
					if (num > 0f)
					{
						dictionary.Add(i, num);
					}
				}
			}
			int result;
			if (dictionary.Count <= 0)
			{
				result = -1;
			}
			else
			{
				IOrderedEnumerable<KeyValuePair<int, float>> source = from x in dictionary
				orderby x.Value
				select x;
				result = source.First<KeyValuePair<int, float>>().Key;
			}
			return result;
		}

		public static bool SelectPoints(IEditablePoint points, Transform cloudTransform, ref List<int> selection, bool firstSelect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			bool result;
			if (Event.current.alt && Event.current.type != EventType.Repaint)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				Event current = Event.current;
				switch (current.GetTypeForControl(controlID))
				{
				case EventType.MouseDown:
					if ((HandleUtility.nearestControl == controlID || firstSelect) && current.button == 0)
					{
						if (!current.shift && !EditorGUI.actionKey)
						{
							selection.Clear();
							flag = true;
						}
						PointEditor.s_SelectionStart = new List<int>(selection);
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
							flag = true;
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
								if (EditorGUI.actionKey)
								{
									if (PointEditor.s_SelectionStart.Contains(i))
									{
										selection.Remove(i);
									}
								}
								else if (!PointEditor.s_SelectionStart.Contains(i))
								{
									selection.Add(i);
								}
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
				if (flag)
				{
					selection = selection.Distinct<int>().ToList<int>();
				}
				result = flag;
			}
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
