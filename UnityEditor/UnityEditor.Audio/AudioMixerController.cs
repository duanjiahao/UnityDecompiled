using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityEditor.Audio
{
	internal sealed class AudioMixerController : AudioMixer
	{
		public class ConnectionNode
		{
			public bool visited = false;

			public object groupTail = null;

			public List<object> targets = new List<object>();

			public AudioMixerGroupController group = null;

			public AudioMixerEffectController effect = null;

			public string GetDisplayString()
			{
				string text = this.group.GetDisplayString();
				if (this.effect != null)
				{
					text = text + AudioMixerController.s_GroupEffectDisplaySeperator + AudioMixerController.FixNameForPopupMenu(this.effect.effectName);
				}
				return text;
			}
		}

		[NonSerialized]
		public int m_HighlightEffectIndex = -1;

		[NonSerialized]
		private List<AudioMixerGroupController> m_CachedSelection = null;

		public static float kMinVolume = -80f;

		public static float kMaxEffect = 0f;

		public static float kVolumeWarp = 1.7f;

		public static string s_GroupEffectDisplaySeperator = "\\";

		[NonSerialized]
		private Dictionary<GUID, AudioParameterPath> m_ExposedParamPathCache;

		public event ChangedExposedParameterHandler ChangedExposedParameter
		{
			add
			{
				ChangedExposedParameterHandler changedExposedParameterHandler = this.ChangedExposedParameter;
				ChangedExposedParameterHandler changedExposedParameterHandler2;
				do
				{
					changedExposedParameterHandler2 = changedExposedParameterHandler;
					changedExposedParameterHandler = Interlocked.CompareExchange<ChangedExposedParameterHandler>(ref this.ChangedExposedParameter, (ChangedExposedParameterHandler)Delegate.Combine(changedExposedParameterHandler2, value), changedExposedParameterHandler);
				}
				while (changedExposedParameterHandler != changedExposedParameterHandler2);
			}
			remove
			{
				ChangedExposedParameterHandler changedExposedParameterHandler = this.ChangedExposedParameter;
				ChangedExposedParameterHandler changedExposedParameterHandler2;
				do
				{
					changedExposedParameterHandler2 = changedExposedParameterHandler;
					changedExposedParameterHandler = Interlocked.CompareExchange<ChangedExposedParameterHandler>(ref this.ChangedExposedParameter, (ChangedExposedParameterHandler)Delegate.Remove(changedExposedParameterHandler2, value), changedExposedParameterHandler);
				}
				while (changedExposedParameterHandler != changedExposedParameterHandler2);
			}
		}

		public AudioMixerGroupController[] allGroups
		{
			get
			{
				List<AudioMixerGroupController> list = new List<AudioMixerGroupController>();
				AudioMixerController.GetGroupsRecurse(this.masterGroup, list);
				return list.ToArray();
			}
		}

		public extern int numExposedParameters
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ExposedAudioParameter[] exposedParameters
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AudioMixerGroupController masterGroup
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AudioMixerSnapshot startSnapshot
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AudioMixerSnapshotController TargetSnapshot
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AudioMixerSnapshotController[] snapshots
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public List<AudioMixerGroupController> CachedSelection
		{
			get
			{
				if (this.m_CachedSelection == null)
				{
					this.m_CachedSelection = new List<AudioMixerGroupController>();
				}
				return this.m_CachedSelection;
			}
		}

		public extern int currentViewIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern MixerGroupView[] views
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isSuspended
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private Dictionary<GUID, AudioParameterPath> exposedParamCache
		{
			get
			{
				if (this.m_ExposedParamPathCache == null)
				{
					this.m_ExposedParamPathCache = new Dictionary<GUID, AudioParameterPath>();
				}
				return this.m_ExposedParamPathCache;
			}
		}

		public AudioMixerController()
		{
			AudioMixerController.Internal_CreateAudioMixerController(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateAudioMixerController(AudioMixerController mono);

		private static void GetGroupsRecurse(AudioMixerGroupController group, List<AudioMixerGroupController> groups)
		{
			groups.Add(group);
			AudioMixerGroupController[] children = group.children;
			for (int i = 0; i < children.Length; i++)
			{
				AudioMixerController.GetGroupsRecurse(children[i], groups);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetGroupVUInfo(GUID group, bool fader, ref float[] vuLevel, ref float[] vuPeak);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateMuteSolo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateBypass();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool CurrentViewContainsGroup(GUID group);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CheckForCyclicReferences(AudioMixer mixer, AudioMixerGroup group);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetMaxVolume();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetVolumeSplitPoint();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool EditingTargetSnapshot();

		public void OnChangedExposedParameter()
		{
			if (this.ChangedExposedParameter != null)
			{
				this.ChangedExposedParameter();
			}
		}

		public void ClearEventHandlers()
		{
			if (this.ChangedExposedParameter != null)
			{
				Delegate[] invocationList = this.ChangedExposedParameter.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Delegate @delegate = invocationList[i];
					this.ChangedExposedParameter -= (ChangedExposedParameterHandler)@delegate;
				}
			}
		}

		private string FindUniqueParameterName(string template, ExposedAudioParameter[] parameters)
		{
			string text = template;
			int num = 1;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (text == parameters[i].name)
				{
					text = template + " " + num++;
					i = -1;
				}
			}
			return text;
		}

		private int SortFuncForExposedParameters(ExposedAudioParameter p1, ExposedAudioParameter p2)
		{
			return string.CompareOrdinal(this.ResolveExposedParameterPath(p1.guid, true), this.ResolveExposedParameterPath(p2.guid, true));
		}

		public void AddExposedParameter(AudioParameterPath path)
		{
			if (!this.ContainsExposedParameter(path.parameter))
			{
				List<ExposedAudioParameter> list = new List<ExposedAudioParameter>(this.exposedParameters);
				list.Add(new ExposedAudioParameter
				{
					name = this.FindUniqueParameterName("MyExposedParam", this.exposedParameters),
					guid = path.parameter
				});
				list.Sort(new Comparison<ExposedAudioParameter>(this.SortFuncForExposedParameters));
				this.exposedParameters = list.ToArray();
				this.OnChangedExposedParameter();
				this.exposedParamCache[path.parameter] = path;
				AudioMixerUtility.RepaintAudioMixerAndInspectors();
			}
			else
			{
				Debug.LogError("Cannot expose the same parameter more than once!");
			}
		}

		public bool ContainsExposedParameter(GUID parameter)
		{
			return (from val in this.exposedParameters
			where val.guid == parameter
			select val).ToArray<ExposedAudioParameter>().Length > 0;
		}

		public void RemoveExposedParameter(GUID parameter)
		{
			this.exposedParameters = (from val in this.exposedParameters
			where val.guid != parameter
			select val).ToArray<ExposedAudioParameter>();
			this.OnChangedExposedParameter();
			if (this.exposedParamCache.ContainsKey(parameter))
			{
				this.exposedParamCache.Remove(parameter);
			}
			AudioMixerUtility.RepaintAudioMixerAndInspectors();
		}

		public string ResolveExposedParameterPath(GUID parameter, bool getOnlyBasePath)
		{
			string result;
			if (this.exposedParamCache.ContainsKey(parameter))
			{
				AudioParameterPath audioParameterPath = this.exposedParamCache[parameter];
				result = audioParameterPath.ResolveStringPath(getOnlyBasePath);
			}
			else
			{
				List<AudioMixerGroupController> allAudioGroupsSlow = this.GetAllAudioGroupsSlow();
				foreach (AudioMixerGroupController current in allAudioGroupsSlow)
				{
					if (current.GetGUIDForVolume() == parameter || current.GetGUIDForPitch() == parameter)
					{
						AudioGroupParameterPath audioGroupParameterPath = new AudioGroupParameterPath(current, parameter);
						this.exposedParamCache[parameter] = audioGroupParameterPath;
						result = audioGroupParameterPath.ResolveStringPath(getOnlyBasePath);
						return result;
					}
					for (int i = 0; i < current.effects.Length; i++)
					{
						AudioMixerEffectController audioMixerEffectController = current.effects[i];
						MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(audioMixerEffectController.effectName);
						for (int j = 0; j < effectParameters.Length; j++)
						{
							GUID gUIDForParameter = audioMixerEffectController.GetGUIDForParameter(effectParameters[j].name);
							if (gUIDForParameter == parameter)
							{
								AudioEffectParameterPath audioEffectParameterPath = new AudioEffectParameterPath(current, audioMixerEffectController, parameter);
								this.exposedParamCache[parameter] = audioEffectParameterPath;
								result = audioEffectParameterPath.ResolveStringPath(getOnlyBasePath);
								return result;
							}
						}
					}
				}
				result = "Error finding Parameter path";
			}
			return result;
		}

		public static AudioMixerController CreateMixerControllerAtPath(string path)
		{
			AudioMixerController audioMixerController = new AudioMixerController();
			audioMixerController.CreateDefaultAsset(path);
			return audioMixerController;
		}

		public void CreateDefaultAsset(string path)
		{
			this.masterGroup = new AudioMixerGroupController(this);
			this.masterGroup.name = "Master";
			this.masterGroup.PreallocateGUIDs();
			AudioMixerEffectController audioMixerEffectController = new AudioMixerEffectController("Attenuation");
			audioMixerEffectController.PreallocateGUIDs();
			this.masterGroup.InsertEffect(audioMixerEffectController, 0);
			AudioMixerSnapshotController audioMixerSnapshotController = new AudioMixerSnapshotController(this);
			audioMixerSnapshotController.name = "Snapshot";
			this.snapshots = new AudioMixerSnapshotController[]
			{
				audioMixerSnapshotController
			};
			this.startSnapshot = audioMixerSnapshotController;
			UnityEngine.Object[] assets = new UnityEngine.Object[]
			{
				this,
				this.masterGroup,
				audioMixerEffectController,
				audioMixerSnapshotController
			};
			AssetDatabase.CreateAssetFromObjects(assets, path);
		}

		private void BuildTestSetup(System.Random r, AudioMixerGroupController parent, int minSpan, int maxSpan, int maxGroups, string prefix, ref int numGroups)
		{
			int num = (numGroups != 0) ? r.Next(minSpan, maxSpan + 1) : maxSpan;
			for (int i = 0; i < num; i++)
			{
				string text = prefix + i;
				AudioMixerGroupController audioMixerGroupController = this.CreateNewGroup(text, false);
				this.AddChildToParent(audioMixerGroupController, parent);
				if (++numGroups >= maxGroups)
				{
					break;
				}
				this.BuildTestSetup(r, audioMixerGroupController, minSpan, (maxSpan <= minSpan) ? minSpan : (maxSpan - 1), maxGroups, text, ref numGroups);
			}
		}

		public void BuildTestSetup(int minSpan, int maxSpan, int maxGroups)
		{
			int num = 0;
			this.DeleteGroups(this.masterGroup.children);
			this.BuildTestSetup(new System.Random(), this.masterGroup, minSpan, maxSpan, maxGroups, "G", ref num);
		}

		public List<AudioMixerGroupController> GetAllAudioGroupsSlow()
		{
			List<AudioMixerGroupController> result = new List<AudioMixerGroupController>();
			if (this.masterGroup != null)
			{
				this.GetAllAudioGroupsSlowRecurse(this.masterGroup, ref result);
			}
			return result;
		}

		private void GetAllAudioGroupsSlowRecurse(AudioMixerGroupController g, ref List<AudioMixerGroupController> groups)
		{
			groups.Add(g);
			AudioMixerGroupController[] children = g.children;
			for (int i = 0; i < children.Length; i++)
			{
				AudioMixerGroupController g2 = children[i];
				this.GetAllAudioGroupsSlowRecurse(g2, ref groups);
			}
		}

		public bool HasMoreThanOneGroup()
		{
			return this.masterGroup.children.Length > 0;
		}

		private bool IsChildOf(AudioMixerGroupController child, List<AudioMixerGroupController> groups)
		{
			bool result;
			while (child != null)
			{
				child = this.FindParentGroup(this.masterGroup, child);
				if (groups.Contains(child))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public bool AreAnyOfTheGroupsInTheListAncestors(List<AudioMixerGroupController> groups)
		{
			return groups.Any((AudioMixerGroupController g) => this.IsChildOf(g, groups));
		}

		private void RemoveAncestorGroups(List<AudioMixerGroupController> groups)
		{
			groups.RemoveAll((AudioMixerGroupController g) => this.IsChildOf(g, groups));
			object.Equals(this.AreAnyOfTheGroupsInTheListAncestors(groups), false);
		}

		private void DestroyExposedParametersContainedInEffect(AudioMixerEffectController effect)
		{
			Undo.RecordObject(this, "Changed Exposed Parameters");
			ExposedAudioParameter[] exposedParameters = this.exposedParameters;
			ExposedAudioParameter[] array = exposedParameters;
			for (int i = 0; i < array.Length; i++)
			{
				ExposedAudioParameter exposedAudioParameter = array[i];
				if (effect.ContainsParameterGUID(exposedAudioParameter.guid))
				{
					this.RemoveExposedParameter(exposedAudioParameter.guid);
				}
			}
		}

		private void DestroyExposedParametersContainedInGroup(AudioMixerGroupController group)
		{
			Undo.RecordObject(this, "Remove Exposed Parameter");
			ExposedAudioParameter[] exposedParameters = this.exposedParameters;
			ExposedAudioParameter[] array = exposedParameters;
			for (int i = 0; i < array.Length; i++)
			{
				ExposedAudioParameter exposedAudioParameter = array[i];
				if (group.GetGUIDForVolume() == exposedAudioParameter.guid || group.GetGUIDForPitch() == exposedAudioParameter.guid)
				{
					this.RemoveExposedParameter(exposedAudioParameter.guid);
				}
			}
		}

		private void DeleteSubGroupRecursive(AudioMixerGroupController group)
		{
			AudioMixerGroupController[] children = group.children;
			for (int i = 0; i < children.Length; i++)
			{
				AudioMixerGroupController group2 = children[i];
				this.DeleteSubGroupRecursive(group2);
			}
			AudioMixerEffectController[] effects = group.effects;
			for (int j = 0; j < effects.Length; j++)
			{
				AudioMixerEffectController audioMixerEffectController = effects[j];
				this.DestroyExposedParametersContainedInEffect(audioMixerEffectController);
				Undo.DestroyObjectImmediate(audioMixerEffectController);
			}
			this.DestroyExposedParametersContainedInGroup(group);
			Undo.DestroyObjectImmediate(group);
		}

		private void DeleteGroupsInternal(List<AudioMixerGroupController> groupsToDelete, List<AudioMixerGroupController> allGroups)
		{
			foreach (AudioMixerGroupController current in allGroups)
			{
				IEnumerable<AudioMixerGroupController> enumerable = groupsToDelete.Intersect(current.children);
				if (enumerable.Count<AudioMixerGroupController>() > 0)
				{
					Undo.RegisterCompleteObjectUndo(current, "Delete Group(s)");
					current.children = current.children.Except(enumerable).ToArray<AudioMixerGroupController>();
				}
			}
			foreach (AudioMixerGroupController current2 in groupsToDelete)
			{
				this.DeleteSubGroupRecursive(current2);
			}
		}

		public void DeleteGroups(AudioMixerGroupController[] groups)
		{
			List<AudioMixerGroupController> list = groups.ToList<AudioMixerGroupController>();
			this.RemoveAncestorGroups(list);
			this.DeleteGroupsInternal(list, this.GetAllAudioGroupsSlow());
			this.OnUnitySelectionChanged();
		}

		public void RemoveEffect(AudioMixerEffectController effect, AudioMixerGroupController group)
		{
			Undo.RecordObject(group, "Delete Effect");
			List<AudioMixerEffectController> list = new List<AudioMixerEffectController>(group.effects);
			list.Remove(effect);
			group.effects = list.ToArray();
			this.DestroyExposedParametersContainedInEffect(effect);
			Undo.DestroyObjectImmediate(effect);
		}

		public void OnSubAssetChanged()
		{
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
		}

		public void CloneNewSnapshotFromTarget(bool storeUndoState)
		{
			List<AudioMixerSnapshotController> list = new List<AudioMixerSnapshotController>(this.snapshots);
			AudioMixerSnapshotController audioMixerSnapshotController = UnityEngine.Object.Instantiate<AudioMixerSnapshotController>(this.TargetSnapshot);
			AudioMixerSnapshotController audioMixerSnapshotController2 = audioMixerSnapshotController;
			audioMixerSnapshotController2.name = this.TargetSnapshot.name + " - Copy";
			list.Add(audioMixerSnapshotController2);
			this.snapshots = list.ToArray();
			this.TargetSnapshot = list[list.Count - 1];
			AssetDatabase.AddObjectToAsset(audioMixerSnapshotController2, this);
			if (storeUndoState)
			{
				Undo.RegisterCreatedObjectUndo(audioMixerSnapshotController2, "");
			}
			this.OnSubAssetChanged();
		}

		public void RemoveTargetSnapshot()
		{
			if (this.snapshots.Length >= 2)
			{
				AudioMixerSnapshotController targetSnapshot = this.TargetSnapshot;
				Undo.RecordObject(this, "Remove Snapshot");
				List<AudioMixerSnapshotController> list = new List<AudioMixerSnapshotController>(this.snapshots);
				list.Remove(targetSnapshot);
				this.snapshots = list.ToArray();
				Undo.DestroyObjectImmediate(targetSnapshot);
				this.OnSubAssetChanged();
			}
		}

		public void RemoveSnapshot(AudioMixerSnapshotController snapshot)
		{
			if (this.snapshots.Length >= 2)
			{
				Undo.RecordObject(this, "Remove Snapshot");
				List<AudioMixerSnapshotController> list = new List<AudioMixerSnapshotController>(this.snapshots);
				list.Remove(snapshot);
				this.snapshots = list.ToArray();
				Undo.DestroyObjectImmediate(snapshot);
				this.OnSubAssetChanged();
			}
		}

		public AudioMixerGroupController CreateNewGroup(string name, bool storeUndoState)
		{
			AudioMixerGroupController audioMixerGroupController = new AudioMixerGroupController(this);
			audioMixerGroupController.name = name;
			audioMixerGroupController.PreallocateGUIDs();
			AudioMixerEffectController audioMixerEffectController = new AudioMixerEffectController("Attenuation");
			this.AddNewSubAsset(audioMixerEffectController, storeUndoState);
			audioMixerEffectController.PreallocateGUIDs();
			audioMixerGroupController.InsertEffect(audioMixerEffectController, 0);
			this.AddNewSubAsset(audioMixerGroupController, storeUndoState);
			return audioMixerGroupController;
		}

		public void AddChildToParent(AudioMixerGroupController child, AudioMixerGroupController parent)
		{
			this.RemoveGroupsFromParent(new AudioMixerGroupController[]
			{
				child
			}, false);
			parent.children = new List<AudioMixerGroupController>(parent.children)
			{
				child
			}.ToArray();
		}

		private void AddNewSubAsset(UnityEngine.Object obj, bool storeUndoState)
		{
			AssetDatabase.AddObjectToAsset(obj, this);
			if (storeUndoState)
			{
				Undo.RegisterCreatedObjectUndo(obj, "");
			}
		}

		public void RemoveGroupsFromParent(AudioMixerGroupController[] groups, bool storeUndoState)
		{
			List<AudioMixerGroupController> list = groups.ToList<AudioMixerGroupController>();
			this.RemoveAncestorGroups(list);
			if (storeUndoState)
			{
				Undo.RecordObject(this, "Remove group");
			}
			foreach (AudioMixerGroupController current in list)
			{
				List<AudioMixerGroupController> allAudioGroupsSlow = this.GetAllAudioGroupsSlow();
				foreach (AudioMixerGroupController current2 in allAudioGroupsSlow)
				{
					List<AudioMixerGroupController> list2 = new List<AudioMixerGroupController>(current2.children);
					if (list2.Contains(current))
					{
						list2.Remove(current);
					}
					if (current2.children.Length != list2.Count)
					{
						current2.children = list2.ToArray();
					}
				}
			}
		}

		public AudioMixerGroupController FindParentGroup(AudioMixerGroupController node, AudioMixerGroupController group)
		{
			int i = 0;
			AudioMixerGroupController result;
			while (i < node.children.Length)
			{
				if (node.children[i] == group)
				{
					result = node;
				}
				else
				{
					AudioMixerGroupController audioMixerGroupController = this.FindParentGroup(node.children[i], group);
					if (!(audioMixerGroupController != null))
					{
						i++;
						continue;
					}
					result = audioMixerGroupController;
				}
				return result;
			}
			result = null;
			return result;
		}

		public AudioMixerEffectController CopyEffect(AudioMixerEffectController sourceEffect)
		{
			AudioMixerEffectController audioMixerEffectController = new AudioMixerEffectController(sourceEffect.effectName);
			audioMixerEffectController.name = sourceEffect.name;
			audioMixerEffectController.PreallocateGUIDs();
			MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(sourceEffect.effectName);
			AudioMixerSnapshotController[] snapshots = this.snapshots;
			for (int i = 0; i < snapshots.Length; i++)
			{
				AudioMixerSnapshotController audioMixerSnapshotController = snapshots[i];
				float value;
				if (audioMixerSnapshotController.GetValue(sourceEffect.GetGUIDForMixLevel(), out value))
				{
					audioMixerSnapshotController.SetValue(audioMixerEffectController.GetGUIDForMixLevel(), value);
				}
				MixerParameterDefinition[] array = effectParameters;
				for (int j = 0; j < array.Length; j++)
				{
					MixerParameterDefinition mixerParameterDefinition = array[j];
					if (audioMixerSnapshotController.GetValue(sourceEffect.GetGUIDForParameter(mixerParameterDefinition.name), out value))
					{
						audioMixerSnapshotController.SetValue(audioMixerEffectController.GetGUIDForParameter(mixerParameterDefinition.name), value);
					}
				}
			}
			AssetDatabase.AddObjectToAsset(audioMixerEffectController, this);
			return audioMixerEffectController;
		}

		private AudioMixerGroupController DuplicateGroupRecurse(AudioMixerGroupController sourceGroup)
		{
			AudioMixerGroupController audioMixerGroupController = new AudioMixerGroupController(this);
			List<AudioMixerEffectController> list = new List<AudioMixerEffectController>();
			AudioMixerEffectController[] effects = sourceGroup.effects;
			for (int i = 0; i < effects.Length; i++)
			{
				AudioMixerEffectController sourceEffect = effects[i];
				list.Add(this.CopyEffect(sourceEffect));
			}
			List<AudioMixerGroupController> list2 = new List<AudioMixerGroupController>();
			AudioMixerGroupController[] children = sourceGroup.children;
			for (int j = 0; j < children.Length; j++)
			{
				AudioMixerGroupController sourceGroup2 = children[j];
				list2.Add(this.DuplicateGroupRecurse(sourceGroup2));
			}
			audioMixerGroupController.name = sourceGroup.name + " - Copy";
			audioMixerGroupController.PreallocateGUIDs();
			audioMixerGroupController.effects = list.ToArray();
			audioMixerGroupController.children = list2.ToArray();
			audioMixerGroupController.solo = sourceGroup.solo;
			audioMixerGroupController.mute = sourceGroup.mute;
			audioMixerGroupController.bypassEffects = sourceGroup.bypassEffects;
			AudioMixerSnapshotController[] snapshots = this.snapshots;
			for (int k = 0; k < snapshots.Length; k++)
			{
				AudioMixerSnapshotController audioMixerSnapshotController = snapshots[k];
				float value;
				if (audioMixerSnapshotController.GetValue(sourceGroup.GetGUIDForVolume(), out value))
				{
					audioMixerSnapshotController.SetValue(audioMixerGroupController.GetGUIDForVolume(), value);
				}
				if (audioMixerSnapshotController.GetValue(sourceGroup.GetGUIDForPitch(), out value))
				{
					audioMixerSnapshotController.SetValue(audioMixerGroupController.GetGUIDForPitch(), value);
				}
			}
			AssetDatabase.AddObjectToAsset(audioMixerGroupController, this);
			if (this.CurrentViewContainsGroup(sourceGroup.groupID))
			{
				audioMixerGroupController.controller.AddGroupToCurrentView(audioMixerGroupController);
			}
			return audioMixerGroupController;
		}

		public List<AudioMixerGroupController> DuplicateGroups(AudioMixerGroupController[] sourceGroups)
		{
			List<AudioMixerGroupController> list = sourceGroups.ToList<AudioMixerGroupController>();
			this.RemoveAncestorGroups(list);
			List<AudioMixerGroupController> list2 = new List<AudioMixerGroupController>();
			foreach (AudioMixerGroupController current in list)
			{
				AudioMixerGroupController audioMixerGroupController = this.FindParentGroup(this.masterGroup, current);
				if (audioMixerGroupController != null && current != null)
				{
					AudioMixerGroupController item = this.DuplicateGroupRecurse(current);
					audioMixerGroupController.children = new List<AudioMixerGroupController>(audioMixerGroupController.children)
					{
						item
					}.ToArray();
					list2.Add(item);
				}
			}
			return list2;
		}

		public void CopyEffectSettingsToAllSnapshots(AudioMixerGroupController group, int effectIndex, AudioMixerSnapshotController snapshot, bool includeWetParam)
		{
			AudioMixerSnapshotController[] snapshots = this.snapshots;
			for (int i = 0; i < snapshots.Length; i++)
			{
				if (!(snapshots[i] == snapshot))
				{
					AudioMixerEffectController audioMixerEffectController = group.effects[effectIndex];
					MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(audioMixerEffectController.effectName);
					if (includeWetParam)
					{
						GUID gUIDForMixLevel = audioMixerEffectController.GetGUIDForMixLevel();
						float value;
						if (snapshot.GetValue(gUIDForMixLevel, out value))
						{
							snapshots[i].SetValue(gUIDForMixLevel, value);
						}
					}
					MixerParameterDefinition[] array = effectParameters;
					for (int j = 0; j < array.Length; j++)
					{
						MixerParameterDefinition mixerParameterDefinition = array[j];
						GUID gUIDForParameter = audioMixerEffectController.GetGUIDForParameter(mixerParameterDefinition.name);
						float value;
						if (snapshot.GetValue(gUIDForParameter, out value))
						{
							snapshots[i].SetValue(gUIDForParameter, value);
						}
					}
				}
			}
		}

		public void CopyAllSettingsToAllSnapshots(AudioMixerGroupController group, AudioMixerSnapshotController snapshot)
		{
			for (int i = 0; i < group.effects.Length; i++)
			{
				this.CopyEffectSettingsToAllSnapshots(group, i, snapshot, true);
			}
			AudioMixerSnapshotController[] snapshots = this.snapshots;
			for (int j = 0; j < snapshots.Length; j++)
			{
				if (!(snapshots[j] == snapshot))
				{
					AudioMixerSnapshotController snapshot2 = snapshots[j];
					group.SetValueForVolume(this, snapshot2, group.GetValueForVolume(this, snapshot));
					group.SetValueForPitch(this, snapshot2, group.GetValueForPitch(this, snapshot));
				}
			}
		}

		public void CopyAttenuationToAllSnapshots(AudioMixerGroupController group, AudioMixerSnapshotController snapshot)
		{
			AudioMixerSnapshotController[] snapshots = this.snapshots;
			for (int i = 0; i < snapshots.Length; i++)
			{
				if (!(snapshots[i] == snapshot))
				{
					AudioMixerSnapshotController snapshot2 = snapshots[i];
					group.SetValueForVolume(this, snapshot2, group.GetValueForVolume(this, snapshot));
				}
			}
		}

		public void ReparentSelection(AudioMixerGroupController newParent, int insertionIndex, List<AudioMixerGroupController> selection)
		{
			if (insertionIndex >= 0)
			{
				insertionIndex -= newParent.children.ToList<AudioMixerGroupController>().GetRange(0, insertionIndex).Count(new Func<AudioMixerGroupController, bool>(selection.Contains));
			}
			Undo.RecordObject(newParent, "Change Audio Mixer Group Parent");
			List<AudioMixerGroupController> allAudioGroupsSlow = this.GetAllAudioGroupsSlow();
			foreach (AudioMixerGroupController current in allAudioGroupsSlow)
			{
				if (current.children.Intersect(selection).Any<AudioMixerGroupController>())
				{
					Undo.RecordObject(current, string.Empty);
					List<AudioMixerGroupController> list = new List<AudioMixerGroupController>(current.children);
					foreach (AudioMixerGroupController current2 in selection)
					{
						list.Remove(current2);
					}
					current.children = list.ToArray();
				}
			}
			if (insertionIndex == -1)
			{
				insertionIndex = 0;
			}
			List<AudioMixerGroupController> list2 = new List<AudioMixerGroupController>(newParent.children);
			list2.InsertRange(insertionIndex, selection);
			newParent.children = list2.ToArray();
		}

		public static bool InsertEffect(AudioMixerEffectController effect, ref List<AudioMixerEffectController> targetEffects, int targetIndex)
		{
			bool result;
			if (targetIndex < 0 || targetIndex > targetEffects.Count)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Inserting effect failed! size: ",
					targetEffects.Count,
					" at index: ",
					targetIndex
				}));
				result = false;
			}
			else
			{
				targetEffects.Insert(targetIndex, effect);
				result = true;
			}
			return result;
		}

		public static bool MoveEffect(ref List<AudioMixerEffectController> sourceEffects, int sourceIndex, ref List<AudioMixerEffectController> targetEffects, int targetIndex)
		{
			bool result;
			if (sourceEffects == targetEffects)
			{
				if (targetIndex > sourceIndex)
				{
					targetIndex--;
				}
				if (sourceIndex == targetIndex)
				{
					result = false;
					return result;
				}
			}
			if (sourceIndex < 0 || sourceIndex >= sourceEffects.Count)
			{
				result = false;
			}
			else if (targetIndex < 0 || targetIndex > targetEffects.Count)
			{
				result = false;
			}
			else
			{
				AudioMixerEffectController item = sourceEffects[sourceIndex];
				sourceEffects.RemoveAt(sourceIndex);
				targetEffects.Insert(targetIndex, item);
				result = true;
			}
			return result;
		}

		public static string FixNameForPopupMenu(string s)
		{
			return s;
		}

		public void ClearSendConnectionsTo(AudioMixerEffectController sendTarget)
		{
			List<AudioMixerGroupController> allAudioGroupsSlow = this.GetAllAudioGroupsSlow();
			foreach (AudioMixerGroupController current in allAudioGroupsSlow)
			{
				AudioMixerEffectController[] effects = current.effects;
				for (int i = 0; i < effects.Length; i++)
				{
					AudioMixerEffectController audioMixerEffectController = effects[i];
					if (audioMixerEffectController.IsSend() && audioMixerEffectController.sendTarget == sendTarget)
					{
						Undo.RecordObject(audioMixerEffectController, "Clear Send target");
						audioMixerEffectController.sendTarget = null;
					}
				}
			}
		}

		private static Dictionary<object, AudioMixerController.ConnectionNode> BuildTemporaryGraph(List<AudioMixerGroupController> allGroups, AudioMixerGroupController groupWhoseEffectIsChanged, AudioMixerEffectController effectWhoseTargetIsChanged, AudioMixerEffectController targetToTest, AudioMixerGroupController modifiedGroup1, List<AudioMixerEffectController> modifiedGroupEffects1, AudioMixerGroupController modifiedGroup2, List<AudioMixerEffectController> modifiedGroupEffects2)
		{
			Dictionary<object, AudioMixerController.ConnectionNode> dictionary = new Dictionary<object, AudioMixerController.ConnectionNode>();
			foreach (AudioMixerGroupController current in allGroups)
			{
				dictionary[current] = new AudioMixerController.ConnectionNode
				{
					group = current,
					effect = null
				};
				object obj = current;
				List<AudioMixerEffectController> list = (!(current == modifiedGroup1)) ? ((!(current == modifiedGroup2)) ? current.effects.ToList<AudioMixerEffectController>() : modifiedGroupEffects2) : modifiedGroupEffects1;
				foreach (AudioMixerEffectController current2 in list)
				{
					if (!dictionary.ContainsKey(current2))
					{
						dictionary[current2] = new AudioMixerController.ConnectionNode();
					}
					dictionary[current2].group = current;
					dictionary[current2].effect = current2;
					if (!dictionary[obj].targets.Contains(current2))
					{
						dictionary[obj].targets.Add(current2);
					}
					AudioMixerEffectController audioMixerEffectController = (!(current == groupWhoseEffectIsChanged) || !(effectWhoseTargetIsChanged == current2)) ? current2.sendTarget : targetToTest;
					if (audioMixerEffectController != null)
					{
						if (!dictionary.ContainsKey(audioMixerEffectController))
						{
							dictionary[audioMixerEffectController] = new AudioMixerController.ConnectionNode();
							dictionary[audioMixerEffectController].group = current;
							dictionary[audioMixerEffectController].effect = audioMixerEffectController;
						}
						if (!dictionary[current2].targets.Contains(audioMixerEffectController))
						{
							dictionary[current2].targets.Add(audioMixerEffectController);
						}
					}
					obj = current2;
				}
				dictionary[current].groupTail = obj;
			}
			return dictionary;
		}

		private static void ListTemporaryGraph(Dictionary<object, AudioMixerController.ConnectionNode> graph)
		{
			Debug.Log("Listing temporary graph:");
			int num = 0;
			foreach (KeyValuePair<object, AudioMixerController.ConnectionNode> current in graph)
			{
				Debug.Log(string.Format("Node {0}: {1}", num++, current.Value.GetDisplayString()));
				int num2 = 0;
				foreach (object current2 in current.Value.targets)
				{
					Debug.Log(string.Format("  Target {0}: {1}", num2++, graph[current2].GetDisplayString()));
				}
			}
		}

		private static bool CheckForCycle(object curr, Dictionary<object, AudioMixerController.ConnectionNode> graph, List<AudioMixerController.ConnectionNode> identifiedLoop)
		{
			AudioMixerController.ConnectionNode connectionNode = graph[curr];
			bool result;
			if (connectionNode.visited)
			{
				if (identifiedLoop != null)
				{
					identifiedLoop.Clear();
					identifiedLoop.Add(connectionNode);
				}
				result = true;
			}
			else
			{
				connectionNode.visited = true;
				foreach (object current in connectionNode.targets)
				{
					if (AudioMixerController.CheckForCycle(current, graph, identifiedLoop))
					{
						connectionNode.visited = false;
						if (identifiedLoop != null)
						{
							identifiedLoop.Add(connectionNode);
						}
						result = true;
						return result;
					}
				}
				connectionNode.visited = false;
				result = false;
			}
			return result;
		}

		public static bool DoesTheTemporaryGraphHaveAnyCycles(List<AudioMixerGroupController> allGroups, List<AudioMixerController.ConnectionNode> identifiedLoop, Dictionary<object, AudioMixerController.ConnectionNode> graph)
		{
			bool result;
			foreach (AudioMixerGroupController current in allGroups)
			{
				if (AudioMixerController.CheckForCycle(current, graph, identifiedLoop))
				{
					if (identifiedLoop != null)
					{
						AudioMixerController.ConnectionNode connectionNode = identifiedLoop[0];
						int i = 1;
						while (i < identifiedLoop.Count)
						{
							if (identifiedLoop[i++] == connectionNode)
							{
								break;
							}
						}
						identifiedLoop.RemoveRange(i, identifiedLoop.Count - i);
						identifiedLoop.Reverse();
					}
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public static bool WillChangeOfEffectTargetCauseFeedback(List<AudioMixerGroupController> allGroups, AudioMixerGroupController groupWhoseEffectIsChanged, int effectWhoseTargetIsChanged, AudioMixerEffectController targetToTest, List<AudioMixerController.ConnectionNode> identifiedLoop)
		{
			Dictionary<object, AudioMixerController.ConnectionNode> dictionary = AudioMixerController.BuildTemporaryGraph(allGroups, groupWhoseEffectIsChanged, groupWhoseEffectIsChanged.effects[effectWhoseTargetIsChanged], targetToTest, null, null, null, null);
			foreach (AudioMixerGroupController current in allGroups)
			{
				AudioMixerGroupController[] children = current.children;
				for (int i = 0; i < children.Length; i++)
				{
					AudioMixerGroupController key = children[i];
					object groupTail = dictionary[key].groupTail;
					if (!dictionary[groupTail].targets.Contains(current))
					{
						dictionary[groupTail].targets.Add(current);
					}
				}
			}
			return AudioMixerController.DoesTheTemporaryGraphHaveAnyCycles(allGroups, identifiedLoop, dictionary);
		}

		public static bool WillModificationOfTopologyCauseFeedback(List<AudioMixerGroupController> allGroups, List<AudioMixerGroupController> groupsToBeMoved, AudioMixerGroupController newParentForMovedGroups, List<AudioMixerController.ConnectionNode> identifiedLoop)
		{
			Dictionary<object, AudioMixerController.ConnectionNode> dictionary = AudioMixerController.BuildTemporaryGraph(allGroups, null, null, null, null, null, null, null);
			foreach (AudioMixerGroupController current in allGroups)
			{
				AudioMixerGroupController[] children = current.children;
				for (int i = 0; i < children.Length; i++)
				{
					AudioMixerGroupController audioMixerGroupController = children[i];
					AudioMixerGroupController item = (!groupsToBeMoved.Contains(audioMixerGroupController)) ? current : newParentForMovedGroups;
					object groupTail = dictionary[audioMixerGroupController].groupTail;
					if (!dictionary[groupTail].targets.Contains(item))
					{
						dictionary[groupTail].targets.Add(item);
					}
				}
			}
			return AudioMixerController.DoesTheTemporaryGraphHaveAnyCycles(allGroups, identifiedLoop, dictionary);
		}

		public static bool WillMovingEffectCauseFeedback(List<AudioMixerGroupController> allGroups, AudioMixerGroupController sourceGroup, int sourceIndex, AudioMixerGroupController targetGroup, int targetIndex, List<AudioMixerController.ConnectionNode> identifiedLoop)
		{
			bool result;
			Dictionary<object, AudioMixerController.ConnectionNode> dictionary;
			if (sourceGroup == targetGroup)
			{
				List<AudioMixerEffectController> modifiedGroupEffects = sourceGroup.effects.ToList<AudioMixerEffectController>();
				if (!AudioMixerController.MoveEffect(ref modifiedGroupEffects, sourceIndex, ref modifiedGroupEffects, targetIndex))
				{
					result = false;
					return result;
				}
				dictionary = AudioMixerController.BuildTemporaryGraph(allGroups, null, null, null, sourceGroup, modifiedGroupEffects, null, null);
			}
			else
			{
				List<AudioMixerEffectController> modifiedGroupEffects2 = sourceGroup.effects.ToList<AudioMixerEffectController>();
				List<AudioMixerEffectController> modifiedGroupEffects3 = targetGroup.effects.ToList<AudioMixerEffectController>();
				if (!AudioMixerController.MoveEffect(ref modifiedGroupEffects2, sourceIndex, ref modifiedGroupEffects3, targetIndex))
				{
					result = false;
					return result;
				}
				dictionary = AudioMixerController.BuildTemporaryGraph(allGroups, null, null, null, sourceGroup, modifiedGroupEffects2, targetGroup, modifiedGroupEffects3);
			}
			foreach (AudioMixerGroupController current in allGroups)
			{
				AudioMixerGroupController[] children = current.children;
				for (int i = 0; i < children.Length; i++)
				{
					AudioMixerGroupController key = children[i];
					object groupTail = dictionary[key].groupTail;
					if (!dictionary[groupTail].targets.Contains(current))
					{
						dictionary[groupTail].targets.Add(current);
					}
				}
			}
			result = AudioMixerController.DoesTheTemporaryGraphHaveAnyCycles(allGroups, identifiedLoop, dictionary);
			return result;
		}

		public static float DbToLin(float x)
		{
			float result;
			if (x < AudioMixerController.kMinVolume)
			{
				result = 0f;
			}
			else
			{
				result = Mathf.Pow(10f, x * 0.05f);
			}
			return result;
		}

		public void CloneViewFromCurrent()
		{
			Undo.RecordObject(this, "Create view");
			List<MixerGroupView> list = new List<MixerGroupView>(this.views);
			list.Add(new MixerGroupView
			{
				name = this.views[this.currentViewIndex].name + " - Copy",
				guids = this.views[this.currentViewIndex].guids
			});
			this.views = list.ToArray();
			this.currentViewIndex = list.Count - 1;
		}

		public void DeleteView(int index)
		{
			Undo.RecordObject(this, "Delete view");
			List<MixerGroupView> list = new List<MixerGroupView>(this.views);
			list.RemoveAt(index);
			this.views = list.ToArray();
			int index2 = Mathf.Clamp(this.currentViewIndex, 0, list.Count - 1);
			this.ForceSetView(index2);
		}

		public void SetView(int index)
		{
			if (this.currentViewIndex != index)
			{
				this.ForceSetView(index);
			}
		}

		public void SanitizeGroupViews()
		{
			List<AudioMixerGroupController> allGroups = this.GetAllAudioGroupsSlow();
			MixerGroupView[] views = this.views;
			for (int i = 0; i < views.Length; i++)
			{
				views[i].guids = (from x in views[i].guids
				from y in allGroups
				select new
				{
					x,
					y
				} into <>__TranspIdent0
				where <>__TranspIdent0.y.groupID == <>__TranspIdent0.x
				select <>__TranspIdent0.x).ToArray<GUID>();
			}
			this.views = views.ToArray<MixerGroupView>();
		}

		public void ForceSetView(int index)
		{
			this.currentViewIndex = index;
			this.SanitizeGroupViews();
		}

		public void AddGroupToCurrentView(AudioMixerGroupController group)
		{
			MixerGroupView[] views = this.views;
			List<GUID> list = views[this.currentViewIndex].guids.ToList<GUID>();
			list.Add(group.groupID);
			views[this.currentViewIndex].guids = list.ToArray();
			this.views = views.ToArray<MixerGroupView>();
		}

		public void SetCurrentViewVisibility(GUID[] guids)
		{
			MixerGroupView[] views = this.views;
			views[this.currentViewIndex].guids = guids;
			this.views = views.ToArray<MixerGroupView>();
			this.SanitizeGroupViews();
		}

		public AudioMixerGroupController[] GetCurrentViewGroupList()
		{
			List<AudioMixerGroupController> allAudioGroupsSlow = this.GetAllAudioGroupsSlow();
			MixerGroupView view = this.views[this.currentViewIndex];
			return (from g in allAudioGroupsSlow
			where view.guids.Contains(g.groupID)
			select g).ToArray<AudioMixerGroupController>();
		}

		public static float VolumeToScreenMapping(float value, float screenRange, bool forward)
		{
			float num = AudioMixerController.GetVolumeSplitPoint() * screenRange;
			float num2 = screenRange - num;
			float result;
			if (forward)
			{
				result = ((value <= 0f) ? (Mathf.Pow(value / AudioMixerController.kMinVolume, 1f / AudioMixerController.kVolumeWarp) * num2 + num) : (num - Mathf.Pow(value / AudioMixerController.GetMaxVolume(), 1f / AudioMixerController.kVolumeWarp) * num));
			}
			else
			{
				result = ((value >= num) ? (Mathf.Pow((value - num) / num2, AudioMixerController.kVolumeWarp) * AudioMixerController.kMinVolume) : (Mathf.Pow(1f - value / num, AudioMixerController.kVolumeWarp) * AudioMixerController.GetMaxVolume()));
			}
			return result;
		}

		public void OnUnitySelectionChanged()
		{
			List<AudioMixerGroupController> allAudioGroupsSlow = this.GetAllAudioGroupsSlow();
			UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(AudioMixerGroupController), SelectionMode.Deep);
			this.m_CachedSelection = allAudioGroupsSlow.Intersect(from g in filtered
			select (AudioMixerGroupController)g).ToList<AudioMixerGroupController>();
		}
	}
}
