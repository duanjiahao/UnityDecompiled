using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Animation", useTypeNameAsIconName = true)]
	internal class AnimationWindow : EditorWindow
	{
		private static List<AnimationWindow> s_AnimationWindows = new List<AnimationWindow>();

		[SerializeField]
		private AnimEditor m_AnimEditor;

		[SerializeField]
		private AnimationWindowPolicy m_Policy;

		private GUIStyle m_LockButtonStyle;

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
		}

		public void OnDisable()
		{
			AnimationWindow.s_AnimationWindows.Remove(this);
			this.m_AnimEditor.OnDisable();
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
			this.m_AnimEditor.OnAnimEditorGUI(this, base.position);
		}

		public void OnSelectionChange()
		{
			if (this.m_AnimEditor == null)
			{
				return;
			}
			GameObject activeGameObject = Selection.activeGameObject;
			if (activeGameObject == null)
			{
				return;
			}
			if (EditorUtility.IsPersistent(activeGameObject))
			{
				return;
			}
			if ((activeGameObject.hideFlags & HideFlags.NotEditable) != HideFlags.None)
			{
				return;
			}
			this.SynchronizePolicy();
			AnimationWindowSelectionItem selectedItem = this.BuildSelection(activeGameObject);
			if (!this.m_AnimEditor.locked && this.ShouldUpdateSelection(selectedItem))
			{
				this.m_AnimEditor.state.recording = false;
				this.m_AnimEditor.selectedItem = selectedItem;
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

		protected virtual void ShowButton(Rect r)
		{
			if (this.m_LockButtonStyle == null)
			{
				this.m_LockButtonStyle = "IN LockButton";
			}
			EditorGUI.BeginChangeCheck();
			using (new EditorGUI.DisabledScope(this.m_AnimEditor.stateDisabled))
			{
				this.m_AnimEditor.locked = GUI.Toggle(r, this.m_AnimEditor.locked, GUIContent.none, this.m_LockButtonStyle);
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.OnSelectionChange();
			}
		}

		private void SynchronizePolicy()
		{
			if (this.m_AnimEditor == null)
			{
				return;
			}
			if (this.m_Policy == null)
			{
				this.m_Policy = new AnimationWindowPolicy();
				this.m_Policy.allowRecording = true;
			}
			if (this.m_Policy.unitialized)
			{
				this.m_Policy.SynchronizeFrameRate = delegate(ref float frameRate, ref bool timeInFrames)
				{
					AnimationWindowSelectionItem selectedItem = this.m_AnimEditor.selectedItem;
					if (selectedItem != null && selectedItem.animationClip != null)
					{
						frameRate = selectedItem.animationClip.frameRate;
					}
					else
					{
						frameRate = 60f;
					}
					timeInFrames = false;
					return true;
				};
				this.m_Policy.unitialized = false;
			}
			this.m_AnimEditor.policy = this.m_Policy;
		}

		private AnimationWindowSelectionItem BuildSelection(GameObject gameObject)
		{
			StandaloneSelectionItem selectedItem = StandaloneSelectionItem.Create();
			selectedItem.gameObject = gameObject;
			selectedItem.animationClip = null;
			selectedItem.timeOffset = 0f;
			selectedItem.id = 0;
			if (selectedItem.rootGameObject != null)
			{
				AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(selectedItem.rootGameObject);
				if (selectedItem.animationClip == null && selectedItem.gameObject != null)
				{
					selectedItem.animationClip = ((animationClips.Length <= 0) ? null : animationClips[0]);
				}
				else if (!Array.Exists<AnimationClip>(animationClips, (AnimationClip x) => x == selectedItem.animationClip))
				{
					selectedItem.animationClip = ((animationClips.Length <= 0) ? null : animationClips[0]);
				}
			}
			return selectedItem;
		}

		private bool ShouldUpdateSelection(AnimationWindowSelectionItem selectedItem)
		{
			if (selectedItem.rootGameObject == null)
			{
				return true;
			}
			AnimationWindowSelectionItem currentlySelectedItem = this.m_AnimEditor.selectedItem;
			if (!(currentlySelectedItem != null))
			{
				return true;
			}
			if (selectedItem.rootGameObject != currentlySelectedItem.rootGameObject)
			{
				return true;
			}
			if (currentlySelectedItem.animationClip == null)
			{
				return true;
			}
			if (currentlySelectedItem.rootGameObject != null)
			{
				AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(currentlySelectedItem.rootGameObject);
				if (!Array.Exists<AnimationClip>(animationClips, (AnimationClip x) => x == currentlySelectedItem.animationClip))
				{
					return true;
				}
			}
			return false;
		}
	}
}
