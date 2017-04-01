using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Cloth))]
	internal class ClothInspector : Editor
	{
		public enum DrawMode
		{
			MaxDistance = 1,
			CollisionSphereDistance
		}

		public enum ToolMode
		{
			Select,
			Paint
		}

		private enum RectSelectionMode
		{
			Replace,
			Add,
			Substract
		}

		private bool[] m_Selection;

		private bool[] m_RectSelection;

		private int m_MouseOver = -1;

		private int m_MeshVerticesPerSelectionVertex = 0;

		private Mesh[] m_SelectionMesh;

		private Mesh[] m_SelectedMesh;

		private Mesh m_VertexMesh;

		private Mesh m_VertexMeshSelected;

		private Vector3[] m_LastVertices;

		private Vector2 m_SelectStartPoint;

		private Vector2 m_SelectMousePoint;

		private bool m_RectSelecting = false;

		private bool m_DidSelect = false;

		private float[] m_MaxVisualizedValue = new float[3];

		private float[] m_MinVisualizedValue = new float[3];

		private ClothInspector.RectSelectionMode m_RectSelectionMode = ClothInspector.RectSelectionMode.Add;

		private static Color s_SelectionColor;

		private static Material s_SelectionMaterial = null;

		private static Material s_SelectionMaterialBackfaces = null;

		private static Material s_SelectedMaterial = null;

		private static Texture2D s_ColorTexture = null;

		private static int s_MaxVertices;

		private const float kDisabledValue = 3.40282347E+38f;

		private static GUIContent[] s_ToolIcons = null;

		private static GUIContent[] s_ModeStrings = null;

		private static GUIContent s_PaintIcon = null;

		private ClothInspectorState state
		{
			get
			{
				return ScriptableSingleton<ClothInspectorState>.instance;
			}
		}

		private ClothInspector.DrawMode drawMode
		{
			get
			{
				return this.state.DrawMode;
			}
			set
			{
				if (this.state.DrawMode != value)
				{
					this.state.DrawMode = value;
					this.SetupSelectionMeshColors();
					base.Repaint();
				}
			}
		}

		private Cloth cloth
		{
			get
			{
				return (Cloth)base.target;
			}
		}

		public bool editing
		{
			get
			{
				return EditMode.editMode == EditMode.SceneViewEditMode.Cloth && EditMode.IsOwner(this);
			}
		}

		private GUIContent GetModeString(ClothInspector.DrawMode mode)
		{
			return ClothInspector.s_ModeStrings[(int)mode];
		}

		private Texture2D GenerateColorTexture(int width)
		{
			Texture2D texture2D = new Texture2D(width, 1, TextureFormat.RGBA32, false);
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			texture2D.wrapMode = TextureWrapMode.Clamp;
			texture2D.hideFlags = HideFlags.DontSave;
			Color[] array = new Color[width];
			for (int i = 0; i < width; i++)
			{
				array[i] = this.GetGradientColor((float)i / (float)(width - 1));
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginDisabledGroup(base.targets.Length > 1);
			EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Cloth, "Edit Constraints", EditorGUIUtility.IconContent("EditCollider"), this.GetClothBounds(), this);
			EditorGUI.EndDisabledGroup();
			base.OnInspectorGUI();
			MeshRenderer component = this.cloth.GetComponent<MeshRenderer>();
			if (component != null)
			{
				Debug.LogWarning("MeshRenderer will not work with a cloth component! Use only SkinnedMeshRenderer. Any MeshRenderer's attached to a cloth component will be deleted at runtime.");
			}
		}

		private Bounds GetClothBounds()
		{
			Bounds result;
			if (base.target is Cloth)
			{
				Cloth cloth = (Cloth)base.target;
				SkinnedMeshRenderer component = cloth.GetComponent<SkinnedMeshRenderer>();
				if (component != null)
				{
					result = component.bounds;
					return result;
				}
			}
			result = default(Bounds);
			return result;
		}

		private bool SelectionMeshDirty()
		{
			SkinnedMeshRenderer component = this.cloth.GetComponent<SkinnedMeshRenderer>();
			Vector3[] vertices = this.cloth.vertices;
			Transform actualRootBone = component.actualRootBone;
			bool result;
			if (this.m_LastVertices.Length != vertices.Length)
			{
				result = true;
			}
			else
			{
				for (int i = 0; i < this.m_LastVertices.Length; i++)
				{
					Vector3 rhs = actualRootBone.rotation * vertices[i] + actualRootBone.position;
					if (!(this.m_LastVertices[i] == rhs))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		private void GenerateSelectionMesh()
		{
			SkinnedMeshRenderer component = this.cloth.GetComponent<SkinnedMeshRenderer>();
			Vector3[] vertices = this.cloth.vertices;
			int num = vertices.Length;
			this.m_Selection = new bool[vertices.Length];
			this.m_RectSelection = new bool[vertices.Length];
			if (this.m_SelectionMesh != null)
			{
				Mesh[] selectionMesh = this.m_SelectionMesh;
				for (int i = 0; i < selectionMesh.Length; i++)
				{
					Mesh obj = selectionMesh[i];
					UnityEngine.Object.DestroyImmediate(obj);
				}
				Mesh[] selectedMesh = this.m_SelectedMesh;
				for (int j = 0; j < selectedMesh.Length; j++)
				{
					Mesh obj2 = selectedMesh[j];
					UnityEngine.Object.DestroyImmediate(obj2);
				}
			}
			int num2 = num / ClothInspector.s_MaxVertices + 1;
			this.m_SelectionMesh = new Mesh[num2];
			this.m_SelectedMesh = new Mesh[num2];
			this.m_LastVertices = new Vector3[num];
			this.m_MeshVerticesPerSelectionVertex = this.m_VertexMesh.vertices.Length;
			Transform actualRootBone = component.actualRootBone;
			for (int k = 0; k < num2; k++)
			{
				this.m_SelectionMesh[k] = new Mesh();
				this.m_SelectionMesh[k].hideFlags |= HideFlags.DontSave;
				this.m_SelectedMesh[k] = new Mesh();
				this.m_SelectedMesh[k].hideFlags |= HideFlags.DontSave;
				int num3 = num - k * ClothInspector.s_MaxVertices;
				if (num3 > ClothInspector.s_MaxVertices)
				{
					num3 = ClothInspector.s_MaxVertices;
				}
				CombineInstance[] array = new CombineInstance[num3];
				int num4 = k * ClothInspector.s_MaxVertices;
				for (int l = 0; l < num3; l++)
				{
					this.m_LastVertices[num4 + l] = actualRootBone.rotation * vertices[num4 + l] + actualRootBone.position;
					array[l].mesh = this.m_VertexMesh;
					array[l].transform = Matrix4x4.TRS(this.m_LastVertices[num4 + l], Quaternion.identity, Vector3.one);
				}
				this.m_SelectionMesh[k].CombineMeshes(array);
				for (int m = 0; m < num3; m++)
				{
					array[m].mesh = this.m_VertexMeshSelected;
				}
				this.m_SelectedMesh[k].CombineMeshes(array);
			}
			this.SetupSelectionMeshColors();
		}

		private void OnEnable()
		{
			if (ClothInspector.s_SelectionMaterial == null)
			{
				ClothInspector.s_SelectionMaterial = (EditorGUIUtility.LoadRequired("SceneView/VertexSelectionMaterial.mat") as Material);
				ClothInspector.s_SelectionMaterialBackfaces = (EditorGUIUtility.LoadRequired("SceneView/VertexSelectionBackfacesMaterial.mat") as Material);
				ClothInspector.s_SelectedMaterial = (EditorGUIUtility.LoadRequired("SceneView/VertexSelectedMaterial.mat") as Material);
			}
			if (ClothInspector.s_ColorTexture == null)
			{
				ClothInspector.s_ColorTexture = this.GenerateColorTexture(100);
			}
			if (ClothInspector.s_ToolIcons == null)
			{
				ClothInspector.s_ToolIcons = new GUIContent[2];
				ClothInspector.s_ToolIcons[0] = EditorGUIUtility.TextContent("Select|Select vertices and edit their cloth coefficients in the inspector.");
				ClothInspector.s_ToolIcons[1] = EditorGUIUtility.TextContent("Paint|Paint cloth coefficients on to vertices.");
			}
			if (ClothInspector.s_ModeStrings == null)
			{
				ClothInspector.s_ModeStrings = new GUIContent[3];
				ClothInspector.s_ModeStrings[0] = EditorGUIUtility.TextContent("Fixed");
				ClothInspector.s_ModeStrings[1] = EditorGUIUtility.TextContent("Max Distance");
				ClothInspector.s_ModeStrings[2] = EditorGUIUtility.TextContent("Surface Penetration");
			}
			if (ClothInspector.s_PaintIcon == null)
			{
				ClothInspector.s_PaintIcon = EditorGUIUtility.IconContent("ClothInspector.PaintValue", "|Change this vertex coefficient value by painting in the scene view.");
			}
			this.m_VertexMesh = new Mesh();
			this.m_VertexMesh.hideFlags |= HideFlags.DontSave;
			Mesh mesh = (Mesh)Resources.GetBuiltinResource(typeof(Mesh), "Cube.fbx");
			this.m_VertexMesh.vertices = new Vector3[mesh.vertices.Length];
			this.m_VertexMesh.normals = mesh.normals;
			Vector4[] array = new Vector4[mesh.vertices.Length];
			Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < mesh.vertices.Length; i++)
			{
				array[i] = vertices[i] * -0.01f;
			}
			this.m_VertexMesh.tangents = array;
			this.m_VertexMesh.triangles = mesh.triangles;
			this.m_VertexMeshSelected = new Mesh();
			this.m_VertexMeshSelected.hideFlags |= HideFlags.DontSave;
			this.m_VertexMeshSelected.vertices = this.m_VertexMesh.vertices;
			this.m_VertexMeshSelected.normals = this.m_VertexMesh.normals;
			for (int j = 0; j < mesh.vertices.Length; j++)
			{
				array[j] = vertices[j] * -0.02f;
			}
			this.m_VertexMeshSelected.tangents = array;
			this.m_VertexMeshSelected.triangles = this.m_VertexMesh.triangles;
			ClothInspector.s_MaxVertices = 65536 / this.m_VertexMesh.vertices.Length;
			this.GenerateSelectionMesh();
			this.SetupSelectedMeshColors();
			SceneView.onPreSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onPreSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnPreSceneGUICallback));
		}

		private float GetCoefficient(ClothSkinningCoefficient coefficient)
		{
			ClothInspector.DrawMode drawMode = this.drawMode;
			float result;
			if (drawMode != ClothInspector.DrawMode.MaxDistance)
			{
				if (drawMode != ClothInspector.DrawMode.CollisionSphereDistance)
				{
					result = 0f;
				}
				else
				{
					result = coefficient.collisionSphereDistance;
				}
			}
			else
			{
				result = coefficient.maxDistance;
			}
			return result;
		}

		private Color GetGradientColor(float val)
		{
			Color result;
			if (val < 0.3f)
			{
				result = Color.Lerp(Color.red, Color.magenta, val / 0.2f);
			}
			else if (val < 0.7f)
			{
				result = Color.Lerp(Color.magenta, Color.yellow, (val - 0.2f) / 0.5f);
			}
			else
			{
				result = Color.Lerp(Color.yellow, Color.green, (val - 0.7f) / 0.3f);
			}
			return result;
		}

		private void AssignColorsToMeshArray(Color[] colors, Mesh[] meshArray)
		{
			int num = colors.Length / this.m_MeshVerticesPerSelectionVertex;
			int num2 = num / ClothInspector.s_MaxVertices + 1;
			for (int i = 0; i < num2; i++)
			{
				int num3 = num - i * ClothInspector.s_MaxVertices;
				if (num3 > ClothInspector.s_MaxVertices)
				{
					num3 = ClothInspector.s_MaxVertices;
				}
				Color[] array = new Color[num3 * this.m_MeshVerticesPerSelectionVertex];
				Array.Copy(colors, i * ClothInspector.s_MaxVertices * this.m_MeshVerticesPerSelectionVertex, array, 0, num3 * this.m_MeshVerticesPerSelectionVertex);
				meshArray[i].colors = array;
			}
		}

		private void SetupSelectionMeshColors()
		{
			ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
			int num = coefficients.Length;
			Color[] array = new Color[num * this.m_MeshVerticesPerSelectionVertex];
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < coefficients.Length; i++)
			{
				float coefficient = this.GetCoefficient(coefficients[i]);
				if (coefficient < 3.40282347E+38f)
				{
					if (coefficient < num2)
					{
						num2 = coefficient;
					}
					if (coefficient > num3)
					{
						num3 = coefficient;
					}
				}
			}
			for (int j = 0; j < num; j++)
			{
				float num4 = this.GetCoefficient(coefficients[j]);
				Color color;
				if (num4 >= 3.40282347E+38f)
				{
					color = Color.black;
				}
				else
				{
					if (num3 - num2 != 0f)
					{
						num4 = (num4 - num2) / (num3 - num2);
					}
					else
					{
						num4 = 0f;
					}
					color = this.GetGradientColor(num4);
				}
				for (int k = 0; k < this.m_MeshVerticesPerSelectionVertex; k++)
				{
					array[j * this.m_MeshVerticesPerSelectionVertex + k] = color;
				}
			}
			this.m_MaxVisualizedValue[(int)this.drawMode] = num3;
			this.m_MinVisualizedValue[(int)this.drawMode] = num2;
			this.AssignColorsToMeshArray(array, this.m_SelectionMesh);
		}

		private void SetupSelectedMeshColors()
		{
			int num = this.cloth.coefficients.Length;
			Color[] array = new Color[num * this.m_MeshVerticesPerSelectionVertex];
			for (int i = 0; i < num; i++)
			{
				bool flag = this.m_Selection[i];
				if (this.m_RectSelecting)
				{
					ClothInspector.RectSelectionMode rectSelectionMode = this.m_RectSelectionMode;
					if (rectSelectionMode != ClothInspector.RectSelectionMode.Replace)
					{
						if (rectSelectionMode != ClothInspector.RectSelectionMode.Add)
						{
							if (rectSelectionMode == ClothInspector.RectSelectionMode.Substract)
							{
								flag = (flag && !this.m_RectSelection[i]);
							}
						}
						else
						{
							flag |= this.m_RectSelection[i];
						}
					}
					else
					{
						flag = this.m_RectSelection[i];
					}
				}
				Color color = (!flag) ? Color.clear : ClothInspector.s_SelectionColor;
				for (int j = 0; j < this.m_MeshVerticesPerSelectionVertex; j++)
				{
					array[i * this.m_MeshVerticesPerSelectionVertex + j] = color;
				}
			}
			this.AssignColorsToMeshArray(array, this.m_SelectedMesh);
		}

		private void OnDisable()
		{
			SceneView.onPreSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onPreSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnPreSceneGUICallback));
			if (this.m_SelectionMesh != null)
			{
				Mesh[] selectionMesh = this.m_SelectionMesh;
				for (int i = 0; i < selectionMesh.Length; i++)
				{
					Mesh obj = selectionMesh[i];
					UnityEngine.Object.DestroyImmediate(obj);
				}
				Mesh[] selectedMesh = this.m_SelectedMesh;
				for (int j = 0; j < selectedMesh.Length; j++)
				{
					Mesh obj2 = selectedMesh[j];
					UnityEngine.Object.DestroyImmediate(obj2);
				}
			}
			UnityEngine.Object.DestroyImmediate(this.m_VertexMesh);
			UnityEngine.Object.DestroyImmediate(this.m_VertexMeshSelected);
		}

		private float CoefficientField(float value, float useValue, bool enabled, ClothInspector.DrawMode mode)
		{
			GUIContent modeString = this.GetModeString(mode);
			using (new EditorGUI.DisabledScope(!enabled))
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUI.showMixedValue = (useValue < 0f);
				EditorGUI.BeginChangeCheck();
				useValue = (float)((!EditorGUILayout.Toggle(GUIContent.none, useValue != 0f, new GUILayoutOption[0])) ? 0 : 1);
				if (EditorGUI.EndChangeCheck())
				{
					if (useValue > 0f)
					{
						value = 0f;
					}
					else
					{
						value = 3.40282347E+38f;
					}
					this.drawMode = mode;
				}
				GUILayout.Space(-152f);
				EditorGUI.showMixedValue = false;
				using (new EditorGUI.DisabledScope(useValue != 1f))
				{
					float num = value;
					EditorGUI.showMixedValue = (value < 0f);
					EditorGUI.BeginChangeCheck();
					int keyboardControl = GUIUtility.keyboardControl;
					if (useValue > 0f)
					{
						num = EditorGUILayout.FloatField(modeString, value, new GUILayoutOption[0]);
					}
					else
					{
						EditorGUILayout.FloatField(modeString, 0f, new GUILayoutOption[0]);
					}
					bool flag = EditorGUI.EndChangeCheck();
					if (flag)
					{
						value = num;
						if (value < 0f)
						{
							value = 0f;
						}
					}
					if (flag || keyboardControl != GUIUtility.keyboardControl)
					{
						this.drawMode = mode;
					}
				}
			}
			if (useValue > 0f)
			{
				float num2 = this.m_MinVisualizedValue[(int)mode];
				float num3 = this.m_MaxVisualizedValue[(int)mode];
				if (num3 - num2 > 0f)
				{
					this.DrawColorBox(null, this.GetGradientColor((value - num2) / (num3 - num2)));
				}
				else
				{
					this.DrawColorBox(null, this.GetGradientColor((float)((value > num2) ? 1 : 0)));
				}
			}
			else
			{
				this.DrawColorBox(null, Color.black);
			}
			EditorGUI.showMixedValue = false;
			GUILayout.EndHorizontal();
			return value;
		}

		private float PaintField(float value, ref bool enabled, ClothInspector.DrawMode mode)
		{
			GUIContent modeString = this.GetModeString(mode);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			enabled = GUILayout.Toggle(enabled, ClothInspector.s_PaintIcon, "MiniButton", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			bool flag;
			float num;
			using (new EditorGUI.DisabledScope(!enabled))
			{
				EditorGUI.BeginChangeCheck();
				flag = EditorGUILayout.Toggle(GUIContent.none, value < 3.40282347E+38f, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					if (flag)
					{
						value = 0f;
					}
					else
					{
						value = 3.40282347E+38f;
					}
					this.drawMode = mode;
				}
				GUILayout.Space(-162f);
				using (new EditorGUI.DisabledScope(!flag))
				{
					num = value;
					int keyboardControl = GUIUtility.keyboardControl;
					EditorGUI.BeginChangeCheck();
					if (flag)
					{
						num = EditorGUILayout.FloatField(modeString, value, new GUILayoutOption[0]);
					}
					else
					{
						EditorGUILayout.FloatField(modeString, 0f, new GUILayoutOption[0]);
					}
					if (num < 0f)
					{
						num = 0f;
					}
					if (EditorGUI.EndChangeCheck() || keyboardControl != GUIUtility.keyboardControl)
					{
						this.drawMode = mode;
					}
				}
			}
			if (flag)
			{
				float num2 = this.m_MinVisualizedValue[(int)mode];
				float num3 = this.m_MaxVisualizedValue[(int)mode];
				if (num3 - num2 > 0f)
				{
					this.DrawColorBox(null, this.GetGradientColor((value - num2) / (num3 - num2)));
				}
				else
				{
					this.DrawColorBox(null, this.GetGradientColor((float)((value > num2) ? 1 : 0)));
				}
			}
			else
			{
				this.DrawColorBox(null, Color.black);
			}
			GUILayout.EndHorizontal();
			return num;
		}

		private void SelectionGUI()
		{
			ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			bool flag = true;
			for (int i = 0; i < this.m_Selection.Length; i++)
			{
				if (this.m_Selection[i])
				{
					if (flag)
					{
						num = coefficients[i].maxDistance;
						num2 = (float)((num >= 3.40282347E+38f) ? 0 : 1);
						num3 = coefficients[i].collisionSphereDistance;
						num4 = (float)((num3 >= 3.40282347E+38f) ? 0 : 1);
						flag = false;
					}
					if (coefficients[i].maxDistance != num)
					{
						num = -1f;
					}
					if (coefficients[i].collisionSphereDistance != num3)
					{
						num3 = -1f;
					}
					if (num2 != (float)((coefficients[i].maxDistance >= 3.40282347E+38f) ? 0 : 1))
					{
						num2 = -1f;
					}
					if (num4 != (float)((coefficients[i].collisionSphereDistance >= 3.40282347E+38f) ? 0 : 1))
					{
						num4 = -1f;
					}
					num5++;
				}
			}
			float num6 = this.CoefficientField(num, num2, num5 > 0, ClothInspector.DrawMode.MaxDistance);
			if (num6 != num)
			{
				for (int j = 0; j < coefficients.Length; j++)
				{
					if (this.m_Selection[j])
					{
						coefficients[j].maxDistance = num6;
					}
				}
				this.cloth.coefficients = coefficients;
				this.SetupSelectionMeshColors();
				Undo.RegisterCompleteObjectUndo(base.target, "Change Cloth Coefficients");
			}
			float num7 = this.CoefficientField(num3, num4, num5 > 0, ClothInspector.DrawMode.CollisionSphereDistance);
			if (num7 != num3)
			{
				for (int k = 0; k < coefficients.Length; k++)
				{
					if (this.m_Selection[k])
					{
						coefficients[k].collisionSphereDistance = num7;
					}
				}
				this.cloth.coefficients = coefficients;
				this.SetupSelectionMeshColors();
				Undo.RegisterCompleteObjectUndo(base.target, "Change Cloth Coefficients");
			}
			using (new EditorGUI.DisabledScope(true))
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				if (num5 > 0)
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label(num5 + " selected", new GUILayoutOption[0]);
				}
				else
				{
					GUILayout.Label("Select cloth vertices to edit their constraints.", new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
				}
				GUILayout.EndHorizontal();
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Backspace)
			{
				for (int l = 0; l < coefficients.Length; l++)
				{
					if (this.m_Selection[l])
					{
						ClothInspector.DrawMode drawMode = this.drawMode;
						if (drawMode != ClothInspector.DrawMode.MaxDistance)
						{
							if (drawMode == ClothInspector.DrawMode.CollisionSphereDistance)
							{
								coefficients[l].collisionSphereDistance = 3.40282347E+38f;
							}
						}
						else
						{
							coefficients[l].maxDistance = 3.40282347E+38f;
						}
					}
				}
				this.cloth.coefficients = coefficients;
				this.SetupSelectionMeshColors();
			}
		}

		private void PaintGUI()
		{
			this.state.PaintMaxDistance = this.PaintField(this.state.PaintMaxDistance, ref this.state.PaintMaxDistanceEnabled, ClothInspector.DrawMode.MaxDistance);
			this.state.PaintCollisionSphereDistance = this.PaintField(this.state.PaintCollisionSphereDistance, ref this.state.PaintCollisionSphereDistanceEnabled, ClothInspector.DrawMode.CollisionSphereDistance);
			if (this.state.PaintMaxDistanceEnabled && !this.state.PaintCollisionSphereDistanceEnabled)
			{
				this.drawMode = ClothInspector.DrawMode.MaxDistance;
			}
			else if (!this.state.PaintMaxDistanceEnabled && this.state.PaintCollisionSphereDistanceEnabled)
			{
				this.drawMode = ClothInspector.DrawMode.CollisionSphereDistance;
			}
			using (new EditorGUI.DisabledScope(true))
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Set constraints to paint onto cloth vertices.", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}

		private int GetMouseVertex(Event e)
		{
			int result;
			if (Tools.current != Tool.None)
			{
				result = -1;
			}
			else
			{
				SkinnedMeshRenderer component = this.cloth.GetComponent<SkinnedMeshRenderer>();
				Vector3[] normals = this.cloth.normals;
				ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
				Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
				float num = 1000f;
				int num2 = -1;
				Quaternion rotation = component.actualRootBone.rotation;
				for (int i = 0; i < coefficients.Length; i++)
				{
					Vector3 lhs = this.m_LastVertices[i] - ray.origin;
					float sqrMagnitude = Vector3.Cross(lhs, ray.direction).sqrMagnitude;
					if ((Vector3.Dot(rotation * normals[i], Camera.current.transform.forward) <= 0f || this.state.ManipulateBackfaces) && sqrMagnitude < num && sqrMagnitude < 0.00250000018f)
					{
						num = sqrMagnitude;
						num2 = i;
					}
				}
				result = num2;
			}
			return result;
		}

		private void DrawVertices()
		{
			if (this.SelectionMeshDirty())
			{
				this.GenerateSelectionMesh();
			}
			if (this.state.ToolMode == ClothInspector.ToolMode.Select)
			{
				for (int i = 0; i < ClothInspector.s_SelectedMaterial.passCount; i++)
				{
					ClothInspector.s_SelectedMaterial.SetPass(i);
					Mesh[] selectedMesh = this.m_SelectedMesh;
					for (int j = 0; j < selectedMesh.Length; j++)
					{
						Mesh mesh = selectedMesh[j];
						Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
					}
				}
			}
			Material material = (!this.state.ManipulateBackfaces) ? ClothInspector.s_SelectionMaterial : ClothInspector.s_SelectionMaterialBackfaces;
			for (int k = 0; k < material.passCount; k++)
			{
				material.SetPass(k);
				Mesh[] selectionMesh = this.m_SelectionMesh;
				for (int l = 0; l < selectionMesh.Length; l++)
				{
					Mesh mesh2 = selectionMesh[l];
					Graphics.DrawMeshNow(mesh2, Matrix4x4.identity);
				}
			}
			if (this.m_MouseOver != -1)
			{
				Matrix4x4 matrix = Matrix4x4.TRS(this.m_LastVertices[this.m_MouseOver], Quaternion.identity, Vector3.one * 1.2f);
				if (this.state.ToolMode == ClothInspector.ToolMode.Select)
				{
					material = ClothInspector.s_SelectedMaterial;
					material.color = new Color(ClothInspector.s_SelectionColor.r, ClothInspector.s_SelectionColor.g, ClothInspector.s_SelectionColor.b, 0.5f);
				}
				else
				{
					int num = this.m_MouseOver / ClothInspector.s_MaxVertices;
					int num2 = this.m_MouseOver - ClothInspector.s_MaxVertices * num;
					material.color = this.m_SelectionMesh[num].colors[num2];
				}
				for (int m = 0; m < material.passCount; m++)
				{
					material.SetPass(m);
					Graphics.DrawMeshNow(this.m_VertexMeshSelected, matrix);
				}
				material.color = Color.white;
			}
		}

		private bool UpdateRectSelection()
		{
			bool result = false;
			SkinnedMeshRenderer component = this.cloth.GetComponent<SkinnedMeshRenderer>();
			Vector3[] normals = this.cloth.normals;
			ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
			float x = Mathf.Min(this.m_SelectStartPoint.x, this.m_SelectMousePoint.x);
			float x2 = Mathf.Max(this.m_SelectStartPoint.x, this.m_SelectMousePoint.x);
			float y = Mathf.Min(this.m_SelectStartPoint.y, this.m_SelectMousePoint.y);
			float y2 = Mathf.Max(this.m_SelectStartPoint.y, this.m_SelectMousePoint.y);
			Ray ray = HandleUtility.GUIPointToWorldRay(new Vector2(x, y));
			Ray ray2 = HandleUtility.GUIPointToWorldRay(new Vector2(x2, y));
			Ray ray3 = HandleUtility.GUIPointToWorldRay(new Vector2(x, y2));
			Ray ray4 = HandleUtility.GUIPointToWorldRay(new Vector2(x2, y2));
			Plane plane = new Plane(ray2.origin + ray2.direction, ray.origin + ray.direction, ray.origin);
			Plane plane2 = new Plane(ray3.origin + ray3.direction, ray4.origin + ray4.direction, ray4.origin);
			Plane plane3 = new Plane(ray.origin + ray.direction, ray3.origin + ray3.direction, ray3.origin);
			Plane plane4 = new Plane(ray4.origin + ray4.direction, ray2.origin + ray2.direction, ray2.origin);
			Quaternion rotation = component.actualRootBone.rotation;
			for (int i = 0; i < coefficients.Length; i++)
			{
				Vector3 inPt = this.m_LastVertices[i];
				bool flag = Vector3.Dot(rotation * normals[i], Camera.current.transform.forward) <= 0f;
				bool flag2 = plane.GetSide(inPt) && plane2.GetSide(inPt) && plane3.GetSide(inPt) && plane4.GetSide(inPt);
				flag2 = (flag2 && (this.state.ManipulateBackfaces || flag));
				if (this.m_RectSelection[i] != flag2)
				{
					this.m_RectSelection[i] = flag2;
					result = true;
				}
			}
			return result;
		}

		private void ApplyRectSelection()
		{
			ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
			for (int i = 0; i < coefficients.Length; i++)
			{
				ClothInspector.RectSelectionMode rectSelectionMode = this.m_RectSelectionMode;
				if (rectSelectionMode != ClothInspector.RectSelectionMode.Replace)
				{
					if (rectSelectionMode != ClothInspector.RectSelectionMode.Add)
					{
						if (rectSelectionMode == ClothInspector.RectSelectionMode.Substract)
						{
							this.m_Selection[i] = (this.m_Selection[i] && !this.m_RectSelection[i]);
						}
					}
					else
					{
						this.m_Selection[i] |= this.m_RectSelection[i];
					}
				}
				else
				{
					this.m_Selection[i] = this.m_RectSelection[i];
				}
			}
		}

		private bool RectSelectionModeFromEvent()
		{
			Event current = Event.current;
			ClothInspector.RectSelectionMode rectSelectionMode = ClothInspector.RectSelectionMode.Replace;
			if (current.shift)
			{
				rectSelectionMode = ClothInspector.RectSelectionMode.Add;
			}
			if (current.alt)
			{
				rectSelectionMode = ClothInspector.RectSelectionMode.Substract;
			}
			bool result;
			if (this.m_RectSelectionMode != rectSelectionMode)
			{
				this.m_RectSelectionMode = rectSelectionMode;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal void SendCommandsOnModifierKeys()
		{
			SceneView.lastActiveSceneView.SendEvent(EditorGUIUtility.CommandEvent("ModifierKeysChanged"));
		}

		private void SelectionPreSceneGUI(int id)
		{
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(id);
			switch (typeForControl)
			{
			case EventType.MouseDown:
			{
				if (current.alt || current.control || current.command || current.button != 0)
				{
					return;
				}
				GUIUtility.hotControl = id;
				int mouseVertex = this.GetMouseVertex(current);
				if (mouseVertex != -1)
				{
					if (current.shift)
					{
						this.m_Selection[mouseVertex] = !this.m_Selection[mouseVertex];
					}
					else
					{
						for (int i = 0; i < this.m_Selection.Length; i++)
						{
							this.m_Selection[i] = false;
						}
						this.m_Selection[mouseVertex] = true;
					}
					this.m_DidSelect = true;
					this.SetupSelectedMeshColors();
					base.Repaint();
				}
				else
				{
					this.m_DidSelect = false;
				}
				this.m_SelectStartPoint = current.mousePosition;
				current.Use();
				return;
			}
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && current.button == 0)
				{
					GUIUtility.hotControl = 0;
					if (this.m_RectSelecting)
					{
						EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendCommandsOnModifierKeys));
						this.m_RectSelecting = false;
						this.RectSelectionModeFromEvent();
						this.ApplyRectSelection();
					}
					else if (!this.m_DidSelect)
					{
						if (!current.alt && !current.control && !current.command)
						{
							ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
							for (int j = 0; j < coefficients.Length; j++)
							{
								this.m_Selection[j] = false;
							}
						}
					}
					GUIUtility.keyboardControl = 0;
					this.SetupSelectedMeshColors();
					SceneView.RepaintAll();
				}
				return;
			case EventType.MouseMove:
				IL_25:
				if (typeForControl != EventType.ExecuteCommand)
				{
					return;
				}
				if (this.m_RectSelecting && current.commandName == "ModifierKeysChanged")
				{
					if (this.RectSelectionModeFromEvent() || this.UpdateRectSelection())
					{
						this.SetupSelectedMeshColors();
					}
				}
				return;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					if (!this.m_RectSelecting && (current.mousePosition - this.m_SelectStartPoint).magnitude > 2f)
					{
						if (!current.alt && !current.control && !current.command)
						{
							EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendCommandsOnModifierKeys));
							this.m_RectSelecting = true;
							this.RectSelectionModeFromEvent();
							this.SetupSelectedMeshColors();
						}
					}
					if (this.m_RectSelecting)
					{
						this.m_SelectMousePoint = new Vector2(Mathf.Max(current.mousePosition.x, 0f), Mathf.Max(current.mousePosition.y, 0f));
						if (this.RectSelectionModeFromEvent() || this.UpdateRectSelection())
						{
							this.SetupSelectedMeshColors();
						}
						current.Use();
					}
				}
				return;
			}
			goto IL_25;
		}

		private void PaintPreSceneGUI(int id)
		{
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(id);
			if (typeForControl == EventType.MouseDown || typeForControl == EventType.MouseDrag)
			{
				ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
				if (GUIUtility.hotControl == id || (!current.alt && !current.control && !current.command && current.button == 0))
				{
					if (typeForControl == EventType.MouseDown)
					{
						GUIUtility.hotControl = id;
					}
					int mouseVertex = this.GetMouseVertex(current);
					if (mouseVertex != -1)
					{
						bool flag = false;
						if (this.state.PaintMaxDistanceEnabled && coefficients[mouseVertex].maxDistance != this.state.PaintMaxDistance)
						{
							coefficients[mouseVertex].maxDistance = this.state.PaintMaxDistance;
							flag = true;
						}
						if (this.state.PaintCollisionSphereDistanceEnabled && coefficients[mouseVertex].collisionSphereDistance != this.state.PaintCollisionSphereDistance)
						{
							coefficients[mouseVertex].collisionSphereDistance = this.state.PaintCollisionSphereDistance;
							flag = true;
						}
						if (flag)
						{
							Undo.RegisterCompleteObjectUndo(base.target, "Paint Cloth");
							this.cloth.coefficients = coefficients;
							this.SetupSelectionMeshColors();
							base.Repaint();
						}
					}
					current.Use();
				}
			}
			else if (typeForControl == EventType.MouseUp)
			{
				if (GUIUtility.hotControl == id && current.button == 0)
				{
					GUIUtility.hotControl = 0;
					current.Use();
				}
			}
		}

		private void OnPreSceneGUICallback(SceneView sceneView)
		{
			if (this.editing)
			{
				if (base.targets.Length <= 1)
				{
					Tools.current = Tool.None;
					if (this.state.ToolMode == (ClothInspector.ToolMode)(-1))
					{
						this.state.ToolMode = ClothInspector.ToolMode.Select;
					}
					ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
					if (this.m_Selection.Length != coefficients.Length && this.m_Selection.Length != ClothInspector.s_MaxVertices)
					{
						this.OnEnable();
					}
					Handles.BeginGUI();
					int controlID = GUIUtility.GetControlID(FocusType.Passive);
					Event current = Event.current;
					EventType typeForControl = current.GetTypeForControl(controlID);
					if (typeForControl != EventType.Layout)
					{
						if (typeForControl == EventType.MouseMove || typeForControl == EventType.MouseDrag)
						{
							int mouseOver = this.m_MouseOver;
							this.m_MouseOver = this.GetMouseVertex(current);
							if (this.m_MouseOver != mouseOver)
							{
								SceneView.RepaintAll();
							}
						}
					}
					else
					{
						HandleUtility.AddDefaultControl(controlID);
					}
					ClothInspector.ToolMode toolMode = this.state.ToolMode;
					if (toolMode != ClothInspector.ToolMode.Select)
					{
						if (toolMode == ClothInspector.ToolMode.Paint)
						{
							this.PaintPreSceneGUI(controlID);
						}
					}
					else
					{
						this.SelectionPreSceneGUI(controlID);
					}
					Handles.EndGUI();
				}
			}
		}

		public void OnSceneGUI()
		{
			if (this.editing)
			{
				if (Selection.gameObjects.Length <= 1)
				{
					ClothInspector.s_SelectionColor = GUI.skin.settings.selectionColor;
					if (Event.current.type == EventType.Repaint)
					{
						this.DrawVertices();
					}
					Event current = Event.current;
					if (current.commandName == "SelectAll")
					{
						if (current.type == EventType.ValidateCommand)
						{
							current.Use();
						}
						if (current.type == EventType.ExecuteCommand)
						{
							int num = this.cloth.vertices.Length;
							for (int i = 0; i < num; i++)
							{
								this.m_Selection[i] = true;
							}
							this.SetupSelectedMeshColors();
							SceneView.RepaintAll();
							this.state.ToolMode = ClothInspector.ToolMode.Select;
							current.Use();
						}
					}
					Handles.BeginGUI();
					if (this.m_RectSelecting && this.state.ToolMode == ClothInspector.ToolMode.Select && Event.current.type == EventType.Repaint)
					{
						EditorStyles.selectionRect.Draw(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), GUIContent.none, false, false, false, false);
					}
					Handles.EndGUI();
					SceneViewOverlay.Window(new GUIContent("Cloth Constraints"), new SceneViewOverlay.WindowFunction(this.VertexEditing), 0, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
				}
			}
		}

		public void VisualizationMenuSetMaxDistanceMode()
		{
			this.drawMode = ClothInspector.DrawMode.MaxDistance;
			if (!this.state.PaintMaxDistanceEnabled)
			{
				this.state.PaintCollisionSphereDistanceEnabled = false;
				this.state.PaintMaxDistanceEnabled = true;
			}
		}

		public void VisualizationMenuSetCollisionSphereMode()
		{
			this.drawMode = ClothInspector.DrawMode.CollisionSphereDistance;
			if (!this.state.PaintCollisionSphereDistanceEnabled)
			{
				this.state.PaintCollisionSphereDistanceEnabled = true;
				this.state.PaintMaxDistanceEnabled = false;
			}
		}

		public void VisualizationMenuToggleManipulateBackfaces()
		{
			this.state.ManipulateBackfaces = !this.state.ManipulateBackfaces;
		}

		public void DrawColorBox(Texture gradientTex, Color col)
		{
			if (!GUI.enabled)
			{
				col = new Color(0.3f, 0.3f, 0.3f, 1f);
				EditorGUI.showMixedValue = false;
			}
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Space(5f);
			Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(10f)
			});
			GUI.Box(rect, GUIContent.none);
			rect = new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);
			if (gradientTex)
			{
				GUI.DrawTexture(rect, gradientTex);
			}
			else
			{
				EditorGUIUtility.DrawColorSwatch(rect, col, false);
			}
			GUILayout.EndVertical();
		}

		private bool IsConstrained()
		{
			ClothSkinningCoefficient[] coefficients = this.cloth.coefficients;
			ClothSkinningCoefficient[] array = coefficients;
			int i = 0;
			bool result;
			while (i < array.Length)
			{
				ClothSkinningCoefficient clothSkinningCoefficient = array[i];
				if (clothSkinningCoefficient.maxDistance < 3.40282347E+38f)
				{
					result = true;
				}
				else
				{
					if (clothSkinningCoefficient.collisionSphereDistance >= 3.40282347E+38f)
					{
						i++;
						continue;
					}
					result = true;
				}
				return result;
			}
			result = false;
			return result;
		}

		private void VertexEditing(UnityEngine.Object unused, SceneView sceneView)
		{
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(300f)
			});
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Visualization: ", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (EditorGUILayout.DropdownButton(this.GetModeString(this.drawMode), FocusType.Passive, EditorStyles.toolbarDropDown, new GUILayoutOption[0]))
			{
				Rect last = GUILayoutUtility.topLevel.GetLast();
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(this.GetModeString(ClothInspector.DrawMode.MaxDistance), this.drawMode == ClothInspector.DrawMode.MaxDistance, new GenericMenu.MenuFunction(this.VisualizationMenuSetMaxDistanceMode));
				genericMenu.AddItem(this.GetModeString(ClothInspector.DrawMode.CollisionSphereDistance), this.drawMode == ClothInspector.DrawMode.CollisionSphereDistance, new GenericMenu.MenuFunction(this.VisualizationMenuSetCollisionSphereMode));
				genericMenu.AddSeparator("");
				genericMenu.AddItem(new GUIContent("Manipulate Backfaces"), this.state.ManipulateBackfaces, new GenericMenu.MenuFunction(this.VisualizationMenuToggleManipulateBackfaces));
				genericMenu.DropDown(last);
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(this.m_MinVisualizedValue[(int)this.drawMode].ToString(), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			this.DrawColorBox(ClothInspector.s_ColorTexture, Color.clear);
			GUILayout.Label(this.m_MaxVisualizedValue[(int)this.drawMode].ToString(), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUILayout.Label("Unconstrained:", new GUILayoutOption[0]);
			GUILayout.Space(-24f);
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Width(20f)
			});
			this.DrawColorBox(null, Color.black);
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
			if (Tools.current != Tool.None)
			{
				this.state.ToolMode = (ClothInspector.ToolMode)(-1);
			}
			ClothInspector.ToolMode toolMode = this.state.ToolMode;
			this.state.ToolMode = (ClothInspector.ToolMode)GUILayout.Toolbar((int)this.state.ToolMode, ClothInspector.s_ToolIcons, new GUILayoutOption[0]);
			if (this.state.ToolMode != toolMode)
			{
				GUIUtility.keyboardControl = 0;
				SceneView.RepaintAll();
				this.SetupSelectionMeshColors();
				this.SetupSelectedMeshColors();
			}
			ClothInspector.ToolMode toolMode2 = this.state.ToolMode;
			if (toolMode2 != ClothInspector.ToolMode.Select)
			{
				if (toolMode2 == ClothInspector.ToolMode.Paint)
				{
					Tools.current = Tool.None;
					this.PaintGUI();
				}
			}
			else
			{
				Tools.current = Tool.None;
				this.SelectionGUI();
			}
			GUILayout.EndVertical();
			if (!this.IsConstrained())
			{
				EditorGUILayout.HelpBox("No constraints have been set up, so the cloth will move freely. Set up vertex constraints here to restrict it.", MessageType.Info);
			}
			GUILayout.EndVertical();
			GUILayout.Space(-4f);
		}
	}
}
