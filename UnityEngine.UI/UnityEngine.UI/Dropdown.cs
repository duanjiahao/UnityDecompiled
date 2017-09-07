using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI.CoroutineTween;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Dropdown", 35), RequireComponent(typeof(RectTransform))]
	public class Dropdown : Selectable, IPointerClickHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
	{
		protected internal class DropdownItem : MonoBehaviour, IPointerEnterHandler, ICancelHandler, IEventSystemHandler
		{
			[SerializeField]
			private Text m_Text;

			[SerializeField]
			private Image m_Image;

			[SerializeField]
			private RectTransform m_RectTransform;

			[SerializeField]
			private Toggle m_Toggle;

			public Text text
			{
				get
				{
					return this.m_Text;
				}
				set
				{
					this.m_Text = value;
				}
			}

			public Image image
			{
				get
				{
					return this.m_Image;
				}
				set
				{
					this.m_Image = value;
				}
			}

			public RectTransform rectTransform
			{
				get
				{
					return this.m_RectTransform;
				}
				set
				{
					this.m_RectTransform = value;
				}
			}

			public Toggle toggle
			{
				get
				{
					return this.m_Toggle;
				}
				set
				{
					this.m_Toggle = value;
				}
			}

			public virtual void OnPointerEnter(PointerEventData eventData)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}

			public virtual void OnCancel(BaseEventData eventData)
			{
				Dropdown componentInParent = base.GetComponentInParent<Dropdown>();
				if (componentInParent)
				{
					componentInParent.Hide();
				}
			}
		}

		[Serializable]
		public class OptionData
		{
			[SerializeField]
			private string m_Text;

			[SerializeField]
			private Sprite m_Image;

			public string text
			{
				get
				{
					return this.m_Text;
				}
				set
				{
					this.m_Text = value;
				}
			}

			public Sprite image
			{
				get
				{
					return this.m_Image;
				}
				set
				{
					this.m_Image = value;
				}
			}

			public OptionData()
			{
			}

			public OptionData(string text)
			{
				this.text = text;
			}

			public OptionData(Sprite image)
			{
				this.image = image;
			}

			public OptionData(string text, Sprite image)
			{
				this.text = text;
				this.image = image;
			}
		}

		[Serializable]
		public class OptionDataList
		{
			[SerializeField]
			private List<Dropdown.OptionData> m_Options;

			public List<Dropdown.OptionData> options
			{
				get
				{
					return this.m_Options;
				}
				set
				{
					this.m_Options = value;
				}
			}

			public OptionDataList()
			{
				this.options = new List<Dropdown.OptionData>();
			}
		}

		[Serializable]
		public class DropdownEvent : UnityEvent<int>
		{
		}

		[SerializeField]
		private RectTransform m_Template;

		[SerializeField]
		private Text m_CaptionText;

		[SerializeField]
		private Image m_CaptionImage;

		[SerializeField, Space]
		private Text m_ItemText;

		[SerializeField]
		private Image m_ItemImage;

		[SerializeField, Space]
		private int m_Value;

		[SerializeField, Space]
		private Dropdown.OptionDataList m_Options = new Dropdown.OptionDataList();

		[SerializeField, Space]
		private Dropdown.DropdownEvent m_OnValueChanged = new Dropdown.DropdownEvent();

		private GameObject m_Dropdown;

		private GameObject m_Blocker;

		private List<Dropdown.DropdownItem> m_Items = new List<Dropdown.DropdownItem>();

		private TweenRunner<FloatTween> m_AlphaTweenRunner;

		private bool validTemplate = false;

		private static Dropdown.OptionData s_NoOptionData = new Dropdown.OptionData();

		public RectTransform template
		{
			get
			{
				return this.m_Template;
			}
			set
			{
				this.m_Template = value;
				this.RefreshShownValue();
			}
		}

		public Text captionText
		{
			get
			{
				return this.m_CaptionText;
			}
			set
			{
				this.m_CaptionText = value;
				this.RefreshShownValue();
			}
		}

		public Image captionImage
		{
			get
			{
				return this.m_CaptionImage;
			}
			set
			{
				this.m_CaptionImage = value;
				this.RefreshShownValue();
			}
		}

		public Text itemText
		{
			get
			{
				return this.m_ItemText;
			}
			set
			{
				this.m_ItemText = value;
				this.RefreshShownValue();
			}
		}

		public Image itemImage
		{
			get
			{
				return this.m_ItemImage;
			}
			set
			{
				this.m_ItemImage = value;
				this.RefreshShownValue();
			}
		}

		public List<Dropdown.OptionData> options
		{
			get
			{
				return this.m_Options.options;
			}
			set
			{
				this.m_Options.options = value;
				this.RefreshShownValue();
			}
		}

		public Dropdown.DropdownEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		public int value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				if (!Application.isPlaying || (value != this.m_Value && this.options.Count != 0))
				{
					this.m_Value = Mathf.Clamp(value, 0, this.options.Count - 1);
					this.RefreshShownValue();
					UISystemProfilerApi.AddMarker("Dropdown.value", this);
					this.m_OnValueChanged.Invoke(this.m_Value);
				}
			}
		}

		protected Dropdown()
		{
		}

		protected override void Awake()
		{
			if (Application.isPlaying)
			{
				this.m_AlphaTweenRunner = new TweenRunner<FloatTween>();
				this.m_AlphaTweenRunner.Init(this);
				if (this.m_CaptionImage)
				{
					this.m_CaptionImage.enabled = (this.m_CaptionImage.sprite != null);
				}
				if (this.m_Template)
				{
					this.m_Template.gameObject.SetActive(false);
				}
			}
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			if (this.IsActive())
			{
				this.RefreshShownValue();
			}
		}

		public void RefreshShownValue()
		{
			Dropdown.OptionData optionData = Dropdown.s_NoOptionData;
			if (this.options.Count > 0)
			{
				optionData = this.options[Mathf.Clamp(this.m_Value, 0, this.options.Count - 1)];
			}
			if (this.m_CaptionText)
			{
				if (optionData != null && optionData.text != null)
				{
					this.m_CaptionText.text = optionData.text;
				}
				else
				{
					this.m_CaptionText.text = "";
				}
			}
			if (this.m_CaptionImage)
			{
				if (optionData != null)
				{
					this.m_CaptionImage.sprite = optionData.image;
				}
				else
				{
					this.m_CaptionImage.sprite = null;
				}
				this.m_CaptionImage.enabled = (this.m_CaptionImage.sprite != null);
			}
		}

		public void AddOptions(List<Dropdown.OptionData> options)
		{
			this.options.AddRange(options);
			this.RefreshShownValue();
		}

		public void AddOptions(List<string> options)
		{
			for (int i = 0; i < options.Count; i++)
			{
				this.options.Add(new Dropdown.OptionData(options[i]));
			}
			this.RefreshShownValue();
		}

		public void AddOptions(List<Sprite> options)
		{
			for (int i = 0; i < options.Count; i++)
			{
				this.options.Add(new Dropdown.OptionData(options[i]));
			}
			this.RefreshShownValue();
		}

		public void ClearOptions()
		{
			this.options.Clear();
			this.RefreshShownValue();
		}

		private void SetupTemplate()
		{
			this.validTemplate = false;
			if (!this.m_Template)
			{
				UnityEngine.Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this);
			}
			else
			{
				GameObject gameObject = this.m_Template.gameObject;
				gameObject.SetActive(true);
				Toggle componentInChildren = this.m_Template.GetComponentInChildren<Toggle>();
				this.validTemplate = true;
				if (!componentInChildren || componentInChildren.transform == this.template)
				{
					this.validTemplate = false;
					UnityEngine.Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", this.template);
				}
				else if (!(componentInChildren.transform.parent is RectTransform))
				{
					this.validTemplate = false;
					UnityEngine.Debug.LogError("The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", this.template);
				}
				else if (this.itemText != null && !this.itemText.transform.IsChildOf(componentInChildren.transform))
				{
					this.validTemplate = false;
					UnityEngine.Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", this.template);
				}
				else if (this.itemImage != null && !this.itemImage.transform.IsChildOf(componentInChildren.transform))
				{
					this.validTemplate = false;
					UnityEngine.Debug.LogError("The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", this.template);
				}
				if (!this.validTemplate)
				{
					gameObject.SetActive(false);
				}
				else
				{
					Dropdown.DropdownItem dropdownItem = componentInChildren.gameObject.AddComponent<Dropdown.DropdownItem>();
					dropdownItem.text = this.m_ItemText;
					dropdownItem.image = this.m_ItemImage;
					dropdownItem.toggle = componentInChildren;
					dropdownItem.rectTransform = (RectTransform)componentInChildren.transform;
					Canvas orAddComponent = Dropdown.GetOrAddComponent<Canvas>(gameObject);
					orAddComponent.overrideSorting = true;
					orAddComponent.sortingOrder = 30000;
					Dropdown.GetOrAddComponent<GraphicRaycaster>(gameObject);
					Dropdown.GetOrAddComponent<CanvasGroup>(gameObject);
					gameObject.SetActive(false);
					this.validTemplate = true;
				}
			}
		}

		private static T GetOrAddComponent<T>(GameObject go) where T : Component
		{
			T t = go.GetComponent<T>();
			if (!t)
			{
				t = go.AddComponent<T>();
			}
			return t;
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			this.Show();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			this.Show();
		}

		public virtual void OnCancel(BaseEventData eventData)
		{
			this.Hide();
		}

		public void Show()
		{
			if (this.IsActive() && this.IsInteractable() && !(this.m_Dropdown != null))
			{
				if (!this.validTemplate)
				{
					this.SetupTemplate();
					if (!this.validTemplate)
					{
						return;
					}
				}
				List<Canvas> list = ListPool<Canvas>.Get();
				base.gameObject.GetComponentsInParent<Canvas>(false, list);
				if (list.Count != 0)
				{
					Canvas canvas = list[0];
					ListPool<Canvas>.Release(list);
					this.m_Template.gameObject.SetActive(true);
					this.m_Dropdown = this.CreateDropdownList(this.m_Template.gameObject);
					this.m_Dropdown.name = "Dropdown List";
					this.m_Dropdown.SetActive(true);
					RectTransform rectTransform = this.m_Dropdown.transform as RectTransform;
					rectTransform.SetParent(this.m_Template.transform.parent, false);
					Dropdown.DropdownItem componentInChildren = this.m_Dropdown.GetComponentInChildren<Dropdown.DropdownItem>();
					GameObject gameObject = componentInChildren.rectTransform.parent.gameObject;
					RectTransform rectTransform2 = gameObject.transform as RectTransform;
					componentInChildren.rectTransform.gameObject.SetActive(true);
					Rect rect = rectTransform2.rect;
					Rect rect2 = componentInChildren.rectTransform.rect;
					Vector2 vector = rect2.min - rect.min + componentInChildren.rectTransform.localPosition;
					Vector2 vector2 = rect2.max - rect.max + componentInChildren.rectTransform.localPosition;
					Vector2 size = rect2.size;
					this.m_Items.Clear();
					Toggle toggle = null;
					for (int i = 0; i < this.options.Count; i++)
					{
						Dropdown.OptionData data = this.options[i];
						Dropdown.DropdownItem item = this.AddItem(data, this.value == i, componentInChildren, this.m_Items);
						if (!(item == null))
						{
							item.toggle.isOn = (this.value == i);
							item.toggle.onValueChanged.AddListener(delegate(bool x)
							{
								this.OnSelectItem(item.toggle);
							});
							if (item.toggle.isOn)
							{
								item.toggle.Select();
							}
							if (toggle != null)
							{
								Navigation navigation = toggle.navigation;
								Navigation navigation2 = item.toggle.navigation;
								navigation.mode = Navigation.Mode.Explicit;
								navigation2.mode = Navigation.Mode.Explicit;
								navigation.selectOnDown = item.toggle;
								navigation.selectOnRight = item.toggle;
								navigation2.selectOnLeft = toggle;
								navigation2.selectOnUp = toggle;
								toggle.navigation = navigation;
								item.toggle.navigation = navigation2;
							}
							toggle = item.toggle;
						}
					}
					Vector2 sizeDelta = rectTransform2.sizeDelta;
					sizeDelta.y = size.y * (float)this.m_Items.Count + vector.y - vector2.y;
					rectTransform2.sizeDelta = sizeDelta;
					float num = rectTransform.rect.height - rectTransform2.rect.height;
					if (num > 0f)
					{
						rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - num);
					}
					Vector3[] array = new Vector3[4];
					rectTransform.GetWorldCorners(array);
					RectTransform rectTransform3 = canvas.transform as RectTransform;
					Rect rect3 = rectTransform3.rect;
					for (int j = 0; j < 2; j++)
					{
						bool flag = false;
						for (int k = 0; k < 4; k++)
						{
							Vector3 vector3 = rectTransform3.InverseTransformPoint(array[k]);
							if (vector3[j] < rect3.min[j] || vector3[j] > rect3.max[j])
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							RectTransformUtility.FlipLayoutOnAxis(rectTransform, j, false, false);
						}
					}
					for (int l = 0; l < this.m_Items.Count; l++)
					{
						RectTransform rectTransform4 = this.m_Items[l].rectTransform;
						rectTransform4.anchorMin = new Vector2(rectTransform4.anchorMin.x, 0f);
						rectTransform4.anchorMax = new Vector2(rectTransform4.anchorMax.x, 0f);
						rectTransform4.anchoredPosition = new Vector2(rectTransform4.anchoredPosition.x, vector.y + size.y * (float)(this.m_Items.Count - 1 - l) + size.y * rectTransform4.pivot.y);
						rectTransform4.sizeDelta = new Vector2(rectTransform4.sizeDelta.x, size.y);
					}
					this.AlphaFadeList(0.15f, 0f, 1f);
					this.m_Template.gameObject.SetActive(false);
					componentInChildren.gameObject.SetActive(false);
					this.m_Blocker = this.CreateBlocker(canvas);
				}
			}
		}

		protected virtual GameObject CreateBlocker(Canvas rootCanvas)
		{
			GameObject gameObject = new GameObject("Blocker");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.SetParent(rootCanvas.transform, false);
			rectTransform.anchorMin = Vector3.zero;
			rectTransform.anchorMax = Vector3.one;
			rectTransform.sizeDelta = Vector2.zero;
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.overrideSorting = true;
			Canvas component = this.m_Dropdown.GetComponent<Canvas>();
			canvas.sortingLayerID = component.sortingLayerID;
			canvas.sortingOrder = component.sortingOrder - 1;
			gameObject.AddComponent<GraphicRaycaster>();
			Image image = gameObject.AddComponent<Image>();
			image.color = Color.clear;
			Button button = gameObject.AddComponent<Button>();
			button.onClick.AddListener(new UnityAction(this.Hide));
			return gameObject;
		}

		protected virtual void DestroyBlocker(GameObject blocker)
		{
			UnityEngine.Object.Destroy(blocker);
		}

		protected virtual GameObject CreateDropdownList(GameObject template)
		{
			return UnityEngine.Object.Instantiate<GameObject>(template);
		}

		protected virtual void DestroyDropdownList(GameObject dropdownList)
		{
			UnityEngine.Object.Destroy(dropdownList);
		}

		protected virtual Dropdown.DropdownItem CreateItem(Dropdown.DropdownItem itemTemplate)
		{
			return UnityEngine.Object.Instantiate<Dropdown.DropdownItem>(itemTemplate);
		}

		protected virtual void DestroyItem(Dropdown.DropdownItem item)
		{
		}

		private Dropdown.DropdownItem AddItem(Dropdown.OptionData data, bool selected, Dropdown.DropdownItem itemTemplate, List<Dropdown.DropdownItem> items)
		{
			Dropdown.DropdownItem dropdownItem = this.CreateItem(itemTemplate);
			dropdownItem.rectTransform.SetParent(itemTemplate.rectTransform.parent, false);
			dropdownItem.gameObject.SetActive(true);
			dropdownItem.gameObject.name = "Item " + items.Count + ((data.text == null) ? "" : (": " + data.text));
			if (dropdownItem.toggle != null)
			{
				dropdownItem.toggle.isOn = false;
			}
			if (dropdownItem.text)
			{
				dropdownItem.text.text = data.text;
			}
			if (dropdownItem.image)
			{
				dropdownItem.image.sprite = data.image;
				dropdownItem.image.enabled = (dropdownItem.image.sprite != null);
			}
			items.Add(dropdownItem);
			return dropdownItem;
		}

		private void AlphaFadeList(float duration, float alpha)
		{
			CanvasGroup component = this.m_Dropdown.GetComponent<CanvasGroup>();
			this.AlphaFadeList(duration, component.alpha, alpha);
		}

		private void AlphaFadeList(float duration, float start, float end)
		{
			if (!end.Equals(start))
			{
				FloatTween info = new FloatTween
				{
					duration = duration,
					startValue = start,
					targetValue = end
				};
				info.AddOnChangedCallback(new UnityAction<float>(this.SetAlpha));
				info.ignoreTimeScale = true;
				this.m_AlphaTweenRunner.StartTween(info);
			}
		}

		private void SetAlpha(float alpha)
		{
			if (this.m_Dropdown)
			{
				CanvasGroup component = this.m_Dropdown.GetComponent<CanvasGroup>();
				component.alpha = alpha;
			}
		}

		public void Hide()
		{
			if (this.m_Dropdown != null)
			{
				this.AlphaFadeList(0.15f, 0f);
				if (this.IsActive())
				{
					base.StartCoroutine(this.DelayedDestroyDropdownList(0.15f));
				}
			}
			if (this.m_Blocker != null)
			{
				this.DestroyBlocker(this.m_Blocker);
			}
			this.m_Blocker = null;
			this.Select();
		}

		[DebuggerHidden]
		private IEnumerator DelayedDestroyDropdownList(float delay)
		{
			Dropdown.<DelayedDestroyDropdownList>c__Iterator0 <DelayedDestroyDropdownList>c__Iterator = new Dropdown.<DelayedDestroyDropdownList>c__Iterator0();
			<DelayedDestroyDropdownList>c__Iterator.delay = delay;
			<DelayedDestroyDropdownList>c__Iterator.$this = this;
			return <DelayedDestroyDropdownList>c__Iterator;
		}

		private void OnSelectItem(Toggle toggle)
		{
			if (!toggle.isOn)
			{
				toggle.isOn = true;
			}
			int num = -1;
			Transform transform = toggle.transform;
			Transform parent = transform.parent;
			for (int i = 0; i < parent.childCount; i++)
			{
				if (parent.GetChild(i) == transform)
				{
					num = i - 1;
					break;
				}
			}
			if (num >= 0)
			{
				this.value = num;
				this.Hide();
			}
		}
	}
}
