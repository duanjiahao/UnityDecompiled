using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEditor.Events
{
	[CustomPreview(typeof(GameObject))]
	internal class InterceptedEventsPreview : ObjectPreview
	{
		protected class ComponentInterceptedEvents
		{
			public GUIContent componentName;

			public int[] interceptedEvents;
		}

		private class Styles
		{
			public GUIStyle labelStyle = new GUIStyle(EditorStyles.label);

			public GUIStyle componentName = new GUIStyle(EditorStyles.boldLabel);

			public Styles()
			{
				Color textColor = new Color(0.7f, 0.7f, 0.7f);
				this.labelStyle.padding.right += 20;
				this.labelStyle.normal.textColor = textColor;
				this.labelStyle.active.textColor = textColor;
				this.labelStyle.focused.textColor = textColor;
				this.labelStyle.hover.textColor = textColor;
				this.labelStyle.onNormal.textColor = textColor;
				this.labelStyle.onActive.textColor = textColor;
				this.labelStyle.onFocused.textColor = textColor;
				this.labelStyle.onHover.textColor = textColor;
				this.componentName.normal.textColor = textColor;
				this.componentName.active.textColor = textColor;
				this.componentName.focused.textColor = textColor;
				this.componentName.hover.textColor = textColor;
				this.componentName.onNormal.textColor = textColor;
				this.componentName.onActive.textColor = textColor;
				this.componentName.onFocused.textColor = textColor;
				this.componentName.onHover.textColor = textColor;
			}
		}

		private Dictionary<GameObject, List<InterceptedEventsPreview.ComponentInterceptedEvents>> m_TargetEvents;

		private bool m_InterceptsAnyEvent = false;

		private GUIContent m_Title;

		private InterceptedEventsPreview.Styles m_Styles = new InterceptedEventsPreview.Styles();

		private static List<Type> s_EventSystemInterfaces = null;

		private static List<GUIContent> s_PossibleEvents = null;

		private static Dictionary<Type, List<int>> s_InterfaceEventSystemEvents = null;

		private static readonly Dictionary<Type, InterceptedEventsPreview.ComponentInterceptedEvents> s_ComponentEvents2 = new Dictionary<Type, InterceptedEventsPreview.ComponentInterceptedEvents>();

		public override void Initialize(UnityEngine.Object[] targets)
		{
			base.Initialize(targets);
			this.m_TargetEvents = new Dictionary<GameObject, List<InterceptedEventsPreview.ComponentInterceptedEvents>>(targets.Count<UnityEngine.Object>());
			this.m_InterceptsAnyEvent = false;
			for (int i = 0; i < targets.Length; i++)
			{
				GameObject gameObject = targets[i] as GameObject;
				List<InterceptedEventsPreview.ComponentInterceptedEvents> eventsInfo = InterceptedEventsPreview.GetEventsInfo(gameObject);
				this.m_TargetEvents.Add(gameObject, eventsInfo);
				if (eventsInfo.Any<InterceptedEventsPreview.ComponentInterceptedEvents>())
				{
					this.m_InterceptsAnyEvent = true;
				}
			}
		}

		public override GUIContent GetPreviewTitle()
		{
			if (this.m_Title == null)
			{
				this.m_Title = new GUIContent("Intercepted Events");
			}
			return this.m_Title;
		}

		public override bool HasPreviewGUI()
		{
			return this.m_TargetEvents != null && this.m_InterceptsAnyEvent;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (this.m_Styles == null)
				{
					this.m_Styles = new InterceptedEventsPreview.Styles();
				}
				Vector2 zero = Vector2.zero;
				int num = 0;
				List<InterceptedEventsPreview.ComponentInterceptedEvents> list = this.m_TargetEvents[this.target as GameObject];
				foreach (InterceptedEventsPreview.ComponentInterceptedEvents current in list)
				{
					int[] interceptedEvents = current.interceptedEvents;
					for (int i = 0; i < interceptedEvents.Length; i++)
					{
						int index = interceptedEvents[i];
						GUIContent content = InterceptedEventsPreview.s_PossibleEvents[index];
						num++;
						Vector2 vector = this.m_Styles.labelStyle.CalcSize(content);
						if (zero.x < vector.x)
						{
							zero.x = vector.x;
						}
						if (zero.y < vector.y)
						{
							zero.y = vector.y;
						}
					}
				}
				RectOffset rectOffset = new RectOffset(-5, -5, -5, -5);
				r = rectOffset.Add(r);
				int num2 = Mathf.Max(Mathf.FloorToInt(r.width / zero.x), 1);
				int num3 = Mathf.Max(num / num2, 1) + list.Count;
				float num4 = r.x + Mathf.Max(0f, (r.width - zero.x * (float)num2) / 2f);
				float y = r.y + Mathf.Max(0f, (r.height - zero.y * (float)num3) / 2f);
				Rect position = new Rect(num4, y, zero.x, zero.y);
				int num5 = 0;
				foreach (InterceptedEventsPreview.ComponentInterceptedEvents current2 in list)
				{
					GUI.Label(position, current2.componentName, this.m_Styles.componentName);
					position.y += position.height;
					position.x = num4;
					int[] interceptedEvents2 = current2.interceptedEvents;
					for (int j = 0; j < interceptedEvents2.Length; j++)
					{
						int index2 = interceptedEvents2[j];
						GUIContent content2 = InterceptedEventsPreview.s_PossibleEvents[index2];
						GUI.Label(position, content2, this.m_Styles.labelStyle);
						if (num5 < num2 - 1)
						{
							position.x += position.width;
						}
						else
						{
							position.y += position.height;
							position.x = num4;
						}
						num5 = (num5 + 1) % num2;
					}
					if (position.x != num4)
					{
						position.y += position.height;
						position.x = num4;
					}
				}
			}
		}

		protected static List<InterceptedEventsPreview.ComponentInterceptedEvents> GetEventsInfo(GameObject gameObject)
		{
			InterceptedEventsPreview.InitializeEvetnsInterfaceCacheIfNeeded();
			List<InterceptedEventsPreview.ComponentInterceptedEvents> list = new List<InterceptedEventsPreview.ComponentInterceptedEvents>();
			MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
			int i = 0;
			int num = components.Length;
			while (i < num)
			{
				InterceptedEventsPreview.ComponentInterceptedEvents componentInterceptedEvents = null;
				MonoBehaviour monoBehaviour = components[i];
				if (!(monoBehaviour == null))
				{
					Type type = monoBehaviour.GetType();
					if (!InterceptedEventsPreview.s_ComponentEvents2.ContainsKey(type))
					{
						List<int> list2 = null;
						if (typeof(IEventSystemHandler).IsAssignableFrom(type))
						{
							for (int j = 0; j < InterceptedEventsPreview.s_EventSystemInterfaces.Count; j++)
							{
								Type type2 = InterceptedEventsPreview.s_EventSystemInterfaces[j];
								if (type2.IsAssignableFrom(type))
								{
									if (list2 == null)
									{
										list2 = new List<int>();
									}
									list2.AddRange(InterceptedEventsPreview.s_InterfaceEventSystemEvents[type2]);
								}
							}
						}
						if (list2 != null)
						{
							componentInterceptedEvents = new InterceptedEventsPreview.ComponentInterceptedEvents();
							componentInterceptedEvents.componentName = new GUIContent(type.Name);
							componentInterceptedEvents.interceptedEvents = (from index in list2
							orderby InterceptedEventsPreview.s_PossibleEvents[index].text
							select index).ToArray<int>();
						}
						InterceptedEventsPreview.s_ComponentEvents2.Add(type, componentInterceptedEvents);
					}
					else
					{
						componentInterceptedEvents = InterceptedEventsPreview.s_ComponentEvents2[type];
					}
					if (componentInterceptedEvents != null)
					{
						list.Add(componentInterceptedEvents);
					}
				}
				i++;
			}
			return list;
		}

		private static void InitializeEvetnsInterfaceCacheIfNeeded()
		{
			if (InterceptedEventsPreview.s_EventSystemInterfaces == null)
			{
				InterceptedEventsPreview.s_EventSystemInterfaces = new List<Type>();
				InterceptedEventsPreview.s_PossibleEvents = new List<GUIContent>();
				InterceptedEventsPreview.s_InterfaceEventSystemEvents = new Dictionary<Type, List<int>>();
				foreach (Type current in InterceptedEventsPreview.GetAccessibleTypesInLoadedAssemblies())
				{
					if (current.IsInterface)
					{
						if (typeof(IEventSystemHandler).IsAssignableFrom(current))
						{
							InterceptedEventsPreview.s_EventSystemInterfaces.Add(current);
							List<int> list = new List<int>();
							MethodInfo[] methods = current.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							for (int i = 0; i < methods.Length; i++)
							{
								MethodInfo methodInfo = methods[i];
								list.Add(InterceptedEventsPreview.s_PossibleEvents.Count);
								InterceptedEventsPreview.s_PossibleEvents.Add(new GUIContent(methodInfo.Name));
							}
							InterceptedEventsPreview.s_InterfaceEventSystemEvents.Add(current, list);
						}
					}
				}
			}
		}

		[DebuggerHidden]
		private static IEnumerable<Type> GetAccessibleTypesInLoadedAssemblies()
		{
			InterceptedEventsPreview.<GetAccessibleTypesInLoadedAssemblies>c__Iterator0 <GetAccessibleTypesInLoadedAssemblies>c__Iterator = new InterceptedEventsPreview.<GetAccessibleTypesInLoadedAssemblies>c__Iterator0();
			InterceptedEventsPreview.<GetAccessibleTypesInLoadedAssemblies>c__Iterator0 expr_07 = <GetAccessibleTypesInLoadedAssemblies>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
