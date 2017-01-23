using System;

namespace UnityEditor
{
	public struct EditorCurveBinding
	{
		public string path;

		private Type m_type;

		public string propertyName;

		private int m_isPPtrCurve;

		private int m_isPhantom;

		internal int m_ClassID;

		internal int m_ScriptInstanceID;

		public bool isPPtrCurve
		{
			get
			{
				return this.m_isPPtrCurve != 0;
			}
		}

		internal bool isPhantom
		{
			get
			{
				return this.m_isPhantom != 0;
			}
			set
			{
				this.m_isPhantom = ((!value) ? 0 : 1);
			}
		}

		public Type type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
				this.m_ClassID = 0;
				this.m_ScriptInstanceID = 0;
			}
		}

		public static bool operator ==(EditorCurveBinding lhs, EditorCurveBinding rhs)
		{
			bool result;
			if (lhs.m_ClassID != 0 && rhs.m_ClassID != 0)
			{
				if (lhs.m_ClassID != rhs.m_ClassID || lhs.m_ScriptInstanceID != rhs.m_ScriptInstanceID)
				{
					result = false;
					return result;
				}
			}
			result = (lhs.path == rhs.path && lhs.type == rhs.type && lhs.propertyName == rhs.propertyName && lhs.m_isPPtrCurve == rhs.m_isPPtrCurve);
			return result;
		}

		public static bool operator !=(EditorCurveBinding lhs, EditorCurveBinding rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return string.Concat(new object[]
			{
				this.path,
				':',
				this.type.Name,
				':',
				this.propertyName
			}).GetHashCode();
		}

		public override bool Equals(object other)
		{
			bool result;
			if (!(other is EditorCurveBinding))
			{
				result = false;
			}
			else
			{
				EditorCurveBinding rhs = (EditorCurveBinding)other;
				result = (this == rhs);
			}
			return result;
		}

		public static EditorCurveBinding FloatCurve(string inPath, Type inType, string inPropertyName)
		{
			return new EditorCurveBinding
			{
				path = inPath,
				type = inType,
				propertyName = inPropertyName,
				m_isPPtrCurve = 0,
				m_isPhantom = 0
			};
		}

		public static EditorCurveBinding PPtrCurve(string inPath, Type inType, string inPropertyName)
		{
			return new EditorCurveBinding
			{
				path = inPath,
				type = inType,
				propertyName = inPropertyName,
				m_isPPtrCurve = 1,
				m_isPhantom = 0
			};
		}
	}
}
