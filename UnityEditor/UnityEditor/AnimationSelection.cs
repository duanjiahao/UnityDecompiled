using System;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
namespace UnityEditor
{
	[Serializable]
	internal class AnimationSelection
	{
		private AnimationWindow m_AnimationWindow;
		private int m_AnimatedObjectIndex;
		private GameObject[] m_AnimatedOptions;
		private AnimationClip m_Clip;
		private bool m_ClipCanceled;
		private FoldoutTree[] m_Trees = new FoldoutTree[0];
		private Hashtable m_AnimatedCurves;
		private Hashtable m_AnimatedPaths;
		private Hashtable m_LeftoverCurves;
		public AnimationWindow animationWindow
		{
			get
			{
				return this.m_AnimationWindow;
			}
		}
		public int animatedObjectIndex
		{
			get
			{
				return this.m_AnimatedObjectIndex;
			}
		}
		public GameObject animatedObject
		{
			get
			{
				return this.animatedOptions[this.m_AnimatedObjectIndex];
			}
		}
		public GameObject avatarRootObject
		{
			get
			{
				if (this.animatedObject)
				{
					Animator component = this.animatedObject.GetComponent<Animator>();
					if (component)
					{
						Transform avatarRoot = component.avatarRoot;
						if (avatarRoot)
						{
							return avatarRoot.gameObject;
						}
					}
				}
				return this.animatedObject;
			}
		}
		public GameObject[] animatedOptions
		{
			get
			{
				return this.m_AnimatedOptions;
			}
		}
		public bool hasAnimationComponent
		{
			get
			{
				return this.animatedObject && (this.animatedObject.GetComponent<Animation>() != null || this.animatedObject.GetComponent<Animator>() != null);
			}
		}
		public AnimationClip clip
		{
			get
			{
				return this.m_Clip;
			}
		}
		public int clipIndex
		{
			get
			{
				return this.GetIndexOfClip(this.m_Clip);
			}
		}
		public bool CanClipAndGameObjectBeEdited
		{
			get
			{
				return !(this.m_Clip == null) && (this.m_Clip.hideFlags & HideFlags.NotEditable) == HideFlags.None && this.GameObjectIsAnimatable;
			}
		}
		public bool GameObjectIsAnimatable
		{
			get
			{
				return !(this.animatedObject == null) && (this.animatedObject.hideFlags & HideFlags.NotEditable) == HideFlags.None && !EditorUtility.IsPersistent(this.animatedObject);
			}
		}
		public FoldoutTree[] trees
		{
			get
			{
				return this.m_Trees;
			}
		}
		public Hashtable animatedCurves
		{
			get
			{
				if (this.m_AnimatedCurves == null)
				{
					this.SetAnimationCurves();
				}
				return this.m_AnimatedCurves;
			}
		}
		public Hashtable animatedPaths
		{
			get
			{
				if (this.m_AnimatedPaths == null)
				{
					this.SetAnimationCurves();
				}
				return this.m_AnimatedPaths;
			}
		}
		public Hashtable leftoverCurves
		{
			get
			{
				return this.m_LeftoverCurves;
			}
		}
		public AnimationSelection(GameObject[] animatedOptions, SerializedStringTable chosenAnimated, SerializedStringTable chosenClip, AnimationWindow editor)
		{
			this.m_AnimationWindow = editor;
			this.m_AnimatedOptions = animatedOptions;
			string stringArrayHashCode = AnimationSelection.GetStringArrayHashCode(this.GetAnimatedObjectNames());
			if (!chosenAnimated.Contains(stringArrayHashCode))
			{
				chosenAnimated.Set(stringArrayHashCode, animatedOptions.Length - 1);
			}
			this.m_AnimatedObjectIndex = chosenAnimated.Get(stringArrayHashCode);
			this.RefreshChosenClip(chosenClip);
		}
		public GameObject ShownRoot()
		{
			if (this.trees.Length < 1)
			{
				return null;
			}
			return this.trees[0].rootGameObject;
		}
		private AnimationClip GetClipAtIndex(int index)
		{
			if (!this.hasAnimationComponent)
			{
				return null;
			}
			AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(this.animatedObject);
			if (index == -1)
			{
				return null;
			}
			if (index >= animationClips.Length)
			{
				return null;
			}
			return animationClips[index];
		}
		private int GetIndexOfClip(AnimationClip clip)
		{
			if (this.hasAnimationComponent)
			{
				AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(this.animatedObject);
				for (int i = 0; i < animationClips.Length; i++)
				{
					if (animationClips[i] == clip)
					{
						return i;
					}
				}
			}
			return -1;
		}
		private void RefreshChosenClip(SerializedStringTable chosenClip)
		{
			if (this.hasAnimationComponent)
			{
				string stringArrayHashCode = AnimationSelection.GetStringArrayHashCode(this.GetClipNames());
				if (!chosenClip.Contains(stringArrayHashCode))
				{
					this.m_Clip = null;
					AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(this.animatedObject);
					for (int i = 0; i < animationClips.Length; i++)
					{
						if (animationClips[i] != null)
						{
							this.m_Clip = animationClips[i];
							break;
						}
					}
				}
				else
				{
					this.m_Clip = this.GetClipAtIndex(chosenClip.Get(stringArrayHashCode));
				}
			}
		}
		private void SetAnimationCurves()
		{
			this.m_AnimatedCurves = new Hashtable();
			this.m_AnimatedPaths = new Hashtable();
			this.m_LeftoverCurves = new Hashtable();
			if (this.clip != null)
			{
				EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(this.clip);
				EditorCurveBinding[] array = curveBindings;
				for (int i = 0; i < array.Length; i++)
				{
					EditorCurveBinding editorCurveBinding = array[i];
					int curveID = CurveUtility.GetCurveID(this.clip, editorCurveBinding);
					this.m_AnimatedCurves[curveID] = true;
					if (!this.CheckIfPropertyExists(editorCurveBinding))
					{
						this.m_LeftoverCurves[editorCurveBinding] = editorCurveBinding;
					}
					else
					{
						this.m_AnimatedPaths[CurveUtility.GetPathAndTypeID(editorCurveBinding.path, editorCurveBinding.type)] = true;
						string text = editorCurveBinding.path;
						while (true)
						{
							int hashCode = text.GetHashCode();
							if (this.m_AnimatedPaths.Contains(hashCode))
							{
								break;
							}
							this.m_AnimatedPaths[hashCode] = true;
							if (text.Length == 0)
							{
								break;
							}
							int num = text.LastIndexOf('/');
							if (num > 0)
							{
								text = text.Substring(0, num);
							}
							else
							{
								text = string.Empty;
							}
						}
					}
				}
			}
		}
		private bool CheckIfPropertyExists(EditorCurveBinding data)
		{
			return AnimationUtility.GetEditorCurveValueType(this.animatedObject, data) != null;
		}
		public void Refresh()
		{
			this.SetAnimationCurves();
			FoldoutTree[] trees = this.trees;
			for (int i = 0; i < trees.Length; i++)
			{
				FoldoutTree foldoutTree = trees[i];
				foldoutTree.Refresh(this.GetAnimationHierarchyData(foldoutTree));
			}
		}
		public static void OnGUISelection(AnimationSelection animSel)
		{
			if (animSel == null)
			{
				bool enabled = GUI.enabled;
				GUI.enabled = false;
				GUILayout.Label(string.Empty, EditorStyles.toolbarPopup, new GUILayoutOption[0]);
				GUI.enabled = enabled;
				return;
			}
			animSel.m_ClipCanceled = false;
			bool flag = false;
			string[] clipMenuContent = animSel.GetClipMenuContent();
			int clipIndex = animSel.clipIndex;
			int num = EditorGUILayout.Popup(clipIndex, clipMenuContent, EditorStyles.toolbarPopup, new GUILayoutOption[0]);
			if (num != clipIndex)
			{
				AnimationClip animationClip = animSel.GetClipAtIndex(num);
				if (animationClip == null)
				{
					animationClip = animSel.CreateNewClip();
				}
				if (animationClip != null)
				{
					animSel.ChooseClip(animationClip);
					flag = true;
				}
			}
			if (flag)
			{
				GUI.changed = true;
				animSel.animationWindow.ReEnterAnimationMode();
				animSel.Refresh();
			}
		}
		public static string GetStringArrayHashCode(string[] array)
		{
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				string str = array[i];
				text = text + "|" + str;
			}
			return text;
		}
		public static string GetObjectListHashCode(GameObject[] animatedOptions)
		{
			string[] array = new string[animatedOptions.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = animatedOptions[i].name;
			}
			return AnimationSelection.GetStringArrayHashCode(array);
		}
		public string[] GetClipNames()
		{
			string[] array;
			if (this.hasAnimationComponent)
			{
				AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(this.animatedObject);
				array = new string[animationClips.Length];
				for (int i = 0; i < animationClips.Length; i++)
				{
					array[i] = CurveUtility.GetClipName(animationClips[i]);
				}
			}
			else
			{
				array = new string[0];
			}
			return array;
		}
		public string[] GetClipMenuContent()
		{
			string[] array;
			if (this.hasAnimationComponent)
			{
				string[] clipNames = this.GetClipNames();
				array = ((!this.animationWindow.state.IsEditable) ? new string[clipNames.Length] : new string[clipNames.Length + 2]);
				for (int i = 0; i < clipNames.Length; i++)
				{
					array[i] = clipNames[i];
				}
			}
			else
			{
				array = new string[1];
			}
			if (this.animationWindow.state.IsEditable)
			{
				array[array.Length - 1] = "[Create New Clip]";
			}
			return array;
		}
		public string[] GetAnimatedObjectNames()
		{
			string[] array = new string[this.animatedOptions.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.animatedOptions[i].name;
			}
			return array;
		}
		public AnimationHierarchyData GetAnimationHierarchyData(FoldoutTree tree)
		{
			return new AnimationHierarchyData
			{
				animationWindow = this.m_AnimationWindow,
				animationSelection = this,
				states = tree.states
			};
		}
		public void AddTree(FoldoutTree tree)
		{
			Array.Resize<FoldoutTree>(ref this.m_Trees, this.m_Trees.Length + 1);
			this.trees[this.trees.Length - 1] = tree;
		}
		public bool EnsureClipPresence()
		{
			if (this.clip == null)
			{
				if (this.m_ClipCanceled)
				{
					return false;
				}
				Vector2 s_EditorScreenPointOffset = GUIUtility.s_EditorScreenPointOffset;
				AnimationClip animationClip = this.CreateNewClip();
				GUIUtility.s_EditorScreenPointOffset = s_EditorScreenPointOffset;
				if (!(animationClip != null))
				{
					GUIUtility.keyboardControl = 0;
					GUIUtility.hotControl = 0;
					this.m_ClipCanceled = true;
					return false;
				}
				this.ChooseClip(animationClip);
				this.Refresh();
			}
			return true;
		}
		internal static AnimationClip AllocateAndSetupClip(bool useAnimator)
		{
			AnimationClip animationClip = new AnimationClip();
			if (useAnimator)
			{
				AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(animationClip);
				animationClipSettings.loopTime = true;
				AnimationUtility.SetAnimationClipSettingsNoDirty(animationClip, animationClipSettings);
			}
			return animationClip;
		}
		private AnimationClip CreateNewClip()
		{
			bool flag = this.animatedObject.GetComponent<Animator>() != null || this.animatedObject.GetComponent<Animation>() == null;
			string message = string.Format("Create a new animation for the game object '{0}':", this.animatedObject.name);
			string text = EditorUtility.SaveFilePanelInProject("Create New Animation", "New Animation", "anim", message, ProjectWindowUtil.GetActiveFolderPath());
			if (text == string.Empty)
			{
				return null;
			}
			AnimationClip animationClip = AnimationSelection.AllocateAndSetupClip(flag);
			AssetDatabase.CreateAsset(animationClip, text);
			this.m_AnimationWindow.EndAnimationMode();
			if (flag)
			{
				return AnimationSelection.AddClipToAnimatorComponent(this.animatedObject, animationClip);
			}
			return this.AddClipToAnimationComponent(animationClip);
		}
		public static AnimationClip AddClipToAnimatorComponent(GameObject animatedObject, AnimationClip newClip)
		{
			Animator animator = animatedObject.GetComponent<Animator>();
			if (animator == null)
			{
				animator = animatedObject.AddComponent<Animator>();
			}
			AnimatorController animatorController = AnimatorController.GetEffectiveAnimatorController(animator);
			if (!(animatorController == null))
			{
				animatorController.AddMotion(newClip);
				return newClip;
			}
			animatorController = AnimatorController.CreateAnimatorControllerForClip(newClip, animatedObject);
			AnimatorController.SetAnimatorController(animator, animatorController);
			if (animatorController != null)
			{
				return newClip;
			}
			return null;
		}
		private AnimationClip AddClipToAnimationComponent(AnimationClip newClip)
		{
			if (this.animatedObject.GetComponent<Animation>() == null)
			{
				Animation animation = this.animatedObject.AddComponent(typeof(Animation)) as Animation;
				animation.clip = newClip;
			}
			AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(this.animatedObject);
			Array.Resize<AnimationClip>(ref animationClips, animationClips.Length + 1);
			animationClips[animationClips.Length - 1] = newClip;
			AnimationUtility.SetAnimationClips(this.animatedObject.GetComponent<Animation>(), animationClips);
			return newClip;
		}
		private void ChooseClip(AnimationClip newClip)
		{
			this.ChooseClip(newClip, this.GetIndexOfClip(newClip));
		}
		private void ChooseClip(AnimationClip newClip, int newClipIndex)
		{
			this.m_Clip = newClip;
			this.m_AnimationWindow.chosenClip.Set(AnimationSelection.GetStringArrayHashCode(this.GetClipNames()), newClipIndex);
			this.m_AnimationWindow.state.m_ActiveAnimationClip = this.m_Clip;
		}
		public void DrawRightIcon(Texture image, Color color, float width)
		{
			Color color2 = GUI.color;
			GUI.color = color;
			Rect iconRect = this.m_AnimationWindow.GetIconRect(width);
			iconRect.width = (float)image.width;
			GUI.DrawTexture(iconRect, image, ScaleMode.ScaleToFit, true, 1f);
			GUI.color = color2;
		}
		public static string GetPath(Transform t)
		{
			return AnimationUtility.CalculateTransformPath(t, t.root);
		}
	}
}
