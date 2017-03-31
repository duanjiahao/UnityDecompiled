using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchyGUI : TreeViewGUI
	{
		private readonly GUIContent k_AnimatePropertyLabel = new GUIContent("Add Property");

		private GUIStyle m_AnimationRowEvenStyle;

		private GUIStyle m_AnimationRowOddStyle;

		private GUIStyle m_AnimationSelectionTextField;

		private GUIStyle m_AnimationLineStyle;

		private GUIStyle m_AnimationCurveDropdown;

		private AnimationWindowHierarchyNode m_RenamedNode;

		private Color m_LightSkinPropertyTextColor = new Color(0.35f, 0.35f, 0.35f);

		private Color m_PhantomCurveColor = new Color(0f, 0.6f, 0.6f);

		private int[] m_HierarchyItemFoldControlIDs;

		private int[] m_HierarchyItemValueControlIDs;

		private int[] m_HierarchyItemButtonControlIDs;

		private const float k_RowRightOffset = 10f;

		private const float k_ValueFieldDragWidth = 15f;

		private const float k_ValueFieldWidth = 50f;

		private const float k_ValueFieldOffsetFromRightSide = 75f;

		private const float k_ColorIndicatorTopMargin = 3f;

		public const float k_DopeSheetRowHeight = 16f;

		public const float k_DopeSheetRowHeightTall = 32f;

		public const float k_AddCurveButtonNodeHeight = 40f;

		public const float k_RowBackgroundColorBrightness = 0.28f;

		private const float k_SelectedPhantomCurveColorMultiplier = 1.4f;

		private static readonly Color k_KeyColorInDopesheetMode = new Color(0.7f, 0.7f, 0.7f, 1f);

		private static readonly Color k_KeyColorForNonCurves = new Color(0.7f, 0.7f, 0.7f, 0.5f);

		private static readonly Color k_LeftoverCurveColor = Color.yellow;

		internal static int s_WasInsideValueRectFrame = -1;

		public AnimationWindowState state
		{
			get;
			set;
		}

		public AnimationWindowHierarchyGUI(TreeViewController treeView, AnimationWindowState state) : base(treeView)
		{
			this.state = state;
			this.InitStyles();
		}

		protected void InitStyles()
		{
			if (this.m_AnimationRowEvenStyle == null)
			{
				this.m_AnimationRowEvenStyle = new GUIStyle("AnimationRowEven");
			}
			if (this.m_AnimationRowOddStyle == null)
			{
				this.m_AnimationRowOddStyle = new GUIStyle("AnimationRowOdd");
			}
			if (this.m_AnimationSelectionTextField == null)
			{
				this.m_AnimationSelectionTextField = new GUIStyle("AnimationSelectionTextField");
			}
			if (this.m_AnimationLineStyle == null)
			{
				this.m_AnimationLineStyle = new GUIStyle(TreeViewGUI.Styles.lineStyle);
				this.m_AnimationLineStyle.padding.left = 0;
			}
			if (this.m_AnimationCurveDropdown == null)
			{
				this.m_AnimationCurveDropdown = new GUIStyle("AnimPropDropdown");
			}
		}

		protected void DoNodeGUI(Rect rect, AnimationWindowHierarchyNode node, bool selected, bool focused, int row)
		{
			this.InitStyles();
			if (!(node is AnimationWindowHierarchyMasterNode))
			{
				float indent = this.k_BaseIndent + (float)(node.depth + node.indent) * this.k_IndentWidth;
				if (node is AnimationWindowHierarchyAddButtonNode)
				{
					if (Event.current.type == EventType.MouseMove && AnimationWindowHierarchyGUI.s_WasInsideValueRectFrame >= 0)
					{
						if (AnimationWindowHierarchyGUI.s_WasInsideValueRectFrame >= Time.frameCount - 1)
						{
							Event.current.Use();
						}
						else
						{
							AnimationWindowHierarchyGUI.s_WasInsideValueRectFrame = -1;
						}
					}
					using (new EditorGUI.DisabledScope(!this.state.selection.canAddCurves))
					{
						this.DoAddCurveButton(rect, node, row);
					}
				}
				else
				{
					this.DoRowBackground(rect, row);
					this.DoIconAndName(rect, node, selected, focused, indent);
					this.DoFoldout(node, rect, indent, row);
					bool flag = false;
					if (node.curves != null)
					{
						flag = !Array.Exists<AnimationWindowCurve>(node.curves, (AnimationWindowCurve curve) => !curve.animationIsEditable);
					}
					using (new EditorGUI.DisabledScope(!flag))
					{
						this.DoValueField(rect, node, row);
					}
					this.DoCurveDropdown(rect, node, row, flag);
					this.HandleContextMenu(rect, node, flag);
					this.DoCurveColorIndicator(rect, node);
				}
				EditorGUIUtility.SetIconSize(Vector2.zero);
			}
		}

		public override void BeginRowGUI()
		{
			base.BeginRowGUI();
			this.HandleDelete();
			int rowCount = this.m_TreeView.data.rowCount;
			this.m_HierarchyItemFoldControlIDs = new int[rowCount];
			this.m_HierarchyItemValueControlIDs = new int[rowCount];
			this.m_HierarchyItemButtonControlIDs = new int[rowCount];
			for (int i = 0; i < rowCount; i++)
			{
				this.m_HierarchyItemFoldControlIDs[i] = GUIUtility.GetControlID(FocusType.Passive);
				this.m_HierarchyItemValueControlIDs[i] = GUIUtility.GetControlID(FocusType.Passive);
				this.m_HierarchyItemButtonControlIDs[i] = GUIUtility.GetControlID(FocusType.Passive);
			}
		}

		private void DoAddCurveButton(Rect rect, AnimationWindowHierarchyNode node, int row)
		{
			float num = (rect.width - 230f) / 2f;
			float num2 = 10f;
			Rect rect2 = new Rect(rect.xMin + num, rect.yMin + num2, rect.width - num * 2f, rect.height - num2 * 2f);
			if (this.DoTreeViewButton(this.m_HierarchyItemButtonControlIDs[row], rect2, this.k_AnimatePropertyLabel, GUI.skin.button))
			{
				AddCurvesPopup.selection = this.state.selection;
				AddCurvesPopupHierarchyDataSource.showEntireHierarchy = true;
				if (AddCurvesPopup.ShowAtPosition(rect2, this.state, new AddCurvesPopup.OnNewCurveAdded(this.OnNewCurveAdded)))
				{
					GUIUtility.ExitGUI();
				}
			}
		}

		private void OnNewCurveAdded(AddCurvesPopupPropertyNode node)
		{
		}

		private void DoRowBackground(Rect rect, int row)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (row % 2 == 0)
				{
					this.m_AnimationRowEvenStyle.Draw(rect, false, false, false, false);
				}
				else
				{
					this.m_AnimationRowOddStyle.Draw(rect, false, false, false, false);
				}
			}
		}

		private void DoFoldout(AnimationWindowHierarchyNode node, Rect rect, float indent, int row)
		{
			if (this.m_TreeView.data.IsExpandable(node))
			{
				Rect position = rect;
				position.x = indent;
				position.width = TreeViewGUI.Styles.foldoutWidth;
				EditorGUI.BeginChangeCheck();
				bool expand = GUI.Toggle(position, this.m_HierarchyItemFoldControlIDs[row], this.m_TreeView.data.IsExpanded(node), GUIContent.none, TreeViewGUI.Styles.foldout);
				if (EditorGUI.EndChangeCheck())
				{
					if (Event.current.alt)
					{
						this.m_TreeView.data.SetExpandedWithChildren(node, expand);
					}
					else
					{
						this.m_TreeView.data.SetExpanded(node, expand);
					}
				}
			}
			else
			{
				AnimationWindowHierarchyPropertyNode animationWindowHierarchyPropertyNode = node as AnimationWindowHierarchyPropertyNode;
				AnimationWindowHierarchyState animationWindowHierarchyState = this.m_TreeView.state as AnimationWindowHierarchyState;
				if (animationWindowHierarchyPropertyNode != null && animationWindowHierarchyPropertyNode.isPptrNode)
				{
					Rect position2 = rect;
					position2.x = indent;
					position2.width = TreeViewGUI.Styles.foldoutWidth;
					EditorGUI.BeginChangeCheck();
					bool flag = animationWindowHierarchyState.GetTallMode(animationWindowHierarchyPropertyNode);
					flag = GUI.Toggle(position2, this.m_HierarchyItemFoldControlIDs[row], flag, GUIContent.none, TreeViewGUI.Styles.foldout);
					if (EditorGUI.EndChangeCheck())
					{
						animationWindowHierarchyState.SetTallMode(animationWindowHierarchyPropertyNode, flag);
					}
				}
			}
		}

		private void DoIconAndName(Rect rect, AnimationWindowHierarchyNode node, bool selected, bool focused, float indent)
		{
			EditorGUIUtility.SetIconSize(new Vector2(13f, 13f));
			if (Event.current.type == EventType.Repaint)
			{
				if (selected)
				{
					TreeViewGUI.Styles.selectionStyle.Draw(rect, false, false, true, focused);
				}
				if (AnimationMode.InAnimationMode())
				{
					rect.width -= 77f;
				}
				bool flag = AnimationWindowUtility.IsNodeLeftOverCurve(node);
				bool flag2 = AnimationWindowUtility.IsNodeAmbiguous(node);
				bool flag3 = AnimationWindowUtility.IsNodePhantom(node);
				string text = "";
				string tooltip = "";
				if (flag3)
				{
					text = " (Default Value)";
					tooltip = "Transform position, rotation and scale can't be partially animated. This value will be animated to the default value";
				}
				if (flag)
				{
					text = " (Missing!)";
					tooltip = "The GameObject or Component is missing (" + node.path + ")";
				}
				if (flag2)
				{
					text = " (Duplicate GameObject name!)";
					tooltip = "Target for curve is ambiguous since there are multiple GameObjects with same name (" + node.path + ")";
				}
				Color textColor = this.m_AnimationLineStyle.normal.textColor;
				Color color;
				if (node.depth == 0)
				{
					string str = "";
					if (node.curves.Length > 0)
					{
						AnimationWindowSelectionItem selectionBinding = node.curves[0].selectionBinding;
						if (selectionBinding != null)
						{
							if (selectionBinding.rootGameObject != null)
							{
								Transform x = selectionBinding.rootGameObject.transform.Find(node.path);
								if (x == null)
								{
									flag = true;
								}
							}
						}
						string gameObjectName = this.GetGameObjectName((!(selectionBinding != null)) ? null : selectionBinding.rootGameObject, node.path);
						str = ((!string.IsNullOrEmpty(gameObjectName)) ? (gameObjectName + " : ") : "");
					}
					TreeViewGUI.Styles.content = new GUIContent(str + node.displayName + text, this.GetIconForItem(node), tooltip);
					color = ((!EditorGUIUtility.isProSkin) ? Color.black : (Color.gray * 1.35f));
				}
				else
				{
					TreeViewGUI.Styles.content = new GUIContent(node.displayName + text, this.GetIconForItem(node), tooltip);
					color = ((!EditorGUIUtility.isProSkin) ? this.m_LightSkinPropertyTextColor : Color.gray);
					Color color2 = (!selected) ? this.m_PhantomCurveColor : (this.m_PhantomCurveColor * 1.4f);
					color = ((!flag3) ? color : color2);
				}
				color = ((!flag && !flag2) ? color : AnimationWindowHierarchyGUI.k_LeftoverCurveColor);
				this.SetStyleTextColor(this.m_AnimationLineStyle, color);
				rect.xMin += (float)((int)(indent + TreeViewGUI.Styles.foldoutWidth + (float)this.m_AnimationLineStyle.margin.left));
				GUI.Label(rect, TreeViewGUI.Styles.content, this.m_AnimationLineStyle);
				this.SetStyleTextColor(this.m_AnimationLineStyle, textColor);
			}
			if (this.IsRenaming(node.id) && Event.current.type != EventType.Layout)
			{
				base.GetRenameOverlay().editFieldRect = new Rect(rect.x + this.k_IndentWidth, rect.y, rect.width - this.k_IndentWidth - 1f, rect.height);
			}
		}

		private string GetGameObjectName(GameObject rootGameObject, string path)
		{
			string result;
			if (string.IsNullOrEmpty(path))
			{
				result = ((!(rootGameObject != null)) ? "" : rootGameObject.name);
			}
			else
			{
				string[] array = path.Split(new char[]
				{
					'/'
				});
				result = array[array.Length - 1];
			}
			return result;
		}

		private string GetPathWithoutChildmostGameObject(string path)
		{
			string result;
			if (string.IsNullOrEmpty(path))
			{
				result = "";
			}
			else
			{
				int num = path.LastIndexOf('/');
				result = path.Substring(0, num + 1);
			}
			return result;
		}

		private void DoValueField(Rect rect, AnimationWindowHierarchyNode node, int row)
		{
			bool flag = false;
			if (node is AnimationWindowHierarchyPropertyNode)
			{
				AnimationWindowCurve[] curves = node.curves;
				if (curves == null || curves.Length == 0)
				{
					return;
				}
				AnimationWindowCurve animationWindowCurve = curves[0];
				object currentValue = CurveBindingUtility.GetCurrentValue(this.state, animationWindowCurve);
				if (currentValue is float)
				{
					float num = (float)currentValue;
					Rect dragHotZone = new Rect(rect.xMax - 75f - 15f, rect.y, 15f, rect.height);
					Rect position = new Rect(rect.xMax - 75f, rect.y, 50f, rect.height);
					if (Event.current.type == EventType.MouseMove && position.Contains(Event.current.mousePosition))
					{
						AnimationWindowHierarchyGUI.s_WasInsideValueRectFrame = Time.frameCount;
					}
					EditorGUI.BeginChangeCheck();
					if (animationWindowCurve.valueType == typeof(bool))
					{
						num = (float)((!GUI.Toggle(position, this.m_HierarchyItemValueControlIDs[row], num != 0f, GUIContent.none, EditorStyles.toggle)) ? 0 : 1);
					}
					else
					{
						int num2 = this.m_HierarchyItemValueControlIDs[row];
						bool flag2 = GUIUtility.keyboardControl == num2 && EditorGUIUtility.editingTextField && Event.current.type == EventType.KeyDown && (Event.current.character == '\n' || Event.current.character == '\u0003');
						if (EditorGUI.s_RecycledEditor.controlID == num2 && Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
						{
							GUIUtility.keyboardControl = num2;
						}
						num = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position, dragHotZone, num2, num, EditorGUI.kFloatFieldFormatString, this.m_AnimationSelectionTextField, true);
						if (flag2)
						{
							GUI.changed = true;
							Event.current.Use();
						}
					}
					if (float.IsInfinity(num) || float.IsNaN(num))
					{
						num = 0f;
					}
					if (EditorGUI.EndChangeCheck())
					{
						string undoLabel = "Edit Key";
						float num3 = this.state.currentTime - animationWindowCurve.timeOffset;
						AnimationKeyTime time = AnimationKeyTime.Time(num3, animationWindowCurve.clip.frameRate);
						AnimationWindowKeyframe animationWindowKeyframe = null;
						foreach (AnimationWindowKeyframe current in animationWindowCurve.m_Keyframes)
						{
							if (Mathf.Approximately(current.time, num3))
							{
								animationWindowKeyframe = current;
							}
						}
						if (animationWindowKeyframe == null)
						{
							AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, num, animationWindowCurve.valueType, time);
						}
						else
						{
							animationWindowKeyframe.value = num;
						}
						this.state.SaveCurve(animationWindowCurve, undoLabel);
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.state.StartRecording();
			}
		}

		private bool DoTreeViewButton(int id, Rect position, GUIContent content, GUIStyle style)
		{
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(id);
			bool result;
			if (typeForControl != EventType.Repaint)
			{
				if (typeForControl != EventType.MouseDown)
				{
					if (typeForControl == EventType.MouseUp)
					{
						if (GUIUtility.hotControl == id)
						{
							GUIUtility.hotControl = 0;
							current.Use();
							if (position.Contains(current.mousePosition))
							{
								result = true;
								return result;
							}
						}
					}
				}
				else if (position.Contains(current.mousePosition) && current.button == 0)
				{
					GUIUtility.hotControl = id;
					current.Use();
				}
			}
			else
			{
				style.Draw(position, content, id, false);
			}
			result = false;
			return result;
		}

		private void DoCurveDropdown(Rect rect, AnimationWindowHierarchyNode node, int row, bool enabled)
		{
			rect = new Rect(rect.xMax - 10f - 12f, rect.yMin + 2f, 22f, 12f);
			if (this.DoTreeViewButton(this.m_HierarchyItemButtonControlIDs[row], rect, GUIContent.none, this.m_AnimationCurveDropdown))
			{
				this.state.SelectHierarchyItem(node.id, false, false);
				GenericMenu genericMenu = this.GenerateMenu(new AnimationWindowHierarchyNode[]
				{
					node
				}.ToList<AnimationWindowHierarchyNode>(), enabled);
				genericMenu.DropDown(rect);
				Event.current.Use();
			}
		}

		private void DoCurveColorIndicator(Rect rect, AnimationWindowHierarchyNode node)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Color color = GUI.color;
				if (!this.state.showCurveEditor)
				{
					GUI.color = AnimationWindowHierarchyGUI.k_KeyColorInDopesheetMode;
				}
				else if (node.curves.Length == 1 && !node.curves[0].isPPtrCurve)
				{
					GUI.color = CurveUtility.GetPropertyColor(node.curves[0].binding.propertyName);
				}
				else
				{
					GUI.color = AnimationWindowHierarchyGUI.k_KeyColorForNonCurves;
				}
				bool flag = false;
				if (AnimationMode.InAnimationMode())
				{
					AnimationWindowCurve[] curves = node.curves;
					for (int i = 0; i < curves.Length; i++)
					{
						AnimationWindowCurve animationWindowCurve = curves[i];
						if (animationWindowCurve.m_Keyframes.Any((AnimationWindowKeyframe key) => this.state.time.ContainsTime(key.time)))
						{
							flag = true;
						}
					}
				}
				Texture texture = (!flag) ? CurveUtility.GetIconCurve() : CurveUtility.GetIconKey();
				rect = new Rect(rect.xMax - 10f - (float)(texture.width / 2) - 5f, rect.yMin + 3f, (float)texture.width, (float)texture.height);
				GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true, 1f);
				GUI.color = color;
			}
		}

		private void HandleDelete()
		{
			if (this.m_TreeView.HasFocus())
			{
				EventType type = Event.current.type;
				if (type != EventType.ExecuteCommand)
				{
					if (type == EventType.KeyDown)
					{
						if (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete)
						{
							this.RemoveCurvesFromSelectedNodes();
							Event.current.Use();
						}
					}
				}
				else if (Event.current.commandName == "SoftDelete" || Event.current.commandName == "Delete")
				{
					if (Event.current.type == EventType.ExecuteCommand)
					{
						this.RemoveCurvesFromSelectedNodes();
					}
					Event.current.Use();
				}
			}
		}

		private void HandleContextMenu(Rect rect, AnimationWindowHierarchyNode node, bool enabled)
		{
			if (Event.current.type == EventType.ContextClick)
			{
				if (rect.Contains(Event.current.mousePosition))
				{
					this.state.SelectHierarchyItem(node.id, true, true);
					this.GenerateMenu(this.state.selectedHierarchyNodes, enabled).ShowAsContext();
					Event.current.Use();
				}
			}
		}

		private GenericMenu GenerateMenu(List<AnimationWindowHierarchyNode> interactedNodes, bool enabled)
		{
			List<AnimationWindowCurve> curvesAffectedByNodes = this.GetCurvesAffectedByNodes(interactedNodes, false);
			List<AnimationWindowCurve> curvesAffectedByNodes2 = this.GetCurvesAffectedByNodes(interactedNodes, true);
			bool flag = curvesAffectedByNodes.Count == 1 && AnimationWindowUtility.ForceGrouping(curvesAffectedByNodes[0].binding);
			GenericMenu genericMenu = new GenericMenu();
			GUIContent content = new GUIContent((curvesAffectedByNodes.Count <= 1 && !flag) ? "Remove Property" : "Remove Properties");
			if (!enabled)
			{
				genericMenu.AddDisabledItem(content);
			}
			else
			{
				genericMenu.AddItem(content, false, new GenericMenu.MenuFunction(this.RemoveCurvesFromSelectedNodes));
			}
			bool flag2 = true;
			EditorCurveBinding[] array = new EditorCurveBinding[curvesAffectedByNodes2.Count];
			for (int i = 0; i < curvesAffectedByNodes2.Count; i++)
			{
				array[i] = curvesAffectedByNodes2[i].binding;
			}
			RotationCurveInterpolation.Mode rotationInterpolationMode = this.GetRotationInterpolationMode(array);
			if (rotationInterpolationMode == RotationCurveInterpolation.Mode.Undefined)
			{
				flag2 = false;
			}
			else
			{
				foreach (AnimationWindowHierarchyNode current in interactedNodes)
				{
					if (!(current is AnimationWindowHierarchyPropertyGroupNode))
					{
						flag2 = false;
					}
				}
			}
			if (flag2)
			{
				string str = (!this.state.activeAnimationClip.legacy) ? "" : " (Not fully supported in Legacy)";
				GenericMenu.MenuFunction2 menuFunction = null;
				genericMenu.AddItem(new GUIContent("Interpolation/Euler Angles" + str), rotationInterpolationMode == RotationCurveInterpolation.Mode.RawEuler, (!enabled) ? menuFunction : new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.RawEuler);
				genericMenu.AddItem(new GUIContent("Interpolation/Euler Angles (Quaternion)"), rotationInterpolationMode == RotationCurveInterpolation.Mode.Baked, (!enabled) ? menuFunction : new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.Baked);
				genericMenu.AddItem(new GUIContent("Interpolation/Quaternion"), rotationInterpolationMode == RotationCurveInterpolation.Mode.NonBaked, (!enabled) ? menuFunction : new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.NonBaked);
			}
			if (AnimationMode.InAnimationMode())
			{
				genericMenu.AddSeparator("");
				bool flag3 = true;
				bool flag4 = true;
				foreach (AnimationWindowCurve current2 in curvesAffectedByNodes)
				{
					if (!current2.HasKeyframe(this.state.time))
					{
						flag3 = false;
					}
					else
					{
						flag4 = false;
					}
				}
				string text = "Add Key";
				if (flag3 || !enabled)
				{
					genericMenu.AddDisabledItem(new GUIContent(text));
				}
				else
				{
					genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.AddKeysAtCurrentTime), curvesAffectedByNodes);
				}
				text = "Delete Key";
				if (flag4 || !enabled)
				{
					genericMenu.AddDisabledItem(new GUIContent(text));
				}
				else
				{
					genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.DeleteKeysAtCurrentTime), curvesAffectedByNodes);
				}
			}
			return genericMenu;
		}

		private void AddKeysAtCurrentTime(object obj)
		{
			this.AddKeysAtCurrentTime((List<AnimationWindowCurve>)obj);
		}

		private void AddKeysAtCurrentTime(List<AnimationWindowCurve> curves)
		{
			AnimationWindowUtility.AddKeyframes(this.state, curves.ToArray(), this.state.time);
		}

		private void DeleteKeysAtCurrentTime(object obj)
		{
			this.DeleteKeysAtCurrentTime((List<AnimationWindowCurve>)obj);
		}

		private void DeleteKeysAtCurrentTime(List<AnimationWindowCurve> curves)
		{
			AnimationWindowUtility.RemoveKeyframes(this.state, curves.ToArray(), this.state.time);
		}

		private void ChangeRotationInterpolation(object interpolationMode)
		{
			RotationCurveInterpolation.Mode mode = (RotationCurveInterpolation.Mode)interpolationMode;
			AnimationWindowCurve[] array = this.state.activeCurves.ToArray();
			EditorCurveBinding[] array2 = new EditorCurveBinding[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].binding;
			}
			RotationCurveInterpolation.SetInterpolation(this.state.activeAnimationClip, array2, mode);
			this.MaintainTreeviewStateAfterRotationInterpolation(mode);
			this.state.hierarchyData.ReloadData();
		}

		private void RemoveCurvesFromSelectedNodes()
		{
			this.RemoveCurvesFromNodes(this.state.selectedHierarchyNodes);
		}

		private void RemoveCurvesFromNodes(List<AnimationWindowHierarchyNode> nodes)
		{
			string undoLabel = "Remove Curve";
			this.state.SaveKeySelection(undoLabel);
			foreach (AnimationWindowHierarchyNode current in nodes)
			{
				AnimationWindowHierarchyNode animationWindowHierarchyNode = current;
				if (animationWindowHierarchyNode.parent is AnimationWindowHierarchyPropertyGroupNode)
				{
					EditorCurveBinding? binding = animationWindowHierarchyNode.binding;
					if (binding.HasValue)
					{
						EditorCurveBinding? binding2 = animationWindowHierarchyNode.binding;
						if (AnimationWindowUtility.ForceGrouping(binding2.Value))
						{
							animationWindowHierarchyNode = (AnimationWindowHierarchyNode)animationWindowHierarchyNode.parent;
						}
					}
				}
				if (animationWindowHierarchyNode.curves != null)
				{
					List<AnimationWindowCurve> list;
					if (animationWindowHierarchyNode is AnimationWindowHierarchyPropertyGroupNode || animationWindowHierarchyNode is AnimationWindowHierarchyPropertyNode)
					{
						list = AnimationWindowUtility.FilterCurves(animationWindowHierarchyNode.curves.ToArray<AnimationWindowCurve>(), animationWindowHierarchyNode.path, animationWindowHierarchyNode.animatableObjectType, animationWindowHierarchyNode.propertyName);
					}
					else
					{
						list = AnimationWindowUtility.FilterCurves(animationWindowHierarchyNode.curves.ToArray<AnimationWindowCurve>(), animationWindowHierarchyNode.path, animationWindowHierarchyNode.animatableObjectType);
					}
					foreach (AnimationWindowCurve current2 in list)
					{
						this.state.RemoveCurve(current2, undoLabel);
					}
				}
			}
			this.m_TreeView.ReloadData();
			this.state.controlInterface.ResampleAnimation();
		}

		private List<AnimationWindowCurve> GetCurvesAffectedByNodes(List<AnimationWindowHierarchyNode> nodes, bool includeLinkedCurves)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			foreach (AnimationWindowHierarchyNode current in nodes)
			{
				AnimationWindowHierarchyNode animationWindowHierarchyNode = current;
				if (animationWindowHierarchyNode.parent is AnimationWindowHierarchyPropertyGroupNode && includeLinkedCurves)
				{
					animationWindowHierarchyNode = (AnimationWindowHierarchyNode)animationWindowHierarchyNode.parent;
				}
				if (animationWindowHierarchyNode.curves != null)
				{
					if (animationWindowHierarchyNode.curves.Length > 0)
					{
						if (animationWindowHierarchyNode is AnimationWindowHierarchyPropertyGroupNode || animationWindowHierarchyNode is AnimationWindowHierarchyPropertyNode)
						{
							list.AddRange(AnimationWindowUtility.FilterCurves(animationWindowHierarchyNode.curves, animationWindowHierarchyNode.path, animationWindowHierarchyNode.animatableObjectType, animationWindowHierarchyNode.propertyName));
						}
						else
						{
							list.AddRange(AnimationWindowUtility.FilterCurves(animationWindowHierarchyNode.curves, animationWindowHierarchyNode.path, animationWindowHierarchyNode.animatableObjectType));
						}
					}
				}
			}
			return list.Distinct<AnimationWindowCurve>().ToList<AnimationWindowCurve>();
		}

		private void MaintainTreeviewStateAfterRotationInterpolation(RotationCurveInterpolation.Mode newMode)
		{
			List<int> selectedIDs = this.state.hierarchyState.selectedIDs;
			List<int> expandedIDs = this.state.hierarchyState.expandedIDs;
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < selectedIDs.Count; i++)
			{
				AnimationWindowHierarchyNode animationWindowHierarchyNode = this.state.hierarchyData.FindItem(selectedIDs[i]) as AnimationWindowHierarchyNode;
				if (animationWindowHierarchyNode != null && !animationWindowHierarchyNode.propertyName.Equals(RotationCurveInterpolation.GetPrefixForInterpolation(newMode)))
				{
					string oldValue = animationWindowHierarchyNode.propertyName.Split(new char[]
					{
						'.'
					})[0];
					string str = animationWindowHierarchyNode.propertyName.Replace(oldValue, RotationCurveInterpolation.GetPrefixForInterpolation(newMode));
					list.Add(selectedIDs[i]);
					list2.Add((animationWindowHierarchyNode.path + animationWindowHierarchyNode.animatableObjectType.Name + str).GetHashCode());
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (selectedIDs.Contains(list[j]))
				{
					int index = selectedIDs.IndexOf(list[j]);
					selectedIDs[index] = list2[j];
				}
				if (expandedIDs.Contains(list[j]))
				{
					int index2 = expandedIDs.IndexOf(list[j]);
					expandedIDs[index2] = list2[j];
				}
				if (this.state.hierarchyState.lastClickedID == list[j])
				{
					this.state.hierarchyState.lastClickedID = list2[j];
				}
			}
			this.state.hierarchyState.selectedIDs = new List<int>(selectedIDs);
			this.state.hierarchyState.expandedIDs = new List<int>(expandedIDs);
		}

		private RotationCurveInterpolation.Mode GetRotationInterpolationMode(EditorCurveBinding[] curves)
		{
			RotationCurveInterpolation.Mode result;
			if (curves == null || curves.Length == 0)
			{
				result = RotationCurveInterpolation.Mode.Undefined;
			}
			else
			{
				RotationCurveInterpolation.Mode modeFromCurveData = RotationCurveInterpolation.GetModeFromCurveData(curves[0]);
				for (int i = 1; i < curves.Length; i++)
				{
					RotationCurveInterpolation.Mode modeFromCurveData2 = RotationCurveInterpolation.GetModeFromCurveData(curves[i]);
					if (modeFromCurveData != modeFromCurveData2)
					{
						result = RotationCurveInterpolation.Mode.Undefined;
						return result;
					}
				}
				result = modeFromCurveData;
			}
			return result;
		}

		private void SetStyleTextColor(GUIStyle style, Color color)
		{
			style.normal.textColor = color;
			style.focused.textColor = color;
			style.active.textColor = color;
			style.hover.textColor = color;
		}

		public override void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
		{
			firstRowVisible = 0;
			lastRowVisible = this.m_TreeView.data.rowCount - 1;
		}

		public float GetNodeHeight(AnimationWindowHierarchyNode node)
		{
			float result;
			if (node is AnimationWindowHierarchyAddButtonNode)
			{
				result = 40f;
			}
			else
			{
				AnimationWindowHierarchyState animationWindowHierarchyState = this.m_TreeView.state as AnimationWindowHierarchyState;
				result = ((!animationWindowHierarchyState.GetTallMode(node)) ? 16f : 32f);
			}
			return result;
		}

		public override Vector2 GetTotalSize()
		{
			IList<TreeViewItem> rows = this.m_TreeView.data.GetRows();
			float num = 0f;
			for (int i = 0; i < rows.Count; i++)
			{
				AnimationWindowHierarchyNode node = rows[i] as AnimationWindowHierarchyNode;
				num += this.GetNodeHeight(node);
			}
			return new Vector2(1f, num);
		}

		private float GetTopPixelOfRow(int row, IList<TreeViewItem> rows)
		{
			float num = 0f;
			int num2 = 0;
			while (num2 < row && num2 < rows.Count)
			{
				AnimationWindowHierarchyNode node = rows[num2] as AnimationWindowHierarchyNode;
				num += this.GetNodeHeight(node);
				num2++;
			}
			return num;
		}

		public override Rect GetRowRect(int row, float rowWidth)
		{
			IList<TreeViewItem> rows = this.m_TreeView.data.GetRows();
			AnimationWindowHierarchyNode animationWindowHierarchyNode = rows[row] as AnimationWindowHierarchyNode;
			float? topPixel = animationWindowHierarchyNode.topPixel;
			if (!topPixel.HasValue)
			{
				animationWindowHierarchyNode.topPixel = new float?(this.GetTopPixelOfRow(row, rows));
			}
			float nodeHeight = this.GetNodeHeight(animationWindowHierarchyNode);
			float arg_66_0 = 0f;
			float? topPixel2 = animationWindowHierarchyNode.topPixel;
			return new Rect(arg_66_0, topPixel2.Value, rowWidth, nodeHeight);
		}

		public override void OnRowGUI(Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
		{
			AnimationWindowHierarchyNode node2 = node as AnimationWindowHierarchyNode;
			this.DoNodeGUI(rowRect, node2, selected, focused, row);
		}

		public override bool BeginRename(TreeViewItem item, float delay)
		{
			this.m_RenamedNode = (item as AnimationWindowHierarchyNode);
			if (this.m_RenamedNode.curves.Length > 0)
			{
				AnimationWindowSelectionItem selectionBinding = this.m_RenamedNode.curves[0].selectionBinding;
				if (selectionBinding != null)
				{
					GameObject rootGameObject = selectionBinding.rootGameObject;
				}
			}
			return base.GetRenameOverlay().BeginRename(this.m_RenamedNode.path, item.id, delay);
		}

		protected override void SyncFakeItem()
		{
		}

		protected override void RenameEnded()
		{
			string name = base.GetRenameOverlay().name;
			string originalName = base.GetRenameOverlay().originalName;
			if (name != originalName)
			{
				Undo.RecordObject(this.state.activeAnimationClip, "Rename Curve");
				AnimationWindowCurve[] curves = this.m_RenamedNode.curves;
				for (int i = 0; i < curves.Length; i++)
				{
					AnimationWindowCurve animationWindowCurve = curves[i];
					EditorCurveBinding renamedBinding = AnimationWindowUtility.GetRenamedBinding(animationWindowCurve.binding, name);
					if (AnimationWindowUtility.CurveExists(renamedBinding, this.state.allCurves.ToArray()))
					{
						Debug.LogWarning("Curve already exists, renaming cancelled.");
					}
					else
					{
						AnimationWindowUtility.RenameCurvePath(animationWindowCurve, renamedBinding, animationWindowCurve.clip);
					}
				}
			}
			this.m_RenamedNode = null;
		}

		protected override Texture GetIconForItem(TreeViewItem item)
		{
			Texture result;
			if (item != null)
			{
				result = item.icon;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
