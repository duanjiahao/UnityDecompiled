using System;

namespace UnityEngine.U2D.Interface
{
	internal interface IGL
	{
		void PushMatrix();

		void PopMatrix();

		void MultMatrix(Matrix4x4 m);

		void Begin(int mode);

		void End();

		void Color(Color c);

		void Vertex(Vector3 v);
	}
}
