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
			this.SynchronizePolicy();
			this.m_AnimEditor.OnAnimEditorGUI(this, base.position);
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
						this.EditAnimationClip(animationClip, null);
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

		public void EditGameObject(GameObject gameObject)
		{
			if (!EditorUtility.IsPersistent(gameObject))
			{
				if ((gameObject.hideFlags & HideFlags.NotEditable) == HideFlags.None)
				{
					GameObjectSelectionItem gameObjectSelectionItem = GameObjectSelectionItem.Create(gameObject);
					if (this.ShouldUpdateSelection(gameObjectSelectionItem))
					{
						this.m_AnimEditor.state.recording = false;
						this.m_AnimEditor.selectedItem = gameObjectSelectionItem;
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(gameObjectSelectionItem);
					}
				}
			}
		}

		public void EditAnimationClip(AnimationClip animationClip, UnityEngine.Object sourceObject)
		{
			AnimationClipSelectionItem animationClipSelectionItem = AnimationClipSelectionItem.Create(animationClip, sourceObject);
			if (this.ShouldUpdateSelection(animationClipSelectionItem))
			{
				this.m_AnimEditor.state.recording = false;
				this.m_AnimEditor.selectedItem = animationClipSelectionItem;
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(animationClipSelectionItem);
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
			if (!(this.m_AnimEditor == null))
			{
				if (this.m_Policy == null)
				{
					this.m_Policy = new AnimationWindowPolicy();
				}
				if (this.m_Policy.unitialized)
				{
					this.m_Policy.SynchronizeFrameRate = delegate(ref float frameRate)
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
						return true;
					};
					this.m_Policy.unitialized = false;
				}
				this.m_AnimEditor.policy = this.m_Policy;
			}
		}

		private bool ShouldUpdateSelection(GameObjectSelectionItem selectedItem)
		{
			bool result;
			if (this.m_AnimEditor.locked)
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

		private bool ShouldUpdateSelection(AnimationClipSelectionItem selectedItem)
		{
			bool result;
			if (this.m_AnimEditor.locked)
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
	}
}
