namespace OVR_Relay_application
{
	partial class DataInoutWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblHeader = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.mtxbToken = new System.Windows.Forms.MaskedTextBox();
			this.mtxbName = new System.Windows.Forms.MaskedTextBox();
			this.mtxbDiscriminator = new System.Windows.Forms.MaskedTextBox();
			this.lblToken = new System.Windows.Forms.Label();
			this.lblOwnerName = new System.Windows.Forms.Label();
			this.lblOwnerDiscriminator = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblHeader
			// 
			this.lblHeader.AutoSize = true;
			this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblHeader.Location = new System.Drawing.Point(12, 9);
			this.lblHeader.Name = "lblHeader";
			this.lblHeader.Size = new System.Drawing.Size(180, 20);
			this.lblHeader.TabIndex = 0;
			this.lblHeader.Text = "First time setup data:";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(653, 134);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(572, 134);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "&Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// mtxbToken
			// 
			this.mtxbToken.Location = new System.Drawing.Point(122, 49);
			this.mtxbToken.Name = "mtxbToken";
			this.mtxbToken.Size = new System.Drawing.Size(606, 20);
			this.mtxbToken.TabIndex = 3;
			// 
			// mtxbName
			// 
			this.mtxbName.Location = new System.Drawing.Point(122, 75);
			this.mtxbName.Name = "mtxbName";
			this.mtxbName.Size = new System.Drawing.Size(606, 20);
			this.mtxbName.TabIndex = 4;
			// 
			// mtxbDiscriminator
			// 
			this.mtxbDiscriminator.Location = new System.Drawing.Point(122, 101);
			this.mtxbDiscriminator.Name = "mtxbDiscriminator";
			this.mtxbDiscriminator.Size = new System.Drawing.Size(606, 20);
			this.mtxbDiscriminator.TabIndex = 5;
			// 
			// lblToken
			// 
			this.lblToken.AutoSize = true;
			this.lblToken.Location = new System.Drawing.Point(12, 52);
			this.lblToken.Name = "lblToken";
			this.lblToken.Size = new System.Drawing.Size(60, 13);
			this.lblToken.TabIndex = 6;
			this.lblToken.Text = "Bot Token:";
			// 
			// lblOwnerName
			// 
			this.lblOwnerName.AutoSize = true;
			this.lblOwnerName.Location = new System.Drawing.Point(12, 78);
			this.lblOwnerName.Name = "lblOwnerName";
			this.lblOwnerName.Size = new System.Drawing.Size(72, 13);
			this.lblOwnerName.TabIndex = 7;
			this.lblOwnerName.Text = "Owner Name:";
			// 
			// lblOwnerDiscriminator
			// 
			this.lblOwnerDiscriminator.AutoSize = true;
			this.lblOwnerDiscriminator.Location = new System.Drawing.Point(12, 104);
			this.lblOwnerDiscriminator.Name = "lblOwnerDiscriminator";
			this.lblOwnerDiscriminator.Size = new System.Drawing.Size(104, 13);
			this.lblOwnerDiscriminator.TabIndex = 8;
			this.lblOwnerDiscriminator.Text = "Owner Discriminator:";
			// 
			// DataInoutWindow
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(740, 169);
			this.Controls.Add(this.lblOwnerDiscriminator);
			this.Controls.Add(this.lblOwnerName);
			this.Controls.Add(this.lblToken);
			this.Controls.Add(this.mtxbDiscriminator);
			this.Controls.Add(this.mtxbName);
			this.Controls.Add(this.mtxbToken);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblHeader);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataInoutWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "DiscordOVR First Time Setup";
			this.Load += new System.EventHandler(this.DataInoutWindow_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblHeader;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.MaskedTextBox mtxbToken;
		private System.Windows.Forms.MaskedTextBox mtxbName;
		private System.Windows.Forms.MaskedTextBox mtxbDiscriminator;
		private System.Windows.Forms.Label lblToken;
		private System.Windows.Forms.Label lblOwnerName;
		private System.Windows.Forms.Label lblOwnerDiscriminator;
	}
}