using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordRelay;

namespace OVR_Relay_application
{
	internal class OVRNotifyIcon
	{
		protected NotifyIcon icon;
		protected Relay relay;
		protected MenuItem amountLabel;

		public OVRNotifyIcon(Relay r)
		{
			relay = r;
			icon = new NotifyIcon();
			icon.Icon = new System.Drawing.Icon(Application.StartupPath + @"\notify.ico");
			amountLabel = new MenuItem("Monitored channels: " + r.MonitoredChannelsNumber)
			{
				Enabled = false
			};
			MenuItem closeBtn = new MenuItem("Close");
			closeBtn.Click += CloseBtn_Click;
			icon.ContextMenu = new ContextMenu(new MenuItem[]{ amountLabel, closeBtn });
			icon.Text = "OpenVR Discord Relay";
			r.OnChannelAmountChanged += R_OnChannelAmountChanged;
			icon.Visible = false;
		}

		public void Dispay()
		{
			icon.Visible = true;
		}

		public void Hide()
		{
			icon.Visible = false;
		}

		private void R_OnChannelAmountChanged(int newAmount)
		{
			amountLabel.Text = "Monitored channels: " + newAmount;
		}

		private void CloseBtn_Click(object sender, EventArgs e)
		{
			icon.Visible = false;
			Application.Exit();
		}
	}
}
