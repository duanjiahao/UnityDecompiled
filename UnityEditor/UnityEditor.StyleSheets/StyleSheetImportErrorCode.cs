using System;

namespace UnityEditor.StyleSheets
{
	internal enum StyleSheetImportErrorCode
	{
		None,
		Internal,
		UnsupportedFunction,
		UnsupportedParserType,
		UnsupportedUnit,
		InvalidSelectorListDelimiter,
		InvalidComplexSelectorDelimiter,
		UnsupportedSelectorFormat,
		RecursiveSelectorDetected
	}
}
