using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditorInternal
{
	public sealed class BlendTreePreviewUtility
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetRootBlendTreeChildWeights(Animator animator, int layerIndex, int stateHash, float[] weightArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CalculateRootBlendTreeChildWeights(Animator animator, int layerIndex, int stateHash, float[] weightArray, float blendX, float blendY);

		public static void CalculateBlendTexture(Animator animator, int layerIndex, int stateHash, Texture2D blendTexture, Texture2D[] weightTextures, Rect rect)
		{
			BlendTreePreviewUtility.INTERNAL_CALL_CalculateBlendTexture(animator, layerIndex, stateHash, blendTexture, weightTextures, ref rect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CalculateBlendTexture(Animator animator, int layerIndex, int stateHash, Texture2D blendTexture, Texture2D[] weightTextures, ref Rect rect);
	}
}
