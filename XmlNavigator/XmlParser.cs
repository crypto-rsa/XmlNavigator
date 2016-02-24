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
				ReadNodes( reader );
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
		private void ReadNodes( XmlReader reader )
		{
			var parentNodes = new Dictionary<int, NodeData>();
			var lineInfo = (IXmlLineInfo) reader;

			while( reader.Read() )
			{
				if( reader.NodeType != XmlNodeType.Element )
					continue;

				NodeData current;
				NodeData parent;

				if( parentNodes.TryGetValue( reader.Depth, out parent ) )
				{
					current = parent.AddChild( reader.LocalName );
				}
				else
				{
					current = new NodeData( reader.LocalName );
				}

				current.NodeExtent.Start = GetCurrentPosition( reader ) - 1;

				if( _rootNode == null && reader.Depth == 0 )
				{
					_rootNode = current;
				}

				parentNodes[reader.Depth + 1] = current;
			}
		}

		/// <summary>
		/// Calculates the offsets of individual lines in the source text
		/// </summary>
		private void CalculateLineOffsets()
		{
			var lines = _text.Split( new string[] { Environment.NewLine }, StringSplitOptions.None );
			int separatorLength = Environment.NewLine.Length;

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
