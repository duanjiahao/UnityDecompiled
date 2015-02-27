using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace UnityEditor
{
	internal static class SpriteUtility
	{
		public static void OnSceneDrag(SceneView sceneView)
		{
			Event current = Event.current;
			if (current.type != EventType.DragUpdated && current.type != EventType.DragPerform && current.type != EventType.DragExited)
			{
				return;
			}
			Sprite[] spriteFromDraggedPathsOrObjects = SpriteUtility.GetSpriteFromDraggedPathsOrObjects();
			if (spriteFromDraggedPathsOrObjects.Length == 0)
			{
				return;
			}
			Sprite sprite = spriteFromDraggedPathsOrObjects[0];
			if (sprite == null)
			{
				return;
			}
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			EventType type = current.type;
			if (type == EventType.DragPerform)
			{
				Vector3 point = HandleUtility.GUIPointToWorldRay(current.mousePosition).GetPoint(10f);
				point.z = 0f;
				GameObject objectToUndo = SpriteUtility.DropFramesToSceneToCreateGO(sprite.name, spriteFromDraggedPathsOrObjects, point);
				Undo.RegisterCreatedObjectUndo(objectToUndo, "Create Sprite");
				current.Use();
			}
		}
		private static bool CreateAnimation(GameObject gameObject, UnityEngine.Object[] frames)
		{
			Array.Sort<UnityEngine.Object>(frames, (UnityEngine.Object a, UnityEngine.Object b) => EditorUtility.NaturalCompare(a.name, b.name));
			string message = string.Format("Create a new animation for the game object '{0}':", gameObject.name);
			string directoryName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(frames[0]));
			string text = EditorUtility.SaveFilePanelInProject("Create New Animation", "New Animation", "anim", message, directoryName);
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			AnimationClip animationClip = AnimationSelection.AllocateAndSetupClip(true);
			AssetDatabase.CreateAsset(animationClip, text);
			AnimationSelection.AddClipToAnimatorComponent(gameObject, animationClip);
			animationClip.frameRate = 12f;
			ObjectReferenceKeyframe[] array = new ObjectReferenceKeyframe[frames.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = default(ObjectReferenceKeyframe);
				array[i].value = SpriteUtility.RemapObjectToSprite(frames[i]);
				array[i].time = (float)i / animationClip.frameRate;
			}
			EditorCurveBinding binding = EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite");
			AnimationUtility.SetObjectReferenceCurve(animationClip, binding, array);
			return true;
		}
		public static Sprite[] GetSpriteFromDraggedPathsOrObjects()
		{
			List<Sprite> list = new List<Sprite>();
			UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
			for (int i = 0; i < objectReferences.Length; i++)
			{
				UnityEngine.Object @object = objectReferences[i];
				if (AssetDatabase.Contains(@object))
				{
					if (@object is Sprite)
					{
						list.Add(@object as Sprite);
					}
					else
					{
						if (@object is Texture2D)
						{
							list.Add(SpriteUtility.TextureToSprite(@object as Texture2D));
						}
					}
				}
			}
			if (list.Count > 0)
			{
				return list.ToArray();
			}
			return new Sprite[]
			{
				SpriteUtility.HandleExternalDrag(Event.current.type == EventType.DragPerform)
			};
		}
		public static Sprite[] GetSpritesFromDraggedObjects()
		{
			List<Sprite> list = new List<Sprite>();
			UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
			for (int i = 0; i < objectReferences.Length; i++)
			{
				UnityEngine.Object @object = objectReferences[i];
				if (@object.GetType() == typeof(Sprite))
				{
					list.Add(@object as Sprite);
				}
				else
				{
					if (@object.GetType() == typeof(Texture2D))
					{
						Sprite sprite = SpriteUtility.TextureToSprite(@object as Texture2D);
						if (sprite != null)
						{
							list.Add(sprite);
						}
					}
				}
			}
			return list.ToArray();
		}
		private static Sprite HandleExternalDrag(bool perform)
		{
			if (DragAndDrop.paths.Length == 0)
			{
				return null;
			}
			string text = DragAndDrop.paths[0];
			if (!SpriteUtility.ValidPathForTextureAsset(text))
			{
				return null;
			}
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			if (!perform)
			{
				return null;
			}
			string text2 = AssetDatabase.GenerateUniqueAssetPath(Path.Combine("Assets", FileUtil.GetLastPathNameComponent(text)));
			if (text2.Length <= 0)
			{
				return null;
			}
			FileUtil.CopyFileOrDirectory(text, text2);
			SpriteUtility.ForcedImportFor(text2);
			return SpriteUtility.GenerateDefaultSprite(AssetDatabase.LoadMainAssetAtPath(text2) as Texture2D);
		}
		public static bool HandleMultipleSpritesDragIntoHierarchy(IHierarchyProperty property, Sprite[] sprites, bool perform)
		{
			GameObject gameObject = null;
			if (property == null || property.pptrValue == null)
			{
				if (perform)
				{
					Analytics.Event("Sprite Drag and Drop", "Drop multiple sprites to empty hierarchy", "null", 1);
					gameObject = new GameObject();
					gameObject.name = sprites[0].name;
					gameObject.transform.position = SpriteUtility.GetDefaultInstantiatePosition();
				}
			}
			else
			{
				UnityEngine.Object pptrValue = property.pptrValue;
				gameObject = (pptrValue as GameObject);
				if (perform)
				{
					Analytics.Event("Sprite Drag and Drop", "Drop multiple sprites to gameobject", "null", 1);
				}
			}
			if (perform)
			{
				SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
				if (spriteRenderer == null)
				{
					spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
				}
				if (spriteRenderer == null)
				{
					return true;
				}
				if (spriteRenderer.sprite == null)
				{
					spriteRenderer.sprite = sprites[0];
				}
				SpriteUtility.CreateAnimation(gameObject, sprites);
				Selection.activeGameObject = gameObject;
			}
			return true;
		}
		public static bool HandleSingleSpriteDragIntoHierarchy(IHierarchyProperty property, Sprite sprite, bool perform)
		{
			GameObject gameObject = null;
			if (property != null && property.pptrValue != null)
			{
				UnityEngine.Object pptrValue = property.pptrValue;
				gameObject = (pptrValue as GameObject);
			}
			if (perform)
			{
				Vector3 defaultInstantiatePosition = SpriteUtility.GetDefaultInstantiatePosition();
				GameObject gameObject2 = SpriteUtility.DropSpriteToSceneToCreateGO(sprite.name, sprite, defaultInstantiatePosition);
				if (gameObject != null)
				{
					Analytics.Event("Sprite Drag and Drop", "Drop single sprite to existing gameobject", "null", 1);
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.transform.localPosition = Vector3.zero;
				}
				else
				{
					Analytics.Event("Sprite Drag and Drop", "Drop single sprite to empty hierarchy", "null", 1);
				}
				Selection.activeGameObject = gameObject2;
			}
			return true;
		}
		private static Vector3 GetDefaultInstantiatePosition()
		{
			Vector3 result = Vector3.zero;
			if (SceneView.lastActiveSceneView)
			{
				if (SceneView.lastActiveSceneView.in2DMode)
				{
					result = SceneView.lastActiveSceneView.camera.transform.position;
					result.z = 0f;
				}
				else
				{
					result = SceneView.lastActiveSceneView.cameraTargetPosition;
				}
			}
			return result;
		}
		private static void ForcedImportFor(string newPath)
		{
			try
			{
				AssetDatabase.StartAssetEditing();
				AssetDatabase.ImportAsset(newPath);
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
			}
		}
		private static Sprite GenerateDefaultSprite(Texture2D texture)
		{
			string assetPath = AssetDatabase.GetAssetPath(texture);
			TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter.textureType != TextureImporterType.Sprite && textureImporter.textureType != TextureImporterType.Advanced)
			{
				return null;
			}
			if (textureImporter.spriteImportMode == SpriteImportMode.None)
			{
				if (textureImporter.textureType == TextureImporterType.Advanced)
				{
					return null;
				}
				textureImporter.spriteImportMode = SpriteImportMode.Single;
				AssetDatabase.WriteImportSettingsIfDirty(assetPath);
				SpriteUtility.ForcedImportFor(assetPath);
			}
			UnityEngine.Object @object = null;
			try
			{
				@object = AssetDatabase.LoadAllAssetsAtPath(assetPath).First((UnityEngine.Object t) => t is Sprite);
			}
			catch (Exception)
			{
				Debug.LogWarning("Texture being dragged has no Sprites.");
			}
			return @object as Sprite;
		}
		public static GameObject DropFramesToSceneToCreateGO(string name, Sprite[] frames, Vector3 position)
		{
			if (frames.Length > 0)
			{
				Sprite sprite = frames[0];
				GameObject gameObject = new GameObject(name);
				SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = sprite;
				gameObject.transform.position = position;
				Selection.activeObject = gameObject;
				if (frames.Length > 1)
				{
					Analytics.Event("Sprite Drag and Drop", "Drop multiple sprites to scene", "null", 1);
					if (!SpriteUtility.CreateAnimation(gameObject, frames))
					{
						UnityEngine.Object.DestroyImmediate(gameObject);
						return null;
					}
				}
				else
				{
					Analytics.Event("Sprite Drag and Drop", "Drop single sprite to scene", "null", 1);
				}
				return gameObject;
			}
			return null;
		}
		public static GameObject DropSpriteToSceneToCreateGO(string name, Sprite sprite, Vector3 position)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.name = sprite.name;
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprite;
			gameObject.transform.position = position;
			Selection.activeObject = gameObject;
			return gameObject;
		}
		public static Sprite RemapObjectToSprite(UnityEngine.Object obj)
		{
			if (obj is Sprite)
			{
				return (Sprite)obj;
			}
			if (obj is Texture2D)
			{
				UnityEngine.Object[] array = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj));
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].GetType() == typeof(Sprite))
					{
						return array[i] as Sprite;
					}
				}
			}
			return null;
		}
		public static Sprite TextureToSprite(Texture2D tex)
		{
			UnityEngine.Object[] array = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tex));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetType() == typeof(Sprite))
				{
					return array[i] as Sprite;
				}
			}
			return SpriteUtility.GenerateDefaultSprite(tex);
		}
		private static bool ValidPathForTextureAsset(string path)
		{
			string a = FileUtil.GetPathExtension(path).ToLower();
			return a == "jpg" || a == "jpeg" || a == "tif" || a == "tiff" || a == "tga" || a == "gif" || a == "png" || a == "psd" || a == "bmp" || a == "iff" || a == "pict" || a == "pic" || a == "pct" || a == "exr";
		}
	}
}
