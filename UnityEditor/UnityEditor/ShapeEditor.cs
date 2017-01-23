using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

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

		private static readonly Color k_UnSelectedOutline = Color.gray;

		private static readonly Color k_UnSelectedFill = Color.white;

		private static readonly Color k_UnSelectedHoveredOutline = Color.white;

		private static readonly Color k_UnSelectedHoveredFill = new Color(0.5137255f, 0.8627451f, 1f);

		private static readonly Color k_SelectedOutline = new Color(0.13333334f, 0.670588255f, 1f);

		private static readonly Color k_SelectedFill = new Color(0.13333334f, 0.670588255f, 1f);

		private static readonly Color k_SelectedHoveredOutline = Color.white;

		private static readonly Color k_SelectedHoveredFill = new Color(0.13333334f, 0.670588255f, 1f);

		private static readonly Color k_TangentColor = new Color(0.13333334f, 0.670588255f, 1f);

		private static readonly Color k_TangentColorAlternative = new Color(0.5137255f, 0.8627451f, 1f);

		private const float k_EdgeHoverDistance = 9f;

		private const float k_EdgeWidth = 2f;

		private const float k_ActiveEdgeWidth = 6f;

		private const float k_MinExistingPointDistanceForInsert = 20f;

		private readonly int k_CreatorID = GUIUtility.GetPermanentControlID();

		private readonly int k_EdgeID = GUIUtility.GetPermanentControlID();

		private readonly int k_RightTangentID = GUIUtility.GetPermanentControlID();

		private readonly int k_LeftTangentID = GUIUtility.GetPermanentControlID();

		[CompilerGenerated]
		private static ShapeEditor.DistanceToControl <>f__mg$cache0;

		[CompilerGenerated]
		private static ShapeEditor.DistanceToControl <>f__mg$cache1;

		[CompilerGenerated]
		private static ShapeEditor.DistanceToControl <>f__mg$cache2;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache3;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache4;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache5;

		[CompilerGenerated]
		private static ShapeEditor.DistanceToControl <>f__mg$cache6;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache7;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache8;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache9;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cacheA;

		public Texture2D lineTexture
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

		private static Quaternion handleMatrixrotation
		{
			get
			{
				return Quaternion.LookRotation(Handles.matrix.GetColumn(2), Handles.matrix.GetColumn(1));
			}
		}

		public ShapeEditor()
		{
			this.m_Selection = new ShapeEditorSelection(this);
		}

		public void OnGUI()
		{
			this.DelayedResetIfNecessary();
			if (Event.current.type == EventType.MouseDown)
			{
				this.StoreMouseDownState();
			}
			Color color = Handles.color;
			Matrix4x4 matrix = Handles.matrix;
			Handles.matrix = this.LocalToWorldMatrix();
			this.Edges();
			if (this.inEditMode)
			{
				this.Framing();
				this.Tangents();
				this.Points();
				this.Selection();
			}
			Handles.color = color;
			Handles.matrix = matrix;
		}

		private void Framing()
		{
			if (Event.current.commandName == "FrameSelected" && this.m_Selection.Count > 0)
			{
				EventType type = Event.current.type;
				if (type != EventType.ExecuteCommand)
				{
					if (type != EventType.ValidateCommand)
					{
						goto IL_DA;
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
				Event.current.Use();
				IL_DA:;
			}
		}

		public void Edges()
		{
			int num = this.GetPointsCount();
			int closestEdge = -1;
			float num2 = 3.40282347E+38f;
			int num3 = 0;
			int num4 = ShapeEditor.NextIndex(num3, num);
			int num5 = (!this.OpenEnded()) ? num : (num - 1);
			for (int i = 0; i < num5; i++)
			{
				Vector3 vector = this.GetPointPosition(num3);
				Vector3 vector2 = this.GetPointPosition(num4);
				Vector3 vector3 = vector + this.GetPointRTangent(num3);
				Vector3 vector4 = vector2 + this.GetPointLTangent(num4);
				Vector2 v = this.LocalToScreen(vector);
				Vector2 v2 = this.LocalToScreen(vector2);
				Vector2 v3 = this.LocalToScreen(vector3);
				Vector2 v4 = this.LocalToScreen(vector4);
				float num6 = HandleUtility.DistancePointBezier(Event.current.mousePosition, v, v2, v3, v4);
				Color color = (num3 != this.m_ActiveEdge) ? Color.white : Color.yellow;
				float width = (num3 != this.m_ActiveEdge && (!ShapeEditor.EdgeDragModifiersActive() || num6 >= 9f)) ? 2f : 6f;
				Handles.DrawBezier(vector, vector2, vector3, vector4, color, this.lineTexture, width);
				if (num6 < num2)
				{
					closestEdge = num3;
					num2 = num6;
				}
				num3 = ShapeEditor.NextIndex(num3, num);
				num4 = ShapeEditor.NextIndex(num4, num);
			}
			if (this.inEditMode)
			{
				this.HandlePointInsertToEdge(closestEdge, num2);
				this.HandleEdgeDragging(closestEdge, num2);
			}
			if (GUIUtility.hotControl != this.k_CreatorID && this.m_NewPointIndex != -1)
			{
				this.m_NewPointDragFinished = true;
				GUIUtility.keyboardControl = 0;
				this.m_NewPointIndex = -1;
			}
			if (GUIUtility.hotControl != this.k_EdgeID && this.m_ActiveEdge != -1)
			{
				this.m_ActiveEdge = -1;
			}
		}

		public void Tangents()
		{
			if (this.activePoint >= 0 && this.m_Selection.Count <= 1 && this.GetTangentMode(this.activePoint) != ShapeEditor.TangentMode.Linear)
			{
				Event current = Event.current;
				Vector3 vector = this.GetPointPosition(this.activePoint);
				Vector3 vector2 = this.GetPointLTangent(this.activePoint);
				Vector3 vector3 = this.GetPointRTangent(this.activePoint);
				bool flag = GUIUtility.hotControl == this.k_RightTangentID || GUIUtility.hotControl == this.k_LeftTangentID;
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
						this.RefreshTangents(this.activePoint, GUIUtility.hotControl == this.k_RightTangentID);
						this.Repaint();
					}
				}
			}
		}

		public void Points()
		{
			bool flag = (Event.current.type == EventType.ExecuteCommand || Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "SoftDelete" || Event.current.commandName == "Delete");
			for (int i = 0; i < this.GetPointsCount(); i++)
			{
				if (i != this.m_NewPointIndex)
				{
					Vector3 vector = this.GetPointPosition(i);
					int controlID = GUIUtility.GetControlID(5353, FocusType.Keyboard);
					bool flag2 = this.m_Selection.Contains(i);
					bool flag3 = Event.current.GetTypeForControl(controlID) == EventType.MouseDown;
					bool flag4 = Event.current.GetTypeForControl(controlID) == EventType.MouseUp;
					EditorGUI.BeginChangeCheck();
					ShapeEditor.handleOutlineColor = this.GetOutlineColorForPoint(i, controlID);
					ShapeEditor.handleFillColor = this.GetFillColorForPoint(i, controlID);
					Vector3 vector2 = vector;
					int hotControl = GUIUtility.hotControl;
					if (!Event.current.alt || GUIUtility.hotControl == controlID)
					{
						vector2 = ShapeEditor.DoSlider(controlID, vector, Vector3.up, Vector3.right, this.GetHandleSizeForPoint(i), this.GetCapForPoint(i));
					}
					else if (Event.current.type == EventType.Repaint)
					{
						this.GetCapForPoint(i)(controlID, vector, Quaternion.LookRotation(Vector3.forward, Vector3.up), this.GetHandleSizeForPoint(i), Event.current.type);
					}
					int hotControl2 = GUIUtility.hotControl;
					if (flag4 && hotControl == controlID && hotControl2 == 0 && Event.current.mousePosition == this.m_MousePositionLastMouseDown && !Event.current.shift)
					{
						this.HandlePointClick(i);
					}
					if (EditorGUI.EndChangeCheck())
					{
						this.RecordUndo();
						vector2 = this.Snap(vector2);
						this.m_Selection.MoveSelection(vector2 - vector);
					}
					if (GUIUtility.hotControl == controlID && !flag2 && flag3)
					{
						this.m_Selection.SelectPoint(i, (!Event.current.shift) ? ShapeEditor.SelectionType.Normal : ShapeEditor.SelectionType.Additive);
						this.Repaint();
					}
					if (this.m_NewPointDragFinished && this.activePoint == i && controlID != -1)
					{
						GUIUtility.keyboardControl = controlID;
						this.m_NewPointDragFinished = false;
					}
				}
			}
			if (flag)
			{
				if (Event.current.type == EventType.ValidateCommand)
				{
					Event.current.Use();
				}
				else if (Event.current.type == EventType.ExecuteCommand)
				{
					this.RecordUndo();
					this.m_Selection.DeleteSelection();
					Event.current.Use();
				}
			}
		}

		private void Selection()
		{
			this.m_Selection.OnGUI();
		}

		public void HandlePointInsertToEdge(int closestEdge, float closestEdgeDist)
		{
			bool flag = GUIUtility.hotControl == this.k_CreatorID;
			bool flag2 = this.MouseDistanceToPoint(this.FindClosestPointToMouse()) > 20f;
			bool flag3 = this.MouseDistanceToClosestTangent() > 20f;
			bool flag4 = flag2 && flag3;
			bool flag5 = closestEdgeDist < 9f;
			if ((flag5 && flag4 && !this.m_Selection.isSelecting && Event.current.modifiers == EventModifiers.None) || flag)
			{
				Vector3 vector = (!flag) ? this.FindClosestPointOnEdge(closestEdge, this.ScreenToLocal(Event.current.mousePosition), 100) : this.GetPointPosition(this.m_NewPointIndex);
				EditorGUI.BeginChangeCheck();
				ShapeEditor.handleFillColor = ShapeEditor.k_SelectedHoveredFill;
				ShapeEditor.handleOutlineColor = ShapeEditor.k_SelectedHoveredOutline;
				if (!flag)
				{
					ShapeEditor.handleFillColor = ShapeEditor.handleFillColor.AlphaMultiplied(0.5f);
					ShapeEditor.handleOutlineColor = ShapeEditor.handleOutlineColor.AlphaMultiplied(0.5f);
				}
				int hotControl = GUIUtility.hotControl;
				int arg_14D_0 = this.k_CreatorID;
				Vector3 arg_14D_1 = vector;
				Vector3 arg_14D_2 = Vector3.up;
				Vector3 arg_14D_3 = Vector3.right;
				float arg_14D_4 = this.GetHandleSizeForPoint(closestEdge);
				if (ShapeEditor.<>f__mg$cache3 == null)
				{
					ShapeEditor.<>f__mg$cache3 = new Handles.CapFunction(ShapeEditor.RectCap);
				}
				Vector3 vector2 = ShapeEditor.DoSlider(arg_14D_0, arg_14D_1, arg_14D_2, arg_14D_3, arg_14D_4, ShapeEditor.<>f__mg$cache3);
				if (hotControl != this.k_CreatorID && GUIUtility.hotControl == this.k_CreatorID)
				{
					this.RecordUndo();
					this.m_NewPointIndex = ShapeEditor.NextIndex(closestEdge, this.GetPointsCount());
					this.InsertPointAt(this.m_NewPointIndex, vector2);
					this.m_Selection.SelectPoint(this.m_NewPointIndex, ShapeEditor.SelectionType.Normal);
				}
				else if (EditorGUI.EndChangeCheck())
				{
					this.RecordUndo();
					vector2 = this.Snap(vector2);
					this.m_Selection.MoveSelection(vector2 - vector);
				}
			}
		}

		private void HandleEdgeDragging(int closestEdge, float closestEdgeDist)
		{
			bool flag = GUIUtility.hotControl == this.k_EdgeID;
			bool flag2 = this.MouseDistanceToPoint(this.FindClosestPointToMouse()) > 20f;
			bool flag3 = this.MouseDistanceToClosestTangent() > 20f;
			bool flag4 = flag2 && flag3;
			bool flag5 = closestEdgeDist < 9f;
			if ((flag5 && flag4 && !this.m_Selection.isSelecting && ShapeEditor.EdgeDragModifiersActive()) || flag)
			{
				EventType type = Event.current.type;
				if (type != EventType.MouseDown)
				{
					if (type != EventType.MouseDrag)
					{
						if (type == EventType.MouseUp)
						{
							this.m_ActiveEdge = -1;
							GUIUtility.hotControl = 0;
							Event.current.Use();
						}
					}
					else
					{
						this.RecordUndo();
						Vector3 a = this.ScreenToLocal(Event.current.mousePosition);
						Vector3 b = a - this.m_EdgeDragStartMousePosition;
						Vector3 b2 = this.GetPointPosition(this.m_ActiveEdge);
						Vector3 vector = this.m_EdgeDragStartP0 + b;
						vector = this.Snap(vector);
						Vector3 b3 = vector - b2;
						int activeEdge = this.m_ActiveEdge;
						int num = ShapeEditor.NextIndex(this.m_ActiveEdge, this.GetPointsCount());
						this.SetPointPosition(this.m_ActiveEdge, this.GetPointPosition(activeEdge) + b3);
						this.SetPointPosition(num, this.GetPointPosition(num) + b3);
						Event.current.Use();
					}
				}
				else
				{
					this.m_ActiveEdge = closestEdge;
					this.m_EdgeDragStartP0 = this.GetPointPosition(this.m_ActiveEdge);
					this.m_EdgeDragStartP1 = this.GetPointPosition(ShapeEditor.NextIndex(this.m_ActiveEdge, this.GetPointsCount()));
					if (Event.current.shift)
					{
						this.RecordUndo();
						this.InsertPointAt(this.m_ActiveEdge + 1, this.m_EdgeDragStartP0);
						this.InsertPointAt(this.m_ActiveEdge + 2, this.m_EdgeDragStartP1);
						this.m_ActiveEdge++;
					}
					this.m_EdgeDragStartMousePosition = this.ScreenToLocal(Event.current.mousePosition);
					GUIUtility.hotControl = this.k_EdgeID;
					Event.current.Use();
				}
			}
		}

		private Vector3 DoTangent(Vector3 p0, Vector3 t0, int cid, int pointIndex, Color color)
		{
			float handleSizeForPoint = this.GetHandleSizeForPoint(pointIndex);
			float tangentSizeForPoint = this.GetTangentSizeForPoint(pointIndex);
			Handles.color = color;
			float num = HandleUtility.DistanceToCircle(t0, tangentSizeForPoint);
			if (this.lineTexture != null)
			{
				Handles.DrawAAPolyLine(this.lineTexture, new Vector3[]
				{
					p0,
					t0
				});
			}
			else
			{
				Handles.DrawLine(p0, t0);
			}
			ShapeEditor.handleOutlineColor = ((num <= 0f) ? ShapeEditor.k_SelectedHoveredOutline : color);
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
			else if (!Event.current.control && !Event.current.shift && this.m_ActivePointOnLastMouseDown == this.activePoint)
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

		private Color GetOutlineColorForPoint(int pointIndex, int handleID)
		{
			bool flag = this.MouseDistanceToPoint(pointIndex) <= 0f;
			bool flag2 = this.m_Selection.Contains(pointIndex);
			Color result;
			if ((flag && flag2) || GUIUtility.hotControl == handleID)
			{
				result = ShapeEditor.k_SelectedHoveredOutline;
			}
			else if (flag)
			{
				result = ShapeEditor.k_UnSelectedHoveredOutline;
			}
			else if (flag2)
			{
				result = ShapeEditor.k_SelectedOutline;
			}
			else
			{
				result = ShapeEditor.k_UnSelectedOutline;
			}
			return result;
		}

		private Color GetFillColorForPoint(int pointIndex, int handleID)
		{
			bool flag = this.MouseDistanceToPoint(pointIndex) <= 0f;
			bool flag2 = this.m_Selection.Contains(pointIndex);
			Color result;
			if ((flag && flag2) || GUIUtility.hotControl == handleID)
			{
				result = ShapeEditor.k_SelectedHoveredFill;
			}
			else if (flag)
			{
				result = ShapeEditor.k_UnSelectedHoveredFill;
			}
			else if (flag2)
			{
				result = ShapeEditor.k_SelectedFill;
			}
			else
			{
				result = ShapeEditor.k_UnSelectedFill;
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
				if (ShapeEditor.<>f__mg$cache4 == null)
				{
					ShapeEditor.<>f__mg$cache4 = new Handles.CapFunction(ShapeEditor.CircleCap);
				}
				result = ShapeEditor.<>f__mg$cache4;
			}
			else
			{
				if (ShapeEditor.<>f__mg$cache5 == null)
				{
					ShapeEditor.<>f__mg$cache5 = new Handles.CapFunction(ShapeEditor.DiamondCap);
				}
				result = ShapeEditor.<>f__mg$cache5;
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
				if (ShapeEditor.<>f__mg$cache6 == null)
				{
					ShapeEditor.<>f__mg$cache6 = new ShapeEditor.DistanceToControl(HandleUtility.DistanceToDiamond);
				}
				result = ShapeEditor.<>f__mg$cache6;
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
						if (ShapeEditor.<>f__mg$cacheA == null)
						{
							ShapeEditor.<>f__mg$cacheA = new Handles.CapFunction(ShapeEditor.DiamondCap);
						}
						result = ShapeEditor.<>f__mg$cacheA;
					}
					else
					{
						if (ShapeEditor.<>f__mg$cache9 == null)
						{
							ShapeEditor.<>f__mg$cache9 = new Handles.CapFunction(ShapeEditor.RectCap);
						}
						result = ShapeEditor.<>f__mg$cache9;
					}
				}
				else
				{
					if (ShapeEditor.<>f__mg$cache8 == null)
					{
						ShapeEditor.<>f__mg$cache8 = new Handles.CapFunction(ShapeEditor.CircleCap);
					}
					result = ShapeEditor.<>f__mg$cache8;
				}
			}
			else
			{
				if (ShapeEditor.<>f__mg$cache7 == null)
				{
					ShapeEditor.<>f__mg$cache7 = new Handles.CapFunction(ShapeEditor.DiamondCap);
				}
				result = ShapeEditor.<>f__mg$cache7;
			}
			return result;
		}

		private static float DistanceToCircleInternal(Vector3 position, Quaternion rotation, float size)
		{
			return HandleUtility.DistanceToCircle(position, size);
		}

		private float GetHandleSizeForPoint(int index)
		{
			return (!(Camera.current != null)) ? (this.GetHandleSize() / Handles.matrix.m00) : (HandleUtility.GetHandleSize(this.GetPointPosition(index)) * 0.075f);
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
			this.m_MousePositionLastMouseDown = Event.current.mousePosition;
			this.m_ActivePointOnLastMouseDown = this.activePoint;
		}

		private void DelayedResetIfNecessary()
		{
			if (this.m_DelayedReset)
			{
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
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
				float magnitude = (position - positionByIndex).magnitude;
				if (magnitude < num2)
				{
					num2 = magnitude;
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
			Vector3 position = this.ScreenToLocal(Event.current.mousePosition);
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
				float magnitude = (a - position).magnitude;
				if (magnitude < num)
				{
					result = i;
					num = magnitude;
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

		private static bool EdgeDragModifiersActive()
		{
			return Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command;
		}

		private static Vector3 DoSlider(int id, Vector3 position, Vector3 slide1, Vector3 slide2, float s, Handles.CapFunction cap)
		{
			return Slider2D.Do(id, position, Vector3.zero, Vector3.Cross(slide1, slide2), slide1, slide2, s, cap, Vector2.zero, false);
		}

		public static void RectCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType == EventType.Layout)
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
			}
			else if (eventType == EventType.Repaint)
			{
				Vector3 planeNormal = Handles.matrix.GetColumn(2);
				Vector3 normalized = (ShapeEditor.ProjectPointOnPlane(planeNormal, position, position + Vector3.up) - position).normalized;
				Quaternion rotation2 = Quaternion.LookRotation(Handles.matrix.GetColumn(2), normalized);
				Vector3 b = rotation2 * Vector3.right * size;
				Vector3 b2 = rotation2 * Vector3.up * size;
				HandleUtility.ApplyWireMaterial();
				GL.PushMatrix();
				GL.MultMatrix(Handles.matrix);
				GL.Begin(7);
				GL.Color(ShapeEditor.handleFillColor);
				GL.Vertex(position + b + b2);
				GL.Vertex(position + b - b2);
				GL.Vertex(position - b - b2);
				GL.Vertex(position - b + b2);
				GL.End();
				GL.Begin(1);
				GL.Color(ShapeEditor.handleOutlineColor);
				GL.Vertex(position + b + b2);
				GL.Vertex(position + b - b2);
				GL.Vertex(position + b - b2);
				GL.Vertex(position - b - b2);
				GL.Vertex(position - b - b2);
				GL.Vertex(position - b + b2);
				GL.Vertex(position - b + b2);
				GL.Vertex(position + b + b2);
				GL.End();
				GL.PopMatrix();
			}
		}

		public static void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType == EventType.Layout)
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
			}
			else if (eventType == EventType.Repaint)
			{
				Handles.StartCapDraw(position, rotation, size);
				Vector3 vector = ShapeEditor.handleMatrixrotation * rotation * Vector3.forward;
				Vector3 from = Vector3.Cross(vector, Vector3.up);
				if (from.sqrMagnitude < 0.001f)
				{
					from = Vector3.Cross(vector, Vector3.right);
				}
				Vector3[] array = new Vector3[60];
				Handles.SetDiscSectionPoints(array, 60, position, vector, from, 360f, size);
				HandleUtility.ApplyWireMaterial();
				GL.PushMatrix();
				GL.Begin(4);
				GL.MultMatrix(Handles.matrix);
				GL.Color(ShapeEditor.handleFillColor);
				for (int i = 1; i < array.Length; i++)
				{
					GL.Vertex(position);
					GL.Vertex(array[i - 1]);
					GL.Vertex(array[i]);
				}
				GL.End();
				GL.Begin(1);
				GL.Color(ShapeEditor.handleOutlineColor);
				for (int j = 0; j < array.Length - 1; j++)
				{
					GL.Vertex(array[j]);
					GL.Vertex(array[j + 1]);
				}
				GL.End();
				GL.PopMatrix();
			}
		}

		public static void DiamondCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (eventType == EventType.Layout)
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
			}
			else if (eventType == EventType.Repaint)
			{
				Vector3 planeNormal = Handles.matrix.GetColumn(2);
				Vector3 normalized = (ShapeEditor.ProjectPointOnPlane(planeNormal, position, position + Vector3.up) - position).normalized;
				Quaternion rotation2 = Quaternion.LookRotation(Handles.matrix.GetColumn(2), normalized);
				Vector3 b = rotation2 * Vector3.right * size * 1.25f;
				Vector3 b2 = rotation2 * Vector3.up * size * 1.25f;
				HandleUtility.ApplyWireMaterial();
				GL.PushMatrix();
				GL.Begin(7);
				GL.MultMatrix(Handles.matrix);
				GL.Color(ShapeEditor.handleFillColor);
				GL.Vertex(position + b);
				GL.Vertex(position - b2);
				GL.Vertex(position - b);
				GL.Vertex(position + b2);
				GL.End();
				GL.Begin(1);
				GL.Color(ShapeEditor.handleOutlineColor);
				GL.Vertex(position + b);
				GL.Vertex(position - b2);
				GL.Vertex(position - b2);
				GL.Vertex(position - b);
				GL.Vertex(position - b);
				GL.Vertex(position + b2);
				GL.Vertex(position + b2);
				GL.Vertex(position + b);
				GL.End();
				GL.PopMatrix();
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
	}
}
