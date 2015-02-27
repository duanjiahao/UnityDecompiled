using System;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	public sealed class Unsupported
	{
		public struct ProgressiveMesh
		{
			internal IntPtr opaquePtr;
			public static void Create(Mesh src, out Unsupported.ProgressiveMesh pm)
			{
				pm.opaquePtr = Unsupported.ProgressiveMesh.CreateImpl(src);
			}
			public static void Destroy(ref Unsupported.ProgressiveMesh pm)
			{
				Unsupported.ProgressiveMesh.DestroyImpl(pm.opaquePtr);
				pm.opaquePtr = IntPtr.Zero;
			}
			public static void CreateInitialGeometry(Unsupported.ProgressiveMesh pm, Mesh mesh)
			{
				Unsupported.ProgressiveMesh.CreateInitialGeometryImpl(pm.opaquePtr, mesh);
			}
			public static void UpdateMesh(Unsupported.ProgressiveMesh pm, int targetTriCount, Mesh mesh)
			{
				Unsupported.ProgressiveMesh.UpdateMeshImpl(pm.opaquePtr, targetTriCount, mesh);
			}
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern IntPtr CreateImpl(Mesh src);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void DestroyImpl(IntPtr pmOpaque);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void CreateInitialGeometryImpl(IntPtr pmOpaque, Mesh target);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void UpdateMeshImpl(IntPtr pmOpaque, int targetTriCount, Mesh target);
		}
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
			return Unsupported.INTERNAL_CALL_MakeNiceVector3(ref vector);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector3 INTERNAL_CALL_MakeNiceVector3(ref Vector3 vector);
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
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyStateToPasteboard(State state, AnimatorController controller);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyStateMachineToPasteboard(StateMachine stateMachine, AnimatorController controller);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PasteToStateMachineFromPasteboard(StateMachine sm, AnimatorController controller);
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
	}
}
