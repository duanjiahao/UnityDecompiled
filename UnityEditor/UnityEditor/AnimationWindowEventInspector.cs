using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AnimationWindowEvent))]
	internal class AnimationWindowEventInspector : Editor
	{
		private struct AnimationWindowEventData
		{
			public GameObject root;

			public AnimationClip clip;

			public AnimationClipInfoProperties clipInfo;

			public AnimationEvent[] events;

			public AnimationEvent[] selectedEvents;
		}

		private const string kNotSupportedPostFix = " (Function Not Supported)";

		private const string kNoneSelected = "(No Function Selected)";

		public override void OnInspectorGUI()
		{
			AnimationWindowEvent[] awEvents = (from o in base.targets
			select o as AnimationWindowEvent).ToArray<AnimationWindowEvent>();
			AnimationWindowEventInspector.OnEditAnimationEvents(awEvents);
		}

		protected override void OnHeaderGUI()
		{
			string header = (base.targets.Length != 1) ? (base.targets.Length + " Animation Events") : "Animation Event";
			Editor.DrawHeaderGUI(this, header);
		}

		public static void OnEditAnimationEvent(AnimationWindowEvent awe)
		{
			AnimationWindowEventInspector.OnEditAnimationEvents(new AnimationWindowEvent[]
			{
				awe
			});
		}

		public static void OnEditAnimationEvents(AnimationWindowEvent[] awEvents)
		{
			AnimationWindowEventInspector.AnimationWindowEventData data = AnimationWindowEventInspector.GetData(awEvents);
			if (data.events != null && data.selectedEvents != null && data.selectedEvents.Length != 0)
			{
				AnimationEvent firstEvent = data.selectedEvents[0];
				bool flag = Array.TrueForAll<AnimationEvent>(data.selectedEvents, (AnimationEvent evt) => evt.functionName == firstEvent.functionName);
				GUI.changed = false;
				if (data.root != null)
				{
					List<AnimationWindowEventMethod> list = AnimationWindowEventInspector.CollectSupportedMethods(data.root);
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
					int num = list.FindIndex((AnimationWindowEventMethod method) => method.name == firstEvent.functionName);
					if (num == -1)
					{
						num = list.Count;
						list.Add(new AnimationWindowEventMethod
						{
							name = firstEvent.functionName,
							parameterType = null
						});
						if (string.IsNullOrEmpty(firstEvent.functionName))
						{
							list2.Add("(No Function Selected)");
						}
						else
						{
							list2.Add(firstEvent.functionName + " (Function Not Supported)");
						}
					}
					EditorGUIUtility.labelWidth = 130f;
					EditorGUI.showMixedValue = !flag;
					int num2 = (!flag) ? -1 : num;
					num = EditorGUILayout.Popup("Function: ", num, list2.ToArray(), new GUILayoutOption[0]);
					if (num2 != num && num != -1 && num != count)
					{
						AnimationEvent[] selectedEvents = data.selectedEvents;
						for (int j = 0; j < selectedEvents.Length; j++)
						{
							AnimationEvent animationEvent = selectedEvents[j];
							animationEvent.functionName = list[num].name;
							animationEvent.stringParameter = string.Empty;
						}
					}
					EditorGUI.showMixedValue = false;
					Type parameterType = list[num].parameterType;
					if (flag && parameterType != null)
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
						AnimationWindowEventInspector.DoEditRegularParameters(data.selectedEvents, parameterType);
					}
				}
				else
				{
					EditorGUI.showMixedValue = !flag;
					string text = (!flag) ? "" : firstEvent.functionName;
					string text2 = EditorGUILayout.TextField(new GUIContent("Function"), text, new GUILayoutOption[0]);
					if (text2 != text)
					{
						AnimationEvent[] selectedEvents2 = data.selectedEvents;
						for (int k = 0; k < selectedEvents2.Length; k++)
						{
							AnimationEvent animationEvent2 = selectedEvents2[k];
							animationEvent2.functionName = text2;
							animationEvent2.stringParameter = string.Empty;
						}
					}
					EditorGUI.showMixedValue = false;
					if (flag)
					{
						AnimationWindowEventInspector.DoEditRegularParameters(data.selectedEvents, typeof(AnimationEvent));
					}
					else
					{
						using (new EditorGUI.DisabledScope(true))
						{
							AnimationEvent animationEvent3 = new AnimationEvent();
							AnimationWindowEventInspector.DoEditRegularParameters(new AnimationEvent[]
							{
								animationEvent3
							}, typeof(AnimationEvent));
						}
					}
				}
				if (GUI.changed)
				{
					AnimationWindowEventInspector.SetData(awEvents, data);
				}
			}
		}

		public static void OnDisabledAnimationEvent()
		{
			AnimationEvent animationEvent = new AnimationEvent();
			using (new EditorGUI.DisabledScope(true))
			{
				animationEvent.functionName = EditorGUILayout.TextField(new GUIContent("Function"), animationEvent.functionName, new GUILayoutOption[0]);
				AnimationWindowEventInspector.DoEditRegularParameters(new AnimationEvent[]
				{
					animationEvent
				}, typeof(AnimationEvent));
			}
		}

		public static List<AnimationWindowEventMethod> CollectSupportedMethods(GameObject gameObject)
		{
			List<AnimationWindowEventMethod> list = new List<AnimationWindowEventMethod>();
			List<AnimationWindowEventMethod> result;
			if (gameObject == null)
			{
				result = list;
			}
			else
			{
				MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
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
												goto IL_1BD;
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
								IL_1BD:;
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

		private static void DoEditRegularParameters(AnimationEvent[] events, Type selectedParameter)
		{
			AnimationEvent firstEvent = events[0];
			if (selectedParameter == typeof(AnimationEvent) || selectedParameter == typeof(float))
			{
				bool flag = Array.TrueForAll<AnimationEvent>(events, (AnimationEvent evt) => evt.floatParameter == firstEvent.floatParameter);
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = !flag;
				float floatParameter = EditorGUILayout.FloatField("Float", firstEvent.floatParameter, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					for (int i = 0; i < events.Length; i++)
					{
						AnimationEvent animationEvent = events[i];
						animationEvent.floatParameter = floatParameter;
					}
				}
			}
			if (selectedParameter == typeof(AnimationEvent) || selectedParameter == typeof(int) || selectedParameter.IsEnum)
			{
				bool flag2 = Array.TrueForAll<AnimationEvent>(events, (AnimationEvent evt) => evt.intParameter == firstEvent.intParameter);
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = !flag2;
				int intParameter;
				if (selectedParameter.IsEnum)
				{
					intParameter = AnimationWindowEventInspector.EnumPopup("Enum", selectedParameter, firstEvent.intParameter);
				}
				else
				{
					intParameter = EditorGUILayout.IntField("Int", firstEvent.intParameter, new GUILayoutOption[0]);
				}
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					for (int j = 0; j < events.Length; j++)
					{
						AnimationEvent animationEvent2 = events[j];
						animationEvent2.intParameter = intParameter;
					}
				}
			}
			if (selectedParameter == typeof(AnimationEvent) || selectedParameter == typeof(string))
			{
				bool flag3 = Array.TrueForAll<AnimationEvent>(events, (AnimationEvent evt) => evt.stringParameter == firstEvent.stringParameter);
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = !flag3;
				string stringParameter = EditorGUILayout.TextField("String", firstEvent.stringParameter, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					for (int k = 0; k < events.Length; k++)
					{
						AnimationEvent animationEvent3 = events[k];
						animationEvent3.stringParameter = stringParameter;
					}
				}
			}
			if (selectedParameter == typeof(AnimationEvent) || selectedParameter.IsSubclassOf(typeof(UnityEngine.Object)) || selectedParameter == typeof(UnityEngine.Object))
			{
				bool flag4 = Array.TrueForAll<AnimationEvent>(events, (AnimationEvent evt) => evt.objectReferenceParameter == firstEvent.objectReferenceParameter);
				EditorGUI.BeginChangeCheck();
				Type type = typeof(UnityEngine.Object);
				if (selectedParameter != typeof(AnimationEvent))
				{
					type = selectedParameter;
				}
				EditorGUI.showMixedValue = !flag4;
				bool allowSceneObjects = false;
				UnityEngine.Object objectReferenceParameter = EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(type.Name), firstEvent.objectReferenceParameter, type, allowSceneObjects, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					for (int l = 0; l < events.Length; l++)
					{
						AnimationEvent animationEvent4 = events[l];
						animationEvent4.objectReferenceParameter = objectReferenceParameter;
					}
				}
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

		private static AnimationWindowEventInspector.AnimationWindowEventData GetData(AnimationWindowEvent[] awEvents)
		{
			AnimationWindowEventInspector.AnimationWindowEventData animationWindowEventData = default(AnimationWindowEventInspector.AnimationWindowEventData);
			AnimationWindowEventInspector.AnimationWindowEventData result;
			if (awEvents.Length == 0)
			{
				result = animationWindowEventData;
			}
			else
			{
				AnimationWindowEvent animationWindowEvent = awEvents[0];
				animationWindowEventData.root = animationWindowEvent.root;
				animationWindowEventData.clip = animationWindowEvent.clip;
				animationWindowEventData.clipInfo = animationWindowEvent.clipInfo;
				if (animationWindowEventData.clip != null)
				{
					animationWindowEventData.events = AnimationUtility.GetAnimationEvents(animationWindowEventData.clip);
				}
				else if (animationWindowEventData.clipInfo != null)
				{
					animationWindowEventData.events = animationWindowEventData.clipInfo.GetEvents();
				}
				if (animationWindowEventData.events != null)
				{
					List<AnimationEvent> list = new List<AnimationEvent>();
					for (int i = 0; i < awEvents.Length; i++)
					{
						AnimationWindowEvent animationWindowEvent2 = awEvents[i];
						if (animationWindowEvent2.eventIndex >= 0 && animationWindowEvent2.eventIndex < animationWindowEventData.events.Length)
						{
							list.Add(animationWindowEventData.events[animationWindowEvent2.eventIndex]);
						}
					}
					animationWindowEventData.selectedEvents = list.ToArray();
				}
				result = animationWindowEventData;
			}
			return result;
		}

		private static void SetData(AnimationWindowEvent[] awEvents, AnimationWindowEventInspector.AnimationWindowEventData data)
		{
			if (data.events != null)
			{
				if (data.clip != null)
				{
					Undo.RegisterCompleteObjectUndo(data.clip, "Animation Event Change");
					AnimationUtility.SetAnimationEvents(data.clip, data.events);
				}
				else if (data.clipInfo != null)
				{
					for (int i = 0; i < awEvents.Length; i++)
					{
						AnimationWindowEvent animationWindowEvent = awEvents[i];
						if (animationWindowEvent.eventIndex >= 0 && animationWindowEvent.eventIndex < data.events.Length)
						{
							data.clipInfo.SetEvent(animationWindowEvent.eventIndex, data.events[animationWindowEvent.eventIndex]);
						}
					}
				}
			}
		}

		[MenuItem("CONTEXT/AnimationWindowEvent/Reset")]
		private static void ResetValues(MenuCommand command)
		{
			AnimationWindowEvent animationWindowEvent = command.context as AnimationWindowEvent;
			AnimationWindowEvent[] awEvents = new AnimationWindowEvent[]
			{
				animationWindowEvent
			};
			AnimationWindowEventInspector.AnimationWindowEventData data = AnimationWindowEventInspector.GetData(awEvents);
			if (data.events != null && data.selectedEvents != null && data.selectedEvents.Length != 0)
			{
				AnimationEvent[] selectedEvents = data.selectedEvents;
				for (int i = 0; i < selectedEvents.Length; i++)
				{
					AnimationEvent animationEvent = selectedEvents[i];
					animationEvent.functionName = "";
					animationEvent.stringParameter = string.Empty;
					animationEvent.floatParameter = 0f;
					animationEvent.intParameter = 0;
					animationEvent.objectReferenceParameter = null;
				}
				AnimationWindowEventInspector.SetData(awEvents, data);
			}
		}
	}
}
