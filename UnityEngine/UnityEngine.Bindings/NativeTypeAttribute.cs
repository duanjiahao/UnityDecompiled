using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
	internal class NativeTypeAttribute : Attribute, IBindingsHeaderProviderAttribute, IBindingsGenerateMarshallingTypeAttribute, IBindingsAttribute
	{
		public string Header
		{
			get;
			set;
		}

		public string IntermediateScriptingStructName
		{
			get;
			set;
		}

		public CodegenOptions CodegenOptions
		{
			get;
			set;
		}

		public NativeTypeAttribute()
		{
			this.CodegenOptions = CodegenOptions.Auto;
		}

		public NativeTypeAttribute(CodegenOptions codegenOptions)
		{
			this.CodegenOptions = codegenOptions;
		}

		public NativeTypeAttribute(string header)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			if (header == "")
			{
				throw new ArgumentException("header cannot be empty", "header");
			}
			this.CodegenOptions = CodegenOptions.Auto;
			this.Header = header;
		}

		public NativeTypeAttribute(string header, CodegenOptions codegenOptions) : this(header)
		{
			this.CodegenOptions = codegenOptions;
		}

		public NativeTypeAttribute(CodegenOptions codegenOptions, string intermediateStructName) : this(codegenOptions)
		{
			this.IntermediateScriptingStructName = intermediateStructName;
		}
	}
}
