using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace OVRCards.OGL.Font
{
	internal class FontChar
	{
		public uint CharCode { get; internal set; }
		public Vector2 Bearing { get; internal set; }
		public Vector2 Size { get; internal set; }
		public Matrix3 UVMatrix { get; internal set; }
		public int TextureAtlas { get; internal set; }
		public float Advance { get; internal set; }

		public FontChar(uint charCode, int AtlasNumber, float advance, Vector2 size, Vector2 bearing, Matrix3 uvMatrix)
		{
			CharCode = charCode;
			TextureAtlas = AtlasNumber;
			Advance = advance;
			Size = size;
			Bearing = bearing;
			UVMatrix = uvMatrix;
		}
	}
}
