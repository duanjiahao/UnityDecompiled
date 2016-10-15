using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	internal class PropertyHandler
	{
		private PropertyDrawer m_PropertyDrawer;

		private List<DecoratorDrawer> m_DecoratorDrawers;

		public string tooltip;

		public List<ContextMenuItemAttribute> contextMenuItems;

		public bool hasPropertyDrawer
		{
			get
			{
				return this.propertyDrawer != null;
			}
		}

		private PropertyDrawer propertyDrawer
		{
			get
			{
				return (!this.isCurrentlyNested) ? this.m_PropertyDrawer : null;
			}
		}

		private bool isCurrentlyNested
		{
			get
			{
				return this.m_PropertyDrawer != null && ScriptAttributeUtility.s_DrawerStack.Any<PropertyDrawer>() && this.m_PropertyDrawer == ScriptAttributeUtility.s_DrawerStack.Peek();
			}
		}

		public bool empty
		{
			get
			{
				return this.m_DecoratorDrawers == null && this.tooltip == null && this.propertyDrawer == null && this.contextMenuItems == null;
			}
		}

		public void HandleAttribute(PropertyAttribute attribute, FieldInfo field, Type propertyType)
		{
			if (attribute is TooltipAttribute)
			{
				this.tooltip = (attribute as TooltipAttribute).tooltip;
				return;
			}
			if (!(attribute is ContextMenuItemAttribute))
			{
				this.HandleDrawnType(attribute.GetType(), propertyType, field, attribute);
				return;
			}
			if (propertyType.IsArrayOrList())
			{
				return;
			}
			if (this.contextMenuItems == null)
			{
				this.contextMenuItems = new List<ContextMenuItemAttribute>();
			}
			this.contextMenuItems.Add(attribute as ContextMenuItemAttribute);
		}

		public void HandleDrawnType(Type drawnType, Type propertyType, FieldInfo field, PropertyAttribute attribute)
		{
			Type drawerTypeForType = ScriptAttributeUtility.GetDrawerTypeForType(drawnType);
			if (drawerTypeForType != null)
			{
				if (typeof(PropertyDrawer).IsAssignableFrom(drawerTypeForType))
				{
					if (propertyType != null && propertyType.IsArrayOrList())
					{
						return;
					}
					this.m_PropertyDrawer = (PropertyDrawer)Activator.CreateInstance(drawerTypeForType);
					this.m_PropertyDrawer.m_FieldInfo = field;
					this.m_PropertyDrawer.m_Attribute = attribute;
				}
				else if (typeof(DecoratorDrawer).IsAssignableFrom(drawerTypeForType))
				{
					if (field != null && field.FieldType.IsArrayOrList() && !propertyType.IsArrayOrList())
					{
						return;
					}
					DecoratorDrawer decoratorDrawer = (DecoratorDrawer)Activator.CreateInstance(drawerTypeForType);
					decoratorDrawer.m_Attribute = attribute;
					if (this.m_DecoratorDrawers == null)
					{
						this.m_DecoratorDrawers = new List<DecoratorDrawer>();
					}
					this.m_DecoratorDrawers.Add(decoratorDrawer);
				}
			}
		}

		public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
		{
			float num = position.height;
			position.height = 0f;
			if (this.m_DecoratorDrawers != null && !this.isCurrentlyNested)
			{
				foreach (DecoratorDrawer current in this.m_DecoratorDrawers)
				{
					position.height = current.GetHeight();
					float labelWidth = EditorGUIUtility.labelWidth;
					float fieldWidth = EditorGUIUtility.fieldWidth;
					current.OnGUI(position);
					EditorGUIUtility.labelWidth = labelWidth;
					EditorGUIUtility.fieldWidth = fieldWidth;
					position.y += position.height;
					num -= position.height;
				}
			}
			position.height = num;
			if (this.propertyDrawer != null)
			{
				float labelWidth = EditorGUIUtility.labelWidth;
				float fieldWidth = EditorGUIUtility.fieldWidth;
				this.propertyDrawer.OnGUISafe(position, property.Copy(), label ?? EditorGUIUtility.TempContent(property.displayName));
				EditorGUIUtility.labelWidth = labelWidth;
				EditorGUIUtility.fieldWidth = fieldWidth;
				return false;
			}
			if (!includeChildren)
			{
				return EditorGUI.DefaultPropertyField(position, property, label);
			}
			Vector2 iconSize = EditorGUIUtility.GetIconSize();
			bool enabled = GUI.enabled;
			int indentLevel = EditorGUI.indentLevel;
			int num2 = indentLevel - property.depth;
			SerializedProperty serializedProperty = property.Copy();
			SerializedProperty endProperty = serializedProperty.GetEndProperty();
			position.height = EditorGUI.GetSinglePropertyHeight(serializedProperty, label);
			EditorGUI.indentLevel = serializedProperty.depth + num2;
			bool enterChildren = EditorGUI.DefaultPropertyField(position, serializedProperty, label) && EditorGUI.HasVisibleChildFields(serializedProperty);
			position.y += position.height + 2f;
			while (serializedProperty.NextVisible(enterChildren) && !SerializedProperty.EqualContents(serializedProperty, endProperty))
			{
				EditorGUI.indentLevel = serializedProperty.depth + num2;
				position.height = EditorGUI.GetPropertyHeight(serializedProperty, null, false);
				EditorGUI.BeginChangeCheck();
				enterChildren = (ScriptAttributeUtility.GetHandler(serializedProperty).OnGUI(position, serializedProperty, null, false) && EditorGUI.HasVisibleChildFields(serializedProperty));
				if (EditorGUI.EndChangeCheck())
				{
					break;
				}
				position.y += position.height + 2f;
			}
			GUI.enabled = enabled;
			EditorGUIUtility.SetIconSize(iconSize);
			EditorGUI.indentLevel = indentLevel;
			return false;
		}

		public bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
		{
			Rect rect;
			if (property.propertyType == SerializedPropertyType.Boolean && this.propertyDrawer == null && (this.m_DecoratorDrawers == null || this.m_DecoratorDrawers.Count == 0))
			{
				rect = EditorGUILayout.GetToggleRect(true, options);
			}
			else
			{
				rect = EditorGUILayout.GetControlRect(EditorGUI.LabelHasContent(label), this.GetHeight(property, label, includeChildren), options);
			}
			EditorGUILayout.s_LastRect = rect;
			return this.OnGUI(rect, property, label, includeChildren);
		}

		public float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
		{
			float num = 0f;
			if (this.m_DecoratorDrawers != null && !this.isCurrentlyNested)
			{
				foreach (DecoratorDrawer current in this.m_DecoratorDrawers)
				{
					num += current.GetHeight();
				}
			}
			if (this.propertyDrawer != null)
			{
				num += this.propertyDrawer.GetPropertyHeightSafe(property.Copy(), label ?? EditorGUIUtility.TempContent(property.displayName));
			}
			else if (!includeChildren)
			{
				num += EditorGUI.GetSinglePropertyHeight(property, label);
			}
			else
			{
				property = property.Copy();
				SerializedProperty endProperty = property.GetEndProperty();
				num += EditorGUI.GetSinglePropertyHeight(property, label);
				bool enterChildren = property.isExpanded && EditorGUI.HasVisibleChildFields(property);
				while (property.NextVisible(enterChildren) && !SerializedProperty.EqualContents(property, endProperty))
				{
					num += ScriptAttributeUtility.GetHandler(property).GetHeight(property, EditorGUIUtility.TempContent(property.displayName), true);
					enterChildren = false;
					num += 2f;
				}
			}
			return num;
		}

		public void AddMenuItems(SerializedProperty property, GenericMenu menu)
		{
			if (this.contextMenuItems == null)
			{
				return;
			}
			Type type = property.serializedObject.targetObject.GetType();
			foreach (ContextMenuItemAttribute current in this.contextMenuItems)
			{
				MethodInfo method = type.GetMethod(current.function, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					menu.AddItem(new GUIContent(current.name), false, delegate
					{
						this.CallMenuCallback(property.serializedObject.targetObjects, method);
					});
				}
			}
		}

		public void CallMenuCallback(object[] targets, MethodInfo method)
		{
			for (int i = 0; i < targets.Length; i++)
			{
				object obj = targets[i];
				method.Invoke(obj, new object[0]);
			}
		}
	}
}
