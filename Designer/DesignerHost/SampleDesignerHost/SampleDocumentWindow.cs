namespace SampleDesignerHost
{
    using System;
    using System.Collections;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Windows.Forms;

    internal class SampleDocumentWindow : Control
    {
        private IDesignerHost designerHost;
        private Control designerView;

        internal SampleDocumentWindow(IDesignerHost designerHost)
        {
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            this.designerHost = designerHost;
            base.TabStop = false;
            base.Visible = false;
            this.Text = "DocumentWindow";
            this.BackColor = SystemColors.Window;
            this.AllowDrop = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SetDesigner(null);
                this.designerHost = null;
            }
            base.Dispose(disposing);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (this.designerView != null)
            {
                this.designerView.Focus();
            }
            else
            {
                base.OnGotFocus(e);
            }
        }

        public void ReportErrors(ICollection errors)
        {
            if (errors.Count > 0)
            {
                ListBox list = new ListBox();
                foreach (object err in errors)
                {
                    list.Items.Add(err);
                }
                list.Dock = DockStyle.Fill;
                list.Height = 200;
                base.Controls.Add(list);
            }
        }

        public void SetDesigner(IRootDesigner document)
        {
            if (this.designerView != null)
            {
                base.Controls.Clear();
                this.designerView.Dispose();
                this.designerView = null;
            }
            if (document != null)
            {
                ViewTechnology[] technologies = document.SupportedTechnologies;
                bool supportedTechnology = false;
                foreach (ViewTechnology tech in technologies)
                {
                    if(tech==ViewTechnology.WindowsForms)
                    {
                        {
                            this.designerView = (Control) document.GetView(ViewTechnology.WindowsForms);
                            this.designerView.Dock = DockStyle.Fill;
                            base.Controls.Add(this.designerView);
                            supportedTechnology = true;
                        }
                    }
                    if (supportedTechnology)
                    {
                        break;
                    }
                }
                if (!supportedTechnology)
                {
                    throw new Exception("Unsupported Technology " + this.designerHost.RootComponent.GetType().FullName);
                }
            }
        }

        public bool DocumentVisible
        {
            get
            {
                return ((this.designerView != null) && this.designerView.Visible);
            }
            set
            {
                if (this.designerView != null)
                {
                    this.designerView.Visible = value;
                }
            }
        }

        public override bool Focused
        {
            get
            {
                if (this.designerView != null)
                {
                    return this.designerView.Focused;
                }
                return base.Focused;
            }
        }
    }
}

