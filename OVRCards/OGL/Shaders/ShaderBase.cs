using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace OVRCards.OGL.Shaders
{
	internal abstract class ShaderBase
	{
		internal readonly static string ShadersPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Shaders" + Path.DirectorySeparatorChar;

		public int Id { get; protected set; }
		public bool Loaded { get; protected set; } = false;

		public abstract bool Compile();

		protected bool CompileFromFile(string vsPath, string fsPath)
		{
			string vsSource, fsSource;
			using (FileStream f = new FileStream(vsPath, FileMode.Open, FileAccess.Read))
			{
				vsSource = new StreamReader(f).ReadToEnd();
			}

			using(FileStream f = new FileStream(fsPath, FileMode.Open, FileAccess.Read))
			{
				fsSource = new StreamReader(f).ReadToEnd();
			}
			return Compile(vsSource, fsSource);
		}

		protected bool Compile(string vsSource, string fsSource)
		{
			int vsId = GL.CreateShader(ShaderType.VertexShader), fsId = GL.CreateShader(ShaderType.FragmentShader);

			GL.ShaderSource(vsId, vsSource);
			GL.CompileShader(vsId);
			GL.GetShader(vsId, ShaderParameter.CompileStatus, out int status);
			Debug.WriteLine("Vertex shader compile status: " + status);
			if(status == 0)
			{
				string infoLog = GL.GetShaderInfoLog(vsId);
				Debug.WriteLine("Vertex Shader compilation error: ");
				Debug.WriteLine(infoLog);
				return false;
			}

			GL.ShaderSource(fsId, fsSource);
			GL.CompileShader(fsId);
			GL.GetShader(fsId, ShaderParameter.CompileStatus, out status);
			if(status == 0)
			{
				string infoLog = GL.GetShaderInfoLog(fsId);
				Debug.WriteLine("Fragment Shader compilation error: ");
				Debug.WriteLine(infoLog);
				return false;
			}

			Id = GL.CreateProgram();
			GL.AttachShader(Id, vsId);
			GL.AttachShader(Id, fsId);
			GL.LinkProgram(Id);
			GL.GetProgram(Id, GetProgramParameterName.LinkStatus, out status);
			if(status == 0)
			{
				string infoLog = GL.GetProgramInfoLog(Id);
				Debug.WriteLine("Program Link error:");
				Debug.WriteLine(infoLog);
				return false;
			}

			GL.DeleteShader(vsId);
			GL.DeleteShader(fsId);
			Debug.WriteLine("Shader compiled");
			return true;
		}

		public abstract void Unload();

		public abstract void Draw(Scene s, IDrawable d);
	}
}
