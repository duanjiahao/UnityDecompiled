using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Input Field", 31)]
	public class InputField : Selectable, IUpdateSelectedHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, ISubmitHandler, ICanvasElement, ILayoutElement, IEventSystemHandler
	{
		public enum ContentType
		{
			Standard,
			Autocorrected,
			IntegerNumber,
			DecimalNumber,
			Alphanumeric,
			Name,
			EmailAddress,
			Password,
			Pin,
			Custom
		}

		public enum InputType
		{
			Standard,
			AutoCorrect,
			Password
		}

		public enum CharacterValidation
		{
			None,
			Integer,
			Decimal,
			Alphanumeric,
			Name,
			EmailAddress
		}

		public enum LineType
		{
			SingleLine,
			MultiLineSubmit,
			MultiLineNewline
		}

		public delegate char OnValidateInput(string text, int charIndex, char addedChar);

		[Serializable]
		public class SubmitEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class OnChangeEvent : UnityEvent<string>
		{
		}

		protected enum EditState
		{
			Continue,
			Finish
		}

		protected TouchScreenKeyboard m_Keyboard;

		private static readonly char[] kSeparators = new char[]
		{
			' ',
			'.',
			',',
			'\t',
			'\r',
			'\n'
		};

		[FormerlySerializedAs("text"), SerializeField]
		protected Text m_TextComponent;

		[SerializeField]
		protected Graphic m_Placeholder;

		[SerializeField]
		private InputField.ContentType m_ContentType = InputField.ContentType.Standard;

		[FormerlySerializedAs("inputType"), SerializeField]
		private InputField.InputType m_InputType = InputField.InputType.Standard;

		[FormerlySerializedAs("asteriskChar"), SerializeField]
		private char m_AsteriskChar = '*';

		[FormerlySerializedAs("keyboardType"), SerializeField]
		private TouchScreenKeyboardType m_KeyboardType = TouchScreenKeyboardType.Default;

		[SerializeField]
		private InputField.LineType m_LineType = InputField.LineType.SingleLine;

		[FormerlySerializedAs("hideMobileInput"), SerializeField]
		private bool m_HideMobileInput = false;

		[FormerlySerializedAs("validation"), SerializeField]
		private InputField.CharacterValidation m_CharacterValidation = InputField.CharacterValidation.None;

		[FormerlySerializedAs("characterLimit"), SerializeField]
		private int m_CharacterLimit = 0;

		[FormerlySerializedAs("onSubmit"), FormerlySerializedAs("m_OnSubmit"), FormerlySerializedAs("m_EndEdit"), SerializeField]
		private InputField.SubmitEvent m_OnEndEdit = new InputField.SubmitEvent();

		[FormerlySerializedAs("onValueChange"), FormerlySerializedAs("m_OnValueChange"), SerializeField]
		private InputField.OnChangeEvent m_OnValueChanged = new InputField.OnChangeEvent();

		[FormerlySerializedAs("onValidateInput"), SerializeField]
		private InputField.OnValidateInput m_OnValidateInput;

		[SerializeField]
		private Color m_CaretColor = new Color(0.196078435f, 0.196078435f, 0.196078435f, 1f);

		[SerializeField]
		private bool m_CustomCaretColor = false;

		[FormerlySerializedAs("selectionColor"), SerializeField]
		private Color m_SelectionColor = new Color(0.65882355f, 0.807843149f, 1f, 0.7529412f);

		[FormerlySerializedAs("mValue"), SerializeField]
		protected string m_Text = string.Empty;

		[Range(0f, 4f), SerializeField]
		private float m_CaretBlinkRate = 0.85f;

		[Range(1f, 5f), SerializeField]
		private int m_CaretWidth = 1;

		[SerializeField]
		private bool m_ReadOnly = false;

		protected int m_CaretPosition = 0;

		protected int m_CaretSelectPosition = 0;

		private RectTransform caretRectTrans = null;

		protected UIVertex[] m_CursorVerts = null;

		private TextGenerator m_InputTextCache;

		private CanvasRenderer m_CachedInputRenderer;

		private bool m_PreventFontCallback = false;

		[NonSerialized]
		protected Mesh m_Mesh;

		private bool m_AllowInput = false;

		private bool m_ShouldActivateNextUpdate = false;

		private bool m_UpdateDrag = false;

		private bool m_DragPositionOutOfBounds = false;

		private const float kHScrollSpeed = 0.05f;

		private const float kVScrollSpeed = 0.1f;

		protected bool m_CaretVisible;

		private Coroutine m_BlinkCoroutine = null;

		private float m_BlinkStartTime = 0f;

		protected int m_DrawStart = 0;

		protected int m_DrawEnd = 0;

		private Coroutine m_DragCoroutine = null;

		private string m_OriginalText = "";

		private bool m_WasCanceled = false;

		private bool m_HasDoneFocusTransition = false;

		private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

		private Event m_ProcessingEvent = new Event();

		private BaseInput input
		{
			get
			{
				BaseInput result;
				if (EventSystem.current && EventSystem.current.currentInputModule)
				{
					result = EventSystem.current.currentInputModule.input;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		private string compositionString
		{
			get
			{
				return (!(this.input != null)) ? Input.compositionString : this.input.compositionString;
			}
		}

		protected Mesh mesh
		{
			get
			{
				if (this.m_Mesh == null)
				{
					this.m_Mesh = new Mesh();
				}
				return this.m_Mesh;
			}
		}

		protected TextGenerator cachedInputTextGenerator
		{
			get
			{
				if (this.m_InputTextCache == null)
				{
					this.m_InputTextCache = new TextGenerator();
				}
				return this.m_InputTextCache;
			}
		}

		public bool shouldHideMobileInput
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				bool result;
				switch (platform)
				{
				case RuntimePlatform.IPhonePlayer:
				case RuntimePlatform.Android:
					goto IL_34;
				case RuntimePlatform.PS3:
				case RuntimePlatform.XBOX360:
					IL_1F:
					if (platform != RuntimePlatform.TizenPlayer && platform != RuntimePlatform.tvOS)
					{
						result = true;
						return result;
					}
					goto IL_34;
				}
				goto IL_1F;
				IL_34:
				result = this.m_HideMobileInput;
				return result;
			}
			set
			{
				SetPropertyUtility.SetStruct<bool>(ref this.m_HideMobileInput, value);
			}
		}

		private bool shouldActivateOnSelect
		{
			get
			{
				return Application.platform != RuntimePlatform.tvOS;
			}
		}

		public string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				if (!(this.text == value))
				{
					if (value == null)
					{
						value = "";
					}
					value = value.Replace("\0", string.Empty);
					if (this.m_LineType == InputField.LineType.SingleLine)
					{
						value = value.Replace("\n", "").Replace("\t", "");
					}
					if (this.onValidateInput != null || this.characterValidation != InputField.CharacterValidation.None)
					{
						this.m_Text = "";
						InputField.OnValidateInput onValidateInput = this.onValidateInput ?? new InputField.OnValidateInput(this.Validate);
						this.m_CaretPosition = (this.m_CaretSelectPosition = value.Length);
						int num = (this.characterLimit <= 0) ? value.Length : Math.Min(this.characterLimit, value.Length);
						for (int i = 0; i < num; i++)
						{
							char c = onValidateInput(this.m_Text, this.m_Text.Length, value[i]);
							if (c != '\0')
							{
								this.m_Text += c;
							}
						}
					}
					else
					{
						this.m_Text = ((this.characterLimit <= 0 || value.Length <= this.characterLimit) ? value : value.Substring(0, this.characterLimit));
					}
					if (!Application.isPlaying)
					{
						this.SendOnValueChangedAndUpdateLabel();
					}
					else
					{
						if (this.m_Keyboard != null)
						{
							this.m_Keyboard.text = this.m_Text;
						}
						if (this.m_CaretPosition > this.m_Text.Length)
						{
							this.m_CaretPosition = (this.m_CaretSelectPosition = this.m_Text.Length);
						}
						else if (this.m_CaretSelectPosition > this.m_Text.Length)
						{
							this.m_CaretSelectPosition = this.m_Text.Length;
						}
						this.SendOnValueChangedAndUpdateLabel();
					}
				}
			}
		}

		public bool isFocused
		{
			get
			{
				return this.m_AllowInput;
			}
		}

		public float caretBlinkRate
		{
			get
			{
				return this.m_CaretBlinkRate;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_CaretBlinkRate, value))
				{
					if (this.m_AllowInput)
					{
						this.SetCaretActive();
					}
				}
			}
		}

		public int caretWidth
		{
			get
			{
				return this.m_CaretWidth;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_CaretWidth, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		public Text textComponent
		{
			get
			{
				return this.m_TextComponent;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Text>(ref this.m_TextComponent, value))
				{
					this.EnforceTextHOverflow();
				}
			}
		}

		public Graphic placeholder
		{
			get
			{
				return this.m_Placeholder;
			}
			set
			{
				SetPropertyUtility.SetClass<Graphic>(ref this.m_Placeholder, value);
			}
		}

		public Color caretColor
		{
			get
			{
				return (!this.customCaretColor) ? this.textComponent.color : this.m_CaretColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_CaretColor, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		public bool customCaretColor
		{
			get
			{
				return this.m_CustomCaretColor;
			}
			set
			{
				if (this.m_CustomCaretColor != value)
				{
					this.m_CustomCaretColor = value;
					this.MarkGeometryAsDirty();
				}
			}
		}

		public Color selectionColor
		{
			get
			{
				return this.m_SelectionColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_SelectionColor, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		public InputField.SubmitEvent onEndEdit
		{
			get
			{
				return this.m_OnEndEdit;
			}
			set
			{
				SetPropertyUtility.SetClass<InputField.SubmitEvent>(ref this.m_OnEndEdit, value);
			}
		}

		[Obsolete("onValueChange has been renamed to onValueChanged")]
		public InputField.OnChangeEvent onValueChange
		{
			get
			{
				return this.onValueChanged;
			}
			set
			{
				this.onValueChanged = value;
			}
		}

		public InputField.OnChangeEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				SetPropertyUtility.SetClass<InputField.OnChangeEvent>(ref this.m_OnValueChanged, value);
			}
		}

		public InputField.OnValidateInput onValidateInput
		{
			get
			{
				return this.m_OnValidateInput;
			}
			set
			{
				SetPropertyUtility.SetClass<InputField.OnValidateInput>(ref this.m_OnValidateInput, value);
			}
		}

		public int characterLimit
		{
			get
			{
				return this.m_CharacterLimit;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_CharacterLimit, Math.Max(0, value)))
				{
					this.UpdateLabel();
				}
			}
		}

		public InputField.ContentType contentType
		{
			get
			{
				return this.m_ContentType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<InputField.ContentType>(ref this.m_ContentType, value))
				{
					this.EnforceContentType();
				}
			}
		}

		public InputField.LineType lineType
		{
			get
			{
				return this.m_LineType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<InputField.LineType>(ref this.m_LineType, value))
				{
					this.SetToCustomIfContentTypeIsNot(new InputField.ContentType[]
					{
						InputField.ContentType.Standard,
						InputField.ContentType.Autocorrected
					});
					this.EnforceTextHOverflow();
				}
			}
		}

		public InputField.InputType inputType
		{
			get
			{
				return this.m_InputType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<InputField.InputType>(ref this.m_InputType, value))
				{
					this.SetToCustom();
				}
			}
		}

		public TouchScreenKeyboardType keyboardType
		{
			get
			{
				return this.m_KeyboardType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TouchScreenKeyboardType>(ref this.m_KeyboardType, value))
				{
					this.SetToCustom();
				}
			}
		}

		public InputField.CharacterValidation characterValidation
		{
			get
			{
				return this.m_CharacterValidation;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<InputField.CharacterValidation>(ref this.m_CharacterValidation, value))
				{
					this.SetToCustom();
				}
			}
		}

		public bool readOnly
		{
			get
			{
				return this.m_ReadOnly;
			}
			set
			{
				this.m_ReadOnly = value;
			}
		}

		public bool multiLine
		{
			get
			{
				return this.m_LineType == InputField.LineType.MultiLineNewline || this.lineType == InputField.LineType.MultiLineSubmit;
			}
		}

		public char asteriskChar
		{
			get
			{
				return this.m_AsteriskChar;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<char>(ref this.m_AsteriskChar, value))
				{
					this.UpdateLabel();
				}
			}
		}

		public bool wasCanceled
		{
			get
			{
				return this.m_WasCanceled;
			}
		}

		protected int caretPositionInternal
		{
			get
			{
				return this.m_CaretPosition + this.compositionString.Length;
			}
			set
			{
				this.m_CaretPosition = value;
				this.ClampPos(ref this.m_CaretPosition);
			}
		}

		protected int caretSelectPositionInternal
		{
			get
			{
				return this.m_CaretSelectPosition + this.compositionString.Length;
			}
			set
			{
				this.m_CaretSelectPosition = value;
				this.ClampPos(ref this.m_CaretSelectPosition);
			}
		}

		private bool hasSelection
		{
			get
			{
				return this.caretPositionInternal != this.caretSelectPositionInternal;
			}
		}

		[Obsolete("caretSelectPosition has been deprecated. Use selectionFocusPosition instead (UnityUpgradable) -> selectionFocusPosition", true)]
		public int caretSelectPosition
		{
			get
			{
				return this.selectionFocusPosition;
			}
			protected set
			{
				this.selectionFocusPosition = value;
			}
		}

		public int caretPosition
		{
			get
			{
				return this.m_CaretSelectPosition + this.compositionString.Length;
			}
			set
			{
				this.selectionAnchorPosition = value;
				this.selectionFocusPosition = value;
			}
		}

		public int selectionAnchorPosition
		{
			get
			{
				return this.m_CaretPosition + this.compositionString.Length;
			}
			set
			{
				if (this.compositionString.Length == 0)
				{
					this.m_CaretPosition = value;
					this.ClampPos(ref this.m_CaretPosition);
				}
			}
		}

		public int selectionFocusPosition
		{
			get
			{
				return this.m_CaretSelectPosition + this.compositionString.Length;
			}
			set
			{
				if (this.compositionString.Length == 0)
				{
					this.m_CaretSelectPosition = value;
					this.ClampPos(ref this.m_CaretSelectPosition);
				}
			}
		}

		private static string clipboard
		{
			get
			{
				return GUIUtility.systemCopyBuffer;
			}
			set
			{
				GUIUtility.systemCopyBuffer = value;
			}
		}

		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				float result;
				if (this.textComponent == null)
				{
					result = 0f;
				}
				else
				{
					TextGenerationSettings generationSettings = this.textComponent.GetGenerationSettings(Vector2.zero);
					result = this.textComponent.cachedTextGeneratorForLayout.GetPreferredWidth(this.m_Text, generationSettings) / this.textComponent.pixelsPerUnit;
				}
				return result;
			}
		}

		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				float result;
				if (this.textComponent == null)
				{
					result = 0f;
				}
				else
				{
					TextGenerationSettings generationSettings = this.textComponent.GetGenerationSettings(new Vector2(this.textComponent.rectTransform.rect.size.x, 0f));
					result = this.textComponent.cachedTextGeneratorForLayout.GetPreferredHeight(this.m_Text, generationSettings) / this.textComponent.pixelsPerUnit;
				}
				return result;
			}
		}

		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		public virtual int layoutPriority
		{
			get
			{
				return 1;
			}
		}

		protected InputField()
		{
			this.EnforceTextHOverflow();
		}

		protected void ClampPos(ref int pos)
		{
			if (pos < 0)
			{
				pos = 0;
			}
			else if (pos > this.text.Length)
			{
				pos = this.text.Length;
			}
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			this.EnforceContentType();
			this.EnforceTextHOverflow();
			this.m_CharacterLimit = Math.Max(0, this.m_CharacterLimit);
			if (this.IsActive())
			{
				this.UpdateLabel();
				if (this.m_AllowInput)
				{
					this.SetCaretActive();
				}
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_Text == null)
			{
				this.m_Text = string.Empty;
			}
			this.m_DrawStart = 0;
			this.m_DrawEnd = this.m_Text.Length;
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
			}
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				this.m_TextComponent.RegisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
				this.UpdateLabel();
			}
		}

		protected override void OnDisable()
		{
			this.m_BlinkCoroutine = null;
			this.DeactivateInputField();
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				this.m_TextComponent.UnregisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.Clear();
			}
			if (this.m_Mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Mesh);
			}
			this.m_Mesh = null;
			base.OnDisable();
		}

		[DebuggerHidden]
		private IEnumerator CaretBlink()
		{
			InputField.<CaretBlink>c__Iterator0 <CaretBlink>c__Iterator = new InputField.<CaretBlink>c__Iterator0();
			<CaretBlink>c__Iterator.$this = this;
			return <CaretBlink>c__Iterator;
		}

		private void SetCaretVisible()
		{
			if (this.m_AllowInput)
			{
				this.m_CaretVisible = true;
				this.m_BlinkStartTime = Time.unscaledTime;
				this.SetCaretActive();
			}
		}

		private void SetCaretActive()
		{
			if (this.m_AllowInput)
			{
				if (this.m_CaretBlinkRate > 0f)
				{
					if (this.m_BlinkCoroutine == null)
					{
						this.m_BlinkCoroutine = base.StartCoroutine(this.CaretBlink());
					}
				}
				else
				{
					this.m_CaretVisible = true;
				}
			}
		}

		private void UpdateCaretMaterial()
		{
			if (this.m_TextComponent != null && this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
			}
		}

		protected void OnFocus()
		{
			this.SelectAll();
		}

		protected void SelectAll()
		{
			this.caretPositionInternal = this.text.Length;
			this.caretSelectPositionInternal = 0;
		}

		public void MoveTextEnd(bool shift)
		{
			int length = this.text.Length;
			if (shift)
			{
				this.caretSelectPositionInternal = length;
			}
			else
			{
				this.caretPositionInternal = length;
				this.caretSelectPositionInternal = this.caretPositionInternal;
			}
			this.UpdateLabel();
		}

		public void MoveTextStart(bool shift)
		{
			int num = 0;
			if (shift)
			{
				this.caretSelectPositionInternal = num;
			}
			else
			{
				this.caretPositionInternal = num;
				this.caretSelectPositionInternal = this.caretPositionInternal;
			}
			this.UpdateLabel();
		}

		private bool InPlaceEditing()
		{
			return !TouchScreenKeyboard.isSupported;
		}

		private void UpdateCaretFromKeyboard()
		{
			RangeInt selection = this.m_Keyboard.selection;
			int start = selection.start;
			int end = selection.end;
			bool flag = false;
			if (this.caretPositionInternal != start)
			{
				flag = true;
				this.caretPositionInternal = start;
			}
			if (this.caretSelectPositionInternal != end)
			{
				this.caretSelectPositionInternal = end;
				flag = true;
			}
			if (flag)
			{
				this.m_BlinkStartTime = Time.unscaledTime;
				this.UpdateLabel();
			}
		}

		protected virtual void LateUpdate()
		{
			if (this.m_ShouldActivateNextUpdate)
			{
				if (!this.isFocused)
				{
					this.ActivateInputFieldInternal();
					this.m_ShouldActivateNextUpdate = false;
					return;
				}
				this.m_ShouldActivateNextUpdate = false;
			}
			if (!this.InPlaceEditing() && this.isFocused)
			{
				this.AssignPositioningIfNeeded();
				if (this.m_Keyboard == null || this.m_Keyboard.done)
				{
					if (this.m_Keyboard != null)
					{
						if (!this.m_ReadOnly)
						{
							this.text = this.m_Keyboard.text;
						}
						if (this.m_Keyboard.wasCanceled)
						{
							this.m_WasCanceled = true;
						}
					}
					this.OnDeselect(null);
				}
				else
				{
					string text = this.m_Keyboard.text;
					if (this.m_Text != text)
					{
						if (this.m_ReadOnly)
						{
							this.m_Keyboard.text = this.m_Text;
						}
						else
						{
							this.m_Text = "";
							for (int i = 0; i < text.Length; i++)
							{
								char c = text[i];
								if (c == '\r' || c == '\u0003')
								{
									c = '\n';
								}
								if (this.onValidateInput != null)
								{
									c = this.onValidateInput(this.m_Text, this.m_Text.Length, c);
								}
								else if (this.characterValidation != InputField.CharacterValidation.None)
								{
									c = this.Validate(this.m_Text, this.m_Text.Length, c);
								}
								if (this.lineType == InputField.LineType.MultiLineSubmit && c == '\n')
								{
									this.m_Keyboard.text = this.m_Text;
									this.OnDeselect(null);
									return;
								}
								if (c != '\0')
								{
									this.m_Text += c;
								}
							}
							if (this.characterLimit > 0 && this.m_Text.Length > this.characterLimit)
							{
								this.m_Text = this.m_Text.Substring(0, this.characterLimit);
							}
							if (this.m_Keyboard.canGetSelection)
							{
								this.UpdateCaretFromKeyboard();
							}
							else
							{
								int length = this.m_Text.Length;
								this.caretSelectPositionInternal = length;
								this.caretPositionInternal = length;
							}
							if (this.m_Text != text)
							{
								this.m_Keyboard.text = this.m_Text;
							}
							this.SendOnValueChangedAndUpdateLabel();
						}
					}
					else if (this.m_Keyboard.canGetSelection)
					{
						this.UpdateCaretFromKeyboard();
					}
					if (this.m_Keyboard.done)
					{
						if (this.m_Keyboard.wasCanceled)
						{
							this.m_WasCanceled = true;
						}
						this.OnDeselect(null);
					}
				}
			}
		}

		[Obsolete("This function is no longer used. Please use RectTransformUtility.ScreenPointToLocalPointInRectangle() instead.")]
		public Vector2 ScreenToLocal(Vector2 screen)
		{
			Canvas canvas = this.m_TextComponent.canvas;
			Vector2 result;
			if (canvas == null)
			{
				result = screen;
			}
			else
			{
				Vector3 vector = Vector3.zero;
				if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					vector = this.m_TextComponent.transform.InverseTransformPoint(screen);
				}
				else if (canvas.worldCamera != null)
				{
					Ray ray = canvas.worldCamera.ScreenPointToRay(screen);
					Plane plane = new Plane(this.m_TextComponent.transform.forward, this.m_TextComponent.transform.position);
					float distance;
					plane.Raycast(ray, out distance);
					vector = this.m_TextComponent.transform.InverseTransformPoint(ray.GetPoint(distance));
				}
				result = new Vector2(vector.x, vector.y);
			}
			return result;
		}

		private int GetUnclampedCharacterLineFromPosition(Vector2 pos, TextGenerator generator)
		{
			int result;
			if (!this.multiLine)
			{
				result = 0;
			}
			else
			{
				float num = pos.y * this.m_TextComponent.pixelsPerUnit;
				float num2 = 0f;
				int i = 0;
				while (i < generator.lineCount)
				{
					float topY = generator.lines[i].topY;
					float num3 = topY - (float)generator.lines[i].height;
					if (num > topY)
					{
						float num4 = topY - num2;
						if (num > topY - 0.5f * num4)
						{
							result = i - 1;
							return result;
						}
						result = i;
						return result;
					}
					else
					{
						if (num > num3)
						{
							result = i;
							return result;
						}
						num2 = num3;
						i++;
					}
				}
				result = generator.lineCount;
			}
			return result;
		}

		protected int GetCharacterIndexFromPosition(Vector2 pos)
		{
			TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
			int result;
			if (cachedTextGenerator.lineCount == 0)
			{
				result = 0;
			}
			else
			{
				int unclampedCharacterLineFromPosition = this.GetUnclampedCharacterLineFromPosition(pos, cachedTextGenerator);
				if (unclampedCharacterLineFromPosition < 0)
				{
					result = 0;
				}
				else if (unclampedCharacterLineFromPosition >= cachedTextGenerator.lineCount)
				{
					result = cachedTextGenerator.characterCountVisible;
				}
				else
				{
					int startCharIdx = cachedTextGenerator.lines[unclampedCharacterLineFromPosition].startCharIdx;
					int lineEndPosition = InputField.GetLineEndPosition(cachedTextGenerator, unclampedCharacterLineFromPosition);
					for (int i = startCharIdx; i < lineEndPosition; i++)
					{
						if (i >= cachedTextGenerator.characterCountVisible)
						{
							break;
						}
						UICharInfo uICharInfo = cachedTextGenerator.characters[i];
						Vector2 vector = uICharInfo.cursorPos / this.m_TextComponent.pixelsPerUnit;
						float num = pos.x - vector.x;
						float num2 = vector.x + uICharInfo.charWidth / this.m_TextComponent.pixelsPerUnit - pos.x;
						if (num < num2)
						{
							result = i;
							return result;
						}
					}
					result = lineEndPosition;
				}
			}
			return result;
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left && this.m_TextComponent != null && this.m_Keyboard == null;
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (this.MayDrag(eventData))
			{
				this.m_UpdateDrag = true;
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (this.MayDrag(eventData))
			{
				Vector2 pos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera, out pos);
				this.caretSelectPositionInternal = this.GetCharacterIndexFromPosition(pos) + this.m_DrawStart;
				this.MarkGeometryAsDirty();
				this.m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera);
				if (this.m_DragPositionOutOfBounds && this.m_DragCoroutine == null)
				{
					this.m_DragCoroutine = base.StartCoroutine(this.MouseDragOutsideRect(eventData));
				}
				eventData.Use();
			}
		}

		[DebuggerHidden]
		private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
		{
			InputField.<MouseDragOutsideRect>c__Iterator1 <MouseDragOutsideRect>c__Iterator = new InputField.<MouseDragOutsideRect>c__Iterator1();
			<MouseDragOutsideRect>c__Iterator.eventData = eventData;
			<MouseDragOutsideRect>c__Iterator.$this = this;
			return <MouseDragOutsideRect>c__Iterator;
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (this.MayDrag(eventData))
			{
				this.m_UpdateDrag = false;
			}
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (this.MayDrag(eventData))
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
				bool allowInput = this.m_AllowInput;
				base.OnPointerDown(eventData);
				if (!this.InPlaceEditing())
				{
					if (this.m_Keyboard == null || !this.m_Keyboard.active)
					{
						this.OnSelect(eventData);
						return;
					}
				}
				if (allowInput)
				{
					Vector2 pos;
					RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera, out pos);
					int num = this.GetCharacterIndexFromPosition(pos) + this.m_DrawStart;
					this.caretPositionInternal = num;
					this.caretSelectPositionInternal = num;
				}
				this.UpdateLabel();
				eventData.Use();
			}
		}

		protected InputField.EditState KeyPressed(Event evt)
		{
			EventModifiers modifiers = evt.modifiers;
			bool flag = (SystemInfo.operatingSystemFamily != OperatingSystemFamily.MacOSX) ? ((modifiers & EventModifiers.Control) != EventModifiers.None) : ((modifiers & EventModifiers.Command) != EventModifiers.None);
			bool flag2 = (modifiers & EventModifiers.Shift) != EventModifiers.None;
			bool flag3 = (modifiers & EventModifiers.Alt) != EventModifiers.None;
			bool flag4 = flag && !flag3 && !flag2;
			KeyCode keyCode = evt.keyCode;
			InputField.EditState result;
			switch (keyCode)
			{
			case KeyCode.KeypadEnter:
				goto IL_222;
			case KeyCode.KeypadEquals:
			case KeyCode.Insert:
				IL_8D:
				switch (keyCode)
				{
				case KeyCode.A:
					if (flag4)
					{
						this.SelectAll();
						result = InputField.EditState.Continue;
						return result;
					}
					goto IL_24D;
				case KeyCode.B:
					IL_A3:
					switch (keyCode)
					{
					case KeyCode.V:
						if (flag4)
						{
							this.Append(InputField.clipboard);
							result = InputField.EditState.Continue;
							return result;
						}
						goto IL_24D;
					case KeyCode.W:
						IL_B9:
						if (keyCode == KeyCode.Backspace)
						{
							this.Backspace();
							result = InputField.EditState.Continue;
							return result;
						}
						if (keyCode == KeyCode.Return)
						{
							goto IL_222;
						}
						if (keyCode == KeyCode.Escape)
						{
							this.m_WasCanceled = true;
							result = InputField.EditState.Finish;
							return result;
						}
						if (keyCode != KeyCode.Delete)
						{
							goto IL_24D;
						}
						this.ForwardSpace();
						result = InputField.EditState.Continue;
						return result;
					case KeyCode.X:
						if (flag4)
						{
							if (this.inputType != InputField.InputType.Password)
							{
								InputField.clipboard = this.GetSelectedString();
							}
							else
							{
								InputField.clipboard = "";
							}
							this.Delete();
							this.SendOnValueChangedAndUpdateLabel();
							result = InputField.EditState.Continue;
							return result;
						}
						goto IL_24D;
					}
					goto IL_B9;
				case KeyCode.C:
					if (flag4)
					{
						if (this.inputType != InputField.InputType.Password)
						{
							InputField.clipboard = this.GetSelectedString();
						}
						else
						{
							InputField.clipboard = "";
						}
						result = InputField.EditState.Continue;
						return result;
					}
					goto IL_24D;
				}
				goto IL_A3;
			case KeyCode.UpArrow:
				this.MoveUp(flag2);
				result = InputField.EditState.Continue;
				return result;
			case KeyCode.DownArrow:
				this.MoveDown(flag2);
				result = InputField.EditState.Continue;
				return result;
			case KeyCode.RightArrow:
				this.MoveRight(flag2, flag);
				result = InputField.EditState.Continue;
				return result;
			case KeyCode.LeftArrow:
				this.MoveLeft(flag2, flag);
				result = InputField.EditState.Continue;
				return result;
			case KeyCode.Home:
				this.MoveTextStart(flag2);
				result = InputField.EditState.Continue;
				return result;
			case KeyCode.End:
				this.MoveTextEnd(flag2);
				result = InputField.EditState.Continue;
				return result;
			}
			goto IL_8D;
			IL_222:
			if (this.lineType != InputField.LineType.MultiLineNewline)
			{
				result = InputField.EditState.Finish;
				return result;
			}
			IL_24D:
			char c = evt.character;
			if (!this.multiLine && (c == '\t' || c == '\r' || c == '\n'))
			{
				result = InputField.EditState.Continue;
			}
			else
			{
				if (c == '\r' || c == '\u0003')
				{
					c = '\n';
				}
				if (this.IsValidChar(c))
				{
					this.Append(c);
				}
				if (c == '\0')
				{
					if (this.compositionString.Length > 0)
					{
						this.UpdateLabel();
					}
				}
				result = InputField.EditState.Continue;
			}
			return result;
		}

		private bool IsValidChar(char c)
		{
			return c != '\u007f' && (c == '\t' || c == '\n' || this.m_TextComponent.font.HasCharacter(c));
		}

		public void ProcessEvent(Event e)
		{
			this.KeyPressed(e);
		}

		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			if (this.isFocused)
			{
				bool flag = false;
				while (Event.PopEvent(this.m_ProcessingEvent))
				{
					if (this.m_ProcessingEvent.rawType == EventType.KeyDown)
					{
						flag = true;
						InputField.EditState editState = this.KeyPressed(this.m_ProcessingEvent);
						if (editState == InputField.EditState.Finish)
						{
							this.DeactivateInputField();
							break;
						}
					}
					EventType type = this.m_ProcessingEvent.type;
					if (type == EventType.ValidateCommand || type == EventType.ExecuteCommand)
					{
						string commandName = this.m_ProcessingEvent.commandName;
						if (commandName != null)
						{
							if (commandName == "SelectAll")
							{
								this.SelectAll();
								flag = true;
							}
						}
					}
				}
				if (flag)
				{
					this.UpdateLabel();
				}
				eventData.Use();
			}
		}

		private string GetSelectedString()
		{
			string result;
			if (!this.hasSelection)
			{
				result = "";
			}
			else
			{
				int num = this.caretPositionInternal;
				int num2 = this.caretSelectPositionInternal;
				if (num > num2)
				{
					int num3 = num;
					num = num2;
					num2 = num3;
				}
				result = this.text.Substring(num, num2 - num);
			}
			return result;
		}

		private int FindtNextWordBegin()
		{
			int result;
			if (this.caretSelectPositionInternal + 1 >= this.text.Length)
			{
				result = this.text.Length;
			}
			else
			{
				int num = this.text.IndexOfAny(InputField.kSeparators, this.caretSelectPositionInternal + 1);
				if (num == -1)
				{
					num = this.text.Length;
				}
				else
				{
					num++;
				}
				result = num;
			}
			return result;
		}

		private void MoveRight(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				int num = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal);
				this.caretSelectPositionInternal = num;
				this.caretPositionInternal = num;
			}
			else
			{
				int num2;
				if (ctrl)
				{
					num2 = this.FindtNextWordBegin();
				}
				else
				{
					num2 = this.caretSelectPositionInternal + 1;
				}
				if (shift)
				{
					this.caretSelectPositionInternal = num2;
				}
				else
				{
					int num = num2;
					this.caretPositionInternal = num;
					this.caretSelectPositionInternal = num;
				}
			}
		}

		private int FindtPrevWordBegin()
		{
			int result;
			if (this.caretSelectPositionInternal - 2 < 0)
			{
				result = 0;
			}
			else
			{
				int num = this.text.LastIndexOfAny(InputField.kSeparators, this.caretSelectPositionInternal - 2);
				if (num == -1)
				{
					num = 0;
				}
				else
				{
					num++;
				}
				result = num;
			}
			return result;
		}

		private void MoveLeft(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				int num = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal);
				this.caretSelectPositionInternal = num;
				this.caretPositionInternal = num;
			}
			else
			{
				int num2;
				if (ctrl)
				{
					num2 = this.FindtPrevWordBegin();
				}
				else
				{
					num2 = this.caretSelectPositionInternal - 1;
				}
				if (shift)
				{
					this.caretSelectPositionInternal = num2;
				}
				else
				{
					int num = num2;
					this.caretPositionInternal = num;
					this.caretSelectPositionInternal = num;
				}
			}
		}

		private int DetermineCharacterLine(int charPos, TextGenerator generator)
		{
			int result;
			for (int i = 0; i < generator.lineCount - 1; i++)
			{
				if (generator.lines[i + 1].startCharIdx > charPos)
				{
					result = i;
					return result;
				}
			}
			result = generator.lineCount - 1;
			return result;
		}

		private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			int result;
			if (originalPos >= this.cachedInputTextGenerator.characters.Count)
			{
				result = 0;
			}
			else
			{
				UICharInfo uICharInfo = this.cachedInputTextGenerator.characters[originalPos];
				int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
				if (num <= 0)
				{
					result = ((!goToFirstChar) ? originalPos : 0);
				}
				else
				{
					int num2 = this.cachedInputTextGenerator.lines[num].startCharIdx - 1;
					for (int i = this.cachedInputTextGenerator.lines[num - 1].startCharIdx; i < num2; i++)
					{
						if (this.cachedInputTextGenerator.characters[i].cursorPos.x >= uICharInfo.cursorPos.x)
						{
							result = i;
							return result;
						}
					}
					result = num2;
				}
			}
			return result;
		}

		private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			int result;
			if (originalPos >= this.cachedInputTextGenerator.characterCountVisible)
			{
				result = this.text.Length;
			}
			else
			{
				UICharInfo uICharInfo = this.cachedInputTextGenerator.characters[originalPos];
				int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
				if (num + 1 >= this.cachedInputTextGenerator.lineCount)
				{
					result = ((!goToLastChar) ? originalPos : this.text.Length);
				}
				else
				{
					int lineEndPosition = InputField.GetLineEndPosition(this.cachedInputTextGenerator, num + 1);
					for (int i = this.cachedInputTextGenerator.lines[num + 1].startCharIdx; i < lineEndPosition; i++)
					{
						if (this.cachedInputTextGenerator.characters[i].cursorPos.x >= uICharInfo.cursorPos.x)
						{
							result = i;
							return result;
						}
					}
					result = lineEndPosition;
				}
			}
			return result;
		}

		private void MoveDown(bool shift)
		{
			this.MoveDown(shift, true);
		}

		private void MoveDown(bool shift, bool goToLastChar)
		{
			if (this.hasSelection && !shift)
			{
				int num = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal);
				this.caretSelectPositionInternal = num;
				this.caretPositionInternal = num;
			}
			int num2 = (!this.multiLine) ? this.text.Length : this.LineDownCharacterPosition(this.caretSelectPositionInternal, goToLastChar);
			if (shift)
			{
				this.caretSelectPositionInternal = num2;
			}
			else
			{
				int num = num2;
				this.caretSelectPositionInternal = num;
				this.caretPositionInternal = num;
			}
		}

		private void MoveUp(bool shift)
		{
			this.MoveUp(shift, true);
		}

		private void MoveUp(bool shift, bool goToFirstChar)
		{
			if (this.hasSelection && !shift)
			{
				int num = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal);
				this.caretSelectPositionInternal = num;
				this.caretPositionInternal = num;
			}
			int num2 = (!this.multiLine) ? 0 : this.LineUpCharacterPosition(this.caretSelectPositionInternal, goToFirstChar);
			if (shift)
			{
				this.caretSelectPositionInternal = num2;
			}
			else
			{
				int num = num2;
				this.caretPositionInternal = num;
				this.caretSelectPositionInternal = num;
			}
		}

		private void Delete()
		{
			if (!this.m_ReadOnly)
			{
				if (this.caretPositionInternal != this.caretSelectPositionInternal)
				{
					if (this.caretPositionInternal < this.caretSelectPositionInternal)
					{
						this.m_Text = this.text.Substring(0, this.caretPositionInternal) + this.text.Substring(this.caretSelectPositionInternal, this.text.Length - this.caretSelectPositionInternal);
						this.caretSelectPositionInternal = this.caretPositionInternal;
					}
					else
					{
						this.m_Text = this.text.Substring(0, this.caretSelectPositionInternal) + this.text.Substring(this.caretPositionInternal, this.text.Length - this.caretPositionInternal);
						this.caretPositionInternal = this.caretSelectPositionInternal;
					}
				}
			}
		}

		private void ForwardSpace()
		{
			if (!this.m_ReadOnly)
			{
				if (this.hasSelection)
				{
					this.Delete();
					this.SendOnValueChangedAndUpdateLabel();
				}
				else if (this.caretPositionInternal < this.text.Length)
				{
					this.m_Text = this.text.Remove(this.caretPositionInternal, 1);
					this.SendOnValueChangedAndUpdateLabel();
				}
			}
		}

		private void Backspace()
		{
			if (!this.m_ReadOnly)
			{
				if (this.hasSelection)
				{
					this.Delete();
					this.SendOnValueChangedAndUpdateLabel();
				}
				else if (this.caretPositionInternal > 0)
				{
					this.m_Text = this.text.Remove(this.caretPositionInternal - 1, 1);
					int num = this.caretPositionInternal - 1;
					this.caretPositionInternal = num;
					this.caretSelectPositionInternal = num;
					this.SendOnValueChangedAndUpdateLabel();
				}
			}
		}

		private void Insert(char c)
		{
			if (!this.m_ReadOnly)
			{
				string text = c.ToString();
				this.Delete();
				if (this.characterLimit <= 0 || this.text.Length < this.characterLimit)
				{
					this.m_Text = this.text.Insert(this.m_CaretPosition, text);
					this.caretSelectPositionInternal = (this.caretPositionInternal += text.Length);
					this.SendOnValueChanged();
				}
			}
		}

		private void SendOnValueChangedAndUpdateLabel()
		{
			this.SendOnValueChanged();
			this.UpdateLabel();
		}

		private void SendOnValueChanged()
		{
			if (this.onValueChanged != null)
			{
				this.onValueChanged.Invoke(this.text);
			}
		}

		protected void SendOnSubmit()
		{
			if (this.onEndEdit != null)
			{
				this.onEndEdit.Invoke(this.m_Text);
			}
		}

		protected virtual void Append(string input)
		{
			if (!this.m_ReadOnly)
			{
				if (this.InPlaceEditing())
				{
					int i = 0;
					int length = input.Length;
					while (i < length)
					{
						char c = input[i];
						if (c >= ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\n')
						{
							this.Append(c);
						}
						i++;
					}
				}
			}
		}

		protected virtual void Append(char input)
		{
			if (!this.m_ReadOnly)
			{
				if (this.InPlaceEditing())
				{
					int num = Math.Min(this.selectionFocusPosition, this.selectionAnchorPosition);
					if (this.onValidateInput != null)
					{
						input = this.onValidateInput(this.text, num, input);
					}
					else if (this.characterValidation != InputField.CharacterValidation.None)
					{
						input = this.Validate(this.text, num, input);
					}
					if (input != '\0')
					{
						this.Insert(input);
					}
				}
			}
		}

		protected void UpdateLabel()
		{
			if (this.m_TextComponent != null && this.m_TextComponent.font != null && !this.m_PreventFontCallback)
			{
				this.m_PreventFontCallback = true;
				string text;
				if (this.compositionString.Length > 0)
				{
					text = this.text.Substring(0, this.m_CaretPosition) + this.compositionString + this.text.Substring(this.m_CaretPosition);
				}
				else
				{
					text = this.text;
				}
				string text2;
				if (this.inputType == InputField.InputType.Password)
				{
					text2 = new string(this.asteriskChar, text.Length);
				}
				else
				{
					text2 = text;
				}
				bool flag = string.IsNullOrEmpty(text);
				if (this.m_Placeholder != null)
				{
					this.m_Placeholder.enabled = flag;
				}
				if (!this.m_AllowInput)
				{
					this.m_DrawStart = 0;
					this.m_DrawEnd = this.m_Text.Length;
				}
				if (!flag)
				{
					Vector2 size = this.m_TextComponent.rectTransform.rect.size;
					TextGenerationSettings generationSettings = this.m_TextComponent.GetGenerationSettings(size);
					generationSettings.generateOutOfBounds = true;
					this.cachedInputTextGenerator.PopulateWithErrors(text2, generationSettings, base.gameObject);
					this.SetDrawRangeToContainCaretPosition(this.caretSelectPositionInternal);
					text2 = text2.Substring(this.m_DrawStart, Mathf.Min(this.m_DrawEnd, text2.Length) - this.m_DrawStart);
					this.SetCaretVisible();
				}
				this.m_TextComponent.text = text2;
				this.MarkGeometryAsDirty();
				this.m_PreventFontCallback = false;
			}
		}

		private bool IsSelectionVisible()
		{
			return this.m_DrawStart <= this.caretPositionInternal && this.m_DrawStart <= this.caretSelectPositionInternal && this.m_DrawEnd >= this.caretPositionInternal && this.m_DrawEnd >= this.caretSelectPositionInternal;
		}

		private static int GetLineStartPosition(TextGenerator gen, int line)
		{
			line = Mathf.Clamp(line, 0, gen.lines.Count - 1);
			return gen.lines[line].startCharIdx;
		}

		private static int GetLineEndPosition(TextGenerator gen, int line)
		{
			line = Mathf.Max(line, 0);
			int result;
			if (line + 1 < gen.lines.Count)
			{
				result = gen.lines[line + 1].startCharIdx - 1;
			}
			else
			{
				result = gen.characterCountVisible;
			}
			return result;
		}

		private void SetDrawRangeToContainCaretPosition(int caretPos)
		{
			if (this.cachedInputTextGenerator.lineCount > 0)
			{
				Vector2 size = this.cachedInputTextGenerator.rectExtents.size;
				if (this.multiLine)
				{
					IList<UILineInfo> lines = this.cachedInputTextGenerator.lines;
					int num = this.DetermineCharacterLine(caretPos, this.cachedInputTextGenerator);
					if (caretPos > this.m_DrawEnd)
					{
						this.m_DrawEnd = InputField.GetLineEndPosition(this.cachedInputTextGenerator, num);
						float num2 = lines[num].topY - (float)lines[num].height;
						if (num == lines.Count - 1)
						{
							num2 += lines[num].leading;
						}
						int i;
						for (i = num; i > 0; i--)
						{
							float topY = lines[i - 1].topY;
							if (topY - num2 > size.y)
							{
								break;
							}
						}
						this.m_DrawStart = InputField.GetLineStartPosition(this.cachedInputTextGenerator, i);
					}
					else
					{
						if (caretPos < this.m_DrawStart)
						{
							this.m_DrawStart = InputField.GetLineStartPosition(this.cachedInputTextGenerator, num);
						}
						int j = this.DetermineCharacterLine(this.m_DrawStart, this.cachedInputTextGenerator);
						int k = j;
						float topY2 = lines[j].topY;
						float num3 = lines[k].topY - (float)lines[k].height;
						if (k == lines.Count - 1)
						{
							num3 += lines[k].leading;
						}
						while (k < lines.Count - 1)
						{
							num3 = lines[k + 1].topY - (float)lines[k + 1].height;
							if (k + 1 == lines.Count - 1)
							{
								num3 += lines[k + 1].leading;
							}
							if (topY2 - num3 > size.y)
							{
								break;
							}
							k++;
						}
						this.m_DrawEnd = InputField.GetLineEndPosition(this.cachedInputTextGenerator, k);
						while (j > 0)
						{
							topY2 = lines[j - 1].topY;
							if (topY2 - num3 > size.y)
							{
								break;
							}
							j--;
						}
						this.m_DrawStart = InputField.GetLineStartPosition(this.cachedInputTextGenerator, j);
					}
				}
				else
				{
					IList<UICharInfo> characters = this.cachedInputTextGenerator.characters;
					if (this.m_DrawEnd > this.cachedInputTextGenerator.characterCountVisible)
					{
						this.m_DrawEnd = this.cachedInputTextGenerator.characterCountVisible;
					}
					float num4 = 0f;
					if (caretPos > this.m_DrawEnd || (caretPos == this.m_DrawEnd && this.m_DrawStart > 0))
					{
						this.m_DrawEnd = caretPos;
						this.m_DrawStart = this.m_DrawEnd - 1;
						while (this.m_DrawStart >= 0)
						{
							if (num4 + characters[this.m_DrawStart].charWidth > size.x)
							{
								break;
							}
							num4 += characters[this.m_DrawStart].charWidth;
							this.m_DrawStart--;
						}
						this.m_DrawStart++;
					}
					else
					{
						if (caretPos < this.m_DrawStart)
						{
							this.m_DrawStart = caretPos;
						}
						this.m_DrawEnd = this.m_DrawStart;
					}
					while (this.m_DrawEnd < this.cachedInputTextGenerator.characterCountVisible)
					{
						num4 += characters[this.m_DrawEnd].charWidth;
						if (num4 > size.x)
						{
							break;
						}
						this.m_DrawEnd++;
					}
				}
			}
		}

		public void ForceLabelUpdate()
		{
			this.UpdateLabel();
		}

		private void MarkGeometryAsDirty()
		{
			if (Application.isPlaying && !(PrefabUtility.GetPrefabObject(base.gameObject) != null))
			{
				CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
			}
		}

		public virtual void Rebuild(CanvasUpdate update)
		{
			if (update == CanvasUpdate.LatePreRender)
			{
				this.UpdateGeometry();
			}
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		private void UpdateGeometry()
		{
			if (Application.isPlaying)
			{
				if (this.shouldHideMobileInput)
				{
					if (this.m_CachedInputRenderer == null && this.m_TextComponent != null)
					{
						GameObject gameObject = new GameObject(base.transform.name + " Input Caret", new Type[]
						{
							typeof(RectTransform),
							typeof(CanvasRenderer)
						});
						gameObject.hideFlags = HideFlags.DontSave;
						gameObject.transform.SetParent(this.m_TextComponent.transform.parent);
						gameObject.transform.SetAsFirstSibling();
						gameObject.layer = base.gameObject.layer;
						this.caretRectTrans = gameObject.GetComponent<RectTransform>();
						this.m_CachedInputRenderer = gameObject.GetComponent<CanvasRenderer>();
						this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
						gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
						this.AssignPositioningIfNeeded();
					}
					if (!(this.m_CachedInputRenderer == null))
					{
						this.OnFillVBO(this.mesh);
						this.m_CachedInputRenderer.SetMesh(this.mesh);
					}
				}
			}
		}

		private void AssignPositioningIfNeeded()
		{
			if (this.m_TextComponent != null && this.caretRectTrans != null && (this.caretRectTrans.localPosition != this.m_TextComponent.rectTransform.localPosition || this.caretRectTrans.localRotation != this.m_TextComponent.rectTransform.localRotation || this.caretRectTrans.localScale != this.m_TextComponent.rectTransform.localScale || this.caretRectTrans.anchorMin != this.m_TextComponent.rectTransform.anchorMin || this.caretRectTrans.anchorMax != this.m_TextComponent.rectTransform.anchorMax || this.caretRectTrans.anchoredPosition != this.m_TextComponent.rectTransform.anchoredPosition || this.caretRectTrans.sizeDelta != this.m_TextComponent.rectTransform.sizeDelta || this.caretRectTrans.pivot != this.m_TextComponent.rectTransform.pivot))
			{
				this.caretRectTrans.localPosition = this.m_TextComponent.rectTransform.localPosition;
				this.caretRectTrans.localRotation = this.m_TextComponent.rectTransform.localRotation;
				this.caretRectTrans.localScale = this.m_TextComponent.rectTransform.localScale;
				this.caretRectTrans.anchorMin = this.m_TextComponent.rectTransform.anchorMin;
				this.caretRectTrans.anchorMax = this.m_TextComponent.rectTransform.anchorMax;
				this.caretRectTrans.anchoredPosition = this.m_TextComponent.rectTransform.anchoredPosition;
				this.caretRectTrans.sizeDelta = this.m_TextComponent.rectTransform.sizeDelta;
				this.caretRectTrans.pivot = this.m_TextComponent.rectTransform.pivot;
			}
		}

		private void OnFillVBO(Mesh vbo)
		{
			using (VertexHelper vertexHelper = new VertexHelper())
			{
				if (!this.isFocused)
				{
					vertexHelper.FillMesh(vbo);
				}
				else
				{
					Vector2 roundingOffset = this.m_TextComponent.PixelAdjustPoint(Vector2.zero);
					if (!this.hasSelection)
					{
						this.GenerateCaret(vertexHelper, roundingOffset);
					}
					else
					{
						this.GenerateHightlight(vertexHelper, roundingOffset);
					}
					vertexHelper.FillMesh(vbo);
				}
			}
		}

		private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
		{
			if (this.m_CaretVisible)
			{
				if (this.m_CursorVerts == null)
				{
					this.CreateCursorVerts();
				}
				float num = (float)this.m_CaretWidth;
				int num2 = Mathf.Max(0, this.caretPositionInternal - this.m_DrawStart);
				TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
				if (cachedTextGenerator != null)
				{
					if (cachedTextGenerator.lineCount != 0)
					{
						Vector2 zero = Vector2.zero;
						if (num2 < cachedTextGenerator.characters.Count)
						{
							zero.x = cachedTextGenerator.characters[num2].cursorPos.x;
						}
						zero.x /= this.m_TextComponent.pixelsPerUnit;
						if (zero.x > this.m_TextComponent.rectTransform.rect.xMax)
						{
							zero.x = this.m_TextComponent.rectTransform.rect.xMax;
						}
						int index = this.DetermineCharacterLine(num2, cachedTextGenerator);
						zero.y = cachedTextGenerator.lines[index].topY / this.m_TextComponent.pixelsPerUnit;
						float num3 = (float)cachedTextGenerator.lines[index].height / this.m_TextComponent.pixelsPerUnit;
						for (int i = 0; i < this.m_CursorVerts.Length; i++)
						{
							this.m_CursorVerts[i].color = this.caretColor;
						}
						this.m_CursorVerts[0].position = new Vector3(zero.x, zero.y - num3, 0f);
						this.m_CursorVerts[1].position = new Vector3(zero.x + num, zero.y - num3, 0f);
						this.m_CursorVerts[2].position = new Vector3(zero.x + num, zero.y, 0f);
						this.m_CursorVerts[3].position = new Vector3(zero.x, zero.y, 0f);
						if (roundingOffset != Vector2.zero)
						{
							for (int j = 0; j < this.m_CursorVerts.Length; j++)
							{
								UIVertex uIVertex = this.m_CursorVerts[j];
								uIVertex.position.x = uIVertex.position.x + roundingOffset.x;
								uIVertex.position.y = uIVertex.position.y + roundingOffset.y;
							}
						}
						vbo.AddUIVertexQuad(this.m_CursorVerts);
						int num4 = Screen.height;
						int targetDisplay = this.m_TextComponent.canvas.targetDisplay;
						if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
						{
							num4 = Display.displays[targetDisplay].renderingHeight;
						}
						zero.y = (float)num4 - zero.y;
						this.input.compositionCursorPos = zero;
					}
				}
			}
		}

		private void CreateCursorVerts()
		{
			this.m_CursorVerts = new UIVertex[4];
			for (int i = 0; i < this.m_CursorVerts.Length; i++)
			{
				this.m_CursorVerts[i] = UIVertex.simpleVert;
				this.m_CursorVerts[i].uv0 = Vector2.zero;
			}
		}

		private void GenerateHightlight(VertexHelper vbo, Vector2 roundingOffset)
		{
			int num = Mathf.Max(0, this.caretPositionInternal - this.m_DrawStart);
			int num2 = Mathf.Max(0, this.caretSelectPositionInternal - this.m_DrawStart);
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			num2--;
			TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
			if (cachedTextGenerator.lineCount > 0)
			{
				int num4 = this.DetermineCharacterLine(num, cachedTextGenerator);
				int lineEndPosition = InputField.GetLineEndPosition(cachedTextGenerator, num4);
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.uv0 = Vector2.zero;
				simpleVert.color = this.selectionColor;
				int num5 = num;
				while (num5 <= num2 && num5 < cachedTextGenerator.characterCount)
				{
					if (num5 == lineEndPosition || num5 == num2)
					{
						UICharInfo uICharInfo = cachedTextGenerator.characters[num];
						UICharInfo uICharInfo2 = cachedTextGenerator.characters[num5];
						Vector2 vector = new Vector2(uICharInfo.cursorPos.x / this.m_TextComponent.pixelsPerUnit, cachedTextGenerator.lines[num4].topY / this.m_TextComponent.pixelsPerUnit);
						Vector2 vector2 = new Vector2((uICharInfo2.cursorPos.x + uICharInfo2.charWidth) / this.m_TextComponent.pixelsPerUnit, vector.y - (float)cachedTextGenerator.lines[num4].height / this.m_TextComponent.pixelsPerUnit);
						if (vector2.x > this.m_TextComponent.rectTransform.rect.xMax || vector2.x < this.m_TextComponent.rectTransform.rect.xMin)
						{
							vector2.x = this.m_TextComponent.rectTransform.rect.xMax;
						}
						int currentVertCount = vbo.currentVertCount;
						simpleVert.position = new Vector3(vector.x, vector2.y, 0f) + roundingOffset;
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector2.x, vector2.y, 0f) + roundingOffset;
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector2.x, vector.y, 0f) + roundingOffset;
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector.x, vector.y, 0f) + roundingOffset;
						vbo.AddVert(simpleVert);
						vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
						vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
						num = num5 + 1;
						num4++;
						lineEndPosition = InputField.GetLineEndPosition(cachedTextGenerator, num4);
					}
					num5++;
				}
			}
		}

		protected char Validate(string text, int pos, char ch)
		{
			char result;
			if (this.characterValidation == InputField.CharacterValidation.None || !base.enabled)
			{
				result = ch;
			}
			else
			{
				if (this.characterValidation == InputField.CharacterValidation.Integer || this.characterValidation == InputField.CharacterValidation.Decimal)
				{
					bool flag = pos == 0 && text.Length > 0 && text[0] == '-';
					bool flag2 = this.caretPositionInternal == 0 || this.caretSelectPositionInternal == 0;
					if (!flag)
					{
						if (ch >= '0' && ch <= '9')
						{
							result = ch;
							return result;
						}
						if (ch == '-' && (pos == 0 || flag2))
						{
							result = ch;
							return result;
						}
						if (ch == '.' && this.characterValidation == InputField.CharacterValidation.Decimal && !text.Contains("."))
						{
							result = ch;
							return result;
						}
					}
				}
				else if (this.characterValidation == InputField.CharacterValidation.Alphanumeric)
				{
					if (ch >= 'A' && ch <= 'Z')
					{
						result = ch;
						return result;
					}
					if (ch >= 'a' && ch <= 'z')
					{
						result = ch;
						return result;
					}
					if (ch >= '0' && ch <= '9')
					{
						result = ch;
						return result;
					}
				}
				else if (this.characterValidation == InputField.CharacterValidation.Name)
				{
					if (char.IsLetter(ch))
					{
						if (char.IsLower(ch) && (pos == 0 || text[pos - 1] == ' '))
						{
							result = char.ToUpper(ch);
							return result;
						}
						if (char.IsUpper(ch) && pos > 0 && text[pos - 1] != ' ' && text[pos - 1] != '\'')
						{
							result = char.ToLower(ch);
							return result;
						}
						result = ch;
						return result;
					}
					else
					{
						if (ch == '\'')
						{
							if (!text.Contains("'") && (pos <= 0 || (text[pos - 1] != ' ' && text[pos - 1] != '\'')) && (pos >= text.Length || (text[pos] != ' ' && text[pos] != '\'')))
							{
								result = ch;
								return result;
							}
						}
						if (ch == ' ')
						{
							if ((pos <= 0 || (text[pos - 1] != ' ' && text[pos - 1] != '\'')) && (pos >= text.Length || (text[pos] != ' ' && text[pos] != '\'')))
							{
								result = ch;
								return result;
							}
						}
					}
				}
				else if (this.characterValidation == InputField.CharacterValidation.EmailAddress)
				{
					if (ch >= 'A' && ch <= 'Z')
					{
						result = ch;
						return result;
					}
					if (ch >= 'a' && ch <= 'z')
					{
						result = ch;
						return result;
					}
					if (ch >= '0' && ch <= '9')
					{
						result = ch;
						return result;
					}
					if (ch == '@' && text.IndexOf('@') == -1)
					{
						result = ch;
						return result;
					}
					if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
					{
						result = ch;
						return result;
					}
					if (ch == '.')
					{
						char c = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
						char c2 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
						if (c != '.' && c2 != '.')
						{
							result = ch;
							return result;
						}
					}
				}
				result = '\0';
			}
			return result;
		}

		public void ActivateInputField()
		{
			if (!(this.m_TextComponent == null) && !(this.m_TextComponent.font == null) && this.IsActive() && this.IsInteractable())
			{
				if (this.isFocused)
				{
					if (this.m_Keyboard != null && !this.m_Keyboard.active)
					{
						this.m_Keyboard.active = true;
						this.m_Keyboard.text = this.m_Text;
					}
				}
				this.m_ShouldActivateNextUpdate = true;
			}
		}

		private void ActivateInputFieldInternal()
		{
			if (!(EventSystem.current == null))
			{
				if (EventSystem.current.currentSelectedGameObject != base.gameObject)
				{
					EventSystem.current.SetSelectedGameObject(base.gameObject);
				}
				if (TouchScreenKeyboard.isSupported)
				{
					if (this.input.touchSupported)
					{
						TouchScreenKeyboard.hideInput = this.shouldHideMobileInput;
					}
					this.m_Keyboard = ((this.inputType != InputField.InputType.Password) ? TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, this.inputType == InputField.InputType.AutoCorrect, this.multiLine) : TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, false, this.multiLine, true));
					this.MoveTextEnd(false);
				}
				else
				{
					this.input.imeCompositionMode = IMECompositionMode.On;
					this.OnFocus();
				}
				this.m_AllowInput = true;
				this.m_OriginalText = this.text;
				this.m_WasCanceled = false;
				this.SetCaretVisible();
				this.UpdateLabel();
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (this.shouldActivateOnSelect)
			{
				this.ActivateInputField();
			}
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.ActivateInputField();
			}
		}

		public void DeactivateInputField()
		{
			if (this.m_AllowInput)
			{
				this.m_HasDoneFocusTransition = false;
				this.m_AllowInput = false;
				if (this.m_Placeholder != null)
				{
					this.m_Placeholder.enabled = string.IsNullOrEmpty(this.m_Text);
				}
				if (this.m_TextComponent != null && this.IsInteractable())
				{
					if (this.m_WasCanceled)
					{
						this.text = this.m_OriginalText;
					}
					if (this.m_Keyboard != null)
					{
						this.m_Keyboard.active = false;
						this.m_Keyboard = null;
					}
					this.m_CaretPosition = (this.m_CaretSelectPosition = 0);
					this.SendOnSubmit();
					this.input.imeCompositionMode = IMECompositionMode.Auto;
				}
				this.MarkGeometryAsDirty();
			}
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			this.DeactivateInputField();
			base.OnDeselect(eventData);
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			if (this.IsActive() && this.IsInteractable())
			{
				if (!this.isFocused)
				{
					this.m_ShouldActivateNextUpdate = true;
				}
			}
		}

		private void EnforceContentType()
		{
			switch (this.contentType)
			{
			case InputField.ContentType.Standard:
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = InputField.CharacterValidation.None;
				break;
			case InputField.ContentType.Autocorrected:
				this.m_InputType = InputField.InputType.AutoCorrect;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = InputField.CharacterValidation.None;
				break;
			case InputField.ContentType.IntegerNumber:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = InputField.CharacterValidation.Integer;
				break;
			case InputField.ContentType.DecimalNumber:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
				this.m_CharacterValidation = InputField.CharacterValidation.Decimal;
				break;
			case InputField.ContentType.Alphanumeric:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.ASCIICapable;
				this.m_CharacterValidation = InputField.CharacterValidation.Alphanumeric;
				break;
			case InputField.ContentType.Name:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = InputField.CharacterValidation.Name;
				break;
			case InputField.ContentType.EmailAddress:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.EmailAddress;
				this.m_CharacterValidation = InputField.CharacterValidation.EmailAddress;
				break;
			case InputField.ContentType.Password:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = InputField.CharacterValidation.None;
				break;
			case InputField.ContentType.Pin:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = InputField.CharacterValidation.Integer;
				break;
			}
			this.EnforceTextHOverflow();
		}

		private void EnforceTextHOverflow()
		{
			if (this.m_TextComponent != null)
			{
				if (this.multiLine)
				{
					this.m_TextComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
				}
				else
				{
					this.m_TextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
				}
			}
		}

		private void SetToCustomIfContentTypeIsNot(params InputField.ContentType[] allowedContentTypes)
		{
			if (this.contentType != InputField.ContentType.Custom)
			{
				for (int i = 0; i < allowedContentTypes.Length; i++)
				{
					if (this.contentType == allowedContentTypes[i])
					{
						return;
					}
				}
				this.contentType = InputField.ContentType.Custom;
			}
		}

		private void SetToCustom()
		{
			if (this.contentType != InputField.ContentType.Custom)
			{
				this.contentType = InputField.ContentType.Custom;
			}
		}

		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (this.m_HasDoneFocusTransition)
			{
				state = Selectable.SelectionState.Highlighted;
			}
			else if (state == Selectable.SelectionState.Pressed)
			{
				this.m_HasDoneFocusTransition = true;
			}
			base.DoStateTransition(state, instant);
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
		}

		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}
	}
}
