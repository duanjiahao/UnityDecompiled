using System;

namespace UnityEditor
{
	public struct EditorCurveBinding
	{
		public string path;

		private Type m_type;

		public string propertyName;

		private int m_isPPtrCurve;

		internal int m_ClassID;

		internal int m_ScriptInstanceID;

		public bool isPPtrCurve
		{
			get
			{
				return this.m_isPPtrCurve != 0;
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

		public override int GetHashCode()
		{
			return this.path.GetHashCode() ^ this.type.GetHashCode() << 2 ^ this.propertyName.GetHashCode() << 4;
		}

		public override bool Equals(object other)
		{
			if (!(other is EditorCurveBinding))
			{
				return false;
			}
			EditorCurveBinding rhs = (EditorCurveBinding)other;
			return this == rhs;
		}

		public static EditorCurveBinding FloatCurve(string inPath, Type inType, string inPropertyName)
		{
			return new EditorCurveBinding
			{
				path = inPath,
				type = inType,
				propertyName = inPropertyName,
				m_isPPtrCurve = 0
			};
		}

		public static EditorCurveBinding PPtrCurve(string inPath, Type inType, string inPropertyName)
		{
			return new EditorCurveBinding
			{
				path = inPath,
				type = inType,
				propertyName = inPropertyName,
				m_isPPtrCurve = 1
			};
		}

		public static bool operator ==(EditorCurveBinding lhs, EditorCurveBinding rhs)
		{
			return (lhs.m_ClassID == 0 || rhs.m_ClassID == 0 || (lhs.m_ClassID == rhs.m_ClassID && lhs.m_ScriptInstanceID == rhs.m_ScriptInstanceID)) && (lhs.path == rhs.path && lhs.type == rhs.type && lhs.propertyName == rhs.propertyName) && lhs.m_isPPtrCurve == rhs.m_isPPtrCurve;
		}

		public static bool operator !=(EditorCurveBinding lhs, EditorCurveBinding rhs)
		{
			return !(lhs == rhs);
		}
	}
}
