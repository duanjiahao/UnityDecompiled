using System;
using System.Collections.Generic;
using UnityEditor.Scripting.Compilers;

internal struct ValidationResult
{
	public bool Success;

	public IValidationRule Rule;

	public IEnumerable<CompilerMessage> CompilerMessages;
}
