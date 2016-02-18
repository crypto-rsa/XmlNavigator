using System;
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

		/// <summary>
		/// The task which generates the tree nodes (returns the root node)
		/// </summary>
		private Task<TreeNode> _generateTreeTask;

		/// <summary>
		/// The cancellation token souce for the tree generation task
		/// </summary>
		private CancellationTokenSource _generateTreeCancellationTokenSource;

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
			_parser = new XmlParser( _path );
			if( _parser.RootNode == null )
				return null;

			var rootNode = GenerateTree( _parser.RootNode, null );

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
		/// <param name="nodeCollection">The collection to add the node to</param>
		/// <returns>The tree node corresponding to <paramref name="data"/></returns>
		private TreeNode GenerateTree( NodeData data, TreeNodeCollection nodeCollection )
		{
			_generateTreeCancellationTokenSource.Token.ThrowIfCancellationRequested();

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

		private void RefreshForm()
		{
			Main.UpdateForm( this.Handle );
		}
	}
}
