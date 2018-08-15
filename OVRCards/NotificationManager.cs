using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Valve.VR;
using OVRCards.OGL.Shaders;
using OVRCards.Cards;
using System.Collections.Concurrent;
using OVRCards.OGL;
using OVRCards.Utils;
using System.IO;
using System.Reflection;
using OVRCards.OGL.Font;
using System.Runtime.InteropServices;

namespace OVRCards
{
	public class NotificationManager
	{
		internal static string AssetsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar;

		private GameWindow render_window;

		public delegate void OnEndDelegate();
		public event OnEndDelegate OnEnd;

		public static NotificationManager Create()
		{
			return new NotificationManager();
		}

		private NotificationManager()
		{


		}

		private Thread RenderThread = null;
		private bool RenderAlive = false;

		public void StartRender()
		{
			if (RenderThread != null)
			{
				return;
			}
			RenderThread = new Thread(() =>
			{
				RenderAlive = true;
				Debug.WriteLine("Starting Render Routine....");
				RenderRoutine();
				RenderThread = null;
				OnEnd();
			});
			RenderThread.Start();
		}

		public void StopRender()
		{
			RenderAlive = false;
		}

		#region OpenVR
		CVRSystem vr;
		CVROverlay overlay;

		uint HmdId;
		uint LeftControllerId;
		uint RightControllerId;

		ulong overlayHandle = 0;
		Texture_t overlayTexture;
		HmdMatrix34_t mat;

		private bool StartupOVR()
		{
			EVRInitError error = EVRInitError.Driver_Failed;
			vr = OpenVR.Init(ref error, EVRApplicationType.VRApplication_Overlay);

			if (error != EVRInitError.None)
			{
				Debug.WriteLine("OpenVR Init Error: " + error.ToString());
				return false;
			}

			overlay = OpenVR.Overlay;

			EVROverlayError overlayError = overlay.CreateOverlay("Jstf_ovr_cards", "OpenVR Cards System Overlay", ref overlayHandle);
			if (overlayError != EVROverlayError.None)
			{
				OpenVR.Shutdown();
				Debug.WriteLine("OpenVR Overlay Error: " + overlayError.ToString());
				return false;
			}

			/*overlayError = overlay.SetOverlayFromFile(overlayHandle, AssetsPath + "overlay.png");
			if (overlayError != EVROverlayError.None)
			{
				CleanupOVR();
				Debug.WriteLine("OpenVR Overlay Error: " + overlayError.ToString());
				return false;
			}*/

			overlayError = overlay.SetOverlayInputMethod(overlayHandle, VROverlayInputMethod.Mouse);
			if (overlayError != EVROverlayError.None)
			{
				CleanupOVR();
				Debug.WriteLine("OpenVR Overlay Error: " + overlayError.ToString());
				return false;
			}

			overlayTexture = new Texture_t
			{
				eType = ETextureType.OpenGL,
				eColorSpace = EColorSpace.Auto,
				handle = (IntPtr)textureId
			};

			overlayError = overlay.SetOverlayTexture(overlayHandle, ref overlayTexture);
			if (overlayError != EVROverlayError.None)
			{
				CleanupOVR();
				Debug.WriteLine("OpenVR Overlay Error: " + overlayError.ToString());
				return false;
			}

			HmdId = OpenVR.k_unTrackedDeviceIndex_Hmd;

			for (uint i = HmdId + 1; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
			{
				if (vr.IsTrackedDeviceConnected(i))
				{
					ETrackedDeviceClass cls = vr.GetTrackedDeviceClass(i);
					if (cls == ETrackedDeviceClass.Controller)
					{
						ETrackedControllerRole rl = vr.GetControllerRoleForTrackedDeviceIndex(i);
						if (rl == ETrackedControllerRole.LeftHand)
						{
							LeftControllerId = i;
							Debug.WriteLine("Found Left Controller");
						}
						else if (rl == ETrackedControllerRole.RightHand)
						{
							RightControllerId = i;
							Debug.WriteLine("Found Right Controller");
						}

					}
				}
			}

			mat = new HmdMatrix34_t
			{
				m0 = 1,
				m1 = 0,
				m2 = 0,
				m3 = 0f,
				m4 = 0,
				m5 = 1,
				m6 = 0,
				m7 = 0.1f,
				m8 = 0,
				m9 = 0,
				m10 = 1,
				m11 = 0f
			};



			overlayError = overlay.SetOverlayTransformTrackedDeviceRelative(overlayHandle, RightControllerId, ref mat);
			if (overlayError != EVROverlayError.None)
			{
				Debug.WriteLine("Cannot bind overlay to Tracked device.");
				Debug.WriteLine("Error: " + overlayError.ToString());
				CleanupOVR();
				return false;
			}
			overlayError = overlay.SetOverlayWidthInMeters(overlayHandle, 0.2f);
			if (overlayError != EVROverlayError.None)
			{
				Debug.WriteLine("Cannot set overlay size.");
				Debug.WriteLine("Error: " + overlayError.ToString());
				CleanupOVR();
				return false;
			}

			overlayError = overlay.SetOverlayAlpha(overlayHandle, 1);
			if (overlayError != EVROverlayError.None)
			{
				Debug.WriteLine("Cannot set overlay alpha.");
				Debug.WriteLine("Error: " + overlayError.ToString());
				CleanupOVR();
				return false;
			}

			overlayError = overlay.SetOverlayColor(overlayHandle, 1, 1, 1);
			if (overlayError != EVROverlayError.None)
			{
				Debug.WriteLine("Cannot set overlay color.");
				Debug.WriteLine("Error: " + overlayError.ToString());
				CleanupOVR();
				return false;
			}
#if DEBUG
			Debug.WriteLine("OpenVR Startup Complete");
#endif
			return true;
		}

		private void CleanupOVR()
		{
			overlay.DestroyOverlay(overlayHandle);
			vr.AcknowledgeQuit_Exiting();
			OpenVR.Shutdown();
		}

		#endregion

		#region OpenGL
		readonly int textureSizeX = 1024;
		readonly int textureSizeY = 1024;

		int frameBufferId;
		int textureId;
		int depthBufferId;
		int VAO;

		private void DBGProc(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
		{
			Debug.WriteLine("OpenGL: [" + severity.ToString() + "] [" + type.ToString() + "] [" + source.ToString() + "] : " + Marshal.PtrToStringAnsi(message));
		}

		private DebugProc cb;

		private void StartupGL()
		{
#if DEBUG
			EchoWindow = true;
#endif
			render_window = new GameWindow(
				(EchoWindow ? textureSizeX : 640),
				(EchoWindow ? textureSizeY : 480),
				new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(8, 8, 8, 8), 16, 0),
				"OpenVR Cards Subsystem",
				GameWindowFlags.Default,
				DisplayDevice.Default,
				3, 3,
				OpenTK.Graphics.GraphicsContextFlags.Default)
			{
				Visible = EchoWindow,
				VSync = VSyncMode.Off
			};
			
			render_window.MakeCurrent();
#if DEBUG
			cb = DBGProc;
			GL.Enable(EnableCap.DebugOutput);
			GL.DebugMessageCallback(cb, IntPtr.Zero);
#endif
			GL.ClearColor(0, 0, 0, 0);

			VAO = GL.GenVertexArray();
			GL.BindVertexArray(VAO);

			frameBufferId = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);
			
			textureId = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, textureId);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, textureSizeX, textureSizeY, 0, PixelFormat.Rgb, PixelType.Byte, IntPtr.Zero);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			
			depthBufferId = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBufferId);

			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent16, textureSizeX, textureSizeY);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBufferId);

			GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, textureId, 0);

			GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

			FramebufferErrorCode error = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if (error != FramebufferErrorCode.FramebufferComplete)
			{
				throw new Exception("OpenGL Error.");
			}

			GL.Viewport(0, 0, textureSizeX, textureSizeY);
			
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);

			ColorShader.Instance.Compile();
			EchoShader.Instance.Compile();
			RectangleGL.Load();

			EchoScene.Instance.Window = render_window;
			EchoScene.Instance.EchoTexture = textureId;

			CardRectangle.Scale = new Vector2(textureSizeX, 300);
			CardRectangle.Shader = ColorShader.Instance;
			CardScene = new Scene(textureSizeX, textureSizeY);
			CardFont.Load();
#if DEBUG
			Debug.WriteLine("StartupGL Complete");
#endif
		}

		private void CleanupGL()
		{
			ColorShader.Instance.Unload();
			EchoShader.Instance.Unload();
			RectangleGL.Unload();
			GL.DeleteRenderbuffer(depthBufferId);
			GL.DeleteFramebuffer(frameBufferId);
			GL.DeleteTexture(textureId);
		}

		#endregion

		#region CARD

		public bool EchoWindow { get; protected set; } = false;

		public ConcurrentQueue<Card> CardQueue { get; private set; } = new ConcurrentQueue<Card>();

		public Card CurrentCard { get; private set; } = null;
		public DateTime CurrentCardShowTimeComplete;
		private Card currentlyVisibleCard = null;
		public int ShowTimeMS { get; set; } = 500;
		public int HideTimeMS { get; set; } = 250;
		
		private int currentCardSizeY = 0;
		private int currentCardMessagePosY = 0;
		private int currentCardCaptionPosY = 0;
		private Vector2 CaptionPos;
		private Vector2 MessagePos;
		public int CaptionSpacingPX { get; set; } = 20;
		public float CaptionScale { get; set; } = 0.8f;
		public float MessageScale { get; set; } = 0.5f;

		private RectangleGL CardRectangle = new RectangleGL();
		private FontBase CardFont = FontBase.Generate(FontBase.FontAssetsDir + "arial.ttf");
		private Scene CardScene;

		private int MarginH = 100;
		private int MarginV = 50;

		private void DrawCard(Card c)
		{
			if (c == null)
				return;

			if (currentlyVisibleCard == null)
			{
				currentlyVisibleCard = c;
				c.Status = CardStatus.Showning;
				c.Show();
				CurrentCardShowTimeComplete = c.OpeningTime.AddMilliseconds(ShowTimeMS);
				CardRectangle.Color = currentlyVisibleCard.Color;
				currentCardSizeY = MarginV;
				Vector2 captionSize = (CardFont.QuerySize(currentlyVisibleCard.Caption, CaptionScale, textureSizeX - MarginH));
				currentCardSizeY += (int)Math.Ceiling(captionSize.Y);
				currentCardSizeY += CaptionSpacingPX;
				currentCardSizeY += (int)Math.Ceiling(CardFont.QuerySize(currentlyVisibleCard.Message, MessageScale, textureSizeX - MarginH).Y);
				currentCardCaptionPosY = currentCardSizeY / 2 - (MarginV / 2) - (int)Math.Ceiling((CardFont.Height * CaptionScale));
				currentCardMessagePosY = currentCardSizeY / 2 - (MarginV / 2) - (int)Math.Ceiling(captionSize.Y) - CaptionSpacingPX - (int)Math.Ceiling(CardFont.Height * MessageScale);
				CardRectangle.Scale.Y = currentCardSizeY;
				CaptionPos = new Vector2(MarginH / 2 - textureSizeX / 2, currentCardCaptionPosY);
				MessagePos = new Vector2(MarginH / 2 - textureSizeX / 2, currentCardMessagePosY);
			}
			
			double percentage;
			float translation;
			switch (c.Status)
			{
				case CardStatus.Showning:
					percentage = (CurrentCardShowTimeComplete - DateTime.Now).TotalMilliseconds / ShowTimeMS;
					if (percentage < 0)
						percentage = 0;
					if (percentage > 1)
						percentage = 1;
					
					translation = (float)percentage * textureSizeX;
					CardRectangle.Position.X = translation;
					CardRectangle.Draw(CardScene);
					CardFont.DrawText(CardScene, CaptionPos + CardRectangle.Position, currentlyVisibleCard.CaptionColor, currentlyVisibleCard.Caption, CaptionScale, textureSizeX - MarginH);
					CardFont.DrawText(CardScene, MessagePos + CardRectangle.Position, currentlyVisibleCard.MessageColor, currentlyVisibleCard.Message, MessageScale, textureSizeX - MarginH);
					if(percentage == 0)
					{
						currentlyVisibleCard.Status = CardStatus.Visible;
					}
					break;
				case CardStatus.Visible:
					CardRectangle.Position.X = 0;
					CardRectangle.Draw(CardScene);
					CardFont.DrawText(CardScene, CaptionPos + CardRectangle.Position, currentlyVisibleCard.CaptionColor, currentlyVisibleCard.Caption, CaptionScale, textureSizeX - MarginH);
					CardFont.DrawText(CardScene, MessagePos + CardRectangle.Position, currentlyVisibleCard.MessageColor, currentlyVisibleCard.Message, MessageScale, textureSizeX - MarginH);
					if (DateTime.Now > currentlyVisibleCard.EndTime)
					{
						currentlyVisibleCard.Status = CardStatus.Hiding;
					}
					break;
				case CardStatus.Hiding:
					percentage = (DateTime.Now - currentlyVisibleCard.EndTime).TotalMilliseconds / HideTimeMS;
					if (percentage < 0)
						percentage = 0;
					if (percentage > 1)
						percentage = 1;
					translation = (float)percentage * textureSizeX;
					CardRectangle.Position.X = translation;
					CardRectangle.Draw(CardScene);
					CardFont.DrawText(CardScene, CaptionPos + CardRectangle.Position, currentlyVisibleCard.CaptionColor, currentlyVisibleCard.Caption, CaptionScale, textureSizeX - MarginH);
					CardFont.DrawText(CardScene, MessagePos + CardRectangle.Position, currentlyVisibleCard.MessageColor, currentlyVisibleCard.Message, MessageScale, textureSizeX - MarginH);
					if (percentage == 1)
					{
						currentlyVisibleCard.Status = CardStatus.Hidden;
						currentlyVisibleCard = null;
					}
					break;
				case CardStatus.Hidden:
					currentlyVisibleCard = null;
					break;
			}

		}

		#endregion

		private void RenderRoutine()
		{

			StartupGL();
			if (!StartupOVR())
			{
				CleanupGL();
				return;
			}

			EVROverlayError overlayError = overlay.ShowOverlay(overlayHandle);
			if(overlayError != EVROverlayError.None)
			{
				Debug.WriteLine("Cannot show overlay.");
				Debug.WriteLine("Error: " + overlayError.ToString());
				CleanupOVR();
				CleanupGL();
				return;
			}

			while (RenderAlive)
			{
				DateTime begin = DateTime.Now;
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
				if(CurrentCard == null)
				{
					if (CardQueue.TryDequeue(out Card c))
					{
						if (c != null)
						{
							CurrentCard = c;
							continue;
						}
					}
					Thread.Sleep(500);
				}
				else
				{
					DrawCard(CurrentCard);

					GL.Clear(ClearBufferMask.DepthBufferBit);

					if(CurrentCard.Status == CardStatus.Hidden)
					{
						CurrentCard = null;
						Debug.WriteLine("Dropping card.");
					}


					overlayError = overlay.SetOverlayTexture(overlayHandle, ref overlayTexture);
					if (overlayError != EVROverlayError.None)
					{
						Debug.WriteLine("Cannot set texture.");
						Debug.WriteLine("Error: " + overlayError.ToString());
						CleanupOVR();
						CleanupGL();
						return;
					}
				}


				if (EchoWindow)
				{
					GL.ClearColor(1, 1, 1, 0);
					
					GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
					GL.DrawBuffer(DrawBufferMode.Back);
					GL.Viewport(0, 0, render_window.Width, render_window.Height);
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
					GL.Disable(EnableCap.Blend);
					EchoScene.Instance.Draw();
					
					render_window.SwapBuffers();
					
					GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);
					GL.Enable(EnableCap.Blend);
					GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
					GL.Viewport(0, 0, textureSizeX, textureSizeY);
					GL.ClearColor(0, 0, 0, 0);
				}

				VREvent_t ev = new VREvent_t();
				if (overlay.PollNextOverlayEvent(overlayHandle, ref ev, (uint)Marshal.SizeOf(ev.GetType())))
				{
					switch ((EVREventType)ev.eventType)
					{
						case EVREventType.VREvent_OverlayShown:
							Debug.WriteLine("Overlay shown.");
							break;
						case EVREventType.VREvent_OverlayHidden:
							Debug.WriteLine("Overlay Hidden.");
							break;
					}
				}

				if (vr.PollNextEvent(ref ev,(uint)Marshal.SizeOf(ev.GetType()))){
					switch ((EVREventType) ev.eventType)
					{
						case EVREventType.VREvent_Quit:
							StopRender();
							break;
						case EVREventType.VREvent_DriverRequestedQuit:
							StopRender();
							break;
					}
				}
				DateTime end = DateTime.Now;
				double ms = (end - begin).TotalMilliseconds;
				Thread.Sleep((int) (Math.Max(12.0 - ms, 0)));
			}

			CleanupOVR();
			CleanupGL();
		}

		public void StopRenderWait()
		{
			StopRender();
			WaitForThread();
		}

		public void WaitForThread()
		{
			if (Thread.CurrentThread == RenderThread)
				return;
			if (RenderThread != null && RenderThread.IsAlive)
				RenderThread.Join();
		}

		~NotificationManager() {
			StopRender();
			if(RenderThread != null && RenderThread.IsAlive)
				RenderThread.Join();
			render_window.Close();
		}
    }
}
