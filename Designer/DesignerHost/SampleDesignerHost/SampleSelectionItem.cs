namespace SampleDesignerHost
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal class SampleSelectionItem
    {
        private System.ComponentModel.Component component;
        private bool primary;
        private SampleSelectionService selectionMgr;

        internal event EventHandler SampleSelectionItemDispose;

        internal event EventHandler SelectionItemInvalidate;

        internal SampleSelectionItem(SampleSelectionService selectionMgr, System.ComponentModel.Component component)
        {
            this.component = component;
            this.selectionMgr = selectionMgr;
        }

        internal virtual void Dispose()
        {
            if (this.primary)
            {
                this.selectionMgr.SetPrimarySelection(null);
            }
            if (this.SampleSelectionItemDispose != null)
            {
                this.SampleSelectionItemDispose(this, EventArgs.Empty);
            }
        }

        internal System.ComponentModel.Component Component
        {
            get
            {
                return this.component;
            }
        }

        internal virtual bool Primary
        {
            get
            {
                return this.primary;
            }
            set
            {
                if (this.primary != value)
                {
                    this.primary = value;
                    if (this.SelectionItemInvalidate != null)
                    {
                        this.SelectionItemInvalidate(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}

