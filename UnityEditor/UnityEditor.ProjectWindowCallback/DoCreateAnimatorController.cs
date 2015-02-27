using System;
using UnityEditorInternal;
namespace UnityEditor.ProjectWindowCallback
{
	internal class DoCreateAnimatorController : EndNameEditAction
	{
		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			AnimatorController o = AnimatorController.CreateAnimatorControllerAtPath(pathName);
			ProjectWindowUtil.ShowCreatedAsset(o);
		}
	}
}
