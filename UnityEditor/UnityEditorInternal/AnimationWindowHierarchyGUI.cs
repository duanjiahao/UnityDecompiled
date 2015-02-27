using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class AnimationWindowHierarchyGUI : TreeViewGUI
	{
		private const float k_RowRightOffset = 10f;
		private const float k_PlusButtonWidth = 17f;
		private const float k_TallModeButtonWidth = 17f;
		private const float k_ValueFieldWidth = 50f;
		private const float k_ValueFieldOffsetFromRightSide = 75f;
		private const float k_ColorIndicatorTopMargin = 3f;
		public const float k_DopeSheetRowHeight = 16f;
		public const float k_DopeSheetRowHeightTall = 32f;
		public const float k_AddCurveButtonNodeHeight = 40f;
		public const float k_RowBackgroundColorBrightness = 0.28f;
		private GUIStyle plusButtonStyle;
		private GUIStyle animationRowEvenStyle;
		private GUIStyle animationRowOddStyle;
		private GUIStyle animationSelectionTextField;
		private GUIStyle animationLineStyle;
		private GUIStyle animationCurveDropdown;
		private AnimationWindowHierarchyNode m_RenamedNode;
		private Color m_LightSkinPropertyTextColor = new Color(0.35f, 0.35f, 0.35f);
		private static Color k_KeyColorInDopesheetMode = new Color(0.7f, 0.7f, 0.7f, 1f);
		private static Color k_KeyColorForNonCurves = new Color(0.7f, 0.7f, 0.7f, 0.5f);
		private static Color k_LeftoverCurveColor = Color.yellow;
		internal static int s_WasInsideValueRectFrame = -1;
		public AnimationWindowState state
		{
			get;
			set;
		}
		public AnimationWindowHierarchyGUI(TreeView treeView, AnimationWindowState state) : base(treeView)
		{
			this.state = state;
		}
		protected override void InitStyles()
		{
			base.InitStyles();
			if (this.plusButtonStyle == null)
			{
				this.plusButtonStyle = new GUIStyle("OL Plus");
			}
			if (this.animationRowEvenStyle == null)
			{
				this.animationRowEvenStyle = new GUIStyle("AnimationRowEven");
			}
			if (this.animationRowOddStyle == null)
			{
				this.animationRowOddStyle = new GUIStyle("AnimationRowOdd");
			}
			if (this.animationSelectionTextField == null)
			{
				this.animationSelectionTextField = new GUIStyle("AnimationSelectionTextField");
			}
			if (this.animationLineStyle == null)
			{
				this.animationLineStyle = new GUIStyle(TreeViewGUI.s_Styles.lineStyle);
				this.animationLineStyle.padding.left = 0;
			}
			if (this.animationCurveDropdown == null)
			{
				this.animationCurveDropdown = new GUIStyle("AnimPropDropdown");
			}
		}
		protected void DoNodeGUI(Rect rect, AnimationWindowHierarchyNode node, bool selected, bool focused, int row)
		{
			this.InitStyles();
			if (node is AnimationWindowHierarchyMasterNode)
			{
				return;
			}
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
				bool flag = this.state.m_ActiveGameObject && AnimationWindowUtility.GameObjectIsAnimatable(this.state.m_ActiveGameObject, this.state.m_ActiveAnimationClip);
				EditorGUI.BeginDisabledGroup(!flag);
				this.DoAddCurveButton(rect);
				EditorGUI.EndDisabledGroup();
			}
			else
			{
				this.DoRowBackground(rect, row);
				this.DoIconAndName(rect, node, selected, focused, indent);
				this.DoFoldout(node, rect, indent);
				EditorGUI.BeginDisabledGroup(this.state.IsReadOnly);
				this.DoValueField(rect, node, row);
				this.HandleContextMenu(rect, node);
				EditorGUI.EndDisabledGroup();
				this.DoCurveDropdown(rect, node);
				this.DoCurveColorIndicator(rect, node);
			}
			EditorGUIUtility.SetIconSize(Vector2.zero);
		}
		public override void BeginRowGUI()
		{
			base.BeginRowGUI();
			this.HandleDelete();
		}
		private void DoAddCurveButton(Rect rect)
		{
			float num = 24f;
			float num2 = 10f;
			Rect rect2 = new Rect(rect.xMin + num, rect.yMin + num2, rect.width - num * 2f, rect.height - num2 * 2f);
			if (GUI.Button(rect2, "Add Curve"))
			{
				if (!this.state.m_AnimationWindow.EnsureAnimationMode())
				{
					return;
				}
				AddCurvesPopup.gameObject = this.state.m_RootGameObject;
				AddCurvesPopupHierarchyDataSource.showEntireHierarchy = true;
				if (AddCurvesPopup.ShowAtPosition(rect2, this.state))
				{
					GUIUtility.ExitGUI();
				}
			}
		}
		private void DoRowBackground(Rect rect, int row)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			if (row % 2 == 0)
			{
				this.animationRowEvenStyle.Draw(rect, false, false, false, false);
			}
			else
			{
				this.animationRowOddStyle.Draw(rect, false, false, false, false);
			}
		}
		private void DoFoldout(AnimationWindowHierarchyNode node, Rect rect, float indent)
		{
			if (this.m_TreeView.data.IsExpandable(node))
			{
				Rect position = rect;
				position.x = indent;
				position.width = this.k_FoldoutWidth;
				EditorGUI.BeginChangeCheck();
				bool flag = GUI.Toggle(position, this.m_TreeView.data.IsExpanded(node), GUIContent.none, TreeViewGUI.s_Styles.foldout);
				if (EditorGUI.EndChangeCheck())
				{
					if (Event.current.alt)
					{
						this.m_TreeView.data.SetExpandedWithChildren(node, flag);
					}
					else
					{
						this.m_TreeView.data.SetExpanded(node, flag);
					}
					if (flag)
					{
						this.m_TreeView.UserExpandedNode(node);
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
					position2.width = this.k_FoldoutWidth;
					EditorGUI.BeginChangeCheck();
					bool flag2 = animationWindowHierarchyState.getTallMode(animationWindowHierarchyPropertyNode);
					flag2 = GUI.Toggle(position2, flag2, GUIContent.none, TreeViewGUI.s_Styles.foldout);
					if (EditorGUI.EndChangeCheck())
					{
						animationWindowHierarchyState.setTallMode(animationWindowHierarchyPropertyNode, flag2);
					}
				}
			}
		}
		private void DoIconAndName(Rect rect, AnimationWindowHierarchyNode node, bool selected, bool focused, float indent)
		{
			EditorGUIUtility.SetIconSize(new Vector2(13f, 13f));
			int itemControlID = TreeView.GetItemControlID(node);
			if (Event.current.type == EventType.Repaint)
			{
				bool flag = this.m_TreeView.dragging.GetDropTargetControlID() == itemControlID && this.m_TreeView.data.CanBeParent(node);
				this.animationLineStyle.Draw(rect, GUIContent.none, flag, flag, selected, focused);
				if (AnimationMode.InAnimationMode())
				{
					rect.width -= 77f;
				}
				bool flag2 = AnimationWindowUtility.IsNodeLeftOverCurve(node, this.state.m_RootGameObject);
				if (node.depth == 0)
				{
					Transform x = this.state.m_RootGameObject.transform.Find(node.path);
					if (x == null)
					{
						flag2 = true;
					}
					GUIContent content = new GUIContent(this.GetGameObjectName(node.path) + " : " + node.displayName, this.GetIconForNode(node));
					Color textColor = this.animationLineStyle.normal.textColor;
					Color color = (!EditorGUIUtility.isProSkin) ? Color.black : (Color.gray * 1.35f);
					color = ((!flag2) ? color : AnimationWindowHierarchyGUI.k_LeftoverCurveColor);
					this.SetStyleTextColor(this.animationLineStyle, color);
					rect.xMin += (float)((int)(indent + this.k_FoldoutWidth));
					this.animationLineStyle.Draw(rect, content, flag, flag, selected, focused);
					this.SetStyleTextColor(this.animationLineStyle, textColor);
				}
				else
				{
					TreeViewGUI.s_Styles.content.text = node.displayName;
					Texture iconForNode = this.GetIconForNode(node);
					TreeViewGUI.s_Styles.content.image = iconForNode;
					Color textColor2 = this.animationLineStyle.normal.textColor;
					Color color2 = (!EditorGUIUtility.isProSkin) ? this.m_LightSkinPropertyTextColor : Color.gray;
					color2 = ((!flag2) ? color2 : AnimationWindowHierarchyGUI.k_LeftoverCurveColor);
					this.SetStyleTextColor(this.animationLineStyle, color2);
					rect.xMin += (float)((int)(indent + this.k_IndentWidth + this.k_FoldoutWidth));
					this.animationLineStyle.Draw(rect, TreeViewGUI.s_Styles.content, flag, flag, selected, focused);
					this.SetStyleTextColor(this.animationLineStyle, textColor2);
					if (this.m_TreeView.dragging.GetRowMarkerControlID() == itemControlID)
					{
						this.m_DraggingInsertionMarkerRect = new Rect(rect.x + indent + this.k_FoldoutWidth, rect.y, rect.width - indent, rect.height);
					}
				}
			}
			if (this.IsRenaming(node.id))
			{
				base.GetRenameOverlay().editFieldRect = new Rect(rect.x + this.k_IndentWidth, rect.y, rect.width - this.k_IndentWidth - 1f, rect.height);
			}
		}
		private string GetGameObjectName(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return this.state.m_RootGameObject.name;
			}
			string[] array = path.Split(new char[]
			{
				'/'
			});
			return array[array.Length - 1];
		}
		private string GetPathWithoutChildmostGameObject(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			int num = path.LastIndexOf('/');
			return path.Substring(0, num + 1);
		}
		private void DoValueField(Rect rect, AnimationWindowHierarchyNode node, int row)
		{
			if (!AnimationMode.InAnimationMode())
			{
				return;
			}
			EditorGUI.BeginDisabledGroup(this.state.IsReadOnly);
			if (node is AnimationWindowHierarchyPropertyNode)
			{
				AnimationWindowCurve[] curves = this.state.GetCurves(node, false);
				if (curves == null || curves.Length == 0)
				{
					return;
				}
				AnimationWindowCurve animationWindowCurve = curves[0];
				object currentValue = AnimationWindowUtility.GetCurrentValue(this.state.m_RootGameObject, animationWindowCurve.binding);
				Type editorCurveValueType = AnimationUtility.GetEditorCurveValueType(this.state.m_RootGameObject, animationWindowCurve.binding);
				if (currentValue is float)
				{
					float num = (float)currentValue;
					Rect position = new Rect(rect.xMax - 75f, rect.y, 50f, rect.height);
					if (Event.current.type == EventType.MouseMove && position.Contains(Event.current.mousePosition))
					{
						AnimationWindowHierarchyGUI.s_WasInsideValueRectFrame = Time.frameCount;
					}
					EditorGUI.BeginChangeCheck();
					if (editorCurveValueType == typeof(bool))
					{
						num = (float)((!EditorGUI.Toggle(position, num != 0f)) ? 0 : 1);
					}
					else
					{
						int controlID = GUIUtility.GetControlID(123456544, FocusType.Keyboard, position);
						bool flag = GUIUtility.keyboardControl == controlID && EditorGUIUtility.editingTextField && Event.current.type == EventType.KeyDown && (Event.current.character == '\n' || Event.current.character == '\u0003');
						num = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position, new Rect(0f, 0f, 0f, 0f), controlID, num, EditorGUI.kFloatFieldFormatString, this.animationSelectionTextField, false);
						if (flag)
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
						AnimationWindowKeyframe animationWindowKeyframe = null;
						foreach (AnimationWindowKeyframe current in animationWindowCurve.m_Keyframes)
						{
							if (Mathf.Approximately(current.time, this.state.time.time))
							{
								animationWindowKeyframe = current;
							}
						}
						if (animationWindowKeyframe == null)
						{
							AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, num, editorCurveValueType, this.state.time);
						}
						else
						{
							animationWindowKeyframe.value = num;
						}
						this.state.SaveCurve(animationWindowCurve);
					}
				}
			}
			EditorGUI.EndDisabledGroup();
		}
		private void DoCurveDropdown(Rect rect, AnimationWindowHierarchyNode node)
		{
			rect = new Rect(rect.xMax - 10f - 12f, rect.yMin + 2f, 22f, 12f);
			if (GUI.Button(rect, GUIContent.none, this.animationCurveDropdown))
			{
				this.state.SelectHierarchyItem(node.id, false, false);
				this.state.m_AnimationWindow.RefreshShownCurves(true);
				GenericMenu genericMenu = this.GenerateMenu(new AnimationWindowHierarchyNode[]
				{
					node
				}.ToList<AnimationWindowHierarchyNode>());
				genericMenu.DropDown(rect);
				Event.current.Use();
			}
		}
		private void DoCurveColorIndicator(Rect rect, AnimationWindowHierarchyNode node)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Color color = GUI.color;
			if (!this.state.m_ShowCurveEditor)
			{
				GUI.color = AnimationWindowHierarchyGUI.k_KeyColorInDopesheetMode;
			}
			else
			{
				if (node.curves.Length == 1 && !node.curves[0].isPPtrCurve)
				{
					GUI.color = CurveUtility.GetPropertyColor(node.curves[0].binding.propertyName);
				}
				else
				{
					GUI.color = AnimationWindowHierarchyGUI.k_KeyColorForNonCurves;
				}
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
		private void HandleDelete()
		{
			if (this.m_TreeView.HasFocus())
			{
				EventType type = Event.current.type;
				if (type != EventType.KeyDown)
				{
					if (type == EventType.ExecuteCommand)
					{
						if (Event.current.commandName == "SoftDelete" || Event.current.commandName == "Delete")
						{
							if (Event.current.type == EventType.ExecuteCommand)
							{
								this.RemoveCurvesFromSelectedNodes();
							}
							Event.current.Use();
						}
					}
				}
				else
				{
					if (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete)
					{
						this.RemoveCurvesFromSelectedNodes();
						Event.current.Use();
					}
				}
			}
		}
		private void HandleContextMenu(Rect rect, AnimationWindowHierarchyNode node)
		{
			if (Event.current.type != EventType.ContextClick)
			{
				return;
			}
			if (rect.Contains(Event.current.mousePosition))
			{
				this.state.SelectHierarchyItem(node.id, false, true);
				this.state.m_AnimationWindow.RefreshShownCurves(true);
				this.GenerateMenu(this.state.selectedHierarchyNodes).ShowAsContext();
				Event.current.Use();
			}
		}
		private GenericMenu GenerateMenu(List<AnimationWindowHierarchyNode> interactedNodes)
		{
			List<AnimationWindowCurve> curvesAffectedByNodes = this.GetCurvesAffectedByNodes(interactedNodes, false);
			List<AnimationWindowCurve> curvesAffectedByNodes2 = this.GetCurvesAffectedByNodes(interactedNodes, true);
			bool flag = curvesAffectedByNodes.Count == 1 && AnimationWindowUtility.ForceGrouping(curvesAffectedByNodes[0].binding);
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent((curvesAffectedByNodes.Count <= 1 && !flag) ? "Remove Property" : "Remove Properties"), false, new GenericMenu.MenuFunction(this.RemoveCurvesFromSelectedNodes));
			bool flag2 = true;
			EditorCurveBinding[] array = new EditorCurveBinding[curvesAffectedByNodes2.Count];
			for (int i = 0; i < curvesAffectedByNodes.Count; i++)
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
				genericMenu.AddItem(new GUIContent("Interpolation/Euler Angles"), rotationInterpolationMode == RotationCurveInterpolation.Mode.Baked, new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.Baked);
				genericMenu.AddItem(new GUIContent("Interpolation/Quaternion"), rotationInterpolationMode == RotationCurveInterpolation.Mode.NonBaked, new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.NonBaked);
			}
			if (AnimationMode.InAnimationMode())
			{
				genericMenu.AddSeparator(string.Empty);
				bool flag3 = true;
				bool flag4 = true;
				bool flag5 = true;
				foreach (AnimationWindowCurve current2 in curvesAffectedByNodes)
				{
					if (!current2.HasKeyframe(this.state.time))
					{
						flag3 = false;
					}
					else
					{
						flag4 = false;
						if (!current2.isPPtrCurve)
						{
							flag5 = false;
						}
					}
				}
				string text = "Add Key";
				if (flag3)
				{
					genericMenu.AddDisabledItem(new GUIContent(text));
				}
				else
				{
					genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.AddKeysAtCurrentTime), curvesAffectedByNodes);
				}
				text = "Delete Key";
				if (flag4)
				{
					genericMenu.AddDisabledItem(new GUIContent(text));
				}
				else
				{
					genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.DeleteKeysAtCurrentTime), curvesAffectedByNodes);
				}
				if (!flag5)
				{
					genericMenu.AddSeparator(string.Empty);
					List<KeyIdentifier> list = new List<KeyIdentifier>();
					foreach (AnimationWindowCurve current3 in curvesAffectedByNodes)
					{
						if (!current3.isPPtrCurve)
						{
							int keyframeIndex = current3.GetKeyframeIndex(this.state.time);
							if (keyframeIndex != -1)
							{
								CurveRenderer curveRenderer = CurveRendererCache.GetCurveRenderer(this.state.m_ActiveAnimationClip, current3.binding);
								int curveID = CurveUtility.GetCurveID(this.state.m_ActiveAnimationClip, current3.binding);
								list.Add(new KeyIdentifier(curveRenderer, curveID, keyframeIndex));
							}
						}
					}
					CurveMenuManager curveMenuManager = new CurveMenuManager(this.state.m_AnimationWindow);
					curveMenuManager.AddTangentMenuItems(genericMenu, list);
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
			foreach (AnimationWindowCurve current in curves)
			{
				AnimationWindowUtility.AddKeyframeToCurve(this.state, current, this.state.time);
			}
		}
		private void DeleteKeysAtCurrentTime(object obj)
		{
			this.DeleteKeysAtCurrentTime((List<AnimationWindowCurve>)obj);
		}
		private void DeleteKeysAtCurrentTime(List<AnimationWindowCurve> curves)
		{
			foreach (AnimationWindowCurve current in curves)
			{
				current.RemoveKeyframe(this.state.time);
				this.state.SaveCurve(current);
			}
		}
		private void ChangeRotationInterpolation(object interpolationMode)
		{
			RotationCurveInterpolation.Mode mode = (RotationCurveInterpolation.Mode)((int)interpolationMode);
			AnimationWindowCurve[] array = this.state.activeCurves.ToArray();
			EditorCurveBinding[] array2 = new EditorCurveBinding[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].binding;
			}
			RotationCurveInterpolation.SetInterpolation(this.state.m_ActiveAnimationClip, array2, mode);
			this.MaintainTreeviewStateAfterRotationInterpolation(mode);
			this.state.m_HierarchyData.ReloadData();
		}
		private void RemoveCurvesFromSelectedNodes()
		{
			this.RemoveCurvesFromNodes(this.state.selectedHierarchyNodes);
		}
		private void RemoveCurvesFromNodes(List<AnimationWindowHierarchyNode> nodes)
		{
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
				AnimationWindowCurve[] array;
				if (animationWindowHierarchyNode is AnimationWindowHierarchyPropertyGroupNode || animationWindowHierarchyNode is AnimationWindowHierarchyPropertyNode)
				{
					array = AnimationWindowUtility.FilterCurves(this.state.allCurves.ToArray(), animationWindowHierarchyNode.path, animationWindowHierarchyNode.animatableObjectType, animationWindowHierarchyNode.propertyName);
				}
				else
				{
					array = AnimationWindowUtility.FilterCurves(this.state.allCurves.ToArray(), animationWindowHierarchyNode.path, animationWindowHierarchyNode.animatableObjectType);
				}
				AnimationWindowCurve[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					AnimationWindowCurve curve = array2[i];
					this.state.RemoveCurve(curve);
				}
			}
			this.m_TreeView.ReloadData();
		}
		private List<AnimationWindowCurve> GetCurvesAffectedByNodes(List<AnimationWindowHierarchyNode> nodes, bool includeLinkedCurves)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			foreach (AnimationWindowHierarchyNode current in nodes)
			{
				AnimationWindowHierarchyNode animationWindowHierarchyNode = current;
				if (animationWindowHierarchyNode.parent is AnimationWindowHierarchyPropertyGroupNode)
				{
					if (!includeLinkedCurves)
					{
						EditorCurveBinding? binding = animationWindowHierarchyNode.binding;
						if (!binding.HasValue)
						{
							goto IL_6B;
						}
						EditorCurveBinding? binding2 = animationWindowHierarchyNode.binding;
						if (!AnimationWindowUtility.ForceGrouping(binding2.Value))
						{
							goto IL_6B;
						}
					}
					animationWindowHierarchyNode = (AnimationWindowHierarchyNode)animationWindowHierarchyNode.parent;
				}
				IL_6B:
				if (animationWindowHierarchyNode is AnimationWindowHierarchyPropertyGroupNode || animationWindowHierarchyNode is AnimationWindowHierarchyPropertyNode)
				{
					list.AddRange(AnimationWindowUtility.FilterCurves(this.state.allCurves.ToArray(), animationWindowHierarchyNode.path, animationWindowHierarchyNode.animatableObjectType, animationWindowHierarchyNode.propertyName));
				}
				else
				{
					list.AddRange(AnimationWindowUtility.FilterCurves(this.state.allCurves.ToArray(), animationWindowHierarchyNode.path, animationWindowHierarchyNode.animatableObjectType));
				}
			}
			return list.Distinct<AnimationWindowCurve>().ToList<AnimationWindowCurve>();
		}
		private void MaintainTreeviewStateAfterRotationInterpolation(RotationCurveInterpolation.Mode newMode)
		{
			List<int> selectedIDs = this.state.m_hierarchyState.selectedIDs;
			List<int> expandedIDs = this.state.m_hierarchyState.expandedIDs;
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < selectedIDs.Count; i++)
			{
				AnimationWindowHierarchyNode animationWindowHierarchyNode = this.state.m_HierarchyData.FindItem(selectedIDs[i]) as AnimationWindowHierarchyNode;
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
				if (this.state.m_hierarchyState.lastClickedID == list[j])
				{
					this.state.m_hierarchyState.lastClickedID = list2[j];
				}
			}
			this.state.m_hierarchyState.selectedIDs = new List<int>(selectedIDs);
			this.state.m_hierarchyState.expandedIDs = new List<int>(expandedIDs);
		}
		private RotationCurveInterpolation.Mode GetRotationInterpolationMode(EditorCurveBinding[] curves)
		{
			if (curves == null || curves.Length == 0)
			{
				return RotationCurveInterpolation.Mode.Undefined;
			}
			RotationCurveInterpolation.Mode modeFromCurveData = RotationCurveInterpolation.GetModeFromCurveData(curves[0]);
			for (int i = 1; i < curves.Length; i++)
			{
				RotationCurveInterpolation.Mode modeFromCurveData2 = RotationCurveInterpolation.GetModeFromCurveData(curves[i]);
				if (modeFromCurveData != modeFromCurveData2)
				{
					return RotationCurveInterpolation.Mode.Undefined;
				}
			}
			return modeFromCurveData;
		}
		private void SetStyleTextColor(GUIStyle style, Color color)
		{
			style.normal.textColor = color;
			style.focused.textColor = color;
			style.active.textColor = color;
			style.hover.textColor = color;
		}
		public override void GetFirstAndLastRowVisible(List<TreeViewItem> rows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible)
		{
			firstRowVisible = 0;
			lastRowVisible = rows.Count - 1;
		}
		public override float GetTopPixelOfRow(int row, List<TreeViewItem> rows)
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
		public float GetNodeHeight(AnimationWindowHierarchyNode node)
		{
			if (node is AnimationWindowHierarchyAddButtonNode)
			{
				return 40f;
			}
			AnimationWindowHierarchyState animationWindowHierarchyState = this.m_TreeView.state as AnimationWindowHierarchyState;
			return (!animationWindowHierarchyState.getTallMode(node)) ? 16f : 32f;
		}
		public override Vector2 GetTotalSize(List<TreeViewItem> rows)
		{
			float num = 0f;
			for (int i = 0; i < rows.Count; i++)
			{
				AnimationWindowHierarchyNode node = rows[i] as AnimationWindowHierarchyNode;
				num += this.GetNodeHeight(node);
			}
			return new Vector2(1f, num);
		}
		public override Rect OnRowGUI(TreeViewItem node, int row, float rowWidth, bool selected, bool focused)
		{
			AnimationWindowHierarchyNode animationWindowHierarchyNode = node as AnimationWindowHierarchyNode;
			float? topPixel = animationWindowHierarchyNode.topPixel;
			if (!topPixel.HasValue)
			{
				animationWindowHierarchyNode.topPixel = new float?(this.GetTopPixelOfRow(row, this.m_TreeView.data.GetVisibleRows()));
			}
			float nodeHeight = this.GetNodeHeight(animationWindowHierarchyNode);
			float arg_5F_1 = 0f;
			float? topPixel2 = animationWindowHierarchyNode.topPixel;
			Rect rect = new Rect(arg_5F_1, topPixel2.Value, rowWidth, nodeHeight);
			this.DoNodeGUI(rect, animationWindowHierarchyNode, selected, focused, row);
			return rect;
		}
		public override bool BeginRename(TreeViewItem item, float delay)
		{
			this.m_RenamedNode = (item as AnimationWindowHierarchyNode);
			return base.GetRenameOverlay().BeginRename(this.GetGameObjectName(this.m_RenamedNode.path), item.id, delay);
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
				AnimationWindowCurve[] curves = this.m_RenamedNode.curves;
				for (int i = 0; i < curves.Length; i++)
				{
					AnimationWindowCurve animationWindowCurve = curves[i];
					string newPath = this.RenamePath(animationWindowCurve.path, name);
					EditorCurveBinding renamedBinding = AnimationWindowUtility.GetRenamedBinding(animationWindowCurve.binding, newPath);
					if (AnimationWindowUtility.CurveExists(renamedBinding, this.state.allCurves.ToArray()))
					{
						Debug.LogWarning("Curve already exists, renaming cancelled.");
					}
					else
					{
						AnimationWindowUtility.RenameCurvePath(animationWindowCurve, renamedBinding, this.state.m_ActiveAnimationClip);
					}
				}
			}
			this.m_RenamedNode = null;
		}
		private string RenamePath(string oldPath, string newGameObjectName)
		{
			if (oldPath.Length > 0)
			{
				string text = this.GetPathWithoutChildmostGameObject(oldPath);
				if (text.Length > 0)
				{
					text += "/";
				}
				return text + newGameObjectName;
			}
			return newGameObjectName;
		}
		protected override Texture GetIconForNode(TreeViewItem item)
		{
			if (item != null)
			{
				return item.icon;
			}
			return null;
		}
	}
}
