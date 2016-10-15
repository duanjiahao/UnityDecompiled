using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationWindowCurve : IComparable<AnimationWindowCurve>
	{
		public const float timeEpsilon = 1E-05f;

		public List<AnimationWindowKeyframe> m_Keyframes;

		public Type m_ValueType;

		private EditorCurveBinding m_Binding;

		private AnimationClip m_Clip;

		private ISelectionBinding m_SelectionBinding;

		public EditorCurveBinding binding
		{
			get
			{
				return this.m_Binding;
			}
		}

		public bool isPPtrCurve
		{
			get
			{
				return this.m_Binding.isPPtrCurve;
			}
		}

		public string propertyName
		{
			get
			{
				return this.m_Binding.propertyName;
			}
		}

		public string path
		{
			get
			{
				return this.m_Binding.path;
			}
		}

		public Type type
		{
			get
			{
				return this.m_Binding.type;
			}
		}

		public int length
		{
			get
			{
				return this.m_Keyframes.Count;
			}
		}

		public int depth
		{
			get
			{
				return (this.path.Length <= 0) ? 0 : this.path.Split(new char[]
				{
					'/'
				}).Length;
			}
		}

		public AnimationClip clip
		{
			get
			{
				return this.m_Clip;
			}
		}

		public float timeOffset
		{
			get
			{
				return (this.m_SelectionBinding == null) ? 0f : this.m_SelectionBinding.timeOffset;
			}
		}

		public bool clipIsEditable
		{
			get
			{
				return this.m_SelectionBinding == null || this.m_SelectionBinding.clipIsEditable;
			}
		}

		public bool animationIsEditable
		{
			get
			{
				return this.m_SelectionBinding == null || this.m_SelectionBinding.animationIsEditable;
			}
		}

		public int selectionID
		{
			get
			{
				return (this.m_SelectionBinding == null) ? 0 : this.m_SelectionBinding.id;
			}
		}

		public ISelectionBinding selectionBindingInterface
		{
			get
			{
				return this.m_SelectionBinding;
			}
			set
			{
				this.m_SelectionBinding = value;
			}
		}

		public AnimationWindowCurve(AnimationClip clip, EditorCurveBinding binding, Type valueType)
		{
			binding = RotationCurveInterpolation.RemapAnimationBindingForRotationCurves(binding, clip);
			this.m_Binding = binding;
			this.m_ValueType = valueType;
			this.m_Clip = clip;
			this.LoadKeyframes(clip);
		}

		public void LoadKeyframes(AnimationClip clip)
		{
			this.m_Keyframes = new List<AnimationWindowKeyframe>();
			if (!this.m_Binding.isPPtrCurve)
			{
				AnimationCurve editorCurve = AnimationUtility.GetEditorCurve(clip, this.binding);
				int num = 0;
				while (editorCurve != null && num < editorCurve.length)
				{
					this.m_Keyframes.Add(new AnimationWindowKeyframe(this, editorCurve[num]));
					num++;
				}
			}
			else
			{
				ObjectReferenceKeyframe[] objectReferenceCurve = AnimationUtility.GetObjectReferenceCurve(clip, this.binding);
				int num2 = 0;
				while (objectReferenceCurve != null && num2 < objectReferenceCurve.Length)
				{
					this.m_Keyframes.Add(new AnimationWindowKeyframe(this, objectReferenceCurve[num2]));
					num2++;
				}
			}
		}

		public override int GetHashCode()
		{
			return this.selectionID * 27 ^ this.binding.GetHashCode();
		}

		public int GetCurveID()
		{
			int num = (!(this.clip == null)) ? this.clip.GetInstanceID() : 0;
			return this.selectionID * 729 ^ num * 27 ^ this.binding.GetHashCode();
		}

		public int CompareTo(AnimationWindowCurve obj)
		{
			bool flag = this.path.Equals(obj.path);
			if (!flag && this.depth != obj.depth)
			{
				return (this.depth >= obj.depth) ? 1 : -1;
			}
			bool flag2 = this.type == typeof(Transform) && obj.type == typeof(Transform) && flag;
			bool flag3 = (this.type == typeof(Transform) || obj.type == typeof(Transform)) && flag;
			if (flag2)
			{
				string nicePropertyGroupDisplayName = AnimationWindowUtility.GetNicePropertyGroupDisplayName(typeof(Transform), AnimationWindowUtility.GetPropertyGroupName(this.propertyName));
				string nicePropertyGroupDisplayName2 = AnimationWindowUtility.GetNicePropertyGroupDisplayName(typeof(Transform), AnimationWindowUtility.GetPropertyGroupName(obj.propertyName));
				if (nicePropertyGroupDisplayName.Contains("Position") && nicePropertyGroupDisplayName2.Contains("Rotation"))
				{
					return -1;
				}
				if (nicePropertyGroupDisplayName.Contains("Rotation") && nicePropertyGroupDisplayName2.Contains("Position"))
				{
					return 1;
				}
			}
			else if (flag3)
			{
				if (this.type == typeof(Transform))
				{
					return -1;
				}
				return 1;
			}
			else if (this.path == obj.path && obj.type == this.type)
			{
				int componentIndex = AnimationWindowUtility.GetComponentIndex(obj.propertyName);
				int componentIndex2 = AnimationWindowUtility.GetComponentIndex(this.propertyName);
				if (componentIndex != -1 && componentIndex2 != -1 && this.propertyName.Substring(0, this.propertyName.Length - 2) == obj.propertyName.Substring(0, obj.propertyName.Length - 2))
				{
					return componentIndex2 - componentIndex;
				}
			}
			return (this.path + this.type + this.propertyName).CompareTo(obj.path + obj.type + obj.propertyName);
		}

		public AnimationCurve ToAnimationCurve()
		{
			int count = this.m_Keyframes.Count;
			AnimationCurve animationCurve = new AnimationCurve();
			List<Keyframe> list = new List<Keyframe>();
			float num = -3.40282347E+38f;
			for (int i = 0; i < count; i++)
			{
				if (Mathf.Abs(this.m_Keyframes[i].time - num) > 1E-05f)
				{
					list.Add(new Keyframe(this.m_Keyframes[i].time, (float)this.m_Keyframes[i].value, this.m_Keyframes[i].m_InTangent, this.m_Keyframes[i].m_OutTangent)
					{
						tangentMode = this.m_Keyframes[i].m_TangentMode
					});
					num = this.m_Keyframes[i].time;
				}
			}
			animationCurve.keys = list.ToArray();
			return animationCurve;
		}

		public ObjectReferenceKeyframe[] ToObjectCurve()
		{
			int count = this.m_Keyframes.Count;
			List<ObjectReferenceKeyframe> list = new List<ObjectReferenceKeyframe>();
			float num = -3.40282347E+38f;
			for (int i = 0; i < count; i++)
			{
				if (Mathf.Abs(this.m_Keyframes[i].time - num) > 1E-05f)
				{
					ObjectReferenceKeyframe item = default(ObjectReferenceKeyframe);
					item.time = this.m_Keyframes[i].time;
					item.value = (UnityEngine.Object)this.m_Keyframes[i].value;
					num = item.time;
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public AnimationWindowKeyframe FindKeyAtTime(AnimationKeyTime keyTime)
		{
			int keyframeIndex = this.GetKeyframeIndex(keyTime);
			if (keyframeIndex == -1)
			{
				return null;
			}
			return this.m_Keyframes[keyframeIndex];
		}

		public void AddKeyframe(AnimationWindowKeyframe key, AnimationKeyTime keyTime)
		{
			this.RemoveKeyframe(keyTime);
			this.m_Keyframes.Add(key);
			this.m_Keyframes.Sort((AnimationWindowKeyframe a, AnimationWindowKeyframe b) => a.time.CompareTo(b.time));
		}

		public void RemoveKeyframe(AnimationKeyTime time)
		{
			for (int i = this.m_Keyframes.Count - 1; i >= 0; i--)
			{
				if (time.ContainsTime(this.m_Keyframes[i].time))
				{
					this.m_Keyframes.RemoveAt(i);
				}
			}
		}

		public bool HasKeyframe(AnimationKeyTime time)
		{
			return this.GetKeyframeIndex(time) != -1;
		}

		public int GetKeyframeIndex(AnimationKeyTime time)
		{
			for (int i = 0; i < this.m_Keyframes.Count; i++)
			{
				if (time.ContainsTime(this.m_Keyframes[i].time))
				{
					return i;
				}
			}
			return -1;
		}

		public void RemoveKeysAtRange(float startTime, float endTime)
		{
			for (int i = this.m_Keyframes.Count - 1; i >= 0; i--)
			{
				if (Mathf.Approximately(endTime, this.m_Keyframes[i].time) || (this.m_Keyframes[i].time > startTime && this.m_Keyframes[i].time < endTime))
				{
					this.m_Keyframes.RemoveAt(i);
				}
			}
		}
	}
}
