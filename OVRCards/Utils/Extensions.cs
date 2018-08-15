using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OVRCards.Utils
{
	internal static class Matrix3E
	{
		public static Matrix3 CreateTranslation(Vector2 translation)
		{
			return CreateTranslation(translation.X, translation.Y);
		}

		public static Matrix3 CreateTranslation(float x, float y)
		{
			Matrix3 m = Matrix3.Identity;
			m.Row2.X = x;
			m.Row2.Y = y;
			return m; 
		}
	}
}
