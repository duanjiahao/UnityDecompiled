using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.U2D.Interface;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D.Interface;

namespace UnityEditor
{
	internal class ShapeEditor
	{
		public delegate float DistanceToControl(Vector3 pos, Quaternion rotation, float handleSize);

		internal enum SelectionType
		{
			Normal,
			Additive,
			Subtractive
		}

		internal enum Tool
		{
			Edit,
			Create,
			Break
		}

		internal enum TangentMode
		{
			Linear,
			Continuous,
			Broken
		}

		private enum ColorEnum
		{
			EUnselected,
			EUnselectedHovered,
			ESelected,
			ESelectedHovered
		}

		private class DrawBatchDataKey
		{
			private int m_Hash;

			public Color color
			{
				get;
				private set;
			}

			public int glMode
			{
				get;
				private set;
			}

			public DrawBatchDataKey(Color c, int mode)
			{
				this.color = c;
				this.glMode = mode;
				this.m_Hash = (this.glMode ^ this.color.GetHashCode() << 2);
			}

			public override int GetHashCode()
			{
				return this.m_Hash;
			}

			public override bool Equals(object obj)
			{
				return this.m_Hash == obj.GetHashCode();
			}
		}

		public Func<int, Vector3> GetPointPosition = (int i) => Vector3.zero;

		public Action<int, Vector3> SetPointPosition = delegate(int i, Vector3 p)
		{
		};

		public Func<int, Vector3> GetPointLTangent = (int i) => Vector3.zero;

		public Action<int, Vector3> SetPointLTangent = delegate(int i, Vector3 p)
		{
		};

		public Func<int, Vector3> GetPointRTangent = (int i) => Vector3.zero;

		public Action<int, Vector3> SetPointRTangent = delegate(int i, Vector3 p)
		{
		};

		public Func<int, ShapeEditor.TangentMode> GetTangentMode = (int i) => ShapeEditor.TangentMode.Linear;

		public Action<int, ShapeEditor.TangentMode> SetTangentMode = delegate(int i, ShapeEditor.TangentMode m)
		{
		};

		public Action<int, Vector3> InsertPointAt = delegate(int i, Vector3 p)
		{
		};

		public Action<int> RemovePointAt = delegate(int i)
		{
		};

		public Func<int> GetPointsCount = () => 0;

		public Func<Vector2, Vector3> ScreenToLocal = (Vector2 i) => i;

		public Func<Vector3, Vector2> LocalToScreen = (Vector3 i) => i;

		public Func<Matrix4x4> LocalToWorldMatrix = () => Matrix4x4.identity;

		public Func<ShapeEditor.DistanceToControl> DistanceToRectangle = delegate
		{
			if (ShapeEditor.<>f__mg$cache0 == null)
			{
				ShapeEditor.<>f__mg$cache0 = new ShapeEditor.DistanceToControl(HandleUtility.DistanceToRectangle);
			}
			return ShapeEditor.<>f__mg$cache0;
		};

		public Func<ShapeEditor.DistanceToControl> DistanceToDiamond = delegate
		{
			if (ShapeEditor.<>f__mg$cache1 == null)
			{
				ShapeEditor.<>f__mg$cache1 = new ShapeEditor.DistanceToControl(HandleUtility.DistanceToDiamond);
			}
			return ShapeEditor.<>f__mg$cache1;
		};

		public Func<ShapeEditor.DistanceToControl> DistanceToCircle = delegate
		{
			if (ShapeEditor.<>f__mg$cache2 == null)
			{
				ShapeEditor.<>f__mg$cache2 = new ShapeEditor.DistanceToControl(ShapeEditor.DistanceToCircleInternal);
			}
			return ShapeEditor.<>f__mg$cache2;
		};

		public Action Repaint = delegate
		{
		};

		public Action RecordUndo = delegate
		{
		};

		public Func<Vector3, Vector3> Snap = (Vector3 i) => i;

		public Action<Bounds> Frame = delegate(Bounds b)
		{
		};

		public Action<int> OnPointClick = delegate(int i)
		{
		};

		public Func<bool> OpenEnded = () => false;

		public Func<float> GetHandleSize = () => 5f;

		private ShapeEditorSelection m_Selection;

		private Vector2 m_MousePositionLastMouseDown;

		private int m_ActivePointOnLastMouseDown = -1;

		private int m_NewPointIndex = -1;

		private Vector3 m_EdgeDragStartMousePosition;

		private Vector3 m_EdgeDragStartP0;

		private Vector3 m_EdgeDragStartP1;

		private bool m_NewPointDragFinished;

		private int m_ActiveEdge = -1;

		private bool m_DelayedReset = false;

		private HashSet<ShapeEditor> m_ShapeEditorListeners = new HashSet<ShapeEditor>();

		private ShapeEditorRectSelectionTool m_RectSelectionTool;

		private int m_MouseClosestEdge = -1;

		private float m_MouseClosestEdgeDist = 3.40282347E+38f;

		private int m_ShapeEditorRegisteredTo = 0;

		private int m_ShapeEditorUpdateDone = 0;

		private Dictionary<ShapeEditor.DrawBatchDataKey, List<Vector3>> m_DrawBatch;

		private Vector3[][] m_EdgePoints;

		private static readonly Color[] k_OutlineColor = new Color[]
		{
			Color.gray,
			Color.white,
			new Color(0.13333334f, 0.670588255f, 1f),
			Color.white
		};

		private static readonly Color[] k_FillColor = new Color[]
		{
			Color.white,
			new Color(0.5137255f, 0.8627451f, 1f),
			new Color(0.13333334f, 0.670588255f, 1f),
			new Color(0.13333334f, 0.670588255f, 1f)
		};

		private static readonly Color k_TangentColor = new Color(0.13333334f, 0.670588255f, 1f);

		private static readonly Color k_TangentColorAlternative = new Color(0.5137255f, 0.8627451f, 1f);

		private const float k_EdgeHoverDistance = 9f;

		private const float k_EdgeWidth = 2f;

		private const float k_ActiveEdgeWidth = 6f;

		private const float k_MinExistingPointDistanceForInsert = 20f;

		private readonly int k_CreatorID;

		private readonly int k_EdgeID;

		private readonly int k_RightTangentID;

		private readonly int k_LeftTangentID;

		private const int k_BezierPatch = 40;

		[CompilerGenerated]
		private static ShapeEditor.DistanceToControl <>f__mg$cache0;

		[CompilerGenerated]
		private static ShapeEditor.DistanceToControl <>f__mg$cache1;

		[CompilerGenerated]
		private static ShapeEditor.DistanceToControl <>f__mg$cache2;

		[CompilerGenerated]
		private static ShapeEditor.DistanceToControl <>f__mg$cache3;

		public ITexture2D lineTexture
		{
			get;
			set;
		}

		public int activePoint
		{
			get;
			set;
		}

		public HashSet<int> selectedPoints
		{
			get
			{
				return this.m_Selection.indices;
			}
		}

		public bool inEditMode
		{
			get;
			set;
		}

		public int activeEdge
		{
			get
			{
				return this.m_ActiveEdge;
			}
			set
			{
				this.m_ActiveEdge = value;
			}
		}

		public bool delayedReset
		{
			set
			{
				this.m_DelayedReset = value;
			}
		}

		private static Color handleOutlineColor
		{
			get;
			set;
		}

		private static Color handleFillColor
		{
			get;
			set;
		}

		private Quaternion handleMatrixrotation
		{
			get
			{
				return Quaternion.LookRotation(this.handles.matrix.GetColumn(2), this.handles.matrix.GetColumn(1));
			}
		}

		private IGUIUtility guiUtility
		{
			get;
			set;
		}

		private IEventSystem eventSystem
		{
			get;
			set;
		}

		private IEvent currentEvent
		{
			get;
			set;
		}

		private IGL glSystem
		{
			get;
			set;
		}

		private IHandles handles
		{
			get;
			set;
		}

		public ShapeEditor(IGUIUtility gu, IEventSystem es)
		{
			this.m_Selection = new ShapeEditorSelection(this);
			this.guiUtility = gu;
			this.eventSystem = es;
			this.k_CreatorID = this.guiUtility.GetPermanentControlID();
			this.k_EdgeID = this.guiUtility.GetPermanentControlID();
			this.k_RightTangentID = this.guiUtility.GetPermanentControlID();
			this.k_LeftTangentID = this.guiUtility.GetPermanentControlID();
			this.glSystem = GLSystem.GetSystem();
			this.handles = HandlesSystem.GetSystem();
		}

		public void SetRectSelectionTool(ShapeEditorRectSelectionTool sers)
		{
			if (this.m_RectSelectionTool != null)
			{
				this.m_RectSelectionTool.RectSelect -= new Action<Rect, ShapeEditor.SelectionType>(this.SelectPointsInRect);
				this.m_RectSelectionTool.ClearSelection -= new Action(this.ClearSelectedPoints);
			}
			this.m_RectSelectionTool = sers;
			this.m_RectSelectionTool.RectSelect += new Action<Rect, ShapeEditor.SelectionType>(this.SelectPointsInRect);
			this.m_RectSelectionTool.ClearSelection += new Action(this.ClearSelectedPoints);
		}

		public void OnDisable()
		{
			this.m_RectSelectionTool.RectSelect -= new Action<Rect, ShapeEditor.SelectionType>(this.SelectPointsInRect);
			this.m_RectSelectionTool.ClearSelection -= new Action(this.ClearSelectedPoints);
			this.m_RectSelectionTool = null;
		}

		private void PrepareDrawBatch()
		{
			if (this.currentEvent.type == EventType.Repaint)
			{
				this.m_DrawBatch = new Dictionary<ShapeEditor.DrawBatchDataKey, List<Vector3>>();
			}
		}

		private void DrawBatch()
		{
			if (this.currentEvent.type == EventType.Repaint)
			{
				HandleUtility.ApplyWireMaterial();
				this.glSystem.PushMatrix();
				this.glSystem.MultMatrix(this.handles.matrix);
				foreach (KeyValuePair<ShapeEditor.DrawBatchDataKey, List<Vector3>> current in this.m_DrawBatch)
				{
					this.glSystem.Begin(current.Key.glMode);
					this.glSystem.Color(current.Key.color);
					foreach (Vector3 current2 in current.Value)
					{
						this.glSystem.Vertex(current2);
					}
					this.glSystem.End();
				}
				this.glSystem.PopMatrix();
			}
		}

		private List<Vector3> GetDrawBatchList(ShapeEditor.DrawBatchDataKey key)
		{
			if (!this.m_DrawBatch.ContainsKey(key))
			{
				this.m_DrawBatch.Add(key, new List<Vector3>());
			}
			return this.m_DrawBatch[key];
		}

		public void OnGUI()
		{
			this.DelayedResetIfNecessary();
			this.currentEvent = this.eventSystem.current;
			if (this.currentEvent.type == EventType.MouseDown)
			{
				this.StoreMouseDownState();
			}
			Color color = this.handles.color;
			Matrix4x4 matrix = this.handles.matrix;
			this.handles.matrix = this.LocalToWorldMatrix();
			this.PrepareDrawBatch();
			this.Edges();
			if (this.inEditMode)
			{
				this.Framing();
				this.Tangents();
				this.Points();
			}
			this.DrawBatch();
			this.handles.color = color;
			this.handles.matrix = matrix;
			this.OnShapeEditorUpdateDone();
			foreach (ShapeEditor current in this.m_ShapeEditorListeners)
			{
				current.OnShapeEditorUpdateDone();
			}
		}

		private void Framing()
		{
			if (this.currentEvent.commandName == "FrameSelected" && this.m_Selection.Count > 0)
			{
				EventType type = this.currentEvent.type;
				if (type != EventType.ExecuteCommand)
				{
					if (type != EventType.ValidateCommand)
					{
						goto IL_DD;
					}
				}
				else
				{
					Bounds obj = new Bounds(this.GetPointPosition(this.selectedPoints.First<int>()), Vector3.zero);
					foreach (int current in this.selectedPoints)
					{
						obj.Encapsulate(this.GetPointPosition(current));
					}
					this.Frame(obj);
				}
				this.currentEvent.Use();
				IL_DD:;
			}
		}

		private void PrepareEdgePointList()
		{
			if (this.m_EdgePoints == null)
			{
				int num = this.GetPointsCount();
				int num2 = (!this.OpenEnded()) ? num : (num - 1);
				this.m_EdgePoints = new Vector3[num2][];
				int num3 = ShapeEditor.mod(num - 1, num2);
				for (int i = ShapeEditor.mod(num3 + 1, num); i < num; i++)
				{
					Vector3 vector = this.GetPointPosition(num3);
					Vector3 vector2 = this.GetPointPosition(i);
					if (this.GetTangentMode(num3) == ShapeEditor.TangentMode.Linear && this.GetTangentMode(i) == ShapeEditor.TangentMode.Linear)
					{
						this.m_EdgePoints[num3] = new Vector3[]
						{
							vector,
							vector2
						};
					}
					else
					{
						Vector3 startTangent = this.GetPointRTangent(num3) + vector;
						Vector3 endTangent = this.GetPointLTangent(i) + vector2;
						this.m_EdgePoints[num3] = this.handles.MakeBezierPoints(vector, vector2, startTangent, endTangent, 40);
					}
					num3 = i;
				}
			}
		}

		private float DistancePointEdge(Vector3 point, Vector3[] edge)
		{
			float num = 3.40282347E+38f;
			int num2 = edge.Length - 1;
			for (int i = 0; i < edge.Length; i++)
			{
				float num3 = HandleUtility.DistancePointLine(point, edge[num2], edge[i]);
				if (num3 < num)
				{
					num = num3;
				}
				num2 = i;
			}
			return num;
		}

		private float GetMouseClosestEdgeDistance()
		{
			float result;
			if (this.guiUtility.hotControl == this.k_CreatorID || this.guiUtility.hotControl == this.k_EdgeID)
			{
				result = -3.40282347E+38f;
			}
			else
			{
				Vector3 point = this.ScreenToLocal(this.eventSystem.current.mousePosition);
				int num = this.GetPointsCount();
				if (this.m_MouseClosestEdge == -1 && num > 0)
				{
					this.PrepareEdgePointList();
					this.m_MouseClosestEdgeDist = 3.40282347E+38f;
					int num2 = (!this.OpenEnded()) ? num : (num - 1);
					for (int i = 0; i < num2; i++)
					{
						float num3 = this.DistancePointEdge(point, this.m_EdgePoints[i]);
						if (num3 < this.m_MouseClosestEdgeDist)
						{
							this.m_MouseClosestEdge = i;
							this.m_MouseClosestEdgeDist = num3;
						}
					}
				}
				result = this.m_MouseClosestEdgeDist;
			}
			return result;
		}

		public void Edges()
		{
			float num = 3.40282347E+38f;
			if (this.m_ShapeEditorListeners.Count > 0)
			{
				num = (from se in this.m_ShapeEditorListeners
				select se.GetMouseClosestEdgeDistance()).Max();
			}
			float mouseClosestEdgeDistance = this.GetMouseClosestEdgeDistance();
			bool flag = this.EdgeDragModifiersActive() && mouseClosestEdgeDistance < 9f && mouseClosestEdgeDistance < num;
			if (this.currentEvent.type == EventType.Repaint)
			{
				Color color = this.handles.color;
				this.PrepareEdgePointList();
				int num2 = this.GetPointsCount();
				int num3 = (!this.OpenEnded()) ? num2 : (num2 - 1);
				for (int i = 0; i < num3; i++)
				{
					Color color2 = (i != this.m_ActiveEdge) ? Color.white : Color.yellow;
					float width = (i != this.m_ActiveEdge && (this.m_MouseClosestEdge != i || !flag)) ? 2f : 6f;
					this.handles.color = color2;
					this.handles.DrawAAPolyLine(this.lineTexture, width, this.m_EdgePoints[i]);
				}
				this.handles.color = color;
			}
			if (this.inEditMode)
			{
				if (num > mouseClosestEdgeDistance)
				{
					bool flag2 = this.MouseDistanceToPoint(this.FindClosestPointToMouse()) > 20f;
					bool flag3 = this.MouseDistanceToClosestTangent() > 20f;
					bool flag4 = flag2 && flag3;
					bool flag5 = this.m_MouseClosestEdgeDist < 9f;
					bool flag6 = flag5 && flag4 && !this.m_RectSelectionTool.isSelecting;
					if (GUIUtility.hotControl == this.k_EdgeID || (this.EdgeDragModifiersActive() && flag6))
					{
						this.HandleEdgeDragging(this.m_MouseClosestEdge);
					}
					else if (GUIUtility.hotControl == this.k_CreatorID || (this.currentEvent.modifiers == EventModifiers.None && flag6))
					{
						this.HandlePointInsertToEdge(this.m_MouseClosestEdge, this.m_MouseClosestEdgeDist);
					}
				}
			}
			if (this.guiUtility.hotControl != this.k_CreatorID && this.m_NewPointIndex != -1)
			{
				this.m_NewPointDragFinished = true;
				this.guiUtility.keyboardControl = 0;
				this.m_NewPointIndex = -1;
			}
			if (this.guiUtility.hotControl != this.k_EdgeID && this.m_ActiveEdge != -1)
			{
				this.m_ActiveEdge = -1;
			}
		}

		public void Tangents()
		{
			if (this.activePoint >= 0 && this.m_Selection.Count <= 1 && this.GetTangentMode(this.activePoint) != ShapeEditor.TangentMode.Linear)
			{
				IEvent current = this.eventSystem.current;
				Vector3 vector = this.GetPointPosition(this.activePoint);
				Vector3 vector2 = this.GetPointLTangent(this.activePoint);
				Vector3 vector3 = this.GetPointRTangent(this.activePoint);
				bool flag = this.guiUtility.hotControl == this.k_RightTangentID || this.guiUtility.hotControl == this.k_LeftTangentID;
				bool flag2 = vector2.sqrMagnitude == 0f && vector3.sqrMagnitude == 0f;
				if (flag || !flag2)
				{
					ShapeEditor.TangentMode tangentMode = this.GetTangentMode(this.activePoint);
					bool flag3 = current.GetTypeForControl(this.k_RightTangentID) == EventType.MouseDown || current.GetTypeForControl(this.k_LeftTangentID) == EventType.MouseDown;
					bool flag4 = current.GetTypeForControl(this.k_RightTangentID) == EventType.MouseUp || current.GetTypeForControl(this.k_LeftTangentID) == EventType.MouseUp;
					Vector3 vector4 = this.DoTangent(vector, vector + vector2, this.k_LeftTangentID, this.activePoint, ShapeEditor.k_TangentColor);
					Vector3 vector5 = this.DoTangent(vector, vector + vector3, this.k_RightTangentID, this.activePoint, (this.GetTangentMode(this.activePoint) != ShapeEditor.TangentMode.Broken) ? ShapeEditor.k_TangentColor : ShapeEditor.k_TangentColorAlternative);
					bool flag5 = vector4 != vector2 || vector5 != vector3;
					flag2 = (vector4.sqrMagnitude == 0f && vector5.sqrMagnitude == 0f);
					if (flag && flag3)
					{
						int num = (int)((tangentMode + 1) % (ShapeEditor.TangentMode)3);
						tangentMode = (ShapeEditor.TangentMode)num;
						this.SetTangentMode(this.activePoint, tangentMode);
					}
					if (flag4 && flag2)
					{
						this.SetTangentMode(this.activePoint, ShapeEditor.TangentMode.Linear);
						flag5 = true;
					}
					if (flag5)
					{
						this.RecordUndo();
						this.SetPointLTangent(this.activePoint, vector4);
						this.SetPointRTangent(this.activePoint, vector5);
						this.RefreshTangents(this.activePoint, this.guiUtility.hotControl == this.k_RightTangentID);
						this.Repaint();
					}
				}
			}
		}

		public void Points()
		{
			bool flag = (UnityEngine.Event.current.type == EventType.ExecuteCommand || UnityEngine.Event.current.type == EventType.ValidateCommand) && (UnityEngine.Event.current.commandName == "SoftDelete" || UnityEngine.Event.current.commandName == "Delete");
			for (int i = 0; i < this.GetPointsCount(); i++)
			{
				if (i != this.m_NewPointIndex)
				{
					Vector3 vector = this.GetPointPosition(i);
					int controlID = this.guiUtility.GetControlID(5353, FocusType.Keyboard);
					bool flag2 = this.currentEvent.GetTypeForControl(controlID) == EventType.MouseDown;
					bool flag3 = this.currentEvent.GetTypeForControl(controlID) == EventType.MouseUp;
					EditorGUI.BeginChangeCheck();
					if (this.currentEvent.type == EventType.Repaint)
					{
						ShapeEditor.ColorEnum colorForPoint = this.GetColorForPoint(i, controlID);
						ShapeEditor.handleOutlineColor = ShapeEditor.k_OutlineColor[(int)colorForPoint];
						ShapeEditor.handleFillColor = ShapeEditor.k_FillColor[(int)colorForPoint];
					}
					Vector3 vector2 = vector;
					int hotControl = this.guiUtility.hotControl;
					if (!this.currentEvent.alt || this.guiUtility.hotControl == controlID)
					{
						vector2 = ShapeEditor.DoSlider(controlID, vector, Vector3.up, Vector3.right, this.GetHandleSizeForPoint(i), this.GetCapForPoint(i));
					}
					else if (this.currentEvent.type == EventType.Repaint)
					{
						this.GetCapForPoint(i)(controlID, vector, Quaternion.LookRotation(Vector3.forward, Vector3.up), this.GetHandleSizeForPoint(i), this.currentEvent.type);
					}
					int hotControl2 = this.guiUtility.hotControl;
					if (flag3 && hotControl == controlID && hotControl2 == 0 && this.currentEvent.mousePosition == this.m_MousePositionLastMouseDown && !this.currentEvent.shift)
					{
						this.HandlePointClick(i);
					}
					if (EditorGUI.EndChangeCheck())
					{
						this.RecordUndo();
						vector2 = this.Snap(vector2);
						this.MoveSelections(vector2 - vector);
					}
					if (this.guiUtility.hotControl == controlID && flag2 && !this.m_Selection.Contains(i))
					{
						this.SelectPoint(i, (!this.currentEvent.shift) ? ShapeEditor.SelectionType.Normal : ShapeEditor.SelectionType.Additive);
						this.Repaint();
					}
					if (this.m_NewPointDragFinished && this.activePoint == i && controlID != -1)
					{
						this.guiUtility.keyboardControl = controlID;
						this.m_NewPointDragFinished = false;
					}
				}
			}
			if (flag)
			{
				if (this.currentEvent.type == EventType.ValidateCommand)
				{
					this.currentEvent.Use();
				}
				else if (this.currentEvent.type == EventType.ExecuteCommand)
				{
					this.RecordUndo();
					this.DeleteSelections();
					this.currentEvent.Use();
				}
			}
		}

		public void HandlePointInsertToEdge(int closestEdge, float closestEdgeDist)
		{
			bool flag = GUIUtility.hotControl == this.k_CreatorID;
			Vector3 vector = (!flag) ? this.FindClosestPointOnEdge(closestEdge, this.ScreenToLocal(this.currentEvent.mousePosition), 100) : this.GetPointPosition(this.m_NewPointIndex);
			EditorGUI.BeginChangeCheck();
			ShapeEditor.handleFillColor = ShapeEditor.k_FillColor[3];
			ShapeEditor.handleOutlineColor = ShapeEditor.k_OutlineColor[3];
			if (!flag)
			{
				ShapeEditor.handleFillColor = ShapeEditor.handleFillColor.AlphaMultiplied(0.5f);
				ShapeEditor.handleOutlineColor = ShapeEditor.handleOutlineColor.AlphaMultiplied(0.5f);
			}
			int hotControl = GUIUtility.hotControl;
			Vector3 vector2 = ShapeEditor.DoSlider(this.k_CreatorID, vector, Vector3.up, Vector3.right, this.GetHandleSizeForPoint(closestEdge), new Handles.CapFunction(this.RectCap));
			if (hotControl != this.k_CreatorID && GUIUtility.hotControl == this.k_CreatorID)
			{
				this.RecordUndo();
				this.m_NewPointIndex = ShapeEditor.NextIndex(closestEdge, this.GetPointsCount());
				this.InsertPointAt(this.m_NewPointIndex, vector2);
				this.SelectPoint(this.m_NewPointIndex, ShapeEditor.SelectionType.Normal);
			}
			else if (EditorGUI.EndChangeCheck())
			{
				this.RecordUndo();
				vector2 = this.Snap(vector2);
				this.MoveSelections(vector2 - vector);
			}
		}

		private void HandleEdgeDragging(int closestEdge)
		{
			EventType type = this.currentEvent.type;
			if (type != EventType.MouseDown)
			{
				if (type != EventType.MouseDrag)
				{
					if (type == EventType.MouseUp)
					{
						this.m_ActiveEdge = -1;
						GUIUtility.hotControl = 0;
						this.currentEvent.Use();
					}
				}
				else
				{
					this.RecordUndo();
					Vector3 a = this.ScreenToLocal(this.currentEvent.mousePosition);
					Vector3 b = a - this.m_EdgeDragStartMousePosition;
					Vector3 b2 = this.GetPointPosition(this.m_ActiveEdge);
					Vector3 vector = this.m_EdgeDragStartP0 + b;
					vector = this.Snap(vector);
					Vector3 b3 = vector - b2;
					int activeEdge = this.m_ActiveEdge;
					int num = ShapeEditor.NextIndex(this.m_ActiveEdge, this.GetPointsCount());
					this.SetPointPosition(this.m_ActiveEdge, this.GetPointPosition(activeEdge) + b3);
					this.SetPointPosition(num, this.GetPointPosition(num) + b3);
					this.currentEvent.Use();
				}
			}
			else
			{
				this.m_ActiveEdge = closestEdge;
				this.m_EdgeDragStartP0 = this.GetPointPosition(this.m_ActiveEdge);
				this.m_EdgeDragStartP1 = this.GetPointPosition(ShapeEditor.NextIndex(this.m_ActiveEdge, this.GetPointsCount()));
				if (this.currentEvent.shift)
				{
					this.RecordUndo();
					this.InsertPointAt(this.m_ActiveEdge + 1, this.m_EdgeDragStartP0);
					this.InsertPointAt(this.m_ActiveEdge + 2, this.m_EdgeDragStartP1);
					this.m_ActiveEdge++;
				}
				this.m_EdgeDragStartMousePosition = this.ScreenToLocal(this.currentEvent.mousePosition);
				GUIUtility.hotControl = this.k_EdgeID;
				this.currentEvent.Use();
			}
		}

		private Vector3 DoTangent(Vector3 p0, Vector3 t0, int cid, int pointIndex, Color color)
		{
			float handleSizeForPoint = this.GetHandleSizeForPoint(pointIndex);
			float tangentSizeForPoint = this.GetTangentSizeForPoint(pointIndex);
			this.handles.color = color;
			float num = HandleUtility.DistanceToCircle(t0, tangentSizeForPoint);
			if (this.lineTexture != null)
			{
				this.handles.DrawAAPolyLine(this.lineTexture, new Vector3[]
				{
					p0,
					t0
				});
			}
			else
			{
				this.handles.DrawLine(p0, t0);
			}
			ShapeEditor.handleOutlineColor = ((num <= 0f) ? ShapeEditor.k_OutlineColor[3] : color);
			ShapeEditor.handleFillColor = color;
			Vector3 vector = ShapeEditor.DoSlider(cid, t0, Vector3.up, Vector3.right, tangentSizeForPoint, this.GetCapForTangent(pointIndex)) - p0;
			return (vector.magnitude >= handleSizeForPoint) ? vector : Vector3.zero;
		}

		public void HandlePointClick(int pointIndex)
		{
			if (this.m_Selection.Count > 1)
			{
				this.m_Selection.SelectPoint(pointIndex, ShapeEditor.SelectionType.Normal);
			}
			else if (!this.currentEvent.control && !this.currentEvent.shift && this.m_ActivePointOnLastMouseDown == this.activePoint)
			{
				this.OnPointClick(pointIndex);
			}
		}

		public void CycleTangentMode()
		{
			ShapeEditor.TangentMode tangentMode = this.GetTangentMode(this.activePoint);
			ShapeEditor.TangentMode nextTangentMode = ShapeEditor.GetNextTangentMode(tangentMode);
			this.SetTangentMode(this.activePoint, nextTangentMode);
			this.RefreshTangentsAfterModeChange(this.activePoint, tangentMode, nextTangentMode);
		}

		public static ShapeEditor.TangentMode GetNextTangentMode(ShapeEditor.TangentMode current)
		{
			return (current + 1) % (ShapeEditor.TangentMode)Enum.GetValues(typeof(ShapeEditor.TangentMode)).Length;
		}

		public void RefreshTangentsAfterModeChange(int pointIndex, ShapeEditor.TangentMode oldMode, ShapeEditor.TangentMode newMode)
		{
			if (oldMode != ShapeEditor.TangentMode.Linear && newMode == ShapeEditor.TangentMode.Linear)
			{
				this.SetPointLTangent(pointIndex, Vector3.zero);
				this.SetPointRTangent(pointIndex, Vector3.zero);
			}
			if (newMode == ShapeEditor.TangentMode.Continuous)
			{
				if (oldMode == ShapeEditor.TangentMode.Broken)
				{
					this.SetPointRTangent(pointIndex, this.GetPointLTangent(pointIndex) * -1f);
				}
				if (oldMode == ShapeEditor.TangentMode.Linear)
				{
					this.FromAllZeroToTangents(pointIndex);
				}
			}
		}

		private ShapeEditor.ColorEnum GetColorForPoint(int pointIndex, int handleID)
		{
			bool flag = this.MouseDistanceToPoint(pointIndex) <= 0f;
			bool flag2 = this.m_Selection.Contains(pointIndex);
			ShapeEditor.ColorEnum result;
			if ((flag && flag2) || GUIUtility.hotControl == handleID)
			{
				result = ShapeEditor.ColorEnum.ESelectedHovered;
			}
			else if (flag)
			{
				result = ShapeEditor.ColorEnum.EUnselectedHovered;
			}
			else if (flag2)
			{
				result = ShapeEditor.ColorEnum.ESelected;
			}
			else
			{
				result = ShapeEditor.ColorEnum.EUnselected;
			}
			return result;
		}

		private void FromAllZeroToTangents(int pointIndex)
		{
			Vector3 vector = this.GetPointPosition(pointIndex);
			int arg = (pointIndex <= 0) ? (this.GetPointsCount() - 1) : (pointIndex - 1);
			Vector3 vector2 = (this.GetPointPosition(arg) - vector) * 0.33f;
			Vector3 vector3 = -vector2;
			float magnitude = (this.LocalToScreen(vector) - this.LocalToScreen(vector + vector2)).magnitude;
			float magnitude2 = (this.LocalToScreen(vector) - this.LocalToScreen(vector + vector3)).magnitude;
			vector2 *= Mathf.Min(100f / magnitude, 1f);
			vector3 *= Mathf.Min(100f / magnitude2, 1f);
			this.SetPointLTangent(pointIndex, vector2);
			this.SetPointRTangent(pointIndex, vector3);
		}

		private Handles.CapFunction GetCapForTangent(int index)
		{
			Handles.CapFunction result;
			if (this.GetTangentMode(index) == ShapeEditor.TangentMode.Continuous)
			{
				result = new Handles.CapFunction(this.CircleCap);
			}
			else
			{
				result = new Handles.CapFunction(this.DiamondCap);
			}
			return result;
		}

		private ShapeEditor.DistanceToControl GetDistanceFuncForTangent(int index)
		{
			ShapeEditor.DistanceToControl result;
			if (this.GetTangentMode(index) == ShapeEditor.TangentMode.Continuous)
			{
				result = this.DistanceToCircle();
			}
			else
			{
				if (ShapeEditor.<>f__mg$cache3 == null)
				{
					ShapeEditor.<>f__mg$cache3 = new ShapeEditor.DistanceToControl(HandleUtility.DistanceToDiamond);
				}
				result = ShapeEditor.<>f__mg$cache3;
			}
			return result;
		}

		private Handles.CapFunction GetCapForPoint(int index)
		{
			ShapeEditor.TangentMode tangentMode = this.GetTangentMode(index);
			Handles.CapFunction result;
			if (tangentMode != ShapeEditor.TangentMode.Broken)
			{
				if (tangentMode != ShapeEditor.TangentMode.Continuous)
				{
					if (tangentMode != ShapeEditor.TangentMode.Linear)
					{
						result = new Handles.CapFunction(this.DiamondCap);
					}
					else
					{
						result = new Handles.CapFunction(this.RectCap);
					}
				}
				else
				{
					result = new Handles.CapFunction(this.CircleCap);
				}
			}
			else
			{
				result = new Handles.CapFunction(this.DiamondCap);
			}
			return result;
		}

		private static float DistanceToCircleInternal(Vector3 position, Quaternion rotation, float size)
		{
			return HandleUtility.DistanceToCircle(position, size);
		}

		private float GetHandleSizeForPoint(int index)
		{
			return (!(Camera.current != null)) ? this.GetHandleSize() : (HandleUtility.GetHandleSize(this.GetPointPosition(index)) * 0.075f);
		}

		private float GetTangentSizeForPoint(int index)
		{
			return this.GetHandleSizeForPoint(index) * 0.8f;
		}

		private void RefreshTangents(int index, bool rightIsActive)
		{
			ShapeEditor.TangentMode tangentMode = this.GetTangentMode(index);
			Vector3 vector = this.GetPointLTangent(index);
			Vector3 vector2 = this.GetPointRTangent(index);
			if (tangentMode == ShapeEditor.TangentMode.Continuous)
			{
				if (rightIsActive)
				{
					vector = -vector2;
					float magnitude = vector.magnitude;
					vector = vector.normalized * magnitude;
				}
				else
				{
					vector2 = -vector;
					float magnitude2 = vector2.magnitude;
					vector2 = vector2.normalized * magnitude2;
				}
			}
			this.SetPointLTangent(this.activePoint, vector);
			this.SetPointRTangent(this.activePoint, vector2);
		}

		private void StoreMouseDownState()
		{
			this.m_MousePositionLastMouseDown = this.currentEvent.mousePosition;
			this.m_ActivePointOnLastMouseDown = this.activePoint;
		}

		private void DelayedResetIfNecessary()
		{
			if (this.m_DelayedReset)
			{
				this.guiUtility.hotControl = 0;
				this.guiUtility.keyboardControl = 0;
				this.m_Selection.Clear();
				this.activePoint = -1;
				this.m_DelayedReset = false;
			}
		}

		public Vector3 FindClosestPointOnEdge(int edgeIndex, Vector3 position, int iterations)
		{
			float num = 1f / (float)iterations;
			float num2 = 3.40282347E+38f;
			float index = (float)edgeIndex;
			for (float num3 = 0f; num3 <= 1f; num3 += num)
			{
				Vector3 positionByIndex = this.GetPositionByIndex((float)edgeIndex + num3);
				float sqrMagnitude = (position - positionByIndex).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					index = (float)edgeIndex + num3;
				}
			}
			return this.GetPositionByIndex(index);
		}

		private Vector3 GetPositionByIndex(float index)
		{
			int num = Mathf.FloorToInt(index);
			int arg = ShapeEditor.NextIndex(num, this.GetPointsCount());
			float t = index - (float)num;
			return ShapeEditor.GetPoint(this.GetPointPosition(num), this.GetPointPosition(arg), this.GetPointRTangent(num), this.GetPointLTangent(arg), t);
		}

		private static Vector3 GetPoint(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			return num * num * num * startPosition + 3f * num * num * t * (startPosition + startTangent) + 3f * num * t * t * (endPosition + endTangent) + t * t * t * endPosition;
		}

		private int FindClosestPointToMouse()
		{
			Vector3 position = this.ScreenToLocal(this.currentEvent.mousePosition);
			return this.FindClosestPointIndex(position);
		}

		private float MouseDistanceToClosestTangent()
		{
			float result;
			if (this.activePoint < 0)
			{
				result = 3.40282347E+38f;
			}
			else
			{
				Vector3 b = this.GetPointLTangent(this.activePoint);
				Vector3 b2 = this.GetPointRTangent(this.activePoint);
				if (b.sqrMagnitude == 0f && b2.sqrMagnitude == 0f)
				{
					result = 3.40282347E+38f;
				}
				else
				{
					Vector3 a = this.GetPointPosition(this.activePoint);
					float tangentSizeForPoint = this.GetTangentSizeForPoint(this.activePoint);
					result = Mathf.Min(HandleUtility.DistanceToRectangle(a + b, Quaternion.identity, tangentSizeForPoint), HandleUtility.DistanceToRectangle(a + b2, Quaternion.identity, tangentSizeForPoint));
				}
			}
			return result;
		}

		private int FindClosestPointIndex(Vector3 position)
		{
			float num = 3.40282347E+38f;
			int result = -1;
			for (int i = 0; i < this.GetPointsCount(); i++)
			{
				Vector3 a = this.GetPointPosition(i);
				float sqrMagnitude = (a - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = i;
					num = sqrMagnitude;
				}
			}
			return result;
		}

		private ShapeEditor.DistanceToControl GetDistanceFuncForPoint(int index)
		{
			ShapeEditor.TangentMode tangentMode = this.GetTangentMode(index);
			ShapeEditor.DistanceToControl result;
			if (tangentMode != ShapeEditor.TangentMode.Broken)
			{
				if (tangentMode != ShapeEditor.TangentMode.Continuous)
				{
					if (tangentMode != ShapeEditor.TangentMode.Linear)
					{
						result = this.DistanceToRectangle();
					}
					else
					{
						result = this.DistanceToRectangle();
					}
				}
				else
				{
					result = this.DistanceToCircle();
				}
			}
			else
			{
				result = this.DistanceToDiamond();
			}
			return result;
		}

		private float MouseDistanceToPoint(int index)
		{
			ShapeEditor.TangentMode tangentMode = this.GetTangentMode(index);
			float result;
			if (tangentMode != ShapeEditor.TangentMode.Broken)
			{
				if (tangentMode != ShapeEditor.TangentMode.Linear)
				{
					if (tangentMode != ShapeEditor.TangentMode.Continuous)
					{
						result = 3.40282347E+38f;
					}
					else
					{
						result = HandleUtility.DistanceToCircle(this.GetPointPosition(index), this.GetHandleSizeForPoint(index));
					}
				}
				else
				{
					result = HandleUtility.DistanceToRectangle(this.GetPointPosition(index), Quaternion.identity, this.GetHandleSizeForPoint(index));
				}
			}
			else
			{
				result = HandleUtility.DistanceToDiamond(this.GetPointPosition(index), Quaternion.identity, this.GetHandleSizeForPoint(index));
			}
			return result;
		}

		private bool EdgeDragModifiersActive()
		{
			return this.currentEvent.modifiers == EventModifiers.Control || this.currentEvent.modifiers == EventModifiers.Command;
		}

		private static Vector3 DoSlider(int id, Vector3 position, Vector3 slide1, Vector3 slide2, float s, Handles.CapFunction cap)
		{
			return Slider2D.Do(id, position, Vector3.zero, Vector3.Cross(slide1, slide2), slide1, slide2, s, cap, Vector2.zero, false);
		}

		public void RectCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType == EventType.Layout)
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
			}
			else if (eventType == EventType.Repaint)
			{
				Vector3 planeNormal = this.handles.matrix.GetColumn(2);
				Vector3 normalized = (ShapeEditor.ProjectPointOnPlane(planeNormal, position, position + Vector3.up) - position).normalized;
				Quaternion rotation2 = Quaternion.LookRotation(this.handles.matrix.GetColumn(2), normalized);
				Vector3 b = rotation2 * Vector3.right * size;
				Vector3 b2 = rotation2 * Vector3.up * size;
				List<Vector3> drawBatchList = this.GetDrawBatchList(new ShapeEditor.DrawBatchDataKey(ShapeEditor.handleFillColor, 4));
				drawBatchList.Add(position + b + b2);
				drawBatchList.Add(position + b - b2);
				drawBatchList.Add(position - b - b2);
				drawBatchList.Add(position - b - b2);
				drawBatchList.Add(position - b + b2);
				drawBatchList.Add(position + b + b2);
				drawBatchList = this.GetDrawBatchList(new ShapeEditor.DrawBatchDataKey(ShapeEditor.handleOutlineColor, 1));
				drawBatchList.Add(position + b + b2);
				drawBatchList.Add(position + b - b2);
				drawBatchList.Add(position + b - b2);
				drawBatchList.Add(position - b - b2);
				drawBatchList.Add(position - b - b2);
				drawBatchList.Add(position - b + b2);
				drawBatchList.Add(position - b + b2);
				drawBatchList.Add(position + b + b2);
			}
		}

		public void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType == EventType.Layout)
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
			}
			else if (eventType == EventType.Repaint)
			{
				Vector3 vector = this.handleMatrixrotation * rotation * Vector3.forward;
				Vector3 from = Vector3.Cross(vector, Vector3.up);
				if (from.sqrMagnitude < 0.001f)
				{
					from = Vector3.Cross(vector, Vector3.right);
				}
				Vector3[] array = new Vector3[60];
				this.handles.SetDiscSectionPoints(array, position, vector, from, 360f, size);
				List<Vector3> drawBatchList = this.GetDrawBatchList(new ShapeEditor.DrawBatchDataKey(ShapeEditor.handleFillColor, 4));
				for (int i = 1; i < array.Length; i++)
				{
					drawBatchList.Add(position);
					drawBatchList.Add(array[i]);
					drawBatchList.Add(array[i - 1]);
				}
				drawBatchList = this.GetDrawBatchList(new ShapeEditor.DrawBatchDataKey(ShapeEditor.handleOutlineColor, 1));
				for (int j = 0; j < array.Length - 1; j++)
				{
					drawBatchList.Add(array[j]);
					drawBatchList.Add(array[j + 1]);
				}
			}
		}

		public void DiamondCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType == EventType.Layout)
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
			}
			else if (eventType == EventType.Repaint)
			{
				Vector3 planeNormal = this.handles.matrix.GetColumn(2);
				Vector3 normalized = (ShapeEditor.ProjectPointOnPlane(planeNormal, position, position + Vector3.up) - position).normalized;
				Quaternion rotation2 = Quaternion.LookRotation(this.handles.matrix.GetColumn(2), normalized);
				Vector3 b = rotation2 * Vector3.right * size * 1.25f;
				Vector3 b2 = rotation2 * Vector3.up * size * 1.25f;
				List<Vector3> drawBatchList = this.GetDrawBatchList(new ShapeEditor.DrawBatchDataKey(ShapeEditor.handleFillColor, 4));
				drawBatchList.Add(position - b2);
				drawBatchList.Add(position + b);
				drawBatchList.Add(position - b);
				drawBatchList.Add(position - b);
				drawBatchList.Add(position + b2);
				drawBatchList.Add(position + b);
				drawBatchList = this.GetDrawBatchList(new ShapeEditor.DrawBatchDataKey(ShapeEditor.handleOutlineColor, 1));
				drawBatchList.Add(position + b);
				drawBatchList.Add(position - b2);
				drawBatchList.Add(position - b2);
				drawBatchList.Add(position - b);
				drawBatchList.Add(position - b);
				drawBatchList.Add(position + b2);
				drawBatchList.Add(position + b2);
				drawBatchList.Add(position + b);
			}
		}

		private static int NextIndex(int index, int total)
		{
			return ShapeEditor.mod(index + 1, total);
		}

		private static int PreviousIndex(int index, int total)
		{
			return ShapeEditor.mod(index - 1, total);
		}

		private static int mod(int x, int m)
		{
			int num = x % m;
			return (num >= 0) ? num : (num + m);
		}

		private static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
		{
			planeNormal.Normalize();
			float d = -Vector3.Dot(planeNormal.normalized, point - planePoint);
			return point + planeNormal * d;
		}

		public void RegisterToShapeEditor(ShapeEditor se)
		{
			this.m_ShapeEditorRegisteredTo++;
			se.m_ShapeEditorListeners.Add(this);
		}

		public void UnregisterFromShapeEditor(ShapeEditor se)
		{
			this.m_ShapeEditorRegisteredTo--;
			se.m_ShapeEditorListeners.Remove(this);
		}

		private void OnShapeEditorUpdateDone()
		{
			this.m_ShapeEditorUpdateDone++;
			if (this.m_ShapeEditorUpdateDone >= this.m_ShapeEditorRegisteredTo)
			{
				this.m_ShapeEditorUpdateDone = 0;
				this.m_MouseClosestEdge = -1;
				this.m_MouseClosestEdgeDist = 3.40282347E+38f;
				this.m_EdgePoints = null;
			}
		}

		private void ClearSelectedPoints()
		{
			this.selectedPoints.Clear();
			this.activePoint = -1;
		}

		private void SelectPointsInRect(Rect r, ShapeEditor.SelectionType st)
		{
			Rect rect = EditorGUIExt.FromToRect(this.ScreenToLocal(r.min), this.ScreenToLocal(r.max));
			this.m_Selection.RectSelect(rect, st);
		}

		private void DeleteSelections()
		{
			foreach (ShapeEditor current in this.m_ShapeEditorListeners)
			{
				current.m_Selection.DeleteSelection();
			}
			this.m_Selection.DeleteSelection();
		}

		private void MoveSelections(Vector2 distance)
		{
			foreach (ShapeEditor current in this.m_ShapeEditorListeners)
			{
				current.m_Selection.MoveSelection(distance);
			}
			this.m_Selection.MoveSelection(distance);
		}

		private void SelectPoint(int index, ShapeEditor.SelectionType st)
		{
			if (st == ShapeEditor.SelectionType.Normal)
			{
				foreach (ShapeEditor current in this.m_ShapeEditorListeners)
				{
					current.ClearSelectedPoints();
				}
			}
			this.m_Selection.SelectPoint(index, st);
		}
	}
}
