﻿using System;
using System.Collections.Generic;
using System.Linq;
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

		/// <summary>
		/// The name of the node to show in the tree view
		/// </summary>
		private string _displayName;

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

		/// <summary>
		/// Returns the name of the node to show in the tree view
		/// </summary>
		/// <returns></returns>
		private string GetDisplayName()
		{
			var nameBuilder = new StringBuilder();

			nameBuilder.Append( this.LocalName );

			if( this.Attributes != null )
			{
				nameBuilder.Append( " " );
				nameBuilder.Append( string.Join( " ", this.Attributes.Select( a => $"[{a.Value}]" ) ) );
			}

			return nameBuilder.ToString();
		}

		/// <summary>
		/// Checks whether a subtree rooted at this node matches the given filter
		/// </summary>
		/// <param name="filterItems">An array of filter items to match</param>
		/// <returns>True if <paramref name="filterItems"/> is null; true if this node or any node in its subtree match the filter; false otherwise</returns>
		public bool SubtreeMatchesFilter( string[] filterItems )
		{
			if( filterItems == null )
				return true;

			if( NameMatchesFilter( filterItems ) )
				return true;

			return this.ChildNodes.Any( c => c.SubtreeMatchesFilter( filterItems ) );
		}

		/// <summary>
		/// Checks whether the display name of this node matches the given filter
		/// </summary>
		/// <param name="filterItems">An array of filter items to match</param>
		/// <returns>True if the display name matches the filter</returns>
		private bool NameMatchesFilter( string[] filterItems )
		{
			// FILTER MATCHING:
			//	- all items must be present in the display name
			//	- order doesn't matter
			//	- same prefixes are matched only once (ie. "a ab" matches the same nodes as "ab")
			return filterItems.All( s => this.DisplayName.IndexOf( s, StringComparison.OrdinalIgnoreCase ) >= 0 );
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
		/// Gets the name of the node to show in the tree view
		/// </summary>
		public string DisplayName
		{
			get
			{
				if( _displayName == null )
				{
					_displayName = GetDisplayName();
				}

				return _displayName;
			}
		}

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
		/// Gets or sets the list of element attributes
		/// </summary>
		public List<KeyValuePair<string, string>> Attributes { get; set; }

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

		/// <summary>
		/// Gets or sets the node comment
		/// </summary>
		public string Comment { get; set; }

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
			bool setContentStart = false;
			bool setNodeEnd = false;

			while( reader.Read() )
			{
				if( setContentStart )
				{
					current.ContentExtent.Start = GetCurrentPosition( reader );
					setContentStart = false;
				}

				if( setNodeEnd )
				{
					current.NodeExtent.End = GetCurrentPosition( reader );
					setNodeEnd = false;
					current = current.Parent;
				}

				switch( reader.NodeType )
				{
					case XmlNodeType.Element:
						current = GetNewElement( reader, current );
						setContentStart = !current.IsEmpty;
						setNodeEnd = current.IsEmpty;
						break;

					case XmlNodeType.EndElement:
						current.ContentExtent.End = GetCurrentPosition( reader );
						setNodeEnd = true;
						break;

					case XmlNodeType.Comment:
						if( current != null )
						{
							current.Comment = reader.Value;
						}
						break;
				}
			}

			if( setNodeEnd )
			{
				current.NodeExtent.End = GetCurrentPosition( reader );
			}
		}

		/// <summary>
		/// Processes a node representing the start of an element (also an end, if it is empty)
		/// </summary>
		/// <param name="reader">The current reader</param>
		/// <param name="parent">The parent of the new node (can be null)</param>
		/// <returns>The newly read node</returns>
		private NodeData GetNewElement( XmlReader reader, NodeData parent )
		{
			int depth = parent != null ? parent.Depth + 1 : 0;
			var current = new NodeData( reader, depth );

			current.NodeExtent.Start = GetCurrentPosition( reader );

			ReadAttributes( reader, current );

			// set the root
			if( _rootNode == null )
			{
				_rootNode = current;
			}

			if( parent != null )
			{
				parent.AddChild( current );
			}

			return current;
		}

		/// <summary>
		/// Reads the attributes of the current element
		/// </summary>
		/// <param name="reader">The current reader</param>
		/// <param name="data">The current element data</param>
		private void ReadAttributes( XmlReader reader, NodeData data )
		{
			if( !reader.HasAttributes )
				return;

			data.Attributes = new List<KeyValuePair<string, string>>( reader.AttributeCount );

			for( int i = 0; i < reader.AttributeCount; i++ )
			{
				reader.MoveToAttribute( i );

				data.Attributes.Add( new KeyValuePair<string, string>( reader.Name, reader.Value ) );
			}

			reader.MoveToElement();
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
