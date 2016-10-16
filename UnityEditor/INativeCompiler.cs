using System;
using System.Collections.Generic;

internal interface INativeCompiler
{
	void CompileDynamicLibrary(string outFile, IEnumerable<string> sources, IEnumerable<string> includePaths, IEnumerable<string> libraries, IEnumerable<string> libraryPaths);
}
