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
	internal class EchoShader : ShaderBase
	{
		private int aVertexLoc;
		private int aUvLoc;
		private int uMvpLoc;
		private int uTxtLoc;

		private string VSPath = ShadersPath + "Echo.vs";
		private string FSPath = ShadersPath + "Echo.fs";

		public override bool Compile()
		{
			if (Loaded)
			{
				return true;
			}
			if (!CompileFromFile(VSPath, FSPath))
			{
				return false;
			}

			aVertexLoc = GL.GetAttribLocation(Id, "aVertex");
			aUvLoc = GL.GetAttribLocation(Id, "aUv");
			uMvpLoc = GL.GetUniformLocation(Id, "uMvp");
			uTxtLoc = GL.GetUniformLocation(Id, "uTxt");

			Debug.WriteLine("Echo shader compiled");

			Loaded = true;
			return true;
		}

		public int CurrentTexture { get; set; }

		public override void Draw(Scene s, IDrawable d)
		{
			if (!Loaded)
			{
				Compile();
			}
			GL.UseProgram(Id);
			GL.BindBuffer(BufferTarget.ArrayBuffer, d.VertexBuffer);
			GL.VertexAttribPointer(aVertexLoc, 3, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(aVertexLoc);

			GL.BindBuffer(BufferTarget.ArrayBuffer, d.UVBuffer);
			GL.VertexAttribPointer(aUvLoc, 2, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(aUvLoc);

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, CurrentTexture);
			GL.Uniform1(uTxtLoc, 0);
			
			Matrix4 mvp = s.Status.Mvp;
			GL.UniformMatrix4(uMvpLoc, false, ref mvp);

			GL.DrawArrays(d.PrimitiveType, d.StartIndex, d.VertexCount);

			GL.DisableVertexAttribArray(aUvLoc);
			GL.DisableVertexAttribArray(aVertexLoc);
		}

		public override void Unload()
		{
			Loaded = false;
			GL.DeleteProgram(Id);
		}

		private static EchoShader _instance = null;

		public static EchoShader Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new EchoShader();
				}
				return _instance;
			}
		}

		private EchoShader()
		{

		}
	}
}
