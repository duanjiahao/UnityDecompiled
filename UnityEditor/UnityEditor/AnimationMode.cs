using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class AnimationMode
	{
		private static bool s_InAnimationPlaybackMode = false;

		private static bool s_InAnimationRecordMode = false;

		private static PrefColor s_AnimatedPropertyColor = new PrefColor("Animation/Property Animated", 0.82f, 0.97f, 1f, 1f, 0.54f, 0.85f, 1f, 1f);

		private static PrefColor s_RecordedPropertyColor = new PrefColor("Animation/Property Recorded", 1f, 0.6f, 0.6f, 1f, 1f, 0.5f, 0.5f, 1f);

		private static PrefColor s_CandidatePropertyColor = new PrefColor("Animation/Property Candidate", 1f, 0.7f, 0.6f, 1f, 1f, 0.67f, 0.43f, 1f);

		private static AnimationModeDriver s_DummyDriver;

		public static Color animatedPropertyColor
		{
			get
			{
				return AnimationMode.s_AnimatedPropertyColor;
			}
		}

		public static Color recordedPropertyColor
		{
			get
			{
				return AnimationMode.s_RecordedPropertyColor;
			}
		}

		public static Color candidatePropertyColor
		{
			get
			{
				return AnimationMode.s_CandidatePropertyColor;
			}
		}

		private static AnimationModeDriver DummyDriver()
		{
			if (AnimationMode.s_DummyDriver == null)
			{
				AnimationMode.s_DummyDriver = ScriptableObject.CreateInstance<AnimationModeDriver>();
				AnimationMode.s_DummyDriver.name = "DummyDriver";
			}
			return AnimationMode.s_DummyDriver;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsPropertyAnimated(UnityEngine.Object target, string propertyPath);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsPropertyCandidate(UnityEngine.Object target, string propertyPath);

		public static void StopAnimationMode()
		{
			AnimationMode.StopAnimationMode(AnimationMode.DummyDriver());
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StopAnimationMode(UnityEngine.Object driver);

		public static bool InAnimationMode()
		{
			return AnimationMode.Internal_InAnimationModeNoDriver();
		}

		internal static bool InAnimationMode(UnityEngine.Object driver)
		{
			return AnimationMode.Internal_InAnimationMode(driver);
		}

		public static void StartAnimationMode()
		{
			AnimationMode.StartAnimationMode(AnimationMode.DummyDriver());
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StartAnimationMode(UnityEngine.Object driver);

		internal static void StopAnimationPlaybackMode()
		{
			AnimationMode.s_InAnimationPlaybackMode = false;
		}

		internal static bool InAnimationPlaybackMode()
		{
			return AnimationMode.s_InAnimationPlaybackMode;
		}

		internal static void StartAnimationPlaybackMode()
		{
			AnimationMode.s_InAnimationPlaybackMode = true;
		}

		internal static void StopAnimationRecording()
		{
			AnimationMode.s_InAnimationRecordMode = false;
		}

		internal static bool InAnimationRecording()
		{
			return AnimationMode.s_InAnimationRecordMode;
		}

		internal static void StartAnimationRecording()
		{
			AnimationMode.s_InAnimationRecordMode = true;
		}

		internal static void StartCandidateRecording(UnityEngine.Object driver)
		{
			if (!AnimationMode.InAnimationMode())
			{
				throw new InvalidOperationException("AnimationMode.StartCandidateRecording may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
			}
			AnimationMode.Internal_StartCandidateRecording(driver);
		}

		internal static void AddCandidate(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride)
		{
			if (!AnimationMode.IsRecordingCandidates())
			{
				throw new InvalidOperationException("AnimationMode.AddCandidate may only be called when recording candidates.  See AnimationMode.StartCandidateRecording.");
			}
			AnimationMode.Internal_AddCandidate(binding, modification, keepPrefabOverride);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StopCandidateRecording();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsRecordingCandidates();

		public static void BeginSampling()
		{
			if (!AnimationMode.InAnimationMode())
			{
				throw new InvalidOperationException("AnimationMode.BeginSampling may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
			}
			AnimationMode.Internal_BeginSampling();
		}

		public static void EndSampling()
		{
			if (!AnimationMode.InAnimationMode())
			{
				throw new InvalidOperationException("AnimationMode.EndSampling may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
			}
			AnimationMode.Internal_EndSampling();
		}

		public static void SampleAnimationClip(GameObject gameObject, AnimationClip clip, float time)
		{
			if (!AnimationMode.InAnimationMode())
			{
				throw new InvalidOperationException("AnimationMode.SampleAnimationClip may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
			}
			AnimationMode.Internal_SampleAnimationClip(gameObject, clip, time);
		}

		internal static void SampleCandidateClip(GameObject gameObject, AnimationClip clip, float time)
		{
			if (!AnimationMode.IsRecordingCandidates())
			{
				throw new InvalidOperationException("AnimationMode.SampleCandidateClip may only be called when recording candidates.  See AnimationMode.StartAnimationMode.");
			}
			AnimationMode.Internal_SampleCandidateClip(gameObject, clip, time);
		}

		public static void AddPropertyModification(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride)
		{
			if (!AnimationMode.InAnimationMode())
			{
				throw new InvalidOperationException("AnimationMode.AddPropertyModification may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
			}
			AnimationMode.Internal_AddPropertyModification(binding, modification, keepPrefabOverride);
		}

		internal static void InitializePropertyModificationForGameObject(GameObject gameObject, AnimationClip clip)
		{
			if (!AnimationMode.InAnimationMode())
			{
				throw new InvalidOperationException("AnimationMode.InitializePropertyModificationForGameObject may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
			}
			AnimationMode.Internal_InitializePropertyModificationForGameObject(gameObject, clip);
		}

		internal static void InitializePropertyModificationForObject(UnityEngine.Object target, AnimationClip clip)
		{
			if (!AnimationMode.InAnimationMode())
			{
				throw new InvalidOperationException("AnimationMode.InitializePropertyModificationForObject may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
			}
			AnimationMode.Internal_InitializePropertyModificationForObject(target, clip);
		}

		internal static void RevertPropertyModificationsForGameObject(GameObject gameObject)
		{
			if (!AnimationMode.InAnimationMode())
			{
				throw new InvalidOperationException("AnimationMode.RevertPropertyModificationsForGameObject may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
			}
			AnimationMode.Internal_RevertPropertyModificationsForGameObject(gameObject);
		}

		internal static void RevertPropertyModificationsForObject(UnityEngine.Object target)
		{
			if (!AnimationMode.InAnimationMode())
			{
				throw new InvalidOperationException("AnimationMode.RevertPropertyModificationsForObject may only be called in animation mode.  See AnimationMode.StartAnimationMode.");
			}
			AnimationMode.Internal_RevertPropertyModificationsForObject(target);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_InAnimationMode(UnityEngine.Object driver);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_InAnimationModeNoDriver();

		private static void Internal_AddCandidate(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride)
		{
			AnimationMode.INTERNAL_CALL_Internal_AddCandidate(ref binding, modification, keepPrefabOverride);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_AddCandidate(ref EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_StartCandidateRecording(UnityEngine.Object driver);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BeginSampling();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_EndSampling();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SampleAnimationClip(GameObject gameObject, AnimationClip clip, float time);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SampleCandidateClip(GameObject gameObject, AnimationClip clip, float time);

		private static void Internal_AddPropertyModification(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride)
		{
			AnimationMode.INTERNAL_CALL_Internal_AddPropertyModification(ref binding, modification, keepPrefabOverride);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_AddPropertyModification(ref EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_InitializePropertyModificationForGameObject(GameObject gameObject, AnimationClip clip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_InitializePropertyModificationForObject(UnityEngine.Object target, AnimationClip clip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_RevertPropertyModificationsForGameObject(GameObject gameObject);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_RevertPropertyModificationsForObject(UnityEngine.Object target);
	}
}
