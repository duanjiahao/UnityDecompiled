using System;
using UnityEditor;
using UnityEngine;

internal abstract class BaseExposedPropertyDrawer : PropertyDrawer
{
	protected enum ExposedPropertyMode
	{
		DefaultValue,
		Named,
		NamedGUID
	}

	protected enum OverrideState
	{
		DefaultValue,
		MissingOverride,
		Overridden
	}

	private static float kDriveWidgetWidth = 18f;

	private static GUIStyle kDropDownStyle = null;

	private static Color kMissingOverrideColor = new Color(1f, 0.11f, 0.11f, 1f);

	protected readonly GUIContent ExposePropertyContent = EditorGUIUtility.TextContent("Expose Property");

	protected readonly GUIContent UnexposePropertyContent = EditorGUIUtility.TextContent("Unexpose Property");

	protected readonly GUIContent NotFoundOn = EditorGUIUtility.TextContent("not found on");

	protected readonly GUIContent OverridenByContent = EditorGUIUtility.TextContent("Overriden by ");

	private GUIContent m_ModifiedLabel = new GUIContent();

	public BaseExposedPropertyDrawer()
	{
		if (BaseExposedPropertyDrawer.kDropDownStyle == null)
		{
			BaseExposedPropertyDrawer.kDropDownStyle = new GUIStyle("ShurikenDropdown");
		}
	}

	private static BaseExposedPropertyDrawer.ExposedPropertyMode GetExposedPropertyMode(string propertyName)
	{
		BaseExposedPropertyDrawer.ExposedPropertyMode result;
		GUID gUID;
		if (string.IsNullOrEmpty(propertyName))
		{
			result = BaseExposedPropertyDrawer.ExposedPropertyMode.DefaultValue;
		}
		else if (GUID.TryParse(propertyName, out gUID))
		{
			result = BaseExposedPropertyDrawer.ExposedPropertyMode.NamedGUID;
		}
		else
		{
			result = BaseExposedPropertyDrawer.ExposedPropertyMode.Named;
		}
		return result;
	}

	protected IExposedPropertyTable GetExposedPropertyTable(SerializedProperty property)
	{
		UnityEngine.Object context = property.serializedObject.context;
		return context as IExposedPropertyTable;
	}

	protected abstract void OnRenderProperty(Rect position, PropertyName exposedPropertyNameString, UnityEngine.Object currentReferenceValue, SerializedProperty exposedPropertyDefault, SerializedProperty exposedPropertyName, BaseExposedPropertyDrawer.ExposedPropertyMode mode, IExposedPropertyTable exposedProperties);

	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		SerializedProperty serializedProperty = prop.FindPropertyRelative("defaultValue");
		SerializedProperty serializedProperty2 = prop.FindPropertyRelative("exposedName");
		string text = serializedProperty2.stringValue;
		BaseExposedPropertyDrawer.ExposedPropertyMode exposedPropertyMode = BaseExposedPropertyDrawer.GetExposedPropertyMode(text);
		Rect rect = position;
		rect.xMax -= BaseExposedPropertyDrawer.kDriveWidgetWidth;
		Rect position2 = position;
		position2.x = rect.xMax;
		position2.width = BaseExposedPropertyDrawer.kDriveWidgetWidth;
		IExposedPropertyTable exposedPropertyTable = this.GetExposedPropertyTable(prop);
		bool flag = exposedPropertyTable != null;
		PropertyName propertyName = new PropertyName(text);
		BaseExposedPropertyDrawer.OverrideState overrideState = BaseExposedPropertyDrawer.OverrideState.DefaultValue;
		UnityEngine.Object currentReferenceValue = this.Resolve(propertyName, exposedPropertyTable, serializedProperty.objectReferenceValue, out overrideState);
		Color color = GUI.color;
		bool boldDefaultFont = EditorGUIUtility.GetBoldDefaultFont();
		Rect position3 = this.DrawLabel(flag, overrideState, label, position, exposedPropertyTable, text, serializedProperty2, serializedProperty);
		EditorGUI.BeginChangeCheck();
		if (exposedPropertyMode == BaseExposedPropertyDrawer.ExposedPropertyMode.DefaultValue || exposedPropertyMode == BaseExposedPropertyDrawer.ExposedPropertyMode.NamedGUID)
		{
			this.OnRenderProperty(position3, propertyName, currentReferenceValue, serializedProperty, serializedProperty2, exposedPropertyMode, exposedPropertyTable);
		}
		else
		{
			position3.width /= 2f;
			EditorGUI.BeginChangeCheck();
			text = EditorGUI.TextField(position3, text);
			if (EditorGUI.EndChangeCheck())
			{
				serializedProperty2.stringValue = text;
			}
			position3.x += position3.width;
			this.OnRenderProperty(position3, new PropertyName(text), currentReferenceValue, serializedProperty, serializedProperty2, exposedPropertyMode, exposedPropertyTable);
		}
		EditorGUI.EndDisabledGroup();
		GUI.color = color;
		EditorGUIUtility.SetBoldDefaultFont(boldDefaultFont);
		if (flag && GUI.Button(position2, GUIContent.none, BaseExposedPropertyDrawer.kDropDownStyle))
		{
			GenericMenu genericMenu = new GenericMenu();
			this.PopulateContextMenu(genericMenu, overrideState, exposedPropertyTable, serializedProperty2, serializedProperty);
			genericMenu.ShowAsContext();
			Event.current.Use();
		}
	}

	private Rect DrawLabel(bool showContextMenu, BaseExposedPropertyDrawer.OverrideState currentOverrideState, GUIContent label, Rect position, IExposedPropertyTable exposedPropertyTable, string exposedNameStr, SerializedProperty exposedName, SerializedProperty defaultValue)
	{
		if (showContextMenu)
		{
			position.xMax -= BaseExposedPropertyDrawer.kDriveWidgetWidth;
		}
		EditorGUIUtility.SetBoldDefaultFont(currentOverrideState != BaseExposedPropertyDrawer.OverrideState.DefaultValue);
		this.m_ModifiedLabel.text = label.text;
		this.m_ModifiedLabel.tooltip = label.tooltip;
		this.m_ModifiedLabel.image = label.image;
		if (!string.IsNullOrEmpty(this.m_ModifiedLabel.tooltip))
		{
			GUIContent expr_78 = this.m_ModifiedLabel;
			expr_78.tooltip += "\n";
		}
		if (currentOverrideState == BaseExposedPropertyDrawer.OverrideState.MissingOverride)
		{
			GUI.color = BaseExposedPropertyDrawer.kMissingOverrideColor;
			GUIContent expr_A6 = this.m_ModifiedLabel;
			string tooltip = expr_A6.tooltip;
			expr_A6.tooltip = string.Concat(new string[]
			{
				tooltip,
				label.text,
				" ",
				this.NotFoundOn.text,
				" ",
				exposedPropertyTable.ToString(),
				"."
			});
		}
		else if (currentOverrideState == BaseExposedPropertyDrawer.OverrideState.Overridden && exposedPropertyTable != null)
		{
			GUIContent expr_115 = this.m_ModifiedLabel;
			expr_115.tooltip = expr_115.tooltip + this.OverridenByContent.text + exposedPropertyTable.ToString() + ".";
		}
		Rect result = EditorGUI.PrefixLabel(position, this.m_ModifiedLabel);
		if (exposedPropertyTable != null && Event.current.type == EventType.ContextClick)
		{
			if (position.Contains(Event.current.mousePosition))
			{
				GenericMenu genericMenu = new GenericMenu();
				this.PopulateContextMenu(genericMenu, (!string.IsNullOrEmpty(exposedNameStr)) ? BaseExposedPropertyDrawer.OverrideState.Overridden : BaseExposedPropertyDrawer.OverrideState.DefaultValue, exposedPropertyTable, exposedName, defaultValue);
				genericMenu.ShowAsContext();
			}
		}
		return result;
	}

	protected UnityEngine.Object Resolve(PropertyName exposedPropertyName, IExposedPropertyTable exposedPropertyTable, UnityEngine.Object defaultValue, out BaseExposedPropertyDrawer.OverrideState currentOverrideState)
	{
		UnityEngine.Object @object = null;
		bool flag = false;
		bool flag2 = !PropertyName.IsNullOrEmpty(exposedPropertyName);
		currentOverrideState = BaseExposedPropertyDrawer.OverrideState.DefaultValue;
		if (exposedPropertyTable != null)
		{
			@object = exposedPropertyTable.GetReferenceValue(exposedPropertyName, out flag);
			if (flag)
			{
				currentOverrideState = BaseExposedPropertyDrawer.OverrideState.Overridden;
			}
			else if (flag2)
			{
				currentOverrideState = BaseExposedPropertyDrawer.OverrideState.MissingOverride;
			}
		}
		return (currentOverrideState != BaseExposedPropertyDrawer.OverrideState.Overridden) ? defaultValue : @object;
	}

	protected abstract void PopulateContextMenu(GenericMenu menu, BaseExposedPropertyDrawer.OverrideState overrideState, IExposedPropertyTable exposedPropertyTable, SerializedProperty exposedName, SerializedProperty defaultValue);
}
