using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
namespace TreeEditor
{
	[CustomEditor(typeof(Tree))]
	internal class TreeEditor : Editor
	{
		private enum PropertyType
		{
			Normal,
			FullUndo,
			FullUpdate,
			FullUndoUpdate
		}
		public enum EditMode
		{
			None = -1,
			MoveNode,
			RotateNode,
			Freehand,
			Parameter,
			Everything,
			Delete,
			CreateGroup,
			Duplicate
		}
		public class Styles
		{
			public GUIContent iconAddLeaves = EditorGUIUtility.IconContent("TreeEditor.AddLeaves", "Add Leaf Group");
			public GUIContent iconAddBranches = EditorGUIUtility.IconContent("TreeEditor.AddBranches", "Add Branch Group");
			public GUIContent iconTrash = EditorGUIUtility.IconContent("TreeEditor.Trash", "Delete Selected Group");
			public GUIContent iconDuplicate = EditorGUIUtility.IconContent("TreeEditor.Duplicate", "Duplicate Selected Group");
			public GUIContent iconRefresh = EditorGUIUtility.IconContent("TreeEditor.Refresh", "Recompute Tree");
			public GUIStyle toolbar = "TE Toolbar";
			public GUIStyle toolbarButton = "TE toolbarbutton";
			public GUIStyle nodeBackground = "TE NodeBackground";
			public GUIStyle[] nodeBoxes = new GUIStyle[]
			{
				"TE NodeBox",
				"TE NodeBoxSelected"
			};
			public GUIContent warningIcon = EditorGUIUtility.IconContent("editicon.sml");
			public GUIContent[] nodeIcons = new GUIContent[]
			{
				EditorGUIUtility.IconContent("tree_icon_branch_frond"),
				EditorGUIUtility.IconContent("tree_icon_branch"),
				EditorGUIUtility.IconContent("tree_icon_frond"),
				EditorGUIUtility.IconContent("tree_icon_leaf"),
				EditorGUIUtility.IconContent("tree_icon")
			};
			public GUIContent[] visibilityIcons = new GUIContent[]
			{
				EditorGUIUtility.IconContent("animationvisibilitytoggleon"),
				EditorGUIUtility.IconContent("animationvisibilitytoggleoff")
			};
			public GUIStyle nodeLabelTop = "TE NodeLabelTop";
			public GUIStyle nodeLabelBot = "TE NodeLabelBot";
			public GUIStyle pinLabel = "TE PinLabel";
		}
		internal class HierachyNode
		{
			internal Vector3 pos;
			internal TreeGroup group;
			internal Rect rect;
		}
		private const float kSectionSpace = 10f;
		private const float kIndentSpace = 16f;
		private const float kCurveSpace = 50f;
		private static Vector3 s_StartPosition;
		private static int s_SelectedPoint = -1;
		private static TreeNode s_SelectedNode;
		private static TreeGroup s_SelectedGroup;
		private static TreeEditor.EditMode s_EditMode = TreeEditor.EditMode.MoveNode;
		private static string s_SavedSourceMaterialsHash;
		private static float s_CutoutMaterialHashBeforeUndo;
		private bool m_WantCompleteUpdate;
		private bool m_WantedCompleteUpdateInPreviousFrame;
		private bool m_SectionHasCurves = true;
		private static int s_ShowCategory = -1;
		private readonly Rect m_CurveRangesA = new Rect(0f, 0f, 1f, 1f);
		private readonly Rect m_CurveRangesB = new Rect(0f, -1f, 1f, 2f);
		private static readonly Color s_GroupColor = new Color(1f, 0f, 1f, 1f);
		private static readonly Color s_NormalColor = new Color(1f, 1f, 0f, 1f);
		private static TreeGroupRoot s_CopyPasteGroup;
		private readonly TreeEditorHelper m_TreeEditorHelper = new TreeEditorHelper();
		private readonly AnimBool[] m_SectionAnimators = new AnimBool[6];
		private Vector3 m_LockedWorldPos = Vector3.zero;
		private Matrix4x4 m_StartMatrix = Matrix4x4.identity;
		private Quaternion m_StartPointRotation = Quaternion.identity;
		private bool m_StartPointRotationDirty;
		private Quaternion m_GlobalToolRotation = Quaternion.identity;
		private TreeSpline m_TempSpline;
		public static TreeEditor.Styles styles;
		private Vector2 hierachyScroll = default(Vector2);
		private Vector2 hierachyNodeSize = new Vector2(40f, 48f);
		private Vector2 hierachyNodeSpace = new Vector2(16f, 16f);
		private Vector2 hierachySpread = new Vector2(32f, 32f);
		private Rect hierachyView = new Rect(0f, 0f, 0f, 0f);
		private Rect hierachyRect = new Rect(0f, 0f, 0f, 0f);
		private Rect hierachyDisplayRect = new Rect(0f, 0f, 0f, 0f);
		private TreeEditor.HierachyNode dragNode;
		private TreeEditor.HierachyNode dropNode;
		private bool isDragging;
		private Vector2 dragClickPos;
		public static TreeEditor.EditMode editMode
		{
			get
			{
				switch (Tools.current)
				{
				case Tool.View:
					TreeEditor.s_EditMode = TreeEditor.EditMode.None;
					break;
				case Tool.Move:
					TreeEditor.s_EditMode = TreeEditor.EditMode.MoveNode;
					break;
				case Tool.Rotate:
					TreeEditor.s_EditMode = TreeEditor.EditMode.RotateNode;
					break;
				case Tool.Scale:
					TreeEditor.s_EditMode = TreeEditor.EditMode.None;
					break;
				}
				return TreeEditor.s_EditMode;
			}
			set
			{
				switch (value + 1)
				{
				case TreeEditor.EditMode.MoveNode:
					break;
				case TreeEditor.EditMode.RotateNode:
					Tools.current = Tool.Move;
					break;
				case TreeEditor.EditMode.Freehand:
					Tools.current = Tool.Rotate;
					break;
				default:
					Tools.current = Tool.None;
					break;
				}
				TreeEditor.s_EditMode = value;
			}
		}
		[MenuItem("GameObject/3D Object/Tree", false, 3001)]
		private static void CreateNewTree(MenuCommand menuCommand)
		{
			Mesh mesh = new Mesh();
			mesh.name = "Mesh";
			Material material = new Material(TreeEditorHelper.DefaultOptimizedBarkShader);
			material.name = "Optimized Bark Material";
			material.hideFlags = (HideFlags.HideInInspector | HideFlags.NotEditable);
			Material material2 = new Material(TreeEditorHelper.DefaultOptimizedLeafShader);
			material2.name = "Optimized Leaf Material";
			material2.hideFlags = (HideFlags.HideInInspector | HideFlags.NotEditable);
			GameObject gameObject = new GameObject("OptimizedTree", new Type[]
			{
				typeof(Tree),
				typeof(MeshFilter),
				typeof(MeshRenderer)
			});
			gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
			string path = "Assets/Tree.prefab";
			path = AssetDatabase.GenerateUniqueAssetPath(path);
			UnityEngine.Object @object = PrefabUtility.CreateEmptyPrefab(path);
			AssetDatabase.AddObjectToAsset(mesh, @object);
			AssetDatabase.AddObjectToAsset(material, @object);
			AssetDatabase.AddObjectToAsset(material2, @object);
			TreeData treeData = ScriptableObject.CreateInstance<TreeData>();
			treeData.name = "Tree Data";
			treeData.Initialize();
			treeData.optimizedSolidMaterial = material;
			treeData.optimizedCutoutMaterial = material2;
			treeData.mesh = mesh;
			gameObject.GetComponent<Tree>().data = treeData;
			AssetDatabase.AddObjectToAsset(treeData, @object);
			GameObject target = PrefabUtility.ReplacePrefab(gameObject, @object, ReplacePrefabOptions.Default);
			UnityEngine.Object.DestroyImmediate(gameObject, false);
			GameObject gameObject2 = PrefabUtility.InstantiatePrefab(target) as GameObject;
			GameObjectUtility.SetParentAndAlign(gameObject2, menuCommand.context as GameObject);
			Undo.RegisterCreatedObjectUndo(gameObject2, "Create New Tree");
			Material[] materials;
			treeData.UpdateMesh(gameObject2.transform.worldToLocalMatrix, out materials);
			TreeEditor.AssignMaterials(gameObject2.GetComponent<Renderer>(), materials, true);
			Selection.activeObject = gameObject2;
		}
		private static TreeData GetTreeData(Tree tree)
		{
			if (tree == null)
			{
				return null;
			}
			return tree.data as TreeData;
		}
		private static void PreviewMesh(Tree tree)
		{
			TreeEditor.PreviewMesh(tree, true);
		}
		private static void PreviewMesh(Tree tree, bool callExitGUI)
		{
			TreeData treeData = TreeEditor.GetTreeData(tree);
			if (treeData == null)
			{
				return;
			}
			Profiler.BeginSample("TreeEditor.PreviewMesh");
			Material[] materials;
			treeData.PreviewMesh(tree.transform.worldToLocalMatrix, out materials);
			TreeEditor.AssignMaterials(tree.GetComponent<Renderer>(), materials, false);
			Profiler.EndSample();
			if (callExitGUI)
			{
				GUIUtility.ExitGUI();
			}
		}
		private static void UpdateMesh(Tree tree)
		{
			TreeEditor.UpdateMesh(tree, true);
		}
		private static void AssignMaterials(Renderer renderer, Material[] materials, bool applyToPrefab)
		{
			if (renderer != null)
			{
				if (materials == null)
				{
					materials = new Material[0];
				}
				if (applyToPrefab)
				{
					Renderer renderer2 = PrefabUtility.GetPrefabParent(renderer) as Renderer;
					if (renderer2 != null)
					{
						renderer2.sharedMaterials = materials;
						SerializedObject serializedObject = new SerializedObject(renderer);
						SerializedProperty serializedProperty = serializedObject.FindProperty("m_Materials");
						serializedProperty.prefabOverride = false;
						serializedObject.ApplyModifiedProperties();
					}
				}
				else
				{
					renderer.sharedMaterials = materials;
				}
			}
		}
		private static void UpdateMesh(Tree tree, bool callExitGUI)
		{
			TreeData treeData = TreeEditor.GetTreeData(tree);
			if (treeData == null)
			{
				return;
			}
			Profiler.BeginSample("TreeEditor.UpdateMesh");
			Material[] materials;
			treeData.UpdateMesh(tree.transform.worldToLocalMatrix, out materials);
			TreeEditor.AssignMaterials(tree.GetComponent<Renderer>(), materials, true);
			TreeEditor.s_SavedSourceMaterialsHash = treeData.materialHash;
			Profiler.EndSample();
			if (callExitGUI)
			{
				GUIUtility.ExitGUI();
			}
		}
		[MenuItem("GameObject/3D Object/Wind Zone", false, 3002)]
		private static void CreateWindZone(MenuCommand menuCommand)
		{
			GameObject gameObject = TreeEditor.CreateDefaultWindZone();
			GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
			Selection.activeObject = gameObject;
		}
		private static GameObject CreateDefaultWindZone()
		{
			GameObject gameObject = new GameObject("WindZone", new Type[]
			{
				typeof(WindZone)
			});
			Undo.RegisterCreatedObjectUndo(gameObject, "Create Wind Zone");
			return gameObject;
		}
		private float FindClosestOffset(TreeData data, Matrix4x4 objMatrix, TreeNode node, Ray mouseRay, ref float rotation)
		{
			TreeGroup group = data.GetGroup(node.groupID);
			if (group == null)
			{
				return 0f;
			}
			if (group.GetType() != typeof(TreeGroupBranch))
			{
				return 0f;
			}
			data.ValidateReferences();
			Matrix4x4 lhs = objMatrix * node.matrix;
			float num = 1f / ((float)node.spline.GetNodeCount() * 10f);
			float num2 = 0f;
			float num3 = 1E+07f;
			Vector3 vector = Vector3.zero;
			Vector3 zero = Vector3.zero;
			Vector3 p = lhs.MultiplyPoint(node.spline.GetPositionAtTime(0f));
			for (float num4 = num; num4 <= 1f; num4 += num)
			{
				Vector3 vector2 = lhs.MultiplyPoint(node.spline.GetPositionAtTime(num4));
				float num5 = 0f;
				float num6 = 0f;
				vector = MathUtils.ClosestPtSegmentRay(p, vector2, mouseRay, out num5, out num6, out zero);
				if (num5 < num3)
				{
					num2 = num4 - num + num * num6;
					num3 = num5;
					float radiusAtTime = node.GetRadiusAtTime(num2);
					float num7 = 0f;
					if (MathUtils.ClosestPtRaySphere(mouseRay, vector, radiusAtTime, ref num7, ref zero))
					{
						Matrix4x4 inverse = (lhs * node.GetLocalMatrixAtTime(num2)).inverse;
						Vector3 v = zero - vector;
						v = inverse.MultiplyVector(v);
						rotation = Mathf.Atan2(v.x, v.z) * 57.29578f;
					}
				}
				p = vector2;
			}
			data.ClearReferences();
			return num2;
		}
		private void SelectGroup(TreeGroup group)
		{
			if (group == null)
			{
				Debug.Log("GROUP SELECTION IS NULL!");
			}
			if (this.m_TreeEditorHelper.NodeHasWrongMaterial(group))
			{
				TreeEditor.s_ShowCategory = 1;
			}
			TreeEditor.s_SelectedGroup = group;
			TreeEditor.s_SelectedNode = null;
			TreeEditor.s_SelectedPoint = -1;
			EditorUtility.SetDirty(this.target);
			Tree tree = this.target as Tree;
			if (tree == null)
			{
				return;
			}
			Renderer component = tree.GetComponent<Renderer>();
			EditorUtility.SetSelectedWireframeHidden(component, !(TreeEditor.s_SelectedGroup is TreeGroupRoot));
		}
		private void SelectNode(TreeNode node, TreeData treeData)
		{
			this.SelectGroup((node != null) ? treeData.GetGroup(node.groupID) : treeData.root);
			TreeEditor.s_SelectedNode = node;
			TreeEditor.s_SelectedPoint = -1;
		}
		private void DuplicateSelected(TreeData treeData)
		{
			this.UndoStoreSelected(TreeEditor.EditMode.Duplicate);
			if (TreeEditor.s_SelectedNode != null)
			{
				TreeEditor.s_SelectedNode = treeData.DuplicateNode(TreeEditor.s_SelectedNode);
				TreeEditor.s_SelectedGroup.Lock();
			}
			else
			{
				this.SelectGroup(treeData.DuplicateGroup(TreeEditor.s_SelectedGroup));
			}
			this.m_WantCompleteUpdate = true;
			TreeEditor.UpdateMesh(this.target as Tree);
			this.m_WantCompleteUpdate = false;
		}
		private void DeleteSelected(TreeData treeData)
		{
			this.UndoStoreSelected(TreeEditor.EditMode.Delete);
			if (TreeEditor.s_SelectedNode != null)
			{
				if (TreeEditor.s_SelectedPoint >= 1)
				{
					if (TreeEditor.s_SelectedNode.spline.nodes.Length > 2)
					{
						if (TreeEditor.s_SelectedGroup.lockFlags == 0)
						{
							TreeEditor.s_SelectedGroup.Lock();
						}
						TreeEditor.s_SelectedNode.spline.RemoveNode(TreeEditor.s_SelectedPoint);
						TreeEditor.s_SelectedPoint = Mathf.Max(TreeEditor.s_SelectedPoint - 1, 0);
					}
				}
				else
				{
					if (TreeEditor.s_SelectedGroup != null && TreeEditor.s_SelectedGroup.nodeIDs.Length == 1)
					{
						TreeEditor.s_SelectedNode = null;
						this.DeleteSelected(treeData);
						return;
					}
					treeData.DeleteNode(TreeEditor.s_SelectedNode);
					TreeEditor.s_SelectedGroup.Lock();
					this.SelectGroup(TreeEditor.s_SelectedGroup);
				}
			}
			else
			{
				if (TreeEditor.s_SelectedGroup != null)
				{
					TreeGroup group = treeData.GetGroup(TreeEditor.s_SelectedGroup.parentGroupID);
					if (group == null)
					{
						return;
					}
					treeData.DeleteGroup(TreeEditor.s_SelectedGroup);
					this.SelectGroup(group);
				}
			}
			this.m_WantCompleteUpdate = true;
			TreeEditor.UpdateMesh(this.target as Tree);
			this.m_WantCompleteUpdate = false;
		}
		private void VerifySelection(TreeData treeData)
		{
			TreeGroup treeGroup = TreeEditor.s_SelectedGroup;
			TreeNode treeNode = TreeEditor.s_SelectedNode;
			if (treeGroup != null)
			{
				treeGroup = treeData.GetGroup(treeGroup.uniqueID);
			}
			if (treeNode != null)
			{
				treeNode = treeData.GetNode(treeNode.uniqueID);
			}
			if (treeGroup != treeData.root && treeGroup != null && !treeData.IsAncestor(treeData.root, treeGroup))
			{
				treeGroup = null;
				treeNode = null;
			}
			if (treeNode != null && treeData.GetGroup(treeNode.groupID) != treeGroup)
			{
				treeNode = null;
			}
			if (treeGroup == null)
			{
				treeGroup = treeData.root;
			}
			if (TreeEditor.s_SelectedGroup != null && treeGroup == TreeEditor.s_SelectedGroup)
			{
				return;
			}
			this.SelectGroup(treeGroup);
			if (treeNode != null)
			{
				this.SelectNode(treeNode, treeData);
			}
		}
		private bool OnCheckHotkeys(TreeData treeData, bool checkFrameSelected)
		{
			EventType type = Event.current.type;
			if (type != EventType.ValidateCommand)
			{
				if (type == EventType.ExecuteCommand)
				{
					if ((Event.current.commandName == "SoftDelete" || Event.current.commandName == "Delete") && TreeEditor.s_SelectedGroup != null && TreeEditor.s_SelectedGroup != treeData.root)
					{
						this.DeleteSelected(treeData);
						Event.current.Use();
					}
					if (Event.current.commandName == "FrameSelected" && checkFrameSelected)
					{
						this.FrameSelected(this.target as Tree);
						Event.current.Use();
					}
					if (Event.current.commandName == "UndoRedoPerformed")
					{
						float num = TreeEditor.GenerateMaterialHash(treeData.optimizedCutoutMaterial);
						if (TreeEditor.s_CutoutMaterialHashBeforeUndo != num)
						{
							TreeEditor.s_CutoutMaterialHashBeforeUndo = num;
						}
						else
						{
							treeData.materialHash = TreeEditor.s_SavedSourceMaterialsHash;
							this.m_StartPointRotationDirty = true;
							TreeEditor.UpdateMesh(this.target as Tree);
						}
						Event.current.Use();
						return true;
					}
					if (Event.current.commandName == "CurveChangeCompleted")
					{
						TreeEditor.UpdateMesh(this.target as Tree);
						Event.current.Use();
						return true;
					}
				}
			}
			else
			{
				if ((Event.current.commandName == "SoftDelete" || Event.current.commandName == "Delete") && TreeEditor.s_SelectedGroup != null && TreeEditor.s_SelectedGroup != treeData.root)
				{
					Event.current.Use();
				}
				if (Event.current.commandName == "FrameSelected" && checkFrameSelected)
				{
					Event.current.Use();
				}
				if (Event.current.commandName == "UndoRedoPerformed")
				{
					Event.current.Use();
				}
			}
			return false;
		}
		private Bounds CalcBounds(TreeData treeData, Matrix4x4 objMatrix, TreeNode node)
		{
			Matrix4x4 matrix4x = objMatrix * node.matrix;
			Bounds result;
			if (treeData.GetGroup(node.groupID).GetType() == typeof(TreeGroupBranch) && node.spline != null && node.spline.nodes.Length > 0)
			{
				result = new Bounds(matrix4x.MultiplyPoint(node.spline.nodes[0].point), Vector3.zero);
				for (int i = 1; i < node.spline.nodes.Length; i++)
				{
					result.Encapsulate(matrix4x.MultiplyPoint(node.spline.nodes[i].point));
				}
			}
			else
			{
				result = new Bounds(matrix4x.MultiplyPoint(Vector3.zero), Vector3.zero);
			}
			return result;
		}
		private void FrameSelected(Tree tree)
		{
			TreeData treeData = TreeEditor.GetTreeData(tree);
			Matrix4x4 localToWorldMatrix = tree.transform.localToWorldMatrix;
			Bounds bounds = new Bounds(localToWorldMatrix.MultiplyPoint(Vector3.zero), Vector3.zero);
			if (TreeEditor.s_SelectedGroup != null)
			{
				if (TreeEditor.s_SelectedGroup.GetType() == typeof(TreeGroupRoot))
				{
					MeshFilter component = tree.GetComponent<MeshFilter>();
					if (component == null || TreeEditor.s_SelectedGroup.childGroupIDs.Length == 0)
					{
						float rootSpread = TreeEditor.s_SelectedGroup.GetRootSpread();
						bounds = new Bounds(localToWorldMatrix.MultiplyPoint(Vector3.zero), localToWorldMatrix.MultiplyVector(new Vector3(rootSpread, rootSpread, rootSpread)));
					}
					else
					{
						bounds = new Bounds(localToWorldMatrix.MultiplyPoint(component.sharedMesh.bounds.center), localToWorldMatrix.MultiplyVector(component.sharedMesh.bounds.size));
					}
				}
				else
				{
					if (TreeEditor.s_SelectedNode != null)
					{
						if (TreeEditor.s_SelectedGroup.GetType() == typeof(TreeGroupLeaf) && TreeEditor.s_SelectedPoint >= 0)
						{
							bounds = new Bounds((localToWorldMatrix * TreeEditor.s_SelectedNode.matrix).MultiplyPoint(TreeEditor.s_SelectedNode.spline.nodes[TreeEditor.s_SelectedPoint].point), Vector3.zero);
						}
						else
						{
							bounds = this.CalcBounds(treeData, localToWorldMatrix, TreeEditor.s_SelectedNode);
						}
					}
					else
					{
						for (int i = 0; i < TreeEditor.s_SelectedGroup.nodeIDs.Length; i++)
						{
							Bounds bounds2 = this.CalcBounds(treeData, localToWorldMatrix, treeData.GetNode(TreeEditor.s_SelectedGroup.nodeIDs[i]));
							if (i == 0)
							{
								bounds = bounds2;
							}
							else
							{
								bounds.Encapsulate(bounds2);
							}
						}
					}
				}
			}
			Vector3 center = bounds.center;
			float newSize = bounds.size.magnitude + 1f;
			SceneView lastActiveSceneView = SceneView.lastActiveSceneView;
			if (lastActiveSceneView)
			{
				lastActiveSceneView.LookAt(center, lastActiveSceneView.rotation, newSize);
			}
		}
		private void UndoStoreSelected(TreeEditor.EditMode mode)
		{
			TreeData treeData = TreeEditor.GetTreeData(this.target as Tree);
			if (!treeData)
			{
				return;
			}
			UnityEngine.Object[] objectsToUndo = new UnityEngine.Object[]
			{
				treeData
			};
			EditorUtility.SetDirty(treeData);
			switch (mode)
			{
			case TreeEditor.EditMode.MoveNode:
				Undo.RegisterCompleteObjectUndo(objectsToUndo, "Move");
				break;
			case TreeEditor.EditMode.RotateNode:
				Undo.RegisterCompleteObjectUndo(objectsToUndo, "Rotate");
				break;
			case TreeEditor.EditMode.Freehand:
				Undo.RegisterCompleteObjectUndo(objectsToUndo, "Freehand Drawing");
				break;
			case TreeEditor.EditMode.Parameter:
				Undo.RegisterCompleteObjectUndo(objectsToUndo, "Parameter Change");
				break;
			case TreeEditor.EditMode.Everything:
				Undo.RegisterCompleteObjectUndo(objectsToUndo, "Parameter Change");
				break;
			case TreeEditor.EditMode.Delete:
				Undo.RegisterCompleteObjectUndo(objectsToUndo, "Delete");
				break;
			case TreeEditor.EditMode.CreateGroup:
				Undo.RegisterCompleteObjectUndo(objectsToUndo, "Create Group");
				break;
			case TreeEditor.EditMode.Duplicate:
				Undo.RegisterCompleteObjectUndo(objectsToUndo, "Duplicate");
				break;
			}
		}
		private void RepaintGUIView()
		{
			GUIView.current.Repaint();
		}
		private void OnEnable()
		{
			Tree tree = this.target as Tree;
			if (tree == null)
			{
				return;
			}
			TreeData treeData = TreeEditor.GetTreeData(tree);
			if (treeData == null)
			{
				return;
			}
			this.m_TreeEditorHelper.OnEnable(treeData);
			this.m_TreeEditorHelper.SetAnimsCallback(new UnityAction(this.RepaintGUIView));
			for (int i = 0; i < this.m_SectionAnimators.Length; i++)
			{
				this.m_SectionAnimators[i] = new AnimBool(TreeEditor.s_ShowCategory == i, new UnityAction(base.Repaint));
			}
			Renderer component = tree.GetComponent<Renderer>();
			EditorUtility.SetSelectedWireframeHidden(component, !(TreeEditor.s_SelectedGroup is TreeGroupRoot));
		}
		private void OnDisable()
		{
			Tools.s_Hidden = false;
			Tree tree = this.target as Tree;
			if (tree == null)
			{
				return;
			}
			Renderer component = tree.GetComponent<Renderer>();
			EditorUtility.SetSelectedWireframeHidden(component, false);
		}
		private void OnSceneGUI()
		{
			Tree tree = this.target as Tree;
			TreeData treeData = TreeEditor.GetTreeData(tree);
			if (!treeData)
			{
				return;
			}
			this.VerifySelection(treeData);
			if (TreeEditor.s_SelectedGroup == null)
			{
				return;
			}
			this.OnCheckHotkeys(treeData, true);
			Transform transform = tree.transform;
			Matrix4x4 localToWorldMatrix = tree.transform.localToWorldMatrix;
			Event current = Event.current;
			if (TreeEditor.s_SelectedGroup.GetType() == typeof(TreeGroupRoot))
			{
				Tools.s_Hidden = false;
				Handles.color = TreeEditor.s_NormalColor;
				Handles.DrawWireDisc(transform.position, transform.up, treeData.root.rootSpread);
			}
			else
			{
				Tools.s_Hidden = true;
				Handles.color = Handles.secondaryColor;
				Handles.DrawWireDisc(transform.position, transform.up, treeData.root.rootSpread);
			}
			if (TreeEditor.s_SelectedGroup != null && TreeEditor.s_SelectedGroup.GetType() == typeof(TreeGroupBranch))
			{
				EventType eventType = current.type;
				if (current.type == EventType.Ignore && current.rawType == EventType.MouseUp)
				{
					eventType = current.rawType;
				}
				Handles.DrawLine(Vector3.zero, Vector3.zero);
				GL.Begin(1);
				for (int i = 0; i < TreeEditor.s_SelectedGroup.nodeIDs.Length; i++)
				{
					TreeNode node = treeData.GetNode(TreeEditor.s_SelectedGroup.nodeIDs[i]);
					TreeSpline spline = node.spline;
					if (spline != null)
					{
						Handles.color = ((node != TreeEditor.s_SelectedNode) ? TreeEditor.s_GroupColor : TreeEditor.s_NormalColor);
						Matrix4x4 matrix4x = localToWorldMatrix * node.matrix;
						Vector3 v = matrix4x.MultiplyPoint(spline.GetPositionAtTime(0f));
						GL.Color(Handles.color);
						for (float num = 0.01f; num <= 1f; num += 0.01f)
						{
							Vector3 vector = matrix4x.MultiplyPoint(spline.GetPositionAtTime(num));
							GL.Vertex(v);
							GL.Vertex(vector);
							v = vector;
						}
					}
				}
				GL.End();
				for (int j = 0; j < TreeEditor.s_SelectedGroup.nodeIDs.Length; j++)
				{
					TreeNode node2 = treeData.GetNode(TreeEditor.s_SelectedGroup.nodeIDs[j]);
					TreeSpline spline2 = node2.spline;
					if (spline2 != null)
					{
						Handles.color = ((node2 != TreeEditor.s_SelectedNode) ? TreeEditor.s_GroupColor : TreeEditor.s_NormalColor);
						Matrix4x4 m = localToWorldMatrix * node2.matrix;
						for (int k = 0; k < spline2.nodes.Length; k++)
						{
							SplineNode splineNode = spline2.nodes[k];
							Vector3 vector2 = m.MultiplyPoint(splineNode.point);
							float size = HandleUtility.GetHandleSize(vector2) * 0.08f;
							Handles.color = Handles.centerColor;
							int keyboardControl = GUIUtility.keyboardControl;
							switch (TreeEditor.editMode)
							{
							case TreeEditor.EditMode.MoveNode:
								if (k == 0)
								{
									vector2 = Handles.FreeMoveHandle(vector2, Quaternion.identity, size, Vector3.zero, new Handles.DrawCapFunction(Handles.CircleCap));
								}
								else
								{
									vector2 = Handles.FreeMoveHandle(vector2, Quaternion.identity, size, Vector3.zero, new Handles.DrawCapFunction(Handles.RectangleCap));
								}
								if (eventType == EventType.MouseDown && current.type == EventType.Used && keyboardControl != GUIUtility.keyboardControl)
								{
									this.SelectNode(node2, treeData);
									TreeEditor.s_SelectedPoint = k;
									this.m_StartPointRotation = MathUtils.QuaternionFromMatrix(m) * splineNode.rot;
								}
								if ((eventType == EventType.MouseDown || eventType == EventType.MouseUp) && current.type == EventType.Used)
								{
									this.m_StartPointRotation = MathUtils.QuaternionFromMatrix(m) * splineNode.rot;
								}
								if (eventType == EventType.MouseUp && current.type == EventType.Used && treeData.isInPreviewMode)
								{
									TreeEditor.UpdateMesh(tree);
								}
								if (GUI.changed)
								{
									Undo.RegisterCompleteObjectUndo(treeData, "Move");
									TreeEditor.s_SelectedGroup.Lock();
									float baseAngle = node2.baseAngle;
									if (k == 0)
									{
										TreeNode node3 = treeData.GetNode(TreeEditor.s_SelectedNode.parentID);
										Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
										float d = 0f;
										if (node3 != null)
										{
											TreeGroup group = treeData.GetGroup(TreeEditor.s_SelectedGroup.parentGroupID);
											if (group.GetType() == typeof(TreeGroupBranch))
											{
												TreeEditor.s_SelectedNode.offset = this.FindClosestOffset(treeData, localToWorldMatrix, node3, ray, ref baseAngle);
												vector2 = m.MultiplyPoint(Vector3.zero);
											}
											else
											{
												if (group.GetType() == typeof(TreeGroupRoot))
												{
													Vector3 vector3 = localToWorldMatrix.MultiplyPoint(Vector3.zero);
													Plane plane = new Plane(localToWorldMatrix.MultiplyVector(Vector3.up), vector3);
													if (plane.Raycast(ray, out d))
													{
														vector2 = ray.origin + ray.direction * d;
														Vector3 v2 = vector2 - vector3;
														v2 = localToWorldMatrix.inverse.MultiplyVector(v2);
														TreeEditor.s_SelectedNode.offset = Mathf.Clamp01(v2.magnitude / treeData.root.rootSpread);
														baseAngle = Mathf.Atan2(v2.z, v2.x) * 57.29578f;
														vector2 = m.MultiplyPoint(Vector3.zero);
													}
													else
													{
														vector2 = m.MultiplyPoint(splineNode.point);
													}
												}
											}
										}
									}
									node2.baseAngle = baseAngle;
									splineNode.point = m.inverse.MultiplyPoint(vector2);
									spline2.UpdateTime();
									spline2.UpdateRotations();
									TreeEditor.PreviewMesh(tree);
									GUI.changed = false;
								}
								break;
							case TreeEditor.EditMode.RotateNode:
								Handles.FreeMoveHandle(vector2, Quaternion.identity, size, Vector3.zero, new Handles.DrawCapFunction(Handles.CircleCap));
								if (eventType == EventType.MouseDown && current.type == EventType.Used && keyboardControl != GUIUtility.keyboardControl)
								{
									this.SelectNode(node2, treeData);
									TreeEditor.s_SelectedPoint = k;
									this.m_GlobalToolRotation = Quaternion.identity;
									this.m_TempSpline = new TreeSpline(node2.spline);
								}
								GUI.changed = false;
								break;
							case TreeEditor.EditMode.Freehand:
								Handles.FreeMoveHandle(vector2, Quaternion.identity, size, Vector3.zero, new Handles.DrawCapFunction(Handles.CircleCap));
								if (eventType == EventType.MouseDown && current.type == EventType.Used && keyboardControl != GUIUtility.keyboardControl)
								{
									Undo.RegisterCompleteObjectUndo(treeData, "Free Hand");
									this.SelectNode(node2, treeData);
									TreeEditor.s_SelectedPoint = k;
									TreeEditor.s_StartPosition = vector2;
									int nodeCount = Mathf.Max(2, TreeEditor.s_SelectedPoint + 1);
									node2.spline.SetNodeCount(nodeCount);
									current.Use();
								}
								if (TreeEditor.s_SelectedPoint == k && TreeEditor.s_SelectedNode == node2 && eventType == EventType.MouseDrag)
								{
									Ray ray2 = HandleUtility.GUIPointToWorldRay(current.mousePosition);
									Vector3 forward = Camera.current.transform.forward;
									Plane plane2 = new Plane(forward, TreeEditor.s_StartPosition);
									float d2 = 0f;
									if (plane2.Raycast(ray2, out d2))
									{
										Vector3 v3 = ray2.origin + d2 * ray2.direction;
										if (TreeEditor.s_SelectedPoint == 0)
										{
											TreeEditor.s_SelectedPoint = 1;
										}
										TreeEditor.s_SelectedGroup.Lock();
										TreeEditor.s_SelectedNode.spline.nodes[TreeEditor.s_SelectedPoint].point = m.inverse.MultiplyPoint(v3);
										Vector3 b = TreeEditor.s_SelectedNode.spline.nodes[TreeEditor.s_SelectedPoint].point - TreeEditor.s_SelectedNode.spline.nodes[TreeEditor.s_SelectedPoint - 1].point;
										if (b.magnitude > 1f)
										{
											TreeEditor.s_SelectedNode.spline.nodes[TreeEditor.s_SelectedPoint].point = TreeEditor.s_SelectedNode.spline.nodes[TreeEditor.s_SelectedPoint - 1].point + b;
											TreeEditor.s_SelectedPoint++;
											if (TreeEditor.s_SelectedPoint >= TreeEditor.s_SelectedNode.spline.nodes.Length)
											{
												TreeEditor.s_SelectedNode.spline.AddPoint(m.inverse.MultiplyPoint(v3), 1.1f);
											}
										}
										TreeEditor.s_SelectedNode.spline.UpdateTime();
										TreeEditor.s_SelectedNode.spline.UpdateRotations();
										current.Use();
										TreeEditor.PreviewMesh(tree);
									}
								}
								break;
							}
							if (TreeEditor.s_SelectedPoint == k && TreeEditor.s_SelectedNode == node2 && this.m_StartPointRotationDirty)
							{
								spline2.UpdateTime();
								spline2.UpdateRotations();
								this.m_StartPointRotation = MathUtils.QuaternionFromMatrix(m) * splineNode.rot;
								this.m_GlobalToolRotation = Quaternion.identity;
								this.m_StartPointRotationDirty = false;
							}
						}
					}
				}
				if (eventType == EventType.MouseUp && TreeEditor.editMode == TreeEditor.EditMode.Freehand)
				{
					TreeEditor.s_SelectedPoint = -1;
					if (treeData.isInPreviewMode)
					{
						TreeEditor.UpdateMesh(tree);
					}
				}
				if (TreeEditor.s_SelectedPoint > 0 && TreeEditor.editMode == TreeEditor.EditMode.MoveNode && TreeEditor.s_SelectedNode != null)
				{
					TreeNode treeNode = TreeEditor.s_SelectedNode;
					SplineNode splineNode2 = treeNode.spline.nodes[TreeEditor.s_SelectedPoint];
					Matrix4x4 m2 = localToWorldMatrix * treeNode.matrix;
					Vector3 vector4 = m2.MultiplyPoint(splineNode2.point);
					Quaternion rotation = Quaternion.identity;
					if (Tools.pivotRotation == PivotRotation.Local)
					{
						if (eventType == EventType.MouseUp || eventType == EventType.MouseDown)
						{
							this.m_StartPointRotation = MathUtils.QuaternionFromMatrix(m2) * splineNode2.rot;
						}
						rotation = this.m_StartPointRotation;
					}
					vector4 = this.DoPositionHandle(vector4, rotation, false);
					if (GUI.changed)
					{
						Undo.RegisterCompleteObjectUndo(treeData, "Move");
						TreeEditor.s_SelectedGroup.Lock();
						splineNode2.point = m2.inverse.MultiplyPoint(vector4);
						treeNode.spline.UpdateTime();
						treeNode.spline.UpdateRotations();
						TreeEditor.PreviewMesh(tree);
					}
					if (eventType == EventType.MouseUp && current.type == EventType.Used && treeData.isInPreviewMode)
					{
						TreeEditor.UpdateMesh(tree);
					}
				}
				if (TreeEditor.s_SelectedPoint >= 0 && TreeEditor.editMode == TreeEditor.EditMode.RotateNode && TreeEditor.s_SelectedNode != null)
				{
					TreeNode treeNode2 = TreeEditor.s_SelectedNode;
					SplineNode splineNode3 = treeNode2.spline.nodes[TreeEditor.s_SelectedPoint];
					Matrix4x4 matrix4x2 = localToWorldMatrix * treeNode2.matrix;
					if (this.m_TempSpline == null)
					{
						this.m_TempSpline = new TreeSpline(treeNode2.spline);
					}
					Vector3 position = matrix4x2.MultiplyPoint(splineNode3.point);
					Quaternion rotation2 = Quaternion.identity;
					this.m_GlobalToolRotation = Handles.RotationHandle(this.m_GlobalToolRotation, position);
					rotation2 = this.m_GlobalToolRotation;
					if (GUI.changed)
					{
						Undo.RegisterCompleteObjectUndo(treeData, "Move");
						TreeEditor.s_SelectedGroup.Lock();
						for (int l = TreeEditor.s_SelectedPoint + 1; l < this.m_TempSpline.nodes.Length; l++)
						{
							Vector3 vector5 = this.m_TempSpline.nodes[l].point - splineNode3.point;
							vector5 = matrix4x2.MultiplyVector(vector5);
							vector5 = rotation2 * vector5;
							vector5 = matrix4x2.inverse.MultiplyVector(vector5);
							Vector3 point = splineNode3.point + vector5;
							TreeEditor.s_SelectedNode.spline.nodes[l].point = point;
						}
						treeNode2.spline.UpdateTime();
						treeNode2.spline.UpdateRotations();
						TreeEditor.PreviewMesh(tree);
					}
					if (eventType == EventType.MouseUp && current.type == EventType.Used && treeData.isInPreviewMode)
					{
						TreeEditor.UpdateMesh(tree);
					}
				}
			}
			if (TreeEditor.s_SelectedGroup != null && TreeEditor.s_SelectedGroup.GetType() == typeof(TreeGroupLeaf))
			{
				for (int n = 0; n < TreeEditor.s_SelectedGroup.nodeIDs.Length; n++)
				{
					TreeNode node4 = treeData.GetNode(TreeEditor.s_SelectedGroup.nodeIDs[n]);
					Matrix4x4 matrix4x3 = localToWorldMatrix * node4.matrix;
					Vector3 vector6 = matrix4x3.MultiplyPoint(Vector3.zero);
					float size2 = HandleUtility.GetHandleSize(vector6) * 0.08f;
					Handles.color = Handles.centerColor;
					EventType eventType2 = current.type;
					int keyboardControl2 = GUIUtility.keyboardControl;
					TreeEditor.EditMode editMode = TreeEditor.editMode;
					if (editMode != TreeEditor.EditMode.MoveNode)
					{
						if (editMode == TreeEditor.EditMode.RotateNode)
						{
							Handles.FreeMoveHandle(vector6, Quaternion.identity, size2, Vector3.zero, new Handles.DrawCapFunction(Handles.CircleCap));
							if (eventType2 == EventType.MouseDown && current.type == EventType.Used && keyboardControl2 != GUIUtility.keyboardControl)
							{
								this.SelectNode(node4, treeData);
								this.m_GlobalToolRotation = MathUtils.QuaternionFromMatrix(matrix4x3);
								this.m_StartMatrix = matrix4x3;
								this.m_StartPointRotation = node4.rotation;
								this.m_LockedWorldPos = new Vector3(matrix4x3.m03, matrix4x3.m13, matrix4x3.m23);
							}
							if (TreeEditor.s_SelectedNode == node4)
							{
								eventType2 = current.GetTypeForControl(GUIUtility.hotControl);
								this.m_GlobalToolRotation = Handles.RotationHandle(this.m_GlobalToolRotation, this.m_LockedWorldPos);
								if (eventType2 == EventType.MouseUp && current.type == EventType.Used)
								{
									this.m_LockedWorldPos = new Vector3(matrix4x3.m03, matrix4x3.m13, matrix4x3.m23);
									if (treeData.isInPreviewMode)
									{
										TreeEditor.UpdateMesh(tree);
									}
								}
								if (GUI.changed)
								{
									TreeEditor.s_SelectedGroup.Lock();
									Quaternion lhs = Quaternion.Inverse(MathUtils.QuaternionFromMatrix(this.m_StartMatrix));
									node4.rotation = this.m_StartPointRotation * (lhs * this.m_GlobalToolRotation);
									MathUtils.QuaternionNormalize(ref node4.rotation);
									TreeEditor.PreviewMesh(tree);
								}
							}
						}
					}
					else
					{
						Handles.FreeMoveHandle(vector6, Quaternion.identity, size2, Vector3.zero, new Handles.DrawCapFunction(Handles.CircleCap));
						if (eventType2 == EventType.MouseDown && current.type == EventType.Used && keyboardControl2 != GUIUtility.keyboardControl)
						{
							this.SelectNode(node4, treeData);
							this.m_GlobalToolRotation = MathUtils.QuaternionFromMatrix(matrix4x3);
							this.m_StartMatrix = matrix4x3;
							this.m_StartPointRotation = node4.rotation;
							this.m_LockedWorldPos = new Vector3(this.m_StartMatrix.m03, this.m_StartMatrix.m13, this.m_StartMatrix.m23);
						}
						if (eventType2 == EventType.MouseUp && current.type == EventType.Used && treeData.isInPreviewMode)
						{
							TreeEditor.UpdateMesh(tree);
						}
						if (GUI.changed)
						{
							TreeEditor.s_SelectedGroup.Lock();
							TreeNode node5 = treeData.GetNode(node4.parentID);
							TreeGroup group2 = treeData.GetGroup(TreeEditor.s_SelectedGroup.parentGroupID);
							Ray ray3 = HandleUtility.GUIPointToWorldRay(current.mousePosition);
							float d3 = 0f;
							float baseAngle2 = node4.baseAngle;
							if (group2.GetType() == typeof(TreeGroupBranch))
							{
								node4.offset = this.FindClosestOffset(treeData, localToWorldMatrix, node5, ray3, ref baseAngle2);
								node4.baseAngle = baseAngle2;
								TreeEditor.PreviewMesh(tree);
							}
							else
							{
								if (group2.GetType() == typeof(TreeGroupRoot))
								{
									Vector3 vector7 = localToWorldMatrix.MultiplyPoint(Vector3.zero);
									Plane plane3 = new Plane(localToWorldMatrix.MultiplyVector(Vector3.up), vector7);
									if (plane3.Raycast(ray3, out d3))
									{
										vector6 = ray3.origin + ray3.direction * d3;
										Vector3 v4 = vector6 - vector7;
										v4 = localToWorldMatrix.inverse.MultiplyVector(v4);
										node4.offset = Mathf.Clamp01(v4.magnitude / treeData.root.rootSpread);
										baseAngle2 = Mathf.Atan2(v4.z, v4.x) * 57.29578f;
									}
									node4.baseAngle = baseAngle2;
									TreeEditor.PreviewMesh(tree);
								}
							}
						}
					}
				}
			}
		}
		private Vector3 DoPositionHandle(Vector3 position, Quaternion rotation, bool hide)
		{
			Color color = Handles.color;
			Handles.color = Handles.xAxisColor;
			position = Handles.Slider(position, rotation * Vector3.right);
			Handles.color = Handles.yAxisColor;
			position = Handles.Slider(position, rotation * Vector3.up);
			Handles.color = Handles.zAxisColor;
			position = Handles.Slider(position, rotation * Vector3.forward);
			Handles.color = color;
			return position;
		}
		private Rect GUIPropBegin()
		{
			return EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
		}
		private void GUIPropEnd()
		{
			this.GUIPropEnd(true);
		}
		private void GUIPropEnd(bool addSpace)
		{
			if (addSpace)
			{
				GUILayout.Space((!this.m_SectionHasCurves) ? 0f : 54f);
			}
			EditorGUILayout.EndHorizontal();
		}
		private void GUIHandlePropertyChange(TreeEditor.PropertyType prop)
		{
			switch (prop)
			{
			case TreeEditor.PropertyType.Normal:
				this.UndoStoreSelected(TreeEditor.EditMode.Parameter);
				break;
			case TreeEditor.PropertyType.FullUndo:
				this.UndoStoreSelected(TreeEditor.EditMode.Everything);
				break;
			case TreeEditor.PropertyType.FullUpdate:
				this.UndoStoreSelected(TreeEditor.EditMode.Parameter);
				this.m_WantCompleteUpdate = true;
				break;
			case TreeEditor.PropertyType.FullUndoUpdate:
				this.UndoStoreSelected(TreeEditor.EditMode.Everything);
				this.m_WantCompleteUpdate = true;
				break;
			}
		}
		private float GUISlider(TreeEditor.PropertyType prop, string contentID, float value, float minimum, float maximum, bool hasCurve)
		{
			this.GUIPropBegin();
			float num = EditorGUILayout.Slider(TreeEditorHelper.GetGUIContent(contentID), value, minimum, maximum, new GUILayoutOption[0]);
			if (num != value)
			{
				this.GUIHandlePropertyChange(prop);
			}
			if (!hasCurve)
			{
				this.GUIPropEnd();
			}
			return num;
		}
		private int GUIIntSlider(TreeEditor.PropertyType prop, string contentID, int value, int minimum, int maximum, bool hasCurve)
		{
			this.GUIPropBegin();
			int num = EditorGUILayout.IntSlider(TreeEditorHelper.GetGUIContent(contentID), value, minimum, maximum, new GUILayoutOption[0]);
			if (num != value)
			{
				this.GUIHandlePropertyChange(prop);
			}
			if (!hasCurve)
			{
				this.GUIPropEnd();
			}
			return num;
		}
		private bool GUIToggle(TreeEditor.PropertyType prop, string contentID, bool value, bool hasCurve)
		{
			this.GUIPropBegin();
			bool flag = EditorGUILayout.Toggle(TreeEditorHelper.GetGUIContent(contentID), value, new GUILayoutOption[0]);
			if (flag != value)
			{
				this.GUIHandlePropertyChange(prop);
				this.m_WantCompleteUpdate = true;
			}
			if (!hasCurve)
			{
				this.GUIPropEnd();
			}
			return flag;
		}
		private int GUIPopup(TreeEditor.PropertyType prop, string contentID, string optionsContentID, string[] optionIDs, int value, bool hasCurve)
		{
			this.GUIPropBegin();
			GUIContent[] array = new GUIContent[optionIDs.Length];
			for (int i = 0; i < optionIDs.Length; i++)
			{
				array[i] = TreeEditorHelper.GetGUIContent(optionsContentID + "." + optionIDs[i]);
			}
			int num = EditorGUILayout.Popup(TreeEditorHelper.GetGUIContent(contentID), value, array, new GUILayoutOption[0]);
			if (num != value)
			{
				this.GUIHandlePropertyChange(prop);
				this.m_WantCompleteUpdate = true;
			}
			if (!hasCurve)
			{
				this.GUIPropEnd();
			}
			return num;
		}
		private Material GUIMaterialField(TreeEditor.PropertyType prop, int uniqueNodeID, string contentID, Material value, TreeEditorHelper.NodeType nodeType)
		{
			string uniqueID = uniqueNodeID + "_" + contentID;
			this.GUIPropBegin();
			Material material = EditorGUILayout.ObjectField(TreeEditorHelper.GetGUIContent(contentID), value, typeof(Material), false, new GUILayoutOption[0]) as Material;
			this.GUIPropEnd();
			bool flag = this.m_TreeEditorHelper.GUIWrongShader(uniqueID, material, nodeType);
			if (material != value || flag)
			{
				this.GUIHandlePropertyChange(prop);
				this.m_WantCompleteUpdate = true;
			}
			return material;
		}
		private UnityEngine.Object GUIObjectField(TreeEditor.PropertyType prop, string contentID, UnityEngine.Object value, Type type, bool hasCurve)
		{
			this.GUIPropBegin();
			UnityEngine.Object @object = EditorGUILayout.ObjectField(TreeEditorHelper.GetGUIContent(contentID), value, type, false, new GUILayoutOption[0]);
			if (@object != value)
			{
				this.GUIHandlePropertyChange(prop);
				this.m_WantCompleteUpdate = true;
			}
			if (!hasCurve)
			{
				this.GUIPropEnd();
			}
			return @object;
		}
		private bool GUICurve(TreeEditor.PropertyType prop, AnimationCurve curve, Rect ranges)
		{
			bool changed = GUI.changed;
			EditorGUILayout.CurveField(curve, Color.green, ranges, new GUILayoutOption[]
			{
				GUILayout.Width(50f)
			});
			this.GUIPropEnd(false);
			if (changed != GUI.changed)
			{
				if (GUIUtility.hotControl == 0)
				{
					this.m_WantCompleteUpdate = true;
				}
				this.GUIHandlePropertyChange(prop);
				return true;
			}
			return false;
		}
		private Vector2 GUIMinMaxSlider(TreeEditor.PropertyType prop, string contentID, Vector2 value, float minimum, float maximum, bool hasCurve)
		{
			this.GUIPropBegin();
			Vector2 result = new Vector2(Mathf.Min(value.x, value.y), Mathf.Max(value.x, value.y));
			GUIContent gUIContent = TreeEditorHelper.GetGUIContent(contentID);
			bool changed = GUI.changed;
			Rect rect = GUILayoutUtility.GetRect(gUIContent, "Button");
			EditorGUI.MinMaxSlider(gUIContent, rect, ref result.x, ref result.y, minimum, maximum);
			if (changed != GUI.changed)
			{
				this.GUIHandlePropertyChange(prop);
			}
			if (!hasCurve)
			{
				this.GUIPropEnd();
			}
			return result;
		}
		public void InspectorHierachy(TreeData treeData, Renderer renderer)
		{
			if (TreeEditor.s_SelectedGroup == null)
			{
				Debug.Log("NO GROUP SELECTED!");
				return;
			}
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			bool changed = GUI.changed;
			Rect sizeRect = this.GUIPropBegin();
			this.DrawHierachy(treeData, renderer, sizeRect);
			if (GUI.changed != changed)
			{
				this.m_WantCompleteUpdate = true;
			}
			this.GUIPropEnd(false);
			this.GUIPropBegin();
			int num = -1;
			GUILayout.BeginHorizontal(TreeEditor.styles.toolbar, new GUILayoutOption[0]);
			if (GUILayout.Button(TreeEditor.styles.iconRefresh, TreeEditor.styles.toolbarButton, new GUILayoutOption[0]))
			{
				TreeGroupLeaf.s_TextureHullsDirty = true;
				TreeEditor.UpdateMesh(this.target as Tree);
			}
			GUILayout.FlexibleSpace();
			GUI.enabled = TreeEditor.s_SelectedGroup.CanHaveSubGroups();
			if (GUILayout.Button(TreeEditor.styles.iconAddLeaves, TreeEditor.styles.toolbarButton, new GUILayoutOption[0]))
			{
				num = 0;
			}
			if (GUILayout.Button(TreeEditor.styles.iconAddBranches, TreeEditor.styles.toolbarButton, new GUILayoutOption[0]))
			{
				num = 1;
			}
			GUI.enabled = true;
			if (TreeEditor.s_SelectedGroup == treeData.root)
			{
				GUI.enabled = false;
			}
			if (GUILayout.Button(TreeEditor.styles.iconDuplicate, TreeEditor.styles.toolbarButton, new GUILayoutOption[0]))
			{
				num = 3;
			}
			if (GUILayout.Button(TreeEditor.styles.iconTrash, TreeEditor.styles.toolbarButton, new GUILayoutOption[0]))
			{
				num = 2;
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			switch (num)
			{
			case 0:
			{
				this.UndoStoreSelected(TreeEditor.EditMode.CreateGroup);
				TreeGroup group = treeData.AddGroup(TreeEditor.s_SelectedGroup, typeof(TreeGroupLeaf));
				this.SelectGroup(group);
				this.m_WantCompleteUpdate = true;
				Event.current.Use();
				break;
			}
			case 1:
			{
				this.UndoStoreSelected(TreeEditor.EditMode.CreateGroup);
				TreeGroup group2 = treeData.AddGroup(TreeEditor.s_SelectedGroup, typeof(TreeGroupBranch));
				this.SelectGroup(group2);
				this.m_WantCompleteUpdate = true;
				Event.current.Use();
				break;
			}
			case 2:
				this.DeleteSelected(treeData);
				Event.current.Use();
				break;
			case 3:
				this.DuplicateSelected(treeData);
				Event.current.Use();
				break;
			}
			this.GUIPropEnd(false);
			GUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
		public void InspectorEditTools(Tree obj)
		{
			if (EditorUtility.IsPersistent(obj))
			{
				return;
			}
			string[] array;
			if (TreeEditor.s_SelectedGroup is TreeGroupBranch)
			{
				array = new string[]
				{
					"TreeEditor.BranchTranslate",
					"TreeEditor.BranchRotate",
					"TreeEditor.BranchFreeHand"
				};
			}
			else
			{
				array = new string[]
				{
					"TreeEditor.LeafTranslate",
					"TreeEditor.LeafRotate"
				};
				if (TreeEditor.editMode == TreeEditor.EditMode.Freehand)
				{
					TreeEditor.editMode = TreeEditor.EditMode.None;
				}
			}
			TreeEditor.EditMode editMode = TreeEditor.editMode;
			TreeEditor.editMode = (TreeEditor.EditMode)this.GUItoolbar((int)TreeEditor.editMode, TreeEditor.BuildToolbarContent(array, (int)TreeEditor.editMode));
			if (editMode != TreeEditor.editMode)
			{
				SceneView.RepaintAll();
			}
			EditorGUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			if (TreeEditor.editMode == TreeEditor.EditMode.None)
			{
				GUILayout.Label("No Tool Selected", new GUILayoutOption[0]);
				GUILayout.Label("Please select a tool", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
			}
			else
			{
				string uIString = TreeEditorHelper.GetUIString(array[(int)TreeEditor.editMode]);
				GUILayout.Label(TreeEditorHelper.ExtractLabel(uIString), new GUILayoutOption[0]);
				GUILayout.Label(TreeEditorHelper.ExtractTooltip(uIString), EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
		}
		private static GUIContent[] BuildToolbarContent(string[] contentStrings, int selection)
		{
			GUIContent[] array = new GUIContent[contentStrings.Length];
			for (int i = 0; i < contentStrings.Length; i++)
			{
				string str = (selection != i) ? string.Empty : " On";
				string tooltip = TreeEditorHelper.ExtractLabel(TreeEditorHelper.GetUIString(contentStrings[i]));
				array[i] = EditorGUIUtility.IconContent(contentStrings[i] + str, tooltip);
			}
			return array;
		}
		public void InspectorDistribution(TreeData treeData, TreeGroup group)
		{
			if (group == null)
			{
				return;
			}
			this.PrepareSpacing(true);
			bool enabled = true;
			if (group.lockFlags != 0)
			{
				enabled = false;
			}
			string str = "TreeEditor." + group.GetType().Name + ".";
			GUI.enabled = enabled;
			int num = group.seed;
			group.seed = this.GUIIntSlider(TreeEditor.PropertyType.Normal, str + "GroupSeed", group.seed, 0, 999999, false);
			if (group.seed != num)
			{
				treeData.UpdateSeed(group.uniqueID);
			}
			num = group.distributionFrequency;
			group.distributionFrequency = this.GUIIntSlider(TreeEditor.PropertyType.FullUndo, str + "Frequency", group.distributionFrequency, 1, 100, false);
			if (group.distributionFrequency != num)
			{
				treeData.UpdateFrequency(group.uniqueID);
			}
			string[] optionIDs = new string[]
			{
				"Random",
				"Alternate",
				"Opposite",
				"Whorled"
			};
			num = (int)group.distributionMode;
			group.distributionMode = (TreeGroup.DistributionMode)this.GUIPopup(TreeEditor.PropertyType.Normal, str + "DistributionMode", "TreeEditor.DistributionModeOption", optionIDs, (int)group.distributionMode, true);
			if (group.distributionMode != (TreeGroup.DistributionMode)num)
			{
				treeData.UpdateDistribution(group.uniqueID);
			}
			AnimationCurve animationCurve = group.distributionCurve;
			if (this.GUICurve(TreeEditor.PropertyType.Normal, animationCurve, this.m_CurveRangesA))
			{
				group.distributionCurve = animationCurve;
				treeData.UpdateDistribution(group.uniqueID);
			}
			if (group.distributionMode != TreeGroup.DistributionMode.Random)
			{
				float distributionTwirl = group.distributionTwirl;
				group.distributionTwirl = this.GUISlider(TreeEditor.PropertyType.Normal, str + "Twirl", group.distributionTwirl, -1f, 1f, false);
				if (group.distributionTwirl != distributionTwirl)
				{
					treeData.UpdateDistribution(group.uniqueID);
				}
			}
			if (group.distributionMode == TreeGroup.DistributionMode.Whorled)
			{
				num = group.distributionNodes;
				group.distributionNodes = this.GUIIntSlider(TreeEditor.PropertyType.Normal, str + "WhorledStep", group.distributionNodes, 1, 21, false);
				if (group.distributionNodes != num)
				{
					treeData.UpdateDistribution(group.uniqueID);
				}
			}
			group.distributionScale = this.GUISlider(TreeEditor.PropertyType.Normal, str + "GrowthScale", group.distributionScale, 0f, 1f, true);
			animationCurve = group.distributionScaleCurve;
			if (this.GUICurve(TreeEditor.PropertyType.Normal, animationCurve, this.m_CurveRangesA))
			{
				group.distributionScaleCurve = animationCurve;
			}
			group.distributionPitch = this.GUISlider(TreeEditor.PropertyType.Normal, str + "GrowthAngle", group.distributionPitch, 0f, 1f, true);
			animationCurve = group.distributionPitchCurve;
			if (this.GUICurve(TreeEditor.PropertyType.Normal, animationCurve, this.m_CurveRangesB))
			{
				group.distributionPitchCurve = animationCurve;
			}
			GUI.enabled = true;
			EditorGUILayout.Space();
		}
		public void InspectorAnimation(TreeData treeData, TreeGroup group)
		{
			if (group == null)
			{
				return;
			}
			this.PrepareSpacing(false);
			string str = "TreeEditor." + group.GetType().Name + ".";
			group.animationPrimary = this.GUISlider(TreeEditor.PropertyType.Normal, str + "MainWind", group.animationPrimary, 0f, 1f, false);
			if (treeData.GetGroup(group.parentGroupID) != treeData.root)
			{
				group.animationSecondary = this.GUISlider(TreeEditor.PropertyType.Normal, str + "MainTurbulence", group.animationSecondary, 0f, 1f, false);
			}
			GUI.enabled = true;
			if (!(group is TreeGroupBranch) || (group as TreeGroupBranch).geometryMode != TreeGroupBranch.GeometryMode.Branch)
			{
				group.animationEdge = this.GUISlider(TreeEditor.PropertyType.Normal, str + "EdgeTurbulence", group.animationEdge, 0f, 1f, false);
			}
			this.GUIPropBegin();
			if (GUILayout.Button(TreeEditorHelper.GetGUIContent("TreeEditor.WindZone.Create"), new GUILayoutOption[0]))
			{
				TreeEditor.CreateDefaultWindZone();
			}
			this.GUIPropEnd();
		}
		private int GUItoolbar(int selection, GUIContent[] names)
		{
			GUI.enabled = true;
			bool changed = GUI.changed;
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			for (int i = 0; i < names.Length; i++)
			{
				GUIStyle style = new GUIStyle("ButtonMid");
				if (i == 0)
				{
					style = new GUIStyle("ButtonLeft");
				}
				if (i == names.Length - 1)
				{
					style = new GUIStyle("ButtonRight");
				}
				if (names[i] != null && GUILayout.Toggle(selection == i, names[i], style, new GUILayoutOption[0]))
				{
					selection = i;
				}
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUI.changed = changed;
			return selection;
		}
		private void GUIunlockbox(TreeData treeData)
		{
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			GUIContent gUIContent = TreeEditorHelper.GetGUIContent("TreeEditor.EditingTools.WarningLabel");
			gUIContent.image = TreeEditor.styles.warningIcon.image;
			GUILayout.Label(gUIContent, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
			GUIStyle gUIStyle = new GUIStyle("minibutton");
			gUIStyle.wordWrap = true;
			GUIContent gUIContent2 = TreeEditorHelper.GetGUIContent("TreeEditor.EditingTools.WarningButton");
			if (GUILayout.Button(gUIContent2, gUIStyle, new GUILayoutOption[0]))
			{
				treeData.UnlockGroup(TreeEditor.s_SelectedGroup);
				this.m_WantCompleteUpdate = true;
			}
			GUILayout.EndVertical();
		}
		private void PrepareSpacing(bool hasCurves)
		{
			this.m_SectionHasCurves = hasCurves;
			EditorGUIUtility.labelWidth = (float)((!hasCurves) ? 120 : 100);
		}
		private bool GUIMaterialColor(Material material, string propertyID, string contentID)
		{
			bool result = false;
			this.GUIPropBegin();
			Color color = material.GetColor(propertyID);
			Color color2 = EditorGUILayout.ColorField(TreeEditorHelper.GetGUIContent(contentID), color, new GUILayoutOption[0]);
			if (color2 != color)
			{
				Undo.RegisterCompleteObjectUndo(material, "Material");
				material.SetColor(propertyID, color2);
				result = true;
			}
			this.GUIPropEnd();
			return result;
		}
		private bool GUIMaterialSlider(Material material, string propertyID, string contentID)
		{
			bool result = false;
			this.GUIPropBegin();
			float @float = material.GetFloat(propertyID);
			float num = EditorGUILayout.Slider(TreeEditorHelper.GetGUIContent(contentID), @float, 0f, 1f, new GUILayoutOption[0]);
			if (num != @float)
			{
				Undo.RegisterCompleteObjectUndo(material, "Material");
				material.SetFloat(propertyID, num);
				result = true;
			}
			this.GUIPropEnd();
			return result;
		}
		private bool GUIMaterialFloatField(Material material, string propertyID, string contentID)
		{
			bool flag;
			float materialFloat = TreeEditor.GetMaterialFloat(material, propertyID, out flag);
			if (!flag)
			{
				return false;
			}
			bool result = false;
			this.GUIPropBegin();
			float num = EditorGUILayout.FloatField(TreeEditorHelper.GetGUIContent(contentID), materialFloat, new GUILayoutOption[0]);
			if (num != materialFloat)
			{
				Undo.RegisterCompleteObjectUndo(material, "Material");
				material.SetFloat(propertyID, num);
				result = true;
			}
			this.GUIPropEnd();
			return result;
		}
		private static float GetMaterialFloat(Material material, string propertyID, out bool success)
		{
			success = false;
			if (!material.HasProperty(propertyID))
			{
				return 0f;
			}
			success = true;
			return material.GetFloat(propertyID);
		}
		private static float GetMaterialFloat(Material material, string propertyID)
		{
			bool flag;
			return TreeEditor.GetMaterialFloat(material, propertyID, out flag);
		}
		private static float GenerateMaterialHash(Material material)
		{
			float num = 0f;
			Color color = material.GetColor("_TranslucencyColor");
			num += color.r + color.g + color.b + color.a;
			num += TreeEditor.GetMaterialFloat(material, "_Cutoff");
			num += TreeEditor.GetMaterialFloat(material, "_TranslucencyViewDependency");
			num += TreeEditor.GetMaterialFloat(material, "_ShadowStrength");
			return num + TreeEditor.GetMaterialFloat(material, "_ShadowOffsetScale");
		}
		public void InspectorRoot(TreeData treeData, TreeGroupRoot group)
		{
			GUIContent[] expr_06 = new GUIContent[6];
			expr_06[0] = TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupRoot.Distribution");
			expr_06[1] = TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupRoot.Geometry");
			expr_06[2] = TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupRoot.MaterialProperties");
			GUIContent[] names = expr_06;
			bool enabled = GUI.enabled;
			TreeEditor.BeginSettingsSection(0, names);
			this.PrepareSpacing(false);
			int seed = group.seed;
			group.seed = this.GUIIntSlider(TreeEditor.PropertyType.Normal, "TreeEditor.TreeGroupRoot.GroupSeed", group.seed, 0, 9999999, false);
			if (group.seed != seed)
			{
				treeData.UpdateSeed(group.uniqueID);
			}
			group.rootSpread = this.GUISlider(TreeEditor.PropertyType.Normal, "TreeEditor.TreeGroupRoot.AreaSpread", group.rootSpread, 0f, 10f, false);
			group.groundOffset = this.GUISlider(TreeEditor.PropertyType.Normal, "TreeEditor.TreeGroupRoot.GroundOffset", group.groundOffset, 0f, 10f, false);
			TreeEditor.EndSettingsSection();
			TreeEditor.BeginSettingsSection(1, names);
			this.PrepareSpacing(false);
			group.adaptiveLODQuality = this.GUISlider(TreeEditor.PropertyType.FullUndo, "TreeEditor.TreeGroupRoot.LODQuality", group.adaptiveLODQuality, 0f, 1f, false);
			group.enableAmbientOcclusion = this.GUIToggle(TreeEditor.PropertyType.FullUndo, "TreeEditor.TreeGroupRoot.AmbientOcclusion", group.enableAmbientOcclusion, false);
			GUI.enabled = group.enableAmbientOcclusion;
			group.aoDensity = this.GUISlider(TreeEditor.PropertyType.Normal, "TreeEditor.TreeGroupRoot.AODensity", group.aoDensity, 0f, 1f, false);
			GUI.enabled = true;
			TreeEditor.EndSettingsSection();
			Material optimizedCutoutMaterial = treeData.optimizedCutoutMaterial;
			if (optimizedCutoutMaterial != null)
			{
				TreeEditor.BeginSettingsSection(2, names);
				this.PrepareSpacing(false);
				bool changed = GUI.changed;
				bool flag = this.GUIMaterialColor(optimizedCutoutMaterial, "_TranslucencyColor", "TreeEditor.TreeGroupRoot.TranslucencyColor");
				flag |= this.GUIMaterialSlider(optimizedCutoutMaterial, "_TranslucencyViewDependency", "TreeEditor.TreeGroupRoot.TranslucencyViewDependency");
				flag |= this.GUIMaterialSlider(optimizedCutoutMaterial, "_Cutoff", "TreeEditor.TreeGroupRoot.AlphaCutoff");
				flag |= this.GUIMaterialSlider(optimizedCutoutMaterial, "_ShadowStrength", "TreeEditor.TreeGroupRoot.ShadowStrength");
				flag |= this.GUIMaterialFloatField(optimizedCutoutMaterial, "_ShadowOffsetScale", "TreeEditor.TreeGroupRoot.ShadowOffsetScale");
				if (flag)
				{
					TreeEditor.s_CutoutMaterialHashBeforeUndo = TreeEditor.GenerateMaterialHash(treeData.optimizedCutoutMaterial);
				}
				string[] optionIDs = new string[]
				{
					"Full",
					"Half",
					"Quarter",
					"OneEighth",
					"OneSixteenth"
				};
				group.shadowTextureQuality = this.GUIPopup(TreeEditor.PropertyType.FullUpdate, "TreeEditor.TreeGroupRoot.ShadowTextureQuality", "TreeEditor.TreeGroupRoot.ShadowTextureQualityOption", optionIDs, group.shadowTextureQuality, false);
				GUI.changed = changed;
				TreeEditor.EndSettingsSection();
			}
			GUI.enabled = enabled;
			EditorGUILayout.Space();
		}
		public void InspectorBranch(TreeData treeData, TreeGroupBranch group)
		{
			this.InspectorEditTools(this.target as Tree);
			GUIContent[] names = new GUIContent[]
			{
				TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupBranch.Distribution"),
				TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupBranch.Geometry"),
				TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupBranch.Shape"),
				TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupBranch.Fronds"),
				TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupBranch.Animation")
			};
			string str = "TreeEditor.TreeGroupBranch.";
			bool enabled = GUI.enabled;
			if (TreeEditor.s_SelectedGroup.lockFlags != 0)
			{
				this.GUIunlockbox(treeData);
			}
			TreeEditor.BeginSettingsSection(0, names);
			this.InspectorDistribution(treeData, group);
			TreeEditor.EndSettingsSection();
			TreeEditor.BeginSettingsSection(1, names);
			this.PrepareSpacing(false);
			group.lodQualityMultiplier = this.GUISlider(TreeEditor.PropertyType.Normal, str + "LODQuality", group.lodQualityMultiplier, 0f, 2f, false);
			string[] optionIDs = new string[]
			{
				"BranchOnly",
				"BranchAndFronds",
				"FrondsOnly"
			};
			group.geometryMode = (TreeGroupBranch.GeometryMode)this.GUIPopup(TreeEditor.PropertyType.FullUpdate, str + "GeometryMode", str + "GeometryModeOption", optionIDs, (int)group.geometryMode, false);
			if (group.geometryMode != TreeGroupBranch.GeometryMode.Frond)
			{
				group.materialBranch = this.GUIMaterialField(TreeEditor.PropertyType.FullUpdate, group.uniqueID, str + "BranchMaterial", group.materialBranch, TreeEditorHelper.NodeType.BarkNode);
			}
			group.materialBreak = this.GUIMaterialField(TreeEditor.PropertyType.FullUpdate, group.uniqueID, str + "BreakMaterial", group.materialBreak, TreeEditorHelper.NodeType.BarkNode);
			if (group.geometryMode != TreeGroupBranch.GeometryMode.Branch)
			{
				group.materialFrond = this.GUIMaterialField(TreeEditor.PropertyType.FullUpdate, group.uniqueID, str + "FrondMaterial", group.materialFrond, TreeEditorHelper.NodeType.BarkNode);
			}
			TreeEditor.EndSettingsSection();
			TreeEditor.BeginSettingsSection(2, names);
			this.PrepareSpacing(true);
			GUI.enabled = (group.lockFlags == 0);
			group.height = this.GUIMinMaxSlider(TreeEditor.PropertyType.Normal, str + "Length", group.height, 0.1f, 50f, false);
			GUI.enabled = (group.geometryMode != TreeGroupBranch.GeometryMode.Frond);
			group.radiusMode = this.GUIToggle(TreeEditor.PropertyType.Normal, str + "IsLengthRelative", group.radiusMode, false);
			GUI.enabled = (group.geometryMode != TreeGroupBranch.GeometryMode.Frond);
			group.radius = this.GUISlider(TreeEditor.PropertyType.Normal, str + "Radius", group.radius, 0.1f, 5f, true);
			AnimationCurve animationCurve = group.radiusCurve;
			if (this.GUICurve(TreeEditor.PropertyType.Normal, animationCurve, this.m_CurveRangesA))
			{
				group.radiusCurve = animationCurve;
			}
			GUI.enabled = (group.geometryMode != TreeGroupBranch.GeometryMode.Frond);
			group.capSmoothing = this.GUISlider(TreeEditor.PropertyType.Normal, str + "CapSmoothing", group.capSmoothing, 0f, 1f, false);
			GUI.enabled = true;
			EditorGUILayout.Space();
			GUI.enabled = (group.lockFlags == 0);
			group.crinklyness = this.GUISlider(TreeEditor.PropertyType.Normal, str + "Crinklyness", group.crinklyness, 0f, 1f, true);
			animationCurve = group.crinkCurve;
			if (this.GUICurve(TreeEditor.PropertyType.Normal, animationCurve, this.m_CurveRangesA))
			{
				group.crinkCurve = animationCurve;
			}
			GUI.enabled = (group.lockFlags == 0);
			group.seekBlend = this.GUISlider(TreeEditor.PropertyType.Normal, str + "SeekSunGround", group.seekBlend, 0f, 1f, true);
			animationCurve = group.seekCurve;
			if (this.GUICurve(TreeEditor.PropertyType.Normal, animationCurve, this.m_CurveRangesB))
			{
				group.seekCurve = animationCurve;
			}
			GUI.enabled = true;
			EditorGUILayout.Space();
			GUI.enabled = (group.geometryMode != TreeGroupBranch.GeometryMode.Frond);
			group.noise = this.GUISlider(TreeEditor.PropertyType.Normal, str + "Noise", group.noise, 0f, 1f, true);
			animationCurve = group.noiseCurve;
			if (this.GUICurve(TreeEditor.PropertyType.Normal, animationCurve, this.m_CurveRangesA))
			{
				group.noiseCurve = animationCurve;
			}
			group.noiseScaleU = this.GUISlider(TreeEditor.PropertyType.Normal, str + "NoiseScaleU", group.noiseScaleU, 0f, 1f, false);
			group.noiseScaleV = this.GUISlider(TreeEditor.PropertyType.Normal, str + "NoiseScaleV", group.noiseScaleV, 0f, 1f, false);
			EditorGUILayout.Space();
			GUI.enabled = (group.geometryMode != TreeGroupBranch.GeometryMode.Frond);
			if (treeData.GetGroup(group.parentGroupID) == treeData.root)
			{
				group.flareSize = this.GUISlider(TreeEditor.PropertyType.Normal, str + "FlareRadius", group.flareSize, 0f, 5f, false);
				group.flareHeight = this.GUISlider(TreeEditor.PropertyType.Normal, str + "FlareHeight", group.flareHeight, 0f, 1f, false);
				group.flareNoise = this.GUISlider(TreeEditor.PropertyType.Normal, str + "FlareNoise", group.flareNoise, 0f, 1f, false);
			}
			else
			{
				group.weldHeight = this.GUISlider(TreeEditor.PropertyType.Normal, str + "WeldHeight", group.weldHeight, 0.01f, 1f, false);
				group.weldSpreadTop = this.GUISlider(TreeEditor.PropertyType.Normal, str + "WeldSpreadTop", group.weldSpreadTop, 0f, 1f, false);
				group.weldSpreadBottom = this.GUISlider(TreeEditor.PropertyType.Normal, str + "WeldSpreadBottom", group.weldSpreadBottom, 0f, 1f, false);
			}
			EditorGUILayout.Space();
			group.breakingChance = this.GUISlider(TreeEditor.PropertyType.Normal, str + "BreakChance", group.breakingChance, 0f, 1f, false);
			group.breakingSpot = this.GUIMinMaxSlider(TreeEditor.PropertyType.Normal, str + "BreakLocation", group.breakingSpot, 0f, 1f, false);
			TreeEditor.EndSettingsSection();
			if (group.geometryMode != TreeGroupBranch.GeometryMode.Branch)
			{
				TreeEditor.BeginSettingsSection(3, names);
				this.PrepareSpacing(true);
				group.frondCount = this.GUIIntSlider(TreeEditor.PropertyType.Normal, str + "FrondCount", group.frondCount, 1, 16, false);
				group.frondWidth = this.GUISlider(TreeEditor.PropertyType.Normal, str + "FrondWidth", group.frondWidth, 0.1f, 10f, true);
				animationCurve = group.frondCurve;
				if (this.GUICurve(TreeEditor.PropertyType.Normal, animationCurve, this.m_CurveRangesA))
				{
					group.frondCurve = animationCurve;
				}
				group.frondRange = this.GUIMinMaxSlider(TreeEditor.PropertyType.Normal, str + "FrondRange", group.frondRange, 0f, 1f, false);
				group.frondRotation = this.GUISlider(TreeEditor.PropertyType.Normal, str + "FrondRotation", group.frondRotation, 0f, 1f, false);
				group.frondCrease = this.GUISlider(TreeEditor.PropertyType.Normal, str + "FrondCrease", group.frondCrease, -1f, 1f, false);
				GUI.enabled = true;
				TreeEditor.EndSettingsSection();
			}
			TreeEditor.BeginSettingsSection(4, names);
			this.InspectorAnimation(treeData, group);
			TreeEditor.EndSettingsSection();
			GUI.enabled = enabled;
			EditorGUILayout.Space();
		}
		public void InspectorLeaf(TreeData treeData, TreeGroupLeaf group)
		{
			this.InspectorEditTools(this.target as Tree);
			GUIContent[] names = new GUIContent[]
			{
				TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupLeaf.Distribution"),
				TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupLeaf.Geometry"),
				TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupLeaf.Shape"),
				null,
				null,
				TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroupLeaf.Animation")
			};
			string str = "TreeEditor.TreeGroupLeaf.";
			bool enabled = GUI.enabled;
			if (TreeEditor.s_SelectedGroup.lockFlags != 0)
			{
				this.GUIunlockbox(treeData);
			}
			TreeEditor.BeginSettingsSection(0, names);
			this.InspectorDistribution(treeData, group);
			TreeEditor.EndSettingsSection();
			TreeEditor.BeginSettingsSection(1, names);
			this.PrepareSpacing(false);
			string[] optionIDs = new string[]
			{
				"Plane",
				"Cross",
				"TriCross",
				"Billboard",
				"Mesh"
			};
			group.geometryMode = this.GUIPopup(TreeEditor.PropertyType.FullUpdate, str + "GeometryMode", str + "GeometryModeOption", optionIDs, group.geometryMode, false);
			if (group.geometryMode != 4)
			{
				group.materialLeaf = this.GUIMaterialField(TreeEditor.PropertyType.FullUpdate, group.uniqueID, str + "Material", group.materialLeaf, TreeEditorHelper.NodeType.LeafNode);
			}
			if (group.geometryMode == 4)
			{
				group.instanceMesh = (this.GUIObjectField(TreeEditor.PropertyType.FullUpdate, str + "Mesh", group.instanceMesh, typeof(GameObject), false) as GameObject);
			}
			TreeEditor.EndSettingsSection();
			TreeEditor.BeginSettingsSection(2, names);
			this.PrepareSpacing(false);
			group.size = this.GUIMinMaxSlider(TreeEditor.PropertyType.Normal, str + "Size", group.size, 0.1f, 2f, false);
			group.perpendicularAlign = this.GUISlider(TreeEditor.PropertyType.Normal, str + "PerpendicularAlign", group.perpendicularAlign, 0f, 1f, false);
			group.horizontalAlign = this.GUISlider(TreeEditor.PropertyType.Normal, str + "HorizontalAlign", group.horizontalAlign, 0f, 1f, false);
			TreeEditor.EndSettingsSection();
			TreeEditor.BeginSettingsSection(5, names);
			this.PrepareSpacing(false);
			this.InspectorAnimation(treeData, group);
			TreeEditor.EndSettingsSection();
			GUI.enabled = enabled;
			EditorGUILayout.Space();
		}
		public override bool UseDefaultMargins()
		{
			return false;
		}
		public override void OnInspectorGUI()
		{
			Tree tree = this.target as Tree;
			TreeData treeData = TreeEditor.GetTreeData(tree);
			if (!treeData)
			{
				return;
			}
			Renderer component = tree.GetComponent<Renderer>();
			this.VerifySelection(treeData);
			if (TreeEditor.s_SelectedGroup == null)
			{
				return;
			}
			this.m_WantCompleteUpdate = false;
			EventType typeForControl = Event.current.GetTypeForControl(GUIUtility.hotControl);
			if ((typeForControl == EventType.MouseUp || (typeForControl == EventType.KeyUp && Event.current.keyCode == KeyCode.Return) || (typeForControl == EventType.ExecuteCommand && Event.current.commandName == "OnLostFocus")) && treeData.isInPreviewMode)
			{
				this.m_WantCompleteUpdate = true;
			}
			if (this.OnCheckHotkeys(treeData, true))
			{
				return;
			}
			this.m_TreeEditorHelper.RefreshAllTreeShaders();
			if (this.m_TreeEditorHelper.GUITooManyShaders())
			{
				this.m_WantCompleteUpdate = true;
			}
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins, new GUILayoutOption[0]);
			this.InspectorHierachy(treeData, component);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
			if (TreeEditor.s_SelectedGroup != null)
			{
				if (TreeEditor.s_SelectedGroup.GetType() == typeof(TreeGroupBranch))
				{
					this.InspectorBranch(treeData, (TreeGroupBranch)TreeEditor.s_SelectedGroup);
				}
				else
				{
					if (TreeEditor.s_SelectedGroup.GetType() == typeof(TreeGroupLeaf))
					{
						this.InspectorLeaf(treeData, (TreeGroupLeaf)TreeEditor.s_SelectedGroup);
					}
					else
					{
						if (TreeEditor.s_SelectedGroup.GetType() == typeof(TreeGroupRoot))
						{
							this.InspectorRoot(treeData, (TreeGroupRoot)TreeEditor.s_SelectedGroup);
						}
					}
				}
			}
			EditorGUILayout.EndVertical();
			if (this.m_WantedCompleteUpdateInPreviousFrame)
			{
				this.m_WantCompleteUpdate = true;
			}
			this.m_WantedCompleteUpdateInPreviousFrame = false;
			if (this.m_WantCompleteUpdate)
			{
				GUI.changed = true;
			}
			if (GUI.changed)
			{
				if (!this.m_TreeEditorHelper.AreShadersCorrect())
				{
					this.m_WantedCompleteUpdateInPreviousFrame = this.m_WantCompleteUpdate;
					return;
				}
				if (this.m_WantCompleteUpdate)
				{
					TreeEditor.UpdateMesh(tree);
					this.m_WantCompleteUpdate = false;
				}
				else
				{
					TreeEditor.PreviewMesh(tree);
				}
			}
		}
		private void DrawHierachy(TreeData treeData, Renderer renderer, Rect sizeRect)
		{
			if (TreeEditor.styles == null)
			{
				TreeEditor.styles = new TreeEditor.Styles();
			}
			this.hierachySpread = this.hierachyNodeSize + this.hierachyNodeSpace;
			this.hierachyView = sizeRect;
			Event @event = new Event(Event.current);
			List<TreeEditor.HierachyNode> nodes = new List<TreeEditor.HierachyNode>();
			this.BuildHierachyNodes(treeData, nodes, treeData.root, 0);
			this.LayoutHierachyNodes(nodes, sizeRect);
			float num = 16f;
			Vector2 zero = Vector2.zero;
			if (sizeRect.width < this.hierachyRect.width)
			{
				zero.y -= 16f;
			}
			bool changed = GUI.changed;
			this.hierachyDisplayRect = GUILayoutUtility.GetRect(sizeRect.width, this.hierachyRect.height + num);
			this.hierachyDisplayRect.width = sizeRect.width;
			GUI.Box(this.hierachyDisplayRect, GUIContent.none, TreeEditor.styles.nodeBackground);
			this.hierachyScroll = GUI.BeginScrollView(this.hierachyDisplayRect, this.hierachyScroll, this.hierachyRect, false, false);
			GUI.changed = changed;
			this.HandleDragHierachyNodes(treeData, nodes);
			this.DrawHierachyNodes(treeData, nodes, treeData.root, zero / 2f, 1f, 1f);
			if (this.dragNode != null && this.isDragging)
			{
				Vector2 a = Event.current.mousePosition - this.dragClickPos;
				this.DrawHierachyNodes(treeData, nodes, this.dragNode.group, a + zero / 2f, 0.5f, 0.5f);
			}
			GUI.EndScrollView();
			MeshFilter component = renderer.GetComponent<MeshFilter>();
			if (component && component.sharedMesh && renderer)
			{
				int num2 = component.sharedMesh.vertices.Length;
				int num3 = component.sharedMesh.triangles.Length / 3;
				int num4 = renderer.sharedMaterials.Length;
				Rect position = new Rect(this.hierachyDisplayRect.xMax - 80f - 4f, this.hierachyDisplayRect.yMax + zero.y - 40f - 4f, 80f, 40f);
				string text = TreeEditorHelper.GetGUIContent("TreeEditor.Hierachy.Stats").text;
				text = text.Replace("[v]", num2.ToString());
				text = text.Replace("[t]", num3.ToString());
				text = text.Replace("[m]", num4.ToString());
				text = text.Replace(" / ", "\n");
				GUI.Label(position, text, EditorStyles.helpBox);
			}
			if (@event.type == EventType.ScrollWheel && Event.current.type == EventType.Used)
			{
				Event.current = @event;
			}
		}
		private void BuildHierachyNodes(TreeData treeData, List<TreeEditor.HierachyNode> nodes, TreeGroup group, int depth)
		{
			nodes.Add(new TreeEditor.HierachyNode
			{
				group = group,
				pos = new Vector3(0f, (float)depth * this.hierachySpread.y, 0f)
			});
			for (int i = 0; i < group.childGroupIDs.Length; i++)
			{
				TreeGroup group2 = treeData.GetGroup(group.childGroupIDs[i]);
				this.BuildHierachyNodes(treeData, nodes, group2, depth - 1);
			}
		}
		private void LayoutHierachyNodes(List<TreeEditor.HierachyNode> nodes, Rect sizeRect)
		{
			Bounds bounds = default(Bounds);
			for (int i = 0; i < nodes.Count; i++)
			{
				for (int j = i + 1; j < nodes.Count; j++)
				{
					if (nodes[i].pos.y == nodes[j].pos.y)
					{
						TreeEditor.HierachyNode expr_4B_cp_0 = nodes[i];
						expr_4B_cp_0.pos.x = expr_4B_cp_0.pos.x - this.hierachySpread.x * 0.5f;
						nodes[j].pos.x = nodes[i].pos.x + this.hierachySpread.x;
					}
				}
				bounds.Encapsulate(nodes[i].pos);
			}
			bounds.Expand(this.hierachySpread);
			this.hierachyRect = new Rect(0f, 0f, bounds.size.x, bounds.size.y);
			this.hierachyRect.width = Mathf.Max(this.hierachyRect.width, this.hierachyView.width);
			Vector3 vector = new Vector3((this.hierachyRect.xMax + this.hierachyRect.xMin) * 0.5f, (this.hierachyRect.yMax + this.hierachyRect.yMin) * 0.5f, 0f);
			vector.y += 8f;
			for (int k = 0; k < nodes.Count; k++)
			{
				nodes[k].pos -= bounds.center;
				TreeEditor.HierachyNode expr_1C3_cp_0 = nodes[k];
				expr_1C3_cp_0.pos.x = expr_1C3_cp_0.pos.x + vector.x;
				TreeEditor.HierachyNode expr_1E3_cp_0 = nodes[k];
				expr_1E3_cp_0.pos.y = expr_1E3_cp_0.pos.y + vector.y;
				nodes[k].rect = new Rect(nodes[k].pos.x - this.hierachyNodeSize.x * 0.5f, nodes[k].pos.y - this.hierachyNodeSize.y * 0.5f, this.hierachyNodeSize.x, this.hierachyNodeSize.y);
			}
		}
		private void HandleDragHierachyNodes(TreeData treeData, List<TreeEditor.HierachyNode> nodes)
		{
			if (this.dragNode == null)
			{
				this.isDragging = false;
				this.dropNode = null;
			}
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			EventType typeForControl = Event.current.GetTypeForControl(controlID);
			if (typeForControl == EventType.MouseDown && Event.current.button == 0)
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					if (nodes[i].rect.Contains(Event.current.mousePosition))
					{
						if (!this.GetHierachyNodeVisRect(nodes[i].rect).Contains(Event.current.mousePosition))
						{
							if (!(nodes[i].group is TreeGroupRoot))
							{
								this.dragClickPos = Event.current.mousePosition;
								this.dragNode = nodes[i];
								GUIUtility.hotControl = controlID;
								Event.current.Use();
								break;
							}
						}
					}
				}
			}
			if (this.dragNode != null)
			{
				this.dropNode = null;
				for (int j = 0; j < nodes.Count; j++)
				{
					if (nodes[j].rect.Contains(Event.current.mousePosition))
					{
						TreeGroup group = this.dragNode.group;
						TreeGroup group2 = nodes[j].group;
						if (group2 != group)
						{
							if (group2.CanHaveSubGroups())
							{
								if (treeData.GetGroup(group.parentGroupID) != group2)
								{
									if (!treeData.IsAncestor(group, group2))
									{
										this.dropNode = nodes[j];
										break;
									}
								}
							}
						}
					}
				}
				if (typeForControl == EventType.MouseMove || typeForControl == EventType.MouseDrag)
				{
					if ((this.dragClickPos - Event.current.mousePosition).magnitude > 10f)
					{
						this.isDragging = true;
					}
					Event.current.Use();
				}
				else
				{
					if (typeForControl == EventType.MouseUp && GUIUtility.hotControl == controlID)
					{
						if (this.dropNode != null)
						{
							this.UndoStoreSelected(TreeEditor.EditMode.Everything);
							TreeGroup group3 = this.dragNode.group;
							TreeGroup group4 = this.dropNode.group;
							treeData.SetGroupParent(group3, group4);
							this.m_WantCompleteUpdate = true;
						}
						else
						{
							base.Repaint();
						}
						this.dragNode = null;
						this.dropNode = null;
						GUIUtility.hotControl = 0;
						Event.current.Use();
					}
				}
			}
		}
		private Rect GetHierachyNodeVisRect(Rect rect)
		{
			return new Rect(rect.x + rect.width - 13f - 1f, rect.y + 11f, 13f, 11f);
		}
		private void DrawHierachyNodes(TreeData treeData, List<TreeEditor.HierachyNode> nodes, TreeGroup group, Vector2 offset, float alpha, float fade)
		{
			if (this.dragNode != null && this.isDragging && this.dragNode.group == group)
			{
				alpha = 0.5f;
				fade = 0.75f;
			}
			Vector3 b = new Vector3(0f, this.hierachyNodeSize.y * 0.5f, 0f);
			Vector3 b2 = new Vector3(offset.x, offset.y);
			Handles.color = new Color(0f, 0f, 0f, 0.5f * alpha);
			if (EditorGUIUtility.isProSkin)
			{
				Handles.color = new Color(0.4f, 0.4f, 0.4f, 0.5f * alpha);
			}
			TreeEditor.HierachyNode hierachyNode = null;
			for (int i = 0; i < nodes.Count; i++)
			{
				if (group == nodes[i].group)
				{
					hierachyNode = nodes[i];
					break;
				}
			}
			if (hierachyNode == null)
			{
				return;
			}
			for (int j = 0; j < group.childGroupIDs.Length; j++)
			{
				TreeGroup group2 = treeData.GetGroup(group.childGroupIDs[j]);
				for (int k = 0; k < nodes.Count; k++)
				{
					if (nodes[k].group == group2)
					{
						Handles.DrawLine(hierachyNode.pos + b2 - b, nodes[k].pos + b2 + b);
					}
				}
			}
			Rect rect = hierachyNode.rect;
			rect.x += offset.x;
			rect.y += offset.y;
			int num = 0;
			if (hierachyNode == this.dropNode)
			{
				num = 1;
			}
			else
			{
				if (TreeEditor.s_SelectedGroup == hierachyNode.group)
				{
					if (TreeEditor.s_SelectedNode != null)
					{
						num = 1;
					}
					else
					{
						num = 1;
					}
				}
			}
			GUI.backgroundColor = new Color(1f, 1f, 1f, alpha);
			GUI.contentColor = new Color(1f, 1f, 1f, alpha);
			GUI.Label(rect, GUIContent.none, TreeEditor.styles.nodeBoxes[num]);
			Rect position = new Rect(rect.x + rect.width / 2f - 4f, rect.y - 2f, 0f, 0f);
			Rect position2 = new Rect(rect.x + rect.width / 2f - 4f, rect.y + rect.height - 2f, 0f, 0f);
			Rect position3 = new Rect(rect.x + 1f, rect.yMax - 36f, 32f, 32f);
			Rect position4 = new Rect(rect.xMax - 18f, rect.yMax - 18f, 16f, 16f);
			Rect position5 = new Rect(rect.x, rect.y, rect.width - 2f, 16f);
			bool flag = true;
			int num2 = 0;
			GUIContent gUIContent = new GUIContent();
			Type type = group.GetType();
			if (type == typeof(TreeGroupBranch))
			{
				gUIContent = TreeEditorHelper.GetGUIContent("TreeEditor.Hierachy.TreeGroupBranch");
				TreeGroupBranch treeGroupBranch = (TreeGroupBranch)group;
				switch (treeGroupBranch.geometryMode)
				{
				case TreeGroupBranch.GeometryMode.Branch:
					num2 = 1;
					break;
				case TreeGroupBranch.GeometryMode.BranchFrond:
					num2 = 0;
					break;
				case TreeGroupBranch.GeometryMode.Frond:
					num2 = 2;
					break;
				}
			}
			else
			{
				if (type == typeof(TreeGroupLeaf))
				{
					gUIContent = TreeEditorHelper.GetGUIContent("TreeEditor.Hierachy.TreeGroupLeaf");
					num2 = 3;
				}
				else
				{
					if (type == typeof(TreeGroupRoot))
					{
						gUIContent = TreeEditorHelper.GetGUIContent("TreeEditor.Hierachy.TreeGroupRoot");
						num2 = 4;
						flag = false;
					}
				}
			}
			if (flag)
			{
				Rect hierachyNodeVisRect = this.GetHierachyNodeVisRect(rect);
				GUIContent gUIContent2 = TreeEditorHelper.GetGUIContent("TreeEditor.Hierachy.ShowHide");
				gUIContent2.image = TreeEditor.styles.visibilityIcons[(!group.visible) ? 1 : 0].image;
				GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
				if (GUI.Button(hierachyNodeVisRect, gUIContent2, GUIStyle.none))
				{
					group.visible = !group.visible;
					GUI.changed = true;
				}
				GUI.contentColor = Color.white;
			}
			gUIContent.image = TreeEditor.styles.nodeIcons[num2].image;
			GUI.contentColor = new Color(1f, 1f, 1f, (!group.visible) ? 0.5f : 1f);
			if (GUI.Button(position3, gUIContent, GUIStyle.none) || this.dragNode == hierachyNode)
			{
				TreeGroup treeGroup = TreeEditor.s_SelectedGroup;
				this.SelectGroup(group);
				if (treeGroup == TreeEditor.s_SelectedGroup)
				{
					Tree tree = this.target as Tree;
					this.FrameSelected(tree);
				}
			}
			GUI.contentColor = Color.white;
			if (group.CanHaveSubGroups())
			{
				GUI.Label(position, GUIContent.none, TreeEditor.styles.pinLabel);
			}
			if (flag)
			{
				GUIContent gUIContent3 = TreeEditorHelper.GetGUIContent("TreeEditor.Hierachy.NodeCount");
				gUIContent3.text = group.nodeIDs.Length.ToString();
				GUI.Label(position5, gUIContent3, TreeEditor.styles.nodeLabelTop);
				if (this.m_TreeEditorHelper.NodeHasWrongMaterial(group))
				{
					GUI.DrawTexture(position4, ConsoleWindow.iconErrorSmall);
				}
				else
				{
					if (group.lockFlags != 0)
					{
						GUI.DrawTexture(position4, TreeEditor.styles.warningIcon.image);
					}
				}
				GUI.Label(position2, GUIContent.none, TreeEditor.styles.pinLabel);
			}
			for (int l = 0; l < group.childGroupIDs.Length; l++)
			{
				TreeGroup group3 = treeData.GetGroup(group.childGroupIDs[l]);
				this.DrawHierachyNodes(treeData, nodes, group3, offset, alpha * fade, fade);
			}
			GUI.backgroundColor = Color.white;
			GUI.contentColor = Color.white;
		}
		private static void BeginSettingsSection(int nr, GUIContent[] names)
		{
			GUILayout.Space(5f);
			GUILayout.Label(names[nr].text, EditorStyles.boldLabel, new GUILayoutOption[0]);
		}
		private static void EndSettingsSection()
		{
			GUI.enabled = true;
			GUILayout.Space(5f);
		}
	}
}
