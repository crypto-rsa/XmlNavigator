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

		/// <summary>
		/// Gets a value indicating whether the extent represents a valid range
		/// </summary>
		public bool IsValid
		{
			get { return (this.Start >= 0 && this.End > this.Start); }
		}

		#endregion
	}

	/// <summary>
	/// Contains information about a single XML node
	/// </summary>
	public class NodeData
	{
		#region Data Members

		/// <summary>
		/// The list of child nodes
		/// </summary>
		private List<NodeData> _childNodes;

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
		/// The default constructor
		/// </summary>
		private NodeData()
		{
			_childNodes = new List<NodeData>();
			_nodeExtent = new NodeExtent();
			_contentExtent = new NodeExtent();
		}

		/// <summary>
		/// Constructs a node using the current state of an <see cref="XmlReader"/>
		/// </summary>
		/// <param name="reader">The current reader</param>
		/// <param name="depth">The depth of the node in the full tree</param>
		public NodeData( XmlReader reader, int depth )
			: this()
		{
			this.Name = reader.Name;
			this.LocalName = reader.LocalName;
			this.Depth = depth;
			this.IsEmpty = reader.IsEmptyElement;
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return this.LocalName;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds an existing node to this node
		/// </summary>
		/// <param name="childNode">The node to add</param>
		public void AddChild( NodeData childNode )
		{
			childNode.Parent = this;

			_childNodes.Add( childNode );
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the full name of the node
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the local name of the node
		/// </summary>
		public string LocalName { get; private set; }

		/// <summary>
		/// Gets or sets the parent node of this node
		/// </summary>
		public NodeData Parent { get; private set; }

		/// <summary>
		/// Gets or sets the depth the node is located at
		/// </summary>
		public int Depth { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the node is empty (ie. does not have the full closing tag)
		/// </summary>
		public bool IsEmpty { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the tree node corresponding to this object has been expanded
		/// </summary>
		public bool TreeNodeExpanded { get; set; }

		/// <summary>
		/// Gets the collection of child nodes
		/// </summary>
		public IReadOnlyList<NodeData> ChildNodes
		{
			get { return _childNodes.AsReadOnly(); }
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
		/// The input text to parse
		/// </summary>
		private string _text;

		/// <summary>
		/// The offsets of individual lines
		/// </summary>
		private int[] _lineOffsets;

		/// <summary>
		/// An object representing the root node of the XML document
		/// </summary>
		private NodeData _rootNode;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates the parser for a specific path
		/// </summary>
		/// <param name="text">The input text to parse</param>
		public XmlParser( string text )
		{
			_text = text;

			CalculateLineOffsets();
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

			System.IO.StringReader textReader = null;
			XmlReader reader = null;

			try
			{
				textReader = new System.IO.StringReader( _text );
				reader = XmlReader.Create( textReader );
				ReadNodes( reader, null );
			}
			catch( Exception e )
			{

				System.Diagnostics.Debug.WriteLine( e.Message );
			}
			finally
			{
				if( reader != null )
				{
					reader.Dispose();
					reader = null;
				}

				if( textReader != null )
				{
					textReader.Dispose();
					textReader = null;
				}
			}
		}

		/// <summary>
		/// Reads the nodes using the given reader
		/// </summary>
		/// <param name="reader">The reader to use</param>
		/// <param name="current">The parent of the current subtree</param>
		private void ReadNodes( XmlReader reader, NodeData current )
		{
			var parent = current;

			if( current != null )
			{
				// skip the opening tag
				reader.Read();
			}

			while( reader.Read() )
			{
				if( CanSetContentStart( current ) )
				{
					current.ContentExtent.Start = GetCurrentPosition( reader );
				}

				if( CanSetNodeEnd( current, reader ) )
				{
					current.NodeExtent.End = GetCurrentPosition( reader );
				}

				switch( reader.NodeType )
				{
					case XmlNodeType.Element:
						current = ProcessElement( reader, parent );
						break;

					case XmlNodeType.EndElement:
						current = current.Parent;
						break;
				}
			}
		}

		/// <summary>
		/// Processes a node representing the start of an element (also an end, if it is empty)
		/// </summary>
		/// <param name="reader">The current reader</param>
		/// <param name="parent">The parent of the new node (can be null)</param>
		/// <returns>The newly read node</returns>
		private NodeData ProcessElement( XmlReader reader, NodeData parent )
		{
			int depth = parent != null ? parent.Depth + 1 : 0;
			var current = new NodeData( reader, depth );

			current.NodeExtent.Start = GetCurrentPosition( reader );

			// set the root
			if( _rootNode == null )
			{
				_rootNode = current;
			}

			if( parent != null )
			{
				parent.AddChild( current );
			}

			if( !current.IsEmpty )
			{
				// read the subtree
				using( var subTreeReader = reader.ReadSubtree() )
				{
					ReadNodes( subTreeReader, current );
				}

				// we are now at the closing tag of the current node
				current.ContentExtent.End = GetCurrentPosition( reader );

				// skip the closing tag
				reader.Read();

				if( reader.NodeType != XmlNodeType.None )
				{
					current.NodeExtent.End = GetCurrentPosition( reader );
				}
				else
				{
					current.NodeExtent.End = _text.Length;
				}
			}

			return current;
		}

		/// <summary>
		/// Checks whether the start position of a node content can be set
		/// </summary>
		/// <param name="node">The node to check</param>
		/// <returns>True if the content start position can be set</returns>
		private bool CanSetContentStart( NodeData node )
		{
			if( node == null )
				return false;

			if( node.IsEmpty )
				return false;

			if( node.ContentExtent.Start >= 0 )
				return false;   // already set

			return true;
		}

		/// <summary>
		/// Checks whether the end position of a node
		/// </summary>
		/// <param name="node">The node to check</param>
		/// <param name="reader">The current reader</param>
		/// <returns>True if the node end position can be set</returns>
		private bool CanSetNodeEnd( NodeData node, XmlReader reader )
		{
			if( node == null )
				return false;

			if( node.NodeExtent.End >= 0 )
				return false;   // already set

			if( !node.IsEmpty )
				return false;	// will be set after the EndElement node is found

			if( reader.Depth > node.Depth )
				return false;   // inside the node's subtree

			return true;
		}

		/// <summary>
		/// Calculates the offsets of individual lines in the source text
		/// </summary>
		private void CalculateLineOffsets()
		{
			var lines = _text.Split( '\n' );
			int separatorLength = 1;	// XmlReader always normalizes line separators to '\n'

			_lineOffsets = new int[lines.Length + 1];
			for( int i = 0; i < _lineOffsets.Length; i++ )
			{
				_lineOffsets[i] = i == 0 ? 0 : _lineOffsets[i - 1] + lines[i - 1].Length + separatorLength;
			}
		}

		/// <summary>
		/// Returns the current position of a reader in the souce text
		/// </summary>
		/// <param name="reader">The reader to get the position of</param>
		/// <returns>The linear position of <paramref name="reader"/></returns>
		private int GetCurrentPosition( XmlReader reader )
		{
			var lineInfo = (IXmlLineInfo) reader;
			int position = _lineOffsets[lineInfo.LineNumber - 1] + lineInfo.LinePosition - 1;

			// include the opening bracket
			switch( reader.NodeType )
			{
				case XmlNodeType.Element:
					position--;
					break;

				case XmlNodeType.EndElement:
					position -= 2;
					break;
			}

			return position;
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
