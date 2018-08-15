using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OVRCards.OGL
{
	internal class GLStatus
	{
		private Matrix4 _projection;
		private Matrix4 _view;
		private Matrix4 _model;

		public Matrix4 Projection { get => _projection; set
			{
				_projection = value;
				shouldUpdate = true;
			}
		}
		public Matrix4 View { get => _view; set
			{
				_view = value;
				shouldUpdate = true;
			}
		}
		public Matrix4 Model { get => _model; set
			{
				_model = value;
				shouldUpdate = true;
			}
		}

		private Matrix4 _mv;
		private Matrix4 _mvp;
		private bool shouldUpdate = false;

		private Stack<Matrix4> ModelStack = new Stack<Matrix4>();

		public void PushModelMatrix()
		{
			ModelStack.Push(_model);
			Matrix4 last = _model;
			_model = Matrix4.Identity * last;
		}

		public void PopModelMatrix()
		{
			_model = ModelStack.Pop();
			shouldUpdate = true;
		}

		public Matrix4 Mvp
		{
			get
			{
				if (shouldUpdate)
				{
					_mv = _model * _view;
					_mvp = _mv * _projection;
				}
				return _mvp;
			}
		}
		public Matrix4 Mv
		{
			get
			{
				if (shouldUpdate)
				{
					_mv = _model * _view;
					_mvp = _mv * _projection;
				}
				return _mv;
			}
		}
	}

	internal class Scene
	{
		internal GLStatus Status { get; private set; }

		public Scene(int width, int height)
		{
			Status = new GLStatus();
			Status.Projection = Matrix4.CreateOrthographic(width, height, 0.01f, 100f);
			Status.View = Matrix4.LookAt(new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector3(0, 1, 0));
			Status.Model = Matrix4.Identity;
		}
	}
}
