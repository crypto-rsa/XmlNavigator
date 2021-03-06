﻿using System;
using System.Collections.Generic;
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
		internal const string PluginName = "XML Navigator";

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
			tbIcons.hToolbarBmp = Properties.Resources.XmlNavigator.GetHbitmap();
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
			using( var form = new AboutForm() )
			{
				var handle = PluginBase.GetCurrentScintilla();
				var window = NativeWindow.FromHandle( handle );

				form.ShowDialog( window );
			}
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

		internal static void ReloadNavigatorTree()
		{
			if( _navigatorForm == null )
				return;

			_navigatorForm.Reload();
		}

		/// <summary>
		/// Sets a new selection in the Scintilla window
		/// </summary>
		/// <param name="startPosition">The start position of the selection</param>
		/// <param name="endPosition">The end position of the selection</param>
		internal static void SetSelection( int startPosition, int endPosition )
		{
			var scintilla = PluginBase.GetCurrentScintilla();

			Win32.SendMessage( scintilla, SciMsg.SCI_SETSELECTIONSTART, startPosition, 0 );
			Win32.SendMessage( scintilla, SciMsg.SCI_SETSELECTIONEND, endPosition, 0 );
			Win32.SendMessage( scintilla, SciMsg.SCI_SCROLLCARET, 0, 0 );
		}

		/// <summary>
		/// Moves a carent in the Scintilla window
		/// </summary>
		/// <param name="position">The position to move the caret to</param>
		internal static void GoToPosition( int position )
		{
			var scintilla = PluginBase.GetCurrentScintilla();

			Win32.SendMessage( scintilla, SciMsg.SCI_GOTOPOS, position, 0 );
		}

		#endregion
	}
}