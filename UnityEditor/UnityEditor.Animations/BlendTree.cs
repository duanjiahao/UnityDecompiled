using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.Animations
{
	public sealed class BlendTree : Motion
	{
		public extern string blendParameter
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string blendParameterY
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern BlendTreeType blendType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ChildMotion[] children
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useAutomaticThresholds
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float minThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float maxThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal extern int recursiveBlendParameterCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public BlendTree()
		{
			BlendTree.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create(BlendTree mono);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetDirectBlendTreeParameter(int index, string parameter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetDirectBlendTreeParameter(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetChildCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Motion GetChildMotion(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SortChildren();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetRecursiveBlendParameter(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetRecursiveBlendParameterMin(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetRecursiveBlendParameterMax(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetInputBlendValue(string blendValueName, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetInputBlendValue(string blendValueName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern AnimationClip[] GetAnimationClipsFlattened();

		public void AddChild(Motion motion)
		{
			this.AddChild(motion, Vector2.zero, 0f);
		}

		public void AddChild(Motion motion, Vector2 position)
		{
			this.AddChild(motion, position, 0f);
		}

		public void AddChild(Motion motion, float threshold)
		{
			this.AddChild(motion, Vector2.zero, threshold);
		}

		public void RemoveChild(int index)
		{
			Undo.RecordObject(this, "Remove Child");
			ChildMotion[] children = this.children;
			ArrayUtility.RemoveAt<ChildMotion>(ref children, index);
			this.children = children;
		}

		internal void AddChild(Motion motion, Vector2 position, float threshold)
		{
			Undo.RecordObject(this, "Added BlendTree Child");
			ChildMotion[] children = this.children;
			ArrayUtility.Add<ChildMotion>(ref children, new ChildMotion
			{
				timeScale = 1f,
				motion = motion,
				position = position,
				threshold = threshold,
				directBlendParameter = "Blend"
			});
			this.children = children;
		}

		public BlendTree CreateBlendTreeChild(float threshold)
		{
			return this.CreateBlendTreeChild(Vector2.zero, threshold);
		}

		public BlendTree CreateBlendTreeChild(Vector2 position)
		{
			return this.CreateBlendTreeChild(position, 0f);
		}

		internal bool HasChild(BlendTree childTree, bool recursive)
		{
			ChildMotion[] children = this.children;
			int i = 0;
			bool result;
			while (i < children.Length)
			{
				ChildMotion childMotion = children[i];
				if (childMotion.motion == childTree)
				{
					result = true;
				}
				else
				{
					if (!recursive || !(childMotion.motion is BlendTree) || !(childMotion.motion as BlendTree).HasChild(childTree, true))
					{
						i++;
						continue;
					}
					result = true;
				}
				return result;
			}
			result = false;
			return result;
		}

		internal BlendTree CreateBlendTreeChild(Vector2 position, float threshold)
		{
			Undo.RecordObject(this, "Created BlendTree Child");
			BlendTree blendTree = new BlendTree();
			blendTree.name = "BlendTree";
			blendTree.hideFlags = HideFlags.HideInHierarchy;
			if (AssetDatabase.GetAssetPath(this) != "")
			{
				AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(this));
			}
			this.AddChild(blendTree, position, threshold);
			return blendTree;
		}
	}
}
