using System;
using System.Collections.Generic;
using UnityEditor.U2D.Interface;
using UnityEngine;
using UnityEngine.U2D.Interface;

namespace UnityEditor.U2D
{
	internal class SpritePhysicsShapeModule : SpriteOutlineModule
	{
		private readonly float kDefaultPhysicsTessellationDetail = 0.25f;

		private readonly byte kDefaultPhysicsAlphaTolerance = 200;

		public override string moduleName
		{
			get
			{
				return "Edit Physics Shape";
			}
		}

		private ISpriteEditor spriteEditorWindow
		{
			get;
			set;
		}

		protected override List<SpriteOutline> selectedShapeOutline
		{
			get
			{
				return this.m_Selected.physicsShape;
			}
			set
			{
				this.m_Selected.physicsShape = value;
			}
		}

		public SpritePhysicsShapeModule(ISpriteEditor sem, IEventSystem ege, IUndoSystem us, IAssetDatabase ad, IGUIUtility gu, IShapeEditorFactory sef, ITexture2D outlineTexture) : base(sem, ege, us, ad, gu, sef, outlineTexture)
		{
			this.spriteEditorWindow = sem;
		}

		protected override bool HasShapeOutline(SpriteRect spriteRect)
		{
			return spriteRect.physicsShape != null && spriteRect.physicsShape.Count > 0;
		}

		protected override void SetupShapeEditorOutline(SpriteRect spriteRect)
		{
			if (spriteRect.physicsShape == null || spriteRect.physicsShape.Count == 0)
			{
				spriteRect.physicsShape = SpriteOutlineModule.GenerateSpriteRectOutline(spriteRect.rect, this.spriteEditorWindow.selectedTexture, (Math.Abs(spriteRect.tessellationDetail - -1f) >= Mathf.Epsilon) ? spriteRect.tessellationDetail : this.kDefaultPhysicsTessellationDetail, this.kDefaultPhysicsAlphaTolerance);
				this.spriteEditorWindow.SetDataModified();
			}
		}
	}
}
