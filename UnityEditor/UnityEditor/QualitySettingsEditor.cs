using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(QualitySettings))]
	internal class QualitySettingsEditor : Editor
	{
		private static class Styles
		{
			public static readonly GUIStyle kToggle = "OL Toggle";

			public static readonly GUIStyle kDefaultToggle = "OL ToggleWhite";

			public static readonly GUIStyle kButton = "Button";

			public static readonly GUIStyle kSelected = "PR Label";

			public static readonly GUIContent kPlatformTooltip = new GUIContent("", "Allow quality setting on platform");

			public static readonly GUIContent kIconTrash = EditorGUIUtility.IconContent("TreeEditor.Trash", "|Delete Level");

			public static readonly GUIContent kSoftParticlesHint = EditorGUIUtility.TextContent("Soft Particles require using Deferred Lighting or making camera render the depth texture.");

			public static readonly GUIContent kBillboardsFaceCameraPos = EditorGUIUtility.TextContent("Billboards Face Camera Position|Make billboards face towards camera position. Otherwise they face towards camera plane. This makes billboards look nicer when camera rotates but is more expensive to render.");

			public static readonly GUIStyle kListEvenBg = "ObjectPickerResultsOdd";

			public static readonly GUIStyle kListOddBg = "ObjectPickerResultsEven";

			public static readonly GUIStyle kDefaultDropdown = "QualitySettingsDefault";

			public const int kMinToggleWidth = 15;

			public const int kMaxToggleWidth = 20;

			public const int kHeaderRowHeight = 20;

			public const int kLabelWidth = 80;
		}

		private struct QualitySetting
		{
			public string m_Name;

			public string m_PropertyPath;

			public List<string> m_ExcludedPlatforms;
		}

		private class Dragging
		{
			public int m_StartPosition;

			public int m_Position;
		}

		public const int kMinAsyncRingBufferSize = 2;

		public const int kMaxAsyncRingBufferSize = 512;

		public const int kMinAsyncUploadTimeSlice = 1;

		public const int kMaxAsyncUploadTimeSlice = 33;

		private SerializedObject m_QualitySettings;

		private SerializedProperty m_QualitySettingsProperty;

		private SerializedProperty m_PerPlatformDefaultQualityProperty;

		private List<BuildPlayerWindow.BuildPlatform> m_ValidPlatforms;

		private readonly int m_QualityElementHash = "QualityElementHash".GetHashCode();

		private QualitySettingsEditor.Dragging m_Dragging;

		private bool m_ShouldAddNewLevel;

		private int m_DeleteLevel = -1;

		public void OnEnable()
		{
			this.m_QualitySettings = new SerializedObject(base.target);
			this.m_QualitySettingsProperty = this.m_QualitySettings.FindProperty("m_QualitySettings");
			this.m_PerPlatformDefaultQualityProperty = this.m_QualitySettings.FindProperty("m_PerPlatformDefaultQuality");
			this.m_ValidPlatforms = BuildPlayerWindow.GetValidPlatforms();
		}

		private int DoQualityLevelSelection(int currentQualitylevel, IList<QualitySettingsEditor.QualitySetting> qualitySettings, Dictionary<string, int> platformDefaultQualitySettings)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			int num = currentQualitylevel;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false),
				GUILayout.Width(80f),
				GUILayout.Height(20f)
			});
			rect.x += EditorGUI.indent;
			rect.width -= EditorGUI.indent;
			GUI.Label(rect, "Levels", EditorStyles.boldLabel);
			foreach (BuildPlayerWindow.BuildPlatform current in this.m_ValidPlatforms)
			{
				Rect rect2 = GUILayoutUtility.GetRect(GUIContent.none, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
				{
					GUILayout.MinWidth(15f),
					GUILayout.MaxWidth(20f),
					GUILayout.Height(20f)
				});
				GUIContent gUIContent = EditorGUIUtility.TempContent(current.smallIcon);
				gUIContent.tooltip = current.title.text;
				GUI.Label(rect2, gUIContent);
				gUIContent.tooltip = "";
			}
			GUILayoutUtility.GetRect(GUIContent.none, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
			{
				GUILayout.MinWidth(15f),
				GUILayout.MaxWidth(20f),
				GUILayout.Height(20f)
			});
			GUILayout.EndHorizontal();
			Event current2 = Event.current;
			for (int i = 0; i < qualitySettings.Count; i++)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUIStyle gUIStyle = (i % 2 != 0) ? QualitySettingsEditor.Styles.kListOddBg : QualitySettingsEditor.Styles.kListEvenBg;
				bool on = num == i;
				Rect rect3 = GUILayoutUtility.GetRect(GUIContent.none, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false),
					GUILayout.Width(80f)
				});
				switch (current2.type)
				{
				case EventType.MouseDown:
					if (rect3.Contains(current2.mousePosition))
					{
						num = i;
						GUIUtility.keyboardControl = 0;
						GUIUtility.hotControl = this.m_QualityElementHash;
						GUI.changed = true;
						this.m_Dragging = new QualitySettingsEditor.Dragging
						{
							m_StartPosition = i,
							m_Position = i
						};
						current2.Use();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == this.m_QualityElementHash)
					{
						GUIUtility.hotControl = 0;
						current2.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == this.m_QualityElementHash)
					{
						if (rect3.Contains(current2.mousePosition))
						{
							this.m_Dragging.m_Position = i;
							current2.Use();
						}
					}
					break;
				case EventType.KeyDown:
					if (current2.keyCode == KeyCode.UpArrow || current2.keyCode == KeyCode.DownArrow)
					{
						num += ((current2.keyCode != KeyCode.UpArrow) ? 1 : -1);
						num = Mathf.Clamp(num, 0, qualitySettings.Count - 1);
						GUIUtility.keyboardControl = 0;
						GUI.changed = true;
						current2.Use();
					}
					break;
				case EventType.Repaint:
					gUIStyle.Draw(rect3, GUIContent.none, false, false, on, false);
					GUI.Label(rect3, EditorGUIUtility.TempContent(qualitySettings[i].m_Name));
					break;
				}
				foreach (BuildPlayerWindow.BuildPlatform current3 in this.m_ValidPlatforms)
				{
					bool flag = false;
					if (platformDefaultQualitySettings.ContainsKey(current3.name) && platformDefaultQualitySettings[current3.name] == i)
					{
						flag = true;
					}
					Rect rect4 = GUILayoutUtility.GetRect(QualitySettingsEditor.Styles.kPlatformTooltip, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
					{
						GUILayout.MinWidth(15f),
						GUILayout.MaxWidth(20f)
					});
					if (Event.current.type == EventType.Repaint)
					{
						gUIStyle.Draw(rect4, GUIContent.none, false, false, on, false);
					}
					Color backgroundColor = GUI.backgroundColor;
					if (flag && !EditorApplication.isPlayingOrWillChangePlaymode)
					{
						GUI.backgroundColor = Color.green;
					}
					bool flag2 = !qualitySettings[i].m_ExcludedPlatforms.Contains(current3.name);
					bool flag3 = GUI.Toggle(rect4, flag2, QualitySettingsEditor.Styles.kPlatformTooltip, (!flag) ? QualitySettingsEditor.Styles.kToggle : QualitySettingsEditor.Styles.kDefaultToggle);
					if (flag2 != flag3)
					{
						if (flag3)
						{
							qualitySettings[i].m_ExcludedPlatforms.Remove(current3.name);
						}
						else
						{
							qualitySettings[i].m_ExcludedPlatforms.Add(current3.name);
						}
					}
					GUI.backgroundColor = backgroundColor;
				}
				Rect rect5 = GUILayoutUtility.GetRect(GUIContent.none, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
				{
					GUILayout.MinWidth(15f),
					GUILayout.MaxWidth(20f)
				});
				if (Event.current.type == EventType.Repaint)
				{
					gUIStyle.Draw(rect5, GUIContent.none, false, false, on, false);
				}
				if (GUI.Button(rect5, QualitySettingsEditor.Styles.kIconTrash, GUIStyle.none))
				{
					this.m_DeleteLevel = i;
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayoutUtility.GetRect(GUIContent.none, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
			{
				GUILayout.MinWidth(15f),
				GUILayout.MaxWidth(20f),
				GUILayout.Height(1f)
			});
			QualitySettingsEditor.DrawHorizontalDivider();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Rect rect6 = GUILayoutUtility.GetRect(GUIContent.none, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false),
				GUILayout.Width(80f),
				GUILayout.Height(20f)
			});
			rect6.x += EditorGUI.indent;
			rect6.width -= EditorGUI.indent;
			GUI.Label(rect6, "Default", EditorStyles.boldLabel);
			foreach (BuildPlayerWindow.BuildPlatform current4 in this.m_ValidPlatforms)
			{
				Rect rect7 = GUILayoutUtility.GetRect(GUIContent.none, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
				{
					GUILayout.MinWidth(15f),
					GUILayout.MaxWidth(20f),
					GUILayout.Height(20f)
				});
				int num2;
				if (!platformDefaultQualitySettings.TryGetValue(current4.name, out num2))
				{
					platformDefaultQualitySettings.Add(current4.name, 0);
				}
				num2 = EditorGUI.Popup(rect7, num2, (from x in qualitySettings
				select x.m_Name).ToArray<string>(), QualitySettingsEditor.Styles.kDefaultDropdown);
				platformDefaultQualitySettings[current4.name] = num2;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayoutUtility.GetRect(GUIContent.none, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
			{
				GUILayout.MinWidth(15f),
				GUILayout.MaxWidth(20f),
				GUILayout.Height(20f)
			});
			Rect rect8 = GUILayoutUtility.GetRect(GUIContent.none, QualitySettingsEditor.Styles.kToggle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			if (GUI.Button(rect8, EditorGUIUtility.TempContent("Add Quality Level")))
			{
				this.m_ShouldAddNewLevel = true;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			return num;
		}

		private List<QualitySettingsEditor.QualitySetting> GetQualitySettings()
		{
			List<QualitySettingsEditor.QualitySetting> list = new List<QualitySettingsEditor.QualitySetting>();
			IEnumerator enumerator = this.m_QualitySettingsProperty.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SerializedProperty serializedProperty = (SerializedProperty)enumerator.Current;
					QualitySettingsEditor.QualitySetting item = new QualitySettingsEditor.QualitySetting
					{
						m_Name = serializedProperty.FindPropertyRelative("name").stringValue,
						m_PropertyPath = serializedProperty.propertyPath
					};
					item.m_PropertyPath = serializedProperty.propertyPath;
					List<string> list2 = new List<string>();
					SerializedProperty serializedProperty2 = serializedProperty.FindPropertyRelative("excludedTargetPlatforms");
					IEnumerator enumerator2 = serializedProperty2.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							SerializedProperty serializedProperty3 = (SerializedProperty)enumerator2.Current;
							list2.Add(serializedProperty3.stringValue);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator2 as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					item.m_ExcludedPlatforms = list2;
					list.Add(item);
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			return list;
		}

		private void SetQualitySettings(IEnumerable<QualitySettingsEditor.QualitySetting> settings)
		{
			foreach (QualitySettingsEditor.QualitySetting current in settings)
			{
				SerializedProperty serializedProperty = this.m_QualitySettings.FindProperty(current.m_PropertyPath);
				if (serializedProperty != null)
				{
					SerializedProperty serializedProperty2 = serializedProperty.FindPropertyRelative("excludedTargetPlatforms");
					if (serializedProperty2.arraySize != current.m_ExcludedPlatforms.Count)
					{
						serializedProperty2.arraySize = current.m_ExcludedPlatforms.Count;
					}
					int num = 0;
					IEnumerator enumerator2 = serializedProperty2.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							SerializedProperty serializedProperty3 = (SerializedProperty)enumerator2.Current;
							if (serializedProperty3.stringValue != current.m_ExcludedPlatforms[num])
							{
								serializedProperty3.stringValue = current.m_ExcludedPlatforms[num];
							}
							num++;
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator2 as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
		}

		private void HandleAddRemoveQualitySetting(ref int selectedLevel, Dictionary<string, int> platformDefaults)
		{
			if (this.m_DeleteLevel >= 0)
			{
				if (this.m_DeleteLevel < selectedLevel || this.m_DeleteLevel == this.m_QualitySettingsProperty.arraySize - 1)
				{
					selectedLevel--;
				}
				if (this.m_QualitySettingsProperty.arraySize > 1 && this.m_DeleteLevel >= 0 && this.m_DeleteLevel < this.m_QualitySettingsProperty.arraySize)
				{
					this.m_QualitySettingsProperty.DeleteArrayElementAtIndex(this.m_DeleteLevel);
					List<string> list = new List<string>(platformDefaults.Keys);
					foreach (string current in list)
					{
						int num = platformDefaults[current];
						if (num != 0 && num >= this.m_DeleteLevel)
						{
							string key;
							platformDefaults[key = current] = platformDefaults[key] - 1;
						}
					}
				}
				this.m_DeleteLevel = -1;
			}
			if (this.m_ShouldAddNewLevel)
			{
				this.m_QualitySettingsProperty.arraySize++;
				SerializedProperty arrayElementAtIndex = this.m_QualitySettingsProperty.GetArrayElementAtIndex(this.m_QualitySettingsProperty.arraySize - 1);
				SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("name");
				serializedProperty.stringValue = "Level " + (this.m_QualitySettingsProperty.arraySize - 1);
				this.m_ShouldAddNewLevel = false;
			}
		}

		private Dictionary<string, int> GetDefaultQualityForPlatforms()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			IEnumerator enumerator = this.m_PerPlatformDefaultQualityProperty.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SerializedProperty serializedProperty = (SerializedProperty)enumerator.Current;
					dictionary.Add(serializedProperty.FindPropertyRelative("first").stringValue, serializedProperty.FindPropertyRelative("second").intValue);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return dictionary;
		}

		private void SetDefaultQualityForPlatforms(Dictionary<string, int> platformDefaults)
		{
			if (this.m_PerPlatformDefaultQualityProperty.arraySize != platformDefaults.Count)
			{
				this.m_PerPlatformDefaultQualityProperty.arraySize = platformDefaults.Count;
			}
			int num = 0;
			foreach (KeyValuePair<string, int> current in platformDefaults)
			{
				SerializedProperty arrayElementAtIndex = this.m_PerPlatformDefaultQualityProperty.GetArrayElementAtIndex(num);
				SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("first");
				SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("second");
				if (serializedProperty.stringValue != current.Key || serializedProperty2.intValue != current.Value)
				{
					serializedProperty.stringValue = current.Key;
					serializedProperty2.intValue = current.Value;
				}
				num++;
			}
		}

		private static void DrawHorizontalDivider()
		{
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(1f)
			});
			Color backgroundColor = GUI.backgroundColor;
			if (EditorGUIUtility.isProSkin)
			{
				GUI.backgroundColor = backgroundColor * 0.7058f;
			}
			else
			{
				GUI.backgroundColor = Color.black;
			}
			if (Event.current.type == EventType.Repaint)
			{
				EditorGUIUtility.whiteTextureStyle.Draw(rect, GUIContent.none, false, false, false, false);
			}
			GUI.backgroundColor = backgroundColor;
		}

		private void SoftParticlesHintGUI()
		{
			Camera main = Camera.main;
			if (!(main == null))
			{
				RenderingPath actualRenderingPath = main.actualRenderingPath;
				if (actualRenderingPath != RenderingPath.DeferredLighting && actualRenderingPath != RenderingPath.DeferredShading)
				{
					if ((main.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
					{
						EditorGUILayout.HelpBox(QualitySettingsEditor.Styles.kSoftParticlesHint.text, MessageType.Warning, false);
					}
				}
			}
		}

		private void DrawCascadeSplitGUI<T>(ref SerializedProperty shadowCascadeSplit)
		{
			float[] array = null;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(float))
			{
				array = new float[]
				{
					shadowCascadeSplit.floatValue
				};
			}
			else if (typeFromHandle == typeof(Vector3))
			{
				Vector3 vector3Value = shadowCascadeSplit.vector3Value;
				array = new float[]
				{
					Mathf.Clamp(vector3Value[0], 0f, 1f),
					Mathf.Clamp(vector3Value[1] - vector3Value[0], 0f, 1f),
					Mathf.Clamp(vector3Value[2] - vector3Value[1], 0f, 1f)
				};
			}
			if (array != null)
			{
				EditorGUI.BeginChangeCheck();
				ShadowCascadeSplitGUI.HandleCascadeSliderGUI(ref array);
				if (EditorGUI.EndChangeCheck())
				{
					if (typeFromHandle == typeof(float))
					{
						shadowCascadeSplit.floatValue = array[0];
					}
					else
					{
						Vector3 vector3Value2 = default(Vector3);
						vector3Value2[0] = array[0];
						vector3Value2[1] = vector3Value2[0] + array[1];
						vector3Value2[2] = vector3Value2[1] + array[2];
						shadowCascadeSplit.vector3Value = vector3Value2;
					}
				}
			}
		}

		public override void OnInspectorGUI()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorGUILayout.HelpBox("Changes made in play mode will not be saved.", MessageType.Warning, true);
			}
			this.m_QualitySettings.Update();
			List<QualitySettingsEditor.QualitySetting> qualitySettings = this.GetQualitySettings();
			Dictionary<string, int> defaultQualityForPlatforms = this.GetDefaultQualityForPlatforms();
			int num = QualitySettings.GetQualityLevel();
			EditorGUI.BeginChangeCheck();
			num = this.DoQualityLevelSelection(num, qualitySettings, defaultQualityForPlatforms);
			if (EditorGUI.EndChangeCheck())
			{
				QualitySettings.SetQualityLevel(num);
			}
			this.SetQualitySettings(qualitySettings);
			this.HandleAddRemoveQualitySetting(ref num, defaultQualityForPlatforms);
			this.SetDefaultQualityForPlatforms(defaultQualityForPlatforms);
			GUILayout.Space(10f);
			QualitySettingsEditor.DrawHorizontalDivider();
			GUILayout.Space(10f);
			SerializedProperty arrayElementAtIndex = this.m_QualitySettingsProperty.GetArrayElementAtIndex(num);
			SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("name");
			SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("pixelLightCount");
			SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("shadows");
			SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("shadowResolution");
			SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("shadowProjection");
			SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("shadowCascades");
			SerializedProperty property5 = arrayElementAtIndex.FindPropertyRelative("shadowDistance");
			SerializedProperty property6 = arrayElementAtIndex.FindPropertyRelative("shadowNearPlaneOffset");
			SerializedProperty serializedProperty3 = arrayElementAtIndex.FindPropertyRelative("shadowCascade2Split");
			SerializedProperty serializedProperty4 = arrayElementAtIndex.FindPropertyRelative("shadowCascade4Split");
			SerializedProperty property7 = arrayElementAtIndex.FindPropertyRelative("blendWeights");
			SerializedProperty property8 = arrayElementAtIndex.FindPropertyRelative("textureQuality");
			SerializedProperty property9 = arrayElementAtIndex.FindPropertyRelative("anisotropicTextures");
			SerializedProperty property10 = arrayElementAtIndex.FindPropertyRelative("antiAliasing");
			SerializedProperty serializedProperty5 = arrayElementAtIndex.FindPropertyRelative("softParticles");
			SerializedProperty property11 = arrayElementAtIndex.FindPropertyRelative("realtimeReflectionProbes");
			SerializedProperty property12 = arrayElementAtIndex.FindPropertyRelative("billboardsFaceCameraPosition");
			SerializedProperty property13 = arrayElementAtIndex.FindPropertyRelative("vSyncCount");
			SerializedProperty property14 = arrayElementAtIndex.FindPropertyRelative("lodBias");
			SerializedProperty property15 = arrayElementAtIndex.FindPropertyRelative("maximumLODLevel");
			SerializedProperty property16 = arrayElementAtIndex.FindPropertyRelative("particleRaycastBudget");
			SerializedProperty serializedProperty6 = arrayElementAtIndex.FindPropertyRelative("asyncUploadTimeSlice");
			SerializedProperty serializedProperty7 = arrayElementAtIndex.FindPropertyRelative("asyncUploadBufferSize");
			if (string.IsNullOrEmpty(serializedProperty.stringValue))
			{
				serializedProperty.stringValue = "Level " + num;
			}
			EditorGUILayout.PropertyField(serializedProperty, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label(EditorGUIUtility.TempContent("Rendering"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property8, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property9, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property10, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(serializedProperty5, new GUILayoutOption[0]);
			if (serializedProperty5.boolValue)
			{
				this.SoftParticlesHintGUI();
			}
			EditorGUILayout.PropertyField(property11, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property12, QualitySettingsEditor.Styles.kBillboardsFaceCameraPos, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label(EditorGUIUtility.TempContent("Shadows"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property2, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property3, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property4, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property5, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property6, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(serializedProperty2, new GUILayoutOption[0]);
			if (serializedProperty2.intValue == 2)
			{
				this.DrawCascadeSplitGUI<float>(ref serializedProperty3);
			}
			else if (serializedProperty2.intValue == 4)
			{
				this.DrawCascadeSplitGUI<Vector3>(ref serializedProperty4);
			}
			GUILayout.Space(10f);
			GUILayout.Label(EditorGUIUtility.TempContent("Other"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property7, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property13, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property14, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property15, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property16, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(serializedProperty6, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(serializedProperty7, new GUILayoutOption[0]);
			serializedProperty6.intValue = Mathf.Clamp(serializedProperty6.intValue, 1, 33);
			serializedProperty7.intValue = Mathf.Clamp(serializedProperty7.intValue, 2, 512);
			if (this.m_Dragging != null && this.m_Dragging.m_Position != this.m_Dragging.m_StartPosition)
			{
				this.m_QualitySettingsProperty.MoveArrayElement(this.m_Dragging.m_StartPosition, this.m_Dragging.m_Position);
				this.m_Dragging.m_StartPosition = this.m_Dragging.m_Position;
				num = this.m_Dragging.m_Position;
				this.m_QualitySettings.ApplyModifiedProperties();
				QualitySettings.SetQualityLevel(Mathf.Clamp(num, 0, this.m_QualitySettingsProperty.arraySize - 1));
			}
			this.m_QualitySettings.ApplyModifiedProperties();
		}
	}
}
