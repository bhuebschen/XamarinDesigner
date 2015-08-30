namespace SampleDesignerHost
{
    using SampleDesignerApplication;
    using System;
    using System.Collections;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Windows.Forms;

    public class SampleToolboxService : IToolboxService
    {
        private IDesignerHost host;
        private ToolboxPane toolbox;

        public SampleToolboxService(IDesignerHost host)
        {
            this.host = host;
            this.toolbox = host.GetService(typeof(ToolboxPane)) as ToolboxPane;
        }

        public void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
        {
        }

        public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host)
        {
        }

        public void AddToolboxItem(ToolboxItem toolboxItem, string category)
        {
        }

        public ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost host)
        {
            return (ToolboxItem) ((DataObject) serializedObject).GetData(typeof(ToolboxItem));
        }

        public ToolboxItem GetSelectedToolboxItem(IDesignerHost host)
        {
            if (this.toolbox.SelectedTool.DisplayName != "<Zeiger>")
            {
                return this.toolbox.SelectedTool;
            }
            return null;
        }

        public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host)
        {
            return this.toolbox.GetToolsFromCategory(category);
        }

        public bool IsSupported(object serializedObject, ICollection filterAttributes)
        {
            return true;
        }

        public bool IsToolboxItem(object serializedObject, IDesignerHost host)
        {
            return (this.DeserializeToolboxItem(serializedObject, host) != null);
        }

        public void Refresh()
        {
            this.toolbox.Refresh();
        }

        public void RemoveCreator(string format, IDesignerHost host)
        {
        }

        public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
        {
        }

        public void SelectedToolboxItemUsed()
        {
            this.toolbox.SelectPointer();
        }

        public object SerializeToolboxItem(ToolboxItem toolboxItem)
        {
            return new DataObject(toolboxItem);
        }

        public bool SetCursor()
        {
            try
            {
                if (this.toolbox.SelectedTool.DisplayName != "<Zeiger>")
                {
                    Cursor.Current = Cursors.Cross;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public void SetSelectedToolboxItem(ToolboxItem toolboxItem)
        {
        }

        void IToolboxService.AddCreator(ToolboxItemCreatorCallback creator, string format)
        {
        }

        void IToolboxService.AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host)
        {
        }

        void IToolboxService.AddToolboxItem(ToolboxItem toolboxItem)
        {
        }

        ToolboxItem IToolboxService.DeserializeToolboxItem(object serializedObject)
        {
            return this.DeserializeToolboxItem(serializedObject, this.host);
        }

        ToolboxItem IToolboxService.GetSelectedToolboxItem()
        {
            return this.GetSelectedToolboxItem(this.host);
        }

        ToolboxItemCollection IToolboxService.GetToolboxItems()
        {
            return this.toolbox.GetAllTools();
        }

        ToolboxItemCollection IToolboxService.GetToolboxItems(IDesignerHost host)
        {
            return this.toolbox.GetAllTools();
        }

        ToolboxItemCollection IToolboxService.GetToolboxItems(string category)
        {
            return this.GetToolboxItems(category, this.host);
        }

        bool IToolboxService.IsSupported(object serializedObject, IDesignerHost host)
        {
            return true;
        }

        bool IToolboxService.IsToolboxItem(object serializedObject)
        {
            return this.IsToolboxItem(serializedObject, this.host);
        }

        void IToolboxService.RemoveCreator(string format)
        {
        }

        void IToolboxService.RemoveToolboxItem(ToolboxItem toolboxItem)
        {
        }

        public CategoryNameCollection CategoryNames
        {
            get
            {
                return this.toolbox.CategoryNames;
            }
        }

        public string SelectedCategory
        {
            get
            {
                return this.toolbox.SelectedCategory;
            }
            set
            {
            }
        }
    }
}

