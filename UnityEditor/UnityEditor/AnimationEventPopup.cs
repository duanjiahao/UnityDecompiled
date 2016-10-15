using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Edit Animation Event", useTypeNameAsIconName = false)]
	internal class AnimationEventPopup : EditorWindow
	{
		private const string kAmbiguousPostFix = " (Function Is Ambiguous)";

		private const string kNotSupportedPostFix = " (Function Not Supported)";

		private const string kNoneSelected = "(No Function Selected)";

		private GameObject m_Root;

		private AnimationClip m_Clip;

		private int m_EventIndex;

		private AnimationClipInfoProperties m_ClipInfo;

		private EditorWindow m_Owner;

		public AnimationClipInfoProperties clipInfo
		{
			get
			{
				return this.m_ClipInfo;
			}
			set
			{
				this.m_ClipInfo = value;
			}
		}

		private int eventIndex
		{
			get
			{
				return this.m_EventIndex;
			}
			set
			{
				this.m_EventIndex = value;
			}
		}

		private void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
		}

		internal static void InitWindow(AnimationEventPopup popup)
		{
			popup.minSize = new Vector2(400f, 140f);
			popup.maxSize = new Vector2(400f, 140f);
			popup.titleContent = EditorGUIUtility.TextContent("Edit Animation Event");
		}

		internal static void Edit(GameObject root, AnimationClip clip, int index, EditorWindow owner)
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
			AnimationEventPopup animationEventPopup = (array.Length <= 0) ? null : ((AnimationEventPopup)array[0]);
			if (animationEventPopup == null)
			{
				animationEventPopup = EditorWindow.GetWindow<AnimationEventPopup>(true);
				AnimationEventPopup.InitWindow(animationEventPopup);
			}
			animationEventPopup.m_Root = root;
			animationEventPopup.m_Clip = clip;
			animationEventPopup.eventIndex = index;
			animationEventPopup.m_Owner = owner;
			animationEventPopup.Repaint();
		}

		internal static void Edit(AnimationClipInfoProperties clipInfo, int index)
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
			AnimationEventPopup animationEventPopup = (array.Length <= 0) ? null : ((AnimationEventPopup)array[0]);
			if (animationEventPopup == null)
			{
				animationEventPopup = EditorWindow.GetWindow<AnimationEventPopup>(true);
				AnimationEventPopup.InitWindow(animationEventPopup);
			}
			animationEventPopup.m_Root = null;
			animationEventPopup.m_Clip = null;
			animationEventPopup.m_ClipInfo = clipInfo;
			animationEventPopup.eventIndex = index;
			animationEventPopup.Repaint();
		}

		internal static void UpdateSelection(GameObject root, AnimationClip clip, int index, EditorWindow owner)
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
			AnimationEventPopup animationEventPopup = (array.Length <= 0) ? null : ((AnimationEventPopup)array[0]);
			if (animationEventPopup == null)
			{
				return;
			}
			animationEventPopup.m_Root = root;
			animationEventPopup.m_Clip = clip;
			animationEventPopup.eventIndex = index;
			animationEventPopup.m_Owner = owner;
			animationEventPopup.Repaint();
		}

		internal static int Create(GameObject root, AnimationClip clip, float time, EditorWindow owner)
		{
			AnimationEvent animationEvent = new AnimationEvent();
			animationEvent.time = time;
			AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents(clip);
			int num = AnimationEventPopup.InsertAnimationEvent(ref animationEvents, clip, animationEvent);
			AnimationEventPopup window = EditorWindow.GetWindow<AnimationEventPopup>(true);
			AnimationEventPopup.InitWindow(window);
			window.m_Root = root;
			window.m_Clip = clip;
			window.eventIndex = num;
			window.m_Owner = owner;
			return num;
		}

		internal static void ClosePopup()
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
			AnimationEventPopup animationEventPopup = (array.Length <= 0) ? null : ((AnimationEventPopup)array[0]);
			if (animationEventPopup != null)
			{
				animationEventPopup.Close();
			}
		}

		public static string FormatEvent(GameObject root, AnimationEvent evt)
		{
			if (string.IsNullOrEmpty(evt.functionName))
			{
				return "(No Function Selected)";
			}
			if (!AnimationEventPopup.IsSupportedMethodName(evt.functionName))
			{
				return evt.functionName + " (Function Not Supported)";
			}
			MonoBehaviour[] components = root.GetComponents<MonoBehaviour>();
			for (int i = 0; i < components.Length; i++)
			{
				MonoBehaviour monoBehaviour = components[i];
				if (!(monoBehaviour == null))
				{
					Type type = monoBehaviour.GetType();
					if (type != typeof(MonoBehaviour) && (type.BaseType == null || !(type.BaseType.Name == "GraphBehaviour")))
					{
						MethodInfo method = type.GetMethod(evt.functionName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						if (method != null)
						{
							IEnumerable<Type> paramTypes = from p in method.GetParameters()
							select p.ParameterType;
							return evt.functionName + AnimationEventPopup.FormatEventArguments(paramTypes, evt);
						}
					}
				}
			}
			return evt.functionName + " (Function Not Supported)";
		}

		private static bool IsSupportedMethodName(string name)
		{
			return !(name == "Main") && !(name == "Start") && !(name == "Awake") && !(name == "Update");
		}

		private static string FormatEventArguments(IEnumerable<Type> paramTypes, AnimationEvent evt)
		{
			if (!paramTypes.Any<Type>())
			{
				return " ( )";
			}
			if (paramTypes.Count<Type>() > 1)
			{
				return " (Function Not Supported)";
			}
			Type type = paramTypes.First<Type>();
			if (type == typeof(string))
			{
				return " ( \"" + evt.stringParameter + "\" )";
			}
			if (type == typeof(float))
			{
				return " ( " + evt.floatParameter + " )";
			}
			if (type == typeof(int))
			{
				return " ( " + evt.intParameter + " )";
			}
			if (type == typeof(int))
			{
				return " ( " + evt.intParameter + " )";
			}
			if (type.IsEnum)
			{
				return string.Concat(new string[]
				{
					" ( ",
					type.Name,
					".",
					Enum.GetName(type, evt.intParameter),
					" )"
				});
			}
			if (type == typeof(AnimationEvent))
			{
				return string.Concat(new object[]
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
			if (type.IsSubclassOf(typeof(UnityEngine.Object)) || type == typeof(UnityEngine.Object))
			{
				return " ( " + ((!(evt.objectReferenceParameter == null)) ? evt.objectReferenceParameter.name : "null") + " )";
			}
			return " (Function Not Supported)";
		}

		private static void CollectSupportedMethods(GameObject root, out List<string> supportedMethods, out List<Type> supportedMethodsParameter)
		{
			supportedMethods = new List<string>();
			supportedMethodsParameter = new List<Type>();
			MonoBehaviour[] components = root.GetComponents<MonoBehaviour>();
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
							if (AnimationEventPopup.IsSupportedMethodName(name))
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
											goto IL_16C;
										}
									}
									if (supportedMethods.Contains(name))
									{
										int index = supportedMethods.IndexOf(name);
										if (supportedMethodsParameter[index] != type2)
										{
											hashSet.Add(name);
										}
									}
									supportedMethods.Add(name);
									supportedMethodsParameter.Add(type2);
								}
							}
							IL_16C:;
						}
						type = type.BaseType;
					}
				}
			}
			foreach (string current in hashSet)
			{
				for (int k = 0; k < supportedMethods.Count; k++)
				{
					if (supportedMethods[k].Equals(current))
					{
						supportedMethods.RemoveAt(k);
						supportedMethodsParameter.RemoveAt(k);
						k--;
					}
				}
			}
		}

		internal static int InsertAnimationEvent(ref AnimationEvent[] events, AnimationClip clip, AnimationEvent evt)
		{
			Undo.RegisterCompleteObjectUndo(clip, "Add Event");
			int num = events.Length;
			for (int i = 0; i < events.Length; i++)
			{
				if (events[i].time > evt.time)
				{
					num = i;
					break;
				}
			}
			ArrayUtility.Insert<AnimationEvent>(ref events, num, evt);
			AnimationUtility.SetAnimationEvents(clip, events);
			events = AnimationUtility.GetAnimationEvents(clip);
			if (events[num].time != evt.time || events[num].functionName != events[num].functionName)
			{
				Debug.LogError("Failed insertion");
			}
			return num;
		}

		public void OnGUI()
		{
			AnimationEvent[] array = null;
			if (this.m_Clip != null)
			{
				array = AnimationUtility.GetAnimationEvents(this.m_Clip);
			}
			else if (this.m_ClipInfo != null)
			{
				array = this.m_ClipInfo.GetEvents();
			}
			if (array == null || this.eventIndex < 0 || this.eventIndex >= array.Length)
			{
				return;
			}
			GUI.changed = false;
			AnimationEvent animationEvent = array[this.eventIndex];
			if (this.m_Root)
			{
				List<string> list;
				List<Type> list2;
				AnimationEventPopup.CollectSupportedMethods(this.m_Root, out list, out list2);
				List<string> list3 = new List<string>(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					string str = " ( )";
					if (list2[i] != null)
					{
						if (list2[i] == typeof(float))
						{
							str = " ( float )";
						}
						else if (list2[i] == typeof(int))
						{
							str = " ( int )";
						}
						else
						{
							str = string.Format(" ( {0} )", list2[i].Name);
						}
					}
					list3.Add(list[i] + str);
				}
				int count = list.Count;
				int num = list.IndexOf(animationEvent.functionName);
				if (num == -1)
				{
					num = list.Count;
					list.Add(animationEvent.functionName);
					if (string.IsNullOrEmpty(animationEvent.functionName))
					{
						list3.Add("(No Function Selected)");
					}
					else
					{
						list3.Add(animationEvent.functionName + " (Function Not Supported)");
					}
					list2.Add(null);
				}
				EditorGUIUtility.labelWidth = 130f;
				int num2 = num;
				num = EditorGUILayout.Popup("Function: ", num, list3.ToArray(), new GUILayoutOption[0]);
				if (num2 != num && num != -1 && num != count)
				{
					animationEvent.functionName = list[num];
					animationEvent.stringParameter = string.Empty;
				}
				Type type = list2[num];
				if (type != null)
				{
					EditorGUILayout.Space();
					if (type == typeof(AnimationEvent))
					{
						EditorGUILayout.PrefixLabel("Event Data");
					}
					else
					{
						EditorGUILayout.PrefixLabel("Parameters");
					}
					AnimationEventPopup.DoEditRegularParameters(animationEvent, type);
				}
			}
			else
			{
				animationEvent.functionName = EditorGUILayout.TextField(new GUIContent("Function"), animationEvent.functionName, new GUILayoutOption[0]);
				AnimationEventPopup.DoEditRegularParameters(animationEvent, typeof(AnimationEvent));
			}
			if (GUI.changed)
			{
				if (this.m_Clip != null)
				{
					Undo.RegisterCompleteObjectUndo(this.m_Clip, "Animation Event Change");
					AnimationUtility.SetAnimationEvents(this.m_Clip, array);
				}
				else if (this.m_ClipInfo != null)
				{
					this.m_ClipInfo.SetEvent(this.m_EventIndex, animationEvent);
				}
			}
		}

		private static bool EscapePressed()
		{
			return Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
		}

		private static bool EnterPressed()
		{
			return Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;
		}

		private static void DoEditRegularParameters(AnimationEvent evt, Type selectedParameter)
		{
			if (selectedParameter == typeof(AnimationEvent) || selectedParameter == typeof(float))
			{
				evt.floatParameter = EditorGUILayout.FloatField("Float", evt.floatParameter, new GUILayoutOption[0]);
			}
			if (selectedParameter.IsEnum)
			{
				evt.intParameter = AnimationEventPopup.EnumPopup("Enum", selectedParameter, evt.intParameter);
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

		public static int EnumPopup(string label, Type enumType, int selected)
		{
			if (!enumType.IsEnum)
			{
				throw new Exception("parameter _enum must be of type System.Enum");
			}
			string[] names = Enum.GetNames(enumType);
			int num = Array.IndexOf<string>(names, Enum.GetName(enumType, selected));
			num = EditorGUILayout.Popup(label, num, names, EditorStyles.popup, new GUILayoutOption[0]);
			if (num == -1)
			{
				return selected;
			}
			Enum value = (Enum)Enum.Parse(enumType, names[num]);
			return Convert.ToInt32(value);
		}

		private void OnDestroy()
		{
			if (this.m_Owner)
			{
				this.m_Owner.Focus();
			}
		}
	}
}
