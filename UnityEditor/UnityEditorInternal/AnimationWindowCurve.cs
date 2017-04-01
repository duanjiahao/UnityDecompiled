using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationWindowCurve : IComparable<AnimationWindowCurve>
	{
		public const float timeEpsilon = 1E-05f;

		public List<AnimationWindowKeyframe> m_Keyframes;

		private EditorCurveBinding m_Binding;

		private int m_BindingHashCode;

		private AnimationClip m_Clip;

		private AnimationWindowSelectionItem m_SelectionBinding;

		private Type m_ValueType;

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

		public bool isPhantom
		{
			get
			{
				return this.m_Binding.isPhantom;
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

		public Type valueType
		{
			get
			{
				return this.m_ValueType;
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

		public GameObject rootGameObject
		{
			get
			{
				return (!(this.m_SelectionBinding != null)) ? null : this.m_SelectionBinding.rootGameObject;
			}
		}

		public ScriptableObject scriptableObject
		{
			get
			{
				return (!(this.m_SelectionBinding != null)) ? null : this.m_SelectionBinding.scriptableObject;
			}
		}

		public float timeOffset
		{
			get
			{
				return (!(this.m_SelectionBinding != null)) ? 0f : this.m_SelectionBinding.timeOffset;
			}
		}

		public bool clipIsEditable
		{
			get
			{
				return !(this.m_SelectionBinding != null) || this.m_SelectionBinding.clipIsEditable;
			}
		}

		public bool animationIsEditable
		{
			get
			{
				return !(this.m_SelectionBinding != null) || this.m_SelectionBinding.animationIsEditable;
			}
		}

		public int selectionID
		{
			get
			{
				return (!(this.m_SelectionBinding != null)) ? 0 : this.m_SelectionBinding.id;
			}
		}

		public AnimationWindowSelectionItem selectionBinding
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
			this.m_BindingHashCode = binding.GetHashCode();
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
			int num = (!(this.clip == null)) ? this.clip.GetInstanceID() : 0;
			return this.selectionID * 92821 ^ num * 19603 ^ this.GetBindingHashCode();
		}

		public int GetBindingHashCode()
		{
			return this.m_BindingHashCode;
		}

		public int CompareTo(AnimationWindowCurve obj)
		{
			bool flag = this.path.Equals(obj.path);
			bool flag2 = obj.type == this.type;
			int result;
			if (!flag && this.depth != obj.depth)
			{
				int num = Math.Min(this.path.Length, obj.path.Length);
				int startIndex = 0;
				int i;
				for (i = 0; i < num; i++)
				{
					if (this.path[i] != obj.path[i])
					{
						break;
					}
					if (this.path[i] == '/')
					{
						startIndex = i + 1;
					}
				}
				if (i == num)
				{
					startIndex = num;
				}
				string text = this.path.Substring(startIndex);
				string text2 = obj.path.Substring(startIndex);
				if (string.IsNullOrEmpty(text))
				{
					result = -1;
				}
				else if (string.IsNullOrEmpty(text2))
				{
					result = 1;
				}
				else
				{
					Regex regex = new Regex("^[^\\/]*\\/");
					Match match = regex.Match(text);
					string text3 = (!match.Success) ? text : match.Value.Substring(0, match.Value.Length - 1);
					Match match2 = regex.Match(text2);
					string strB = (!match2.Success) ? text2 : match2.Value.Substring(0, match2.Value.Length - 1);
					result = text3.CompareTo(strB);
				}
			}
			else
			{
				bool flag3 = this.type == typeof(Transform) && obj.type == typeof(Transform) && flag;
				bool flag4 = (this.type == typeof(Transform) || obj.type == typeof(Transform)) && flag;
				if (flag3)
				{
					string nicePropertyGroupDisplayName = AnimationWindowUtility.GetNicePropertyGroupDisplayName(typeof(Transform), AnimationWindowUtility.GetPropertyGroupName(this.propertyName));
					string nicePropertyGroupDisplayName2 = AnimationWindowUtility.GetNicePropertyGroupDisplayName(typeof(Transform), AnimationWindowUtility.GetPropertyGroupName(obj.propertyName));
					if (nicePropertyGroupDisplayName.Contains("Position") && nicePropertyGroupDisplayName2.Contains("Rotation"))
					{
						result = -1;
						return result;
					}
					if (nicePropertyGroupDisplayName.Contains("Rotation") && nicePropertyGroupDisplayName2.Contains("Position"))
					{
						result = 1;
						return result;
					}
				}
				else if (flag4)
				{
					if (this.type == typeof(Transform))
					{
						result = -1;
						return result;
					}
					result = 1;
					return result;
				}
				if (flag && flag2)
				{
					int componentIndex = AnimationWindowUtility.GetComponentIndex(obj.propertyName);
					int componentIndex2 = AnimationWindowUtility.GetComponentIndex(this.propertyName);
					if (componentIndex != -1 && componentIndex2 != -1 && this.propertyName.Substring(0, this.propertyName.Length - 2) == obj.propertyName.Substring(0, obj.propertyName.Length - 2))
					{
						result = componentIndex2 - componentIndex;
						return result;
					}
				}
				result = (this.path + this.type + this.propertyName).CompareTo(obj.path + obj.type + obj.propertyName);
			}
			return result;
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
			AnimationWindowKeyframe result;
			if (keyframeIndex == -1)
			{
				result = null;
			}
			else
			{
				result = this.m_Keyframes[keyframeIndex];
			}
			return result;
		}

		public object Evaluate(float time)
		{
			object result;
			if (this.m_Keyframes.Count == 0)
			{
				result = null;
			}
			else
			{
				AnimationWindowKeyframe animationWindowKeyframe = this.m_Keyframes[0];
				if (time <= animationWindowKeyframe.time)
				{
					result = animationWindowKeyframe.value;
				}
				else
				{
					AnimationWindowKeyframe animationWindowKeyframe2 = this.m_Keyframes[this.m_Keyframes.Count - 1];
					if (time >= animationWindowKeyframe2.time)
					{
						result = animationWindowKeyframe2.value;
					}
					else
					{
						AnimationWindowKeyframe animationWindowKeyframe3 = animationWindowKeyframe;
						int i = 1;
						while (i < this.m_Keyframes.Count)
						{
							AnimationWindowKeyframe animationWindowKeyframe4 = this.m_Keyframes[i];
							if (animationWindowKeyframe3.time < time && animationWindowKeyframe4.time >= time)
							{
								if (this.isPPtrCurve)
								{
									result = animationWindowKeyframe3.value;
									return result;
								}
								Keyframe keyframe = new Keyframe(animationWindowKeyframe3.time, (float)animationWindowKeyframe3.value, animationWindowKeyframe3.m_InTangent, animationWindowKeyframe3.m_OutTangent);
								Keyframe keyframe2 = new Keyframe(animationWindowKeyframe4.time, (float)animationWindowKeyframe4.value, animationWindowKeyframe4.m_InTangent, animationWindowKeyframe4.m_OutTangent);
								result = new AnimationCurve
								{
									keys = new Keyframe[]
									{
										keyframe,
										keyframe2
									}
								}.Evaluate(time);
								return result;
							}
							else
							{
								animationWindowKeyframe3 = animationWindowKeyframe4;
								i++;
							}
						}
						result = null;
					}
				}
			}
			return result;
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
			int result;
			for (int i = 0; i < this.m_Keyframes.Count; i++)
			{
				if (time.ContainsTime(this.m_Keyframes[i].time))
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
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
