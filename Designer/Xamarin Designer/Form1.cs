using SampleDesignerApplication;
using SampleDesignerHost;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xamarin_Designer
{
    public partial class Form1 : Form
    {
        private SampleDesignerApplication.KeystrokeMessageFilter filter;
        private SampleDesignerHost.SampleDesignerHost host;
        private SampleDesignerLoader loader;
        private ServiceContainer hostingServiceContainer;
        IMenuCommandService mcs;

        public Form1()
        {
            InitializeComponent();
        }

        void CreateDesigner(SampleDesignerLoader loader)
        {
            this.host = new SampleDesignerHost.SampleDesignerHost(this.hostingServiceContainer);
            this.propertyGrid.Site = new SampleDesignerApplication.PropertyGridSite(this.host, this);
            this.propertyGrid.PropertyTabs.AddTabType(typeof(System.Windows.Forms.Design.EventsTab));
            this.host.LoadDocument(loader);
            this.loader = loader;
            this.ToolboxPane1.Host = this.host;
            this.host.View.Dock = DockStyle.Fill;
            this.host.View.Visible = true;
            this.panelMain.Controls.Add(this.host.View);
            this.filter = new KeystrokeMessageFilter(this.host);
            Application.AddMessageFilter(this.filter);
            this.host.SelectionChanged += new EventHandler(host_SelectionChanged);
        }

		private void Form1_Load(object sender, EventArgs e)
        {
			hostingServiceContainer = new ServiceContainer();
			this.hostingServiceContainer = new ServiceContainer();
			this.hostingServiceContainer.AddService(typeof(PropertyGrid), this.propertyGrid);
			this.hostingServiceContainer.AddService(typeof(ToolboxPane), this.ToolboxPane1);
			SampleDesignerLoader designerLoader = new SampleDesignerLoader("template.xdx");
			CreateDesigner(designerLoader);
			designerLoader.dirty = false;
			designerLoader.unsaved = false;
			mcs = ((IServiceProvider)this.host).GetService(typeof(IMenuCommandService)) as IMenuCommandService;
        }

        private void AnordnungToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void NeuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        void host_SelectionChanged(object sender, Object e)
        {
 
        }


        private bool DestroyDesigner()
		{
			if (this.loader != null)
			{
				if (this.loader.PromptDispose())
				{
					Application.RemoveMessageFilter(this.filter);
					this.filter = null;
					this.propertyGrid.Site = null;
					this.host.Dispose();
					this.loader = null;
					this.host = null;
					return true;
				}
				return false;
			}
			return true;
		}

        private void ÖffnenToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if (this.DestroyDesigner() && (this.OpenFileDialog1.ShowDialog(this) == DialogResult.OK))
			{
				SampleDesignerLoader designerLoader = new SampleDesignerLoader(this.OpenFileDialog1.FileName);
				this.CreateDesigner(designerLoader);
			}

        }

        private void SpeichernToolStripMenuItem_Click(object sender, EventArgs e)
        {
			this.loader.Save(false);
        }

        private void SpeichernAlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
			this.loader.Save(true);
        }

        private void SchließenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void KopierenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.Cut);
        }

        private void KopierenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.Cut);
        }

        private void EinfügenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.Paste);
        }

        private void AllesAuswählenToolStripMenuItem_Click(object sender, EventArgs e)
        {
			mcs.GlobalInvoke(StandardCommands.SelectAll);
        }

        private void LöschenToolStripMenuItem_Click(object sender, EventArgs e)
        {
			mcs.GlobalInvoke(StandardCommands.Delete);
        }

        private void GitterAnzeigenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.ShowGrid);

        }

        private void AmGitterAusrichtenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.AlignToGrid);

        }

        private void LinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.AlignLeft);

        }

        private void RechtsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.AlignRight);

        }

        private void ObenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.AlignTop);

        }

        private void UntenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.AlignBottom);

        }

        private void MittigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.AlignVerticalCenters);

        }

        private void ZentriertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.AlignHorizontalCenters);

        }

        private void AmGitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.SnapToGrid);

        }

        private void HorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void VertikalToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ElementenbeireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.SizeToControlWidth);

        }

        private void ElementenhöheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.SizeToControlHeight);

        }

        private void AmElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.SizeToControl);

        }

        private void GitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.AlignToGrid);

        }

        private void ObenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.BringToFront);

        }

        private void UntenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.SendToBack);

        }

        private void GroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.Group);

        }

        private void UngroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.Ungroup);
        }

        private void ToolStripButton21_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.VertSpaceMakeEqual);
        }

        private void ToolStripButton22_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.VertSpaceIncrease);
        }

        private void ToolStripButton27_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.VertSpaceDecrease);
        }

        private void ToolStripButton20_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.VertSpaceConcatenate);
        }

        private void ToolStripButton24_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.HorizSpaceMakeEqual);
        }

        private void ToolStripButton25_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.HorizSpaceIncrease);
        }

        private void ToolStripButton26_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.HorizSpaceDecrease);
        }

        private void ToolStripButton23_Click(object sender, EventArgs e)
        {
            mcs.GlobalInvoke(StandardCommands.HorizSpaceConcatenate);
        }
    }
}
