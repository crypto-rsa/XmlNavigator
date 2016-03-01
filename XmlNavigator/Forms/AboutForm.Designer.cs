namespace XmlNavigator
{
	partial class AboutForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.labelVersionDescription = new System.Windows.Forms.Label();
			this.labelAuthor = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.linkLabelAuthor = new System.Windows.Forms.LinkLabel();
			this.linkLabelCode = new System.Windows.Forms.LinkLabel();
			this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// labelVersionDescription
			// 
			this.labelVersionDescription.AutoSize = true;
			this.labelVersionDescription.Location = new System.Drawing.Point(67, 9);
			this.labelVersionDescription.Name = "labelVersionDescription";
			this.labelVersionDescription.Size = new System.Drawing.Size(45, 13);
			this.labelVersionDescription.TabIndex = 0;
			this.labelVersionDescription.Text = "Version:";
			// 
			// labelAuthor
			// 
			this.labelAuthor.AutoSize = true;
			this.labelAuthor.Location = new System.Drawing.Point(67, 32);
			this.labelAuthor.Name = "labelAuthor";
			this.labelAuthor.Size = new System.Drawing.Size(41, 13);
			this.labelAuthor.TabIndex = 0;
			this.labelAuthor.Text = "Author:";
			// 
			// labelVersion
			// 
			this.labelVersion.AutoSize = true;
			this.labelVersion.Location = new System.Drawing.Point(118, 9);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(74, 13);
			this.labelVersion.TabIndex = 0;
			this.labelVersion.Text = "<placeholder>";
			// 
			// linkLabelAuthor
			// 
			this.linkLabelAuthor.AutoSize = true;
			this.linkLabelAuthor.Location = new System.Drawing.Point(118, 32);
			this.linkLabelAuthor.Name = "linkLabelAuthor";
			this.linkLabelAuthor.Size = new System.Drawing.Size(62, 13);
			this.linkLabelAuthor.TabIndex = 1;
			this.linkLabelAuthor.TabStop = true;
			this.linkLabelAuthor.Text = "Pavel Kotrč";
			this.linkLabelAuthor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAuthor_LinkClicked);
			// 
			// linkLabelCode
			// 
			this.linkLabelCode.AutoSize = true;
			this.linkLabelCode.LinkArea = new System.Windows.Forms.LinkArea(18, 6);
			this.linkLabelCode.Location = new System.Drawing.Point(70, 55);
			this.linkLabelCode.Name = "linkLabelCode";
			this.linkLabelCode.Size = new System.Drawing.Size(131, 17);
			this.linkLabelCode.TabIndex = 1;
			this.linkLabelCode.TabStop = true;
			this.linkLabelCode.Text = "Code available on Github";
			this.linkLabelCode.UseCompatibleTextRendering = true;
			this.linkLabelCode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCode_LinkClicked);
			// 
			// pictureBoxIcon
			// 
			this.pictureBoxIcon.Location = new System.Drawing.Point(12, 12);
			this.pictureBoxIcon.Name = "pictureBoxIcon";
			this.pictureBoxIcon.Size = new System.Drawing.Size(32, 32);
			this.pictureBoxIcon.TabIndex = 2;
			this.pictureBoxIcon.TabStop = false;
			// 
			// AboutForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(218, 88);
			this.Controls.Add(this.pictureBoxIcon);
			this.Controls.Add(this.linkLabelCode);
			this.Controls.Add(this.linkLabelAuthor);
			this.Controls.Add(this.labelAuthor);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.labelVersionDescription);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "AboutForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "XML Navigator";
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelVersionDescription;
		private System.Windows.Forms.Label labelAuthor;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.LinkLabel linkLabelAuthor;
		private System.Windows.Forms.LinkLabel linkLabelCode;
		private System.Windows.Forms.PictureBox pictureBoxIcon;
	}
}