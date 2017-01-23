using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditor
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class SerializedProperty
	{
		private IntPtr m_Property;

		internal SerializedObject m_SerializedObject;

		public SerializedObject serializedObject
		{
			get
			{
				return this.m_SerializedObject;
			}
		}

		public extern bool hasMultipleDifferentValues
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern int hasMultipleDifferentValuesBitwise
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string displayName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string type
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string tooltip
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string propertyPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern int hashCodeForPropertyPathWithoutArrayIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool editable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isAnimated
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isExpanded
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasChildren
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasVisibleChildren
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isInstantiatedPrefab
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool prefabOverride
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern SerializedPropertyType propertyType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int intValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern long longValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool boolValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float floatValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern double doubleValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string stringValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color colorValue
		{
			get
			{
				Color result;
				this.INTERNAL_get_colorValue(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_colorValue(ref value);
			}
		}

		public extern AnimationCurve animationCurveValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal extern Gradient gradientValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern UnityEngine.Object objectReferenceValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int objectReferenceInstanceIDValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal extern string objectReferenceStringValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern string objectReferenceTypeString
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern string layerMaskStringValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int enumValueIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string[] enumNames
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string[] enumDisplayNames
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector2 vector2Value
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_vector2Value(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_vector2Value(ref value);
			}
		}

		public Vector3 vector3Value
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_vector3Value(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_vector3Value(ref value);
			}
		}

		public Vector4 vector4Value
		{
			get
			{
				Vector4 result;
				this.INTERNAL_get_vector4Value(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_vector4Value(ref value);
			}
		}

		public Quaternion quaternionValue
		{
			get
			{
				Quaternion result;
				this.INTERNAL_get_quaternionValue(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_quaternionValue(ref value);
			}
		}

		public Rect rectValue
		{
			get
			{
				Rect result;
				this.INTERNAL_get_rectValue(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_rectValue(ref value);
			}
		}

		public Bounds boundsValue
		{
			get
			{
				Bounds result;
				this.INTERNAL_get_boundsValue(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_boundsValue(ref value);
			}
		}

		public extern bool isArray
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int arraySize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal SerializedProperty()
		{
		}

		~SerializedProperty()
		{
			this.Dispose();
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool EqualContents(SerializedProperty x, SerializedProperty y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetBitAtIndexForAllTargetsImmediate(int index, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_colorValue(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_colorValue(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool ValidateObjectReferenceValue(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AppendFoldoutPPtrValue(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_vector2Value(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_vector2Value(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_vector3Value(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_vector3Value(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_vector4Value(out Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_vector4Value(ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_quaternionValue(out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_quaternionValue(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rectValue(out Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_rectValue(ref Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_boundsValue(out Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_boundsValue(ref Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Next(bool enterChildren);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool NextVisible(bool enterChildren);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Reset();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int CountRemaining();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int CountInProperty();

		public SerializedProperty Copy()
		{
			SerializedProperty serializedProperty = this.CopyInternal();
			serializedProperty.m_SerializedObject = this.m_SerializedObject;
			return serializedProperty;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern SerializedProperty CopyInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool DuplicateCommand();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool DeleteCommand();

		public SerializedProperty FindPropertyRelative(string relativePropertyPath)
		{
			SerializedProperty serializedProperty = this.Copy();
			SerializedProperty result;
			if (serializedProperty.FindPropertyRelativeInternal(relativePropertyPath))
			{
				result = serializedProperty;
			}
			else
			{
				result = null;
			}
			return result;
		}

		[ExcludeFromDocs]
		public SerializedProperty GetEndProperty()
		{
			bool includeInvisible = false;
			return this.GetEndProperty(includeInvisible);
		}

		public SerializedProperty GetEndProperty([DefaultValue("false")] bool includeInvisible)
		{
			SerializedProperty serializedProperty = this.Copy();
			if (includeInvisible)
			{
				serializedProperty.Next(false);
			}
			else
			{
				serializedProperty.NextVisible(false);
			}
			return serializedProperty;
		}

		[DebuggerHidden]
		public IEnumerator GetEnumerator()
		{
			SerializedProperty.<GetEnumerator>c__Iterator0 <GetEnumerator>c__Iterator = new SerializedProperty.<GetEnumerator>c__Iterator0();
			<GetEnumerator>c__Iterator.$this = this;
			return <GetEnumerator>c__Iterator;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool FindPropertyInternal(string propertyPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool FindPropertyRelativeInternal(string propertyPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int[] GetLayerMaskSelectedIndex();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string[] GetLayerMaskNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ToggleLayerMaskAtIndex(int index);

		public SerializedProperty GetArrayElementAtIndex(int index)
		{
			SerializedProperty serializedProperty = this.Copy();
			SerializedProperty result;
			if (serializedProperty.GetArrayElementAtIndexInternal(index))
			{
				result = serializedProperty;
			}
			else
			{
				result = null;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetArrayElementAtIndexInternal(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InsertArrayElementAtIndex(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DeleteArrayElementAtIndex(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool MoveArrayElement(int srcIndex, int dstIndex);

		internal void SetToValueOfTarget(UnityEngine.Object target)
		{
			SerializedProperty serializedProperty = new SerializedObject(target).FindProperty(this.propertyPath);
			if (serializedProperty == null)
			{
				UnityEngine.Debug.LogError(target.name + " does not have the property " + this.propertyPath);
			}
			else
			{
				switch (this.propertyType)
				{
				case SerializedPropertyType.Integer:
					this.intValue = serializedProperty.intValue;
					break;
				case SerializedPropertyType.Boolean:
					this.boolValue = serializedProperty.boolValue;
					break;
				case SerializedPropertyType.Float:
					this.floatValue = serializedProperty.floatValue;
					break;
				case SerializedPropertyType.String:
					this.stringValue = serializedProperty.stringValue;
					break;
				case SerializedPropertyType.Color:
					this.colorValue = serializedProperty.colorValue;
					break;
				case SerializedPropertyType.ObjectReference:
					this.objectReferenceValue = serializedProperty.objectReferenceValue;
					break;
				case SerializedPropertyType.LayerMask:
					this.intValue = serializedProperty.intValue;
					break;
				case SerializedPropertyType.Enum:
					this.enumValueIndex = serializedProperty.enumValueIndex;
					break;
				case SerializedPropertyType.Vector2:
					this.vector2Value = serializedProperty.vector2Value;
					break;
				case SerializedPropertyType.Vector3:
					this.vector3Value = serializedProperty.vector3Value;
					break;
				case SerializedPropertyType.Vector4:
					this.vector4Value = serializedProperty.vector4Value;
					break;
				case SerializedPropertyType.Rect:
					this.rectValue = serializedProperty.rectValue;
					break;
				case SerializedPropertyType.ArraySize:
					this.intValue = serializedProperty.intValue;
					break;
				case SerializedPropertyType.Character:
					this.intValue = serializedProperty.intValue;
					break;
				case SerializedPropertyType.AnimationCurve:
					this.animationCurveValue = serializedProperty.animationCurveValue;
					break;
				case SerializedPropertyType.Bounds:
					this.boundsValue = serializedProperty.boundsValue;
					break;
				case SerializedPropertyType.Gradient:
					this.gradientValue = serializedProperty.gradientValue;
					break;
				}
			}
		}
	}
}
