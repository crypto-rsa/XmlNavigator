﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace XmlNavigator
{
	/// <summary>
	/// The main navigator form
	/// </summary>
	public partial class NavigatorForm : Form
	{
		#region Constants

		/// <summary>
		/// The maximum depth of nodes in the tree view which are generated initially
		/// </summary>
		private const int MaximumExpandedDepth = 1;

		#endregion

		#region Data Members

		/// <summary>
		/// The path to the current file
		/// </summary>
		private string _path;

		/// <summary>
		/// The input text
		/// </summary>
		private string _text;

		/// <summary>
		/// The parser for the current file
		/// </summary>
		private XmlParser _parser;

		/// <summary>
		/// The task which generates the tree nodes (returns the root node)
		/// </summary>
		private Task<TreeNode> _generateTreeTask;

		/// <summary>
		/// The cancellation token souce for the tree generation task
		/// </summary>
		private CancellationTokenSource _generateTreeCancellationTokenSource;

		/// <summary>
		/// An array of strings to filter the nodes by
		/// </summary>
		private string[] _filterItems;

		#endregion

		#region Constructors

		/// <summary>
		/// The default constructor
		/// </summary>
		public NavigatorForm()
		{
			InitializeComponent();
		}

		#endregion

		#region Overrides

		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );

			Reload();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reloads the data using the current file
		/// </summary>
		public void Reload()
		{
			if( !this.Visible )
				return;

			_path = NppPluginNET.PluginBase.GetFullCurrentFileName();
			_text = NppPluginNET.PluginBase.GetCurrentFileText();

			RunFillTreeTask();
		}

		/// <summary>
		/// Runs the task which fills the tree with the nodes from the current document
		/// </summary>
		private void RunFillTreeTask()
		{
			if( _generateTreeTask != null && _generateTreeTask.Status == TaskStatus.Running )
			{
				_generateTreeCancellationTokenSource.Cancel();
			}

			_generateTreeCancellationTokenSource = new CancellationTokenSource();

			_generateTreeTask = Task<TreeNode>.Factory.StartNew( StartGenerating, _generateTreeCancellationTokenSource.Token );
			_generateTreeTask.ContinueWith( UpdateUIOnTaskCompleted );
		}

		/// <summary>
		/// Updates the UI and start generating the tree
		/// </summary>
		/// <returns>The root node of the generated tree</returns>
		private TreeNode StartGenerating()
		{
			Invoke( (Action) UpdateUIBeforeGenerating );

			return GenerateTree();
		}

		/// <summary>
		/// Generates the whole tree
		/// </summary>
		/// <returns>The root node of the tree</returns>
		private TreeNode GenerateTree()
		{
			_parser = new XmlParser( _text );
			if( _parser.RootNode == null )
				return null;

			var rootNode = GenerateTree( _parser.RootNode, MaximumExpandedDepth );

			if( rootNode != null )
			{
				rootNode.Expand();
			}

			return rootNode;
		}

		/// <summary>
		/// Generates a subtree
		/// </summary>
		/// <param name="data">The data representing a node</param>
		/// <param name="maxDepth">The maximum depth of the expanded nodes</param>
		/// <returns>The tree node corresponding to <paramref name="data"/></returns>
		private TreeNode GenerateTree( NodeData data, int maxDepth )
		{
			_generateTreeCancellationTokenSource.Token.ThrowIfCancellationRequested();

			if( !data.SubtreeMatchesFilter( _filterItems ) )
				return null;

			var node = new TreeNode( data.DisplayName ) { Tag = data, ToolTipText = data.Comment };
			var childNodes = GetChildNodes( data, maxDepth );

			if( childNodes != null )
			{
				node.Nodes.AddRange( childNodes );
			}

			return node;
		}

		/// <summary>
		/// Returns an array of tree nodes representing the child nodes of a node
		/// </summary>
		/// <param name="data">The data representing a node to get the child nodes of</param>
		/// <param name="maxDepth">The maximum depth of the expanded nodes</param>
		/// <returns>The array of tree nodes representing child nodes of <paramref name="data"/></returns>
		private TreeNode[] GetChildNodes( NodeData data, int maxDepth )
		{
			TreeNode[] nodeArray = null;

			var childNodes = data.ChildNodes.Where( n => n.SubtreeMatchesFilter( _filterItems ) ).ToList();

			if( data.Depth < maxDepth )
			{
				data.TreeNodeExpanded = true;

				nodeArray = new TreeNode[childNodes.Count];

				for( int i = 0; i < childNodes.Count; i++ )
				{
					nodeArray[i] = GenerateTree( childNodes[i], maxDepth );
				}
			}
			else if( childNodes.Any() )
			{
				// add a dummy node where child nodes exist to allow expanding
				nodeArray = new TreeNode[] { new TreeNode( "<dummy>" ) };
			}

			return nodeArray;
		}

		/// <summary>
		/// Updates the form UI before the tree is generated
		/// </summary>
		private void UpdateUIBeforeGenerating()
		{
			treeViewNodes.BeginUpdate();

			try
			{
				treeViewNodes.Nodes.Clear();
			}
			finally
			{
				treeViewNodes.EndUpdate();
			}
		}

		/// <summary>
		/// Updates the form UI after node generating task completes
		/// </summary>
		/// <param name="task">The finished task</param>
		private void UpdateUIOnTaskCompleted( Task<TreeNode> task )
		{
			if( task != _generateTreeTask )
				return;

			if( task.Status != TaskStatus.RanToCompletion || task.Result == null )
				return;

			Invoke( new Action( () => treeViewNodes.Nodes.Add( task.Result ) ) );
		}

		#endregion

		#region Event Handlers

		#region Tree View

		private void treeViewNodes_AfterSelect( object sender, TreeViewEventArgs e )
		{
			var data = e.Node?.Tag as NodeData;
			if( data == null )
				return;

			Main.GoToPosition( data.NodeExtent.Start );
		}

		private void treeViewNodes_BeforeExpand( object sender, TreeViewCancelEventArgs e )
		{
			var data = e.Node?.Tag as NodeData;
			if( data == null )
				return;

			if( data.TreeNodeExpanded )
				return;

			treeViewNodes.BeginUpdate();

			try
			{
				e.Node.Nodes.Clear();
				e.Node.Nodes.AddRange( GetChildNodes( data, data.Depth + 1 ) );
			}
			finally
			{
				treeViewNodes.EndUpdate();
			}
		}

		private void treeViewNodes_NodeMouseClick( object sender, TreeNodeMouseClickEventArgs e )
		{
			if( e.Button == MouseButtons.Right )
			{
				treeViewNodes.SelectedNode = e.Node;
			}
		}

		#endregion

		#region Node Context Menu

		private void nodeContextMenu_Opening( object sender, CancelEventArgs e )
		{
			var nodeData = treeViewNodes.SelectedNode?.Tag as NodeData;
			bool isValidNode = nodeData != null;
			bool hasContent = isValidNode && !nodeData.IsEmpty;

			toolStripMenuItemGoToNodeStart.Enabled = isValidNode;
			toolStripMenuItemGoToNodeEnd.Enabled = isValidNode;
			toolStripMenuItemSelectNode.Enabled = isValidNode;
			toolStripMenuItemNodeContentStart.Enabled = hasContent;
			toolStripMenuItemNodeContentEnd.Enabled = hasContent;
			toolStripMenuItemSelectContent.Enabled = hasContent;
		}

		private void toolStripMenuItemGoToNodeStart_Click( object sender, EventArgs e )
		{
			var nodeData = treeViewNodes.SelectedNode?.Tag as NodeData;
			if( nodeData == null )
				return;

			Main.GoToPosition( nodeData.NodeExtent.Start );
		}

		private void toolStripMenuItemGoToNodeEnd_Click( object sender, EventArgs e )
		{
			var nodeData = treeViewNodes.SelectedNode?.Tag as NodeData;
			if( nodeData == null )
				return;

			Main.GoToPosition( nodeData.NodeExtent.End );
		}

		private void toolStripMenuItemNodeContentStart_Click( object sender, EventArgs e )
		{
			var nodeData = treeViewNodes.SelectedNode?.Tag as NodeData;
			if( nodeData == null )
				return;

			if( nodeData.IsEmpty )
				return;

			Main.GoToPosition( nodeData.ContentExtent.Start );
		}

		private void toolStripMenuItemNodeContentEnd_Click( object sender, EventArgs e )
		{
			var nodeData = treeViewNodes.SelectedNode?.Tag as NodeData;
			if( nodeData == null )
				return;

			if( nodeData.IsEmpty )
				return;

			Main.GoToPosition( nodeData.ContentExtent.End );
		}

		private void toolStripMenuItemSelectNode_Click( object sender, EventArgs e )
		{
			var nodeData = treeViewNodes.SelectedNode?.Tag as NodeData;
			if( nodeData == null )
				return;

			Main.SetSelection( nodeData.NodeExtent.Start, nodeData.NodeExtent.End );
		}

		private void toolStripMenuItemSelectContent_Click( object sender, EventArgs e )
		{
			var nodeData = treeViewNodes.SelectedNode?.Tag as NodeData;
			if( nodeData == null )
				return;

			if( nodeData.IsEmpty )
				return;

			Main.SetSelection( nodeData.ContentExtent.Start, nodeData.ContentExtent.End );
		}

		#endregion

		#region Filter

		private void textBoxFilter_TextChanged( object sender, EventArgs e )
		{
			if( string.IsNullOrWhiteSpace( textBoxFilter.Text ) )
			{
				_filterItems = null;
			}
			else
			{
				_filterItems = textBoxFilter.Text.Split( ' ' );
			}

			Reload();
		}

		private void buttonResetFilter_Click( object sender, EventArgs e )
		{
			textBoxFilter.Text = string.Empty;
		}

		#endregion

		#endregion
	}
}
