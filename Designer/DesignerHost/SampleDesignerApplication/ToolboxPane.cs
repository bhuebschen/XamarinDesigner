namespace SampleDesignerApplication
{
    using FormsControls;
    using SampleDesignerHost;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Windows.Forms;

    public class ToolboxPane : UserControl
    {
        private Container components = null;
        private System.Type[] componentsToolTypes = new System.Type[0];
        private System.Type[] dataToolTypes = new System.Type[0];
        private SampleDesignerHost host;
        private bool initialPaint = true;
        private ListBox listWindowsForms;
        private ToolboxItem pointer;
        private int selectedIndex;
        private System.Type[] windowsFormsToolTypes = new System.Type[] { typeof(Label), typeof(PictureBox), typeof(Button), typeof(ComboBox), typeof(CheckBox), typeof(ProgressBar), typeof(TrackBar), typeof(WebBrowser), typeof(Panel), typeof(TextBox), typeof(ListView), typeof(Stepper), typeof(ActivityIndicator)};

        public ToolboxPane()
        {
            this.InitializeComponent();
            this.pointer = new ToolboxItem();
            this.pointer.DisplayName = "<Zeiger>";
            this.pointer.Bitmap = new Bitmap(0x10, 0x10);
            this.FillToolbox();
            ListBox list = this.listWindowsForms;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FillToolbox()
        {
            this.listWindowsForms.Items.Add(this.pointer);
            foreach (System.Type type in this.windowsFormsToolTypes)
            {
                ToolboxItem tbi = new ToolboxItem(type);
                ToolboxBitmapAttribute tba = TypeDescriptor.GetAttributes(type)[typeof(ToolboxBitmapAttribute)] as ToolboxBitmapAttribute;
                if (tba == null)
                {
                }
                this.listWindowsForms.Items.Add(tbi);
            }
            this.listWindowsForms.DrawItem += new DrawItemEventHandler(this.list_DrawItem);
            this.listWindowsForms.KeyDown += new KeyEventHandler(this.list_KeyDown);
            this.listWindowsForms.MeasureItem += new MeasureItemEventHandler(this.list_MeasureItem);
            this.listWindowsForms.MouseDown += new MouseEventHandler(this.list_MouseDown);
        }

        public ToolboxItemCollection GetAllTools()
        {
            ArrayList toolsAL = new ArrayList();
            ListBox list = this.listWindowsForms;
            toolsAL.Add(list.Items);
            ToolboxItem[] tools = new ToolboxItem[toolsAL.Count];
            toolsAL.CopyTo(tools);
            return new ToolboxItemCollection(tools);
        }

        public ToolboxItemCollection GetToolsFromCategory(string category)
        {
            if ("WinForms" == category)
            {
                ListBox list = this.listWindowsForms;
                ToolboxItem[] tools = new ToolboxItem[list.Items.Count];
                list.Items.CopyTo(tools, 0);
                return new ToolboxItemCollection(tools);
            }
            return null;
        }

        private void InitializeComponent()
        {
            this.listWindowsForms = new ListBox();
            base.SuspendLayout();
            this.listWindowsForms.AllowDrop = true;
            this.listWindowsForms.BackColor = SystemColors.Window;
            this.listWindowsForms.Dock = DockStyle.Fill;
            this.listWindowsForms.DrawMode = DrawMode.OwnerDrawVariable;
            this.listWindowsForms.Location = new Point(0, 0);
            this.listWindowsForms.Name = "listWindowsForms";
            this.listWindowsForms.Size = new Size(0x120, 0x228);
            this.listWindowsForms.TabIndex = 1;
            this.BackColor = Color.Black;
            base.Controls.Add(this.listWindowsForms);
            base.Name = "ToolboxPane";
            base.Size = new Size(0x120, 0x228);
            base.Paint += new PaintEventHandler(this.ToolboxPane_Paint);
            base.ResumeLayout(false);
        }

        private void list_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox lbSender = sender as ListBox;
            if (this.selectedIndex == e.Index)
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Window), e.Bounds);
            }
            ToolboxItem tbi = lbSender.Items[e.Index] as ToolboxItem;
            Rectangle BitmapBounds = new Rectangle(e.Bounds.Location.X, e.Bounds.Location.Y, tbi.Bitmap.Width, e.Bounds.Height);
            Rectangle StringBounds = new Rectangle(e.Bounds.Location.X + BitmapBounds.Width, e.Bounds.Location.Y, e.Bounds.Width - BitmapBounds.Width, e.Bounds.Height);
            e.Graphics.DrawImage(tbi.Bitmap, BitmapBounds);
            if (this.selectedIndex == e.Index)
            {
                e.Graphics.DrawString(tbi.DisplayName, lbSender.Font, new SolidBrush(SystemColors.HighlightText), StringBounds);
            }
            else
            {
                e.Graphics.DrawString(tbi.DisplayName, lbSender.Font, new SolidBrush(SystemColors.WindowText), StringBounds);
            }
        }

        private void list_KeyDown(object sender, KeyEventArgs e)
        {
            ListBox lbSender = sender as ListBox;
            Rectangle lastSelectedBounds = lbSender.GetItemRectangle(this.selectedIndex);
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (this.selectedIndex > 0)
                    {
                        this.selectedIndex--;
                        lbSender.SelectedIndex = this.selectedIndex;
                        lbSender.Invalidate(lastSelectedBounds);
                        lbSender.Invalidate(lbSender.GetItemRectangle(this.selectedIndex));
                    }
                    break;

                case Keys.Down:
                    if ((this.selectedIndex + 1) < lbSender.Items.Count)
                    {
                        this.selectedIndex++;
                        lbSender.SelectedIndex = this.selectedIndex;
                        lbSender.Invalidate(lastSelectedBounds);
                        lbSender.Invalidate(lbSender.GetItemRectangle(this.selectedIndex));
                    }
                    break;

                case Keys.Return:
                {
                    IToolboxUser tbu = this.host.GetDesigner(this.host.RootComponent) as IToolboxUser;
                    if (tbu != null)
                    {
                        tbu.ToolPicked((ToolboxItem) lbSender.Items[this.selectedIndex]);
                    }
                    break;
                }
            }
        }

        private void list_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            ListBox lbSender = sender as ListBox;
            ToolboxItem tbi = lbSender.Items[e.Index] as ToolboxItem;
            Size textSize = e.Graphics.MeasureString(tbi.DisplayName, lbSender.Font).ToSize();
            e.ItemWidth = tbi.Bitmap.Width + textSize.Width;
            if (tbi.Bitmap.Height > textSize.Height)
            {
                e.ItemHeight = tbi.Bitmap.Height;
            }
            else
            {
                e.ItemHeight = textSize.Height;
            }
        }

        private void list_MouseDown(object sender, MouseEventArgs e)
        {
            ListBox lbSender = sender as ListBox;
            Rectangle lastSelectedBounds = lbSender.GetItemRectangle(this.selectedIndex);
            this.selectedIndex = lbSender.IndexFromPoint(e.X, e.Y);
            lbSender.SelectedIndex = this.selectedIndex;
            lbSender.Invalidate(lastSelectedBounds);
            lbSender.Invalidate(lbSender.GetItemRectangle(this.selectedIndex));
            if (this.selectedIndex != 0)
            {
                if (e.Clicks == 2)
                {
                    IToolboxUser tbu = this.host.GetDesigner(this.host.RootComponent) as IToolboxUser;
                    if (tbu != null)
                    {
                        tbu.ToolPicked((ToolboxItem) lbSender.Items[this.selectedIndex]);
                    }
                }
                else if (e.Clicks < 2)
                {
                    ToolboxItem tbi = lbSender.Items[this.selectedIndex] as ToolboxItem;
                    IToolboxService tbs = ((IServiceProvider) this.host).GetService(typeof(IToolboxService)) as IToolboxService;
                    DataObject d = tbs.SerializeToolboxItem(tbi) as DataObject;
                    try
                    {
                        lbSender.DoDragDrop(d, DragDropEffects.Copy);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        public void SelectPointer()
        {
            ListBox list = this.listWindowsForms;
            list.Invalidate(list.GetItemRectangle(this.selectedIndex));
            this.selectedIndex = 0;
            list.SelectedIndex = 0;
            list.Invalidate(list.GetItemRectangle(this.selectedIndex));
        }

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tcSender = sender as TabControl;
            string text = tcSender.TabPages[e.Index].Text;
            Size textSize = e.Graphics.MeasureString(text, tcSender.Font).ToSize();
            switch (e.Index)
            {
                case 0:
                    e.Graphics.FillRectangle(Brushes.SteelBlue, e.Bounds);
                    break;

                case 1:
                    e.Graphics.FillRectangle(Brushes.Gold, e.Bounds);
                    break;

                case 2:
                    e.Graphics.FillRectangle(Brushes.Tomato, e.Bounds);
                    break;

                case 3:
                    e.Graphics.FillRectangle(Brushes.LimeGreen, e.Bounds);
                    break;
            }
            Point textLocation = new Point(e.Bounds.X + ((e.Bounds.Width - textSize.Width) / 2), e.Bounds.Y + ((e.Bounds.Height - textSize.Height) / 2));
            e.Graphics.DrawString(text, tcSender.Font, Brushes.White, (PointF) textLocation);
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectPointer();
        }

        private void ToolboxPane_Paint(object sender, PaintEventArgs e)
        {
            if (this.initialPaint)
            {
                this.SelectPointer();
            }
            this.initialPaint = false;
            this.listWindowsForms.EndUpdate();
            this.listWindowsForms.BringToFront();
            this.listWindowsForms.Visible = true;
            this.listWindowsForms.BackColor = SystemColors.Window;
            this.listWindowsForms.Enabled = true;
            for (int i = 0; i < this.listWindowsForms.Items.Count; i++)
            {
                this.listWindowsForms.ForeColor = Color.Red;
            }
        }

        public CategoryNameCollection CategoryNames
        {
            get
            {
                string[] categories = new string[1];
                for (int i = 0; i < 1; i++)
                {
                    categories[i] = "WinForms";
                }
                return new CategoryNameCollection(categories);
            }
        }

        public SampleDesignerHost Host
        {
            get
            {
                return this.host;
            }
            set
            {
                this.host = value;
            }
        }

        public string SelectedCategory
        {
            get
            {
                return "WinForms";
            }
        }

        public ToolboxItem SelectedTool
        {
            get
            {
                return (this.listWindowsForms.Items[this.selectedIndex] as ToolboxItem);
            }
        }
    }
}

