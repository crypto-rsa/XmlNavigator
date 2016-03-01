using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XmlNavigator
{
	/// <summary>
	/// Represents the About dialog
	/// </summary>
	public partial class AboutForm : Form
	{
		#region Constructors

		/// <summary>
		/// The default constructor
		/// </summary>
		public AboutForm()
		{
			InitializeComponent();

			labelVersion.Text = GetVersionString();
			pictureBoxIcon.Image = SystemIcons.Information.ToBitmap();
		}

		#endregion

		#region Overrides

		protected override bool ProcessDialogKey( Keys keyData )
		{
			if( keyData == Keys.Escape )
			{
				Close();
				return true;
			}

			return base.ProcessDialogKey( keyData );
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns the version string
		/// </summary>
		/// <returns></returns>
		private string GetVersionString()
		{
			var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

			return string.Join( ".", version.Major, version.Minor, version.Build );
		}

		#endregion

		#region Event Handlers

		private void linkLabelAuthor_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			System.Diagnostics.Process.Start( @"mailto:pkotrc@gmail.com" );
		}

		private void linkLabelCode_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			System.Diagnostics.Process.Start( @"https://github.com/crypto-rsa/XmlNavigator" );
		}

		#endregion
	}
}
