using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordRelay;
using DiscordRelay.Config;
using OVRCards;
using OVRCards.Cards;
using OVR_Relay_application.OVRCServiceReference;

namespace OVR_Relay_application
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MyCustomAppContext());
		}
	}

	internal class MyCustomAppContext : ApplicationContext
	{
		Relay r;
		OVRNotifyIcon icn;
		OVRCardServiceClient mgr;
		
		bool isExiting = false;

		public MyCustomAppContext()
		{
			mgr = new OVRCardServiceClient();
			mgr.Connect();
			r = new Relay();
			string token;
			string username;
			string discriminator;
			if (!File.Exists(r.configPath))
			{
				DataInoutWindow w = new DataInoutWindow();
				DialogResult res = w.ShowDialog();
				switch (res)
				{
					case DialogResult.OK:
						token = w.BotToken;
						username = w.Username;
						discriminator = w.Discriminator;
						break;
					case DialogResult.Cancel:
						Application.Exit();
						return;
					default:
						Application.Exit();
						return;
				}
			}
			else
			{
				ConfigurationFile f = ConfigurationFile.ReadFromFile(r.configPath);
				token = f.BotToken;
				username = f.OwnerUsername;
				discriminator = f.OwnerDiscriminator;
			}
			r.StartDiscord(token, username, discriminator, "^ovr ");
			r.OnDirectMessage += (usr_name, message) =>
			{
				mgr.PostCard(new OVRCard {
					Caption = usr_name,
					Text = message,
					R = 0.3f, G = 0.3f, B = 0.35f,
					DurationMS = 5000,
					CaptionR =  0.8f, CaptionG =  0, CaptionB = 0 });
			};
			r.OnGuildMessage += (guild_name, channel_name, user_name, message) =>
			{
				mgr.PostCard(new OVRCard {
					Caption = user_name + "@" + channel_name,
					Text = message,
					R = 0.5f, G = 0.3f, B = 0.3f,
					DurationMS = 5000,
					CaptionR = 0f, CaptionG = 0.8f, CaptionB = 0f } );
			};
			r.OnDisconnect += () =>
			{
				if(!isExiting)
					Application.Exit();
			};
			icn = new OVRNotifyIcon(r);
			icn.Dispay();
			Application.ApplicationExit += Application_ApplicationExit;
		}

		private void Application_ApplicationExit(object sender, EventArgs e)
		{
			mgr.Disconnect();
			isExiting = true;
			r.StopDiscordAndWait();
			//mgr.StopRenderWait();
			icn.Hide();
		}
	}
}
