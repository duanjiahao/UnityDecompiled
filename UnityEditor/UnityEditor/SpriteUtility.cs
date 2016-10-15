using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal static class SpriteUtility
	{
		private enum DragType
		{
			NotInitialized,
			SpriteAnimation,
			CreateMultiple
		}

		private static List<UnityEngine.Object> s_SceneDragObjects;

		private static SpriteUtility.DragType s_DragType;

		public static void OnSceneDrag(SceneView sceneView)
		{
			Event current = Event.current;
			if (current.type != EventType.DragUpdated && current.type != EventType.DragPerform && current.type != EventType.DragExited)
			{
				return;
			}
			if (!sceneView.in2DMode)
			{
				GameObject gameObject = HandleUtility.PickGameObject(Event.current.mousePosition, true);
				if (gameObject != null && DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0] as Texture != null && gameObject.GetComponent<Renderer>() != null)
				{
					SpriteUtility.CleanUp(true);
					return;
				}
			}
			EventType type = current.type;
			if (type != EventType.DragUpdated)
			{
				if (type != EventType.DragPerform)
				{
					if (type == EventType.DragExited)
					{
						if (SpriteUtility.s_SceneDragObjects != null && SpriteUtility.s_SceneDragObjects != null)
						{
							SpriteUtility.CleanUp(true);
							current.Use();
						}
					}
				}
				else
				{
					Sprite[] spriteFromDraggedPathsOrObjects = SpriteUtility.GetSpriteFromDraggedPathsOrObjects();
					if (spriteFromDraggedPathsOrObjects != null && SpriteUtility.s_SceneDragObjects != null)
					{
						if (SpriteUtility.s_DragType == SpriteUtility.DragType.SpriteAnimation)
						{
							SpriteUtility.AddAnimationToGO((GameObject)SpriteUtility.s_SceneDragObjects[0], spriteFromDraggedPathsOrObjects);
						}
						using (List<UnityEngine.Object>.Enumerator enumerator = SpriteUtility.s_SceneDragObjects.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								GameObject gameObject2 = (GameObject)enumerator.Current;
								Undo.RegisterCreatedObjectUndo(gameObject2, "Create Sprite");
								gameObject2.hideFlags = HideFlags.None;
							}
						}
						Selection.objects = SpriteUtility.s_SceneDragObjects.ToArray();
						SpriteUtility.CleanUp(false);
						current.Use();
					}
				}
			}
			else
			{
				SpriteUtility.DragType dragType = (!current.alt) ? SpriteUtility.DragType.SpriteAnimation : SpriteUtility.DragType.CreateMultiple;
				if (SpriteUtility.s_DragType != dragType || SpriteUtility.s_SceneDragObjects == null)
				{
					Sprite[] spriteFromDraggedPathsOrObjects2 = SpriteUtility.GetSpriteFromDraggedPathsOrObjects();
					if (spriteFromDraggedPathsOrObjects2 == null || spriteFromDraggedPathsOrObjects2.Length == 0)
					{
						return;
					}
					Sprite x = spriteFromDraggedPathsOrObjects2[0];
					if (x == null)
					{
						return;
					}
					if (SpriteUtility.s_DragType != SpriteUtility.DragType.NotInitialized)
					{
						SpriteUtility.CleanUp(true);
					}
					SpriteUtility.s_DragType = dragType;
					SpriteUtility.s_SceneDragObjects = new List<UnityEngine.Object>();
					if (SpriteUtility.s_DragType == SpriteUtility.DragType.CreateMultiple)
					{
						Sprite[] array = spriteFromDraggedPathsOrObjects2;
						for (int i = 0; i < array.Length; i++)
						{
							Sprite frame = array[i];
							SpriteUtility.s_SceneDragObjects.Add(SpriteUtility.CreateDragGO(frame, Vector3.zero));
						}
					}
					else
					{
						SpriteUtility.s_SceneDragObjects.Add(SpriteUtility.CreateDragGO(spriteFromDraggedPathsOrObjects2[0], Vector3.zero));
					}
					List<Transform> list = new List<Transform>();
					using (List<UnityEngine.Object>.Enumerator enumerator2 = SpriteUtility.s_SceneDragObjects.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							GameObject gameObject3 = (GameObject)enumerator2.Current;
							list.AddRange(gameObject3.GetComponentsInChildren<Transform>());
							gameObject3.hideFlags = HideFlags.HideInHierarchy;
						}
					}
					HandleUtility.ignoreRaySnapObjects = list.ToArray();
				}
				Vector3 position = Vector3.zero;
				position = HandleUtility.GUIPointToWorldRay(current.mousePosition).GetPoint(10f);
				if (sceneView.in2DMode)
				{
					position.z = 0f;
				}
				else
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					object obj = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
					if (obj != null)
					{
						position = ((RaycastHit)obj).point;
					}
				}
				using (List<UnityEngine.Object>.Enumerator enumerator3 = SpriteUtility.s_SceneDragObjects.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						GameObject gameObject4 = (GameObject)enumerator3.Current;
						gameObject4.transform.position = position;
					}
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				current.Use();
			}
		}

		private static void CleanUp(bool deleteTempSceneObject)
		{
			if (SpriteUtility.s_SceneDragObjects != null)
			{
				if (deleteTempSceneObject)
				{
					using (List<UnityEngine.Object>.Enumerator enumerator = SpriteUtility.s_SceneDragObjects.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							GameObject obj = (GameObject)enumerator.Current;
							UnityEngine.Object.DestroyImmediate(obj, false);
						}
					}
				}
				SpriteUtility.s_SceneDragObjects.Clear();
				SpriteUtility.s_SceneDragObjects = null;
			}
			HandleUtility.ignoreRaySnapObjects = null;
			SpriteUtility.s_DragType = SpriteUtility.DragType.NotInitialized;
		}

		private static bool CreateAnimation(GameObject gameObject, UnityEngine.Object[] frames)
		{
			Array.Sort<UnityEngine.Object>(frames, (UnityEngine.Object a, UnityEngine.Object b) => EditorUtility.NaturalCompare(a.name, b.name));
			if (!AnimationWindowUtility.EnsureActiveAnimationPlayer(gameObject))
			{
				return false;
			}
			Animator closestAnimatorInParents = AnimationWindowUtility.GetClosestAnimatorInParents(gameObject.transform);
			if (closestAnimatorInParents == null)
			{
				return false;
			}
			AnimationClip animationClip = AnimationWindowUtility.CreateNewClip(gameObject.name);
			if (animationClip == null)
			{
				return false;
			}
			SpriteUtility.AddSpriteAnimationToClip(animationClip, frames);
			return AnimationWindowUtility.AddClipToAnimatorComponent(closestAnimatorInParents, animationClip);
		}

		private static void AddSpriteAnimationToClip(AnimationClip newClip, UnityEngine.Object[] frames)
		{
			newClip.frameRate = 12f;
			ObjectReferenceKeyframe[] array = new ObjectReferenceKeyframe[frames.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = default(ObjectReferenceKeyframe);
				array[i].value = SpriteUtility.RemapObjectToSprite(frames[i]);
				array[i].time = (float)i / newClip.frameRate;
			}
			EditorCurveBinding binding = EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite");
			AnimationUtility.SetObjectReferenceCurve(newClip, binding, array);
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
					else if (@object is Texture2D)
					{
						list.AddRange(SpriteUtility.TextureToSprites(@object as Texture2D));
					}
				}
			}
			if (list.Count > 0)
			{
				return list.ToArray();
			}
			Sprite sprite = SpriteUtility.HandleExternalDrag(Event.current.type == EventType.DragPerform);
			if (sprite != null)
			{
				return new Sprite[]
				{
					sprite
				};
			}
			return null;
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
				else if (@object.GetType() == typeof(Texture2D))
				{
					list.AddRange(SpriteUtility.TextureToSprites(@object as Texture2D));
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
			if (textureImporter == null)
			{
				return null;
			}
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

		public static GameObject CreateDragGO(Sprite frame, Vector3 position)
		{
			string name = (!string.IsNullOrEmpty(frame.name)) ? frame.name : "Sprite";
			name = GameObjectUtility.GetUniqueNameForSibling(null, name);
			GameObject gameObject = new GameObject(name);
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = frame;
			gameObject.transform.position = position;
			return gameObject;
		}

		public static void AddAnimationToGO(GameObject go, Sprite[] frames)
		{
			if (frames != null && frames.Length > 0)
			{
				SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
				if (spriteRenderer == null)
				{
					Debug.LogWarning("There should be a SpriteRenderer in dragged object");
					spriteRenderer = go.AddComponent<SpriteRenderer>();
				}
				spriteRenderer.sprite = frames[0];
				if (frames.Length > 1)
				{
					Analytics.Event("Sprite Drag and Drop", "Drop multiple sprites to scene", "null", 1);
					if (!SpriteUtility.CreateAnimation(go, frames))
					{
						Debug.LogError("Failed to create animation for dragged object");
					}
				}
				else
				{
					Analytics.Event("Sprite Drag and Drop", "Drop single sprite to scene", "null", 1);
				}
			}
		}

		public static GameObject DropSpriteToSceneToCreateGO(Sprite sprite, Vector3 position)
		{
			GameObject gameObject = new GameObject((!string.IsNullOrEmpty(sprite.name)) ? sprite.name : "Sprite");
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

		public static Sprite[] TextureToSprites(Texture2D tex)
		{
			UnityEngine.Object[] array = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tex));
			List<Sprite> list = new List<Sprite>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetType() == typeof(Sprite))
				{
					list.Add(array[i] as Sprite);
				}
			}
			if (list.Count > 0)
			{
				return list.ToArray();
			}
			return new Sprite[]
			{
				SpriteUtility.GenerateDefaultSprite(tex)
			};
		}

		public static Sprite TextureToSprite(Texture2D tex)
		{
			Sprite[] array = SpriteUtility.TextureToSprites(tex);
			if (array.Length > 0)
			{
				return array[0];
			}
			return null;
		}

		private static bool ValidPathForTextureAsset(string path)
		{
			string a = FileUtil.GetPathExtension(path).ToLower();
			return a == "jpg" || a == "jpeg" || a == "tif" || a == "tiff" || a == "tga" || a == "gif" || a == "png" || a == "psd" || a == "bmp" || a == "iff" || a == "pict" || a == "pic" || a == "pct" || a == "exr" || a == "hdr";
		}
	}
}
