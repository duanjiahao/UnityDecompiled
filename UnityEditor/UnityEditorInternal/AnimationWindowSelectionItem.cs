using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationWindowSelectionItem : ScriptableObject, ISelectionBinding, IEquatable<AnimationWindowSelectionItem>
	{
		[SerializeField]
		private float m_TimeOffset;

		[SerializeField]
		private int m_Id;

		[SerializeField]
		private GameObject m_GameObject;

		[SerializeField]
		private AnimationClip m_AnimationClip;

		private List<AnimationWindowCurve> m_CurvesCache;

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
				if (animationPlayer != null)
				{
					return animationPlayer.gameObject;
				}
				return null;
			}
		}

		public virtual Component animationPlayer
		{
			get
			{
				if (this.gameObject != null)
				{
					return AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(this.gameObject.transform);
				}
				return null;
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
						EditorCurveBinding[] array = curveBindings;
						for (int i = 0; i < array.Length; i++)
						{
							EditorCurveBinding editorCurveBinding = array[i];
							if (AnimationWindowUtility.ShouldShowAnimationWindowCurve(editorCurveBinding))
							{
								AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(this.animationClip, editorCurveBinding, CurveBindingUtility.GetEditorCurveValueType(this.rootGameObject, editorCurveBinding));
								animationWindowCurve.selectionBindingInterface = this;
								this.m_CurvesCache.Add(animationWindowCurve);
							}
						}
						EditorCurveBinding[] array2 = objectReferenceCurveBindings;
						for (int j = 0; j < array2.Length; j++)
						{
							EditorCurveBinding editorCurveBinding2 = array2[j];
							AnimationWindowCurve animationWindowCurve2 = new AnimationWindowCurve(this.animationClip, editorCurveBinding2, CurveBindingUtility.GetEditorCurveValueType(this.rootGameObject, editorCurveBinding2));
							animationWindowCurve2.selectionBindingInterface = this;
							this.m_CurvesCache.Add(animationWindowCurve2);
						}
						this.m_CurvesCache.Sort();
					}
				}
				return this.m_CurvesCache;
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
			return this.id * 19603 ^ ((!(this.animationClip != null)) ? 0 : (729 * this.animationClip.GetHashCode())) ^ ((!(this.rootGameObject != null)) ? 0 : (27 * this.rootGameObject.GetHashCode()));
		}

		public void ClearCache()
		{
			this.m_CurvesCache = null;
		}

		public bool Equals(AnimationWindowSelectionItem other)
		{
			return this.id == other.id && this.animationClip == other.animationClip && this.gameObject == other.gameObject;
		}
	}
}
