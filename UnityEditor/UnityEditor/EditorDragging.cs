using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class EditorDragging
	{
		private enum DraggingMode
		{
			NotApplicable,
			Component,
			Script
		}

		private static class Styles
		{
			public static readonly GUIStyle insertionMarker = "InsertionMarker";
		}

		private const string k_DraggingModeKey = "InspectorEditorDraggingMode";

		private InspectorWindow m_InspectorWindow;

		private bool m_TargetAbove;

		private int m_TargetIndex = -1;

		private int m_LastIndex = -1;

		private float m_LastMarkerY = 0f;

		public EditorDragging(InspectorWindow inspectorWindow)
		{
			this.m_InspectorWindow = inspectorWindow;
		}

		public void HandleDraggingToEditor(int editorIndex, Rect dragRect, Rect contentRect, ActiveEditorTracker tracker)
		{
			if (dragRect.height != 0f)
			{
				if (contentRect.height == 0f)
				{
					contentRect = dragRect;
				}
				float num = 8f;
				Rect targetRect = new Rect(contentRect.x, contentRect.yMax - (num - 2f), contentRect.width, num * 2f + 1f);
				float yMax = contentRect.yMax;
				this.m_LastIndex = editorIndex;
				this.m_LastMarkerY = yMax;
				this.HandleEditorDragging(editorIndex, targetRect, yMax, false, tracker);
			}
		}

		public void HandleDraggingToBottomArea(Rect bottomRect, ActiveEditorTracker tracker)
		{
			int lastIndex = this.m_LastIndex;
			if (lastIndex >= 0 && lastIndex < tracker.activeEditors.Length)
			{
				this.HandleEditorDragging(lastIndex, bottomRect, this.m_LastMarkerY, true, tracker);
			}
		}

		private void HandleEditorDragging(int editorIndex, Rect targetRect, float markerY, bool bottomTarget, ActiveEditorTracker tracker)
		{
			Event current = Event.current;
			EventType type = current.type;
			switch (type)
			{
			case EventType.Repaint:
				if (this.m_TargetIndex != -1 && targetRect.Contains(current.mousePosition))
				{
					Rect position = new Rect(targetRect.x, markerY, targetRect.width, 3f);
					if (!this.m_TargetAbove)
					{
						position.y += 2f;
					}
					EditorDragging.Styles.insertionMarker.Draw(position, false, false, false, false);
				}
				return;
			case EventType.Layout:
				IL_26:
				if (type != EventType.DragExited)
				{
					return;
				}
				this.m_TargetIndex = -1;
				return;
			case EventType.DragUpdated:
				if (targetRect.Contains(current.mousePosition))
				{
					EditorDragging.DraggingMode? draggingMode = DragAndDrop.GetGenericData("InspectorEditorDraggingMode") as EditorDragging.DraggingMode?;
					if (!draggingMode.HasValue)
					{
						UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
						if (objectReferences.Length == 0)
						{
							draggingMode = new EditorDragging.DraggingMode?(EditorDragging.DraggingMode.NotApplicable);
						}
						else if (objectReferences.All((UnityEngine.Object o) => o is Component && !(o is Transform)))
						{
							draggingMode = new EditorDragging.DraggingMode?(EditorDragging.DraggingMode.Component);
						}
						else if (objectReferences.All((UnityEngine.Object o) => o is MonoScript))
						{
							draggingMode = new EditorDragging.DraggingMode?(EditorDragging.DraggingMode.Script);
						}
						else
						{
							draggingMode = new EditorDragging.DraggingMode?(EditorDragging.DraggingMode.NotApplicable);
						}
						DragAndDrop.SetGenericData("InspectorEditorDraggingMode", draggingMode);
					}
					if (draggingMode.Value != EditorDragging.DraggingMode.NotApplicable)
					{
						Editor[] activeEditors = tracker.activeEditors;
						UnityEngine.Object[] objectReferences2 = DragAndDrop.objectReferences;
						if (bottomTarget)
						{
							this.m_TargetAbove = false;
							this.m_TargetIndex = this.m_LastIndex;
						}
						else
						{
							this.m_TargetAbove = (current.mousePosition.y < targetRect.y + targetRect.height / 2f);
							this.m_TargetIndex = editorIndex;
							if (this.m_TargetAbove)
							{
								this.m_TargetIndex++;
								while (this.m_TargetIndex < activeEditors.Length && this.m_InspectorWindow.ShouldCullEditor(activeEditors, this.m_TargetIndex))
								{
									this.m_TargetIndex++;
								}
								if (this.m_TargetIndex == activeEditors.Length)
								{
									this.m_TargetIndex = -1;
									return;
								}
							}
						}
						if (this.m_TargetAbove && this.m_InspectorWindow.EditorHasLargeHeader(this.m_TargetIndex, activeEditors))
						{
							this.m_TargetIndex--;
							while (this.m_TargetIndex >= 0 && this.m_InspectorWindow.ShouldCullEditor(activeEditors, this.m_TargetIndex))
							{
								this.m_TargetIndex--;
							}
							if (this.m_TargetIndex == -1)
							{
								return;
							}
							this.m_TargetAbove = false;
						}
						if (draggingMode.Value == EditorDragging.DraggingMode.Script)
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Link;
						}
						else
						{
							bool flag = false;
							if (activeEditors[this.m_TargetIndex].targets.All((UnityEngine.Object t) => t is Component))
							{
								Component[] targetComponents = activeEditors[this.m_TargetIndex].targets.Cast<Component>().ToArray<Component>();
								Component[] sourceComponents = DragAndDrop.objectReferences.Cast<Component>().ToArray<Component>();
								flag = this.MoveOrCopyComponents(sourceComponents, targetComponents, EditorUtility.EventHasDragCopyModifierPressed(current), true);
							}
							if (!flag)
							{
								DragAndDrop.visualMode = DragAndDropVisualMode.None;
								this.m_TargetIndex = -1;
								return;
							}
							DragAndDrop.visualMode = ((!EditorUtility.EventHasDragCopyModifierPressed(current)) ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Copy);
						}
						current.Use();
					}
				}
				else
				{
					this.m_TargetIndex = -1;
				}
				return;
			case EventType.DragPerform:
				if (this.m_TargetIndex != -1)
				{
					EditorDragging.DraggingMode? draggingMode2 = DragAndDrop.GetGenericData("InspectorEditorDraggingMode") as EditorDragging.DraggingMode?;
					if (!draggingMode2.HasValue || draggingMode2.Value == EditorDragging.DraggingMode.NotApplicable)
					{
						this.m_TargetIndex = -1;
					}
					else
					{
						Editor[] activeEditors2 = tracker.activeEditors;
						if (activeEditors2[this.m_TargetIndex].targets.All((UnityEngine.Object t) => t is Component))
						{
							Component[] array = activeEditors2[this.m_TargetIndex].targets.Cast<Component>().ToArray<Component>();
							if (draggingMode2.Value == EditorDragging.DraggingMode.Script)
							{
								IEnumerable<MonoScript> enumerable = DragAndDrop.objectReferences.Cast<MonoScript>();
								bool flag2 = true;
								Component[] array2 = array;
								for (int i = 0; i < array2.Length; i++)
								{
									Component targetComponent = array2[i];
									GameObject gameObject = targetComponent.gameObject;
									if (enumerable.Any((MonoScript s) => !ComponentUtility.WarnCanAddScriptComponent(targetComponent.gameObject, s)))
									{
										flag2 = false;
										break;
									}
								}
								if (flag2)
								{
									Undo.IncrementCurrentGroup();
									int currentGroup = Undo.GetCurrentGroup();
									int num = 0;
									Component[] array3 = new Component[array.Length * enumerable.Count<MonoScript>()];
									Component[] array4 = array;
									for (int j = 0; j < array4.Length; j++)
									{
										Component component = array4[j];
										GameObject gameObject2 = component.gameObject;
										foreach (MonoScript current2 in enumerable)
										{
											array3[num++] = Undo.AddComponent(gameObject2, current2.GetClass());
										}
									}
									if (!ComponentUtility.MoveComponentsRelativeToComponents(array3, array, this.m_TargetAbove))
									{
										Undo.RevertAllDownToGroup(currentGroup);
									}
								}
							}
							else
							{
								Component[] array5 = DragAndDrop.objectReferences.Cast<Component>().ToArray<Component>();
								if (array5.Length == 0 || array.Length == 0)
								{
									return;
								}
								this.MoveOrCopyComponents(array5, array, EditorUtility.EventHasDragCopyModifierPressed(current), false);
							}
							this.m_TargetIndex = -1;
							DragAndDrop.AcceptDrag();
							current.Use();
							GUIUtility.ExitGUI();
						}
					}
				}
				return;
			}
			goto IL_26;
		}

		private bool MoveOrCopyComponents(Component[] sourceComponents, Component[] targetComponents, bool copy, bool validateOnly)
		{
			bool result;
			if (copy)
			{
				result = false;
			}
			else if (sourceComponents.Length == 1 && targetComponents.Length == 1)
			{
				result = (!(sourceComponents[0].gameObject != targetComponents[0].gameObject) && ComponentUtility.MoveComponentRelativeToComponent(sourceComponents[0], targetComponents[0], this.m_TargetAbove, validateOnly));
			}
			else
			{
				result = ComponentUtility.MoveComponentsRelativeToComponents(sourceComponents, targetComponents, this.m_TargetAbove, validateOnly);
			}
			return result;
		}
	}
}
