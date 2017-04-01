using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using System;

namespace UnityEditor.Scripting
{
	internal class InvalidTypeOrNamespaceErrorTypeMapper : AbstractAstVisitor
	{
		private readonly int _line;

		private readonly int _column;

		public string Found
		{
			get;
			private set;
		}

		private InvalidTypeOrNamespaceErrorTypeMapper(int line, int column)
		{
			this._line = line;
			this._column = column;
		}

		public static string IsTypeMovedToNamespaceError(CompilationUnit cu, int line, int column)
		{
			InvalidTypeOrNamespaceErrorTypeMapper invalidTypeOrNamespaceErrorTypeMapper = new InvalidTypeOrNamespaceErrorTypeMapper(line, column);
			cu.AcceptVisitor(invalidTypeOrNamespaceErrorTypeMapper, null);
			return invalidTypeOrNamespaceErrorTypeMapper.Found;
		}

		public override object VisitTypeReference(TypeReference typeReference, object data)
		{
			bool flag = this._column >= typeReference.StartLocation.Column && this._column < typeReference.StartLocation.Column + typeReference.Type.Length;
			object result;
			if (typeReference.StartLocation.Line == this._line && flag)
			{
				this.Found = typeReference.Type;
				result = true;
			}
			else
			{
				result = base.VisitTypeReference(typeReference, data);
			}
			return result;
		}
	}
}
