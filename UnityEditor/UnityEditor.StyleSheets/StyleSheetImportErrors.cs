using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.StyleSheets
{
	internal class StyleSheetImportErrors
	{
		private struct Error
		{
			public readonly StyleSheetImportErrorType error;

			public readonly StyleSheetImportErrorCode code;

			public readonly string context;

			public Error(StyleSheetImportErrorType error, StyleSheetImportErrorCode code, string context)
			{
				this.error = error;
				this.code = code;
				this.context = context;
			}

			public override string ToString()
			{
				return string.Format("[StyleSheetImportError: error={0}, code={1}, context={2}]", this.error, this.code, this.context);
			}
		}

		private List<StyleSheetImportErrors.Error> m_Errors = new List<StyleSheetImportErrors.Error>();

		public bool hasErrors
		{
			get
			{
				return this.m_Errors.Count > 0;
			}
		}

		public void AddSyntaxError(string context)
		{
			this.m_Errors.Add(new StyleSheetImportErrors.Error(StyleSheetImportErrorType.Syntax, StyleSheetImportErrorCode.None, context));
		}

		public void AddSemanticError(StyleSheetImportErrorCode code, string context)
		{
			this.m_Errors.Add(new StyleSheetImportErrors.Error(StyleSheetImportErrorType.Semantic, code, context));
		}

		public void AddInternalError(string context)
		{
			this.m_Errors.Add(new StyleSheetImportErrors.Error(StyleSheetImportErrorType.Internal, StyleSheetImportErrorCode.None, context));
		}

		public IEnumerable<string> FormatErrors()
		{
			return from e in this.m_Errors
			select e.ToString();
		}
	}
}
