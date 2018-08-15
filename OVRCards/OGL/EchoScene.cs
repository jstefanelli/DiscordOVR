using OpenTK;
using OVRCards.Cards;
using OVRCards.OGL.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OVRCards.OGL
{
	internal class EchoScene : Scene
	{
		public GameWindow Window { get; set; }
		public int EchoTexture { get; set; }
		private RectangleGL EchoRect = new RectangleGL()
		{
			Shader = EchoShader.Instance
		};

		private static EchoScene _instance = null;
		public static EchoScene Instance
		{
			get
			{
				if (_instance == null)
					_instance = new EchoScene();
				return _instance;
			}
		}

		public EchoScene() : base(100, 100)
		{
		}

		public void Draw()
		{
			EchoShader.Instance.CurrentTexture = EchoTexture;
			EchoRect.Scale = new Vector2(Window.Width, Window.Height);
			Status.Projection = Matrix4.CreateOrthographic(Window.Width, Window.Height, 0.1f, 100);
			EchoRect.Draw(this);
		}
	}
}
