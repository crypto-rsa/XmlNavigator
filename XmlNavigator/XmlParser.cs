using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XmlNavigator
{
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
		/// The start position of the node in the document
		/// </summary>
		private int _startPosition;

		/// <summary>
		/// The end position of the node in the document
		/// </summary>
		private int _endPosition;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a node
		/// </summary>
		/// <param name="name">The name of this object</param>
		public NodeData( string name )
		{
			_childNodes = new List<NodeData>();
			_name = name;
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
		/// Gets or sets the start position of the node in the document
		/// </summary>
		public int StartPosition
		{
			get { return _startPosition; }
			set { _startPosition = value; }
		}

		/// <summary>
		/// Gets or sets the end position of the node in the document
		/// </summary>
		public int EndPosition
		{
			get { return _endPosition; }
			set { _endPosition = value; }
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
		/// The XML document read from the file (if null, it is not a valid XML document)
		/// </summary>
		private XmlDocument _document;

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
			_document = GetDocument();

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

			if( _document == null )
				return;

			_rootNode = ParseNode( _document.DocumentElement, null );
		}

		/// <summary>
		/// Parses a single XML node along with its child nodes
		/// </summary>
		/// <param name="xmlNode">The XML node to parse</param>
		/// <param name="parentNode">The parent node to add the new node to</param>
		/// <returns>The parsed node</returns>
		private NodeData ParseNode( XmlNode xmlNode, NodeData parentNode )
		{
			var node = parentNode == null ? new NodeData( xmlNode.LocalName ) : parentNode.AddChild( xmlNode.LocalName );

			foreach( XmlNode childNode in xmlNode.ChildNodes )
			{
				if( childNode.NodeType != XmlNodeType.Element )
					continue;

				ParseNode( childNode, node );
			}

			return node;
		}

		/// <summary>
		/// Returns the XML document from the current file
		/// </summary>
		/// <returns>The parsed XML document (null if not valid)</returns>
		private XmlDocument GetDocument()
		{
			var document = new XmlDocument();

			try
			{
				document.Load( _path );
				return document;
			}
			catch { }

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
