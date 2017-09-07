using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationPropertyContextualMenu
	{
		public static AnimationPropertyContextualMenu Instance = new AnimationPropertyContextualMenu();

		private IAnimationContextualResponder m_Responder;

		private static GUIContent addKeyContent = EditorGUIUtility.TextContent("Add Key");

		private static GUIContent updateKeyContent = EditorGUIUtility.TextContent("Update Key");

		private static GUIContent removeKeyContent = EditorGUIUtility.TextContent("Remove Key");

		private static GUIContent removeCurveContent = EditorGUIUtility.TextContent("Remove All Keys");

		private static GUIContent goToPreviousKeyContent = EditorGUIUtility.TextContent("Go to Previous Key");

		private static GUIContent goToNextKeyContent = EditorGUIUtility.TextContent("Go to Next Key");

		private static GUIContent addCandidatesContent = EditorGUIUtility.TextContent("Key All Modified");

		private static GUIContent addAnimatedContent = EditorGUIUtility.TextContent("Key All Animated");

		public AnimationPropertyContextualMenu()
		{
			EditorApplication.contextualPropertyMenu = (EditorApplication.SerializedPropertyCallbackFunction)Delegate.Combine(EditorApplication.contextualPropertyMenu, new EditorApplication.SerializedPropertyCallbackFunction(this.OnPropertyContextMenu));
			MaterialEditor.contextualPropertyMenu = (MaterialEditor.MaterialPropertyCallbackFunction)Delegate.Combine(MaterialEditor.contextualPropertyMenu, new MaterialEditor.MaterialPropertyCallbackFunction(this.OnPropertyContextMenu));
		}

		public void SetResponder(IAnimationContextualResponder responder)
		{
			this.m_Responder = responder;
		}

		public bool IsResponder(IAnimationContextualResponder responder)
		{
			return responder == this.m_Responder;
		}

		private void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
		{
			if (this.m_Responder != null)
			{
				PropertyModification[] modifications = AnimationWindowUtility.SerializedPropertyToPropertyModifications(property);
				bool flag = this.m_Responder.IsAnimatable(modifications);
				if (flag)
				{
					UnityEngine.Object targetObject = property.serializedObject.targetObject;
					if (this.m_Responder.IsEditable(targetObject))
					{
						this.OnPropertyContextMenu(menu, modifications);
					}
					else
					{
						this.OnDisabledPropertyContextMenu(menu);
					}
				}
			}
		}

		private void OnPropertyContextMenu(GenericMenu menu, MaterialProperty property, Renderer targetObject)
		{
			if (this.m_Responder != null)
			{
				if (property.targets != null && property.targets.Length != 0)
				{
					PropertyModification[] modifications = MaterialAnimationUtility.MaterialPropertyToPropertyModifications(property, targetObject);
					if (this.m_Responder.IsEditable(targetObject))
					{
						this.OnPropertyContextMenu(menu, modifications);
					}
					else
					{
						this.OnDisabledPropertyContextMenu(menu);
					}
				}
			}
		}

		private void OnPropertyContextMenu(GenericMenu menu, PropertyModification[] modifications)
		{
			bool flag = this.m_Responder.KeyExists(modifications);
			bool flag2 = this.m_Responder.CandidateExists(modifications);
			bool flag3 = flag || this.m_Responder.CurveExists(modifications);
			bool flag4 = this.m_Responder.HasAnyCandidates();
			bool flag5 = this.m_Responder.HasAnyCurves();
			menu.AddItem((!flag || !flag2) ? AnimationPropertyContextualMenu.addKeyContent : AnimationPropertyContextualMenu.updateKeyContent, false, delegate
			{
				this.m_Responder.AddKey(modifications);
			});
			if (flag)
			{
				menu.AddItem(AnimationPropertyContextualMenu.removeKeyContent, false, delegate
				{
					this.m_Responder.RemoveKey(modifications);
				});
			}
			else
			{
				menu.AddDisabledItem(AnimationPropertyContextualMenu.removeKeyContent);
			}
			if (flag3)
			{
				menu.AddItem(AnimationPropertyContextualMenu.removeCurveContent, false, delegate
				{
					this.m_Responder.RemoveCurve(modifications);
				});
			}
			else
			{
				menu.AddDisabledItem(AnimationPropertyContextualMenu.removeCurveContent);
			}
			menu.AddSeparator(string.Empty);
			if (flag4)
			{
				menu.AddItem(AnimationPropertyContextualMenu.addCandidatesContent, false, delegate
				{
					this.m_Responder.AddCandidateKeys();
				});
			}
			else
			{
				menu.AddDisabledItem(AnimationPropertyContextualMenu.addCandidatesContent);
			}
			if (flag5)
			{
				menu.AddItem(AnimationPropertyContextualMenu.addAnimatedContent, false, delegate
				{
					this.m_Responder.AddAnimatedKeys();
				});
			}
			else
			{
				menu.AddDisabledItem(AnimationPropertyContextualMenu.addAnimatedContent);
			}
			menu.AddSeparator(string.Empty);
			if (flag3)
			{
				menu.AddItem(AnimationPropertyContextualMenu.goToPreviousKeyContent, false, delegate
				{
					this.m_Responder.GoToPreviousKeyframe(modifications);
				});
				menu.AddItem(AnimationPropertyContextualMenu.goToNextKeyContent, false, delegate
				{
					this.m_Responder.GoToNextKeyframe(modifications);
				});
			}
			else
			{
				menu.AddDisabledItem(AnimationPropertyContextualMenu.goToPreviousKeyContent);
				menu.AddDisabledItem(AnimationPropertyContextualMenu.goToNextKeyContent);
			}
		}

		private void OnDisabledPropertyContextMenu(GenericMenu menu)
		{
			menu.AddDisabledItem(AnimationPropertyContextualMenu.addKeyContent);
			menu.AddDisabledItem(AnimationPropertyContextualMenu.removeKeyContent);
			menu.AddDisabledItem(AnimationPropertyContextualMenu.removeCurveContent);
			menu.AddSeparator(string.Empty);
			menu.AddDisabledItem(AnimationPropertyContextualMenu.addCandidatesContent);
			menu.AddDisabledItem(AnimationPropertyContextualMenu.addAnimatedContent);
			menu.AddSeparator(string.Empty);
			menu.AddDisabledItem(AnimationPropertyContextualMenu.goToPreviousKeyContent);
			menu.AddDisabledItem(AnimationPropertyContextualMenu.goToNextKeyContent);
		}
	}
}
