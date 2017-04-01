using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Animation", useTypeNameAsIconName = true)]
	internal class AnimationWindow : EditorWindow
	{
		private static List<AnimationWindow> s_AnimationWindows = new List<AnimationWindow>();

		[SerializeField]
		private AnimEditor m_AnimEditor;

		[SerializeField]
		private bool m_Locked = false;

		private GUIStyle m_LockButtonStyle;

		internal AnimationWindowState state
		{
			get
			{
				AnimationWindowState result;
				if (this.m_AnimEditor != null)
				{
					result = this.m_AnimEditor.state;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public static List<AnimationWindow> GetAllAnimationWindows()
		{
			return AnimationWindow.s_AnimationWindows;
		}

		public void ForceRefresh()
		{
			if (this.m_AnimEditor != null)
			{
				this.m_AnimEditor.state.ForceRefresh();
			}
		}

		public void OnEnable()
		{
			if (this.m_AnimEditor == null)
			{
				this.m_AnimEditor = (ScriptableObject.CreateInstance(typeof(AnimEditor)) as AnimEditor);
				this.m_AnimEditor.hideFlags = HideFlags.HideAndDontSave;
			}
			AnimationWindow.s_AnimationWindows.Add(this);
			base.titleContent = base.GetLocalizedTitleContent();
			this.OnSelectionChange();
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		public void OnDisable()
		{
			AnimationWindow.s_AnimationWindows.Remove(this);
			this.m_AnimEditor.OnDisable();
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		public void OnDestroy()
		{
			UnityEngine.Object.DestroyImmediate(this.m_AnimEditor);
		}

		public void Update()
		{
			this.m_AnimEditor.Update();
		}

		public void OnGUI()
		{
			Profiler.BeginSample("AnimationWindow.OnGUI");
			this.m_AnimEditor.OnAnimEditorGUI(this, base.position);
			Profiler.EndSample();
		}

		public void OnSelectionChange()
		{
			if (!(this.m_AnimEditor == null))
			{
				GameObject activeGameObject = Selection.activeGameObject;
				if (activeGameObject != null)
				{
					this.EditGameObject(activeGameObject);
				}
				else
				{
					AnimationClip animationClip = Selection.activeObject as AnimationClip;
					if (animationClip != null)
					{
						this.EditAnimationClip(animationClip);
					}
				}
			}
		}

		public void OnFocus()
		{
			this.OnSelectionChange();
		}

		public void OnControllerChange()
		{
			this.OnSelectionChange();
		}

		public void OnLostFocus()
		{
			if (this.m_AnimEditor != null)
			{
				this.m_AnimEditor.OnLostFocus();
			}
		}

		public bool EditGameObject(GameObject gameObject)
		{
			return !this.state.linkedWithSequencer && this.EditGameObjectInternal(gameObject, null);
		}

		public bool EditAnimationClip(AnimationClip animationClip)
		{
			return !this.state.linkedWithSequencer && this.EditAnimationClipInternal(animationClip, null, null);
		}

		public bool EditSequencerClip(AnimationClip animationClip, UnityEngine.Object sourceObject, IAnimationWindowControl controlInterface)
		{
			bool result;
			if (this.EditAnimationClipInternal(animationClip, sourceObject, controlInterface))
			{
				this.state.linkedWithSequencer = true;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void UnlinkSequencer()
		{
			if (this.state.linkedWithSequencer)
			{
				this.state.linkedWithSequencer = false;
				this.EditAnimationClip(null);
				this.OnSelectionChange();
			}
		}

		private bool EditGameObjectInternal(GameObject gameObject, IAnimationWindowControl controlInterface)
		{
			bool result;
			if (EditorUtility.IsPersistent(gameObject))
			{
				result = false;
			}
			else if ((gameObject.hideFlags & HideFlags.NotEditable) != HideFlags.None)
			{
				result = false;
			}
			else
			{
				GameObjectSelectionItem gameObjectSelectionItem = GameObjectSelectionItem.Create(gameObject);
				if (this.ShouldUpdateGameObjectSelection(gameObjectSelectionItem))
				{
					this.m_AnimEditor.selectedItem = gameObjectSelectionItem;
					this.m_AnimEditor.overrideControlInterface = controlInterface;
					result = true;
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(gameObjectSelectionItem);
					result = false;
				}
			}
			return result;
		}

		private bool EditAnimationClipInternal(AnimationClip animationClip, UnityEngine.Object sourceObject, IAnimationWindowControl controlInterface)
		{
			AnimationClipSelectionItem animationClipSelectionItem = AnimationClipSelectionItem.Create(animationClip, sourceObject);
			bool result;
			if (this.ShouldUpdateSelection(animationClipSelectionItem))
			{
				this.m_AnimEditor.selectedItem = animationClipSelectionItem;
				this.m_AnimEditor.overrideControlInterface = controlInterface;
				result = true;
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(animationClipSelectionItem);
				result = false;
			}
			return result;
		}

		protected virtual void ShowButton(Rect r)
		{
			if (this.m_LockButtonStyle == null)
			{
				this.m_LockButtonStyle = "IN LockButton";
			}
			if (this.m_AnimEditor.stateDisabled)
			{
				this.m_Locked = false;
			}
			EditorGUI.BeginChangeCheck();
			using (new EditorGUI.DisabledScope(this.m_AnimEditor.stateDisabled))
			{
				this.m_Locked = GUI.Toggle(r, this.m_Locked, GUIContent.none, this.m_LockButtonStyle);
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.OnSelectionChange();
			}
		}

		private bool ShouldUpdateGameObjectSelection(GameObjectSelectionItem selectedItem)
		{
			bool result;
			if (this.m_Locked)
			{
				result = false;
			}
			else if (selectedItem.rootGameObject == null)
			{
				result = true;
			}
			else
			{
				AnimationWindowSelectionItem currentlySelectedItem = this.m_AnimEditor.selectedItem;
				if (currentlySelectedItem != null)
				{
					if (selectedItem.rootGameObject != currentlySelectedItem.rootGameObject)
					{
						result = true;
					}
					else if (currentlySelectedItem.animationClip == null)
					{
						result = true;
					}
					else
					{
						if (currentlySelectedItem.rootGameObject != null)
						{
							AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(currentlySelectedItem.rootGameObject);
							if (!Array.Exists<AnimationClip>(animationClips, (AnimationClip x) => x == currentlySelectedItem.animationClip))
							{
								result = true;
								return result;
							}
						}
						result = false;
					}
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private bool ShouldUpdateSelection(AnimationWindowSelectionItem selectedItem)
		{
			bool result;
			if (this.m_Locked)
			{
				result = false;
			}
			else
			{
				AnimationWindowSelectionItem selectedItem2 = this.m_AnimEditor.selectedItem;
				result = (!(selectedItem2 != null) || selectedItem.GetRefreshHash() != selectedItem2.GetRefreshHash());
			}
			return result;
		}

		private void UndoRedoPerformed()
		{
			base.Repaint();
		}
	}
}
