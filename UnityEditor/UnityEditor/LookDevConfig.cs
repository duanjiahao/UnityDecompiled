using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	internal class LookDevConfig : ScriptableObject
	{
		[SerializeField]
		private LookDevContext[] m_LookDevContexts = new LookDevContext[2];

		[SerializeField]
		private LookDevPropertyInfo[] m_LookDevProperties = new LookDevPropertyInfo[5];

		[SerializeField]
		private GizmoInfo m_Gizmo = new GizmoInfo();

		[SerializeField]
		private LookDevMode m_LookDevMode = LookDevMode.Single1;

		[SerializeField]
		private bool m_EnableToneMap = true;

		[SerializeField]
		private bool m_EnableShadowCubemap = true;

		[SerializeField]
		private float m_ExposureRange = 8f;

		[SerializeField]
		private float m_ShadowDistance = 0f;

		[SerializeField]
		private bool m_ShowBalls = false;

		[SerializeField]
		private bool m_ShowControlWindows = true;

		[SerializeField]
		private bool m_RotateObjectMode = false;

		[SerializeField]
		private float m_EnvRotationSpeed = 1f;

		[SerializeField]
		private bool m_RotateEnvMode = false;

		[SerializeField]
		private float m_ObjRotationSpeed = 1f;

		[SerializeField]
		private bool m_AllowDifferentObjects = false;

		[SerializeField]
		private GameObject[] m_CurrentObject = new GameObject[2];

		[SerializeField]
		private GameObject[] m_PreviewObjects = new GameObject[2];

		[SerializeField]
		private LookDevEditionContext m_CurrentContextEdition = LookDevEditionContext.Left;

		[SerializeField]
		private int m_CurrentEditionContextIndex = 0;

		[SerializeField]
		private float m_DualViewBlendFactor = 0f;

		[SerializeField]
		private GameObject[] m_OriginalGameObject = new GameObject[2];

		[SerializeField]
		private CameraState[] m_CameraState = new CameraState[2];

		[SerializeField]
		private bool m_SideBySideCameraLinked = false;

		[SerializeField]
		private CameraState m_CameraStateCommon = new CameraState();

		[SerializeField]
		private CameraState m_CameraStateLeft = new CameraState();

		[SerializeField]
		private CameraState m_CameraStateRight = new CameraState();

		private LookDevView m_LookDevView = null;

		public bool enableShadowCubemap
		{
			get
			{
				return this.m_EnableShadowCubemap;
			}
			set
			{
				this.m_EnableShadowCubemap = value;
				this.m_LookDevView.Repaint();
			}
		}

		public bool sideBySideCameraLinked
		{
			get
			{
				return this.m_SideBySideCameraLinked;
			}
			set
			{
				this.m_SideBySideCameraLinked = value;
			}
		}

		public int currentEditionContextIndex
		{
			get
			{
				return this.m_CurrentEditionContextIndex;
			}
		}

		public LookDevEditionContext currentEditionContext
		{
			get
			{
				return this.m_CurrentContextEdition;
			}
		}

		public float dualViewBlendFactor
		{
			get
			{
				return this.m_DualViewBlendFactor;
			}
			set
			{
				this.m_DualViewBlendFactor = value;
			}
		}

		public GizmoInfo gizmo
		{
			get
			{
				return this.m_Gizmo;
			}
			set
			{
				this.m_Gizmo = value;
			}
		}

		public LookDevContext[] lookDevContexts
		{
			get
			{
				return this.m_LookDevContexts;
			}
		}

		public LookDevContext currentLookDevContext
		{
			get
			{
				return this.m_LookDevContexts[this.m_CurrentEditionContextIndex];
			}
		}

		public GameObject[] currentObject
		{
			get
			{
				return this.m_CurrentObject;
			}
		}

		public CameraState[] cameraState
		{
			get
			{
				return this.m_CameraState;
			}
		}

		public CameraState cameraStateCommon
		{
			get
			{
				return this.m_CameraStateCommon;
			}
			set
			{
				this.m_CameraStateCommon = value;
			}
		}

		public CameraState cameraStateLeft
		{
			get
			{
				return this.m_CameraStateLeft;
			}
			set
			{
				this.m_CameraStateLeft = value;
			}
		}

		public CameraState cameraStateRight
		{
			get
			{
				return this.m_CameraStateRight;
			}
			set
			{
				this.m_CameraStateRight = value;
			}
		}

		public LookDevMode lookDevMode
		{
			get
			{
				return this.m_LookDevMode;
			}
			set
			{
				this.m_LookDevMode = value;
				this.UpdateCameraArray();
				this.UpdateCurrentObjectArray();
			}
		}

		public bool enableToneMap
		{
			get
			{
				return this.m_EnableToneMap;
			}
			set
			{
				this.m_EnableToneMap = value;
				this.m_LookDevView.Repaint();
			}
		}

		public bool allowDifferentObjects
		{
			get
			{
				return this.m_AllowDifferentObjects;
			}
			set
			{
				this.m_AllowDifferentObjects = value;
				this.ResynchronizeObjects();
				this.m_LookDevView.Repaint();
			}
		}

		public float exposureRange
		{
			get
			{
				return this.m_ExposureRange;
			}
			set
			{
				this.m_ExposureRange = value;
				this.m_LookDevView.Repaint();
			}
		}

		public float shadowDistance
		{
			get
			{
				return this.m_ShadowDistance;
			}
			set
			{
				this.m_ShadowDistance = value;
				this.m_LookDevView.Repaint();
			}
		}

		public bool showBalls
		{
			get
			{
				return this.m_ShowBalls;
			}
			set
			{
				this.m_ShowBalls = value;
				this.m_LookDevView.Repaint();
			}
		}

		public bool showControlWindows
		{
			get
			{
				return this.m_ShowControlWindows;
			}
			set
			{
				this.m_ShowControlWindows = value;
				this.m_LookDevView.Repaint();
			}
		}

		public bool rotateObjectMode
		{
			get
			{
				return this.m_RotateObjectMode;
			}
			set
			{
				this.m_RotateObjectMode = value;
			}
		}

		public float objRotationSpeed
		{
			get
			{
				return this.m_ObjRotationSpeed;
			}
			set
			{
				this.m_ObjRotationSpeed = value;
				this.m_LookDevView.Repaint();
			}
		}

		public bool rotateEnvMode
		{
			get
			{
				return this.m_RotateEnvMode;
			}
			set
			{
				this.m_RotateEnvMode = value;
			}
		}

		public float envRotationSpeed
		{
			get
			{
				return this.m_EnvRotationSpeed;
			}
			set
			{
				this.m_EnvRotationSpeed = value;
				this.m_LookDevView.Repaint();
			}
		}

		public LookDevConfig()
		{
			this.m_LookDevProperties[0] = new LookDevPropertyInfo(LookDevPropertyType.Float);
			this.m_LookDevProperties[3] = new LookDevPropertyInfo(LookDevPropertyType.Float);
			this.m_LookDevProperties[1] = new LookDevPropertyInfo(LookDevPropertyType.Int);
			this.m_LookDevProperties[4] = new LookDevPropertyInfo(LookDevPropertyType.Int);
			this.m_LookDevProperties[2] = new LookDevPropertyInfo(LookDevPropertyType.Int);
		}

		public void UpdateFloatProperty(LookDevProperty type, float value)
		{
			this.UpdateFloatProperty(type, value, true, false);
		}

		public void UpdateFloatProperty(LookDevProperty type, float value, bool recordUndo)
		{
			this.UpdateFloatProperty(type, value, recordUndo, false);
		}

		public void UpdateIntProperty(LookDevProperty property, int value)
		{
			this.UpdateIntProperty(property, value, true, false);
		}

		public void UpdateIntProperty(LookDevProperty property, int value, bool recordUndo)
		{
			this.UpdateIntProperty(property, value, recordUndo, false);
		}

		public float GetFloatProperty(LookDevProperty property, LookDevEditionContext context)
		{
			return this.m_LookDevContexts[(int)context].GetProperty(property).floatValue;
		}

		public int GetIntProperty(LookDevProperty property, LookDevEditionContext context)
		{
			return this.m_LookDevContexts[(int)context].GetProperty(property).intValue;
		}

		public void UpdateFloatProperty(LookDevProperty property, float value, bool recordUndo, bool forceLinked)
		{
			if (recordUndo)
			{
				Undo.RecordObject(this, string.Concat(new object[]
				{
					"Update Float property for ",
					property,
					" with value ",
					value
				}));
			}
			this.lookDevContexts[this.m_CurrentEditionContextIndex].UpdateProperty(property, value);
			if (this.m_LookDevProperties[(int)property].linked || forceLinked)
			{
				this.lookDevContexts[(this.m_CurrentEditionContextIndex + 1) % 2].UpdateProperty(property, value);
			}
			this.m_LookDevView.Repaint();
		}

		public void UpdateIntProperty(LookDevProperty property, int value, bool recordUndo, bool forceLinked)
		{
			if (recordUndo)
			{
				Undo.RecordObject(this, string.Concat(new object[]
				{
					"Update Int property for ",
					property,
					" with value ",
					value
				}));
			}
			this.lookDevContexts[this.m_CurrentEditionContextIndex].UpdateProperty(property, value);
			if (this.m_LookDevProperties[(int)property].linked || forceLinked)
			{
				this.lookDevContexts[(this.m_CurrentEditionContextIndex + 1) % 2].UpdateProperty(property, value);
			}
			this.m_LookDevView.Repaint();
		}

		public bool IsPropertyLinked(LookDevProperty type)
		{
			return this.m_LookDevProperties[(int)type].linked;
		}

		public void UpdatePropertyLink(LookDevProperty property, bool value)
		{
			Undo.RecordObject(this, "Update Link for property " + property);
			this.m_LookDevProperties[(int)property].linked = value;
			LookDevPropertyType propertyType = this.m_LookDevProperties[(int)property].propertyType;
			if (propertyType != LookDevPropertyType.Int)
			{
				if (propertyType == LookDevPropertyType.Float)
				{
					this.UpdateFloatProperty(property, this.lookDevContexts[this.m_CurrentEditionContextIndex].GetProperty(property).floatValue, true, false);
				}
			}
			else
			{
				this.UpdateIntProperty(property, this.lookDevContexts[this.m_CurrentEditionContextIndex].GetProperty(property).intValue, true, false);
			}
			this.m_LookDevView.Repaint();
		}

		public int GetObjectLoDCount(LookDevEditionContext context)
		{
			int result;
			if (this.m_CurrentObject[(int)context] != null)
			{
				LODGroup lODGroup = this.m_CurrentObject[(int)context].GetComponent(typeof(LODGroup)) as LODGroup;
				if (lODGroup != null)
				{
					result = lODGroup.lodCount;
					return result;
				}
			}
			result = 1;
			return result;
		}

		public void UpdateFocus(LookDevEditionContext context)
		{
			if (context != LookDevEditionContext.None)
			{
				this.m_CurrentContextEdition = context;
				this.m_CurrentEditionContextIndex = (int)this.m_CurrentContextEdition;
				this.m_LookDevView.Repaint();
			}
		}

		private void DestroytCurrentPreviewObject(LookDevEditionContext context)
		{
			if (this.m_PreviewObjects[(int)context] != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_PreviewObjects[(int)context]);
				this.m_PreviewObjects[(int)context] = null;
			}
		}

		public void SetEnabledRecursive(GameObject go, bool enabled)
		{
			Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				renderer.enabled = enabled;
			}
		}

		private void DisableLightProbes(GameObject go)
		{
			Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				renderer.lightProbeUsage = LightProbeUsage.Off;
			}
		}

		public void ResynchronizeObjects()
		{
			Undo.RecordObject(this, "Resync objects");
			this.SetCurrentPreviewObject(this.m_OriginalGameObject[this.m_CurrentEditionContextIndex], (this.m_CurrentEditionContextIndex + LookDevEditionContext.Right) % LookDevEditionContext.None);
			this.m_LookDevView.Frame(false);
		}

		private void UpdateCameraArray()
		{
			if (this.m_LookDevMode == LookDevMode.SideBySide || this.m_LookDevMode == LookDevMode.Single1 || this.m_LookDevMode == LookDevMode.Single2)
			{
				this.m_CameraState[0] = this.m_CameraStateLeft;
				this.m_CameraState[1] = this.m_CameraStateRight;
			}
			else
			{
				this.m_CameraState[0] = this.m_CameraStateCommon;
				this.m_CameraState[1] = this.m_CameraStateCommon;
				CameraState cameraStateIn = (this.m_CurrentContextEdition != LookDevEditionContext.Left) ? this.m_CameraStateRight : this.m_CameraStateLeft;
				this.m_CameraStateCommon.Copy(cameraStateIn);
			}
		}

		public void UpdateCurrentObjectArray()
		{
			if (this.allowDifferentObjects)
			{
				this.m_CurrentObject[0] = this.m_PreviewObjects[0];
				this.m_CurrentObject[1] = this.m_PreviewObjects[1];
			}
			else
			{
				this.m_CurrentObject[this.m_CurrentEditionContextIndex] = this.m_PreviewObjects[this.m_CurrentEditionContextIndex];
				this.m_CurrentObject[(this.m_CurrentEditionContextIndex + 1) % 2] = this.m_PreviewObjects[this.m_CurrentEditionContextIndex];
			}
		}

		public bool SetCurrentPreviewObject(GameObject go)
		{
			this.SetCurrentPreviewObject(go, this.m_CurrentContextEdition);
			int num = (this.m_CurrentEditionContextIndex + 1) % 2;
			bool result;
			if (this.m_PreviewObjects[num] == null || !this.m_AllowDifferentObjects)
			{
				this.SetCurrentPreviewObject(go, (LookDevEditionContext)num);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void SetCurrentPreviewObject(GameObject go, LookDevEditionContext context)
		{
			this.DestroytCurrentPreviewObject(context);
			if (go != null)
			{
				this.m_OriginalGameObject[(int)context] = go;
				this.m_PreviewObjects[(int)context] = UnityEngine.Object.Instantiate<GameObject>(this.m_OriginalGameObject[(int)context], Vector3.zero, Quaternion.identity);
				EditorUtility.InitInstantiatedPreviewRecursive(this.m_PreviewObjects[(int)context]);
				this.SetEnabledRecursive(this.m_PreviewObjects[(int)context], false);
				this.DisableLightProbes(this.m_PreviewObjects[(int)context]);
				this.UpdateCurrentObjectArray();
			}
		}

		public void OnEnable()
		{
			if (this.m_LookDevContexts[0] == null)
			{
				for (int i = 0; i < 2; i++)
				{
					this.m_LookDevContexts[i] = new LookDevContext();
				}
			}
			for (int j = 0; j < 2; j++)
			{
				if (this.m_OriginalGameObject[j] != null)
				{
					this.SetCurrentPreviewObject(this.m_OriginalGameObject[j], (LookDevEditionContext)j);
				}
			}
			this.UpdateCameraArray();
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnUndoRedo));
		}

		private void OnUndoRedo()
		{
			for (int i = 0; i < 2; i++)
			{
				if (this.m_OriginalGameObject[i] != null)
				{
					this.SetCurrentPreviewObject(this.m_OriginalGameObject[i], (LookDevEditionContext)i);
				}
			}
		}

		public void OnDestroy()
		{
			this.DestroytCurrentPreviewObject(LookDevEditionContext.Left);
			this.DestroytCurrentPreviewObject(LookDevEditionContext.Right);
		}

		public void Cleanup()
		{
			this.m_CurrentEditionContextIndex = 0;
		}

		public void SetLookDevView(LookDevView lookDevView)
		{
			this.m_LookDevView = lookDevView;
		}
	}
}
