using System;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects]
	internal abstract class Collider2DEditorBase : ColliderEditorBase
	{
		protected class Styles
		{
			public static readonly GUIContent s_ColliderEditDisableHelp = EditorGUIUtility.TextContent("Collider cannot be edited because it is driven by SpriteRenderer's tiling properties.");

			public static readonly GUIContent s_AutoTilingLabel = EditorGUIUtility.TextContent("Auto Tiling | When enabled, the collider's shape will update automaticaly based on the SpriteRenderer's tiling properties");
		}

		private SerializedProperty m_Density;

		private readonly AnimBool m_ShowDensity = new AnimBool();

		private readonly AnimBool m_ShowInfo = new AnimBool();

		private readonly AnimBool m_ShowContacts = new AnimBool();

		private Vector2 m_ContactScrollPosition;

		private static ContactPoint2D[] m_Contacts = new ContactPoint2D[100];

		private SerializedProperty m_Material;

		private SerializedProperty m_IsTrigger;

		private SerializedProperty m_UsedByEffector;

		private SerializedProperty m_UsedByComposite;

		private SerializedProperty m_Offset;

		protected SerializedProperty m_AutoTiling;

		private readonly AnimBool m_ShowCompositeRedundants = new AnimBool();

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Density = base.serializedObject.FindProperty("m_Density");
			this.m_ShowDensity.value = this.ShouldShowDensity();
			this.m_ShowDensity.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowInfo.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowContacts.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ContactScrollPosition = Vector2.zero;
			this.m_Material = base.serializedObject.FindProperty("m_Material");
			this.m_IsTrigger = base.serializedObject.FindProperty("m_IsTrigger");
			this.m_UsedByEffector = base.serializedObject.FindProperty("m_UsedByEffector");
			this.m_UsedByComposite = base.serializedObject.FindProperty("m_UsedByComposite");
			this.m_Offset = base.serializedObject.FindProperty("m_Offset");
			this.m_AutoTiling = base.serializedObject.FindProperty("m_AutoTiling");
			this.m_ShowCompositeRedundants.value = !this.m_UsedByComposite.boolValue;
			this.m_ShowCompositeRedundants.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public override void OnDisable()
		{
			this.m_ShowDensity.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowInfo.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowContacts.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowCompositeRedundants.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			base.OnDisable();
		}

		public override void OnInspectorGUI()
		{
			this.m_ShowCompositeRedundants.target = !this.m_UsedByComposite.boolValue;
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowCompositeRedundants.faded))
			{
				this.m_ShowDensity.target = this.ShouldShowDensity();
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowDensity.faded))
				{
					EditorGUILayout.PropertyField(this.m_Density, new GUILayoutOption[0]);
				}
				Collider2DEditorBase.FixedEndFadeGroup(this.m_ShowDensity.faded);
				EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_UsedByEffector, new GUILayoutOption[0]);
			}
			Collider2DEditorBase.FixedEndFadeGroup(this.m_ShowCompositeRedundants.faded);
			if ((from x in base.targets
			where !(x as Collider2D).compositeCapable
			select x).Count<UnityEngine.Object>() == 0)
			{
				EditorGUILayout.PropertyField(this.m_UsedByComposite, new GUILayoutOption[0]);
			}
			if (this.m_AutoTiling != null)
			{
				EditorGUILayout.PropertyField(this.m_AutoTiling, Collider2DEditorBase.Styles.s_AutoTilingLabel, new GUILayoutOption[0]);
			}
			EditorGUILayout.PropertyField(this.m_Offset, new GUILayoutOption[0]);
		}

		public void FinalizeInspectorGUI()
		{
			this.ShowColliderInfoProperties();
			this.CheckColliderErrorState();
			if (base.targets.Length == 1)
			{
				Collider2D collider2D = base.target as Collider2D;
				if (collider2D.isActiveAndEnabled && collider2D.composite == null && this.m_UsedByComposite.boolValue)
				{
					EditorGUILayout.HelpBox("This collider will not function with a composite until there is a CompositeCollider2D on the GameObject that the attached Rigidbody2D is on.", MessageType.Warning);
				}
			}
			Effector2DEditor.CheckEffectorWarnings(base.target as Collider2D);
		}

		private void ShowColliderInfoProperties()
		{
			this.m_ShowInfo.target = EditorGUILayout.Foldout(this.m_ShowInfo.target, "Info", true);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowInfo.faded))
			{
				if (base.targets.Length == 1)
				{
					Collider2D collider2D = base.targets[0] as Collider2D;
					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.ObjectField("Attached Body", collider2D.attachedRigidbody, typeof(Rigidbody2D), false, new GUILayoutOption[0]);
					EditorGUILayout.FloatField("Friction", collider2D.friction, new GUILayoutOption[0]);
					EditorGUILayout.FloatField("Bounciness", collider2D.bounciness, new GUILayoutOption[0]);
					EditorGUILayout.FloatField("Shape Count", (float)collider2D.shapeCount, new GUILayoutOption[0]);
					if (collider2D.isActiveAndEnabled)
					{
						EditorGUILayout.BoundsField("Bounds", collider2D.bounds, new GUILayoutOption[0]);
					}
					EditorGUI.EndDisabledGroup();
					this.ShowContacts(collider2D);
					base.Repaint();
				}
				else
				{
					EditorGUILayout.HelpBox("Cannot show Info properties when multiple colliders are selected.", MessageType.Info);
				}
			}
			EditorGUILayout.EndFadeGroup();
		}

		private bool ShouldShowDensity()
		{
			bool result;
			if ((from x in base.targets
			select (x as Collider2D).attachedRigidbody).Distinct<Rigidbody2D>().Count<Rigidbody2D>() > 1)
			{
				result = false;
			}
			else
			{
				Rigidbody2D attachedRigidbody = (base.target as Collider2D).attachedRigidbody;
				result = (attachedRigidbody && attachedRigidbody.useAutoMass && attachedRigidbody.bodyType == RigidbodyType2D.Dynamic);
			}
			return result;
		}

		private void ShowContacts(Collider2D collider)
		{
			EditorGUI.indentLevel++;
			this.m_ShowContacts.target = EditorGUILayout.Foldout(this.m_ShowContacts.target, "Contacts");
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowContacts.faded))
			{
				int contacts = collider.GetContacts(Collider2DEditorBase.m_Contacts);
				if (contacts > 0)
				{
					this.m_ContactScrollPosition = EditorGUILayout.BeginScrollView(this.m_ContactScrollPosition, new GUILayoutOption[]
					{
						GUILayout.Height(180f)
					});
					EditorGUI.BeginDisabledGroup(true);
					for (int i = 0; i < contacts; i++)
					{
						ContactPoint2D contactPoint2D = Collider2DEditorBase.m_Contacts[i];
						EditorGUILayout.HelpBox(string.Format("Contact#{0}", i), MessageType.None);
						EditorGUI.indentLevel++;
						EditorGUILayout.Vector2Field("Point", contactPoint2D.point, new GUILayoutOption[0]);
						EditorGUILayout.Vector2Field("Normal", contactPoint2D.normal, new GUILayoutOption[0]);
						EditorGUILayout.Vector2Field("Relative Velocity", contactPoint2D.relativeVelocity, new GUILayoutOption[0]);
						EditorGUILayout.FloatField("Normal Impulse", contactPoint2D.normalImpulse, new GUILayoutOption[0]);
						EditorGUILayout.FloatField("Tangent Impulse", contactPoint2D.tangentImpulse, new GUILayoutOption[0]);
						EditorGUILayout.ObjectField("Collider", contactPoint2D.collider, typeof(Collider2D), false, new GUILayoutOption[0]);
						EditorGUILayout.ObjectField("Rigidbody", contactPoint2D.rigidbody, typeof(Rigidbody2D), false, new GUILayoutOption[0]);
						EditorGUILayout.ObjectField("OtherRigidbody", contactPoint2D.otherRigidbody, typeof(Rigidbody2D), false, new GUILayoutOption[0]);
						EditorGUI.indentLevel--;
						EditorGUILayout.Space();
					}
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndScrollView();
				}
				else
				{
					EditorGUILayout.HelpBox("No Contacts", MessageType.Info);
				}
			}
			Collider2DEditorBase.FixedEndFadeGroup(this.m_ShowContacts.faded);
			EditorGUI.indentLevel--;
		}

		private static void FixedEndFadeGroup(float value)
		{
			if (value != 0f && value != 1f)
			{
				EditorGUILayout.EndFadeGroup();
			}
		}

		internal override void OnForceReloadInspector()
		{
			base.OnForceReloadInspector();
			if (base.editingCollider)
			{
				EditMode.QuitEditMode();
			}
		}

		protected void CheckColliderErrorState()
		{
			ColliderErrorState2D errorState = (base.target as Collider2D).errorState;
			if (errorState != ColliderErrorState2D.NoShapes)
			{
				if (errorState == ColliderErrorState2D.RemovedShapes)
				{
					EditorGUILayout.HelpBox("The collider created collision shape(s) but some were removed as they failed verification.  This could be because they were deemed too small or the vertices were too close.  Vertices can also become close under certain rotations or very small scaling.", MessageType.Warning);
				}
			}
			else
			{
				EditorGUILayout.HelpBox("The collider did not create any collision shapes as they all failed verification.  This could be because they were deemed too small or the vertices were too close.  Vertices can also become close under certain rotations or very small scaling.", MessageType.Warning);
			}
		}

		protected void BeginColliderInspector()
		{
			base.serializedObject.Update();
			using (new EditorGUI.DisabledScope(base.targets.Length > 1))
			{
				base.InspectorEditButtonGUI();
			}
		}

		protected void EndColliderInspector()
		{
			base.serializedObject.ApplyModifiedProperties();
		}

		protected bool CanEditCollider()
		{
			UnityEngine.Object exists = base.targets.FirstOrDefault(delegate(UnityEngine.Object x)
			{
				SpriteRenderer component = (x as Component).GetComponent<SpriteRenderer>();
				return component != null && component.drawMode != SpriteDrawMode.Simple && this.m_AutoTiling.boolValue;
			});
			return !exists;
		}
	}
}
