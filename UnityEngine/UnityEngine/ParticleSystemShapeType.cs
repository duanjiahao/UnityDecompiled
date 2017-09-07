using System;

namespace UnityEngine
{
	public enum ParticleSystemShapeType
	{
		Sphere,
		[Obsolete("SphereShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.")]
		SphereShell,
		Hemisphere,
		[Obsolete("HemisphereShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.")]
		HemisphereShell,
		Cone,
		Box,
		Mesh,
		[Obsolete("ConeShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.")]
		ConeShell,
		ConeVolume,
		[Obsolete("ConeVolumeShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.")]
		ConeVolumeShell,
		Circle,
		[Obsolete("CircleEdge is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.")]
		CircleEdge,
		SingleSidedEdge,
		MeshRenderer,
		SkinnedMeshRenderer,
		BoxShell,
		BoxEdge,
		Donut
	}
}
