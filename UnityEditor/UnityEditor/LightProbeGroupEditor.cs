using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace UnityEditor
{
	internal class LightProbeGroupEditor : IEditablePoint
	{
		private bool m_Editing;

		private List<Vector3> m_SourcePositions;

		private List<int> m_Selection = new List<int>();

		private LightProbeGroupSelection m_SerializedSelectedProbes;

		private readonly LightProbeGroup m_Group;

		private bool m_ShouldRecalculateTetrahedra;

		private Vector3 m_LastPosition = Vector3.zero;

		private Quaternion m_LastRotation = Quaternion.identity;

		private Vector3 m_LastScale = Vector3.one;

		private LightProbeGroupInspector m_Inspector;

		private static readonly Color kCloudColor = new Color(0.784313738f, 0.784313738f, 0.0784313753f, 0.85f);

		private static readonly Color kSelectedCloudColor = new Color(0.3f, 0.6f, 1f, 1f);

		public bool drawTetrahedra
		{
			get;
			set;
		}

		public Bounds selectedProbeBounds
		{
			get
			{
				List<Vector3> list = new List<Vector3>();
				foreach (int current in this.m_Selection)
				{
					list.Add(this.m_SourcePositions[current]);
				}
				return this.GetBounds(list);
			}
		}

		public Bounds bounds
		{
			get
			{
				return this.GetBounds(this.m_SourcePositions);
			}
		}

		public int Count
		{
			get
			{
				return this.m_SourcePositions.Count;
			}
		}

		public int SelectedCount
		{
			get
			{
				return this.m_Selection.Count;
			}
		}

		public LightProbeGroupEditor(LightProbeGroup group, LightProbeGroupInspector inspector)
		{
			this.m_Group = group;
			this.MarkTetrahedraDirty();
			this.m_SerializedSelectedProbes = ScriptableObject.CreateInstance<LightProbeGroupSelection>();
			this.m_SerializedSelectedProbes.hideFlags = HideFlags.HideAndDontSave;
			this.m_Inspector = inspector;
			this.drawTetrahedra = true;
		}

		public void SetEditing(bool editing)
		{
			this.m_Editing = editing;
		}

		public void AddProbe(Vector3 position)
		{
			Undo.RegisterCompleteObjectUndo(new UnityEngine.Object[]
			{
				this.m_Group,
				this.m_SerializedSelectedProbes
			}, "Add Probe");
			this.m_SourcePositions.Add(position);
			this.SelectProbe(this.m_SourcePositions.Count - 1);
			this.MarkTetrahedraDirty();
		}

		private void SelectProbe(int i)
		{
			if (!this.m_Selection.Contains(i))
			{
				this.m_Selection.Add(i);
			}
		}

		public void SelectAllProbes()
		{
			this.DeselectProbes();
			int count = this.m_SourcePositions.Count;
			for (int i = 0; i < count; i++)
			{
				this.m_Selection.Add(i);
			}
		}

		public void DeselectProbes()
		{
			this.m_Selection.Clear();
			this.m_SerializedSelectedProbes.m_Selection = this.m_Selection;
		}

		private IEnumerable<Vector3> SelectedProbePositions()
		{
			return (from t in this.m_Selection
			select this.m_SourcePositions[t]).ToList<Vector3>();
		}

		public void DuplicateSelectedProbes()
		{
			if (this.m_Selection.Count != 0)
			{
				Undo.RegisterCompleteObjectUndo(new UnityEngine.Object[]
				{
					this.m_Group,
					this.m_SerializedSelectedProbes
				}, "Duplicate Probes");
				foreach (Vector3 current in this.SelectedProbePositions())
				{
					this.m_SourcePositions.Add(current);
				}
				this.MarkTetrahedraDirty();
			}
		}

		private void CopySelectedProbes()
		{
			IEnumerable<Vector3> source = this.SelectedProbePositions();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Vector3[]));
			StringWriter stringWriter = new StringWriter();
			xmlSerializer.Serialize(stringWriter, (from pos in source
			select this.m_Group.transform.TransformPoint(pos)).ToArray<Vector3>());
			stringWriter.Close();
			GUIUtility.systemCopyBuffer = stringWriter.ToString();
		}

		private static bool CanPasteProbes()
		{
			bool result;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Vector3[]));
				StringReader stringReader = new StringReader(GUIUtility.systemCopyBuffer);
				xmlSerializer.Deserialize(stringReader);
				stringReader.Close();
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private bool PasteProbes()
		{
			bool result;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Vector3[]));
				StringReader stringReader = new StringReader(GUIUtility.systemCopyBuffer);
				Vector3[] array = (Vector3[])xmlSerializer.Deserialize(stringReader);
				stringReader.Close();
				if (array.Length == 0)
				{
					result = false;
				}
				else
				{
					Undo.RegisterCompleteObjectUndo(new UnityEngine.Object[]
					{
						this.m_Group,
						this.m_SerializedSelectedProbes
					}, "Paste Probes");
					int count = this.m_SourcePositions.Count;
					Vector3[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						Vector3 position = array2[i];
						this.m_SourcePositions.Add(this.m_Group.transform.InverseTransformPoint(position));
					}
					this.DeselectProbes();
					for (int j = count; j < count + array.Length; j++)
					{
						this.SelectProbe(j);
					}
					this.MarkTetrahedraDirty();
					result = true;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public void RemoveSelectedProbes()
		{
			if (this.m_Selection.Count != 0)
			{
				Undo.RegisterCompleteObjectUndo(new UnityEngine.Object[]
				{
					this.m_Group,
					this.m_SerializedSelectedProbes
				}, "Delete Probes");
				IOrderedEnumerable<int> orderedEnumerable = from x in this.m_Selection
				orderby x descending
				select x;
				foreach (int current in orderedEnumerable)
				{
					this.m_SourcePositions.RemoveAt(current);
				}
				this.DeselectProbes();
				this.MarkTetrahedraDirty();
			}
		}

		public void PullProbePositions()
		{
			if (this.m_Group != null && this.m_SerializedSelectedProbes != null)
			{
				this.m_SourcePositions = new List<Vector3>(this.m_Group.probePositions);
				this.m_Selection = new List<int>(this.m_SerializedSelectedProbes.m_Selection);
			}
		}

		public void PushProbePositions()
		{
			this.m_Group.probePositions = this.m_SourcePositions.ToArray();
			this.m_SerializedSelectedProbes.m_Selection = this.m_Selection;
		}

		private void DrawTetrahedra()
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (SceneView.lastActiveSceneView)
				{
					LightProbeVisualization.DrawTetrahedra(this.m_ShouldRecalculateTetrahedra, SceneView.lastActiveSceneView.camera.transform.position);
					this.m_ShouldRecalculateTetrahedra = false;
				}
			}
		}

		public void HandleEditMenuHotKeyCommands()
		{
			if (Event.current.type == EventType.ValidateCommand || Event.current.type == EventType.ExecuteCommand)
			{
				bool flag = Event.current.type == EventType.ExecuteCommand;
				string commandName = Event.current.commandName;
				if (commandName != null)
				{
					if (!(commandName == "SoftDelete") && !(commandName == "Delete"))
					{
						if (!(commandName == "Duplicate"))
						{
							if (!(commandName == "SelectAll"))
							{
								if (!(commandName == "Cut"))
								{
									if (commandName == "Copy")
									{
										if (flag)
										{
											this.CopySelectedProbes();
										}
										Event.current.Use();
									}
								}
								else
								{
									if (flag)
									{
										this.CopySelectedProbes();
										this.RemoveSelectedProbes();
									}
									Event.current.Use();
								}
							}
							else
							{
								if (flag)
								{
									this.SelectAllProbes();
								}
								Event.current.Use();
							}
						}
						else
						{
							if (flag)
							{
								this.DuplicateSelectedProbes();
							}
							Event.current.Use();
						}
					}
					else
					{
						if (flag)
						{
							this.RemoveSelectedProbes();
						}
						Event.current.Use();
					}
				}
			}
		}

		public static void TetrahedralizeSceneProbes(out Vector3[] positions, out int[] indices)
		{
			LightProbeGroup[] array = UnityEngine.Object.FindObjectsOfType(typeof(LightProbeGroup)) as LightProbeGroup[];
			if (array == null)
			{
				positions = new Vector3[0];
				indices = new int[0];
			}
			else
			{
				List<Vector3> list = new List<Vector3>();
				LightProbeGroup[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					LightProbeGroup lightProbeGroup = array2[i];
					Vector3[] probePositions = lightProbeGroup.probePositions;
					Vector3[] array3 = probePositions;
					for (int j = 0; j < array3.Length; j++)
					{
						Vector3 position = array3[j];
						Vector3 item = lightProbeGroup.transform.TransformPoint(position);
						list.Add(item);
					}
				}
				if (list.Count == 0)
				{
					positions = new Vector3[0];
					indices = new int[0];
				}
				else
				{
					Lightmapping.Tetrahedralize(list.ToArray(), out indices, out positions);
				}
			}
		}

		public bool OnSceneGUI(Transform transform)
		{
			bool editing;
			if (!this.m_Group.enabled)
			{
				editing = this.m_Editing;
			}
			else
			{
				if (Event.current.type == EventType.Layout)
				{
					if (this.m_LastPosition != this.m_Group.transform.position || this.m_LastRotation != this.m_Group.transform.rotation || this.m_LastScale != this.m_Group.transform.localScale)
					{
						this.MarkTetrahedraDirty();
					}
					this.m_LastPosition = this.m_Group.transform.position;
					this.m_LastRotation = this.m_Group.transform.rotation;
					this.m_LastScale = this.m_Group.transform.localScale;
				}
				bool firstSelect = false;
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
				{
					if (this.SelectedCount == 0)
					{
						int num = PointEditor.FindNearest(Event.current.mousePosition, transform, this);
						bool flag = num != -1;
						if (flag && !this.m_Editing)
						{
							this.m_Inspector.StartEditMode();
							this.m_Editing = true;
							firstSelect = true;
						}
					}
				}
				bool flag2 = Event.current.type == EventType.MouseUp;
				if (this.m_Editing)
				{
					if (PointEditor.SelectPoints(this, transform, ref this.m_Selection, firstSelect))
					{
						Undo.RegisterCompleteObjectUndo(new UnityEngine.Object[]
						{
							this.m_Group,
							this.m_SerializedSelectedProbes
						}, "Select Probes");
					}
				}
				if ((Event.current.type == EventType.ValidateCommand || Event.current.type == EventType.ExecuteCommand) && Event.current.commandName == "Paste")
				{
					if (Event.current.type == EventType.ValidateCommand)
					{
						if (LightProbeGroupEditor.CanPasteProbes())
						{
							Event.current.Use();
						}
					}
					if (Event.current.type == EventType.ExecuteCommand)
					{
						if (this.PasteProbes())
						{
							Event.current.Use();
							this.m_Editing = true;
						}
					}
				}
				if (this.drawTetrahedra)
				{
					this.DrawTetrahedra();
				}
				PointEditor.Draw(this, transform, this.m_Selection, true);
				if (!this.m_Editing)
				{
					editing = this.m_Editing;
				}
				else
				{
					this.HandleEditMenuHotKeyCommands();
					if (this.m_Editing && PointEditor.MovePoints(this, transform, this.m_Selection))
					{
						Undo.RegisterCompleteObjectUndo(new UnityEngine.Object[]
						{
							this.m_Group,
							this.m_SerializedSelectedProbes
						}, "Move Probes");
						if (LightProbeVisualization.dynamicUpdateLightProbes)
						{
							this.MarkTetrahedraDirty();
						}
					}
					if (this.m_Editing && flag2 && !LightProbeVisualization.dynamicUpdateLightProbes)
					{
						this.MarkTetrahedraDirty();
					}
					editing = this.m_Editing;
				}
			}
			return editing;
		}

		public void MarkTetrahedraDirty()
		{
			this.m_ShouldRecalculateTetrahedra = true;
		}

		private Bounds GetBounds(List<Vector3> positions)
		{
			Bounds result;
			if (positions.Count == 0)
			{
				result = default(Bounds);
			}
			else if (positions.Count == 1)
			{
				result = new Bounds(this.m_Group.transform.TransformPoint(positions[0]), new Vector3(1f, 1f, 1f));
			}
			else
			{
				result = GeometryUtility.CalculateBounds(positions.ToArray(), this.m_Group.transform.localToWorldMatrix);
			}
			return result;
		}

		public Vector3 GetPosition(int idx)
		{
			return this.m_SourcePositions[idx];
		}

		public Vector3 GetWorldPosition(int idx)
		{
			return this.m_Group.transform.TransformPoint(this.m_SourcePositions[idx]);
		}

		public void SetPosition(int idx, Vector3 position)
		{
			if (!(this.m_SourcePositions[idx] == position))
			{
				this.m_SourcePositions[idx] = position;
			}
		}

		public Color GetDefaultColor()
		{
			return LightProbeGroupEditor.kCloudColor;
		}

		public Color GetSelectedColor()
		{
			return LightProbeGroupEditor.kSelectedCloudColor;
		}

		public float GetPointScale()
		{
			return 10f * AnnotationUtility.iconSize;
		}

		public Vector3[] GetSelectedPositions()
		{
			int selectedCount = this.SelectedCount;
			Vector3[] array = new Vector3[selectedCount];
			for (int i = 0; i < selectedCount; i++)
			{
				array[i] = this.m_SourcePositions[this.m_Selection[i]];
			}
			return array;
		}

		public void UpdateSelectedPosition(int idx, Vector3 position)
		{
			if (idx <= this.SelectedCount - 1)
			{
				this.m_SourcePositions[this.m_Selection[idx]] = position;
			}
		}

		public IEnumerable<Vector3> GetPositions()
		{
			return this.m_SourcePositions;
		}

		public Vector3[] GetUnselectedPositions()
		{
			int count = this.Count;
			int selectedCount = this.SelectedCount;
			Vector3[] result;
			if (selectedCount == count)
			{
				result = new Vector3[0];
			}
			else if (selectedCount == 0)
			{
				result = this.m_SourcePositions.ToArray();
			}
			else
			{
				bool[] array = new bool[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = false;
				}
				for (int j = 0; j < selectedCount; j++)
				{
					array[this.m_Selection[j]] = true;
				}
				Vector3[] array2 = new Vector3[count - selectedCount];
				int num = 0;
				for (int k = 0; k < count; k++)
				{
					if (!array[k])
					{
						array2[num++] = this.m_SourcePositions[k];
					}
				}
				result = array2;
			}
			return result;
		}
	}
}
