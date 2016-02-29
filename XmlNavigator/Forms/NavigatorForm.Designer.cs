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
			this.treeViewNodes = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// treeViewNodes
			// 
			this.treeViewNodes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewNodes.Location = new System.Drawing.Point(0, 0);
			this.treeViewNodes.Name = "treeViewNodes";
			this.treeViewNodes.Size = new System.Drawing.Size(284, 262);
			this.treeViewNodes.TabIndex = 1;
			this.treeViewNodes.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewNodes_BeforeExpand);
			this.treeViewNodes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewNodes_AfterSelect);
			// 
			// NavigatorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.treeViewNodes);
			this.Name = "NavigatorForm";
			this.Text = "XML Navigator";
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TreeView treeViewNodes;

	}
}