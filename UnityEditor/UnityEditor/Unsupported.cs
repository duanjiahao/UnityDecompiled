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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MakeNiceVector3(ref Vector3 vector, out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CaptureScreenshotImmediate(string filePath, int x, int y, int width, int height);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern NETVersion GetNETVersion();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetSubmenusCommands(string menuPath);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type GetTypeFromFullName(string fullName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetSubmenus(string menuPath);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetSubmenusIncludingSeparators(string menuPath);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PrepareObjectContextMenu(UnityEngine.Object c, int contextUserData);

		public static bool IsDeveloperBuild()
		{
			return Unsupported.IsDeveloperBuildInternal() && !Unsupported.s_FakeNonDeveloperBuild;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsDeveloperBuildInternal();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsBleedingEdgeBuild();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDestroyScriptableObject(ScriptableObject target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsNativeCodeBuiltInReleaseMode();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetBaseUnityDeveloperFolder();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopPlayingImmediately();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SceneTrackerFlushDirty();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAllowCursorHide(bool allow);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAllowCursorLock(bool allow);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetRenderSettingsUseFogNoDirty(bool fog);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetQualitySettingsShadowDistanceTemporarily(float distance);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteGameObjectSelection();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyGameObjectsToPasteboard();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PasteGameObjectsFromPasteboard();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetSerializedAssetInterfaceSingleton(string className);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DuplicateGameObjectsUsingPasteboard();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CopyComponentToPasteboard(Component component);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PasteComponentFromPasteboard(GameObject go);

		[WrapperlessIcall]
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

		[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_PasteToStateMachineFromPasteboardInternal(AnimatorStateMachine sm, AnimatorController controller, int layerIndex, ref Vector3 position);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasStateMachineDataInPasteboard();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SmartReset(UnityEngine.Object obj);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string ResolveSymlinks(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetApplicationSettingCompressAssetsOnImport(bool value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetApplicationSettingCompressAssetsOnImport();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetLocalIdentifierInFile(int instanceID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsHiddenFile(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearSkinCache();
	}
}
