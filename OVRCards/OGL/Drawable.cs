using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OVRCards.OGL.Shaders;

namespace OVRCards.OGL
{
	internal interface IDrawable
	{
		int VertexBuffer { get; }
		int UVBuffer { get; }
		int StartIndex { get; }
		int VertexCount { get; }
		Vector4 Color { get; }
		PrimitiveType PrimitiveType { get; }
		ShaderBase Shader { get; set; }
		void Draw(Scene s);
	}
}
