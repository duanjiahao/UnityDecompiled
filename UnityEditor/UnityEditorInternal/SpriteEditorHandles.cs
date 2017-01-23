using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	public static class SpriteEditorHandles
	{
		private static Vector2 s_CurrentMousePosition;

		private static Vector2 s_DragStartScreenPosition;

		private static Vector2 s_DragScreenOffset;

		private static int s_RectSelectionID = GUIUtility.GetPermanentControlID();

		internal static Vector2 PointSlider(Vector2 pos, MouseCursor cursor, GUIStyle dragDot, GUIStyle dragDotActive)
		{
			int controlID = GUIUtility.GetControlID("Slider1D".GetHashCode(), FocusType.Keyboard);
			Vector2 vector = Handles.matrix.MultiplyPoint(pos);
			Rect rect = new Rect(vector.x - dragDot.fixedWidth * 0.5f, vector.y - dragDot.fixedHeight * 0.5f, dragDot.fixedWidth, dragDot.fixedHeight);
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(controlID);
			if (typeForControl == EventType.Repaint)
			{
				if (GUIUtility.hotControl == controlID)
				{
					dragDotActive.Draw(rect, GUIContent.none, controlID);
				}
				else
				{
					dragDot.Draw(rect, GUIContent.none, controlID);
				}
			}
			return SpriteEditorHandles.ScaleSlider(pos, cursor, rect);
		}

		internal static Vector2 ScaleSlider(Vector2 pos, MouseCursor cursor, Rect cursorRect)
		{
			int controlID = GUIUtility.GetControlID("Slider1D".GetHashCode(), FocusType.Keyboard);
			return SpriteEditorHandles.ScaleSlider(controlID, pos, cursor, cursorRect);
		}

		private static Vector2 ScaleSlider(int id, Vector2 pos, MouseCursor cursor, Rect cursorRect)
		{
			Vector2 b = Handles.matrix.MultiplyPoint(pos);
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if (current.button == 0 && cursorRect.Contains(Event.current.mousePosition) && !current.alt)
				{
					GUIUtility.keyboardControl = id;
					GUIUtility.hotControl = id;
					SpriteEditorHandles.s_CurrentMousePosition = current.mousePosition;
					SpriteEditorHandles.s_DragStartScreenPosition = current.mousePosition;
					SpriteEditorHandles.s_DragScreenOffset = SpriteEditorHandles.s_CurrentMousePosition - b;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = 0;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					SpriteEditorHandles.s_CurrentMousePosition += current.delta;
					Vector2 a = pos;
					pos = Handles.s_InverseMatrix.MultiplyPoint(SpriteEditorHandles.s_CurrentMousePosition);
					if (!Mathf.Approximately((a - pos).magnitude, 0f))
					{
						GUI.changed = true;
					}
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.hotControl == id)
				{
					if (current.keyCode == KeyCode.Escape)
					{
						pos = Handles.s_InverseMatrix.MultiplyPoint(SpriteEditorHandles.s_DragStartScreenPosition - SpriteEditorHandles.s_DragScreenOffset);
						GUIUtility.hotControl = 0;
						GUI.changed = true;
						current.Use();
					}
				}
				break;
			case EventType.Repaint:
				EditorGUIUtility.AddCursorRect(cursorRect, cursor, id);
				break;
			}
			return pos;
		}

		internal static Vector2 PivotSlider(Rect sprite, Vector2 pos, GUIStyle pivotDot, GUIStyle pivotDotActive)
		{
			int controlID = GUIUtility.GetControlID("Slider1D".GetHashCode(), FocusType.Keyboard);
			pos = new Vector2(sprite.xMin + sprite.width * pos.x, sprite.yMin + sprite.height * pos.y);
			Vector2 vector = Handles.matrix.MultiplyPoint(pos);
			Rect position = new Rect(vector.x - pivotDot.fixedWidth * 0.5f, vector.y - pivotDot.fixedHeight * 0.5f, pivotDotActive.fixedWidth, pivotDotActive.fixedHeight);
			Event current = Event.current;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (current.button == 0 && position.Contains(Event.current.mousePosition) && !current.alt)
				{
					int num = controlID;
					GUIUtility.keyboardControl = num;
					GUIUtility.hotControl = num;
					SpriteEditorHandles.s_CurrentMousePosition = current.mousePosition;
					SpriteEditorHandles.s_DragStartScreenPosition = current.mousePosition;
					Vector2 b = Handles.matrix.MultiplyPoint(pos);
					SpriteEditorHandles.s_DragScreenOffset = SpriteEditorHandles.s_CurrentMousePosition - b;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = 0;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					SpriteEditorHandles.s_CurrentMousePosition += current.delta;
					Vector2 a = pos;
					Vector3 vector2 = Handles.s_InverseMatrix.MultiplyPoint(SpriteEditorHandles.s_CurrentMousePosition - SpriteEditorHandles.s_DragScreenOffset);
					pos = new Vector2(vector2.x, vector2.y);
					if (!Mathf.Approximately((a - pos).magnitude, 0f))
					{
						GUI.changed = true;
					}
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.hotControl == controlID)
				{
					if (current.keyCode == KeyCode.Escape)
					{
						pos = Handles.s_InverseMatrix.MultiplyPoint(SpriteEditorHandles.s_DragStartScreenPosition - SpriteEditorHandles.s_DragScreenOffset);
						GUIUtility.hotControl = 0;
						GUI.changed = true;
						current.Use();
					}
				}
				break;
			case EventType.Repaint:
				EditorGUIUtility.AddCursorRect(position, MouseCursor.Arrow, controlID);
				if (GUIUtility.hotControl == controlID)
				{
					pivotDotActive.Draw(position, GUIContent.none, controlID);
				}
				else
				{
					pivotDot.Draw(position, GUIContent.none, controlID);
				}
				break;
			}
			pos = new Vector2((pos.x - sprite.xMin) / sprite.width, (pos.y - sprite.yMin) / sprite.height);
			return pos;
		}

		internal static Rect SliderRect(Rect pos)
		{
			int controlID = GUIUtility.GetControlID("SliderRect".GetHashCode(), FocusType.Keyboard);
			Event current = Event.current;
			if (SpriteEditorWindow.s_OneClickDragStarted && current.type == EventType.Repaint)
			{
				SpriteEditorHandles.HandleSliderRectMouseDown(controlID, current, pos);
				SpriteEditorWindow.s_OneClickDragStarted = false;
			}
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (current.button == 0 && pos.Contains(Handles.s_InverseMatrix.MultiplyPoint(Event.current.mousePosition)) && !current.alt)
				{
					SpriteEditorHandles.HandleSliderRectMouseDown(controlID, current, pos);
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = 0;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					SpriteEditorHandles.s_CurrentMousePosition += current.delta;
					Vector2 center = pos.center;
					pos.center = Handles.s_InverseMatrix.MultiplyPoint(SpriteEditorHandles.s_CurrentMousePosition - SpriteEditorHandles.s_DragScreenOffset);
					if (!Mathf.Approximately((center - pos.center).magnitude, 0f))
					{
						GUI.changed = true;
					}
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.hotControl == controlID)
				{
					if (current.keyCode == KeyCode.Escape)
					{
						pos.center = Handles.s_InverseMatrix.MultiplyPoint(SpriteEditorHandles.s_DragStartScreenPosition - SpriteEditorHandles.s_DragScreenOffset);
						GUIUtility.hotControl = 0;
						GUI.changed = true;
						current.Use();
					}
				}
				break;
			case EventType.Repaint:
			{
				Vector2 vector = Handles.s_InverseMatrix.MultiplyPoint(new Vector2(pos.xMin, pos.yMin));
				Vector2 vector2 = Handles.s_InverseMatrix.MultiplyPoint(new Vector2(pos.xMax, pos.yMax));
				EditorGUIUtility.AddCursorRect(new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y), MouseCursor.Arrow, controlID);
				break;
			}
			}
			return pos;
		}

		internal static void HandleSliderRectMouseDown(int id, Event evt, Rect pos)
		{
			GUIUtility.keyboardControl = id;
			GUIUtility.hotControl = id;
			SpriteEditorHandles.s_CurrentMousePosition = evt.mousePosition;
			SpriteEditorHandles.s_DragStartScreenPosition = evt.mousePosition;
			Vector2 b = Handles.matrix.MultiplyPoint(pos.center);
			SpriteEditorHandles.s_DragScreenOffset = SpriteEditorHandles.s_CurrentMousePosition - b;
			EditorGUIUtility.SetWantsMouseJumping(1);
		}

		internal static Rect RectCreator(float textureWidth, float textureHeight, GUIStyle rectStyle)
		{
			Event current = Event.current;
			Vector2 mousePosition = current.mousePosition;
			int num = SpriteEditorHandles.s_RectSelectionID;
			Rect result = default(Rect);
			switch (current.GetTypeForControl(num))
			{
			case EventType.MouseDown:
				if (current.button == 0)
				{
					GUIUtility.hotControl = num;
					Rect rect = new Rect(0f, 0f, textureWidth, textureHeight);
					Vector2 v = Handles.s_InverseMatrix.MultiplyPoint(mousePosition);
					v.x = Mathf.Min(Mathf.Max(v.x, rect.xMin), rect.xMax);
					v.y = Mathf.Min(Mathf.Max(v.y, rect.yMin), rect.yMax);
					SpriteEditorHandles.s_DragStartScreenPosition = Handles.s_Matrix.MultiplyPoint(v);
					SpriteEditorHandles.s_CurrentMousePosition = mousePosition;
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == num && current.button == 0)
				{
					if (SpriteEditorHandles.ValidRect(SpriteEditorHandles.s_DragStartScreenPosition, SpriteEditorHandles.s_CurrentMousePosition))
					{
						result = SpriteEditorHandles.GetCurrentRect(false, textureWidth, textureHeight, SpriteEditorHandles.s_DragStartScreenPosition, SpriteEditorHandles.s_CurrentMousePosition);
						GUI.changed = true;
						current.Use();
					}
					GUIUtility.hotControl = 0;
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == num)
				{
					SpriteEditorHandles.s_CurrentMousePosition = new Vector2(mousePosition.x, mousePosition.y);
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.hotControl == num)
				{
					if (current.keyCode == KeyCode.Escape)
					{
						GUIUtility.hotControl = 0;
						GUI.changed = true;
						current.Use();
					}
				}
				break;
			case EventType.Repaint:
				if (GUIUtility.hotControl == num && SpriteEditorHandles.ValidRect(SpriteEditorHandles.s_DragStartScreenPosition, SpriteEditorHandles.s_CurrentMousePosition))
				{
					SpriteEditorUtility.BeginLines(Color.green * 1.5f);
					SpriteEditorUtility.DrawBox(SpriteEditorHandles.GetCurrentRect(false, textureWidth, textureHeight, SpriteEditorHandles.s_DragStartScreenPosition, SpriteEditorHandles.s_CurrentMousePosition));
					SpriteEditorUtility.EndLines();
				}
				break;
			}
			return result;
		}

		private static bool ValidRect(Vector2 startPoint, Vector2 endPoint)
		{
			return Mathf.Abs((endPoint - startPoint).x) > 5f && Mathf.Abs((endPoint - startPoint).y) > 5f;
		}

		private static Rect GetCurrentRect(bool screenSpace, float textureWidth, float textureHeight, Vector2 startPoint, Vector2 endPoint)
		{
			Rect rect = EditorGUIExt.FromToRect(Handles.s_InverseMatrix.MultiplyPoint(startPoint), Handles.s_InverseMatrix.MultiplyPoint(endPoint));
			rect = SpriteEditorUtility.ClampedRect(SpriteEditorUtility.RoundToInt(rect), new Rect(0f, 0f, textureWidth, textureHeight), false);
			if (screenSpace)
			{
				Vector2 vector = Handles.matrix.MultiplyPoint(new Vector2(rect.xMin, rect.yMin));
				Vector2 vector2 = Handles.matrix.MultiplyPoint(new Vector2(rect.xMax, rect.yMax));
				rect = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
			}
			return rect;
		}
	}
}
