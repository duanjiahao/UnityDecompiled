using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	internal static class MenuOptions
	{
		private const string kUILayerName = "UI";

		private const string kStandardSpritePath = "UI/Skin/UISprite.psd";

		private const string kBackgroundSpritePath = "UI/Skin/Background.psd";

		private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";

		private const string kKnobPath = "UI/Skin/Knob.psd";

		private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";

		private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";

		private const string kMaskPath = "UI/Skin/UIMask.psd";

		private static DefaultControls.Resources s_StandardResources;

		private static DefaultControls.Resources GetStandardResources()
		{
			if (MenuOptions.s_StandardResources.standard == null)
			{
				MenuOptions.s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
				MenuOptions.s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
				MenuOptions.s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
				MenuOptions.s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
				MenuOptions.s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
				MenuOptions.s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd");
				MenuOptions.s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
			}
			return MenuOptions.s_StandardResources;
		}

		private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
		{
			SceneView sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null && SceneView.sceneViews.Count > 0)
			{
				sceneView = (SceneView.sceneViews[0] as SceneView);
			}
			if (!(sceneView == null) && !(sceneView.camera == null))
			{
				Camera camera = sceneView.camera;
				Vector3 zero = Vector3.zero;
				Vector2 vector;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2((float)(camera.pixelWidth / 2), (float)(camera.pixelHeight / 2)), camera, out vector))
				{
					vector.x += canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
					vector.y += canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;
					vector.x = Mathf.Clamp(vector.x, 0f, canvasRTransform.sizeDelta.x);
					vector.y = Mathf.Clamp(vector.y, 0f, canvasRTransform.sizeDelta.y);
					zero.x = vector.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
					zero.y = vector.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;
					Vector3 vector2;
					vector2.x = canvasRTransform.sizeDelta.x * (0f - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
					vector2.y = canvasRTransform.sizeDelta.y * (0f - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;
					Vector3 vector3;
					vector3.x = canvasRTransform.sizeDelta.x * (1f - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
					vector3.y = canvasRTransform.sizeDelta.y * (1f - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;
					zero.x = Mathf.Clamp(zero.x, vector2.x, vector3.x);
					zero.y = Mathf.Clamp(zero.y, vector2.y, vector3.y);
				}
				itemTransform.anchoredPosition = zero;
				itemTransform.localRotation = Quaternion.identity;
				itemTransform.localScale = Vector3.one;
			}
		}

		private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
		{
			GameObject gameObject = menuCommand.context as GameObject;
			if (gameObject == null || gameObject.GetComponentInParent<Canvas>() == null)
			{
				gameObject = MenuOptions.GetOrCreateCanvasGameObject();
			}
			string uniqueNameForSibling = GameObjectUtility.GetUniqueNameForSibling(gameObject.transform, element.name);
			element.name = uniqueNameForSibling;
			Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
			Undo.SetTransformParent(element.transform, gameObject.transform, "Parent " + element.name);
			GameObjectUtility.SetParentAndAlign(element, gameObject);
			if (gameObject != menuCommand.context)
			{
				MenuOptions.SetPositionVisibleinSceneView(gameObject.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());
			}
			Selection.activeGameObject = element;
		}

		[MenuItem("GameObject/UI/Text", false, 2000)]
		public static void AddText(MenuCommand menuCommand)
		{
			GameObject element = DefaultControls.CreateText(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(element, menuCommand);
		}

		[MenuItem("GameObject/UI/Image", false, 2001)]
		public static void AddImage(MenuCommand menuCommand)
		{
			GameObject element = DefaultControls.CreateImage(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(element, menuCommand);
		}

		[MenuItem("GameObject/UI/Raw Image", false, 2002)]
		public static void AddRawImage(MenuCommand menuCommand)
		{
			GameObject element = DefaultControls.CreateRawImage(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(element, menuCommand);
		}

		[MenuItem("GameObject/UI/Button", false, 2030)]
		public static void AddButton(MenuCommand menuCommand)
		{
			GameObject element = DefaultControls.CreateButton(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(element, menuCommand);
		}

		[MenuItem("GameObject/UI/Toggle", false, 2031)]
		public static void AddToggle(MenuCommand menuCommand)
		{
			GameObject element = DefaultControls.CreateToggle(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(element, menuCommand);
		}

		[MenuItem("GameObject/UI/Slider", false, 2033)]
		public static void AddSlider(MenuCommand menuCommand)
		{
			GameObject element = DefaultControls.CreateSlider(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(element, menuCommand);
		}

		[MenuItem("GameObject/UI/Scrollbar", false, 2034)]
		public static void AddScrollbar(MenuCommand menuCommand)
		{
			GameObject element = DefaultControls.CreateScrollbar(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(element, menuCommand);
		}

		[MenuItem("GameObject/UI/Dropdown", false, 2035)]
		public static void AddDropdown(MenuCommand menuCommand)
		{
			GameObject element = DefaultControls.CreateDropdown(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(element, menuCommand);
		}

		[MenuItem("GameObject/UI/Input Field", false, 2036)]
		public static void AddInputField(MenuCommand menuCommand)
		{
			GameObject element = DefaultControls.CreateInputField(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(element, menuCommand);
		}

		[MenuItem("GameObject/UI/Canvas", false, 2060)]
		public static void AddCanvas(MenuCommand menuCommand)
		{
			GameObject gameObject = MenuOptions.CreateNewUI();
			GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
			if (gameObject.transform.parent as RectTransform)
			{
				RectTransform rectTransform = gameObject.transform as RectTransform;
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.anchoredPosition = Vector2.zero;
				rectTransform.sizeDelta = Vector2.zero;
			}
			Selection.activeGameObject = gameObject;
		}

		[MenuItem("GameObject/UI/Panel", false, 2061)]
		public static void AddPanel(MenuCommand menuCommand)
		{
			GameObject gameObject = DefaultControls.CreatePanel(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(gameObject, menuCommand);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchoredPosition = Vector2.zero;
			component.sizeDelta = Vector2.zero;
		}

		[MenuItem("GameObject/UI/Scroll View", false, 2062)]
		public static void AddScrollView(MenuCommand menuCommand)
		{
			GameObject element = DefaultControls.CreateScrollView(MenuOptions.GetStandardResources());
			MenuOptions.PlaceUIElementRoot(element, menuCommand);
		}

		public static GameObject CreateNewUI()
		{
			GameObject gameObject = new GameObject("Canvas");
			gameObject.layer = LayerMask.NameToLayer("UI");
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			gameObject.AddComponent<CanvasScaler>();
			gameObject.AddComponent<GraphicRaycaster>();
			Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
			MenuOptions.CreateEventSystem(false);
			return gameObject;
		}

		[MenuItem("GameObject/UI/Event System", false, 2100)]
		public static void CreateEventSystem(MenuCommand menuCommand)
		{
			GameObject parent = menuCommand.context as GameObject;
			MenuOptions.CreateEventSystem(true, parent);
		}

		private static void CreateEventSystem(bool select)
		{
			MenuOptions.CreateEventSystem(select, null);
		}

		private static void CreateEventSystem(bool select, GameObject parent)
		{
			EventSystem eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
			if (eventSystem == null)
			{
				GameObject gameObject = new GameObject("EventSystem");
				GameObjectUtility.SetParentAndAlign(gameObject, parent);
				eventSystem = gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>();
				Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
			}
			if (select && eventSystem != null)
			{
				Selection.activeGameObject = eventSystem.gameObject;
			}
		}

		public static GameObject GetOrCreateCanvasGameObject()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			Canvas canvas = (!(activeGameObject != null)) ? null : activeGameObject.GetComponentInParent<Canvas>();
			GameObject result;
			if (canvas != null && canvas.gameObject.activeInHierarchy)
			{
				result = canvas.gameObject;
			}
			else
			{
				canvas = (UnityEngine.Object.FindObjectOfType(typeof(Canvas)) as Canvas);
				if (canvas != null && canvas.gameObject.activeInHierarchy)
				{
					result = canvas.gameObject;
				}
				else
				{
					result = MenuOptions.CreateNewUI();
				}
			}
			return result;
		}
	}
}
