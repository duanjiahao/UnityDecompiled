using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class Chart
	{
		internal enum ChartAction
		{
			None,
			Activated,
			Closed
		}

		internal enum ChartType
		{
			StackedFill,
			Line
		}

		internal class Styles
		{
			public GUIContent performanceWarning = new GUIContent("", EditorGUIUtility.LoadIcon("console.warnicon.sml"), "Collecting GPU Profiler data might have overhead. Close graph if you don't need its data");

			public GUIStyle background = "OL Box";

			public GUIStyle leftPane = "ProfilerLeftPane";

			public GUIStyle rightPane = "ProfilerRightPane";

			public GUIStyle paneSubLabel = "ProfilerPaneSubLabel";

			public GUIStyle closeButton = "WinBtnClose";

			public GUIStyle whiteLabel = "ProfilerBadge";

			public GUIStyle selectedLabel = "ProfilerSelectedLabel";

			public Color selectedFrameColor1 = new Color(1f, 1f, 1f, 0.6f);

			public Color selectedFrameColor2 = new Color(1f, 1f, 1f, 0.7f);
		}

		private static int s_ChartHash = "Charts".GetHashCode();

		public const float kSideWidth = 170f;

		private const int kDistFromTopToFirstLabel = 20;

		private const int kLabelHeight = 11;

		private const int kCloseButtonSize = 13;

		private const float kLabelXOffset = 40f;

		private const float kWarningLabelHeightOffset = 43f;

		private Vector3[] m_CachedLineData;

		private string m_ChartSettingsName;

		private int m_chartControlID;

		private static Chart.Styles ms_Styles = null;

		private int m_DragItemIndex = -1;

		private Vector2 m_DragDownPos;

		private int[] m_ChartOrderBackup;

		private int m_MouseDownIndex = -1;

		public string m_NotSupportedWarning = null;

		public void LoadAndBindSettings(string chartSettingsName, ChartData cdata)
		{
			this.m_ChartSettingsName = chartSettingsName;
			this.LoadChartsSettings(cdata);
		}

		private int MoveSelectedFrame(int selectedFrame, ChartData cdata, int direction)
		{
			int numberOfFrames = cdata.NumberOfFrames;
			int num = selectedFrame + direction;
			int result;
			if (num < cdata.firstSelectableFrame || num > cdata.firstFrame + numberOfFrames)
			{
				result = selectedFrame;
			}
			else
			{
				result = num;
			}
			return result;
		}

		private int DoFrameSelectionDrag(float x, Rect r, ChartData cdata, int len)
		{
			int num = Mathf.RoundToInt((x - r.x) / r.width * (float)len - 0.5f);
			GUI.changed = true;
			return Mathf.Clamp(num + cdata.firstFrame, cdata.firstSelectableFrame, cdata.firstFrame + len);
		}

		private int HandleFrameSelectionEvents(int selectedFrame, int chartControlID, Rect chartFrame, ChartData cdata, int len)
		{
			Event current = Event.current;
			switch (current.type)
			{
			case EventType.MouseDown:
				if (chartFrame.Contains(current.mousePosition))
				{
					GUIUtility.keyboardControl = chartControlID;
					GUIUtility.hotControl = chartControlID;
					selectedFrame = this.DoFrameSelectionDrag(current.mousePosition.x, chartFrame, cdata, len);
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == chartControlID)
				{
					GUIUtility.hotControl = 0;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == chartControlID)
				{
					selectedFrame = this.DoFrameSelectionDrag(current.mousePosition.x, chartFrame, cdata, len);
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.keyboardControl == chartControlID && selectedFrame >= 0)
				{
					if (current.keyCode == KeyCode.LeftArrow)
					{
						selectedFrame = this.MoveSelectedFrame(selectedFrame, cdata, -1);
						current.Use();
					}
					else if (current.keyCode == KeyCode.RightArrow)
					{
						selectedFrame = this.MoveSelectedFrame(selectedFrame, cdata, 1);
						current.Use();
					}
				}
				break;
			}
			return selectedFrame;
		}

		public void OnLostFocus()
		{
			if (GUIUtility.hotControl == this.m_chartControlID)
			{
				GUIUtility.hotControl = 0;
			}
			this.m_chartControlID = 0;
		}

		public int DoGUI(Chart.ChartType type, int selectedFrame, ChartData cdata, ProfilerArea area, bool active, GUIContent icon, out Chart.ChartAction action)
		{
			action = Chart.ChartAction.None;
			int result;
			if (cdata == null)
			{
				result = selectedFrame;
			}
			else
			{
				int numberOfFrames = cdata.NumberOfFrames;
				if (Chart.ms_Styles == null)
				{
					Chart.ms_Styles = new Chart.Styles();
				}
				this.m_chartControlID = GUIUtility.GetControlID(Chart.s_ChartHash, FocusType.Keyboard);
				Rect rect = GUILayoutUtility.GetRect(GUIContent.none, Chart.ms_Styles.background, new GUILayoutOption[]
				{
					GUILayout.MinHeight(120f)
				});
				Rect rect2 = rect;
				rect2.x += 170f;
				rect2.width -= 170f;
				Event current = Event.current;
				if (current.GetTypeForControl(this.m_chartControlID) == EventType.MouseDown && rect.Contains(current.mousePosition))
				{
					action = Chart.ChartAction.Activated;
				}
				if (this.m_DragItemIndex == -1)
				{
					selectedFrame = this.HandleFrameSelectionEvents(selectedFrame, this.m_chartControlID, rect2, cdata, numberOfFrames);
				}
				Rect rect3 = rect2;
				rect3.x -= 170f;
				rect3.width = 170f;
				GUI.Label(new Rect(rect3.x, rect3.y, rect3.width, 20f), GUIContent.Temp("", icon.tooltip));
				if (current.type == EventType.Repaint)
				{
					Chart.ms_Styles.rightPane.Draw(rect2, false, false, active, false);
					Chart.ms_Styles.leftPane.Draw(rect3, EditorGUIUtility.TempContent(icon.text), false, false, active, false);
					if (this.m_NotSupportedWarning == null)
					{
						rect2.height -= 1f;
						if (type == Chart.ChartType.StackedFill)
						{
							this.DrawChartStacked(selectedFrame, cdata, rect2);
						}
						else
						{
							this.DrawChartLine(selectedFrame, cdata, rect2);
						}
					}
					else
					{
						Rect position = rect2;
						position.x += 56.1000023f;
						position.y += 43f;
						GUI.Label(position, this.m_NotSupportedWarning, EditorStyles.boldLabel);
					}
					rect3.x += 10f;
					rect3.y += 10f;
					GUIStyle.none.Draw(rect3, EditorGUIUtility.TempContent(icon.image), false, false, false, false);
					rect3.x += 40f;
					this.DrawLabelDragger(type, rect3, cdata);
				}
				else
				{
					rect3.y += 10f;
					this.LabelDraggerDrag(this.m_chartControlID, type, cdata, rect3, active);
				}
				if (area == ProfilerArea.GPU)
				{
					GUI.Label(new Rect(rect.x + 170f - (float)Chart.ms_Styles.performanceWarning.image.width, rect.yMax - (float)Chart.ms_Styles.performanceWarning.image.height - 2f, (float)Chart.ms_Styles.performanceWarning.image.width, (float)Chart.ms_Styles.performanceWarning.image.height), Chart.ms_Styles.performanceWarning);
				}
				if (GUI.Button(new Rect(rect.x + 170f - 13f - 2f, rect.y + 2f, 13f, 13f), GUIContent.none, Chart.ms_Styles.closeButton))
				{
					action = Chart.ChartAction.Closed;
				}
				result = selectedFrame;
			}
			return result;
		}

		private void DrawSelectedFrame(int selectedFrame, ChartData cdata, Rect r)
		{
			if (cdata.firstSelectableFrame != -1 && selectedFrame - cdata.firstSelectableFrame >= 0)
			{
				Chart.DrawVerticalLine(selectedFrame, cdata, r, Chart.ms_Styles.selectedFrameColor1, Chart.ms_Styles.selectedFrameColor2, 1f);
			}
		}

		internal static void DrawVerticalLine(int frame, ChartData cdata, Rect r, Color color1, Color color2, float widthFactor)
		{
			if (Event.current.type == EventType.Repaint)
			{
				frame -= cdata.firstFrame;
				if (frame >= 0)
				{
					float num = (float)cdata.NumberOfFrames;
					HandleUtility.ApplyWireMaterial();
					GL.Begin(7);
					GL.Color(color1);
					GL.Vertex3(r.x + r.width / num * (float)frame, r.y + 1f, 0f);
					GL.Vertex3(r.x + r.width / num * (float)frame + r.width / num, r.y + 1f, 0f);
					GL.Color(color2);
					GL.Vertex3(r.x + r.width / num * (float)frame + r.width / num, r.yMax, 0f);
					GL.Vertex3(r.x + r.width / num * (float)frame, r.yMax, 0f);
					GL.End();
				}
			}
		}

		private void DrawMaxValueScale(ChartData cdata, Rect r)
		{
			Handles.Label(new Vector3(r.x + r.width / 2f - 20f, r.yMin + 2f, 0f), "Scale: " + cdata.maxValue);
		}

		private void DrawChartLine(int selectedFrame, ChartData cdata, Rect r)
		{
			for (int i = 0; i < cdata.charts.Length; i++)
			{
				this.DrawChartItemLine(r, cdata, i);
			}
			if (cdata.maxValue > 0f)
			{
				this.DrawMaxValueScale(cdata, r);
			}
			this.DrawSelectedFrame(selectedFrame, cdata, r);
			this.DrawLabelsLine(selectedFrame, cdata, r);
		}

		private void DrawChartStacked(int selectedFrame, ChartData cdata, Rect r)
		{
			HandleUtility.ApplyWireMaterial();
			float[] sumbuf = new float[cdata.NumberOfFrames];
			for (int i = 0; i < cdata.charts.Length; i++)
			{
				if (cdata.hasOverlay)
				{
					this.DrawChartItemStackedOverlay(r, i, cdata, sumbuf);
				}
				this.DrawChartItemStacked(r, i, cdata, sumbuf);
			}
			this.DrawSelectedFrame(selectedFrame, cdata, r);
			this.DrawGridStacked(r, cdata);
			this.DrawLabelsStacked(selectedFrame, cdata, r);
			if (cdata.hasOverlay)
			{
				string text = ProfilerDriver.selectedPropertyPath;
				if (text.Length > 0)
				{
					int num = text.LastIndexOf('/');
					if (num != -1)
					{
						text = text.Substring(num + 1);
					}
					GUIContent content = EditorGUIUtility.TempContent("Selected: " + text);
					Vector2 vector = EditorStyles.whiteBoldLabel.CalcSize(content);
					EditorGUI.DropShadowLabel(new Rect(r.x + r.width - vector.x - 3f, r.y + 3f, vector.x, vector.y), content, Chart.ms_Styles.selectedLabel);
				}
			}
		}

		internal static void DoLabel(float x, float y, string text, float alignment)
		{
			if (!string.IsNullOrEmpty(text))
			{
				GUIContent content = new GUIContent(text);
				Vector2 vector = Chart.ms_Styles.whiteLabel.CalcSize(content);
				Rect position = new Rect(x + vector.x * alignment, y, vector.x, vector.y);
				EditorGUI.DoDropShadowLabel(position, content, Chart.ms_Styles.whiteLabel, 0.3f);
			}
		}

		private static void CorrectLabelPositions(float[] ypositions, float[] heights, float maxHeight)
		{
			int num = 5;
			for (int i = 0; i < num; i++)
			{
				bool flag = false;
				for (int j = 0; j < ypositions.Length; j++)
				{
					if (heights[j] > 0f)
					{
						float num2 = heights[j] / 2f;
						for (int k = j + 2; k < ypositions.Length; k += 2)
						{
							if (heights[k] > 0f)
							{
								float num3 = ypositions[j] - ypositions[k];
								float num4 = (heights[j] + heights[k]) / 2f;
								if (Mathf.Abs(num3) < num4)
								{
									num3 = (num4 - Mathf.Abs(num3)) / 2f * Mathf.Sign(num3);
									ypositions[j] += num3;
									ypositions[k] -= num3;
									flag = true;
								}
							}
						}
						if (ypositions[j] + num2 > maxHeight)
						{
							ypositions[j] = maxHeight - num2;
						}
						if (ypositions[j] - num2 < 0f)
						{
							ypositions[j] = num2;
						}
					}
				}
				if (!flag)
				{
					break;
				}
			}
		}

		private static float GetLabelHeight(string text)
		{
			GUIContent content = new GUIContent(text);
			return Chart.ms_Styles.whiteLabel.CalcSize(content).y;
		}

		private void DrawLabelsStacked(int selectedFrame, ChartData cdata, Rect r)
		{
			if (cdata.selectedLabels != null)
			{
				int numberOfFrames = cdata.NumberOfFrames;
				if (selectedFrame >= cdata.firstSelectableFrame && selectedFrame < cdata.firstFrame + numberOfFrames)
				{
					selectedFrame -= cdata.firstFrame;
					float num = r.width / (float)numberOfFrames;
					float num2 = r.x + num * (float)selectedFrame;
					float num3 = cdata.scale[0] * r.height;
					float[] array = new float[cdata.charts.Length];
					float[] array2 = new float[array.Length];
					float num4 = 0f;
					for (int i = 0; i < cdata.charts.Length; i++)
					{
						array[i] = -1f;
						array2[i] = 0f;
						int num5 = cdata.chartOrder[i];
						if (cdata.charts[num5].enabled)
						{
							float num6 = cdata.charts[num5].data[selectedFrame];
							if (num6 != -1f)
							{
								float num7 = (!cdata.hasOverlay) ? num6 : cdata.charts[num5].overlayData[selectedFrame];
								if (num7 * num3 > 5f)
								{
									array[i] = (num4 + num7 * 0.5f) * num3;
									array2[i] = Chart.GetLabelHeight(cdata.selectedLabels[num5]);
								}
								num4 += num6;
							}
						}
					}
					Chart.CorrectLabelPositions(array, array2, r.height);
					for (int j = 0; j < cdata.charts.Length; j++)
					{
						if (array2[j] > 0f)
						{
							int num8 = cdata.chartOrder[j];
							Color color = cdata.charts[num8].color;
							GUI.contentColor = color * 0.8f + Color.white * 0.2f;
							float alignment = (float)(((num8 & 1) != 0) ? 0 : -1);
							float num9 = ((num8 & 1) != 0) ? (num + 1f) : -1f;
							Chart.DoLabel(num2 + num9, r.y + r.height - array[j] - 8f, cdata.selectedLabels[num8], alignment);
						}
					}
					GUI.contentColor = Color.white;
				}
			}
		}

		private void DrawGridStacked(Rect r, ChartData cdata)
		{
			if (Event.current.type == EventType.Repaint && cdata.grid != null && cdata.gridLabels != null)
			{
				GL.Begin(1);
				GL.Color(new Color(1f, 1f, 1f, 0.2f));
				for (int i = 0; i < cdata.grid.Length; i++)
				{
					float num = r.y + r.height - cdata.grid[i] * cdata.scale[0] * r.height;
					if (num > r.y)
					{
						GL.Vertex3(r.x + 80f, num, 0f);
						GL.Vertex3(r.x + r.width, num, 0f);
					}
				}
				GL.End();
				for (int j = 0; j < cdata.grid.Length; j++)
				{
					float num2 = r.y + r.height - cdata.grid[j] * cdata.scale[0] * r.height;
					if (num2 > r.y)
					{
						Chart.DoLabel(r.x + 5f, num2 - 8f, cdata.gridLabels[j], 0f);
					}
				}
			}
		}

		private void DrawLabelsLine(int selectedFrame, ChartData cdata, Rect r)
		{
			if (cdata.selectedLabels != null)
			{
				int numberOfFrames = cdata.NumberOfFrames;
				if (selectedFrame >= cdata.firstSelectableFrame && selectedFrame < cdata.firstFrame + numberOfFrames)
				{
					selectedFrame -= cdata.firstFrame;
					float[] array = new float[cdata.charts.Length];
					float[] array2 = new float[array.Length];
					for (int i = 0; i < cdata.charts.Length; i++)
					{
						array[i] = -1f;
						array2[i] = 0f;
						float num = cdata.charts[i].data[selectedFrame];
						if (num != -1f)
						{
							array[i] = num * cdata.scale[i] * r.height;
							array2[i] = Chart.GetLabelHeight(cdata.selectedLabels[i]);
						}
					}
					Chart.CorrectLabelPositions(array, array2, r.height);
					float num2 = r.width / (float)numberOfFrames;
					float num3 = r.x + num2 * (float)selectedFrame;
					for (int j = 0; j < cdata.charts.Length; j++)
					{
						if (array2[j] > 0f)
						{
							Color color = cdata.charts[j].color;
							GUI.contentColor = (color + Color.white) * 0.5f;
							float alignment = (float)(((j & 1) != 0) ? 0 : -1);
							float num4 = ((j & 1) != 0) ? (num2 + 1f) : -1f;
							Chart.DoLabel(num3 + num4, r.y + r.height - array[j] - 8f, cdata.selectedLabels[j], alignment);
						}
					}
					GUI.contentColor = Color.white;
				}
			}
		}

		private void DrawChartItemLine(Rect r, ChartData cdata, int index)
		{
			if (cdata.charts[index].enabled)
			{
				Color color = cdata.charts[index].color;
				int numberOfFrames = cdata.NumberOfFrames;
				int num = -cdata.firstFrame;
				num = Mathf.Clamp(num, 0, numberOfFrames);
				int num2 = numberOfFrames - num;
				if (num2 > 0)
				{
					if (this.m_CachedLineData == null || numberOfFrames > this.m_CachedLineData.Length)
					{
						this.m_CachedLineData = new Vector3[numberOfFrames];
					}
					float num3 = r.width / (float)numberOfFrames;
					float num4 = r.x + num3 * 0.5f + (float)num * num3;
					float height = r.height;
					float y = r.y;
					int i = num;
					while (i < numberOfFrames)
					{
						float num5 = y + height;
						if (cdata.charts[index].data[i] != -1f)
						{
							float num6 = cdata.charts[index].data[i] * cdata.scale[index] * height;
							num5 -= num6;
						}
						this.m_CachedLineData[i - num].Set(num4, num5, 0f);
						i++;
						num4 += num3;
					}
					Handles.color = color;
					Handles.DrawAAPolyLine(2f, num2, this.m_CachedLineData);
				}
			}
		}

		private void DrawChartItemStacked(Rect r, int index, ChartData cdata, float[] sumbuf)
		{
			if (Event.current.type == EventType.Repaint)
			{
				int numberOfFrames = cdata.NumberOfFrames;
				float num = r.width / (float)numberOfFrames;
				index = cdata.chartOrder[index];
				if (cdata.charts[index].enabled)
				{
					Color color = cdata.charts[index].color;
					if (cdata.hasOverlay)
					{
						color.r *= 0.9f;
						color.g *= 0.9f;
						color.b *= 0.9f;
						color.a *= 0.4f;
					}
					Color c = color;
					c.r *= 0.8f;
					c.g *= 0.8f;
					c.b *= 0.8f;
					c.a *= 0.8f;
					GL.Begin(5);
					float num2 = r.x + num * 0.5f;
					float height = r.height;
					float y = r.y;
					int i = 0;
					while (i < numberOfFrames)
					{
						float num3 = y + height - sumbuf[i];
						float num4 = cdata.charts[index].data[i];
						if (num4 != -1f)
						{
							float num5 = num4 * cdata.scale[0] * height;
							if (num3 - num5 < r.yMin)
							{
								num5 = num3 - r.yMin;
							}
							GL.Color(color);
							GL.Vertex3(num2, num3 - num5, 0f);
							GL.Color(c);
							GL.Vertex3(num2, num3, 0f);
							sumbuf[i] += num5;
						}
						i++;
						num2 += num;
					}
					GL.End();
				}
			}
		}

		private void DrawChartItemStackedOverlay(Rect r, int index, ChartData cdata, float[] sumbuf)
		{
			if (Event.current.type == EventType.Repaint)
			{
				int numberOfFrames = cdata.NumberOfFrames;
				float num = r.width / (float)numberOfFrames;
				index = cdata.chartOrder[index];
				if (cdata.charts[index].enabled)
				{
					Color color = cdata.charts[index].color;
					Color c = color;
					c.r *= 0.8f;
					c.g *= 0.8f;
					c.b *= 0.8f;
					c.a *= 0.8f;
					GL.Begin(5);
					float num2 = r.x + num * 0.5f;
					float height = r.height;
					float y = r.y;
					int i = 0;
					while (i < numberOfFrames)
					{
						float num3 = y + height - sumbuf[i];
						float num4 = cdata.charts[index].overlayData[i];
						if (num4 != -1f)
						{
							float num5 = num4 * cdata.scale[0] * height;
							GL.Color(color);
							GL.Vertex3(num2, num3 - num5, 0f);
							GL.Color(c);
							GL.Vertex3(num2, num3, 0f);
						}
						i++;
						num2 += num;
					}
					GL.End();
				}
			}
		}

		protected virtual void DrawLabelDragger(Chart.ChartType type, Rect r, ChartData cdata)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			if (type == Chart.ChartType.StackedFill)
			{
				int num = 0;
				int i = cdata.charts.Length - 1;
				while (i >= 0)
				{
					Rect position = (this.m_DragItemIndex != i) ? new Rect(r.x, r.y + 20f + (float)(num * 11), 170f, 11f) : new Rect(r.x, mousePosition.y - this.m_DragDownPos.y, 170f, 11f);
					if (cdata.charts[cdata.chartOrder[i]].enabled)
					{
						GUI.backgroundColor = cdata.charts[cdata.chartOrder[i]].color;
					}
					else
					{
						GUI.backgroundColor = Color.black;
					}
					GUI.Label(position, cdata.charts[cdata.chartOrder[i]].name, Chart.ms_Styles.paneSubLabel);
					i--;
					num++;
				}
			}
			else
			{
				for (int j = 0; j < cdata.charts.Length; j++)
				{
					GUI.backgroundColor = cdata.charts[j].color;
					string name = cdata.charts[j].name;
					Chart.DrawSubLabel(r, j, name);
				}
			}
			GUI.backgroundColor = Color.white;
		}

		protected static void DrawSubLabel(Rect r, int i, string name)
		{
			Rect position = new Rect(r.x, r.y + 20f + (float)(i * 11), 170f, 11f);
			GUI.Label(position, name, Chart.ms_Styles.paneSubLabel);
		}

		protected internal virtual void LabelDraggerDrag(int chartControlID, Chart.ChartType chartType, ChartData cdata, Rect r, bool active)
		{
			if (chartType != Chart.ChartType.Line && active)
			{
				Event current = Event.current;
				EventType typeForControl = current.GetTypeForControl(chartControlID);
				if (typeForControl == EventType.MouseDown || typeForControl == EventType.MouseUp || typeForControl == EventType.KeyDown || typeForControl == EventType.MouseDrag)
				{
					if (typeForControl == EventType.KeyDown && current.keyCode == KeyCode.Escape && this.m_DragItemIndex != -1)
					{
						GUIUtility.hotControl = 0;
						Array.Copy(this.m_ChartOrderBackup, cdata.chartOrder, this.m_ChartOrderBackup.Length);
						this.m_DragItemIndex = -1;
						current.Use();
					}
					int num = 0;
					int i = cdata.charts.Length - 1;
					while (i >= 0)
					{
						if ((current.type == EventType.MouseUp && this.m_MouseDownIndex != -1) || current.type == EventType.MouseDown)
						{
							if (Chart.GetToggleRect(r, num).Contains(current.mousePosition))
							{
								this.m_DragItemIndex = -1;
								if (current.type == EventType.MouseUp && this.m_MouseDownIndex == i)
								{
									this.m_MouseDownIndex = -1;
									cdata.charts[cdata.chartOrder[i]].enabled = !cdata.charts[cdata.chartOrder[i]].enabled;
									if (chartType == Chart.ChartType.StackedFill)
									{
										this.SaveChartsSettingsEnabled(cdata);
									}
								}
								else
								{
									this.m_MouseDownIndex = i;
								}
								current.Use();
							}
						}
						if (current.type == EventType.MouseDown)
						{
							Rect rect = new Rect(r.x, r.y + 20f + (float)(num * 11), 170f, 11f);
							if (rect.Contains(current.mousePosition))
							{
								this.m_MouseDownIndex = -1;
								this.m_DragItemIndex = i;
								this.m_DragDownPos = current.mousePosition;
								this.m_DragDownPos.x = this.m_DragDownPos.x - rect.x;
								this.m_DragDownPos.y = this.m_DragDownPos.y - rect.y;
								this.m_ChartOrderBackup = new int[cdata.chartOrder.Length];
								Array.Copy(cdata.chartOrder, this.m_ChartOrderBackup, this.m_ChartOrderBackup.Length);
								GUIUtility.hotControl = chartControlID;
								Event.current.Use();
							}
						}
						else if (this.m_DragItemIndex != -1 && typeForControl == EventType.MouseDrag && i != this.m_DragItemIndex)
						{
							float y = current.mousePosition.y;
							float num2 = r.y + 20f + (float)(num * 11);
							if (y >= num2 && y < num2 + 11f)
							{
								int num3 = cdata.chartOrder[i];
								cdata.chartOrder[i] = cdata.chartOrder[this.m_DragItemIndex];
								cdata.chartOrder[this.m_DragItemIndex] = num3;
								this.m_DragItemIndex = i;
								this.SaveChartsSettingsOrder(cdata);
							}
						}
						i--;
						num++;
					}
					if (typeForControl == EventType.MouseDrag && this.m_DragItemIndex != -1)
					{
						current.Use();
					}
					if (typeForControl == EventType.MouseUp && GUIUtility.hotControl == chartControlID)
					{
						GUIUtility.hotControl = 0;
						this.m_DragItemIndex = -1;
						current.Use();
					}
				}
			}
		}

		protected static Rect GetToggleRect(Rect r, int idx)
		{
			return new Rect(r.x + 10f + 40f, r.y + 20f + (float)(idx * 11), 9f, 9f);
		}

		private void LoadChartsSettings(ChartData cdata)
		{
			if (!string.IsNullOrEmpty(this.m_ChartSettingsName))
			{
				string @string = EditorPrefs.GetString(this.m_ChartSettingsName + "Order");
				if (!string.IsNullOrEmpty(@string))
				{
					try
					{
						string[] array = @string.Split(new char[]
						{
							','
						});
						if (array.Length == cdata.charts.Length)
						{
							for (int i = 0; i < cdata.charts.Length; i++)
							{
								cdata.chartOrder[i] = int.Parse(array[i]);
							}
						}
					}
					catch (FormatException)
					{
					}
				}
				@string = EditorPrefs.GetString(this.m_ChartSettingsName + "Visible");
				for (int j = 0; j < cdata.charts.Length; j++)
				{
					if (j < @string.Length && @string[j] == '0')
					{
						cdata.charts[j].enabled = false;
					}
				}
			}
		}

		private void SaveChartsSettingsOrder(ChartData cdata)
		{
			if (!string.IsNullOrEmpty(this.m_ChartSettingsName))
			{
				string text = string.Empty;
				for (int i = 0; i < cdata.charts.Length; i++)
				{
					if (text.Length != 0)
					{
						text += ",";
					}
					text += cdata.chartOrder[i];
				}
				EditorPrefs.SetString(this.m_ChartSettingsName + "Order", text);
			}
		}

		private void SaveChartsSettingsEnabled(ChartData cdata)
		{
			string text = string.Empty;
			for (int i = 0; i < cdata.charts.Length; i++)
			{
				text += ((!cdata.charts[i].enabled) ? '0' : '1');
			}
			EditorPrefs.SetString(this.m_ChartSettingsName + "Visible", text);
		}
	}
}
