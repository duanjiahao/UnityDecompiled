using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class AnimatorOverrideController : RuntimeAnimatorController
	{
		internal delegate void OnOverrideControllerDirtyCallback();

		internal AnimatorOverrideController.OnOverrideControllerDirtyCallback OnOverrideControllerDirty;

		public extern RuntimeAnimatorController runtimeAnimatorController
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public AnimationClip this[string name]
		{
			get
			{
				return this.Internal_GetClipByName(name, true);
			}
			set
			{
				this.Internal_SetClipByName(name, value);
			}
		}

		public AnimationClip this[AnimationClip clip]
		{
			get
			{
				return this.Internal_GetClip(clip, true);
			}
			set
			{
				this.Internal_SetClip(clip, value);
			}
		}

		public AnimationClipPair[] clips
		{
			get
			{
				AnimationClip[] array = this.GetOriginalClips();
				Dictionary<AnimationClip, bool> dictionary = new Dictionary<AnimationClip, bool>(array.Length);
				AnimationClip[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					AnimationClip key = array2[i];
					dictionary[key] = true;
				}
				array = new AnimationClip[dictionary.Count];
				dictionary.Keys.CopyTo(array, 0);
				AnimationClipPair[] array3 = new AnimationClipPair[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array3[j] = new AnimationClipPair();
					array3[j].originalClip = array[j];
					array3[j].overrideClip = this.Internal_GetClip(array[j], false);
				}
				return array3;
			}
			set
			{
				for (int i = 0; i < value.Length; i++)
				{
					this.Internal_SetClip(value[i].originalClip, value[i].overrideClip, false);
				}
				this.Internal_SetDirty();
			}
		}

		public AnimatorOverrideController()
		{
			AnimatorOverrideController.Internal_CreateAnimationSet(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateAnimationSet([Writable] AnimatorOverrideController self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnimationClip Internal_GetClipByName(string name, bool returnEffectiveClip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetClipByName(string name, AnimationClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnimationClip Internal_GetClip(AnimationClip originalClip, bool returnEffectiveClip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetClip(AnimationClip originalClip, AnimationClip overrideClip, [DefaultValue("true")] bool notify);

		[ExcludeFromDocs]
		private void Internal_SetClip(AnimationClip originalClip, AnimationClip overrideClip)
		{
			bool notify = true;
			this.Internal_SetClip(originalClip, overrideClip, notify);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetDirty();

		[RequiredByNativeCode]
		internal static void OnInvalidateOverrideController(AnimatorOverrideController controller)
		{
			if (controller.OnOverrideControllerDirty != null)
			{
				controller.OnOverrideControllerDirty();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnimationClip[] GetOriginalClips();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnimationClip[] GetOverrideClips();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void PerformOverrideClipListCleanup();
	}
}
