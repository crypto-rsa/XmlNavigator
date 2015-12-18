using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NppPluginNET;

namespace XmlNavigator
{
	/// <summary>
	/// The main plugin class
	/// </summary>
	public class Main
	{
		#region Constants

		/// <summary>
		/// The plugin name
		/// </summary>
		internal const string PluginName = "XmlNavigator";

		#endregion

		#region Data Members

		/// <summary>
		/// The path to the configuration file
		/// </summary>
		private static string _iniFilePath = null;

		/// <summary>
		/// The main navigator form
		/// </summary>
		private static NavigatorForm _navigatorForm = null;

		/// <summary>
		/// The ID of the 'Show Navigator' command
		/// </summary>
		private static int _idNavigatorForm;

		#endregion

		#region Methods

		/// <summary>
		/// Initializes the menu commands
		/// </summary>
		internal static void CommandMenuInit()
		{
			// initialize the configuration file
			StringBuilder sbIniFilePath = new StringBuilder( Win32.MAX_PATH );
			Win32.SendMessage( PluginBase.nppData._nppHandle, NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath );
			_iniFilePath = sbIniFilePath.ToString();
			if( !Directory.Exists( _iniFilePath ) )
			{
				Directory.CreateDirectory( _iniFilePath );
			}

			_iniFilePath = Path.Combine( _iniFilePath, PluginName + ".ini" );

			// initialize the menu commands
			_idNavigatorForm = 0;
			PluginBase.SetCommand( 0, Properties.Resources.commandShowNavigator, ShowNavigatorCommand );
			PluginBase.SetCommand( 1, Properties.Resources.commandAbout, AboutCommand );
		}

		/// <summary>
		/// Adds the toolbar icons
		/// </summary>
		internal static void SetToolBarIcon()
		{
			toolbarIcons tbIcons = new toolbarIcons();
			tbIcons.hToolbarBmp = Properties.Resources.star.GetHbitmap();
			IntPtr pTbIcons = Marshal.AllocHGlobal( Marshal.SizeOf( tbIcons ) );
			Marshal.StructureToPtr( tbIcons, pTbIcons, false );
			Win32.SendMessage( PluginBase.nppData._nppHandle, NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[_idNavigatorForm]._cmdID, pTbIcons );
			Marshal.FreeHGlobal( pTbIcons );
		}

		/// <summary>
		/// Performs cleanup
		/// </summary>
		internal static void PluginCleanUp()
		{
			// TODO write the configuration using Win32.WritePrivateProfileString
		}

		/// <summary>
		/// Shows the About message
		/// </summary>
		internal static void AboutCommand()
		{
			MessageBox.Show( "Hello N++!" );
		}

		/// <summary>
		/// Shows the Navigator window
		/// </summary>
		internal static void ShowNavigatorCommand()
		{
			if( _navigatorForm == null )
			{
				_navigatorForm = new NavigatorForm();

				NppTbData nppTbData = new NppTbData();
				nppTbData.hClient = _navigatorForm.Handle;
				nppTbData.pszName = "XML Navigator";
				nppTbData.dlgID = _idNavigatorForm;
				nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
				nppTbData.pszModuleName = Main.PluginName;
				IntPtr ptrNppTbData = Marshal.AllocHGlobal( Marshal.SizeOf( nppTbData ) );
				Marshal.StructureToPtr( nppTbData, ptrNppTbData, false );

				Win32.SendMessage( PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMREGASDCKDLG, 0, ptrNppTbData );
			}
			else
			{
				Win32.SendMessage( PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, _navigatorForm.Handle );
			}
		}

		#endregion
	}
}