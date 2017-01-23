using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(AnimationWindowEvent))]
	internal class AnimationWindowEventInspector : Editor
	{
		private const string kNotSupportedPostFix = " (Function Not Supported)";

		private const string kNoneSelected = "(No Function Selected)";

		public override void OnInspectorGUI()
		{
			AnimationWindowEventInspector.OnEditAnimationEvent(base.target as AnimationWindowEvent);
		}

		public static void OnEditAnimationEvent(AnimationWindowEvent awevt)
		{
			AnimationEvent[] array = null;
			if (awevt.clip != null)
			{
				array = AnimationUtility.GetAnimationEvents(awevt.clip);
			}
			else if (awevt.clipInfo != null)
			{
				array = awevt.clipInfo.GetEvents();
			}
			if (array != null && awevt.eventIndex >= 0 && awevt.eventIndex < array.Length)
			{
				AnimationEvent evt = array[awevt.eventIndex];
				GUI.changed = false;
				if (awevt.root != null)
				{
					List<AnimationWindowEventMethod> list = AnimationWindowEventInspector.CollectSupportedMethods(awevt);
					List<string> list2 = new List<string>(list.Count);
					for (int i = 0; i < list.Count; i++)
					{
						AnimationWindowEventMethod animationWindowEventMethod = list[i];
						string str = " ( )";
						if (animationWindowEventMethod.parameterType != null)
						{
							if (animationWindowEventMethod.parameterType == typeof(float))
							{
								str = " ( float )";
							}
							else if (animationWindowEventMethod.parameterType == typeof(int))
							{
								str = " ( int )";
							}
							else
							{
								str = string.Format(" ( {0} )", animationWindowEventMethod.parameterType.Name);
							}
						}
						list2.Add(animationWindowEventMethod.name + str);
					}
					int count = list.Count;
					int num = list.FindIndex((AnimationWindowEventMethod method) => method.name == evt.functionName);
					if (num == -1)
					{
						num = list.Count;
						list.Add(new AnimationWindowEventMethod
						{
							name = evt.functionName,
							parameterType = null
						});
						if (string.IsNullOrEmpty(evt.functionName))
						{
							list2.Add("(No Function Selected)");
						}
						else
						{
							list2.Add(evt.functionName + " (Function Not Supported)");
						}
					}
					EditorGUIUtility.labelWidth = 130f;
					int num2 = num;
					num = EditorGUILayout.Popup("Function: ", num, list2.ToArray(), new GUILayoutOption[0]);
					if (num2 != num && num != -1 && num != count)
					{
						evt.functionName = list[num].name;
						evt.stringParameter = string.Empty;
					}
					Type parameterType = list[num].parameterType;
					if (parameterType != null)
					{
						EditorGUILayout.Space();
						if (parameterType == typeof(AnimationEvent))
						{
							EditorGUILayout.PrefixLabel("Event Data");
						}
						else
						{
							EditorGUILayout.PrefixLabel("Parameters");
						}
						AnimationWindowEventInspector.DoEditRegularParameters(evt, parameterType);
					}
				}
				else
				{
					evt.functionName = EditorGUILayout.TextField(new GUIContent("Function"), evt.functionName, new GUILayoutOption[0]);
					AnimationWindowEventInspector.DoEditRegularParameters(evt, typeof(AnimationEvent));
				}
				if (GUI.changed)
				{
					if (awevt.clip != null)
					{
						Undo.RegisterCompleteObjectUndo(awevt.clip, "Animation Event Change");
						AnimationUtility.SetAnimationEvents(awevt.clip, array);
					}
					else if (awevt.clipInfo != null)
					{
						awevt.clipInfo.SetEvent(awevt.eventIndex, evt);
					}
				}
			}
		}

		public static void OnDisabledAnimationEvent()
		{
			AnimationEvent animationEvent = new AnimationEvent();
			using (new EditorGUI.DisabledScope(true))
			{
				animationEvent.functionName = EditorGUILayout.TextField(new GUIContent("Function"), animationEvent.functionName, new GUILayoutOption[0]);
				AnimationWindowEventInspector.DoEditRegularParameters(animationEvent, typeof(AnimationEvent));
			}
		}

		public static List<AnimationWindowEventMethod> CollectSupportedMethods(AnimationWindowEvent awevt)
		{
			List<AnimationWindowEventMethod> list = new List<AnimationWindowEventMethod>();
			List<AnimationWindowEventMethod> result;
			if (awevt.root == null)
			{
				result = list;
			}
			else
			{
				MonoBehaviour[] components = awevt.root.GetComponents<MonoBehaviour>();
				HashSet<string> hashSet = new HashSet<string>();
				MonoBehaviour[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					MonoBehaviour monoBehaviour = array[i];
					if (!(monoBehaviour == null))
					{
						Type type = monoBehaviour.GetType();
						while (type != typeof(MonoBehaviour) && type != null)
						{
							MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							for (int j = 0; j < methods.Length; j++)
							{
								MethodInfo methodInfo = methods[j];
								string name = methodInfo.Name;
								if (AnimationWindowEventInspector.IsSupportedMethodName(name))
								{
									ParameterInfo[] parameters = methodInfo.GetParameters();
									if (parameters.Length <= 1)
									{
										Type type2 = null;
										if (parameters.Length == 1)
										{
											type2 = parameters[0].ParameterType;
											if (type2 != typeof(string) && type2 != typeof(float) && type2 != typeof(int) && type2 != typeof(AnimationEvent) && type2 != typeof(UnityEngine.Object) && !type2.IsSubclassOf(typeof(UnityEngine.Object)) && !type2.IsEnum)
											{
												goto IL_1C7;
											}
										}
										AnimationWindowEventMethod item = default(AnimationWindowEventMethod);
										item.name = methodInfo.Name;
										item.parameterType = type2;
										int num = list.FindIndex((AnimationWindowEventMethod m) => m.name == name);
										if (num != -1)
										{
											if (list[num].parameterType != type2)
											{
												hashSet.Add(name);
											}
										}
										list.Add(item);
									}
								}
								IL_1C7:;
							}
							type = type.BaseType;
						}
					}
				}
				foreach (string current in hashSet)
				{
					for (int k = list.Count - 1; k >= 0; k--)
					{
						if (list[k].name.Equals(current))
						{
							list.RemoveAt(k);
						}
					}
				}
				result = list;
			}
			return result;
		}

		public static string FormatEvent(GameObject root, AnimationEvent evt)
		{
			string result;
			if (string.IsNullOrEmpty(evt.functionName))
			{
				result = "(No Function Selected)";
			}
			else if (!AnimationWindowEventInspector.IsSupportedMethodName(evt.functionName))
			{
				result = evt.functionName + " (Function Not Supported)";
			}
			else if (root == null)
			{
				result = evt.functionName + " (Function Not Supported)";
			}
			else
			{
				MonoBehaviour[] components = root.GetComponents<MonoBehaviour>();
				for (int i = 0; i < components.Length; i++)
				{
					MonoBehaviour monoBehaviour = components[i];
					if (!(monoBehaviour == null))
					{
						Type type = monoBehaviour.GetType();
						if (type != typeof(MonoBehaviour) && (type.BaseType == null || !(type.BaseType.Name == "GraphBehaviour")))
						{
							MethodInfo methodInfo = null;
							try
							{
								methodInfo = type.GetMethod(evt.functionName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							}
							catch (AmbiguousMatchException)
							{
							}
							if (methodInfo != null)
							{
								IEnumerable<Type> paramTypes = from p in methodInfo.GetParameters()
								select p.ParameterType;
								result = evt.functionName + AnimationWindowEventInspector.FormatEventArguments(paramTypes, evt);
								return result;
							}
						}
					}
				}
				result = evt.functionName + " (Function Not Supported)";
			}
			return result;
		}

		private static void DoEditRegularParameters(AnimationEvent evt, Type selectedParameter)
		{
			if (selectedParameter == typeof(AnimationEvent) || selectedParameter == typeof(float))
			{
				evt.floatParameter = EditorGUILayout.FloatField("Float", evt.floatParameter, new GUILayoutOption[0]);
			}
			if (selectedParameter.IsEnum)
			{
				evt.intParameter = AnimationWindowEventInspector.EnumPopup("Enum", selectedParameter, evt.intParameter);
			}
			else if (selectedParameter == typeof(AnimationEvent) || selectedParameter == typeof(int))
			{
				evt.intParameter = EditorGUILayout.IntField("Int", evt.intParameter, new GUILayoutOption[0]);
			}
			if (selectedParameter == typeof(AnimationEvent) || selectedParameter == typeof(string))
			{
				evt.stringParameter = EditorGUILayout.TextField("String", evt.stringParameter, new GUILayoutOption[0]);
			}
			if (selectedParameter == typeof(AnimationEvent) || selectedParameter.IsSubclassOf(typeof(UnityEngine.Object)) || selectedParameter == typeof(UnityEngine.Object))
			{
				Type type = typeof(UnityEngine.Object);
				if (selectedParameter != typeof(AnimationEvent))
				{
					type = selectedParameter;
				}
				bool allowSceneObjects = false;
				evt.objectReferenceParameter = EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(type.Name), evt.objectReferenceParameter, type, allowSceneObjects, new GUILayoutOption[0]);
			}
		}

		private static int EnumPopup(string label, Type enumType, int selected)
		{
			if (!enumType.IsEnum)
			{
				throw new Exception("parameter _enum must be of type System.Enum");
			}
			string[] names = Enum.GetNames(enumType);
			int num = Array.IndexOf<string>(names, Enum.GetName(enumType, selected));
			num = EditorGUILayout.Popup(label, num, names, EditorStyles.popup, new GUILayoutOption[0]);
			int result;
			if (num == -1)
			{
				result = selected;
			}
			else
			{
				Enum value = (Enum)Enum.Parse(enumType, names[num]);
				result = Convert.ToInt32(value);
			}
			return result;
		}

		private static bool IsSupportedMethodName(string name)
		{
			return !(name == "Main") && !(name == "Start") && !(name == "Awake") && !(name == "Update");
		}

		private static string FormatEventArguments(IEnumerable<Type> paramTypes, AnimationEvent evt)
		{
			string result;
			if (!paramTypes.Any<Type>())
			{
				result = " ( )";
			}
			else if (paramTypes.Count<Type>() > 1)
			{
				result = " (Function Not Supported)";
			}
			else
			{
				Type type = paramTypes.First<Type>();
				if (type == typeof(string))
				{
					result = " ( \"" + evt.stringParameter + "\" )";
				}
				else if (type == typeof(float))
				{
					result = " ( " + evt.floatParameter + " )";
				}
				else if (type == typeof(int))
				{
					result = " ( " + evt.intParameter + " )";
				}
				else if (type.IsEnum)
				{
					result = string.Concat(new string[]
					{
						" ( ",
						type.Name,
						".",
						Enum.GetName(type, evt.intParameter),
						" )"
					});
				}
				else if (type == typeof(AnimationEvent))
				{
					result = string.Concat(new object[]
					{
						" ( ",
						evt.floatParameter,
						" / ",
						evt.intParameter,
						" / \"",
						evt.stringParameter,
						"\" / ",
						(!(evt.objectReferenceParameter == null)) ? evt.objectReferenceParameter.name : "null",
						" )"
					});
				}
				else if (type.IsSubclassOf(typeof(UnityEngine.Object)) || type == typeof(UnityEngine.Object))
				{
					result = " ( " + ((!(evt.objectReferenceParameter == null)) ? evt.objectReferenceParameter.name : "null") + " )";
				}
				else
				{
					result = " (Function Not Supported)";
				}
			}
			return result;
		}
	}
}
