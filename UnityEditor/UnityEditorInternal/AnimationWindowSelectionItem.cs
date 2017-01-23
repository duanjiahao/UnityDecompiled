using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationWindowSelectionItem : ScriptableObject, IEquatable<AnimationWindowSelectionItem>, ISelectionBinding
	{
		[SerializeField]
		private float m_TimeOffset;

		[SerializeField]
		private int m_Id;

		[SerializeField]
		private GameObject m_GameObject;

		[SerializeField]
		private ScriptableObject m_ScriptableObject;

		[SerializeField]
		private AnimationClip m_AnimationClip;

		private List<AnimationWindowCurve> m_CurvesCache = null;

		public virtual float timeOffset
		{
			get
			{
				return this.m_TimeOffset;
			}
			set
			{
				this.m_TimeOffset = value;
			}
		}

		public virtual int id
		{
			get
			{
				return this.m_Id;
			}
			set
			{
				this.m_Id = value;
			}
		}

		public virtual GameObject gameObject
		{
			get
			{
				return this.m_GameObject;
			}
			set
			{
				this.m_GameObject = value;
			}
		}

		public virtual ScriptableObject scriptableObject
		{
			get
			{
				return this.m_ScriptableObject;
			}
			set
			{
				this.m_ScriptableObject = value;
			}
		}

		public virtual AnimationClip animationClip
		{
			get
			{
				return this.m_AnimationClip;
			}
			set
			{
				this.m_AnimationClip = value;
			}
		}

		public virtual GameObject rootGameObject
		{
			get
			{
				Component animationPlayer = this.animationPlayer;
				GameObject result;
				if (animationPlayer != null)
				{
					result = animationPlayer.gameObject;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public virtual Component animationPlayer
		{
			get
			{
				Component result;
				if (this.gameObject != null)
				{
					result = AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(this.gameObject.transform);
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public virtual bool animationIsEditable
		{
			get
			{
				return (!this.animationClip || (this.animationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None) && !this.objectIsPrefab;
			}
		}

		public virtual bool clipIsEditable
		{
			get
			{
				return this.animationClip && (this.animationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None && AssetDatabase.IsOpenForEdit(this.animationClip);
			}
		}

		public virtual bool objectIsPrefab
		{
			get
			{
				return this.gameObject && (EditorUtility.IsPersistent(this.gameObject) || (this.gameObject.hideFlags & HideFlags.NotEditable) != HideFlags.None);
			}
		}

		public virtual bool canRecord
		{
			get
			{
				return this.rootGameObject != null;
			}
		}

		public virtual bool canChangeAnimationClip
		{
			get
			{
				return this.rootGameObject != null;
			}
		}

		public virtual bool canAddCurves
		{
			get
			{
				bool result;
				if (this.gameObject != null)
				{
					result = (!this.objectIsPrefab && this.clipIsEditable);
				}
				else
				{
					result = (this.scriptableObject != null);
				}
				return result;
			}
		}

		public virtual bool canSyncSceneSelection
		{
			get
			{
				return true;
			}
		}

		public List<AnimationWindowCurve> curves
		{
			get
			{
				if (this.m_CurvesCache == null)
				{
					this.m_CurvesCache = new List<AnimationWindowCurve>();
					if (this.animationClip != null)
					{
						EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(this.animationClip);
						EditorCurveBinding[] objectReferenceCurveBindings = AnimationUtility.GetObjectReferenceCurveBindings(this.animationClip);
						List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
						EditorCurveBinding[] array = curveBindings;
						for (int i = 0; i < array.Length; i++)
						{
							EditorCurveBinding editorCurveBinding = array[i];
							if (AnimationWindowUtility.ShouldShowAnimationWindowCurve(editorCurveBinding))
							{
								AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(this.animationClip, editorCurveBinding, this.GetEditorCurveValueType(editorCurveBinding));
								animationWindowCurve.selectionBinding = this;
								this.m_CurvesCache.Add(animationWindowCurve);
								if (AnimationWindowUtility.IsTransformType(editorCurveBinding.type))
								{
									list.Add(animationWindowCurve);
								}
							}
						}
						EditorCurveBinding[] array2 = objectReferenceCurveBindings;
						for (int j = 0; j < array2.Length; j++)
						{
							EditorCurveBinding editorCurveBinding2 = array2[j];
							AnimationWindowCurve animationWindowCurve2 = new AnimationWindowCurve(this.animationClip, editorCurveBinding2, this.GetEditorCurveValueType(editorCurveBinding2));
							animationWindowCurve2.selectionBinding = this;
							this.m_CurvesCache.Add(animationWindowCurve2);
						}
						list.Sort();
						if (list.Count > 0)
						{
							this.FillInMissingTransformCurves(list, ref this.m_CurvesCache);
						}
					}
					this.m_CurvesCache.Sort();
				}
				return this.m_CurvesCache;
			}
		}

		private void FillInMissingTransformCurves(List<AnimationWindowCurve> transformCurves, ref List<AnimationWindowCurve> curvesCache)
		{
			EditorCurveBinding lastBinding = transformCurves[0].binding;
			EditorCurveBinding?[] array = new EditorCurveBinding?[3];
			foreach (AnimationWindowCurve current in transformCurves)
			{
				EditorCurveBinding binding = current.binding;
				if (binding.path != lastBinding.path || AnimationWindowUtility.GetPropertyGroupName(binding.propertyName) != AnimationWindowUtility.GetPropertyGroupName(lastBinding.propertyName))
				{
					string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(lastBinding.propertyName);
					this.FillPropertyGroup(ref array, lastBinding, propertyGroupName, ref curvesCache);
					lastBinding = binding;
					array = new EditorCurveBinding?[3];
				}
				this.AssignBindingToRightSlot(binding, ref array);
			}
			this.FillPropertyGroup(ref array, lastBinding, AnimationWindowUtility.GetPropertyGroupName(lastBinding.propertyName), ref curvesCache);
		}

		private void FillPropertyGroup(ref EditorCurveBinding?[] propertyGroup, EditorCurveBinding lastBinding, string propertyGroupName, ref List<AnimationWindowCurve> curvesCache)
		{
			EditorCurveBinding editorCurveBinding = lastBinding;
			editorCurveBinding.isPhantom = true;
			if (!propertyGroup[0].HasValue)
			{
				editorCurveBinding.propertyName = propertyGroupName + ".x";
				AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(this.animationClip, editorCurveBinding, this.GetEditorCurveValueType(editorCurveBinding));
				animationWindowCurve.selectionBinding = this;
				curvesCache.Add(animationWindowCurve);
			}
			if (!propertyGroup[1].HasValue)
			{
				editorCurveBinding.propertyName = propertyGroupName + ".y";
				AnimationWindowCurve animationWindowCurve2 = new AnimationWindowCurve(this.animationClip, editorCurveBinding, this.GetEditorCurveValueType(editorCurveBinding));
				animationWindowCurve2.selectionBinding = this;
				curvesCache.Add(animationWindowCurve2);
			}
			if (!propertyGroup[2].HasValue)
			{
				editorCurveBinding.propertyName = propertyGroupName + ".z";
				AnimationWindowCurve animationWindowCurve3 = new AnimationWindowCurve(this.animationClip, editorCurveBinding, this.GetEditorCurveValueType(editorCurveBinding));
				animationWindowCurve3.selectionBinding = this;
				curvesCache.Add(animationWindowCurve3);
			}
		}

		private void AssignBindingToRightSlot(EditorCurveBinding transformBinding, ref EditorCurveBinding?[] propertyGroup)
		{
			if (transformBinding.propertyName.EndsWith(".x"))
			{
				propertyGroup[0] = new EditorCurveBinding?(transformBinding);
			}
			else if (transformBinding.propertyName.EndsWith(".y"))
			{
				propertyGroup[1] = new EditorCurveBinding?(transformBinding);
			}
			else if (transformBinding.propertyName.EndsWith(".z"))
			{
				propertyGroup[2] = new EditorCurveBinding?(transformBinding);
			}
		}

		public static AnimationWindowSelectionItem Create()
		{
			AnimationWindowSelectionItem animationWindowSelectionItem = ScriptableObject.CreateInstance(typeof(AnimationWindowSelectionItem)) as AnimationWindowSelectionItem;
			animationWindowSelectionItem.hideFlags = HideFlags.HideAndDontSave;
			return animationWindowSelectionItem;
		}

		public int GetRefreshHash()
		{
			return this.id * 19603 ^ ((!(this.animationClip != null)) ? 0 : (729 * this.animationClip.GetHashCode())) ^ ((!(this.rootGameObject != null)) ? 0 : (27 * this.rootGameObject.GetHashCode())) ^ ((!(this.scriptableObject != null)) ? 0 : this.scriptableObject.GetHashCode());
		}

		public void ClearCache()
		{
			this.m_CurvesCache = null;
		}

		public virtual void Synchronize()
		{
		}

		public bool Equals(AnimationWindowSelectionItem other)
		{
			return this.id == other.id && this.animationClip == other.animationClip && this.gameObject == other.gameObject && this.scriptableObject == other.scriptableObject;
		}

		public Type GetEditorCurveValueType(EditorCurveBinding curveBinding)
		{
			Type result;
			if (this.rootGameObject != null)
			{
				result = AnimationUtility.GetEditorCurveValueType(this.rootGameObject, curveBinding);
			}
			else if (this.scriptableObject != null)
			{
				result = AnimationUtility.GetScriptableObjectEditorCurveValueType(this.scriptableObject, curveBinding);
			}
			else if (curveBinding.isPPtrCurve)
			{
				result = null;
			}
			else
			{
				result = typeof(float);
			}
			return result;
		}
	}
}
