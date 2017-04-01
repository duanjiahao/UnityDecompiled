using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	internal class HostView : GUIView
	{
		internal static Color kViewColor = new Color(0.76f, 0.76f, 0.76f, 1f);

		internal static PrefColor kPlayModeDarken = new PrefColor("Playmode tint", 0.8f, 0.8f, 0.8f, 1f);

		internal GUIStyle background;

		[SerializeField]
		protected EditorWindow m_ActualView;

		[NonSerialized]
		private Rect m_BackgroundClearRect = new Rect(0f, 0f, 0f, 0f);

		[NonSerialized]
		protected readonly RectOffset m_BorderSize = new RectOffset();

		internal EditorWindow actualView
		{
			get
			{
				return this.m_ActualView;
			}
			set
			{
				if (!(this.m_ActualView == value))
				{
					this.DeregisterSelectedPane(true);
					this.m_ActualView = value;
					this.RegisterSelectedPane();
				}
			}
		}

		internal RectOffset borderSize
		{
			get
			{
				return this.GetBorderSize();
			}
		}

		protected virtual void OnViewChange(EditorWindow view)
		{
		}

		protected override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			this.SetActualViewPosition(newPos);
		}

		protected virtual void SetActualViewPosition(Rect newPos)
		{
			if (this.m_ActualView != null)
			{
				this.m_ActualView.m_Pos = newPos;
				this.m_ActualView.OnResized();
			}
		}

		public void OnEnable()
		{
			this.background = null;
			this.RegisterSelectedPane();
		}

		public void OnDisable()
		{
			this.DeregisterSelectedPane(false);
		}

		private void OnGUI()
		{
			EditorGUIUtility.ResetGUIState();
			base.DoWindowDecorationStart();
			if (this.background == null)
			{
				this.background = "hostview";
				this.background.padding.top = 0;
			}
			GUILayout.BeginVertical(this.background, new GUILayoutOption[0]);
			if (this.actualView)
			{
				this.actualView.m_Pos = base.screenPosition;
			}
			this.Invoke("OnGUI");
			EditorGUIUtility.ResetGUIState();
			if (this.m_ActualView != null && this.m_ActualView.m_FadeoutTime != 0f && Event.current.type == EventType.Repaint)
			{
				this.m_ActualView.DrawNotification();
			}
			GUILayout.EndVertical();
			base.DoWindowDecorationEnd();
			EditorGUI.ShowRepaints();
		}

		protected override bool OnFocus()
		{
			this.Invoke("OnFocus");
			bool result;
			if (this == null)
			{
				result = false;
			}
			else
			{
				base.Repaint();
				result = true;
			}
			return result;
		}

		private void OnLostFocus()
		{
			this.Invoke("OnLostFocus");
			base.Repaint();
		}

		public new void OnDestroy()
		{
			if (this.m_ActualView)
			{
				UnityEngine.Object.DestroyImmediate(this.m_ActualView, true);
			}
			base.OnDestroy();
		}

		protected Type[] GetPaneTypes()
		{
			return new Type[]
			{
				typeof(SceneView),
				typeof(GameView),
				typeof(InspectorWindow),
				typeof(SceneHierarchyWindow),
				typeof(ProjectBrowser),
				typeof(ProfilerWindow),
				typeof(AnimationWindow)
			};
		}

		internal void OnProjectChange()
		{
			this.Invoke("OnProjectChange");
		}

		internal void OnSelectionChange()
		{
			this.Invoke("OnSelectionChange");
		}

		internal void OnDidOpenScene()
		{
			this.Invoke("OnDidOpenScene");
		}

		internal void OnInspectorUpdate()
		{
			this.Invoke("OnInspectorUpdate");
		}

		internal void OnHierarchyChange()
		{
			this.Invoke("OnHierarchyChange");
		}

		private MethodInfo GetPaneMethod(string methodName)
		{
			return this.GetPaneMethod(methodName, this.m_ActualView);
		}

		private MethodInfo GetPaneMethod(string methodName, object obj)
		{
			MethodInfo result;
			if (obj == null)
			{
				result = null;
			}
			else
			{
				for (Type type = obj.GetType(); type != null; type = type.BaseType)
				{
					MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (method != null)
					{
						result = method;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		public static void EndOffsetArea()
		{
			if (Event.current.type != EventType.Used)
			{
				GUILayoutUtility.EndLayoutGroup();
				GUI.EndGroup();
			}
		}

		public static void BeginOffsetArea(Rect screenRect, GUIContent content, GUIStyle style)
		{
			GUILayoutGroup gUILayoutGroup = EditorGUILayoutUtilityInternal.BeginLayoutArea(style, typeof(GUILayoutGroup));
			EventType type = Event.current.type;
			if (type == EventType.Layout)
			{
				gUILayoutGroup.resetCoords = false;
				gUILayoutGroup.minWidth = (gUILayoutGroup.maxWidth = screenRect.width + 1f);
				gUILayoutGroup.minHeight = (gUILayoutGroup.maxHeight = screenRect.height + 2f);
				gUILayoutGroup.rect = Rect.MinMaxRect(-1f, -1f, gUILayoutGroup.rect.xMax, gUILayoutGroup.rect.yMax - 10f);
			}
			GUI.BeginGroup(screenRect, content, style);
		}

		public void InvokeOnGUI(Rect onGUIPosition)
		{
			base.DoWindowDecorationStart();
			GUIStyle gUIStyle = "dockareaoverlay";
			if (this.actualView is GameView)
			{
				GUI.Box(onGUIPosition, GUIContent.none, gUIStyle);
			}
			HostView.BeginOffsetArea(new Rect(onGUIPosition.x + 2f, onGUIPosition.y + 17f, onGUIPosition.width - 4f, onGUIPosition.height - 17f - 2f), GUIContent.none, "TabWindowBackground");
			EditorGUIUtility.ResetGUIState();
			bool flag = false;
			try
			{
				this.Invoke("OnGUI");
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException is ExitGUIException)
				{
					flag = true;
				}
				throw ex.InnerException;
			}
			finally
			{
				if (!flag)
				{
					if (this.actualView != null && this.actualView.m_FadeoutTime != 0f && Event.current != null && Event.current.type == EventType.Repaint)
					{
						this.actualView.DrawNotification();
					}
					HostView.EndOffsetArea();
					EditorGUIUtility.ResetGUIState();
					base.DoWindowDecorationEnd();
					if (Event.current.type == EventType.Repaint)
					{
						gUIStyle.Draw(onGUIPosition, GUIContent.none, 0);
					}
				}
			}
		}

		protected void Invoke(string methodName)
		{
			this.Invoke(methodName, this.m_ActualView);
		}

		protected void Invoke(string methodName, object obj)
		{
			MethodInfo paneMethod = this.GetPaneMethod(methodName, obj);
			if (paneMethod != null)
			{
				paneMethod.Invoke(obj, null);
			}
		}

		protected void RegisterSelectedPane()
		{
			if (this.m_ActualView)
			{
				this.m_ActualView.m_Parent = this;
				if (this.GetPaneMethod("Update") != null)
				{
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.SendUpdate));
				}
				if (this.GetPaneMethod("ModifierKeysChanged") != null)
				{
					EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendModKeysChanged));
				}
				this.m_ActualView.MakeParentsSettingsMatchMe();
				if (this.m_ActualView.m_FadeoutTime != 0f)
				{
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.m_ActualView.CheckForWindowRepaint));
				}
				this.OnViewChange(this.m_ActualView);
				try
				{
					this.Invoke("OnBecameVisible");
					this.Invoke("OnFocus");
				}
				catch (TargetInvocationException ex)
				{
					Debug.LogError(ex.InnerException.GetType().Name + ":" + ex.InnerException.Message);
				}
			}
		}

		protected void DeregisterSelectedPane(bool clearActualView)
		{
			if (this.m_ActualView)
			{
				if (this.GetPaneMethod("Update") != null)
				{
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.SendUpdate));
				}
				if (this.GetPaneMethod("ModifierKeysChanged") != null)
				{
					EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendModKeysChanged));
				}
				if (this.m_ActualView.m_FadeoutTime != 0f)
				{
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.m_ActualView.CheckForWindowRepaint));
				}
				if (clearActualView)
				{
					EditorWindow actualView = this.m_ActualView;
					this.m_ActualView = null;
					this.Invoke("OnLostFocus", actualView);
					this.Invoke("OnBecameInvisible", actualView);
				}
			}
		}

		private void SendUpdate()
		{
			this.Invoke("Update");
		}

		private void SendModKeysChanged()
		{
			this.Invoke("ModifierKeysChanged");
		}

		protected virtual RectOffset GetBorderSize()
		{
			return this.m_BorderSize;
		}

		protected void ShowGenericMenu()
		{
			GUIStyle gUIStyle = "PaneOptions";
			Rect rect = new Rect(base.position.width - gUIStyle.fixedWidth - 4f, Mathf.Floor((float)(this.background.margin.top + 20) - gUIStyle.fixedHeight), gUIStyle.fixedWidth, gUIStyle.fixedHeight);
			if (EditorGUI.DropdownButton(rect, GUIContent.none, FocusType.Passive, "PaneOptions"))
			{
				this.PopupGenericMenu(this.m_ActualView, rect);
			}
			MethodInfo paneMethod = this.GetPaneMethod("ShowButton", this.m_ActualView);
			if (paneMethod != null)
			{
				object[] parameters = new object[]
				{
					new Rect(base.position.width - gUIStyle.fixedWidth - 20f, Mathf.Floor((float)(this.background.margin.top + 4)), 16f, 16f)
				};
				paneMethod.Invoke(this.m_ActualView, parameters);
			}
		}

		public void PopupGenericMenu(EditorWindow view, Rect pos)
		{
			GenericMenu genericMenu = new GenericMenu();
			IHasCustomMenu hasCustomMenu = view as IHasCustomMenu;
			if (hasCustomMenu != null)
			{
				hasCustomMenu.AddItemsToMenu(genericMenu);
			}
			this.AddDefaultItemsToMenu(genericMenu, view);
			genericMenu.DropDown(pos);
			Event.current.Use();
		}

		protected virtual void AddDefaultItemsToMenu(GenericMenu menu, EditorWindow view)
		{
		}

		protected void ClearBackground()
		{
			if (Event.current.type == EventType.Repaint)
			{
				EditorWindow actualView = this.actualView;
				if (actualView != null && actualView.dontClearBackground)
				{
					if (base.backgroundValid && base.position == this.m_BackgroundClearRect)
					{
						return;
					}
				}
				Color color = (!EditorGUIUtility.isProSkin) ? HostView.kViewColor : EditorGUIUtility.kDarkViewBackground;
				GL.Clear(true, true, (!EditorApplication.isPlayingOrWillChangePlaymode) ? color : (color * HostView.kPlayModeDarken));
				base.backgroundValid = true;
				this.m_BackgroundClearRect = base.position;
			}
		}
	}
}
