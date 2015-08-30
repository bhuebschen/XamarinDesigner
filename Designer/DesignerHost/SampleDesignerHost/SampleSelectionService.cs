namespace SampleDesignerHost
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    internal class SampleSelectionService : ISelectionService
    {
        private bool batchMode;
        private IContainer container;
        private IDesignerHost host;
        private SampleSelectionItem primarySelection;
        private bool selectionChanged;
        private bool selectionContentsChanged;
        private Hashtable selectionsByComponent;

        public event EventHandler SelectionChanged;

        public event EventHandler SelectionChanging;

        internal SampleSelectionService(IDesignerHost host)
        {
            this.host = host;
            this.container = host.Container;
            this.selectionsByComponent = new Hashtable();
            this.selectionChanged = false;
            this.batchMode = true;
            IComponentChangeService cs = (IComponentChangeService) host.GetService(typeof(IComponentChangeService));
            Debug.Assert(cs != null, "IComponentChangeService not found");
            if (cs != null)
            {
                cs.ComponentAdded += new ComponentEventHandler(this.DesignerHost_ComponentAdd);
                cs.ComponentRemoved += new ComponentEventHandler(this.DesignerHost_ComponentRemove);
                cs.ComponentChanged += new ComponentChangedEventHandler(this.DesignerHost_ComponentChanged);
            }
            host.TransactionOpened += new EventHandler(this.DesignerHost_TransactionOpened);
            host.TransactionClosed += new DesignerTransactionCloseEventHandler(this.DesignerHost_TransactionClosed);
            if (host.InTransaction)
            {
                this.DesignerHost_TransactionOpened(host, EventArgs.Empty);
            }
            host.LoadComplete += new EventHandler(this.DesignerHost_LoadComplete);
        }

        private void AddSelection(SampleSelectionItem sel)
        {
            this.selectionsByComponent[sel.Component] = sel;
        }

        private void DesignerHost_ComponentAdd(object sender, ComponentEventArgs ce)
        {
            this.OnSelectionContentsChanged();
        }

        private void DesignerHost_ComponentChanged(object sender, ComponentChangedEventArgs ce)
        {
            if (this.selectionsByComponent[ce.Component] != null)
            {
                this.OnSelectionContentsChanged();
            }
        }

        private void DesignerHost_ComponentRemove(object sender, ComponentEventArgs ce)
        {
            SampleSelectionItem sel = (SampleSelectionItem) this.selectionsByComponent[ce.Component];
            if (sel != null)
            {
                this.RemoveSelection(sel);
                if ((this.selectionsByComponent.Count == 0) && (this.host != null))
                {
                    IComponent[] comps = new IComponent[this.host.Container.Components.Count];
                    this.host.Container.Components.CopyTo(comps, 0);
                    if (comps != null)
                    {
                        int maxZOrder = -1;
                        int defaultIndex = -1;
                        object maxIndexComp = null;
                        object baseComp = this.host.RootComponent;
                        if (baseComp != null)
                        {
                            for (int i = comps.Length - 1; i >= 0; i--)
                            {
                                if (comps[i] != baseComp)
                                {
                                    if (defaultIndex == -1)
                                    {
                                        defaultIndex = i;
                                    }
                                    if (comps[i] is Control)
                                    {
                                        int zorder = ((Control) comps[i]).TabIndex;
                                        if (zorder > maxZOrder)
                                        {
                                            maxZOrder = zorder;
                                            maxIndexComp = comps[i];
                                        }
                                    }
                                }
                            }
                            if (maxIndexComp == null)
                            {
                                if (defaultIndex != -1)
                                {
                                    maxIndexComp = comps[defaultIndex];
                                }
                                else
                                {
                                    maxIndexComp = baseComp;
                                }
                            }
                            ((ISelectionService) this).SetSelectedComponents(new object[] { maxIndexComp }, SelectionTypes.Replace);
                        }
                    }
                }
                else
                {
                    this.OnSelectionChanged();
                }
            }
            else
            {
                this.OnSelectionContentsChanged();
            }
        }

        private void DesignerHost_LoadComplete(object sender, EventArgs e)
        {
            this.batchMode = false;
            this.FlushSelectionChanges();
        }

        private void DesignerHost_TransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
        {
            this.batchMode = false;
            this.FlushSelectionChanges();
        }

        private void DesignerHost_TransactionOpened(object sender, EventArgs e)
        {
            this.batchMode = true;
        }

        internal void Dispose()
        {
            this.host.RemoveService(typeof(ISelectionService));
            this.host.TransactionOpened -= new EventHandler(this.DesignerHost_TransactionOpened);
            this.host.TransactionClosed -= new DesignerTransactionCloseEventHandler(this.DesignerHost_TransactionClosed);
            if (this.host.InTransaction)
            {
                this.DesignerHost_TransactionClosed(this.host, new DesignerTransactionCloseEventArgs(true));
            }
            this.host.LoadComplete -= new EventHandler(this.DesignerHost_LoadComplete);
            IComponentChangeService cs = (IComponentChangeService) this.host.GetService(typeof(IComponentChangeService));
            if (cs != null)
            {
                cs.ComponentAdded -= new ComponentEventHandler(this.DesignerHost_ComponentAdd);
                cs.ComponentRemoved -= new ComponentEventHandler(this.DesignerHost_ComponentRemove);
                cs.ComponentChanged -= new ComponentChangedEventHandler(this.DesignerHost_ComponentChanged);
            }
            SampleSelectionItem[] sels = new SampleSelectionItem[this.selectionsByComponent.Values.Count];
            this.selectionsByComponent.Values.CopyTo(sels, 0);
            for (int i = 0; i < sels.Length; i++)
            {
                sels[i].Dispose();
            }
            this.selectionsByComponent.Clear();
            this.primarySelection = null;
        }

        private void FlushSelectionChanges()
        {
            if (!this.batchMode)
            {
                if (this.selectionChanged)
                {
                    this.OnSelectionChanged();
                }
                if (this.selectionContentsChanged)
                {
                    this.OnSelectionContentsChanged();
                }
            }
        }

        private void OnSelectionChanged()
        {
            if (this.batchMode)
            {
                this.selectionChanged = true;
            }
            else
            {
                this.selectionChanged = false;
                if (this.SelectionChanging != null)
                {
                    try
                    {
                        this.SelectionChanging(this, EventArgs.Empty);
                    }
                    catch (Exception)
                    {
                        Debug.Fail("Exception thrown in selectionChanging event");
                    }
                }
                if (this.SelectionChanged != null)
                {
                    try
                    {
                        this.SelectionChanged(this, EventArgs.Empty);
                    }
                    catch (Exception)
                    {
                        Debug.Fail("Exception thrown in selectionChanging event");
                    }
                }
                this.OnSelectionContentsChanged();
            }
        }


        private void OnSelectionContentsChanged()
        {
            if (this.batchMode)
            {
                this.selectionContentsChanged = true;
            }
            else
            {
                this.selectionContentsChanged = false;

                PropertyGrid grid = (PropertyGrid) this.host.GetService(typeof(PropertyGrid));
                if (grid != null)
                {
                    ICollection col = ((ISelectionService) this).GetSelectedComponents();
                    object[] selection = new object[col.Count];
                    col.CopyTo(selection, 0);
                    grid.SelectedObjects = selection;
                }
            }
        }

        private void RemoveSelection(SampleSelectionItem s)
        {
            this.selectionsByComponent.Remove(s.Component);
            s.Dispose();
        }

        internal void SetPrimarySelection(SampleSelectionItem sel)
        {
            if (sel != this.primarySelection)
            {
                if (null != this.primarySelection)
                {
                    this.primarySelection.Primary = false;
                }
                this.primarySelection = sel;
            }
            if (null != this.primarySelection)
            {
                this.primarySelection.Primary = true;
            }
        }

        bool ISelectionService.GetComponentSelected(object component)
        {
             return ((component != null) && (null != this.selectionsByComponent[component]));
        }

        ICollection ISelectionService.GetSelectedComponents()
        {
            object[] sels = new object[this.selectionsByComponent.Values.Count];
            this.selectionsByComponent.Values.CopyTo(sels, 0);
            object[] objects = new object[sels.Length];
            for (int i = 0; i < sels.Length; i++)
            {
                objects[i] = ((SampleSelectionItem) sels[i]).Component;
            }
            return objects;
        }

        void ISelectionService.SetSelectedComponents(ICollection components)
        {
            ((ISelectionService) this).SetSelectedComponents(components, SelectionTypes.Auto);
        }

        void ISelectionService.SetSelectedComponents(ICollection components, SelectionTypes selectionType)
        {
            bool fToggle = false;
            bool fControl = false;
            bool fClick = false;
            bool fChanged = false;
            if (components == null)
            {
                components = new Component[0];
            }
            if (((selectionType & SelectionTypes.Auto) == SelectionTypes.Auto) || ((selectionType & SelectionTypes.Click) == SelectionTypes.Click))
            {
                fControl = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                fToggle = ((((Control.ModifierKeys & Keys.Control) != Keys.None) || ((Control.ModifierKeys & Keys.Shift) != Keys.None)) && (components.Count == 1)) && ((selectionType & SelectionTypes.MouseUp) != SelectionTypes.MouseUp);
            }
            if ((selectionType & SelectionTypes.Click) == SelectionTypes.Click)
            {
                fClick = true;
            }
            if (!fToggle && !fControl)
            {
                object firstSelection = null;
                foreach (object o in components)
                {
                    firstSelection = o;
                    break;
                }
                if ((fClick && (1 == components.Count)) && ((ISelectionService) this).GetComponentSelected(firstSelection))
                {
                    SampleSelectionItem oldPrimary = this.primarySelection;
                    this.SetPrimarySelection((SampleSelectionItem) this.selectionsByComponent[firstSelection]);
                    if (oldPrimary != this.primarySelection)
                    {
                        fChanged = true;
                    }
                }
                else
                {
                    SampleSelectionItem[] selections = new SampleSelectionItem[this.selectionsByComponent.Values.Count];
                    this.selectionsByComponent.Values.CopyTo(selections, 0);
                    foreach (SampleSelectionItem item in selections)
                    {
                        bool remove = true;
                        foreach (object comp in components)
                        {
                            if (comp == item.Component)
                            {
                                remove = false;
                                break;
                            }
                        }
                        if (remove)
                        {
                            this.RemoveSelection(item);
                            fChanged = true;
                        }
                    }
                }
            }
            SampleSelectionItem primarySel = null;
            int selectedCount = this.selectionsByComponent.Count;
            foreach (Component comp in components)
            {
                if (comp != null)
                {
                    SampleSelectionItem s = (SampleSelectionItem) this.selectionsByComponent[comp];
                    if (null == s)
                    {
                        s = new SampleSelectionItem(this, comp);
                        this.AddSelection(s);
                        if (fControl || fToggle)
                        {
                            primarySel = s;
                        }
                        fChanged = true;
                    }
                    else if (fToggle)
                    {
                        this.RemoveSelection(s);
                        fChanged = true;
                    }
                }
            }
            if (primarySel != null)
            {
                this.SetPrimarySelection(primarySel);
            }
            if (fChanged)
            {
                this.OnSelectionChanged();
            }
        }

        object ISelectionService.PrimarySelection
        {
            get
            {
                if (this.primarySelection == null)
                {
                    IDictionaryEnumerator selections = this.selectionsByComponent.GetEnumerator();
                    if (selections.MoveNext())
                    {
                        this.primarySelection = (SampleSelectionItem) selections.Value;
                        this.primarySelection.Primary = true;
                    }
                }
                if (this.primarySelection != null)
                {
                    return this.primarySelection.Component;
                }
                return null;
            }
        }

        int ISelectionService.SelectionCount
        {
            get
            {
                return this.selectionsByComponent.Count;
            }
        }
    }
}

