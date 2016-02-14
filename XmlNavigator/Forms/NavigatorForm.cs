using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace XmlNavigator
{
	/// <summary>
	/// The main navigator form
	/// </summary>
	public partial class NavigatorForm : Form
	{
		#region Data Members

		/// <summary>
		/// The path to the current file
		/// </summary>
		private string _path;

		/// <summary>
		/// The parser for the current file
		/// </summary>
		private XmlParser _parser;

		/// <summary>
		/// A mapping from the parsed nodes to the tree nodes
		/// </summary>
		private Dictionary<XmlNode, TreeNode> _nodeDictionary;

		#endregion

		#region Constructors

		/// <summary>
		/// The default constructor
		/// </summary>
		public NavigatorForm()
		{
			InitializeComponent();

			Reload();
		}

		#endregion

		#region Methods

		// TEST
		public void AddNode( string text )
		{
			treeViewNodes.Nodes.Add( text );
		}

		public void Reload()
		{
			_path = NppPluginNET.PluginBase.GetFullCurrentFileName();
			_parser = new XmlParser( _path );

			FillTree();
		}

		private void FillTree()
		{
			treeViewNodes.BeginUpdate();

			treeViewNodes.Nodes.Clear();

			if( _parser.RootNode == null )
				return;

			try
			{
				var treeRoot = GenerateTree( _parser.RootNode, null );
				treeViewNodes.Nodes.Add( treeRoot );
			}
			finally
			{
				treeViewNodes.EndUpdate();
			}

			RefreshForm();
		}

		#endregion

		private void AddNodeToTree( NodeData data, TreeNodeCollection nodeCollection )
		{
			var node = new TreeNode( data.ToString() );

			nodeCollection.Add( node );

			foreach( var childData in data.ChildNodes )
			{
				AddNodeToTree( childData, node.Nodes );
			}
		}

		private TreeNode GenerateTree( NodeData data, TreeNodeCollection nodeCollection )
		{
			var node = new TreeNode( data.ToString() );

			if( nodeCollection != null )
			{
				nodeCollection.Add( node );
			}

			foreach( var childData in data.ChildNodes )
			{
				GenerateTree( childData, node.Nodes );
			}

			return node;
		}

		private void RefreshForm()
		{
			Main.UpdateForm( this.Handle );
		}
	}
}
