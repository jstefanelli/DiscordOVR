using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFont;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OVRCards.Utils;
using OVRCards.OGL.Shaders;

namespace OVRCards.OGL.Font
{
	internal class FontBase
	{
		internal static string FontAssetsDir = NotificationManager.AssetsPath + Path.DirectorySeparatorChar + "Fonts" + Path.DirectorySeparatorChar;
		internal static Library library = null;
		internal static uint FontHeightPX { get; set; } = 72;

		internal static uint minCharacter = 0;
		internal static uint maxCharacter = 255;

		internal static readonly int spacingPX = 2;
		internal static readonly int atlasSizeX = 2048;
		internal static readonly int atlasSizeY = 2048;
		internal static readonly int numAtlas = 2;
		internal static readonly int atlasSpacingX = 2;
		internal static readonly int atlasSpacingY = 2;

		public Face face = null;
		public bool Loaded { get; protected set; }
		public string FontPath { get; protected set; }
		public Dictionary<char, FontChar> Characters { get; protected set; } = new Dictionary<char, FontChar>();


		public int Height { get => (int) FontHeightPX; }

		public int TxtArr { get; protected set; }

		public static FontBase Generate(string fontPath)
		{
			FontBase f = new FontBase
			{
				FontPath = fontPath
			};

			return f;
		}

		internal static void LoadStatic()
		{
			library = new Library();
		}

		private FontBase()
		{
			
		}

		internal void Load()
		{
			if (library == null)
				LoadStatic();
			if (Loaded)
				return;

			TxtArr = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2DArray, TxtArr);
			GL.TexImage3D(
				TextureTarget.Texture2DArray,
				0,
				PixelInternalFormat.R8,
				atlasSizeX,
				atlasSizeY,
				numAtlas,
				0,
				PixelFormat.Red,
				PixelType.UnsignedByte,
				IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
			face = new Face(library, FontPath, 0);
			face.SetPixelSizes(0, FontHeightPX);
			FontChar nullChar = new FontChar(0, 0, 0, new Vector2(0, 0), new Vector2(0, 0), Matrix3.CreateScale(0, 0, 0));
			int currentAtlas = 0;
			int currentAtlasX = 0;
			int currentAtlasY = 0;
			int maxLineY = 0;
			for (uint i = minCharacter; i <= maxCharacter; i++)
			{
				try
				{
					face.LoadChar(i, LoadFlags.Render, LoadTarget.Normal);
				}
				catch (FreeTypeException ex)
				{
					Debug.WriteLine("FreeType Exception: " + ex.Message);
					Debug.WriteLine("Stacktrace: ");
					Debug.WriteLine(ex.StackTrace);
					Characters.Add((char)i, nullChar);
					continue;
				}
				if (face.Glyph.Bitmap.Width == 0 || face.Glyph.Bitmap.Rows == 0)
				{
					Characters.Add((char)i, nullChar);
					Characters[(char)i].Advance = face.Glyph.Advance.X.ToSingle();
					continue;
				}
				if(currentAtlasX + face.Glyph.Bitmap.Width >= atlasSizeX)
				{
					currentAtlasX = 0;
					currentAtlasY += maxLineY + atlasSpacingY;
					maxLineY = 0;
					if(currentAtlasY + face.Glyph.Bitmap.Rows >= atlasSizeY)
					{
						currentAtlas++;
						if(currentAtlas >= numAtlas)
						{
							GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
							throw new OverflowException("Not enough atlases in texture array.");
						}
						currentAtlasY = 0;
					}
				}

				GL.TexSubImage3D(
					TextureTarget.Texture2DArray,
					0,
					currentAtlasX,
					currentAtlasY,
					currentAtlas,
					face.Glyph.Bitmap.Width,
					face.Glyph.Bitmap.Rows,
					1,
					PixelFormat.Red,
					PixelType.UnsignedByte,
					face.Glyph.Bitmap.BufferData);

				Matrix3 uvMatrix = Matrix3.CreateScale(new Vector3((float)face.Glyph.Bitmap.Width / atlasSizeX, (float)face.Glyph.Bitmap.Rows / atlasSizeY, 1));
				uvMatrix *= Matrix3E.CreateTranslation((float) currentAtlasX / atlasSizeX, (float) currentAtlasY / atlasSizeY);

				currentAtlasX += face.Glyph.Bitmap.Width + atlasSpacingX;
				if (face.Glyph.Bitmap.Rows > maxLineY)
					maxLineY = face.Glyph.Bitmap.Rows;

				FontChar chr = new FontChar(i, currentAtlas, face.Glyph.Advance.X.ToSingle(), new Vector2(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows), new Vector2(face.Glyph.BitmapLeft, face.Glyph.BitmapTop), uvMatrix);
				Characters.Add((char)i, chr);
			}
			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
			Loaded = true;
		}

		internal void Unload()
		{
			if (!Loaded)
				return;
			GL.DeleteTexture(TxtArr);
			Characters.Clear();
			Loaded = false;
		}

		internal float GetWordLength(string s, float scale)
		{
			float length = 0;
			foreach(char c in s)
			{
				length += Characters[c].Advance * scale;
			}
			return length;
		}

		internal List<string> GenLines(string s, float scale, float maxLen, out int charNumber)
		{
			charNumber = 0;
			List<string> lines = new List<string>();
			string[] rawLines = s.Split('\n');
			foreach(string l in rawLines)
			{
				string currentLine = "";
				float currentLineLength = 0;
				foreach (string w in l.Split(' ')){
					string wt = w.Trim();
					float len = GetWordLength(wt, scale);
					charNumber += wt.Length;
					if(currentLineLength + len > maxLen)
					{
						lines.Add(currentLine.Trim());
						currentLine = "";
						currentLineLength = 0;
					}
					currentLine += wt + " ";
					charNumber++;
					currentLineLength += len;
					currentLineLength += Characters[' '].Advance * scale;
				}
				if (currentLine.Length > 0)
					lines.Add(currentLine.Trim());
			}
			return lines;
		}

		internal Vector2 QuerySize(string s, float scale, float maxLen)
		{
			float x = 0;
			float y = 0;
			List<string> lines = GenLines(s, scale, maxLen, out int _);
			y = lines.Count * ((FontHeightPX + spacingPX) * scale);
			y += lines.Count - 1 * spacingPX;
			foreach(string l in lines)
			{
				float currentX = 0;
				foreach(char c in l)
				{
					currentX += Characters[c].Advance * scale;
				}
				if (currentX > x)
					x = currentX;
			}
			return new Vector2(x, y);
		}

		internal static Matrix4 offCenterMat = Matrix4.CreateTranslation(0.5f, 0.5f, 0f);

		internal void DrawText(Scene s, Vector2 position, Vector3 color, string text, float scale, float maxLen)
		{
			s.Status.PushModelMatrix();
			s.Status.Model = s.Status.Model * Matrix4.CreateTranslation(position.X, position.Y, -1);

			List<string> lines = GenLines(text, scale, maxLen, out int charNumber);
			TextDrawInstance inst = new TextDrawInstance(charNumber, this, color);
			float currentY = 0;
			int charId = 0;
			foreach(string l in lines)
			{
				float currentX = 0;
				foreach(char c in l)
				{
					Vector2 translation = new Vector2(currentX, -currentY) + new Vector2(Characters[c].Bearing.X * scale, - (Characters[c].Size.Y - Characters[c].Bearing.Y) * scale);
					Vector2 size = Characters[c].Size * scale;
					Matrix4 offsetMatrix = Matrix4.CreateScale(new Vector3(size.X, size.Y, 1));
					offsetMatrix *= Matrix4.CreateTranslation(new Vector3(translation.X, translation.Y, 0));
					

					inst.Chars[charId] = offCenterMat;
					inst.UVs[charId] = Characters[c].UVMatrix;
					inst.Atlases[charId] = Characters[c].TextureAtlas;
					inst.Offsets[charId] = offsetMatrix;
					currentX += Characters[c].Advance * scale;
					charId++;
				}
				currentY += (FontHeightPX) * scale;
			}
			FontShader.Instance.DrawText(s, inst);
			s.Status.PopModelMatrix();
		}

		internal static void UnloadStatic()
		{
			library.Dispose();
		}
	}

	internal class TextDrawInstance
	{
		public int Amount { get; protected set; }
		public int[] Atlases { get; protected set; }
		public Matrix4[] Chars { get; protected set; }
		public Matrix4[] Offsets { get; protected set; }
		public Matrix3[] UVs { get; protected set; }
		public FontBase Font { get; protected set; }
		public Vector3 Color { get; protected set; }

		public TextDrawInstance(int amount, FontBase f, Vector3 color)
		{
			Amount = amount;
			Atlases = new int[amount];
			Chars = new Matrix4[amount];
			Offsets = new Matrix4[amount];
			UVs = new Matrix3[amount];
			Font = f;
			Color = color;
		}
	}
}
