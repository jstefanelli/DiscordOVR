using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OVRCards.OGL.Font;
using OVRCards.Cards;

namespace OVRCards.OGL.Shaders
{
	internal class FontShader : ShaderBase
	{
		private static readonly int maxInstances = 100;

		private static FontShader _instance = null;

		public static FontShader Instance
		{
			get
			{
				if (_instance == null)
					_instance = new FontShader();
				return _instance;
			}
		}

		private int aVertexLoc;
		private int aUvLoc;
		private int uMvpLoc;
		private int uColorLoc;
		private int uTxtLoc;

		private string VSPath = ShadersPath + "Font.vs";
		private string FSPath = ShadersPath + "Font.fs";

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
			aUvLoc = GL.GetAttribLocation(Id, "aUv");
			uMvpLoc = GL.GetUniformLocation(Id, "uMvp");
			uColorLoc = GL.GetUniformLocation(Id, "uColor");
			uTxtLoc = GL.GetUniformLocation(Id, "uTxt");

			if (!RectangleGL.StaticLoaded)
			{
				RectangleGL.Load();
			}

			Loaded = true;
			return true;
		}

		public override void Draw(Scene s, IDrawable d)
		{
			//Unused
			throw new InvalidOperationException("This shader cannot render arbitrary drawables.");
		}

		private void DrawInstances(int number)
		{
			GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, number);
		}

		public void DrawText(Scene s, TextDrawInstance instance)
		{
			if (!Loaded)
				Compile();

			GL.UseProgram(Id);

			GL.EnableVertexAttribArray(aVertexLoc);
			GL.BindBuffer(BufferTarget.ArrayBuffer, RectangleGL.vertexBuffer);
			GL.VertexAttribPointer(aVertexLoc, 3, VertexAttribPointerType.Float, false, 0, 0);

			GL.EnableVertexAttribArray(aUvLoc);
			GL.BindBuffer(BufferTarget.ArrayBuffer, RectangleGL.uvBuffer);
			GL.VertexAttribPointer(aUvLoc, 2, VertexAttribPointerType.Float, false, 0, 0);

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2DArray, instance.Font.TxtArr);
			GL.Uniform1(uTxtLoc, 0);

			Vector3 col = instance.Color;
			GL.Uniform3(uColorLoc, ref col);

			Matrix4 mvp = s.Status.Mvp;
			GL.UniformMatrix4(uMvpLoc, false, ref mvp);

			int currentInstance = 0;

			for (int i = 0; i < instance.Amount; i++)
			{
				int offsetLoc = GL.GetUniformLocation(Id, "uOffset[" + currentInstance + "]");
				int uvLoc = GL.GetUniformLocation(Id, "uUVMat[" + currentInstance + "]");
				int atlasLoc = GL.GetUniformLocation(Id, "uAtlas[" + currentInstance + "]");

				Matrix4 offsetMatrix = instance.Chars[i] * instance.Offsets[i];
				GL.UniformMatrix4(offsetLoc, false, ref offsetMatrix);
				Matrix3 uvMat = instance.UVs[i];
				GL.UniformMatrix3(uvLoc, false, ref uvMat);
				GL.Uniform1(atlasLoc,(float) instance.Atlases[i]);

				if(i > maxInstances)
				{
					DrawInstances(currentInstance + 1);
					currentInstance = 0;

				}else
					currentInstance++;
			}
			if (currentInstance != 0)
				DrawInstances(currentInstance);
			GL.DisableVertexAttribArray(aUvLoc);
			GL.DisableVertexAttribArray(aVertexLoc);
		}

		public override void Unload()
		{
			if (!Loaded)
				return;
			GL.DeleteProgram(Id);
		}

		private FontShader()
		{

		}
	}
}
