using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditorInternal
{
	[CustomPropertyDrawer(typeof(UnityEventBase), true)]
	public class UnityEventDrawer : PropertyDrawer
	{
		protected class State
		{
			internal ReorderableList m_ReorderableList;

			public int lastSelectedIndex;
		}

		private class Styles
		{
			public readonly GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus");

			public readonly GUIStyle genericFieldStyle = EditorStyles.label;

			public readonly GUIStyle removeButton = "InvisibleButton";
		}

		private struct ValidMethodMap
		{
			public UnityEngine.Object target;

			public MethodInfo methodInfo;

			public PersistentListenerMode mode;
		}

		private struct UnityEventFunction
		{
			private readonly SerializedProperty m_Listener;

			private readonly UnityEngine.Object m_Target;

			private readonly MethodInfo m_Method;

			private readonly PersistentListenerMode m_Mode;

			public UnityEventFunction(SerializedProperty listener, UnityEngine.Object target, MethodInfo method, PersistentListenerMode mode)
			{
				this.m_Listener = listener;
				this.m_Target = target;
				this.m_Method = method;
				this.m_Mode = mode;
			}

			public void Assign()
			{
				SerializedProperty serializedProperty = this.m_Listener.FindPropertyRelative("m_Target");
				SerializedProperty serializedProperty2 = this.m_Listener.FindPropertyRelative("m_MethodName");
				SerializedProperty serializedProperty3 = this.m_Listener.FindPropertyRelative("m_Mode");
				SerializedProperty serializedProperty4 = this.m_Listener.FindPropertyRelative("m_Arguments");
				serializedProperty.objectReferenceValue = this.m_Target;
				serializedProperty2.stringValue = this.m_Method.Name;
				serializedProperty3.enumValueIndex = (int)this.m_Mode;
				if (this.m_Mode == PersistentListenerMode.Object)
				{
					SerializedProperty serializedProperty5 = serializedProperty4.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
					ParameterInfo[] parameters = this.m_Method.GetParameters();
					if (parameters.Length == 1 && typeof(UnityEngine.Object).IsAssignableFrom(parameters[0].ParameterType))
					{
						serializedProperty5.stringValue = parameters[0].ParameterType.AssemblyQualifiedName;
					}
					else
					{
						serializedProperty5.stringValue = typeof(UnityEngine.Object).AssemblyQualifiedName;
					}
				}
				this.ValidateObjectParamater(serializedProperty4, this.m_Mode);
				this.m_Listener.m_SerializedObject.ApplyModifiedProperties();
			}

			private void ValidateObjectParamater(SerializedProperty arguments, PersistentListenerMode mode)
			{
				SerializedProperty serializedProperty = arguments.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
				SerializedProperty serializedProperty2 = arguments.FindPropertyRelative("m_ObjectArgument");
				UnityEngine.Object objectReferenceValue = serializedProperty2.objectReferenceValue;
				if (mode != PersistentListenerMode.Object)
				{
					serializedProperty.stringValue = typeof(UnityEngine.Object).AssemblyQualifiedName;
					serializedProperty2.objectReferenceValue = null;
				}
				else if (!(objectReferenceValue == null))
				{
					Type type = Type.GetType(serializedProperty.stringValue, false);
					if (!typeof(UnityEngine.Object).IsAssignableFrom(type) || !type.IsInstanceOfType(objectReferenceValue))
					{
						serializedProperty2.objectReferenceValue = null;
					}
				}
			}

			public void Clear()
			{
				SerializedProperty serializedProperty = this.m_Listener.FindPropertyRelative("m_MethodName");
				serializedProperty.stringValue = null;
				SerializedProperty serializedProperty2 = this.m_Listener.FindPropertyRelative("m_Mode");
				serializedProperty2.enumValueIndex = 1;
				this.m_Listener.m_SerializedObject.ApplyModifiedProperties();
			}
		}

		private const string kNoFunctionString = "No Function";

		private const string kInstancePath = "m_Target";

		private const string kCallStatePath = "m_CallState";

		private const string kArgumentsPath = "m_Arguments";

		private const string kModePath = "m_Mode";

		private const string kMethodNamePath = "m_MethodName";

		private const string kFloatArgument = "m_FloatArgument";

		private const string kIntArgument = "m_IntArgument";

		private const string kObjectArgument = "m_ObjectArgument";

		private const string kStringArgument = "m_StringArgument";

		private const string kBoolArgument = "m_BoolArgument";

		private const string kObjectArgumentAssemblyTypeName = "m_ObjectArgumentAssemblyTypeName";

		private UnityEventDrawer.Styles m_Styles;

		private string m_Text;

		private UnityEventBase m_DummyEvent;

		private SerializedProperty m_Prop;

		private SerializedProperty m_ListenersArray;

		private const int kExtraSpacing = 9;

		private ReorderableList m_ReorderableList;

		private int m_LastSelectedIndex;

		private Dictionary<string, UnityEventDrawer.State> m_States = new Dictionary<string, UnityEventDrawer.State>();

		[CompilerGenerated]
		private static GenericMenu.MenuFunction2 <>f__mg$cache0;

		[CompilerGenerated]
		private static GenericMenu.MenuFunction2 <>f__mg$cache1;

		private static string GetEventParams(UnityEventBase evt)
		{
			MethodInfo methodInfo = evt.FindMethod("Invoke", evt, PersistentListenerMode.EventDefined, null);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" (");
			Type[] array = (from x in methodInfo.GetParameters()
			select x.ParameterType).ToArray<Type>();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].Name);
				if (i < array.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private UnityEventDrawer.State GetState(SerializedProperty prop)
		{
			string propertyPath = prop.propertyPath;
			UnityEventDrawer.State state;
			this.m_States.TryGetValue(propertyPath, out state);
			if (state == null)
			{
				state = new UnityEventDrawer.State();
				SerializedProperty elements = prop.FindPropertyRelative("m_PersistentCalls.m_Calls");
				state.m_ReorderableList = new ReorderableList(prop.serializedObject, elements, false, true, true, true);
				state.m_ReorderableList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawEventHeader);
				state.m_ReorderableList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawEventListener);
				state.m_ReorderableList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.SelectEventListener);
				state.m_ReorderableList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.EndDragChild);
				state.m_ReorderableList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddEventListener);
				state.m_ReorderableList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveButton);
				state.m_ReorderableList.elementHeight = 43f;
				this.m_States[propertyPath] = state;
			}
			return state;
		}

		private UnityEventDrawer.State RestoreState(SerializedProperty property)
		{
			UnityEventDrawer.State state = this.GetState(property);
			this.m_ListenersArray = state.m_ReorderableList.serializedProperty;
			this.m_ReorderableList = state.m_ReorderableList;
			this.m_LastSelectedIndex = state.lastSelectedIndex;
			this.m_ReorderableList.index = this.m_LastSelectedIndex;
			return state;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			this.m_Prop = property;
			this.m_Text = label.text;
			UnityEventDrawer.State state = this.RestoreState(property);
			this.OnGUI(position);
			state.lastSelectedIndex = this.m_LastSelectedIndex;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			this.RestoreState(property);
			float result = 0f;
			if (this.m_ReorderableList != null)
			{
				result = this.m_ReorderableList.GetHeight();
			}
			return result;
		}

		public void OnGUI(Rect position)
		{
			if (this.m_ListenersArray != null && this.m_ListenersArray.isArray)
			{
				this.m_DummyEvent = UnityEventDrawer.GetDummyEvent(this.m_Prop);
				if (this.m_DummyEvent != null)
				{
					if (this.m_Styles == null)
					{
						this.m_Styles = new UnityEventDrawer.Styles();
					}
					if (this.m_ReorderableList != null)
					{
						int indentLevel = EditorGUI.indentLevel;
						EditorGUI.indentLevel = 0;
						this.m_ReorderableList.DoList(position);
						EditorGUI.indentLevel = indentLevel;
					}
				}
			}
		}

		protected virtual void DrawEventHeader(Rect headerRect)
		{
			headerRect.height = 16f;
			string text = ((!string.IsNullOrEmpty(this.m_Text)) ? this.m_Text : "Event") + UnityEventDrawer.GetEventParams(this.m_DummyEvent);
			GUI.Label(headerRect, text);
		}

		private static PersistentListenerMode GetMode(SerializedProperty mode)
		{
			return (PersistentListenerMode)mode.enumValueIndex;
		}

		private void DrawEventListener(Rect rect, int index, bool isactive, bool isfocused)
		{
			SerializedProperty arrayElementAtIndex = this.m_ListenersArray.GetArrayElementAtIndex(index);
			rect.y += 1f;
			Rect[] rowRects = this.GetRowRects(rect);
			Rect position = rowRects[0];
			Rect position2 = rowRects[1];
			Rect rect2 = rowRects[2];
			Rect position3 = rowRects[3];
			SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("m_CallState");
			SerializedProperty mode = arrayElementAtIndex.FindPropertyRelative("m_Mode");
			SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("m_Arguments");
			SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("m_Target");
			SerializedProperty serializedProperty3 = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.white;
			EditorGUI.PropertyField(position, property, GUIContent.none);
			EditorGUI.BeginChangeCheck();
			GUI.Box(position2, GUIContent.none);
			EditorGUI.PropertyField(position2, serializedProperty2, GUIContent.none);
			if (EditorGUI.EndChangeCheck())
			{
				serializedProperty3.stringValue = null;
			}
			PersistentListenerMode persistentListenerMode = UnityEventDrawer.GetMode(mode);
			if (serializedProperty2.objectReferenceValue == null || string.IsNullOrEmpty(serializedProperty3.stringValue))
			{
				persistentListenerMode = PersistentListenerMode.Void;
			}
			SerializedProperty serializedProperty4;
			switch (persistentListenerMode)
			{
			case PersistentListenerMode.Object:
				serializedProperty4 = serializedProperty.FindPropertyRelative("m_ObjectArgument");
				break;
			case PersistentListenerMode.Int:
				serializedProperty4 = serializedProperty.FindPropertyRelative("m_IntArgument");
				break;
			case PersistentListenerMode.Float:
				serializedProperty4 = serializedProperty.FindPropertyRelative("m_FloatArgument");
				break;
			case PersistentListenerMode.String:
				serializedProperty4 = serializedProperty.FindPropertyRelative("m_StringArgument");
				break;
			case PersistentListenerMode.Bool:
				serializedProperty4 = serializedProperty.FindPropertyRelative("m_BoolArgument");
				break;
			default:
				serializedProperty4 = serializedProperty.FindPropertyRelative("m_IntArgument");
				break;
			}
			string stringValue = serializedProperty.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;
			Type type = typeof(UnityEngine.Object);
			if (!string.IsNullOrEmpty(stringValue))
			{
				type = (Type.GetType(stringValue, false) ?? typeof(UnityEngine.Object));
			}
			if (persistentListenerMode == PersistentListenerMode.Object)
			{
				EditorGUI.BeginChangeCheck();
				UnityEngine.Object objectReferenceValue = EditorGUI.ObjectField(position3, GUIContent.none, serializedProperty4.objectReferenceValue, type, true);
				if (EditorGUI.EndChangeCheck())
				{
					serializedProperty4.objectReferenceValue = objectReferenceValue;
				}
			}
			else if (persistentListenerMode != PersistentListenerMode.Void && persistentListenerMode != PersistentListenerMode.EventDefined)
			{
				EditorGUI.PropertyField(position3, serializedProperty4, GUIContent.none);
			}
			using (new EditorGUI.DisabledScope(serializedProperty2.objectReferenceValue == null))
			{
				EditorGUI.BeginProperty(rect2, GUIContent.none, serializedProperty3);
				GUIContent content;
				if (EditorGUI.showMixedValue)
				{
					content = EditorGUI.mixedValueContent;
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (serializedProperty2.objectReferenceValue == null || string.IsNullOrEmpty(serializedProperty3.stringValue))
					{
						stringBuilder.Append("No Function");
					}
					else if (!UnityEventDrawer.IsPersistantListenerValid(this.m_DummyEvent, serializedProperty3.stringValue, serializedProperty2.objectReferenceValue, UnityEventDrawer.GetMode(mode), type))
					{
						string arg = "UnknownComponent";
						UnityEngine.Object objectReferenceValue2 = serializedProperty2.objectReferenceValue;
						if (objectReferenceValue2 != null)
						{
							arg = objectReferenceValue2.GetType().Name;
						}
						stringBuilder.Append(string.Format("<Missing {0}.{1}>", arg, serializedProperty3.stringValue));
					}
					else
					{
						stringBuilder.Append(serializedProperty2.objectReferenceValue.GetType().Name);
						if (!string.IsNullOrEmpty(serializedProperty3.stringValue))
						{
							stringBuilder.Append(".");
							if (serializedProperty3.stringValue.StartsWith("set_"))
							{
								stringBuilder.Append(serializedProperty3.stringValue.Substring(4));
							}
							else
							{
								stringBuilder.Append(serializedProperty3.stringValue);
							}
						}
					}
					content = GUIContent.Temp(stringBuilder.ToString());
				}
				if (GUI.Button(rect2, content, EditorStyles.popup))
				{
					UnityEventDrawer.BuildPopupList(serializedProperty2.objectReferenceValue, this.m_DummyEvent, arrayElementAtIndex).DropDown(rect2);
				}
				EditorGUI.EndProperty();
			}
			GUI.backgroundColor = backgroundColor;
		}

		private Rect[] GetRowRects(Rect rect)
		{
			Rect[] array = new Rect[4];
			rect.height = 16f;
			rect.y += 2f;
			Rect rect2 = rect;
			rect2.width *= 0.3f;
			Rect rect3 = rect2;
			rect3.y += EditorGUIUtility.singleLineHeight + 2f;
			Rect rect4 = rect;
			rect4.xMin = rect3.xMax + 5f;
			Rect rect5 = rect4;
			rect5.y += EditorGUIUtility.singleLineHeight + 2f;
			array[0] = rect2;
			array[1] = rect3;
			array[2] = rect4;
			array[3] = rect5;
			return array;
		}

		private void RemoveButton(ReorderableList list)
		{
			ReorderableList.defaultBehaviours.DoRemoveButton(list);
			this.m_LastSelectedIndex = list.index;
		}

		private void AddEventListener(ReorderableList list)
		{
			if (this.m_ListenersArray.hasMultipleDifferentValues)
			{
				UnityEngine.Object[] targetObjects = this.m_ListenersArray.serializedObject.targetObjects;
				for (int i = 0; i < targetObjects.Length; i++)
				{
					UnityEngine.Object obj = targetObjects[i];
					SerializedObject serializedObject = new SerializedObject(obj);
					SerializedProperty serializedProperty = serializedObject.FindProperty(this.m_ListenersArray.propertyPath);
					serializedProperty.arraySize++;
					serializedObject.ApplyModifiedProperties();
				}
				this.m_ListenersArray.serializedObject.SetIsDifferentCacheDirty();
				this.m_ListenersArray.serializedObject.Update();
				list.index = list.serializedProperty.arraySize - 1;
			}
			else
			{
				ReorderableList.defaultBehaviours.DoAddButton(list);
			}
			this.m_LastSelectedIndex = list.index;
			SerializedProperty arrayElementAtIndex = this.m_ListenersArray.GetArrayElementAtIndex(list.index);
			SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("m_CallState");
			SerializedProperty serializedProperty3 = arrayElementAtIndex.FindPropertyRelative("m_Target");
			SerializedProperty serializedProperty4 = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
			SerializedProperty serializedProperty5 = arrayElementAtIndex.FindPropertyRelative("m_Mode");
			SerializedProperty serializedProperty6 = arrayElementAtIndex.FindPropertyRelative("m_Arguments");
			serializedProperty2.enumValueIndex = 2;
			serializedProperty3.objectReferenceValue = null;
			serializedProperty4.stringValue = null;
			serializedProperty5.enumValueIndex = 1;
			serializedProperty6.FindPropertyRelative("m_FloatArgument").floatValue = 0f;
			serializedProperty6.FindPropertyRelative("m_IntArgument").intValue = 0;
			serializedProperty6.FindPropertyRelative("m_ObjectArgument").objectReferenceValue = null;
			serializedProperty6.FindPropertyRelative("m_StringArgument").stringValue = null;
			serializedProperty6.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue = null;
		}

		private void SelectEventListener(ReorderableList list)
		{
			this.m_LastSelectedIndex = list.index;
		}

		private void EndDragChild(ReorderableList list)
		{
			this.m_LastSelectedIndex = list.index;
		}

		private static UnityEventBase GetDummyEvent(SerializedProperty prop)
		{
			string stringValue = prop.FindPropertyRelative("m_TypeName").stringValue;
			Type type = Type.GetType(stringValue, false);
			UnityEventBase result;
			if (type == null)
			{
				result = new UnityEvent();
			}
			else
			{
				result = (Activator.CreateInstance(type) as UnityEventBase);
			}
			return result;
		}

		private static IEnumerable<UnityEventDrawer.ValidMethodMap> CalculateMethodMap(UnityEngine.Object target, Type[] t, bool allowSubclasses)
		{
			List<UnityEventDrawer.ValidMethodMap> list = new List<UnityEventDrawer.ValidMethodMap>();
			IEnumerable<UnityEventDrawer.ValidMethodMap> result;
			if (target == null || t == null)
			{
				result = list;
			}
			else
			{
				Type type = target.GetType();
				List<MethodInfo> list2 = (from x in type.GetMethods()
				where !x.IsSpecialName
				select x).ToList<MethodInfo>();
				IEnumerable<PropertyInfo> source = type.GetProperties().AsEnumerable<PropertyInfo>();
				source = from x in source
				where x.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0 && x.GetSetMethod() != null
				select x;
				list2.AddRange(from x in source
				select x.GetSetMethod());
				foreach (MethodInfo current in list2)
				{
					ParameterInfo[] parameters = current.GetParameters();
					if (parameters.Length == t.Length)
					{
						if (current.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length <= 0)
						{
							if (current.ReturnType == typeof(void))
							{
								bool flag = true;
								for (int i = 0; i < t.Length; i++)
								{
									if (!parameters[i].ParameterType.IsAssignableFrom(t[i]))
									{
										flag = false;
									}
									if (allowSubclasses && t[i].IsAssignableFrom(parameters[i].ParameterType))
									{
										flag = true;
									}
								}
								if (flag)
								{
									list.Add(new UnityEventDrawer.ValidMethodMap
									{
										target = target,
										methodInfo = current
									});
								}
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		public static bool IsPersistantListenerValid(UnityEventBase dummyEvent, string methodName, UnityEngine.Object uObject, PersistentListenerMode modeEnum, Type argumentType)
		{
			return !(uObject == null) && !string.IsNullOrEmpty(methodName) && dummyEvent.FindMethod(methodName, uObject, modeEnum, argumentType) != null;
		}

		private static GenericMenu BuildPopupList(UnityEngine.Object target, UnityEventBase dummyEvent, SerializedProperty listener)
		{
			UnityEngine.Object @object = target;
			if (@object is Component)
			{
				@object = (target as Component).gameObject;
			}
			SerializedProperty serializedProperty = listener.FindPropertyRelative("m_MethodName");
			GenericMenu genericMenu = new GenericMenu();
			GenericMenu arg_6D_0 = genericMenu;
			GUIContent arg_6D_1 = new GUIContent("No Function");
			bool arg_6D_2 = string.IsNullOrEmpty(serializedProperty.stringValue);
			if (UnityEventDrawer.<>f__mg$cache0 == null)
			{
				UnityEventDrawer.<>f__mg$cache0 = new GenericMenu.MenuFunction2(UnityEventDrawer.ClearEventFunction);
			}
			arg_6D_0.AddItem(arg_6D_1, arg_6D_2, UnityEventDrawer.<>f__mg$cache0, new UnityEventDrawer.UnityEventFunction(listener, null, null, PersistentListenerMode.EventDefined));
			GenericMenu result;
			if (@object == null)
			{
				result = genericMenu;
			}
			else
			{
				genericMenu.AddSeparator("");
				Type type = dummyEvent.GetType();
				MethodInfo method = type.GetMethod("Invoke");
				Type[] delegateArgumentsTypes = (from x in method.GetParameters()
				select x.ParameterType).ToArray<Type>();
				UnityEventDrawer.GeneratePopUpForType(genericMenu, @object, false, listener, delegateArgumentsTypes);
				if (@object is GameObject)
				{
					Component[] components = (@object as GameObject).GetComponents<Component>();
					List<string> list = (from c in components
					where c != null
					select c.GetType().Name into x
					group x by x into g
					where g.Count<string>() > 1
					select g.Key).ToList<string>();
					Component[] array = components;
					for (int i = 0; i < array.Length; i++)
					{
						Component component = array[i];
						if (!(component == null))
						{
							UnityEventDrawer.GeneratePopUpForType(genericMenu, component, list.Contains(component.GetType().Name), listener, delegateArgumentsTypes);
						}
					}
				}
				result = genericMenu;
			}
			return result;
		}

		private static void GeneratePopUpForType(GenericMenu menu, UnityEngine.Object target, bool useFullTargetName, SerializedProperty listener, Type[] delegateArgumentsTypes)
		{
			List<UnityEventDrawer.ValidMethodMap> list = new List<UnityEventDrawer.ValidMethodMap>();
			string text = (!useFullTargetName) ? target.GetType().Name : target.GetType().FullName;
			bool flag = false;
			if (delegateArgumentsTypes.Length != 0)
			{
				UnityEventDrawer.GetMethodsForTargetAndMode(target, delegateArgumentsTypes, list, PersistentListenerMode.EventDefined);
				if (list.Count > 0)
				{
					menu.AddDisabledItem(new GUIContent(text + "/Dynamic " + string.Join(", ", (from e in delegateArgumentsTypes
					select UnityEventDrawer.GetTypeName(e)).ToArray<string>())));
					UnityEventDrawer.AddMethodsToMenu(menu, listener, list, text);
					flag = true;
				}
			}
			list.Clear();
			UnityEventDrawer.GetMethodsForTargetAndMode(target, new Type[]
			{
				typeof(float)
			}, list, PersistentListenerMode.Float);
			UnityEventDrawer.GetMethodsForTargetAndMode(target, new Type[]
			{
				typeof(int)
			}, list, PersistentListenerMode.Int);
			UnityEventDrawer.GetMethodsForTargetAndMode(target, new Type[]
			{
				typeof(string)
			}, list, PersistentListenerMode.String);
			UnityEventDrawer.GetMethodsForTargetAndMode(target, new Type[]
			{
				typeof(bool)
			}, list, PersistentListenerMode.Bool);
			UnityEventDrawer.GetMethodsForTargetAndMode(target, new Type[]
			{
				typeof(UnityEngine.Object)
			}, list, PersistentListenerMode.Object);
			UnityEventDrawer.GetMethodsForTargetAndMode(target, new Type[0], list, PersistentListenerMode.Void);
			if (list.Count > 0)
			{
				if (flag)
				{
					menu.AddItem(new GUIContent(text + "/ "), false, null);
				}
				if (delegateArgumentsTypes.Length != 0)
				{
					menu.AddDisabledItem(new GUIContent(text + "/Static Parameters"));
				}
				UnityEventDrawer.AddMethodsToMenu(menu, listener, list, text);
			}
		}

		private static void AddMethodsToMenu(GenericMenu menu, SerializedProperty listener, List<UnityEventDrawer.ValidMethodMap> methods, string targetName)
		{
			IEnumerable<UnityEventDrawer.ValidMethodMap> enumerable = from e in methods
			orderby (!e.methodInfo.Name.StartsWith("set_")) ? 1 : 0, e.methodInfo.Name
			select e;
			foreach (UnityEventDrawer.ValidMethodMap current in enumerable)
			{
				UnityEventDrawer.AddFunctionsForScript(menu, listener, current, targetName);
			}
		}

		private static void GetMethodsForTargetAndMode(UnityEngine.Object target, Type[] delegateArgumentsTypes, List<UnityEventDrawer.ValidMethodMap> methods, PersistentListenerMode mode)
		{
			IEnumerable<UnityEventDrawer.ValidMethodMap> enumerable = UnityEventDrawer.CalculateMethodMap(target, delegateArgumentsTypes, mode == PersistentListenerMode.Object);
			foreach (UnityEventDrawer.ValidMethodMap current in enumerable)
			{
				UnityEventDrawer.ValidMethodMap item = current;
				item.mode = mode;
				methods.Add(item);
			}
		}

		private static void AddFunctionsForScript(GenericMenu menu, SerializedProperty listener, UnityEventDrawer.ValidMethodMap method, string targetName)
		{
			PersistentListenerMode mode = method.mode;
			UnityEngine.Object objectReferenceValue = listener.FindPropertyRelative("m_Target").objectReferenceValue;
			string stringValue = listener.FindPropertyRelative("m_MethodName").stringValue;
			PersistentListenerMode mode2 = UnityEventDrawer.GetMode(listener.FindPropertyRelative("m_Mode"));
			SerializedProperty serializedProperty = listener.FindPropertyRelative("m_Arguments").FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
			StringBuilder stringBuilder = new StringBuilder();
			int num = method.methodInfo.GetParameters().Length;
			for (int i = 0; i < num; i++)
			{
				ParameterInfo parameterInfo = method.methodInfo.GetParameters()[i];
				stringBuilder.Append(string.Format("{0}", UnityEventDrawer.GetTypeName(parameterInfo.ParameterType)));
				if (i < num - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			bool flag = objectReferenceValue == method.target && stringValue == method.methodInfo.Name && mode == mode2;
			if (flag && mode == PersistentListenerMode.Object && method.methodInfo.GetParameters().Length == 1)
			{
				flag &= (method.methodInfo.GetParameters()[0].ParameterType.AssemblyQualifiedName == serializedProperty.stringValue);
			}
			string formattedMethodName = UnityEventDrawer.GetFormattedMethodName(targetName, method.methodInfo.Name, stringBuilder.ToString(), mode == PersistentListenerMode.EventDefined);
			GUIContent arg_1A9_1 = new GUIContent(formattedMethodName);
			bool arg_1A9_2 = flag;
			if (UnityEventDrawer.<>f__mg$cache1 == null)
			{
				UnityEventDrawer.<>f__mg$cache1 = new GenericMenu.MenuFunction2(UnityEventDrawer.SetEventFunction);
			}
			menu.AddItem(arg_1A9_1, arg_1A9_2, UnityEventDrawer.<>f__mg$cache1, new UnityEventDrawer.UnityEventFunction(listener, method.target, method.methodInfo, mode));
		}

		private static string GetTypeName(Type t)
		{
			string result;
			if (t == typeof(int))
			{
				result = "int";
			}
			else if (t == typeof(float))
			{
				result = "float";
			}
			else if (t == typeof(string))
			{
				result = "string";
			}
			else if (t == typeof(bool))
			{
				result = "bool";
			}
			else
			{
				result = t.Name;
			}
			return result;
		}

		private static string GetFormattedMethodName(string targetName, string methodName, string args, bool dynamic)
		{
			string result;
			if (dynamic)
			{
				if (methodName.StartsWith("set_"))
				{
					result = string.Format("{0}/{1}", targetName, methodName.Substring(4));
				}
				else
				{
					result = string.Format("{0}/{1}", targetName, methodName);
				}
			}
			else if (methodName.StartsWith("set_"))
			{
				result = string.Format("{0}/{2} {1}", targetName, methodName.Substring(4), args);
			}
			else
			{
				result = string.Format("{0}/{1} ({2})", targetName, methodName, args);
			}
			return result;
		}

		private static void SetEventFunction(object source)
		{
			((UnityEventDrawer.UnityEventFunction)source).Assign();
		}

		private static void ClearEventFunction(object source)
		{
			((UnityEventDrawer.UnityEventFunction)source).Clear();
		}
	}
}
