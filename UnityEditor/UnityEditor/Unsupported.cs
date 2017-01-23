using System;
using System.Runtime.CompilerServices;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor
{
	public sealed class Unsupported
	{
		private static bool s_FakeNonDeveloperBuild = EditorPrefs.GetBool("FakeNonDeveloperBuild", false);

		internal static bool fakeNonDeveloperBuild
		{
			get
			{
				return Unsupported.s_FakeNonDeveloperBuild;
			}
			set
			{
				Unsupported.s_FakeNonDeveloperBuild = value;
				EditorPrefs.SetBool("FakeNonDeveloperBuild", value);
			}
		}

		internal static Vector3 MakeNiceVector3(Vector3 vector)
		{
			Vector3 result;
			Unsupported.INTERNAL_CALL_MakeNiceVector3(ref vector, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MakeNiceVector3(ref Vector3 vector, out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CaptureScreenshotImmediate(string filePath, int x, int y, int width, int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern NETVersion GetNETVersion();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetSubmenusCommands(string menuPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type GetTypeFromFullName(string fullName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetSubmenus(string menuPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetSubmenusIncludingSeparators(string menuPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PrepareObjectContextMenu(UnityEngine.Object c, int contextUserData);

		public static bool IsDeveloperBuild()
		{
			return Unsupported.IsDeveloperBuildInternal() && !Unsupported.s_FakeNonDeveloperBuild;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsDeveloperBuildInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsBleedingEdgeBuild();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDestroyScriptableObject(ScriptableObject target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsNativeCodeBuiltInReleaseMode();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetBaseUnityDeveloperFolder();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopPlayingImmediately();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SceneTrackerFlushDirty();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAllowCursorHide(bool allow);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAllowCursorLock(bool allow);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetRenderSettingsUseFogNoDirty(bool fog);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetQualitySettingsShadowDistanceTemporarily(float distance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteGameObjectSelection();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyGameObjectsToPasteboard();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PasteGameObjectsFromPasteboard();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetSerializedAssetInterfaceSingleton(string className);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DuplicateGameObjectsUsingPasteboard();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CopyComponentToPasteboard(Component component);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PasteComponentFromPasteboard(GameObject go);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PasteComponentValuesFromPasteboard(Component component);

		public static void CopyStateMachineDataToPasteboard(UnityEngine.Object stateMachineObject, AnimatorController controller, int layerIndex)
		{
			Unsupported.CopyStateMachineDataToPasteboard(new UnityEngine.Object[]
			{
				stateMachineObject
			}, new Vector3[]
			{
				default(Vector3)
			}, controller, layerIndex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyStateMachineDataToPasteboard(UnityEngine.Object[] stateMachineObjects, Vector3[] monoPositions, AnimatorController controller, int layerIndex);

		public static void PasteToStateMachineFromPasteboard(AnimatorStateMachine sm, AnimatorController controller, int layerIndex, Vector3 position)
		{
			Undo.RegisterCompleteObjectUndo(sm, "Paste to StateMachine");
			Unsupported.PasteToStateMachineFromPasteboardInternal(sm, controller, layerIndex, position);
		}

		internal static void PasteToStateMachineFromPasteboardInternal(AnimatorStateMachine sm, AnimatorController controller, int layerIndex, Vector3 position)
		{
			Unsupported.INTERNAL_CALL_PasteToStateMachineFromPasteboardInternal(sm, controller, layerIndex, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_PasteToStateMachineFromPasteboardInternal(AnimatorStateMachine sm, AnimatorController controller, int layerIndex, ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasStateMachineDataInPasteboard();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SmartReset(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string ResolveSymlinks(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetApplicationSettingCompressAssetsOnImport(bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetApplicationSettingCompressAssetsOnImport();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetLocalIdentifierInFile(int instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsHiddenFile(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearSkinCache();
	}
}
