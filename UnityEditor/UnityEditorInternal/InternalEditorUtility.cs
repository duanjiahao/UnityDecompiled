using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Utils;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditorInternal
{
	public sealed class InternalEditorUtility
	{
		public enum HierarchyDropMode
		{
			kHierarchyDragNormal,
			kHierarchyDropUpon,
			kHierarchyDropBetween,
			kHierarchyDropAfterParent = 4,
			kHierarchySearchActive = 8
		}

		public enum ScriptEditor
		{
			Internal,
			MonoDevelop,
			VisualStudio,
			VisualStudioExpress,
			VisualStudioCode,
			Other = 32
		}

		public static extern bool isApplicationActive
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool inBatchMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isHumanControllingUs
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int[] expandedProjectWindowItems
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string[] tags
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string[] layers
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern string[] sortingLayerNames
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern int[] sortingLayerUniqueIDs
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string unityPreferencesFolder
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float defaultScreenWidth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float defaultScreenHeight
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float defaultWebScreenWidth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float defaultWebScreenHeight
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float remoteScreenWidth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float remoteScreenHeight
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool ignoreInspectorChanges
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BumpMapSettingsFixingWindowReportResult(int result);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BumpMapTextureNeedsFixingInternal(Material material, string propName, bool flaggedAsNormal);

		internal static bool BumpMapTextureNeedsFixing(MaterialProperty prop)
		{
			bool result;
			if (prop.type != MaterialProperty.PropType.Texture)
			{
				result = false;
			}
			else
			{
				bool flaggedAsNormal = (prop.flags & MaterialProperty.PropFlags.Normal) != MaterialProperty.PropFlags.None;
				UnityEngine.Object[] targets = prop.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					Material material = (Material)targets[i];
					if (InternalEditorUtility.BumpMapTextureNeedsFixingInternal(material, prop.name, flaggedAsNormal))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FixNormalmapTextureInternal(Material material, string propName);

		internal static void FixNormalmapTexture(MaterialProperty prop)
		{
			UnityEngine.Object[] targets = prop.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				InternalEditorUtility.FixNormalmapTextureInternal(material, prop.name);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetEditorAssemblyPath();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetEngineAssemblyPath();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string CalculateHashForObjectsAndDependencies(UnityEngine.Object[] objects);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExecuteCommandOnKeyWindow(string commandName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Material[] InstantiateMaterialsInEditMode(Renderer renderer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern CanAppendBuild BuildCanBeAppended(BuildTarget target, string location);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RegisterExtensionDll(string dllLocation, string guid);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetupCustomDll(string dllName, string dllLocation);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly LoadAssemblyWrapper(string dllName, string dllLocation);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPlatformPath(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int AddScriptComponentUncheckedUndoable(GameObject gameObject, MonoScript script);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int CreateScriptableObjectUnchecked(MonoScript script);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RequestScriptReload();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SwitchSkinAndRepaintAllViews();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RepaintAllViews();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetIsInspectorExpanded(UnityEngine.Object obj);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetIsInspectorExpanded(UnityEngine.Object obj, bool isExpanded);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SaveToSerializedFileAndForget(UnityEngine.Object[] obj, string path, bool allowTextSerialization);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object[] LoadSerializedFileAndForget(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern DragAndDropVisualMode ProjectWindowDrag(HierarchyProperty property, bool perform);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern DragAndDropVisualMode HierarchyWindowDrag(HierarchyProperty property, bool perform, InternalEditorUtility.HierarchyDropMode dropMode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern DragAndDropVisualMode InspectorWindowDrag(UnityEngine.Object[] targets, bool perform);

		public static DragAndDropVisualMode SceneViewDrag(UnityEngine.Object dropUpon, Vector3 worldPosition, Vector2 viewportPosition, bool perform)
		{
			return InternalEditorUtility.INTERNAL_CALL_SceneViewDrag(dropUpon, ref worldPosition, ref viewportPosition, perform);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DragAndDropVisualMode INTERNAL_CALL_SceneViewDrag(UnityEngine.Object dropUpon, ref Vector3 worldPosition, ref Vector2 viewportPosition, bool perform);

		public static void SetRectTransformTemporaryRect(RectTransform rectTransform, Rect rect)
		{
			InternalEditorUtility.INTERNAL_CALL_SetRectTransformTemporaryRect(rectTransform, ref rect);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetRectTransformTemporaryRect(RectTransform rectTransform, ref Rect rect);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasTeamLicense();

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasPro();

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasFreeLicense();

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasEduLicense();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasAdvancedLicenseOnBuildTarget(BuildTarget target);

		public static Rect GetBoundsOfDesktopAtPoint(Vector2 pos)
		{
			Rect result;
			InternalEditorUtility.INTERNAL_CALL_GetBoundsOfDesktopAtPoint(ref pos, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetBoundsOfDesktopAtPoint(ref Vector2 pos, out Rect value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RemoveTag(string tag);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddTag(string tag);

		public static LayerMask ConcatenatedLayersMaskToLayerMask(int concatenatedLayersMask)
		{
			LayerMask result;
			InternalEditorUtility.INTERNAL_CALL_ConcatenatedLayersMaskToLayerMask(concatenatedLayersMask, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ConcatenatedLayersMaskToLayerMask(int concatenatedLayersMask, out LayerMask value);

		public static int LayerMaskToConcatenatedLayersMask(LayerMask mask)
		{
			return InternalEditorUtility.INTERNAL_CALL_LayerMaskToConcatenatedLayersMask(ref mask);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_LayerMaskToConcatenatedLayersMask(ref LayerMask mask);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetSortingLayerName(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSortingLayerUniqueID(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetSortingLayerNameFromUniqueID(int id);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSortingLayerCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetSortingLayerName(int index, string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetSortingLayerLocked(int index, bool locked);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetSortingLayerLocked(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsSortingLayerDefault(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void AddSortingLayer();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateSortingLayersOrder();

		public static Vector4 GetSpriteOuterUV(Sprite sprite, bool getAtlasData)
		{
			Vector4 result;
			InternalEditorUtility.INTERNAL_CALL_GetSpriteOuterUV(sprite, getAtlasData, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSpriteOuterUV(Sprite sprite, bool getAtlasData, out Vector4 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetObjectFromInstanceID(int instanceID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetClassIDWithoutLoadingObject(int instanceID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetLoadedObjectFromInstanceID(int instanceID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetLayerName(int layer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAssetsFolder();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetEditorFolder();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsInEditorFolder(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReloadWindowLayoutMenu();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RevertFactoryLayoutSettings(bool quitOnCancel);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadDefaultLayout();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CalculateAmbientProbeFromSkybox();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetupShaderMenu(Material material);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUnityVersionFull();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetFullUnityVersion();

		public static Version GetUnityVersion()
		{
			Version version = new Version(InternalEditorUtility.GetUnityVersionDigits());
			return new Version(version.Major, version.Minor, version.Build, InternalEditorUtility.GetUnityRevision());
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUnityVersionDigits();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUnityBuildBranch();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetUnityVersionDate();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetUnityRevision();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsUnityBeta();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUnityCopyright();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetLicenseInfo();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int[] GetLicenseFlags();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAuthToken();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OpenEditorConsole();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetGameObjectInstanceIDFromComponent(int instanceID);

		public static Color[] ReadScreenPixel(Vector2 pixelPos, int sizex, int sizey)
		{
			return InternalEditorUtility.INTERNAL_CALL_ReadScreenPixel(ref pixelPos, sizex, sizey);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color[] INTERNAL_CALL_ReadScreenPixel(ref Vector2 pixelPos, int sizex, int sizey);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OpenPlayerConsole();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Resolution GetDesktopResolution();

		public static string TextifyEvent(Event evt)
		{
			string result;
			if (evt == null)
			{
				result = "none";
			}
			else
			{
				KeyCode keyCode = evt.keyCode;
				string str;
				switch (keyCode)
				{
				case KeyCode.Keypad0:
					str = "[0]";
					goto IL_2EE;
				case KeyCode.Keypad1:
					str = "[1]";
					goto IL_2EE;
				case KeyCode.Keypad2:
					str = "[2]";
					goto IL_2EE;
				case KeyCode.Keypad3:
					str = "[3]";
					goto IL_2EE;
				case KeyCode.Keypad4:
					str = "[4]";
					goto IL_2EE;
				case KeyCode.Keypad5:
					str = "[5]";
					goto IL_2EE;
				case KeyCode.Keypad6:
					str = "[6]";
					goto IL_2EE;
				case KeyCode.Keypad7:
					str = "[7]";
					goto IL_2EE;
				case KeyCode.Keypad8:
					str = "[8]";
					goto IL_2EE;
				case KeyCode.Keypad9:
					str = "[9]";
					goto IL_2EE;
				case KeyCode.KeypadPeriod:
					str = "[.]";
					goto IL_2EE;
				case KeyCode.KeypadDivide:
					str = "[/]";
					goto IL_2EE;
				case KeyCode.KeypadMultiply:
					IL_CB:
					if (keyCode == KeyCode.Backspace)
					{
						str = "backspace";
						goto IL_2EE;
					}
					if (keyCode == KeyCode.Return)
					{
						str = "return";
						goto IL_2EE;
					}
					if (keyCode == KeyCode.Escape)
					{
						str = "[esc]";
						goto IL_2EE;
					}
					if (keyCode != KeyCode.Delete)
					{
						str = "" + evt.keyCode;
						goto IL_2EE;
					}
					str = "delete";
					goto IL_2EE;
				case KeyCode.KeypadMinus:
					str = "[-]";
					goto IL_2EE;
				case KeyCode.KeypadPlus:
					str = "[+]";
					goto IL_2EE;
				case KeyCode.KeypadEnter:
					str = "enter";
					goto IL_2EE;
				case KeyCode.KeypadEquals:
					str = "[=]";
					goto IL_2EE;
				case KeyCode.UpArrow:
					str = "up";
					goto IL_2EE;
				case KeyCode.DownArrow:
					str = "down";
					goto IL_2EE;
				case KeyCode.RightArrow:
					str = "right";
					goto IL_2EE;
				case KeyCode.LeftArrow:
					str = "left";
					goto IL_2EE;
				case KeyCode.Insert:
					str = "insert";
					goto IL_2EE;
				case KeyCode.Home:
					str = "home";
					goto IL_2EE;
				case KeyCode.End:
					str = "end";
					goto IL_2EE;
				case KeyCode.PageUp:
					str = "page up";
					goto IL_2EE;
				case KeyCode.PageDown:
					str = "page down";
					goto IL_2EE;
				case KeyCode.F1:
					str = "F1";
					goto IL_2EE;
				case KeyCode.F2:
					str = "F2";
					goto IL_2EE;
				case KeyCode.F3:
					str = "F3";
					goto IL_2EE;
				case KeyCode.F4:
					str = "F4";
					goto IL_2EE;
				case KeyCode.F5:
					str = "F5";
					goto IL_2EE;
				case KeyCode.F6:
					str = "F6";
					goto IL_2EE;
				case KeyCode.F7:
					str = "F7";
					goto IL_2EE;
				case KeyCode.F8:
					str = "F8";
					goto IL_2EE;
				case KeyCode.F9:
					str = "F9";
					goto IL_2EE;
				case KeyCode.F10:
					str = "F10";
					goto IL_2EE;
				case KeyCode.F11:
					str = "F11";
					goto IL_2EE;
				case KeyCode.F12:
					str = "F12";
					goto IL_2EE;
				case KeyCode.F13:
					str = "F13";
					goto IL_2EE;
				case KeyCode.F14:
					str = "F14";
					goto IL_2EE;
				case KeyCode.F15:
					str = "F15";
					goto IL_2EE;
				}
				goto IL_CB;
				IL_2EE:
				string str2 = string.Empty;
				if (evt.alt)
				{
					str2 += "Alt+";
				}
				if (evt.command)
				{
					str2 += ((Application.platform != RuntimePlatform.OSXEditor) ? "Ctrl+" : "Cmd+");
				}
				if (evt.control)
				{
					str2 += "Ctrl+";
				}
				if (evt.shift)
				{
					str2 += "Shift+";
				}
				result = str2 + str;
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAvailableDiffTools();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetNoDiffToolsDetectedMessage();

		public static Bounds TransformBounds(Bounds b, Transform t)
		{
			Bounds result;
			InternalEditorUtility.INTERNAL_CALL_TransformBounds(ref b, t, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_TransformBounds(ref Bounds b, Transform t, out Bounds value);

		public static void SetCustomLighting(Light[] lights, Color ambient)
		{
			InternalEditorUtility.INTERNAL_CALL_SetCustomLighting(lights, ref ambient);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetCustomLighting(Light[] lights, ref Color ambient);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ClearSceneLighting();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RemoveCustomLighting();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DrawSkyboxMaterial(Material mat, Camera cam);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasFullscreenCamera();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetCursor();

		public static Bounds CalculateSelectionBounds(bool usePivotOnlyForParticles, bool onlyUseActiveSelection)
		{
			Bounds result;
			InternalEditorUtility.INTERNAL_CALL_CalculateSelectionBounds(usePivotOnlyForParticles, onlyUseActiveSelection, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CalculateSelectionBounds(bool usePivotOnlyForParticles, bool onlyUseActiveSelection, out Bounds value);

		internal static Bounds CalculateSelectionBoundsInSpace(Vector3 position, Quaternion rotation, bool rectBlueprintMode)
		{
			Quaternion rotation2 = Quaternion.Inverse(rotation);
			Vector3 vector = new Vector3(3.40282347E+38f, 3.40282347E+38f, 3.40282347E+38f);
			Vector3 vector2 = new Vector3(-3.40282347E+38f, -3.40282347E+38f, -3.40282347E+38f);
			Vector3[] array = new Vector3[2];
			GameObject[] gameObjects = Selection.gameObjects;
			for (int i = 0; i < gameObjects.Length; i++)
			{
				GameObject gameObject = gameObjects[i];
				Bounds localBounds = InternalEditorUtility.GetLocalBounds(gameObject);
				array[0] = localBounds.min;
				array[1] = localBounds.max;
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						for (int l = 0; l < 2; l++)
						{
							Vector3 vector3 = new Vector3(array[j].x, array[k].y, array[l].z);
							if (rectBlueprintMode && InternalEditorUtility.SupportsRectLayout(gameObject.transform))
							{
								Vector3 localPosition = gameObject.transform.localPosition;
								localPosition.z = 0f;
								vector3 = gameObject.transform.parent.TransformPoint(vector3 + localPosition);
							}
							else
							{
								vector3 = gameObject.transform.TransformPoint(vector3);
							}
							vector3 = rotation2 * (vector3 - position);
							for (int m = 0; m < 3; m++)
							{
								vector[m] = Mathf.Min(vector[m], vector3[m]);
								vector2[m] = Mathf.Max(vector2[m], vector3[m]);
							}
						}
					}
				}
			}
			return new Bounds((vector + vector2) * 0.5f, vector2 - vector);
		}

		internal static bool SupportsRectLayout(Transform tr)
		{
			return !(tr == null) && !(tr.parent == null) && !(tr.GetComponent<RectTransform>() == null) && !(tr.parent.GetComponent<RectTransform>() == null);
		}

		private static Bounds GetLocalBounds(GameObject gameObject)
		{
			RectTransform component = gameObject.GetComponent<RectTransform>();
			Bounds result;
			if (component)
			{
				result = new Bounds(component.rect.center, component.rect.size);
			}
			else
			{
				Renderer component2 = gameObject.GetComponent<Renderer>();
				if (component2 is MeshRenderer)
				{
					MeshFilter component3 = component2.GetComponent<MeshFilter>();
					if (component3 != null && component3.sharedMesh != null)
					{
						result = component3.sharedMesh.bounds;
						return result;
					}
				}
				if (component2 is SpriteRenderer)
				{
					result = ((SpriteRenderer)component2).GetSpriteBounds();
				}
				else
				{
					result = new Bounds(Vector3.zero, Vector3.zero);
				}
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OnGameViewFocus(bool focus);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool OpenFileAtLineExternal(string filename, int line);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MonoIsland[] GetMonoIslands();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MonoIsland[] GetMonoIslandsForPlayer();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool WiiUSaveStartupScreenToFile(Texture2D image, string path, int outputWidth, int outputHeight);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CanConnectToCacheServer();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong VerifyCacheServerIntegrity();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong FixCacheServerIntegrityErrors();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern DllType DetectDotNetDll(string path);

		public static bool IsDotNet4Dll(string path)
		{
			DllType dllType = InternalEditorUtility.DetectDotNetDll(path);
			bool result;
			switch (dllType)
			{
			case DllType.Unknown:
			case DllType.Native:
			case DllType.UnknownManaged:
			case DllType.ManagedNET35:
				result = false;
				break;
			case DllType.ManagedNET40:
			case DllType.WinMDNative:
			case DllType.WinMDNET40:
				result = true;
				break;
			default:
				throw new Exception(string.Format("Unknown dll type: {0}", dllType));
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetCrashReportFolder();

		[ExcludeFromDocs]
		internal static bool RunningUnderWindows8()
		{
			bool orHigher = true;
			return InternalEditorUtility.RunningUnderWindows8(orHigher);
		}

		internal static bool RunningUnderWindows8([DefaultValue("true")] bool orHigher)
		{
			bool result;
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				OperatingSystem oSVersion = Environment.OSVersion;
				int major = oSVersion.Version.Major;
				int minor = oSVersion.Version.Minor;
				if (orHigher)
				{
					result = (major > 6 || (major == 6 && minor >= 2));
				}
				else
				{
					result = (major == 6 && minor == 2);
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int DetermineDepthOrder(Transform lhs, Transform rhs);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ShowPackageManagerWindow();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void AuxWindowManager_OnAssemblyReload();

		public static Vector2 PassAndReturnVector2(Vector2 v)
		{
			Vector2 result;
			InternalEditorUtility.INTERNAL_CALL_PassAndReturnVector2(ref v, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_PassAndReturnVector2(ref Vector2 v, out Vector2 value);

		public static Color32 PassAndReturnColor32(Color32 c)
		{
			Color32 result;
			InternalEditorUtility.INTERNAL_CALL_PassAndReturnColor32(ref c, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_PassAndReturnColor32(ref Color32 c, out Color32 value);

		public static string CountToString(ulong count)
		{
			string[] array = new string[]
			{
				"g",
				"m",
				"k",
				""
			};
			float[] array2 = new float[]
			{
				1E+09f,
				1000000f,
				1000f,
				1f
			};
			int num = 0;
			while (num < 3 && count < array2[num] / 2f)
			{
				num++;
			}
			return (count / array2[num]).ToString("0.0") + array[num];
		}

		[Obsolete("use EditorSceneManager.EnsureUntitledSceneHasBeenSaved"), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool EnsureSceneHasBeenSaved(string operation);

		internal static void PrepareDragAndDropTesting(EditorWindow editorWindow)
		{
			if (editorWindow.m_Parent != null)
			{
				InternalEditorUtility.PrepareDragAndDropTestingInternal(editorWindow.m_Parent);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void PrepareDragAndDropTestingInternal(GUIView guiView);

		public static bool SaveCursorToFile(string path, Texture2D image, Vector2 hotSpot)
		{
			return InternalEditorUtility.INTERNAL_CALL_SaveCursorToFile(path, image, ref hotSpot);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SaveCursorToFile(string path, Texture2D image, ref Vector2 hotSpot);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool LaunchApplication(string path, string[] arguments);

		public static InternalEditorUtility.ScriptEditor GetScriptEditorFromPath(string path)
		{
			string text = path.ToLower();
			InternalEditorUtility.ScriptEditor result;
			if (text == "internal")
			{
				result = InternalEditorUtility.ScriptEditor.Internal;
			}
			else if (text.Contains("monodevelop") || text.Contains("xamarinstudio") || text.Contains("xamarin studio"))
			{
				result = InternalEditorUtility.ScriptEditor.MonoDevelop;
			}
			else if (text.EndsWith("devenv.exe"))
			{
				result = InternalEditorUtility.ScriptEditor.VisualStudio;
			}
			else if (text.EndsWith("vcsexpress.exe"))
			{
				result = InternalEditorUtility.ScriptEditor.VisualStudioExpress;
			}
			else
			{
				string a = Path.GetFileName(Paths.UnifyDirectorySeparator(text)).Replace(" ", "");
				if (a == "code.exe" || a == "visualstudiocode.app" || a == "vscode.app" || a == "code.app" || a == "code")
				{
					result = InternalEditorUtility.ScriptEditor.VisualStudioCode;
				}
				else
				{
					result = InternalEditorUtility.ScriptEditor.Other;
				}
			}
			return result;
		}

		public static bool IsScriptEditorSpecial(string path)
		{
			return InternalEditorUtility.GetScriptEditorFromPath(path) != InternalEditorUtility.ScriptEditor.Other;
		}

		public static string GetExternalScriptEditor()
		{
			return EditorPrefs.GetString("kScriptsDefaultApp");
		}

		public static void SetExternalScriptEditor(string path)
		{
			EditorPrefs.SetString("kScriptsDefaultApp", path);
		}

		private static string GetScriptEditorArgsKey(string path)
		{
			string result;
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				result = "kScriptEditorArgs_" + path;
			}
			else
			{
				result = "kScriptEditorArgs" + path;
			}
			return result;
		}

		private static string GetDefaultStringEditorArgs()
		{
			string result;
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				result = "";
			}
			else
			{
				result = "\"$(File)\"";
			}
			return result;
		}

		public static string GetExternalScriptEditorArgs()
		{
			string externalScriptEditor = InternalEditorUtility.GetExternalScriptEditor();
			string result;
			if (InternalEditorUtility.IsScriptEditorSpecial(externalScriptEditor))
			{
				result = "";
			}
			else
			{
				result = EditorPrefs.GetString(InternalEditorUtility.GetScriptEditorArgsKey(externalScriptEditor), InternalEditorUtility.GetDefaultStringEditorArgs());
			}
			return result;
		}

		public static void SetExternalScriptEditorArgs(string args)
		{
			string externalScriptEditor = InternalEditorUtility.GetExternalScriptEditor();
			EditorPrefs.SetString(InternalEditorUtility.GetScriptEditorArgsKey(externalScriptEditor), args);
		}

		public static InternalEditorUtility.ScriptEditor GetScriptEditorFromPreferences()
		{
			return InternalEditorUtility.GetScriptEditorFromPath(InternalEditorUtility.GetExternalScriptEditor());
		}

		public static Texture2D GetIconForFile(string fileName)
		{
			int num = fileName.LastIndexOf('.');
			string text = (num != -1) ? fileName.Substring(num + 1).ToLower() : "";
			Texture2D result;
			switch (text)
			{
			case "boo":
				result = EditorGUIUtility.FindTexture("boo Script Icon");
				return result;
			case "cginc":
				result = EditorGUIUtility.FindTexture("CGProgram Icon");
				return result;
			case "cs":
				result = EditorGUIUtility.FindTexture("cs Script Icon");
				return result;
			case "guiskin":
				result = EditorGUIUtility.FindTexture("GUISkin Icon");
				return result;
			case "js":
				result = EditorGUIUtility.FindTexture("Js Script Icon");
				return result;
			case "mat":
				result = EditorGUIUtility.FindTexture("Material Icon");
				return result;
			case "physicmaterial":
				result = EditorGUIUtility.FindTexture("PhysicMaterial Icon");
				return result;
			case "prefab":
				result = EditorGUIUtility.FindTexture("PrefabNormal Icon");
				return result;
			case "shader":
				result = EditorGUIUtility.FindTexture("Shader Icon");
				return result;
			case "txt":
				result = EditorGUIUtility.FindTexture("TextAsset Icon");
				return result;
			case "unity":
				result = EditorGUIUtility.FindTexture("SceneAsset Icon");
				return result;
			case "asset":
			case "prefs":
				result = EditorGUIUtility.FindTexture("GameManager Icon");
				return result;
			case "anim":
				result = EditorGUIUtility.FindTexture("Animation Icon");
				return result;
			case "meta":
				result = EditorGUIUtility.FindTexture("MetaFile Icon");
				return result;
			case "mixer":
				result = EditorGUIUtility.FindTexture("AudioMixerController Icon");
				return result;
			case "ttf":
			case "otf":
			case "fon":
			case "fnt":
				result = EditorGUIUtility.FindTexture("Font Icon");
				return result;
			case "aac":
			case "aif":
			case "aiff":
			case "au":
			case "mid":
			case "midi":
			case "mp3":
			case "mpa":
			case "ra":
			case "ram":
			case "wma":
			case "wav":
			case "wave":
			case "ogg":
				result = EditorGUIUtility.FindTexture("AudioClip Icon");
				return result;
			case "ai":
			case "apng":
			case "png":
			case "bmp":
			case "cdr":
			case "dib":
			case "eps":
			case "exif":
			case "gif":
			case "ico":
			case "icon":
			case "j":
			case "j2c":
			case "j2k":
			case "jas":
			case "jiff":
			case "jng":
			case "jp2":
			case "jpc":
			case "jpe":
			case "jpeg":
			case "jpf":
			case "jpg":
			case "jpw":
			case "jpx":
			case "jtf":
			case "mac":
			case "omf":
			case "qif":
			case "qti":
			case "qtif":
			case "tex":
			case "tfw":
			case "tga":
			case "tif":
			case "tiff":
			case "wmf":
			case "psd":
			case "exr":
			case "hdr":
				result = EditorGUIUtility.FindTexture("Texture Icon");
				return result;
			case "3df":
			case "3dm":
			case "3dmf":
			case "3ds":
			case "3dv":
			case "3dx":
			case "blend":
			case "c4d":
			case "lwo":
			case "lws":
			case "ma":
			case "max":
			case "mb":
			case "mesh":
			case "obj":
			case "vrl":
			case "wrl":
			case "wrz":
			case "fbx":
				result = EditorGUIUtility.FindTexture("Mesh Icon");
				return result;
			case "asf":
			case "asx":
			case "avi":
			case "dat":
			case "divx":
			case "dvx":
			case "mlv":
			case "m2l":
			case "m2t":
			case "m2ts":
			case "m2v":
			case "m4e":
			case "m4v":
			case "mjp":
			case "mov":
			case "movie":
			case "mp21":
			case "mp4":
			case "mpe":
			case "mpeg":
			case "mpg":
			case "mpv2":
			case "ogm":
			case "qt":
			case "rm":
			case "rmvb":
			case "wmw":
			case "xvid":
				result = EditorGUIUtility.FindTexture("MovieTexture Icon");
				return result;
			case "colors":
			case "gradients":
			case "curves":
			case "curvesnormalized":
			case "particlecurves":
			case "particlecurvessigned":
			case "particledoublecurves":
			case "particledoublecurvessigned":
				result = EditorGUIUtility.FindTexture("ScriptableObject Icon");
				return result;
			}
			result = EditorGUIUtility.FindTexture("DefaultAsset Icon");
			return result;
		}

		public static string[] GetEditorSettingsList(string prefix, int count)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 1; i <= count; i++)
			{
				string @string = EditorPrefs.GetString(prefix + i, "defaultValue");
				if (@string == "defaultValue")
				{
					break;
				}
				arrayList.Add(@string);
			}
			return arrayList.ToArray(typeof(string)) as string[];
		}

		public static void SaveEditorSettingsList(string prefix, string[] aList, int count)
		{
			for (int i = 0; i < aList.Length; i++)
			{
				EditorPrefs.SetString(prefix + (i + 1), aList[i]);
			}
			for (int i = aList.Length + 1; i <= count; i++)
			{
				EditorPrefs.DeleteKey(prefix + i);
			}
		}

		public static string TextAreaForDocBrowser(Rect position, string text, GUIStyle style)
		{
			int controlID = GUIUtility.GetControlID("TextAreaWithTabHandling".GetHashCode(), FocusType.Keyboard, position);
			EditorGUI.RecycledTextEditor s_RecycledEditor = EditorGUI.s_RecycledEditor;
			Event current = Event.current;
			if (s_RecycledEditor.IsEditingControl(controlID) && current.type == EventType.KeyDown)
			{
				if (current.character == '\t')
				{
					s_RecycledEditor.Insert('\t');
					current.Use();
					GUI.changed = true;
					text = s_RecycledEditor.text;
				}
				if (current.character == '\n')
				{
					s_RecycledEditor.Insert('\n');
					current.Use();
					GUI.changed = true;
					text = s_RecycledEditor.text;
				}
			}
			bool flag;
			text = EditorGUI.DoTextField(s_RecycledEditor, controlID, EditorGUI.IndentedRect(position), text, style, null, out flag, false, true, false);
			return text;
		}

		public static Camera[] GetSceneViewCameras()
		{
			return SceneView.GetAllSceneCameras();
		}

		public static void ShowGameView()
		{
			WindowLayout.ShowAppropriateViewOnEnterExitPlaymode(true);
		}

		public static List<int> GetNewSelection(int clickedInstanceID, List<int> allInstanceIDs, List<int> selectedInstanceIDs, int lastClickedInstanceID, bool keepMultiSelection, bool useShiftAsActionKey, bool allowMultiSelection)
		{
			List<int> list = new List<int>();
			bool flag = Event.current.shift || (EditorGUI.actionKey && useShiftAsActionKey);
			bool flag2 = EditorGUI.actionKey && !useShiftAsActionKey;
			if (!allowMultiSelection)
			{
				flag2 = (flag = false);
			}
			List<int> result;
			if (flag2)
			{
				list.AddRange(selectedInstanceIDs);
				if (list.Contains(clickedInstanceID))
				{
					list.Remove(clickedInstanceID);
				}
				else
				{
					list.Add(clickedInstanceID);
				}
			}
			else if (flag)
			{
				if (clickedInstanceID == lastClickedInstanceID)
				{
					list.AddRange(selectedInstanceIDs);
					result = list;
					return result;
				}
				int num;
				int num2;
				if (!InternalEditorUtility.GetFirstAndLastSelected(allInstanceIDs, selectedInstanceIDs, out num, out num2))
				{
					list.Add(clickedInstanceID);
					result = list;
					return result;
				}
				int num3 = -1;
				int num4 = -1;
				for (int i = 0; i < allInstanceIDs.Count; i++)
				{
					if (allInstanceIDs[i] == clickedInstanceID)
					{
						num3 = i;
					}
					if (lastClickedInstanceID != 0 && allInstanceIDs[i] == lastClickedInstanceID)
					{
						num4 = i;
					}
				}
				int num5 = 0;
				if (num4 != -1)
				{
					num5 = ((num3 <= num4) ? -1 : 1);
				}
				int num6;
				int num7;
				if (num3 > num2)
				{
					num6 = num;
					num7 = num3;
				}
				else if (num3 >= num && num3 < num2)
				{
					if (num5 > 0)
					{
						num6 = num3;
						num7 = num2;
					}
					else
					{
						num6 = num;
						num7 = num3;
					}
				}
				else
				{
					num6 = num3;
					num7 = num2;
				}
				for (int j = num6; j <= num7; j++)
				{
					list.Add(allInstanceIDs[j]);
				}
			}
			else
			{
				if (keepMultiSelection)
				{
					if (selectedInstanceIDs.Contains(clickedInstanceID))
					{
						list.AddRange(selectedInstanceIDs);
						result = list;
						return result;
					}
				}
				list.Add(clickedInstanceID);
			}
			result = list;
			return result;
		}

		private static bool GetFirstAndLastSelected(List<int> allInstanceIDs, List<int> selectedInstanceIDs, out int firstIndex, out int lastIndex)
		{
			firstIndex = -1;
			lastIndex = -1;
			for (int i = 0; i < allInstanceIDs.Count; i++)
			{
				if (selectedInstanceIDs.Contains(allInstanceIDs[i]))
				{
					if (firstIndex == -1)
					{
						firstIndex = i;
					}
					lastIndex = i;
				}
			}
			return firstIndex != -1 && lastIndex != -1;
		}

		internal static string GetApplicationExtensionForRuntimePlatform(RuntimePlatform platform)
		{
			string result;
			if (platform != RuntimePlatform.OSXEditor)
			{
				if (platform != RuntimePlatform.WindowsEditor)
				{
					result = string.Empty;
				}
				else
				{
					result = "exe";
				}
			}
			else
			{
				result = "app";
			}
			return result;
		}

		public static bool IsValidFileName(string filename)
		{
			string text = InternalEditorUtility.RemoveInvalidCharsFromFileName(filename, false);
			return !(text != filename) && !string.IsNullOrEmpty(text);
		}

		public static string RemoveInvalidCharsFromFileName(string filename, bool logIfInvalidChars)
		{
			string result;
			if (string.IsNullOrEmpty(filename))
			{
				result = filename;
			}
			else
			{
				filename = filename.Trim();
				if (string.IsNullOrEmpty(filename))
				{
					result = filename;
				}
				else
				{
					string text = new string(Path.GetInvalidFileNameChars());
					string text2 = "";
					bool flag = false;
					string text3 = filename;
					for (int i = 0; i < text3.Length; i++)
					{
						char c = text3[i];
						if (text.IndexOf(c) == -1)
						{
							text2 += c;
						}
						else
						{
							flag = true;
						}
					}
					if (flag && logIfInvalidChars)
					{
						string displayStringOfInvalidCharsOfFileName = InternalEditorUtility.GetDisplayStringOfInvalidCharsOfFileName(filename);
						if (displayStringOfInvalidCharsOfFileName.Length > 0)
						{
							Debug.LogWarningFormat("A filename cannot contain the following character{0}:  {1}", new object[]
							{
								(displayStringOfInvalidCharsOfFileName.Length <= 1) ? "" : "s",
								displayStringOfInvalidCharsOfFileName
							});
						}
					}
					result = text2;
				}
			}
			return result;
		}

		public static string GetDisplayStringOfInvalidCharsOfFileName(string filename)
		{
			string result;
			if (string.IsNullOrEmpty(filename))
			{
				result = "";
			}
			else
			{
				string text = new string(Path.GetInvalidFileNameChars());
				string text2 = "";
				for (int i = 0; i < filename.Length; i++)
				{
					char c = filename[i];
					if (text.IndexOf(c) >= 0)
					{
						if (text2.IndexOf(c) == -1)
						{
							if (text2.Length > 0)
							{
								text2 += " ";
							}
							text2 += c;
						}
					}
				}
				result = text2;
			}
			return result;
		}

		internal static bool IsScriptOrAssembly(string filename)
		{
			bool result;
			if (string.IsNullOrEmpty(filename))
			{
				result = false;
			}
			else
			{
				string text = Path.GetExtension(filename).ToLower();
				if (text != null)
				{
					if (text == ".cs" || text == ".js" || text == ".boo")
					{
						result = true;
						return result;
					}
					if (text == ".dll" || text == ".exe")
					{
						result = AssemblyHelper.IsManagedAssembly(filename);
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		internal static T ParentHasComponent<T>(Transform trans) where T : Component
		{
			T result;
			if (trans != null)
			{
				T component = trans.GetComponent<T>();
				if (component)
				{
					result = component;
				}
				else
				{
					result = InternalEditorUtility.ParentHasComponent<T>(trans.parent);
				}
			}
			else
			{
				result = (T)((object)null);
			}
			return result;
		}

		internal static IEnumerable<string> GetAllScriptGUIDs()
		{
			return from asset in AssetDatabase.GetAllAssetPaths()
			where InternalEditorUtility.IsScriptOrAssembly(asset)
			select AssetDatabase.AssetPathToGUID(asset);
		}
	}
}
