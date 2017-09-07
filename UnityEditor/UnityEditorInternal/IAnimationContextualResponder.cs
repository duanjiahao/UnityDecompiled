using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal interface IAnimationContextualResponder
	{
		bool IsAnimatable(PropertyModification[] modifications);

		bool IsEditable(UnityEngine.Object targetObject);

		bool KeyExists(PropertyModification[] modifications);

		bool CandidateExists(PropertyModification[] modifications);

		bool CurveExists(PropertyModification[] modifications);

		bool HasAnyCandidates();

		bool HasAnyCurves();

		void AddKey(PropertyModification[] modifications);

		void RemoveKey(PropertyModification[] modifications);

		void RemoveCurve(PropertyModification[] modifications);

		void AddCandidateKeys();

		void AddAnimatedKeys();

		void GoToNextKeyframe(PropertyModification[] modifications);

		void GoToPreviousKeyframe(PropertyModification[] modifications);
	}
}
