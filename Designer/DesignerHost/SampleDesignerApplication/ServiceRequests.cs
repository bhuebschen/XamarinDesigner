namespace SampleDesignerApplication
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class ServiceRequests : Form
    {
        private Button button1;
        private Container components = null;
        private Label label1;
        private Hashtable serviceRequests;
        private TreeNode successfulNode;
        private TreeView treeView1;
        private TreeNode unsuccessfulNode;

        public ServiceRequests()
        {
            this.InitializeComponent();
            this.InitializeTreeViewNodes();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.treeView1.Nodes.Clear();
            this.InitializeTreeViewNodes();
            if (this.serviceRequests != null)
            {
                this.serviceRequests.Clear();
            }
        }

        private TreeNode CreateNodeForType(System.Type type)
        {
            TreeNode node = new TreeNode(type.FullName);
            string[] entries = Environment.StackTrace.Split(new char[] { '\r', '\n' });
            bool recordEntry = false;
            foreach (string entry in entries)
            {
                if (entry.Length > 0)
                {
                    if (recordEntry)
                    {
                        node.Nodes.Add(new TreeNode(entry));
                    }
                    else if (entry.IndexOf("GetService") != -1)
                    {
                        recordEntry = true;
                    }
                }
            }
            return node;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.treeView1 = new TreeView();
            this.button1 = new Button();
            this.label1 = new Label();
            base.SuspendLayout();
            this.treeView1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.treeView1.ImageIndex = -1;
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = -1;
            this.treeView1.Size = new Size(440, 0x148);
            this.treeView1.TabIndex = 0;
            this.button1.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.button1.Location = new Point(360, 0x180);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x48, 0x18);
            this.button1.TabIndex = 1;
            this.button1.Text = "&Clear";
            this.button1.Click += new EventHandler(this.button1_Click);
            this.label1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.label1.Location = new Point(8, 0x158);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x158, 0x48);
            this.label1.TabIndex = 2;
            this.label1.Text = "This window shows the services that have been requested by the designer.  There are two top-level categories:  Successful and Unsuccessful.  Unsuccessful service requests result in degraded designer functionality.";
            this.AutoScaleBaseSize = new Size(5, 13);
            base.ClientSize = new Size(440, 0x1a6);
            base.Controls.AddRange(new Control[] { this.label1, this.button1, this.treeView1 });
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ServiceRequests";
            this.Text = "ServiceRequests";
            base.ResumeLayout(false);
        }

        private void InitializeTreeViewNodes()
        {
            this.successfulNode = new TreeNode("Successful Requests");
            this.unsuccessfulNode = new TreeNode("Unsuccessful Requests");
            this.treeView1.Nodes.Add(this.successfulNode);
            this.treeView1.Nodes.Add(this.unsuccessfulNode);
        }

        public void ServiceFailed(System.Type type)
        {
            if (this.serviceRequests == null)
            {
                this.serviceRequests = new Hashtable();
            }
            if (!this.serviceRequests.ContainsKey(type))
            {
                this.serviceRequests[type] = false;
                this.unsuccessfulNode.Nodes.Add(this.CreateNodeForType(type));
            }
        }

        public void ServiceSucceeded(System.Type type)
        {
            if (this.serviceRequests == null)
            {
                this.serviceRequests = new Hashtable();
            }
            if (!this.serviceRequests.ContainsKey(type))
            {
                this.serviceRequests[type] = true;
                this.successfulNode.Nodes.Add(this.CreateNodeForType(type));
            }
        }
    }
}

