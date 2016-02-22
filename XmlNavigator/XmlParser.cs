using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XmlNavigator
{
	/// <summary>
	/// Represents an extent of a node in the source string
	/// </summary>
	public class NodeExtent
	{
		#region Properties

		/// <summary>
		/// Gets or sets the start position of the extent
		/// </summary>
		public int Start { get; set; } = -1;

		/// <summary>
		/// Gets or sets the end position of the extent
		/// </summary>
		public int End { get; set; } = -1;

		#endregion
	}

	/// <summary>
	/// Contains information about a single XML node
	/// </summary>
	public class NodeData
	{
		#region Data Members

		/// <summary>
		/// The name of this node
		/// </summary>
		private string _name;

		/// <summary>
		/// The parent node of this node
		/// </summary>
		private NodeData _parentNode;

		/// <summary>
		/// The list of child nodes
		/// </summary>
		private List<NodeData> _childNodes;

		/// <summary>
		/// The index of the node in its parent's child node list
		/// </summary>
		private int _index;

		/// <summary>
		/// The extent of the full node including the opening and closing tags
		/// </summary>
		private NodeExtent _nodeExtent;

		/// <summary>
		/// The extent of the node content
		/// </summary>
		private NodeExtent _contentExtent;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a node
		/// </summary>
		/// <param name="name">The name of this object</param>
		public NodeData( string name )
		{
			_name = name;
			_childNodes = new List<NodeData>();
			_nodeExtent = new NodeExtent();
			_contentExtent = new NodeExtent();
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return _name;
		}
		
		#endregion

		#region Methods

		/// <summary>
		/// Adds a child node
		/// </summary>
		/// <param name="name">The name of the child node</param>
		/// <returns>The newly created child node</returns>
		public NodeData AddChild( string name )
		{
			var childNode = new NodeData( name );
			AddChild( childNode );

			return childNode;
		}

		/// <summary>
		/// Adds an existing node to this node
		/// </summary>
		/// <param name="childNode">The node to add</param>
		public void AddChild( NodeData childNode )
		{
			childNode.Parent = this;
			childNode.Index = _childNodes.Count;

			_childNodes.Add( childNode );
		}

		/// <summary>
		/// Inserts an existing node at the given position
		/// </summary>
		/// <param name="childNode">The node to insert</param>
		/// <param name="index">The position to insert the node at</param>
		public void InsertChild( NodeData childNode, int index )
		{
			if( index < 0 || index > _childNodes.Count )
				throw new ArgumentOutOfRangeException( "index" );

			childNode.Parent = this;
			_childNodes.Insert( index, childNode );

			// update the indices
			for( int i = index; i < _childNodes.Count; i++ )
			{
				_childNodes[i].Index = i;
			}
		}

		/// <summary>
		/// Removes the given node
		/// </summary>
		/// <param name="childNode">The node to remove</param>
		public void RemoveChild( NodeData childNode )
		{
			int index = _childNodes.FindIndex( d => d == childNode );
			if( index >= 0 )
			{
				RemoveChildAt( index );
			}
		}

		/// <summary>
		/// Removes a child node at the given position
		/// </summary>
		/// <param name="index">The position to remove the node at</param>
		public void RemoveChildAt( int index )
		{
			if( index < 0 || index >= _childNodes.Count )
				throw new ArgumentOutOfRangeException( "index" );

			_childNodes.RemoveAt( index );

			// update the indices
			for( int i = index; i < _childNodes.Count; i++ )
			{
				_childNodes[i].Index = i;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the parent node of this node
		/// </summary>
		public NodeData Parent
		{
			get { return _parentNode; }
			private set { _parentNode = value; }
		}

		/// <summary>
		/// Gets the collection of child nodes
		/// </summary>
		public IEnumerable<NodeData> ChildNodes
		{
			get { return _childNodes.AsReadOnly(); }
		}

		/// <summary>
		/// Gets or sets the index of the node in its parent's child node list
		/// </summary>
		public int Index
		{
			get { return _index; }
			private set { _index = value; }
		}

		/// <summary>
		/// Gets the extent of the full node (including the opening and closing tags)
		/// </summary>
		public NodeExtent NodeExtent
		{
			get { return _nodeExtent; }
		}

		/// <summary>
		/// Gets the extent of the node content
		/// </summary>
		public NodeExtent ContentExtent
		{
			get { return _contentExtent; }
		}

		#endregion
	}

	/// <summary>
	/// Parses an XML file and creates its data model
	/// </summary>
	public class XmlParser
	{
		#region Data Members

		/// <summary>
		/// The full path to the file to parse
		/// </summary>
		private string _path;

		/// <summary>
		/// The reader for the XML
		/// </summary>
		private XmlReader _reader;

		/// <summary>
		/// An object representing the root node of the XML document
		/// </summary>
		private NodeData _rootNode;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates the parser for a specific path
		/// </summary>
		/// <param name="path">The full path to the file to parse</param>
		public XmlParser( string path )
		{
			_path = path;

			CreateReader();
			Parse();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Parses the current document
		/// </summary>
		public void Parse()
		{
			_rootNode = null;

			if( _reader == null )
				return;

			try
			{
				ReadNodes();
			}
			catch( Exception e )
			{
				System.Diagnostics.Debug.WriteLine( e.Message );
			}
		}

		/// <summary>
		/// Reads the nodes using the current <see cref="_reader"/>
		/// </summary>
		private void ReadNodes()
		{
			var parentNodes = new Dictionary<int, NodeData>();

			while( _reader.Read() )
			{
				if( _reader.NodeType != XmlNodeType.Element )
					continue;

				NodeData current;
				NodeData parent;

				if( parentNodes.TryGetValue( _reader.Depth, out parent ) )
				{
					current = parent.AddChild( _reader.LocalName );
				}
				else
				{
					current = new NodeData( _reader.LocalName );
				}

				if( _rootNode == null && _reader.Depth == 0 )
				{
					_rootNode = current;
				}

				parentNodes[_reader.Depth + 1] = current;
			}
		}

		/// <summary>
		/// Creates the XML reader for the current file
		/// </summary>
		private XmlReader CreateReader()
		{
			try
			{
				_reader = XmlReader.Create( _path );
			}
			catch( Exception e )
			{
				System.Diagnostics.Debug.WriteLine( e.Message );
			}

			return null;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the root node of the document
		/// </summary>
		public NodeData RootNode
		{
			get { return _rootNode; }
		}

		#endregion
	}
}
