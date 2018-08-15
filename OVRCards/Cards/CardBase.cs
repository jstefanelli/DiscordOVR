using OpenTK;
using OpenTK.Graphics.OpenGL;
using OVRCards.OGL;
using OVRCards.OGL.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OVRCards.Cards
{
	public enum CardStatus
	{
		Showning, Visible, Hiding, Hidden
	}

	public class Card
	{
		public Vector4 Color { get; set; }
		public Vector3 CaptionColor { get; set; }
		public Vector3 MessageColor { get; set; } 
		internal DateTime OpeningTime;
		internal DateTime EndTime;
		public CardStatus Status { get; internal set; }
		public int DurationMS { get; set; }
		public string Caption { get; set; }
		public string Message { get; set; }

		public Card(string caption, string message, float r, float g, float b, int durationMS, float cr = 1, float cg = 1, float cb = 1, float mr = 1, float mg = 1, float mb = 1)
		{
			Color = new Vector4(r, g, b, 1);
			Caption = caption;
			Message = message;
			DurationMS = durationMS;
			CaptionColor = new Vector3(cr, cg, cb);
			MessageColor = new Vector3(mr, mg, mb);
		}

		internal void Show()
		{
			OpeningTime = DateTime.Now;
			EndTime = OpeningTime.AddMilliseconds(DurationMS);
		}
	}

	internal class RectangleGL : IDrawable
	{
		internal static int vertexBuffer;
		internal static int uvBuffer;

		public static bool StaticLoaded { get; protected set; } = false;

		public static void Load()
		{
			if (StaticLoaded)
				return;

			float[] vertices = new float[]
			{
				-0.5f, -0.5f, 0f,
				0.5f, -0.5f, 0f,
				-0.5f, 0.5f, 0f,

				-0.5f, 0.5f, 0f,
				0.5f, -0.5f, 0f,
				0.5f, 0.5f, 0f
			};

			float[] uvs = new float[]
			{
				0f, 0f,
				1f, 0f,
				0f, 1f,

				0f, 1f,
				1f, 0f,
				1f, 1f
			};

			vertexBuffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

			uvBuffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, uvBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * uvs.Length, uvs, BufferUsageHint.StaticDraw);

			StaticLoaded = true;
		}

		public Vector2 Position = new Vector2(0, 0);
		public Vector2 Scale = new Vector2(1, 1);
		public float Depth = 1;

		public int VertexCount { get => 6; }

		public int StartIndex { get => 0; }

		public int UVBuffer
		{
			get
			{
				if (!StaticLoaded)
					Load();
				return uvBuffer;
			}
		}

		public int VertexBuffer
		{
			get
			{
				if (!StaticLoaded)
					Load();
				return vertexBuffer;
			}
		}

		public PrimitiveType PrimitiveType { get => PrimitiveType.Triangles; }

		public Vector4 Color { get; set; }
		public ShaderBase Shader { get; set; }

		public void Draw(Scene s)
		{
			s.Status.PushModelMatrix();
			s.Status.Model = s.Status.Model * Matrix4.CreateScale(new Vector3(Scale.X, Scale.Y, 1));
			s.Status.Model = s.Status.Model * Matrix4.CreateTranslation(new Vector3(Position.X, Position.Y, -Depth));
			Shader.Draw(s, this);
			s.Status.PopModelMatrix();
		}

		internal static void Unload()
		{
			if (!StaticLoaded)
				return;
			StaticLoaded = false;
			GL.DeleteBuffer(vertexBuffer);
			GL.DeleteBuffer(uvBuffer);
		}
	}

}
