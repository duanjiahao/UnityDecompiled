using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.Scripting.Compilers;

namespace UnityEditor.Scripting.ScriptCompilation
{
	internal class CompilationTask
	{
		private HashSet<ScriptAssembly> pendingAssemblies;

		private Dictionary<ScriptAssembly, CompilerMessage[]> processedAssemblies = new Dictionary<ScriptAssembly, CompilerMessage[]>();

		private Dictionary<ScriptAssembly, ScriptCompilerBase> compilerTasks = new Dictionary<ScriptAssembly, ScriptCompilerBase>();

		private string buildOutputDirectory;

		private int compilePhase = 0;

		private BuildFlags buildFlags;

		private int maxConcurrentCompilers;

		public event Action<ScriptAssembly, int> OnCompilationStarted
		{
			add
			{
				Action<ScriptAssembly, int> action = this.OnCompilationStarted;
				Action<ScriptAssembly, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<ScriptAssembly, int>>(ref this.OnCompilationStarted, (Action<ScriptAssembly, int>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<ScriptAssembly, int> action = this.OnCompilationStarted;
				Action<ScriptAssembly, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<ScriptAssembly, int>>(ref this.OnCompilationStarted, (Action<ScriptAssembly, int>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public event Action<ScriptAssembly, List<CompilerMessage>> OnCompilationFinished
		{
			add
			{
				Action<ScriptAssembly, List<CompilerMessage>> action = this.OnCompilationFinished;
				Action<ScriptAssembly, List<CompilerMessage>> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<ScriptAssembly, List<CompilerMessage>>>(ref this.OnCompilationFinished, (Action<ScriptAssembly, List<CompilerMessage>>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<ScriptAssembly, List<CompilerMessage>> action = this.OnCompilationFinished;
				Action<ScriptAssembly, List<CompilerMessage>> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<ScriptAssembly, List<CompilerMessage>>>(ref this.OnCompilationFinished, (Action<ScriptAssembly, List<CompilerMessage>>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public bool Stopped
		{
			get;
			private set;
		}

		public bool CompileErrors
		{
			get;
			private set;
		}

		public bool IsCompiling
		{
			get
			{
				return this.pendingAssemblies.Count > 0 || this.compilerTasks.Count > 0;
			}
		}

		public Dictionary<ScriptAssembly, CompilerMessage[]> CompilerMessages
		{
			get
			{
				return this.processedAssemblies;
			}
		}

		public CompilationTask(ScriptAssembly[] scriptAssemblies, string buildOutputDirectory, BuildFlags buildFlags, int maxConcurrentCompilers)
		{
			this.pendingAssemblies = new HashSet<ScriptAssembly>(scriptAssemblies);
			this.CompileErrors = false;
			this.buildOutputDirectory = buildOutputDirectory;
			this.buildFlags = buildFlags;
			this.maxConcurrentCompilers = maxConcurrentCompilers;
		}

		~CompilationTask()
		{
			this.Stop();
		}

		public void Stop()
		{
			if (!this.Stopped)
			{
				foreach (KeyValuePair<ScriptAssembly, ScriptCompilerBase> current in this.compilerTasks)
				{
					ScriptCompilerBase value = current.Value;
					value.Dispose();
				}
				this.compilerTasks.Clear();
				this.Stopped = true;
			}
		}

		public bool Poll()
		{
			bool result;
			if (this.Stopped)
			{
				result = true;
			}
			else
			{
				Dictionary<ScriptAssembly, ScriptCompilerBase> dictionary = null;
				foreach (KeyValuePair<ScriptAssembly, ScriptCompilerBase> current in this.compilerTasks)
				{
					ScriptCompilerBase value = current.Value;
					if (value.Poll())
					{
						if (dictionary == null)
						{
							dictionary = new Dictionary<ScriptAssembly, ScriptCompilerBase>();
						}
						ScriptAssembly key = current.Key;
						dictionary.Add(key, value);
					}
				}
				if (dictionary != null)
				{
					foreach (KeyValuePair<ScriptAssembly, ScriptCompilerBase> current2 in dictionary)
					{
						ScriptAssembly key2 = current2.Key;
						ScriptCompilerBase value2 = current2.Value;
						CompilerMessage[] compilerMessages = value2.GetCompilerMessages();
						List<CompilerMessage> list = compilerMessages.ToList<CompilerMessage>();
						if (this.OnCompilationFinished != null)
						{
							this.OnCompilationFinished(key2, list);
						}
						this.processedAssemblies.Add(key2, list.ToArray());
						if (!this.CompileErrors)
						{
							this.CompileErrors = compilerMessages.Any((CompilerMessage m) => m.type == CompilerMessageType.Error);
						}
						this.compilerTasks.Remove(key2);
						value2.Dispose();
					}
				}
				if (this.CompileErrors)
				{
					if (this.pendingAssemblies.Count > 0)
					{
						foreach (ScriptAssembly current3 in this.pendingAssemblies)
						{
							this.processedAssemblies.Add(current3, new CompilerMessage[0]);
						}
						this.pendingAssemblies.Clear();
					}
					result = (this.compilerTasks.Count == 0);
				}
				else
				{
					if (this.compilerTasks.Count == 0 || (dictionary != null && dictionary.Count > 0))
					{
						this.QueuePendingAssemblies();
					}
					result = (this.pendingAssemblies.Count == 0 && this.compilerTasks.Count == 0);
				}
			}
			return result;
		}

		private void QueuePendingAssemblies()
		{
			if (this.pendingAssemblies.Count != 0)
			{
				List<ScriptAssembly> list = null;
				foreach (ScriptAssembly current in this.pendingAssemblies)
				{
					bool flag = true;
					ScriptAssembly[] scriptAssemblyReferences = current.ScriptAssemblyReferences;
					for (int i = 0; i < scriptAssemblyReferences.Length; i++)
					{
						ScriptAssembly key = scriptAssemblyReferences[i];
						if (!this.processedAssemblies.ContainsKey(key))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						if (list == null)
						{
							list = new List<ScriptAssembly>();
						}
						list.Add(current);
					}
				}
				if (list != null)
				{
					bool buildingForEditor = (this.buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
					foreach (ScriptAssembly current2 in list)
					{
						this.pendingAssemblies.Remove(current2);
						MonoIsland island = current2.ToMonoIsland(this.buildFlags, this.buildOutputDirectory);
						ScriptCompilerBase scriptCompilerBase = ScriptCompilers.CreateCompilerInstance(island, buildingForEditor, island._target, current2.RunUpdater);
						this.compilerTasks.Add(current2, scriptCompilerBase);
						scriptCompilerBase.BeginCompiling();
						if (this.OnCompilationStarted != null)
						{
							this.OnCompilationStarted(current2, this.compilePhase);
						}
						if (this.compilerTasks.Count == this.maxConcurrentCompilers)
						{
							break;
						}
					}
					this.compilePhase++;
				}
			}
		}
	}
}
