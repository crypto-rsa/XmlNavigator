:warning: **NOTE: This repository is no longer under active development and has been archived**. :warning:

(_feel free to fork it and/or contact me if you need advice_)

# XML Navigator
A Notepad++ plugin which shows the structure of an XML file in a tree view and allows fast navigation within it.

## Features:
  * fast loading (less than 1 second on a ~16 MB file with over 180 thousand nodes)
  * filtering visible nodes by name
  * accurate navigation to
    * node start/end (ie. start of opening tag/end of closing tag)
    * content start/end (where _content_ is the node text excluding the opening and closing tags)
  * allows selecting whole node or node content directly from the context menu
  * shows attribute values next to the node name, greatly simplifying navigation within a node which has lots of child nodes with the same names
  
##### Requirements
  * .NET 4.5 or higher
