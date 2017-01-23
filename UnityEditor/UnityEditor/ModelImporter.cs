using System;
using System.Runtime.CompilerServices;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor
{
	public class ModelImporter : AssetImporter
	{
		[Obsolete("Use importMaterials, materialName and materialSearch instead")]
		public extern ModelImporterGenerateMaterials generateMaterials
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool importMaterials
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ModelImporterMaterialName materialName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ModelImporterMaterialSearch materialSearch
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float globalScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isUseFileUnitsSupported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool useFileUnits
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float fileScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isFileScaleUsed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool importBlendShapes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool addCollider
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float normalSmoothingAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Please use tangentImportMode instead")]
		public bool splitTangentsAcrossSeams
		{
			get
			{
				return this.importTangents == ModelImporterTangents.CalculateLegacyWithSplitTangents;
			}
			set
			{
				if (this.importTangents == ModelImporterTangents.CalculateLegacyWithSplitTangents && !value)
				{
					this.importTangents = ModelImporterTangents.CalculateLegacy;
				}
				else if (this.importTangents == ModelImporterTangents.CalculateLegacy && value)
				{
					this.importTangents = ModelImporterTangents.CalculateLegacyWithSplitTangents;
				}
			}
		}

		public extern bool swapUVChannels
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool generateSecondaryUV
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float secondaryUVAngleDistortion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float secondaryUVAreaDistortion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float secondaryUVHardAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float secondaryUVPackMargin
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ModelImporterGenerateAnimations generateAnimations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TakeInfo[] importedTakeInfos
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string[] transformPaths
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string[] referencedClips
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool optimizeMesh
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("normalImportMode is deprecated. Use importNormals instead")]
		public extern ModelImporterTangentSpaceMode normalImportMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("tangentImportMode is deprecated. Use importTangents instead")]
		public extern ModelImporterTangentSpaceMode tangentImportMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ModelImporterNormals importNormals
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ModelImporterTangents importTangents
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool bakeIK
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isBakeIKSupported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("use resampleCurves instead.")]
		public extern bool resampleRotations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool resampleCurves
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isTangentImportSupported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Use animationCompression instead", true)]
		private bool reduceKeyframes
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public extern ModelImporterMeshCompression meshCompression
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool importAnimation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool optimizeGameObjects
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string[] extraExposedTransformPaths
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ModelImporterAnimationCompression animationCompression
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float animationRotationError
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float animationPositionError
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float animationScaleError
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern WrapMode animationWrapMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ModelImporterAnimationType animationType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ModelImporterHumanoidOversampling humanoidOversampling
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string motionNodeName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Avatar sourceAvatar
		{
			get
			{
				return this.sourceAvatarInternal;
			}
			set
			{
				Avatar sourceAvatarInternal = value;
				if (value != null)
				{
					ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(value)) as ModelImporter;
					if (modelImporter != null)
					{
						this.humanDescription = modelImporter.humanDescription;
					}
					else
					{
						Debug.LogError("Avatar must be from a ModelImporter, otherwise use ModelImporter.humanDescription");
						sourceAvatarInternal = null;
					}
				}
				this.sourceAvatarInternal = sourceAvatarInternal;
			}
		}

		internal extern Avatar sourceAvatarInternal
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public HumanDescription humanDescription
		{
			get
			{
				HumanDescription result;
				this.INTERNAL_get_humanDescription(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_humanDescription(ref value);
			}
		}

		[Obsolete("splitAnimations has been deprecated please use clipAnimations instead.", true)]
		public bool splitAnimations
		{
			get
			{
				return this.clipAnimations.Length != 0;
			}
			set
			{
			}
		}

		public extern ModelImporterClipAnimation[] clipAnimations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ModelImporterClipAnimation[] defaultClipAnimations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool isAssetOlderOr42
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_humanDescription(out HumanDescription value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_humanDescription(ref HumanDescription value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateSkeletonPose(SkeletonBone[] skeletonBones, SerializedProperty serializedProperty);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateTransformMask(AvatarMask mask, SerializedProperty serializedProperty);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern AnimationClip GetPreviewAnimationClipForTake(string takeName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string CalculateBestFittingPreviewGameObject();

		public void CreateDefaultMaskForClip(ModelImporterClipAnimation clip)
		{
			if (this.defaultClipAnimations.Length > 0)
			{
				AvatarMask avatarMask = new AvatarMask();
				this.defaultClipAnimations[0].ConfigureMaskFromClip(ref avatarMask);
				clip.ConfigureClipFromMask(avatarMask);
				UnityEngine.Object.DestroyImmediate(avatarMask);
			}
			else
			{
				Debug.LogError("Cannot create default mask because the current importer doesn't have any animation information");
			}
		}
	}
}
