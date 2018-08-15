using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace OVRCards.OGL.Shaders
{
	internal class ColorShader : ShaderBase
	{
		private int aVertexLoc;
		private int uMvpLoc;
		private int uColorLoc;

		private string VSPath = ShadersPath + "Color.vs";
		private string FSPath = ShadersPath + "Color.fs";

		public override bool Compile()
		{
			if (Loaded)
			{
				return true;
			}
			if(!CompileFromFile(VSPath, FSPath))
			{
				return false;
			}

			aVertexLoc = GL.GetAttribLocation(Id, "aVertex");
			uMvpLoc = GL.GetUniformLocation(Id, "uMvp");
			uColorLoc = GL.GetUniformLocation(Id, "uColor");

			Debug.WriteLine("Color Shader compiled.");

			Loaded = true;
			return true;
		}
		

		public override void Draw(Scene s, IDrawable d)
		{
			if (!Loaded)
			{
				Compile();
			}
			GL.UseProgram(Id);
			GL.EnableVertexAttribArray(aVertexLoc);
			GL.BindBuffer(BufferTarget.ArrayBuffer, d.VertexBuffer);
			GL.VertexAttribPointer(aVertexLoc, 3, VertexAttribPointerType.Float, false, 0, 0);

			GL.Uniform4(uColorLoc, d.Color);
			Matrix4 mvp = s.Status.Mvp;
			GL.UniformMatrix4(uMvpLoc, false, ref mvp);

			GL.DrawArrays(d.PrimitiveType, d.StartIndex, d.VertexCount);

			GL.DisableVertexAttribArray(aVertexLoc);
		}

		public override void Unload()
		{
			Loaded = false;
			GL.DeleteProgram(Id);
		}

		private static ColorShader _instance = null;

		public static ColorShader Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ColorShader();
				}
				return _instance;
			}
		}

		private ColorShader()
		{

		}
	}
}
