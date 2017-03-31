using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ExposedReference<>))]
internal class ExposedReferencePropertyDrawer : BaseExposedPropertyDrawer
{
	protected override void OnRenderProperty(Rect position, PropertyName exposedPropertyNameString, UnityEngine.Object currentReferenceValue, SerializedProperty exposedPropertyDefault, SerializedProperty exposedPropertyName, BaseExposedPropertyDrawer.ExposedPropertyMode mode, IExposedPropertyTable exposedPropertyTable)
	{
		Type objType = base.fieldInfo.FieldType.GetGenericArguments()[0];
		EditorGUI.BeginChangeCheck();
		UnityEngine.Object @object = EditorGUI.ObjectField(position, currentReferenceValue, objType, exposedPropertyTable != null);
		if (EditorGUI.EndChangeCheck())
		{
			if (mode == BaseExposedPropertyDrawer.ExposedPropertyMode.DefaultValue)
			{
				if (!EditorUtility.IsPersistent(exposedPropertyDefault.serializedObject.targetObject) || @object == null || EditorUtility.IsPersistent(@object))
				{
					if (!EditorGUI.CheckForCrossSceneReferencing(exposedPropertyDefault.serializedObject.targetObject, @object))
					{
						exposedPropertyDefault.objectReferenceValue = @object;
					}
				}
				else
				{
					string text = GUID.Generate().ToString();
					exposedPropertyNameString = new PropertyName(text);
					exposedPropertyName.stringValue = text;
					Undo.RecordObject(exposedPropertyTable as UnityEngine.Object, "Set Exposed Property");
					exposedPropertyTable.SetReferenceValue(exposedPropertyNameString, @object);
				}
			}
			else
			{
				Undo.RecordObject(exposedPropertyTable as UnityEngine.Object, "Set Exposed Property");
				exposedPropertyTable.SetReferenceValue(exposedPropertyNameString, @object);
			}
		}
	}

	protected override void PopulateContextMenu(GenericMenu menu, BaseExposedPropertyDrawer.OverrideState overrideState, IExposedPropertyTable exposedPropertyTable, SerializedProperty exposedName, SerializedProperty defaultValue)
	{
		PropertyName propertyName = new PropertyName(exposedName.stringValue);
		BaseExposedPropertyDrawer.OverrideState overrideState2;
		UnityEngine.Object currentValue = base.Resolve(new PropertyName(exposedName.stringValue), exposedPropertyTable, defaultValue.objectReferenceValue, out overrideState2);
		if (overrideState == BaseExposedPropertyDrawer.OverrideState.DefaultValue)
		{
			menu.AddItem(new GUIContent(this.ExposePropertyContent.text), false, delegate(object userData)
			{
				GUID gUID = GUID.Generate();
				exposedName.stringValue = gUID.ToString();
				exposedName.serializedObject.ApplyModifiedProperties();
				PropertyName id = new PropertyName(exposedName.stringValue);
				Undo.RecordObject(exposedPropertyTable as UnityEngine.Object, "Set Exposed Property");
				exposedPropertyTable.SetReferenceValue(id, currentValue);
			}, null);
		}
		else
		{
			menu.AddItem(this.UnexposePropertyContent, false, delegate(object userData)
			{
				exposedName.stringValue = "";
				exposedName.serializedObject.ApplyModifiedProperties();
				Undo.RecordObject(exposedPropertyTable as UnityEngine.Object, "Clear Exposed Property");
				exposedPropertyTable.ClearReferenceValue(propertyName);
			}, null);
		}
	}
}
