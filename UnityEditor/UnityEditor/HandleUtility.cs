using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class HandleUtility
	{
		internal delegate GameObject PickClosestGameObjectFunc(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);

		private sealed class SavedCamera
		{
			private float near;

			private float far;

			private Rect pixelRect;

			private Vector3 pos;

			private Quaternion rot;

			private CameraClearFlags clearFlags;

			private int cullingMask;

			private float fov;

			private float orthographicSize;

			private bool isOrtho;

			internal SavedCamera(Camera source)
			{
				this.near = source.nearClipPlane;
				this.far = source.farClipPlane;
				this.pixelRect = source.pixelRect;
				this.pos = source.transform.position;
				this.rot = source.transform.rotation;
				this.clearFlags = source.clearFlags;
				this.cullingMask = source.cullingMask;
				this.fov = source.fieldOfView;
				this.orthographicSize = source.orthographicSize;
				this.isOrtho = source.orthographic;
			}

			internal void Restore(Camera dest)
			{
				dest.nearClipPlane = this.near;
				dest.farClipPlane = this.far;
				dest.pixelRect = this.pixelRect;
				dest.transform.position = this.pos;
				dest.transform.rotation = this.rot;
				dest.clearFlags = this.clearFlags;
				dest.fieldOfView = this.fov;
				dest.orthographicSize = this.orthographicSize;
				dest.orthographic = this.isOrtho;
				dest.cullingMask = this.cullingMask;
			}
		}

		private static bool s_UseYSign = false;

		private static bool s_UseYSignZoom = false;

		private static Vector3[] points = new Vector3[]
		{
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero
		};

		private static int s_NearestControl;

		private static float s_NearestDistance;

		internal const float kPickDistance = 5f;

		internal static float s_CustomPickDistance = 5f;

		private const float kHandleSize = 80f;

		internal static HandleUtility.PickClosestGameObjectFunc pickClosestGameObjectDelegate;

		private static Material s_HandleMaterial;

		private static Material s_HandleWireMaterial;

		private static Material s_HandleWireMaterial2D;

		private static int s_HandleWireTextureIndex;

		private static int s_HandleWireTextureIndex2D;

		private static Material s_HandleDottedWireMaterial;

		private static Material s_HandleDottedWireMaterial2D;

		private static int s_HandleDottedWireTextureIndex;

		private static int s_HandleDottedWireTextureIndex2D;

		private static Stack s_SavedCameras = new Stack();

		internal static Transform[] ignoreRaySnapObjects = null;

		public static float acceleration
		{
			get
			{
				return (float)((!Event.current.shift) ? 1 : 4) * ((!Event.current.alt) ? 1f : 0.25f);
			}
		}

		public static float niceMouseDelta
		{
			get
			{
				Vector2 delta = Event.current.delta;
				delta.y = -delta.y;
				if (Mathf.Abs(Mathf.Abs(delta.x) - Mathf.Abs(delta.y)) / Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y)) > 0.1f)
				{
					if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
					{
						HandleUtility.s_UseYSign = false;
					}
					else
					{
						HandleUtility.s_UseYSign = true;
					}
				}
				float result;
				if (HandleUtility.s_UseYSign)
				{
					result = Mathf.Sign(delta.y) * delta.magnitude * HandleUtility.acceleration;
				}
				else
				{
					result = Mathf.Sign(delta.x) * delta.magnitude * HandleUtility.acceleration;
				}
				return result;
			}
		}

		public static float niceMouseDeltaZoom
		{
			get
			{
				Vector2 vector = -Event.current.delta;
				if (Mathf.Abs(Mathf.Abs(vector.x) - Mathf.Abs(vector.y)) / Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y)) > 0.1f)
				{
					if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
					{
						HandleUtility.s_UseYSignZoom = false;
					}
					else
					{
						HandleUtility.s_UseYSignZoom = true;
					}
				}
				float result;
				if (HandleUtility.s_UseYSignZoom)
				{
					result = Mathf.Sign(vector.y) * vector.magnitude * HandleUtility.acceleration;
				}
				else
				{
					result = Mathf.Sign(vector.x) * vector.magnitude * HandleUtility.acceleration;
				}
				return result;
			}
		}

		public static int nearestControl
		{
			get
			{
				return (HandleUtility.s_NearestDistance > 5f) ? 0 : HandleUtility.s_NearestControl;
			}
			set
			{
				HandleUtility.s_NearestControl = value;
			}
		}

		public static Material handleMaterial
		{
			get
			{
				if (!HandleUtility.s_HandleMaterial)
				{
					HandleUtility.s_HandleMaterial = (Material)EditorGUIUtility.Load("SceneView/Handles.mat");
				}
				return HandleUtility.s_HandleMaterial;
			}
		}

		private static Material handleWireMaterial
		{
			get
			{
				HandleUtility.InitHandleMaterials();
				return (!Camera.current) ? HandleUtility.s_HandleWireMaterial2D : HandleUtility.s_HandleWireMaterial;
			}
		}

		private static Material handleDottedWireMaterial
		{
			get
			{
				HandleUtility.InitHandleMaterials();
				return (!Camera.current) ? HandleUtility.s_HandleDottedWireMaterial2D : HandleUtility.s_HandleDottedWireMaterial;
			}
		}

		public static float CalcLineTranslation(Vector2 src, Vector2 dest, Vector3 srcPosition, Vector3 constraintDir)
		{
			srcPosition = Handles.matrix.MultiplyPoint(srcPosition);
			constraintDir = Handles.matrix.MultiplyVector(constraintDir);
			float num = 1f;
			Vector3 forward = Camera.current.transform.forward;
			if (Vector3.Dot(constraintDir, forward) < 0f)
			{
				num = -1f;
			}
			Vector3 vector = constraintDir;
			vector.y = -vector.y;
			Camera current = Camera.current;
			Vector2 vector2 = EditorGUIUtility.PixelsToPoints(current.WorldToScreenPoint(srcPosition));
			Vector2 vector3 = EditorGUIUtility.PixelsToPoints(current.WorldToScreenPoint(srcPosition + constraintDir * num));
			Vector2 x = dest;
			Vector2 x2 = src;
			float result;
			if (vector2 == vector3)
			{
				result = 0f;
			}
			else
			{
				x.y = -x.y;
				x2.y = -x2.y;
				float parametrization = HandleUtility.GetParametrization(x2, vector2, vector3);
				float parametrization2 = HandleUtility.GetParametrization(x, vector2, vector3);
				float num2 = (parametrization2 - parametrization) * num;
				result = num2;
			}
			return result;
		}

		internal static float GetParametrization(Vector2 x0, Vector2 x1, Vector2 x2)
		{
			return -(Vector2.Dot(x1 - x0, x2 - x1) / (x2 - x1).sqrMagnitude);
		}

		public static float PointOnLineParameter(Vector3 point, Vector3 linePoint, Vector3 lineDirection)
		{
			return Vector3.Dot(lineDirection, point - linePoint) / lineDirection.sqrMagnitude;
		}

		public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 rhs = point - lineStart;
			Vector3 vector = lineEnd - lineStart;
			float magnitude = vector.magnitude;
			Vector3 vector2 = vector;
			if (magnitude > 1E-06f)
			{
				vector2 /= magnitude;
			}
			float num = Vector3.Dot(vector2, rhs);
			num = Mathf.Clamp(num, 0f, magnitude);
			return lineStart + vector2 * num;
		}

		public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
		{
			return Vector3.Magnitude(HandleUtility.ProjectPointLine(point, lineStart, lineEnd) - point);
		}

		public static float DistancePointBezier(Vector3 point, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
		{
			return HandleUtility.INTERNAL_CALL_DistancePointBezier(ref point, ref startPosition, ref endPosition, ref startTangent, ref endTangent);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_DistancePointBezier(ref Vector3 point, ref Vector3 startPosition, ref Vector3 endPosition, ref Vector3 startTangent, ref Vector3 endTangent);

		public static float DistanceToLine(Vector3 p1, Vector3 p2)
		{
			p1 = HandleUtility.WorldToGUIPoint(p1);
			p2 = HandleUtility.WorldToGUIPoint(p2);
			Vector2 mousePosition = Event.current.mousePosition;
			float num = HandleUtility.DistancePointLine(mousePosition, p1, p2);
			if (num < 0f)
			{
				num = 0f;
			}
			return num;
		}

		public static float DistanceToCircle(Vector3 position, float radius)
		{
			Vector2 a = HandleUtility.WorldToGUIPoint(position);
			Camera current = Camera.current;
			Vector2 b = Vector2.zero;
			if (current)
			{
				b = HandleUtility.WorldToGUIPoint(position + current.transform.right * radius);
				radius = (a - b).magnitude;
			}
			float magnitude = (a - Event.current.mousePosition).magnitude;
			float result;
			if (magnitude < radius)
			{
				result = 0f;
			}
			else
			{
				result = magnitude - radius;
			}
			return result;
		}

		public static float DistanceToRectangle(Vector3 position, Quaternion rotation, float size)
		{
			return HandleUtility.DistanceToRectangleInternal(position, rotation, new Vector2(size, size));
		}

		internal static float DistanceToRectangleInternal(Vector3 position, Quaternion rotation, Vector2 size)
		{
			Vector3 b = rotation * new Vector3(size.x, 0f, 0f);
			Vector3 b2 = rotation * new Vector3(0f, size.y, 0f);
			HandleUtility.points[0] = HandleUtility.WorldToGUIPoint(position + b + b2);
			HandleUtility.points[1] = HandleUtility.WorldToGUIPoint(position + b - b2);
			HandleUtility.points[2] = HandleUtility.WorldToGUIPoint(position - b - b2);
			HandleUtility.points[3] = HandleUtility.WorldToGUIPoint(position - b + b2);
			HandleUtility.points[4] = HandleUtility.points[0];
			Vector2 mousePosition = Event.current.mousePosition;
			bool flag = false;
			int num = 4;
			for (int i = 0; i < 5; i++)
			{
				if (HandleUtility.points[i].y > mousePosition.y != HandleUtility.points[num].y > mousePosition.y)
				{
					if (mousePosition.x < (HandleUtility.points[num].x - HandleUtility.points[i].x) * (mousePosition.y - HandleUtility.points[i].y) / (HandleUtility.points[num].y - HandleUtility.points[i].y) + HandleUtility.points[i].x)
					{
						flag = !flag;
					}
				}
				num = i;
			}
			float result;
			if (!flag)
			{
				float num2 = -1f;
				num = 1;
				for (int j = 0; j < 4; j++)
				{
					float num3 = HandleUtility.DistancePointToLineSegment(mousePosition, HandleUtility.points[j], HandleUtility.points[num++]);
					if (num3 < num2 || num2 < 0f)
					{
						num2 = num3;
					}
				}
				result = num2;
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		internal static float DistanceToDiamond(Vector3 position, Quaternion rotation, float size)
		{
			return HandleUtility.DistanceToDiamondInternal(position, rotation, size, Event.current.mousePosition);
		}

		internal static float DistanceToDiamondInternal(Vector3 position, Quaternion rotation, float size, Vector2 mousePosition)
		{
			Vector3 b = rotation * new Vector3(size, 0f, 0f);
			Vector3 b2 = rotation * new Vector3(0f, size, 0f);
			HandleUtility.points[0] = HandleUtility.WorldToGUIPoint(position + b);
			HandleUtility.points[1] = HandleUtility.WorldToGUIPoint(position - b2);
			HandleUtility.points[2] = HandleUtility.WorldToGUIPoint(position - b);
			HandleUtility.points[3] = HandleUtility.WorldToGUIPoint(position + b2);
			HandleUtility.points[4] = HandleUtility.points[0];
			Vector2 p = mousePosition;
			bool flag = false;
			int num = 4;
			for (int i = 0; i < 5; i++)
			{
				if (HandleUtility.points[i].y > p.y != HandleUtility.points[num].y > p.y)
				{
					if (p.x < (HandleUtility.points[num].x - HandleUtility.points[i].x) * (p.y - HandleUtility.points[i].y) / (HandleUtility.points[num].y - HandleUtility.points[i].y) + HandleUtility.points[i].x)
					{
						flag = !flag;
					}
				}
				num = i;
			}
			float result;
			if (!flag)
			{
				float num2 = -1f;
				num = 1;
				for (int j = 0; j < 4; j++)
				{
					float num3 = HandleUtility.DistancePointToLineSegment(p, HandleUtility.points[j], HandleUtility.points[num++]);
					if (num3 < num2 || num2 < 0f)
					{
						num2 = num3;
					}
				}
				result = num2;
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		public static float DistancePointToLine(Vector2 p, Vector2 a, Vector2 b)
		{
			return Mathf.Abs((b.x - a.x) * (a.y - p.y) - (a.x - p.x) * (b.y - a.y)) / (b - a).magnitude;
		}

		public static float DistancePointToLineSegment(Vector2 p, Vector2 a, Vector2 b)
		{
			float sqrMagnitude = (b - a).sqrMagnitude;
			float magnitude;
			if ((double)sqrMagnitude == 0.0)
			{
				magnitude = (p - a).magnitude;
			}
			else
			{
				float num = Vector2.Dot(p - a, b - a) / sqrMagnitude;
				if ((double)num < 0.0)
				{
					magnitude = (p - a).magnitude;
				}
				else if ((double)num > 1.0)
				{
					magnitude = (p - b).magnitude;
				}
				else
				{
					Vector2 b2 = a + num * (b - a);
					magnitude = (p - b2).magnitude;
				}
			}
			return magnitude;
		}

		public static float DistanceToDisc(Vector3 center, Vector3 normal, float radius)
		{
			Vector3 from = Vector3.Cross(normal, Vector3.up);
			if (from.sqrMagnitude < 0.001f)
			{
				from = Vector3.Cross(normal, Vector3.right);
			}
			return HandleUtility.DistanceToArc(center, normal, from, 360f, radius);
		}

		public static Vector3 ClosestPointToDisc(Vector3 center, Vector3 normal, float radius)
		{
			Vector3 from = Vector3.Cross(normal, Vector3.up);
			if (from.sqrMagnitude < 0.001f)
			{
				from = Vector3.Cross(normal, Vector3.right);
			}
			return HandleUtility.ClosestPointToArc(center, normal, from, 360f, radius);
		}

		public static float DistanceToArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
		{
			Vector3[] dest = new Vector3[60];
			Handles.SetDiscSectionPoints(dest, center, normal, from, angle, radius);
			return HandleUtility.DistanceToPolyLine(dest);
		}

		public static Vector3 ClosestPointToArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
		{
			Vector3[] array = new Vector3[60];
			Handles.SetDiscSectionPoints(array, center, normal, from, angle, radius);
			return HandleUtility.ClosestPointToPolyLine(array);
		}

		public static float DistanceToPolyLine(params Vector3[] points)
		{
			float num = HandleUtility.DistanceToLine(points[0], points[1]);
			for (int i = 2; i < points.Length; i++)
			{
				float num2 = HandleUtility.DistanceToLine(points[i - 1], points[i]);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		public static Vector3 ClosestPointToPolyLine(params Vector3[] vertices)
		{
			float num = HandleUtility.DistanceToLine(vertices[0], vertices[1]);
			int num2 = 0;
			for (int i = 2; i < vertices.Length; i++)
			{
				float num3 = HandleUtility.DistanceToLine(vertices[i - 1], vertices[i]);
				if (num3 < num)
				{
					num = num3;
					num2 = i - 1;
				}
			}
			Vector3 vector = vertices[num2];
			Vector3 vector2 = vertices[num2 + 1];
			Vector2 v = Event.current.mousePosition - HandleUtility.WorldToGUIPoint(vector);
			Vector2 v2 = HandleUtility.WorldToGUIPoint(vector2) - HandleUtility.WorldToGUIPoint(vector);
			float magnitude = v2.magnitude;
			float num4 = Vector3.Dot(v2, v);
			if (magnitude > 1E-06f)
			{
				num4 /= magnitude * magnitude;
			}
			num4 = Mathf.Clamp01(num4);
			return Vector3.Lerp(vector, vector2, num4);
		}

		public static void AddControl(int controlId, float distance)
		{
			if (distance < HandleUtility.s_CustomPickDistance && distance > 5f)
			{
				distance = 5f;
			}
			if (distance <= HandleUtility.s_NearestDistance)
			{
				HandleUtility.s_NearestDistance = distance;
				HandleUtility.s_NearestControl = controlId;
			}
		}

		public static void AddDefaultControl(int controlId)
		{
			HandleUtility.AddControl(controlId, 5f);
		}

		[RequiredByNativeCode]
		private static void BeginHandles()
		{
			Handles.Init();
			EventType type = Event.current.type;
			if (type == EventType.Layout)
			{
				HandleUtility.s_NearestControl = 0;
				HandleUtility.s_NearestDistance = 5f;
			}
			Handles.lighting = true;
			Handles.color = Color.white;
			Handles.zTest = CompareFunction.Always;
			HandleUtility.s_CustomPickDistance = 5f;
			Handles.Internal_SetCurrentCamera(null);
			EditorGUI.s_DelayedTextEditor.BeginGUI();
		}

		[RequiredByNativeCode]
		private static void SetViewInfo(Vector2 screenPosition)
		{
			GUIUtility.s_EditorScreenPointOffset = screenPosition;
		}

		[RequiredByNativeCode]
		private static void EndHandles()
		{
			EditorGUI.s_DelayedTextEditor.EndGUI(Event.current.type);
		}

		public static float GetHandleSize(Vector3 position)
		{
			Camera current = Camera.current;
			position = Handles.matrix.MultiplyPoint(position);
			float result;
			if (current)
			{
				Transform transform = current.transform;
				Vector3 position2 = transform.position;
				float z = Vector3.Dot(position - position2, transform.TransformDirection(new Vector3(0f, 0f, 1f)));
				Vector3 a = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(0f, 0f, z)));
				Vector3 b = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(1f, 0f, z)));
				float magnitude = (a - b).magnitude;
				result = 80f / Mathf.Max(magnitude, 0.0001f) * EditorGUIUtility.pixelsPerPoint;
			}
			else
			{
				result = 20f;
			}
			return result;
		}

		public static Vector2 WorldToGUIPoint(Vector3 world)
		{
			world = Handles.matrix.MultiplyPoint(world);
			Camera current = Camera.current;
			Vector2 result;
			if (current)
			{
				Vector2 vector = current.WorldToScreenPoint(world);
				vector.y = (float)Screen.height - vector.y;
				vector = EditorGUIUtility.PixelsToPoints(vector);
				result = GUIClip.Clip(vector);
			}
			else
			{
				result = new Vector2(world.x, world.y);
			}
			return result;
		}

		public static Vector2 GUIPointToScreenPixelCoordinate(Vector2 guiPoint)
		{
			Vector2 position = GUIClip.Unclip(guiPoint);
			Vector2 result = EditorGUIUtility.PointsToPixels(position);
			result.y = (float)Screen.height - result.y;
			return result;
		}

		public static Ray GUIPointToWorldRay(Vector2 position)
		{
			Ray result;
			if (!Camera.current)
			{
				Debug.LogError("Unable to convert GUI point to world ray if a camera has not been set up!");
				result = new Ray(Vector3.zero, Vector3.forward);
			}
			else
			{
				Vector2 v = HandleUtility.GUIPointToScreenPixelCoordinate(position);
				Camera current = Camera.current;
				result = current.ScreenPointToRay(v);
			}
			return result;
		}

		public static Rect WorldPointToSizedRect(Vector3 position, GUIContent content, GUIStyle style)
		{
			Vector2 vector = HandleUtility.WorldToGUIPoint(position);
			Vector2 vector2 = style.CalcSize(content);
			Rect rect = new Rect(vector.x, vector.y, vector2.x, vector2.y);
			switch (style.alignment)
			{
			case TextAnchor.UpperCenter:
				rect.xMin -= rect.width * 0.5f;
				break;
			case TextAnchor.UpperRight:
				rect.xMin -= rect.width;
				break;
			case TextAnchor.MiddleLeft:
				rect.yMin -= rect.height * 0.5f;
				break;
			case TextAnchor.MiddleCenter:
				rect.xMin -= rect.width * 0.5f;
				rect.yMin -= rect.height * 0.5f;
				break;
			case TextAnchor.MiddleRight:
				rect.xMin -= rect.width;
				rect.yMin -= rect.height * 0.5f;
				break;
			case TextAnchor.LowerLeft:
				rect.yMin -= rect.height * 0.5f;
				break;
			case TextAnchor.LowerCenter:
				rect.xMin -= rect.width * 0.5f;
				rect.yMin -= rect.height;
				break;
			case TextAnchor.LowerRight:
				rect.xMin -= rect.width;
				rect.yMin -= rect.height;
				break;
			}
			return style.padding.Add(rect);
		}

		public static GameObject[] PickRectObjects(Rect rect)
		{
			return HandleUtility.PickRectObjects(rect, true);
		}

		public static GameObject[] PickRectObjects(Rect rect, bool selectPrefabRootsOnly)
		{
			Camera current = Camera.current;
			rect = EditorGUIUtility.PointsToPixels(rect);
			rect.x /= (float)current.pixelWidth;
			rect.width /= (float)current.pixelWidth;
			rect.y /= (float)current.pixelHeight;
			rect.height /= (float)current.pixelHeight;
			return HandleUtility.Internal_PickRectObjects(current, rect, selectPrefabRootsOnly);
		}

		internal static GameObject[] Internal_PickRectObjects(Camera cam, Rect rect, bool selectPrefabRoots)
		{
			return HandleUtility.INTERNAL_CALL_Internal_PickRectObjects(cam, ref rect, selectPrefabRoots);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GameObject[] INTERNAL_CALL_Internal_PickRectObjects(Camera cam, ref Rect rect, bool selectPrefabRoots);

		internal static bool FindNearestVertex(Vector2 guiPoint, Transform[] objectsToSearch, out Vector3 vertex)
		{
			Camera current = Camera.current;
			Vector2 screenPoint = EditorGUIUtility.PointsToPixels(guiPoint);
			screenPoint.y = current.pixelRect.yMax - screenPoint.y;
			return HandleUtility.Internal_FindNearestVertex(current, screenPoint, objectsToSearch, HandleUtility.ignoreRaySnapObjects, out vertex);
		}

		private static bool Internal_FindNearestVertex(Camera cam, Vector2 screenPoint, Transform[] objectsToSearch, Transform[] ignoreObjects, out Vector3 vertex)
		{
			return HandleUtility.INTERNAL_CALL_Internal_FindNearestVertex(cam, ref screenPoint, objectsToSearch, ignoreObjects, out vertex);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_FindNearestVertex(Camera cam, ref Vector2 screenPoint, Transform[] objectsToSearch, Transform[] ignoreObjects, out Vector3 vertex);

		public static GameObject PickGameObject(Vector2 position, out int materialIndex)
		{
			return HandleUtility.PickGameObjectDelegated(position, null, null, out materialIndex);
		}

		public static GameObject PickGameObject(Vector2 position, GameObject[] ignore, out int materialIndex)
		{
			return HandleUtility.PickGameObjectDelegated(position, ignore, null, out materialIndex);
		}

		internal static GameObject PickGameObjectDelegated(Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex)
		{
			Camera current = Camera.current;
			int cullingMask = current.cullingMask;
			position = GUIClip.Unclip(position);
			position = EditorGUIUtility.PointsToPixels(position);
			position.y = (float)Screen.height - position.y - current.pixelRect.yMin;
			materialIndex = -1;
			GameObject gameObject = null;
			if (HandleUtility.pickClosestGameObjectDelegate != null)
			{
				gameObject = HandleUtility.pickClosestGameObjectDelegate(current, cullingMask, position, ignore, filter, out materialIndex);
			}
			if (gameObject == null)
			{
				gameObject = HandleUtility.Internal_PickClosestGO(current, cullingMask, position, ignore, filter, out materialIndex);
			}
			return gameObject;
		}

		public static GameObject PickGameObject(Vector2 position, bool selectPrefabRoot)
		{
			return HandleUtility.PickGameObject(position, selectPrefabRoot, null);
		}

		public static GameObject PickGameObject(Vector2 position, bool selectPrefabRoot, GameObject[] ignore)
		{
			return HandleUtility.PickGameObject(position, selectPrefabRoot, ignore, null);
		}

		internal static GameObject PickGameObject(Vector2 position, bool selectPrefabRoot, GameObject[] ignore, GameObject[] filter)
		{
			int num;
			GameObject gameObject = HandleUtility.PickGameObjectDelegated(position, ignore, filter, out num);
			GameObject result;
			if (gameObject && selectPrefabRoot)
			{
				GameObject gameObject2 = HandleUtility.FindSelectionBase(gameObject) ?? gameObject;
				Transform activeTransform = Selection.activeTransform;
				GameObject y = (!activeTransform) ? null : (HandleUtility.FindSelectionBase(activeTransform.gameObject) ?? activeTransform.gameObject);
				if (gameObject2 == y)
				{
					result = gameObject;
				}
				else
				{
					result = gameObject2;
				}
			}
			else
			{
				result = gameObject;
			}
			return result;
		}

		internal static GameObject FindSelectionBase(GameObject go)
		{
			GameObject result;
			if (go == null)
			{
				result = null;
			}
			else
			{
				Transform y = null;
				PrefabType prefabType = PrefabUtility.GetPrefabType(go);
				if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.ModelPrefabInstance)
				{
					y = PrefabUtility.FindPrefabRoot(go).transform;
				}
				Transform transform = go.transform;
				while (transform != null)
				{
					if (transform == y)
					{
						result = transform.gameObject;
						return result;
					}
					if (AttributeHelper.GameObjectContainsAttribute(transform.gameObject, typeof(SelectionBaseAttribute)))
					{
						result = transform.gameObject;
						return result;
					}
					transform = transform.parent;
				}
				result = null;
			}
			return result;
		}

		internal static GameObject Internal_PickClosestGO(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex)
		{
			return HandleUtility.INTERNAL_CALL_Internal_PickClosestGO(cam, layers, ref position, ignore, filter, out materialIndex);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GameObject INTERNAL_CALL_Internal_PickClosestGO(Camera cam, int layers, ref Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);

		private static void InitHandleMaterials()
		{
			if (!HandleUtility.s_HandleWireMaterial)
			{
				HandleUtility.s_HandleWireMaterial = (Material)EditorGUIUtility.LoadRequired("SceneView/HandleLines.mat");
				HandleUtility.s_HandleWireMaterial2D = (Material)EditorGUIUtility.LoadRequired("SceneView/2DHandleLines.mat");
				HandleUtility.s_HandleWireTextureIndex = ShaderUtil.GetTextureBindingIndex(HandleUtility.s_HandleWireMaterial.shader, Shader.PropertyToID("_MainTex"));
				HandleUtility.s_HandleWireTextureIndex2D = ShaderUtil.GetTextureBindingIndex(HandleUtility.s_HandleWireMaterial2D.shader, Shader.PropertyToID("_MainTex"));
				HandleUtility.s_HandleDottedWireMaterial = (Material)EditorGUIUtility.LoadRequired("SceneView/HandleDottedLines.mat");
				HandleUtility.s_HandleDottedWireMaterial2D = (Material)EditorGUIUtility.LoadRequired("SceneView/2DHandleDottedLines.mat");
				HandleUtility.s_HandleDottedWireTextureIndex = ShaderUtil.GetTextureBindingIndex(HandleUtility.s_HandleDottedWireMaterial.shader, Shader.PropertyToID("_MainTex"));
				HandleUtility.s_HandleDottedWireTextureIndex2D = ShaderUtil.GetTextureBindingIndex(HandleUtility.s_HandleDottedWireMaterial2D.shader, Shader.PropertyToID("_MainTex"));
			}
		}

		[ExcludeFromDocs]
		internal static void ApplyWireMaterial()
		{
			CompareFunction zTest = CompareFunction.Always;
			HandleUtility.ApplyWireMaterial(zTest);
		}

		internal static void ApplyWireMaterial([DefaultValue("UnityEngine.Rendering.CompareFunction.Always")] CompareFunction zTest)
		{
			Material handleWireMaterial = HandleUtility.handleWireMaterial;
			handleWireMaterial.SetInt("_HandleZTest", (int)zTest);
			handleWireMaterial.SetPass(0);
			int textureIndex = (!Camera.current) ? HandleUtility.s_HandleWireTextureIndex2D : HandleUtility.s_HandleWireTextureIndex;
			HandleUtility.Internal_SetHandleWireTextureIndex(textureIndex);
		}

		[ExcludeFromDocs]
		internal static void ApplyDottedWireMaterial()
		{
			CompareFunction zTest = CompareFunction.Always;
			HandleUtility.ApplyDottedWireMaterial(zTest);
		}

		internal static void ApplyDottedWireMaterial([DefaultValue("UnityEngine.Rendering.CompareFunction.Always")] CompareFunction zTest)
		{
			Material handleDottedWireMaterial = HandleUtility.handleDottedWireMaterial;
			handleDottedWireMaterial.SetInt("_HandleZTest", (int)zTest);
			handleDottedWireMaterial.SetPass(0);
			int textureIndex = (!Camera.current) ? HandleUtility.s_HandleDottedWireTextureIndex2D : HandleUtility.s_HandleDottedWireTextureIndex;
			HandleUtility.Internal_SetHandleWireTextureIndex(textureIndex);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetHandleWireTextureIndex(int textureIndex);

		public static void PushCamera(Camera camera)
		{
			HandleUtility.s_SavedCameras.Push(new HandleUtility.SavedCamera(camera));
		}

		public static void PopCamera(Camera camera)
		{
			HandleUtility.SavedCamera savedCamera = (HandleUtility.SavedCamera)HandleUtility.s_SavedCameras.Pop();
			savedCamera.Restore(camera);
		}

		public static object RaySnap(Ray ray)
		{
			RaycastHit[] array = Physics.RaycastAll(ray, float.PositiveInfinity, Camera.current.cullingMask);
			float num = float.PositiveInfinity;
			int num2 = -1;
			if (HandleUtility.ignoreRaySnapObjects != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (!array[i].collider.isTrigger && array[i].distance < num)
					{
						bool flag = false;
						for (int j = 0; j < HandleUtility.ignoreRaySnapObjects.Length; j++)
						{
							if (array[i].transform == HandleUtility.ignoreRaySnapObjects[j])
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							num = array[i].distance;
							num2 = i;
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < array.Length; k++)
				{
					if (array[k].distance < num)
					{
						num = array[k].distance;
						num2 = k;
					}
				}
			}
			object result;
			if (num2 >= 0)
			{
				result = array[num2];
			}
			else
			{
				result = null;
			}
			return result;
		}

		internal static float CalcRayPlaceOffset(Transform[] objects, Vector3 normal)
		{
			return HandleUtility.INTERNAL_CALL_CalcRayPlaceOffset(objects, ref normal);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_CalcRayPlaceOffset(Transform[] objects, ref Vector3 normal);

		public static void Repaint()
		{
			HandleUtility.Internal_Repaint();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Repaint();

		internal static bool IntersectRayMesh(Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastHit hit)
		{
			return HandleUtility.INTERNAL_CALL_IntersectRayMesh(ref ray, mesh, ref matrix, out hit);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IntersectRayMesh(ref Ray ray, Mesh mesh, ref Matrix4x4 matrix, out RaycastHit hit);
	}
}
