using System;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(SkinnedCloth))]
	internal class ClothInspector : Editor
	{
		private enum RectSelectionMode
		{
			Replace,
			Add,
			Substract
		}
		private enum ToolMode
		{
			Select,
			Paint,
			Settings
		}
		private const int clothToolID = 1200;
		private bool[] m_Selection;
		private bool[] m_RectSelection;
		private int m_MouseOver = -1;
		private int m_DrawMode = 1;
		private int m_MeshVerticesPerSelectionVertex;
		private Mesh m_SelectionMesh;
		private Mesh m_VertexMesh;
		private Vector3[] m_LastVertices;
		private Vector2 m_SelectStartPoint;
		private Vector2 m_SelectMousePoint;
		private bool m_RectSelecting;
		private bool m_DidSelect;
		private bool m_PaintMaxDistanceEnabled;
		private bool m_PaintMaxDistanceBiasEnabled;
		private bool m_PaintCollisionSphereRadiusEnabled;
		private bool m_PaintCollisionSphereDistanceEnabled;
		private float m_PaintMaxDistance = 0.2f;
		private float m_PaintMaxDistanceBias;
		private float m_PaintCollisionSphereRadius = 0.5f;
		private float m_PaintCollisionSphereDistance;
		private static Material s_SelectionMaterial = null;
		private ClothInspector.RectSelectionMode m_RectSelectionMode = ClothInspector.RectSelectionMode.Add;
		private static ClothInspector.ToolMode s_ToolMode = ClothInspector.ToolMode.Settings;
		private static int maxVertices;
		private static GUIContent[] s_ToolIcons = new GUIContent[]
		{
			EditorGUIUtility.IconContent("ClothInspector.SelectTool", "Select vertices and edit their cloth coefficients in the inspector."),
			EditorGUIUtility.IconContent("ClothInspector.PaintTool", "Paint cloth coefficients on to vertices."),
			EditorGUIUtility.IconContent("ClothInspector.SettingsTool", "Set cloth options.")
		};
		private static GUIContent s_ViewIcon = EditorGUIUtility.IconContent("ClothInspector.ViewValue", "Visualize this vertex coefficient value in the scene view.");
		private static GUIContent s_PaintIcon = EditorGUIUtility.IconContent("ClothInspector.PaintValue", "Change this vertex coefficient value by painting in the scene view.");
		private bool SelectionMeshDirty()
		{
			SkinnedCloth skinnedCloth = (SkinnedCloth)this.target;
			SkinnedMeshRenderer component = skinnedCloth.GetComponent<SkinnedMeshRenderer>();
			Vector3[] vertices = skinnedCloth.vertices;
			Quaternion rotation = component.actualRootBone.rotation;
			Vector3 position = component.actualRootBone.position;
			for (int i = 0; i < this.m_LastVertices.Length; i++)
			{
				if (this.m_LastVertices[i] != rotation * vertices[i] + position)
				{
					return true;
				}
			}
			return false;
		}
		private void GenerateSelectionMesh()
		{
			SkinnedCloth skinnedCloth = (SkinnedCloth)this.target;
			SkinnedMeshRenderer component = skinnedCloth.GetComponent<SkinnedMeshRenderer>();
			Vector3[] vertices = skinnedCloth.vertices;
			if (this.m_SelectionMesh != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_SelectionMesh);
			}
			this.m_SelectionMesh = new Mesh();
			this.m_SelectionMesh.hideFlags |= HideFlags.DontSave;
			CombineInstance[] array = new CombineInstance[vertices.Length];
			this.m_MeshVerticesPerSelectionVertex = this.m_VertexMesh.vertices.Length;
			this.m_LastVertices = new Vector3[vertices.Length];
			Quaternion rotation = component.actualRootBone.rotation;
			Vector3 position = component.actualRootBone.position;
			int num = 0;
			while (num < vertices.Length && num < ClothInspector.maxVertices)
			{
				this.m_LastVertices[num] = rotation * vertices[num] + position;
				array[num].mesh = this.m_VertexMesh;
				array[num].transform = Matrix4x4.TRS(this.m_LastVertices[num], Quaternion.identity, 0.015f * Vector3.one);
				num++;
			}
			this.m_SelectionMesh.CombineMeshes(array);
			this.SetupMeshColors();
		}
		private void OnEnable()
		{
			if (ClothInspector.s_SelectionMaterial == null)
			{
				ClothInspector.s_SelectionMaterial = (EditorGUIUtility.LoadRequired("SceneView/VertexSelectionMaterial.mat") as Material);
			}
			SkinnedCloth skinnedCloth = (SkinnedCloth)this.target;
			ClothSkinningCoefficient[] coefficients = skinnedCloth.coefficients;
			this.m_Selection = new bool[coefficients.Length];
			this.m_RectSelection = new bool[coefficients.Length];
			this.m_VertexMesh = (Mesh)Resources.GetBuiltinResource(typeof(Mesh), "Cube.fbx");
			ClothInspector.maxVertices = 65536 / this.m_VertexMesh.vertices.Length;
			if (skinnedCloth.vertices.Length >= ClothInspector.maxVertices)
			{
				Debug.LogWarning("The mesh has too many vertices to be able to edit all skin coefficients. Only the first " + ClothInspector.maxVertices + " vertices will be displayed");
			}
			this.GenerateSelectionMesh();
		}
		private float GetCoefficient(ClothSkinningCoefficient coefficient)
		{
			switch (this.m_DrawMode)
			{
			case 1:
				return coefficient.maxDistance;
			case 2:
				return coefficient.maxDistanceBias;
			case 3:
				return coefficient.collisionSphereRadius;
			case 4:
				return coefficient.collisionSphereDistance;
			default:
				return 0f;
			}
		}
		private void SetupMeshColors()
		{
			SkinnedCloth skinnedCloth = (SkinnedCloth)this.target;
			ClothSkinningCoefficient[] coefficients = skinnedCloth.coefficients;
			Color[] array = new Color[this.m_SelectionMesh.vertices.Length];
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < coefficients.Length; i++)
			{
				float coefficient = this.GetCoefficient(coefficients[i]);
				if (coefficient < num)
				{
					num = coefficient;
				}
				if (coefficient > num2)
				{
					num2 = coefficient;
				}
			}
			int num3 = 0;
			while (num3 < coefficients.Length && num3 < ClothInspector.maxVertices)
			{
				for (int j = 0; j < this.m_MeshVerticesPerSelectionVertex; j++)
				{
					bool flag = this.m_Selection[num3];
					if (this.m_RectSelecting)
					{
						switch (this.m_RectSelectionMode)
						{
						case ClothInspector.RectSelectionMode.Replace:
							flag = this.m_RectSelection[num3];
							break;
						case ClothInspector.RectSelectionMode.Add:
							flag |= this.m_RectSelection[num3];
							break;
						case ClothInspector.RectSelectionMode.Substract:
							flag = (flag && !this.m_RectSelection[num3]);
							break;
						}
					}
					Color color;
					if (flag)
					{
						color = Color.red;
					}
					else
					{
						float num4;
						if (num2 - num != 0f)
						{
							num4 = (this.GetCoefficient(coefficients[num3]) - num) / (num2 - num);
						}
						else
						{
							num4 = 0.5f;
						}
						if (num4 < 0.5f)
						{
							color = Color.Lerp(Color.green, Color.yellow, 2f * num4);
						}
						else
						{
							color = Color.Lerp(Color.yellow, Color.blue, 2f * num4 - 1f);
						}
					}
					array[num3 * this.m_MeshVerticesPerSelectionVertex + j] = color;
				}
				num3++;
			}
			this.m_SelectionMesh.colors = array;
		}
		private void OnDisable()
		{
			UnityEngine.Object.DestroyImmediate(this.m_SelectionMesh);
		}
		private float CoefficientField(string label, float value, bool enabled, int mode)
		{
			bool enabled2 = GUI.enabled;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Toggle(this.m_DrawMode == mode, ClothInspector.s_ViewIcon, "MiniButton", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				this.m_DrawMode = mode;
				this.SetupMeshColors();
			}
			GUI.enabled = enabled;
			float result = EditorGUILayout.FloatField(label, value, new GUILayoutOption[0]);
			GUI.enabled = enabled2;
			GUILayout.EndHorizontal();
			return result;
		}
		private float PaintField(string label, float value, ref bool enabled, int mode)
		{
			bool enabled2 = GUI.enabled;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Toggle(this.m_DrawMode == mode, ClothInspector.s_ViewIcon, "MiniButton", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				this.m_DrawMode = mode;
				this.SetupMeshColors();
			}
			enabled = GUILayout.Toggle(enabled, ClothInspector.s_PaintIcon, "MiniButton", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUI.enabled = enabled;
			float result = EditorGUILayout.FloatField(label, value, new GUILayoutOption[0]);
			GUI.enabled = enabled2;
			GUILayout.EndHorizontal();
			return result;
		}
		private void SelectionGUI()
		{
			SkinnedCloth skinnedCloth = (SkinnedCloth)this.target;
			Vector3[] vertices = skinnedCloth.vertices;
			ClothSkinningCoefficient[] coefficients = skinnedCloth.coefficients;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Select All", new GUILayoutOption[0]))
			{
				for (int i = 0; i < vertices.Length; i++)
				{
					this.m_Selection[i] = true;
				}
				this.SetupMeshColors();
				SceneView.RepaintAll();
			}
			if (GUILayout.Button("Select None", new GUILayoutOption[0]))
			{
				for (int j = 0; j < vertices.Length; j++)
				{
					this.m_Selection[j] = false;
				}
				this.SetupMeshColors();
				SceneView.RepaintAll();
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			for (int k = 0; k < coefficients.Length; k++)
			{
				if (this.m_Selection[k])
				{
					num += coefficients[k].maxDistance;
					num2 += coefficients[k].maxDistanceBias;
					num3 += coefficients[k].collisionSphereRadius;
					num4 += coefficients[k].collisionSphereDistance;
					num5++;
				}
			}
			GUILayout.Label(num5 + " selected", new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			if (num5 > 0)
			{
				num /= (float)num5;
				num2 /= (float)num5;
				num3 /= (float)num5;
				num4 /= (float)num5;
			}
			float num6 = this.CoefficientField("max Distance", num, num5 > 0, 1);
			float num7 = this.CoefficientField("distance bias", num2, num5 > 0, 2);
			float num8 = this.CoefficientField("collsionSphereRadius", num3, num5 > 0, 3);
			float num9 = this.CoefficientField("collisionSphereDistance", num4, num5 > 0, 4);
			num7 = Mathf.Clamp(num7, -1f, 1f);
			if (num6 != num)
			{
				for (int l = 0; l < coefficients.Length; l++)
				{
					if (this.m_Selection[l])
					{
						coefficients[l].maxDistance = num6;
					}
				}
				skinnedCloth.coefficients = coefficients;
				this.SetupMeshColors();
			}
			if (num7 != num2)
			{
				for (int m = 0; m < coefficients.Length; m++)
				{
					if (this.m_Selection[m])
					{
						coefficients[m].maxDistanceBias = num7;
					}
				}
				skinnedCloth.coefficients = coefficients;
				this.SetupMeshColors();
			}
			if (num8 != num3)
			{
				for (int n = 0; n < coefficients.Length; n++)
				{
					if (this.m_Selection[n])
					{
						coefficients[n].collisionSphereRadius = num8;
					}
				}
				skinnedCloth.coefficients = coefficients;
				this.SetupMeshColors();
			}
			if (num9 != num4)
			{
				for (int num10 = 0; num10 < coefficients.Length; num10++)
				{
					if (this.m_Selection[num10])
					{
						coefficients[num10].collisionSphereDistance = num9;
					}
				}
				skinnedCloth.coefficients = coefficients;
				this.SetupMeshColors();
			}
		}
		private void PaintGUI()
		{
			this.m_PaintMaxDistance = this.PaintField("max Distance", this.m_PaintMaxDistance, ref this.m_PaintMaxDistanceEnabled, 1);
			this.m_PaintMaxDistanceBias = this.PaintField("distance bias", this.m_PaintMaxDistanceBias, ref this.m_PaintMaxDistanceBiasEnabled, 2);
			this.m_PaintMaxDistanceBias = Mathf.Clamp(this.m_PaintMaxDistanceBias, -1f, 1f);
			this.m_PaintCollisionSphereRadius = this.PaintField("collsionSphereRadius", this.m_PaintCollisionSphereRadius, ref this.m_PaintCollisionSphereRadiusEnabled, 3);
			this.m_PaintCollisionSphereDistance = this.PaintField("collisionSphereDistance", this.m_PaintCollisionSphereDistance, ref this.m_PaintCollisionSphereDistanceEnabled, 4);
		}
		public override void OnInspectorGUI()
		{
			ClothInspector.ToolMode toolMode = ClothInspector.s_ToolMode;
			if (Tools.current != Tool.None)
			{
				ClothInspector.s_ToolMode = ClothInspector.ToolMode.Settings;
			}
			ClothInspector.s_ToolMode = (ClothInspector.ToolMode)GUILayout.Toolbar((int)ClothInspector.s_ToolMode, ClothInspector.s_ToolIcons, new GUILayoutOption[0]);
			if (ClothInspector.s_ToolMode != toolMode)
			{
				GUIUtility.keyboardControl = 0;
				if (ClothInspector.s_ToolMode != ClothInspector.ToolMode.Settings)
				{
					Tools.current = Tool.None;
				}
				SceneView.RepaintAll();
				this.SetupMeshColors();
			}
			switch (ClothInspector.s_ToolMode)
			{
			case ClothInspector.ToolMode.Select:
				this.SelectionGUI();
				break;
			case ClothInspector.ToolMode.Paint:
				this.PaintGUI();
				break;
			case ClothInspector.ToolMode.Settings:
				base.DrawDefaultInspector();
				break;
			}
		}
		private int GetMouseVertex(Event e)
		{
			SkinnedCloth skinnedCloth = (SkinnedCloth)this.target;
			SkinnedMeshRenderer component = skinnedCloth.GetComponent<SkinnedMeshRenderer>();
			Vector3[] normals = skinnedCloth.normals;
			ClothSkinningCoefficient[] coefficients = skinnedCloth.coefficients;
			Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
			float num = 1000f;
			int result = -1;
			Quaternion rotation = component.actualRootBone.rotation;
			bool flag = false;
			if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.renderMode == DrawCameraMode.Wireframe)
			{
				flag = true;
			}
			for (int i = 0; i < coefficients.Length; i++)
			{
				Vector3 lhs = this.m_LastVertices[i] - ray.origin;
				float sqrMagnitude = Vector3.Cross(lhs, ray.direction).sqrMagnitude;
				if ((Vector3.Dot(rotation * normals[i], Camera.current.transform.forward) <= 0f || flag) && sqrMagnitude < num && sqrMagnitude < 0.00250000018f)
				{
					num = sqrMagnitude;
					result = i;
				}
			}
			return result;
		}
		private void DrawVertices()
		{
			if (this.SelectionMeshDirty())
			{
				this.GenerateSelectionMesh();
			}
			for (int i = 0; i < ClothInspector.s_SelectionMaterial.passCount; i++)
			{
				ClothInspector.s_SelectionMaterial.SetPass(i);
				Graphics.DrawMeshNow(this.m_SelectionMesh, Matrix4x4.identity);
			}
			if (this.m_MouseOver != -1)
			{
				Matrix4x4 matrix = Matrix4x4.TRS(this.m_LastVertices[this.m_MouseOver], Quaternion.identity, 0.02f * Vector3.one);
				ClothInspector.s_SelectionMaterial.color = this.m_SelectionMesh.colors[this.m_MouseOver * this.m_MeshVerticesPerSelectionVertex];
				for (int j = 0; j < ClothInspector.s_SelectionMaterial.passCount; j++)
				{
					ClothInspector.s_SelectionMaterial.SetPass(j);
					Graphics.DrawMeshNow(this.m_VertexMesh, matrix);
				}
				ClothInspector.s_SelectionMaterial.color = Color.white;
			}
		}
		private bool UpdateRectSelection()
		{
			bool result = false;
			SkinnedCloth skinnedCloth = (SkinnedCloth)this.target;
			SkinnedMeshRenderer component = skinnedCloth.GetComponent<SkinnedMeshRenderer>();
			Vector3[] normals = skinnedCloth.normals;
			ClothSkinningCoefficient[] coefficients = skinnedCloth.coefficients;
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
			bool flag = false;
			if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.renderMode == DrawCameraMode.Wireframe)
			{
				flag = true;
			}
			for (int i = 0; i < coefficients.Length; i++)
			{
				Vector3 inPt = this.m_LastVertices[i];
				bool flag2 = Vector3.Dot(rotation * normals[i], Camera.current.transform.forward) <= 0f;
				bool flag3 = plane.GetSide(inPt) && plane2.GetSide(inPt) && plane3.GetSide(inPt) && plane4.GetSide(inPt);
				flag3 = (flag3 && (flag || flag2));
				if (this.m_RectSelection[i] != flag3)
				{
					this.m_RectSelection[i] = flag3;
					result = true;
				}
			}
			return result;
		}
		private void ApplyRectSelection()
		{
			SkinnedCloth skinnedCloth = (SkinnedCloth)this.target;
			ClothSkinningCoefficient[] coefficients = skinnedCloth.coefficients;
			for (int i = 0; i < coefficients.Length; i++)
			{
				switch (this.m_RectSelectionMode)
				{
				case ClothInspector.RectSelectionMode.Replace:
					this.m_Selection[i] = this.m_RectSelection[i];
					break;
				case ClothInspector.RectSelectionMode.Add:
					this.m_Selection[i] |= this.m_RectSelection[i];
					break;
				case ClothInspector.RectSelectionMode.Substract:
					this.m_Selection[i] = (this.m_Selection[i] && !this.m_RectSelection[i]);
					break;
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
			if (this.m_RectSelectionMode != rectSelectionMode)
			{
				this.m_RectSelectionMode = rectSelectionMode;
				return true;
			}
			return false;
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
					this.SetupMeshColors();
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
					else
					{
						if (!this.m_DidSelect && !current.alt && !current.control && !current.command)
						{
							SkinnedCloth skinnedCloth = (SkinnedCloth)this.target;
							ClothSkinningCoefficient[] coefficients = skinnedCloth.coefficients;
							for (int j = 0; j < coefficients.Length; j++)
							{
								this.m_Selection[j] = false;
							}
						}
					}
					this.SetupMeshColors();
					base.Repaint();
				}
				return;
			case EventType.MouseMove:
				IL_26:
				if (typeForControl != EventType.ExecuteCommand)
				{
					return;
				}
				if (this.m_RectSelecting && current.commandName == "ModifierKeysChanged" && (this.RectSelectionModeFromEvent() || this.UpdateRectSelection()))
				{
					this.SetupMeshColors();
				}
				return;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					if (!this.m_RectSelecting && (current.mousePosition - this.m_SelectStartPoint).magnitude > 2f && !current.alt && !current.control && !current.command)
					{
						EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendCommandsOnModifierKeys));
						this.m_RectSelecting = true;
						this.RectSelectionModeFromEvent();
						this.SetupMeshColors();
					}
					if (this.m_RectSelecting)
					{
						this.m_SelectMousePoint = new Vector2(Mathf.Max(current.mousePosition.x, 0f), Mathf.Max(current.mousePosition.y, 0f));
						if (this.RectSelectionModeFromEvent() || this.UpdateRectSelection())
						{
							this.SetupMeshColors();
						}
						current.Use();
					}
				}
				return;
			}
			goto IL_26;
		}
		private void PaintPreSceneGUI(int id)
		{
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(id);
			if (typeForControl == EventType.MouseDown || typeForControl == EventType.MouseDrag)
			{
				SkinnedCloth skinnedCloth = (SkinnedCloth)this.target;
				ClothSkinningCoefficient[] coefficients = skinnedCloth.coefficients;
				if (GUIUtility.hotControl != id && (current.alt || current.control || current.command || current.button != 0))
				{
					return;
				}
				if (typeForControl == EventType.MouseDown)
				{
					GUIUtility.hotControl = id;
				}
				int mouseVertex = this.GetMouseVertex(current);
				if (mouseVertex != -1)
				{
					bool flag = false;
					if (this.m_PaintMaxDistanceEnabled && coefficients[mouseVertex].maxDistance != this.m_PaintMaxDistance)
					{
						coefficients[mouseVertex].maxDistance = this.m_PaintMaxDistance;
						flag = true;
					}
					if (this.m_PaintMaxDistanceBiasEnabled && coefficients[mouseVertex].maxDistanceBias != this.m_PaintMaxDistanceBias)
					{
						coefficients[mouseVertex].maxDistanceBias = this.m_PaintMaxDistanceBias;
						flag = true;
					}
					if (this.m_PaintCollisionSphereRadiusEnabled && coefficients[mouseVertex].collisionSphereRadius != this.m_PaintCollisionSphereRadius)
					{
						coefficients[mouseVertex].collisionSphereRadius = this.m_PaintCollisionSphereRadius;
						flag = true;
					}
					if (this.m_PaintCollisionSphereDistanceEnabled && coefficients[mouseVertex].collisionSphereDistance != this.m_PaintCollisionSphereDistance)
					{
						coefficients[mouseVertex].collisionSphereDistance = this.m_PaintCollisionSphereDistance;
						flag = true;
					}
					if (flag)
					{
						skinnedCloth.coefficients = coefficients;
						this.SetupMeshColors();
						base.Repaint();
					}
				}
				current.Use();
			}
			else
			{
				if (typeForControl == EventType.MouseUp && GUIUtility.hotControl == id && current.button == 0)
				{
					GUIUtility.hotControl = 0;
					current.Use();
				}
			}
		}
		public void OnPreSceneGUI()
		{
			if (ClothInspector.s_ToolMode == ClothInspector.ToolMode.Settings)
			{
				return;
			}
			Handles.BeginGUI();
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(controlID);
			if (typeForControl != EventType.MouseMove)
			{
				if (typeForControl == EventType.Layout)
				{
					HandleUtility.AddDefaultControl(controlID);
				}
			}
			else
			{
				int mouseOver = this.m_MouseOver;
				this.m_MouseOver = this.GetMouseVertex(current);
				if (this.m_MouseOver != mouseOver)
				{
					SceneView.RepaintAll();
				}
			}
			ClothInspector.ToolMode toolMode = ClothInspector.s_ToolMode;
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
		public void OnSceneGUI()
		{
			if (ClothInspector.s_ToolMode == ClothInspector.ToolMode.Settings)
			{
				return;
			}
			if (Event.current.type == EventType.Repaint)
			{
				this.DrawVertices();
			}
			Handles.BeginGUI();
			if (this.m_RectSelecting && ClothInspector.s_ToolMode == ClothInspector.ToolMode.Select && Event.current.type == EventType.Repaint)
			{
				EditorStyles.selectionRect.Draw(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), GUIContent.none, false, false, false, false);
			}
			Handles.EndGUI();
		}
	}
}
