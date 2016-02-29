namespace XmlNavigator
{
    partial class NavigatorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.treeViewNodes = new System.Windows.Forms.TreeView();
			this.nodeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemGoTo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemGoToNodeEnd = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemNodeContentStart = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemNodeContentEnd = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSelectNode = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSelectContent = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemGoToNodeStart = new System.Windows.Forms.ToolStripMenuItem();
			this.nodeContextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeViewNodes
			// 
			this.treeViewNodes.ContextMenuStrip = this.nodeContextMenu;
			this.treeViewNodes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewNodes.Location = new System.Drawing.Point(0, 0);
			this.treeViewNodes.Name = "treeViewNodes";
			this.treeViewNodes.Size = new System.Drawing.Size(284, 262);
			this.treeViewNodes.TabIndex = 1;
			this.treeViewNodes.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewNodes_BeforeExpand);
			this.treeViewNodes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewNodes_AfterSelect);
			this.treeViewNodes.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewNodes_NodeMouseClick);
			// 
			// nodeContextMenu
			// 
			this.nodeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemGoToNodeStart,
            this.toolStripMenuItemGoTo,
            this.toolStripMenuItemSelectNode,
            this.toolStripMenuItemSelectContent});
			this.nodeContextMenu.Name = "nodeContextMenu";
			this.nodeContextMenu.Size = new System.Drawing.Size(175, 92);
			this.nodeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.nodeContextMenu_Opening);
			// 
			// toolStripMenuItemGoTo
			// 
			this.toolStripMenuItemGoTo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemGoToNodeEnd,
            this.toolStripMenuItemNodeContentStart,
            this.toolStripMenuItemNodeContentEnd});
			this.toolStripMenuItemGoTo.Name = "toolStripMenuItemGoTo";
			this.toolStripMenuItemGoTo.Size = new System.Drawing.Size(174, 22);
			this.toolStripMenuItemGoTo.Text = "Go To";
			// 
			// toolStripMenuItemGoToNodeEnd
			// 
			this.toolStripMenuItemGoToNodeEnd.Name = "toolStripMenuItemGoToNodeEnd";
			this.toolStripMenuItemGoToNodeEnd.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItemGoToNodeEnd.Text = "Node End";
			this.toolStripMenuItemGoToNodeEnd.Click += new System.EventHandler(this.toolStripMenuItemGoToNodeEnd_Click);
			// 
			// toolStripMenuItemNodeContentStart
			// 
			this.toolStripMenuItemNodeContentStart.Name = "toolStripMenuItemNodeContentStart";
			this.toolStripMenuItemNodeContentStart.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItemNodeContentStart.Text = "Content Start";
			this.toolStripMenuItemNodeContentStart.Click += new System.EventHandler(this.toolStripMenuItemNodeContentStart_Click);
			// 
			// toolStripMenuItemNodeContentEnd
			// 
			this.toolStripMenuItemNodeContentEnd.Name = "toolStripMenuItemNodeContentEnd";
			this.toolStripMenuItemNodeContentEnd.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItemNodeContentEnd.Text = "Content End";
			this.toolStripMenuItemNodeContentEnd.Click += new System.EventHandler(this.toolStripMenuItemNodeContentEnd_Click);
			// 
			// toolStripMenuItemSelectNode
			// 
			this.toolStripMenuItemSelectNode.Name = "toolStripMenuItemSelectNode";
			this.toolStripMenuItemSelectNode.Size = new System.Drawing.Size(174, 22);
			this.toolStripMenuItemSelectNode.Text = "Select Whole Node";
			this.toolStripMenuItemSelectNode.Click += new System.EventHandler(this.toolStripMenuItemSelectNode_Click);
			// 
			// toolStripMenuItemSelectContent
			// 
			this.toolStripMenuItemSelectContent.Name = "toolStripMenuItemSelectContent";
			this.toolStripMenuItemSelectContent.Size = new System.Drawing.Size(174, 22);
			this.toolStripMenuItemSelectContent.Text = "Select Content";
			this.toolStripMenuItemSelectContent.Click += new System.EventHandler(this.toolStripMenuItemSelectContent_Click);
			// 
			// toolStripMenuItemGoToNodeStart
			// 
			this.toolStripMenuItemGoToNodeStart.Name = "toolStripMenuItemGoToNodeStart";
			this.toolStripMenuItemGoToNodeStart.Size = new System.Drawing.Size(174, 22);
			this.toolStripMenuItemGoToNodeStart.Text = "Go To Node Start";
			this.toolStripMenuItemGoToNodeStart.Click += new System.EventHandler(this.toolStripMenuItemGoToNodeStart_Click);
			// 
			// NavigatorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.treeViewNodes);
			this.Name = "NavigatorForm";
			this.Text = "XML Navigator";
			this.nodeContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TreeView treeViewNodes;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemGoTo;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemGoToNodeEnd;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNodeContentStart;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNodeContentEnd;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectNode;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectContent;
		private System.Windows.Forms.ContextMenuStrip nodeContextMenu;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemGoToNodeStart;
	}
}