using System;
using System.Collections.Generic;

internal interface IValidationRule
{
	ValidationResult Validate(IEnumerable<string> userAssemblies, params object[] options);
}
