using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[Serializable]
	internal class SceneViewRotation
	{
		private class Styles
		{
			public GUIStyle viewLabelStyleLeftAligned;

			public GUIStyle viewLabelStyleCentered;

			public GUIStyle viewAxisLabelStyle;

			public Styles()
			{
				this.viewLabelStyleLeftAligned = new GUIStyle("SC ViewLabel");
				this.viewLabelStyleCentered = new GUIStyle("SC ViewLabel");
				this.viewLabelStyleLeftAligned.alignment = TextAnchor.MiddleLeft;
				this.viewLabelStyleCentered.alignment = TextAnchor.MiddleCenter;
				this.viewAxisLabelStyle = "SC ViewAxisLabel";
			}
		}

		private const int kRotationSize = 100;

		private const int kRotationMenuInset = 22;

		private static Quaternion[] kDirectionRotations = new Quaternion[]
		{
			Quaternion.LookRotation(new Vector3(-1f, 0f, 0f)),
			Quaternion.LookRotation(new Vector3(0f, -1f, 0f)),
			Quaternion.LookRotation(new Vector3(0f, 0f, -1f)),
			Quaternion.LookRotation(new Vector3(1f, 0f, 0f)),
			Quaternion.LookRotation(new Vector3(0f, 1f, 0f)),
			Quaternion.LookRotation(new Vector3(0f, 0f, 1f))
		};

		private static string[] kDirNames = new string[]
		{
			"Right",
			"Top",
			"Front",
			"Left",
			"Bottom",
			"Back",
			"Iso",
			"Persp",
			"2D"
		};

		private static string[] kMenuDirNames = new string[]
		{
			"Free",
			"Right",
			"Top",
			"Front",
			"Left",
			"Bottom",
			"Back",
			string.Empty,
			"Perspective"
		};

		private int[] m_ViewDirectionControlIDs;

		private int m_CenterButtonControlID;

		private int currentDir = 7;

		private AnimBool[] dirVisible = new AnimBool[]
		{
			new AnimBool(true),
			new AnimBool(true),
			new AnimBool(true)
		};

		private AnimBool[] dirNameVisible = new AnimBool[]
		{
			new AnimBool(),
			new AnimBool(),
			new AnimBool(),
			new AnimBool(),
			new AnimBool(),
			new AnimBool(),
			new AnimBool(),
			new AnimBool(),
			new AnimBool()
		};

		private AnimBool m_Visible = new AnimBool();

		private static SceneViewRotation.Styles s_Styles;

		private float faded2Dgray
		{
			get
			{
				return this.dirNameVisible[8].faded;
			}
		}

		private static SceneViewRotation.Styles styles
		{
			get
			{
				if (SceneViewRotation.s_Styles == null)
				{
					SceneViewRotation.s_Styles = new SceneViewRotation.Styles();
				}
				return SceneViewRotation.s_Styles;
			}
		}

		public void Register(SceneView view)
		{
			for (int i = 0; i < this.dirVisible.Length; i++)
			{
				this.dirVisible[i].valueChanged.AddListener(new UnityAction(view.Repaint));
			}
			for (int j = 0; j < this.dirNameVisible.Length; j++)
			{
				this.dirNameVisible[j].valueChanged.AddListener(new UnityAction(view.Repaint));
			}
			this.m_Visible.valueChanged.AddListener(new UnityAction(view.Repaint));
			int labelIndexForView = this.GetLabelIndexForView(view, view.rotation * Vector3.forward, view.orthographic);
			for (int k = 0; k < this.dirNameVisible.Length; k++)
			{
				this.dirNameVisible[k].value = (k == labelIndexForView);
			}
			this.m_Visible.value = (labelIndexForView != 8);
			this.SwitchDirNameVisible(labelIndexForView);
			if (this.m_ViewDirectionControlIDs == null)
			{
				this.m_ViewDirectionControlIDs = new int[SceneViewRotation.kDirectionRotations.Length];
				for (int l = 0; l < this.m_ViewDirectionControlIDs.Length; l++)
				{
					this.m_ViewDirectionControlIDs[l] = GUIUtility.GetPermanentControlID();
				}
				this.m_CenterButtonControlID = GUIUtility.GetPermanentControlID();
			}
		}

		private void AxisSelectors(SceneView view, Camera cam, float size, float sgn, GUIStyle viewAxisLabelStyle)
		{
			for (int i = SceneViewRotation.kDirectionRotations.Length - 1; i >= 0; i--)
			{
				Quaternion quaternion = SceneViewRotation.kDirectionRotations[i];
				string[] array = new string[]
				{
					"x",
					"y",
					"z"
				};
				float faded = this.dirVisible[i % 3].faded;
				Vector3 vector = SceneViewRotation.kDirectionRotations[i] * Vector3.forward;
				float num = Vector3.Dot(view.camera.transform.forward, vector);
				if ((double)num > 0.0 || sgn <= 0f)
				{
					if ((double)num <= 0.0 || sgn >= 0f)
					{
						Color color;
						switch (i)
						{
						case 0:
							color = Handles.xAxisColor;
							break;
						case 1:
							color = Handles.yAxisColor;
							break;
						case 2:
							color = Handles.zAxisColor;
							break;
						default:
							color = Handles.centerColor;
							break;
						}
						if (view.in2DMode)
						{
							color = Color.Lerp(color, Color.gray, this.faded2Dgray);
						}
						color.a *= faded * this.m_Visible.faded;
						Handles.color = color;
						if (color.a <= 0.1f)
						{
							GUI.enabled = false;
						}
						if (sgn > 0f && Handles.Button(this.m_ViewDirectionControlIDs[i], quaternion * Vector3.forward * size * -1.2f, quaternion, size, size * 0.7f, new Handles.DrawCapFunction(Handles.ConeCap)) && !view.in2DMode)
						{
							this.ViewAxisDirection(view, i);
						}
						if (i < 3)
						{
							GUI.color = new Color(1f, 1f, 1f, this.dirVisible[i].faded * this.m_Visible.faded);
							Vector3 a = vector;
							a += num * view.camera.transform.forward * -0.5f;
							a = (a * 0.7f + a.normalized * 1.5f) * size;
							Handles.Label(-a, new GUIContent(array[i]), SceneViewRotation.styles.viewAxisLabelStyle);
						}
						if (sgn < 0f && Handles.Button(this.m_ViewDirectionControlIDs[i], quaternion * Vector3.forward * size * -1.2f, quaternion, size, size * 0.7f, new Handles.DrawCapFunction(Handles.ConeCap)) && !view.in2DMode)
						{
							this.ViewAxisDirection(view, i);
						}
						Handles.color = Color.white;
						GUI.color = Color.white;
						GUI.enabled = true;
					}
				}
			}
		}

		internal void HandleContextClick(SceneView view)
		{
			if (!view.in2DMode)
			{
				Event current = Event.current;
				if (current.type == EventType.MouseDown && current.button == 1)
				{
					float num = Mathf.Min(view.position.width, view.position.height);
					if (num < 100f)
					{
						return;
					}
					Rect rect = new Rect(view.position.width - 100f + 22f, 22f, 56f, 56f);
					if (rect.Contains(current.mousePosition))
					{
						this.DisplayContextMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), view);
						current.Use();
					}
				}
			}
		}

		private void DisplayContextMenu(Rect buttonOrCursorRect, SceneView view)
		{
			int[] array = new int[(!view.orthographic) ? 2 : 1];
			array[0] = ((this.currentDir < 6) ? (this.currentDir + 1) : 0);
			if (!view.orthographic)
			{
				array[1] = 8;
			}
			EditorUtility.DisplayCustomMenu(buttonOrCursorRect, SceneViewRotation.kMenuDirNames, array, new EditorUtility.SelectMenuItemFunction(this.ContextMenuDelegate), view);
			GUIUtility.ExitGUI();
		}

		private void ContextMenuDelegate(object userData, string[] options, int selected)
		{
			SceneView sceneView = userData as SceneView;
			if (sceneView == null)
			{
				return;
			}
			if (selected == 0)
			{
				this.ViewFromNiceAngle(sceneView, false);
			}
			else if (selected >= 1 && selected <= 6)
			{
				int dir = selected - 1;
				this.ViewAxisDirection(sceneView, dir);
			}
			else if (selected == 8)
			{
				this.ViewSetOrtho(sceneView, !sceneView.orthographic);
			}
			else if (selected == 10)
			{
				sceneView.LookAt(sceneView.pivot, Quaternion.LookRotation(new Vector3(-1f, -0.7f, -1f)), sceneView.size, sceneView.orthographic);
			}
			else if (selected == 11)
			{
				sceneView.LookAt(sceneView.pivot, Quaternion.LookRotation(new Vector3(1f, -0.7f, -1f)), sceneView.size, sceneView.orthographic);
			}
			else if (selected == 12)
			{
				sceneView.LookAt(sceneView.pivot, Quaternion.LookRotation(new Vector3(1f, -0.7f, 1f)), sceneView.size, sceneView.orthographic);
			}
		}

		private void DrawIsoStatusSymbol(Vector3 center, SceneView view, float alpha)
		{
			float num = 1f - Mathf.Clamp01(view.m_Ortho.faded * 1.2f - 0.1f);
			Vector3 a = Vector3.up * 3f;
			Vector3 vector = Vector3.right * 10f;
			Vector3 vector2 = center - vector * 0.5f;
			Handles.color = new Color(1f, 1f, 1f, 0.6f * alpha);
			Handles.DrawAAPolyLine(new Vector3[]
			{
				vector2 + a * (1f - num),
				vector2 + vector + a * (1f + num * 0.5f)
			});
			Handles.DrawAAPolyLine(new Vector3[]
			{
				vector2,
				vector2 + vector
			});
			Handles.DrawAAPolyLine(new Vector3[]
			{
				vector2 - a * (1f - num),
				vector2 + vector - a * (1f + num * 0.5f)
			});
		}

		private void DrawLabels(SceneView view)
		{
			Rect rect = new Rect(view.position.width - 100f + 17f, 92f, 66f, 16f);
			if (!view.in2DMode && GUI.Button(rect, string.Empty, SceneViewRotation.styles.viewLabelStyleLeftAligned))
			{
				if (Event.current.button == 1)
				{
					this.DisplayContextMenu(rect, view);
				}
				else
				{
					this.ViewSetOrtho(view, !view.orthographic);
				}
			}
			if (Event.current.type == EventType.Repaint)
			{
				int num = 8;
				Rect position = rect;
				float num2 = 0f;
				float num3 = 0f;
				for (int i = 0; i < SceneViewRotation.kDirNames.Length; i++)
				{
					if (i != num)
					{
						num3 += this.dirNameVisible[i].faded;
						if (this.dirNameVisible[i].faded > 0f)
						{
							num2 += SceneViewRotation.styles.viewLabelStyleLeftAligned.CalcSize(EditorGUIUtility.TempContent(SceneViewRotation.kDirNames[i])).x * this.dirNameVisible[i].faded;
						}
					}
				}
				if (num3 > 0f)
				{
					num2 /= num3;
				}
				position.x += 37f - num2 * 0.5f;
				position.x = (float)Mathf.RoundToInt(position.x);
				int num4 = 0;
				while (num4 < this.dirNameVisible.Length && num4 < SceneViewRotation.kDirNames.Length)
				{
					if (num4 != num)
					{
						Color centerColor = Handles.centerColor;
						centerColor.a *= this.dirNameVisible[num4].faded;
						if (centerColor.a > 0f)
						{
							GUI.color = centerColor;
							GUI.Label(position, SceneViewRotation.kDirNames[num4], SceneViewRotation.styles.viewLabelStyleLeftAligned);
						}
					}
					num4++;
				}
				Color centerColor2 = Handles.centerColor;
				centerColor2.a *= this.faded2Dgray * this.m_Visible.faded;
				if (centerColor2.a > 0f)
				{
					GUI.color = centerColor2;
					GUI.Label(rect, SceneViewRotation.kDirNames[num], SceneViewRotation.styles.viewLabelStyleCentered);
				}
				if (this.faded2Dgray < 1f)
				{
					this.DrawIsoStatusSymbol(new Vector3(position.x - 8f, position.y + 8.5f, 0f), view, 1f - this.faded2Dgray);
				}
			}
		}

		internal void OnGUI(SceneView view)
		{
			float num = Mathf.Min(view.position.width, view.position.height);
			if (num < 100f)
			{
				return;
			}
			if (Event.current.type == EventType.Repaint)
			{
				Profiler.BeginSample("SceneView.AxisSelector");
			}
			this.HandleContextClick(view);
			Camera camera = view.camera;
			HandleUtility.PushCamera(camera);
			if (camera.orthographic)
			{
				camera.orthographicSize = 0.5f;
			}
			camera.cullingMask = 0;
			camera.transform.position = camera.transform.rotation * new Vector3(0f, 0f, -5f);
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.nearClipPlane = 0.1f;
			camera.farClipPlane = 10f;
			camera.fieldOfView = view.m_Ortho.Fade(70f, 0f);
			SceneView.AddCursorRect(new Rect(view.position.width - 100f + 22f, 22f, 56f, 102f), MouseCursor.Arrow);
			Handles.SetCamera(new Rect(view.position.width - 100f, 0f, 100f, 100f), camera);
			Handles.BeginGUI();
			this.DrawLabels(view);
			Handles.EndGUI();
			for (int i = 0; i < 3; i++)
			{
				Vector3 rhs = SceneViewRotation.kDirectionRotations[i] * Vector3.forward;
				this.dirVisible[i].target = (Mathf.Abs(Vector3.Dot(camera.transform.forward, rhs)) < 0.9f);
			}
			float num2 = HandleUtility.GetHandleSize(Vector3.zero) * 0.2f;
			this.AxisSelectors(view, camera, num2, -1f, SceneViewRotation.styles.viewAxisLabelStyle);
			Color color = Handles.centerColor;
			color = Color.Lerp(color, Color.gray, this.faded2Dgray);
			color.a *= this.m_Visible.faded;
			if (color.a <= 0.1f)
			{
				GUI.enabled = false;
			}
			Handles.color = color;
			if (Handles.Button(this.m_CenterButtonControlID, Vector3.zero, Quaternion.identity, num2 * 0.8f, num2, new Handles.DrawCapFunction(Handles.CubeCap)) && !view.in2DMode)
			{
				if (Event.current.clickCount == 2)
				{
					view.FrameSelected();
				}
				else if (Event.current.shift || Event.current.button == 2)
				{
					this.ViewFromNiceAngle(view, true);
				}
				else
				{
					this.ViewSetOrtho(view, !view.orthographic);
				}
			}
			this.AxisSelectors(view, camera, num2, 1f, SceneViewRotation.styles.viewAxisLabelStyle);
			GUI.enabled = true;
			if (!view.in2DMode && Event.current.type == EditorGUIUtility.swipeGestureEventType)
			{
				Event current = Event.current;
				Vector3 a;
				if (current.delta.y > 0f)
				{
					a = Vector3.up;
				}
				else if (current.delta.y < 0f)
				{
					a = -Vector3.up;
				}
				else if (current.delta.x < 0f)
				{
					a = Vector3.right;
				}
				else
				{
					a = -Vector3.right;
				}
				Vector3 vector = -a - Vector3.forward * 0.9f;
				vector = view.camera.transform.TransformDirection(vector);
				float num3 = 0f;
				int dir = 0;
				for (int j = 0; j < 6; j++)
				{
					float num4 = Vector3.Dot(SceneViewRotation.kDirectionRotations[j] * -Vector3.forward, vector);
					if (num4 > num3)
					{
						num3 = num4;
						dir = j;
					}
				}
				this.ViewAxisDirection(view, dir);
				Event.current.Use();
			}
			HandleUtility.PopCamera(camera);
			Handles.SetCamera(camera);
			if (Event.current.type == EventType.Repaint)
			{
				Profiler.EndSample();
			}
		}

		private void ViewAxisDirection(SceneView view, int dir)
		{
			bool ortho = view.orthographic;
			if (Event.current != null && (Event.current.shift || Event.current.button == 2))
			{
				ortho = true;
			}
			view.LookAt(view.pivot, SceneViewRotation.kDirectionRotations[dir], view.size, ortho);
			this.SwitchDirNameVisible(dir);
		}

		private void ViewSetOrtho(SceneView view, bool ortho)
		{
			view.LookAt(view.pivot, view.rotation, view.size, ortho);
		}

		internal void UpdateGizmoLabel(SceneView view, Vector3 direction, bool ortho)
		{
			this.SwitchDirNameVisible(this.GetLabelIndexForView(view, direction, ortho));
		}

		internal int GetLabelIndexForView(SceneView view, Vector3 direction, bool ortho)
		{
			if (!view.in2DMode)
			{
				if (this.IsAxisAligned(direction))
				{
					for (int i = 0; i < 6; i++)
					{
						if (Vector3.Dot(SceneViewRotation.kDirectionRotations[i] * Vector3.forward, direction) > 0.9f)
						{
							return i;
						}
					}
				}
				return (!ortho) ? 7 : 6;
			}
			return 8;
		}

		private void ViewFromNiceAngle(SceneView view, bool forcePerspective)
		{
			Vector3 vector = view.rotation * Vector3.forward;
			vector.y = 0f;
			if (vector == Vector3.zero)
			{
				vector = Vector3.forward;
			}
			else
			{
				vector = vector.normalized;
			}
			vector.y = -0.5f;
			bool flag = !forcePerspective && view.orthographic;
			view.LookAt(view.pivot, Quaternion.LookRotation(vector), view.size, flag);
			this.SwitchDirNameVisible((!flag) ? 7 : 6);
		}

		private bool IsAxisAligned(Vector3 v)
		{
			return Mathf.Abs(v.x * v.y) < 0.0001f && Mathf.Abs(v.y * v.z) < 0.0001f && Mathf.Abs(v.z * v.x) < 0.0001f;
		}

		private void SwitchDirNameVisible(int newVisible)
		{
			if (newVisible == this.currentDir)
			{
				return;
			}
			this.dirNameVisible[this.currentDir].target = false;
			this.currentDir = newVisible;
			this.dirNameVisible[this.currentDir].target = true;
			if (newVisible == 8)
			{
				this.m_Visible.speed = 0.3f;
			}
			else
			{
				this.m_Visible.speed = 2f;
			}
			this.m_Visible.target = (newVisible != 8);
		}
	}
}
