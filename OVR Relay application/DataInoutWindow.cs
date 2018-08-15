using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OVR_Relay_application
{
	public partial class DataInoutWindow : Form
	{
		public DataInoutWindow()
		{
			InitializeComponent();
		}

		public string BotToken
		{
			get => mtxbToken.Text;
		}

		public string Username
		{
			get => mtxbName.Text;
		}

		public string Discriminator
		{
			get => mtxbDiscriminator.Text;
		}

		private void DataInoutWindow_Load(object sender, EventArgs e)
		{

		}
	}
}
