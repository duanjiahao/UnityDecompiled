using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditorInternal
{
	public sealed class InternalEditorUtility
	{
		public enum HierarchyDropMode
		{
			kHierarchyDragNormal,
			kHierarchyDropUpon,
			kHierarchyDropBetween,
			kHierarchyDropAfterParent = 4
		}

		public static extern bool isApplicationActive
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool inBatchMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isHumanControllingUs
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int[] expandedProjectWindowItems
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string[] tags
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string[] layers
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern string[] sortingLayerNames
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern int[] sortingLayerUniqueIDs
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string unityPreferencesFolder
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float defaultScreenWidth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float defaultScreenHeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float defaultWebScreenWidth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float defaultWebScreenHeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float remoteScreenWidth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float remoteScreenHeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool ignoreInspectorChanges
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BumpMapSettingsFixingWindowReportResult(int result);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BumpMapTextureNeedsFixingInternal(Material material, string propName, bool flaggedAsNormal);

		internal static bool BumpMapTextureNeedsFixing(MaterialProperty prop)
		{
			if (prop.type != MaterialProperty.PropType.Texture)
			{
				return false;
			}
			bool flaggedAsNormal = (prop.flags & MaterialProperty.PropFlags.Normal) != MaterialProperty.PropFlags.None;
			UnityEngine.Object[] targets = prop.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				if (InternalEditorUtility.BumpMapTextureNeedsFixingInternal(material, prop.name, flaggedAsNormal))
				{
					return true;
				}
			}
			return false;
		}

		[WrapperlessIcall]
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

		internal static bool HDRTextureNeedsFixing(MaterialProperty prop, out bool canBeFixedAutomatically)
		{
			if ((prop.flags & MaterialProperty.PropFlags.HDR) != MaterialProperty.PropFlags.None || prop.displayName.Contains("(HDR"))
			{
				Texture textureValue = prop.textureValue;
				if (textureValue)
				{
					string assetPath = AssetDatabase.GetAssetPath(textureValue);
					TextureImporter x = AssetImporter.GetAtPath(assetPath) as TextureImporter;
					canBeFixedAutomatically = (x != null);
					bool flag = TextureUtil.HasAlphaTextureFormat(TextureUtil.GetTextureFormat(textureValue));
					bool flag2 = TextureUtil.GetUsageMode(textureValue) == TextureUsageMode.RGBMEncoded;
					if (flag && !flag2)
					{
						return true;
					}
				}
			}
			canBeFixedAutomatically = false;
			return false;
		}

		internal static void FixHDRTexture(MaterialProperty prop)
		{
			string assetPath = AssetDatabase.GetAssetPath(prop.textureValue);
			TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (!textureImporter)
			{
				return;
			}
			TextureImporterFormat textureFormat = TextureImporterFormat.RGB24;
			textureImporter.textureFormat = textureFormat;
			List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
			foreach (BuildPlayerWindow.BuildPlatform current in validPlatforms)
			{
				int maxTextureSize;
				TextureImporterFormat textureImporterFormat;
				int compressionQuality;
				bool platformTextureSettings = textureImporter.GetPlatformTextureSettings(current.name, out maxTextureSize, out textureImporterFormat, out compressionQuality);
				if (platformTextureSettings)
				{
					textureImporter.SetPlatformTextureSettings(current.name, maxTextureSize, textureFormat, compressionQuality, false);
				}
			}
			AssetDatabase.ImportAsset(assetPath);
			UnityEngine.Object[] targets = prop.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				UnityEngine.Object dirty = targets[i];
				EditorUtility.SetDirty(dirty);
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetEditorAssemblyPath();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetEngineAssemblyPath();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string CalculateHashForObjectsAndDependencies(UnityEngine.Object[] objects);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExecuteCommandOnKeyWindow(string commandName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Material[] InstantiateMaterialsInEditMode(Renderer renderer);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern CanAppendBuild BuildCanBeAppended(BuildTarget target, string location);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RegisterExtensionDll(string dllLocation, string guid);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetupCustomDll(string dllName, string dllLocation);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly LoadAssemblyWrapper(string dllName, string dllLocation);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPlatformPath(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int AddScriptComponentUncheckedUndoable(GameObject gameObject, MonoScript script);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int CreateScriptableObjectUnchecked(MonoScript script);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RequestScriptReload();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SwitchSkinAndRepaintAllViews();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RepaintAllViews();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetIsInspectorExpanded(UnityEngine.Object obj);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetIsInspectorExpanded(UnityEngine.Object obj, bool isExpanded);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SaveToSerializedFileAndForget(UnityEngine.Object[] obj, string path, bool allowTextSerialization);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object[] LoadSerializedFileAndForget(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern DragAndDropVisualMode ProjectWindowDrag(HierarchyProperty property, bool perform);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern DragAndDropVisualMode HierarchyWindowDrag(HierarchyProperty property, bool perform, InternalEditorUtility.HierarchyDropMode dropMode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern DragAndDropVisualMode InspectorWindowDrag(UnityEngine.Object[] targets, bool perform);

		public static DragAndDropVisualMode SceneViewDrag(UnityEngine.Object dropUpon, Vector3 worldPosition, Vector2 viewportPosition, bool perform)
		{
			return InternalEditorUtility.INTERNAL_CALL_SceneViewDrag(dropUpon, ref worldPosition, ref viewportPosition, perform);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DragAndDropVisualMode INTERNAL_CALL_SceneViewDrag(UnityEngine.Object dropUpon, ref Vector3 worldPosition, ref Vector2 viewportPosition, bool perform);

		public static void SetRectTransformTemporaryRect(RectTransform rectTransform, Rect rect)
		{
			InternalEditorUtility.INTERNAL_CALL_SetRectTransformTemporaryRect(rectTransform, ref rect);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetRectTransformTemporaryRect(RectTransform rectTransform, ref Rect rect);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasTeamLicense();

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasPro();

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasFreeLicense();

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasEduLicense();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasAdvancedLicenseOnBuildTarget(BuildTarget target);

		public static Rect GetBoundsOfDesktopAtPoint(Vector2 pos)
		{
			Rect result;
			InternalEditorUtility.INTERNAL_CALL_GetBoundsOfDesktopAtPoint(ref pos, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetBoundsOfDesktopAtPoint(ref Vector2 pos, out Rect value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetSortingLayerName(int index);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSortingLayerUniqueID(int index);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetSortingLayerNameFromUniqueID(int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSortingLayerCount();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetSortingLayerName(int index, string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetSortingLayerLocked(int index, bool locked);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetSortingLayerLocked(int index);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsSortingLayerDefault(int index);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void AddSortingLayer();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateSortingLayersOrder();

		public static Vector4 GetSpriteOuterUV(Sprite sprite, bool getAtlasData)
		{
			Vector4 result;
			InternalEditorUtility.INTERNAL_CALL_GetSpriteOuterUV(sprite, getAtlasData, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSpriteOuterUV(Sprite sprite, bool getAtlasData, out Vector4 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetObjectFromInstanceID(int instanceID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetClassIDWithoutLoadingObject(int instanceID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetLoadedObjectFromInstanceID(int instanceID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetLayerName(int layer);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetExternalScriptEditor();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetExternalScriptEditorArgs();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReloadWindowLayoutMenu();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RevertFactoryLayoutSettings(bool quitOnCancel);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadDefaultLayout();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CalculateAmbientProbeFromSkybox();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetupShaderMenu(Material material);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUnityVersionFull();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetFullUnityVersion();

		public static Version GetUnityVersion()
		{
			Version version = new Version(InternalEditorUtility.GetUnityVersionDigits());
			return new Version(version.Major, version.Minor, version.Build, InternalEditorUtility.GetUnityRevision());
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUnityVersionDigits();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUnityBuildBranch();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetUnityVersionDate();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetUnityRevision();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsUnityBeta();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUnityCopyright();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetLicenseInfo();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int[] GetLicenseFlags();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAuthToken();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OpenEditorConsole();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetGameObjectInstanceIDFromComponent(int instanceID);

		public static Color[] ReadScreenPixel(Vector2 pixelPos, int sizex, int sizey)
		{
			return InternalEditorUtility.INTERNAL_CALL_ReadScreenPixel(ref pixelPos, sizex, sizey);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color[] INTERNAL_CALL_ReadScreenPixel(ref Vector2 pixelPos, int sizex, int sizey);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OpenPlayerConsole();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Resolution GetDesktopResolution();

		public static string TextifyEvent(Event evt)
		{
			if (evt == null)
			{
				return "none";
			}
			KeyCode keyCode = evt.keyCode;
			string str;
			switch (keyCode)
			{
			case KeyCode.Keypad0:
				str = "[0]";
				goto IL_2E8;
			case KeyCode.Keypad1:
				str = "[1]";
				goto IL_2E8;
			case KeyCode.Keypad2:
				str = "[2]";
				goto IL_2E8;
			case KeyCode.Keypad3:
				str = "[3]";
				goto IL_2E8;
			case KeyCode.Keypad4:
				str = "[4]";
				goto IL_2E8;
			case KeyCode.Keypad5:
				str = "[5]";
				goto IL_2E8;
			case KeyCode.Keypad6:
				str = "[6]";
				goto IL_2E8;
			case KeyCode.Keypad7:
				str = "[7]";
				goto IL_2E8;
			case KeyCode.Keypad8:
				str = "[8]";
				goto IL_2E8;
			case KeyCode.Keypad9:
				str = "[9]";
				goto IL_2E8;
			case KeyCode.KeypadPeriod:
				str = "[.]";
				goto IL_2E8;
			case KeyCode.KeypadDivide:
				str = "[/]";
				goto IL_2E8;
			case KeyCode.KeypadMultiply:
				IL_C5:
				if (keyCode == KeyCode.Backspace)
				{
					str = "backspace";
					goto IL_2E8;
				}
				if (keyCode == KeyCode.Return)
				{
					str = "return";
					goto IL_2E8;
				}
				if (keyCode == KeyCode.Escape)
				{
					str = "[esc]";
					goto IL_2E8;
				}
				if (keyCode != KeyCode.Delete)
				{
					str = string.Empty + evt.keyCode;
					goto IL_2E8;
				}
				str = "delete";
				goto IL_2E8;
			case KeyCode.KeypadMinus:
				str = "[-]";
				goto IL_2E8;
			case KeyCode.KeypadPlus:
				str = "[+]";
				goto IL_2E8;
			case KeyCode.KeypadEnter:
				str = "enter";
				goto IL_2E8;
			case KeyCode.KeypadEquals:
				str = "[=]";
				goto IL_2E8;
			case KeyCode.UpArrow:
				str = "up";
				goto IL_2E8;
			case KeyCode.DownArrow:
				str = "down";
				goto IL_2E8;
			case KeyCode.RightArrow:
				str = "right";
				goto IL_2E8;
			case KeyCode.LeftArrow:
				str = "left";
				goto IL_2E8;
			case KeyCode.Insert:
				str = "insert";
				goto IL_2E8;
			case KeyCode.Home:
				str = "home";
				goto IL_2E8;
			case KeyCode.End:
				str = "end";
				goto IL_2E8;
			case KeyCode.PageUp:
				str = "page up";
				goto IL_2E8;
			case KeyCode.PageDown:
				str = "page down";
				goto IL_2E8;
			case KeyCode.F1:
				str = "F1";
				goto IL_2E8;
			case KeyCode.F2:
				str = "F2";
				goto IL_2E8;
			case KeyCode.F3:
				str = "F3";
				goto IL_2E8;
			case KeyCode.F4:
				str = "F4";
				goto IL_2E8;
			case KeyCode.F5:
				str = "F5";
				goto IL_2E8;
			case KeyCode.F6:
				str = "F6";
				goto IL_2E8;
			case KeyCode.F7:
				str = "F7";
				goto IL_2E8;
			case KeyCode.F8:
				str = "F8";
				goto IL_2E8;
			case KeyCode.F9:
				str = "F9";
				goto IL_2E8;
			case KeyCode.F10:
				str = "F10";
				goto IL_2E8;
			case KeyCode.F11:
				str = "F11";
				goto IL_2E8;
			case KeyCode.F12:
				str = "F12";
				goto IL_2E8;
			case KeyCode.F13:
				str = "F13";
				goto IL_2E8;
			case KeyCode.F14:
				str = "F14";
				goto IL_2E8;
			case KeyCode.F15:
				str = "F15";
				goto IL_2E8;
			}
			goto IL_C5;
			IL_2E8:
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
			return str2 + str;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAvailableDiffTools();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetNoDiffToolsDetectedMessage();

		public static Bounds TransformBounds(Bounds b, Transform t)
		{
			Bounds result;
			InternalEditorUtility.INTERNAL_CALL_TransformBounds(ref b, t, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_TransformBounds(ref Bounds b, Transform t, out Bounds value);

		public static void SetCustomLighting(Light[] lights, Color ambient)
		{
			InternalEditorUtility.INTERNAL_CALL_SetCustomLighting(lights, ref ambient);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetCustomLighting(Light[] lights, ref Color ambient);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ClearSceneLighting();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RemoveCustomLighting();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DrawSkyboxMaterial(Material mat, Camera cam);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasFullscreenCamera();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetCursor();

		public static Bounds CalculateSelectionBounds(bool usePivotOnlyForParticles, bool onlyUseActiveSelection)
		{
			Bounds result;
			InternalEditorUtility.INTERNAL_CALL_CalculateSelectionBounds(usePivotOnlyForParticles, onlyUseActiveSelection, out result);
			return result;
		}

		[WrapperlessIcall]
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
			if (component)
			{
				return new Bounds(component.rect.center, component.rect.size);
			}
			Renderer component2 = gameObject.GetComponent<Renderer>();
			if (component2 is MeshRenderer)
			{
				MeshFilter component3 = component2.GetComponent<MeshFilter>();
				if (component3 != null && component3.sharedMesh != null)
				{
					return component3.sharedMesh.bounds;
				}
			}
			if (component2 is SpriteRenderer)
			{
				return ((SpriteRenderer)component2).GetSpriteBounds();
			}
			return new Bounds(Vector3.zero, Vector3.zero);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OnGameViewFocus(bool focus);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool OpenFileAtLineExternal(string filename, int line);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MonoIsland[] GetMonoIslands();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Xbox360GenerateSPAConfig(string spaPath);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Xbox360SaveSplashScreenToFile(Texture2D image, string spaPath);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool WiiUSaveStartupScreenToFile(Texture2D image, string path, int outputWidth, int outputHeight);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CanConnectToCacheServer();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong VerifyCacheServerIntegrity();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong FixCacheServerIntegrityErrors();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern DllType DetectDotNetDll(string path);

		public static bool IsDotNet4Dll(string path)
		{
			DllType dllType = InternalEditorUtility.DetectDotNetDll(path);
			switch (dllType)
			{
			case DllType.Unknown:
			case DllType.Native:
			case DllType.UnknownManaged:
			case DllType.ManagedNET35:
				return false;
			case DllType.ManagedNET40:
			case DllType.WinMDNative:
			case DllType.WinMDNET40:
				return true;
			default:
				throw new Exception(string.Format("Unknown dll type: {0}", dllType));
			}
		}

		[WrapperlessIcall]
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
			if (Application.platform != RuntimePlatform.WindowsEditor)
			{
				return false;
			}
			OperatingSystem oSVersion = Environment.OSVersion;
			int major = oSVersion.Version.Major;
			int minor = oSVersion.Version.Minor;
			if (orHigher)
			{
				return major > 6 || (major == 6 && minor >= 2);
			}
			return major == 6 && minor == 2;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int DetermineDepthOrder(Transform lhs, Transform rhs);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ShowPackageManagerWindow();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void AuxWindowManager_OnAssemblyReload();

		public static Vector2 PassAndReturnVector2(Vector2 v)
		{
			Vector2 result;
			InternalEditorUtility.INTERNAL_CALL_PassAndReturnVector2(ref v, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_PassAndReturnVector2(ref Vector2 v, out Vector2 value);

		public static Color32 PassAndReturnColor32(Color32 c)
		{
			Color32 result;
			InternalEditorUtility.INTERNAL_CALL_PassAndReturnColor32(ref c, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_PassAndReturnColor32(ref Color32 c, out Color32 value);

		[Obsolete("use EditorSceneManager.EnsureUntitledSceneHasBeenSaved"), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool EnsureSceneHasBeenSaved(string operation);

		public static Texture2D GetIconForFile(string fileName)
		{
			int num = fileName.LastIndexOf('.');
			string text = (num != -1) ? fileName.Substring(num + 1).ToLower() : string.Empty;
			string text2 = text;
			switch (text2)
			{
			case "boo":
				return EditorGUIUtility.FindTexture("boo Script Icon");
			case "cginc":
				return EditorGUIUtility.FindTexture("CGProgram Icon");
			case "cs":
				return EditorGUIUtility.FindTexture("cs Script Icon");
			case "guiskin":
				return EditorGUIUtility.FindTexture("GUISkin Icon");
			case "js":
				return EditorGUIUtility.FindTexture("Js Script Icon");
			case "mat":
				return EditorGUIUtility.FindTexture("Material Icon");
			case "physicmaterial":
				return EditorGUIUtility.FindTexture("PhysicMaterial Icon");
			case "prefab":
				return EditorGUIUtility.FindTexture("PrefabNormal Icon");
			case "shader":
				return EditorGUIUtility.FindTexture("Shader Icon");
			case "txt":
				return EditorGUIUtility.FindTexture("TextAsset Icon");
			case "unity":
				return EditorGUIUtility.FindTexture("SceneAsset Icon");
			case "asset":
			case "prefs":
				return EditorGUIUtility.FindTexture("GameManager Icon");
			case "anim":
				return EditorGUIUtility.FindTexture("Animation Icon");
			case "meta":
				return EditorGUIUtility.FindTexture("MetaFile Icon");
			case "mixer":
				return EditorGUIUtility.FindTexture("AudioMixerController Icon");
			case "ttf":
			case "otf":
			case "fon":
			case "fnt":
				return EditorGUIUtility.FindTexture("Font Icon");
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
				return EditorGUIUtility.FindTexture("AudioClip Icon");
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
				return EditorGUIUtility.FindTexture("Texture Icon");
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
				return EditorGUIUtility.FindTexture("Mesh Icon");
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
				return EditorGUIUtility.FindTexture("MovieTexture Icon");
			case "colors":
			case "gradients":
			case "curves":
			case "curvesnormalized":
			case "particlecurves":
			case "particlecurvessigned":
			case "particledoublecurves":
			case "particledoublecurvessigned":
				return EditorGUIUtility.FindTexture("ScriptableObject Icon");
			}
			return EditorGUIUtility.FindTexture("DefaultAsset Icon");
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
					return selectedInstanceIDs;
				}
				int num;
				int num2;
				if (!InternalEditorUtility.GetFirstAndLastSelected(allInstanceIDs, selectedInstanceIDs, out num, out num2))
				{
					list.Add(clickedInstanceID);
					return list;
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
				if (keepMultiSelection && selectedInstanceIDs.Contains(clickedInstanceID))
				{
					list.AddRange(selectedInstanceIDs);
					return list;
				}
				list.Add(clickedInstanceID);
			}
			return list;
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

		public static bool IsValidFileName(string filename)
		{
			string text = InternalEditorUtility.RemoveInvalidCharsFromFileName(filename, false);
			return !(text != filename) && !string.IsNullOrEmpty(text);
		}

		public static string RemoveInvalidCharsFromFileName(string filename, bool logIfInvalidChars)
		{
			if (string.IsNullOrEmpty(filename))
			{
				return filename;
			}
			filename = filename.Trim();
			if (string.IsNullOrEmpty(filename))
			{
				return filename;
			}
			string text = new string(Path.GetInvalidFileNameChars());
			string text2 = string.Empty;
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
						(displayStringOfInvalidCharsOfFileName.Length <= 1) ? string.Empty : "s",
						displayStringOfInvalidCharsOfFileName
					});
				}
			}
			return text2;
		}

		public static string GetDisplayStringOfInvalidCharsOfFileName(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				return string.Empty;
			}
			string text = new string(Path.GetInvalidFileNameChars());
			string text2 = string.Empty;
			for (int i = 0; i < filename.Length; i++)
			{
				char c = filename[i];
				if (text.IndexOf(c) >= 0 && text2.IndexOf(c) == -1)
				{
					if (text2.Length > 0)
					{
						text2 += " ";
					}
					text2 += c;
				}
			}
			return text2;
		}

		internal static bool IsScriptOrAssembly(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				return false;
			}
			string text = Path.GetExtension(filename).ToLower();
			if (text != null)
			{
				if (InternalEditorUtility.<>f__switch$map8 == null)
				{
					InternalEditorUtility.<>f__switch$map8 = new Dictionary<string, int>(5)
					{
						{
							".cs",
							0
						},
						{
							".js",
							0
						},
						{
							".boo",
							0
						},
						{
							".dll",
							1
						},
						{
							".exe",
							1
						}
					};
				}
				int num;
				if (InternalEditorUtility.<>f__switch$map8.TryGetValue(text, out num))
				{
					if (num == 0)
					{
						return true;
					}
					if (num == 1)
					{
						return AssemblyHelper.IsManagedAssembly(filename);
					}
				}
			}
			return false;
		}

		internal static T ParentHasComponent<T>(Transform trans) where T : Component
		{
			if (!(trans != null))
			{
				return (T)((object)null);
			}
			T component = trans.GetComponent<T>();
			if (component)
			{
				return component;
			}
			return InternalEditorUtility.ParentHasComponent<T>(trans.parent);
		}

		internal static IEnumerable<string> GetAllScriptGUIDs()
		{
			return from asset in AssetDatabase.GetAllAssetPaths()
			where InternalEditorUtility.IsScriptOrAssembly(asset)
			select AssetDatabase.AssetPathToGUID(asset);
		}
	}
}
