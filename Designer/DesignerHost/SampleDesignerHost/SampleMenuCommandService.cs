namespace SampleDesignerHost
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Windows.Forms;

    public class SampleMenuCommandService : IMenuCommandService
    {
        private ContextMenu cm;
        private Hashtable commands;
        private ArrayList globalVerbs;
        private IDesignerHost host;
        private IComponent lastSelection;
        private Hashtable menuItemsFromVerbs;
        private Hashtable verbsFromMenuItems;

        public SampleMenuCommandService(IDesignerHost host)
        {
            this.host = host;
        }

        public void AddCommand(MenuCommand command)
        {
            if (this.commands == null)
            {
                this.commands = new Hashtable();
            }
            if (this.FindCommand(command.CommandID) == null)
            {
                this.commands.Add(command.CommandID, command);
            }
        }

        public void AddLocalVerb(DesignerVerb verb)
        {
            if ((this.globalVerbs == null) || !this.globalVerbs.Contains(verb))
            {
                if (this.cm == null)
                {
                    this.cm = new ContextMenu();
                    this.verbsFromMenuItems = new Hashtable();
                    this.menuItemsFromVerbs = new Hashtable();
                }
                MenuItem menuItem = new MenuItem(verb.Text);
                menuItem.Click += new EventHandler(this.menuItem_Click);
                this.verbsFromMenuItems.Add(menuItem, verb);
                this.menuItemsFromVerbs.Add(verb, menuItem);
                this.cm.MenuItems.Add(menuItem);
            }
        }

        public void AddVerb(DesignerVerb verb)
        {
            if (this.globalVerbs == null)
            {
                this.globalVerbs = new ArrayList();
            }
            this.globalVerbs.Add(verb);
            if (this.cm == null)
            {
                this.cm = new ContextMenu();
                this.verbsFromMenuItems = new Hashtable();
                this.menuItemsFromVerbs = new Hashtable();
            }
            MenuItem menuItem = new MenuItem(verb.Text);
            menuItem.Click += new EventHandler(this.menuItem_Click);
            this.verbsFromMenuItems.Add(menuItem, verb);
            this.menuItemsFromVerbs.Add(verb, menuItem);
            this.cm.MenuItems.Add(menuItem);
        }

        public MenuCommand FindCommand(CommandID commandID)
        {
            if (this.commands != null)
            {
                MenuCommand command = this.commands[commandID] as MenuCommand;
                if (command != null)
                {
                    return command;
                }
            }
            return null;
        }

        public bool GlobalInvoke(CommandID commandID)
        {
            MenuCommand command = this.FindCommand(commandID);
            if (command != null)
            {
                command.Invoke();
                return true;
            }
            return false;
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            MenuItem key = sender as MenuItem;
            (this.verbsFromMenuItems[key] as DesignerVerb).Invoke();
        }

        public void RemoveCommand(MenuCommand command)
        {
            if (this.commands != null)
            {
                this.commands.Remove(command.CommandID);
            }
        }

        public void RemoveLocalVerb(DesignerVerb verb)
        {
            if (((this.globalVerbs == null) || !this.globalVerbs.Contains(verb)) && (this.cm != null))
            {
                MenuItem key = this.menuItemsFromVerbs[verb] as MenuItem;
                this.verbsFromMenuItems.Remove(key);
                this.menuItemsFromVerbs.Remove(verb);
                this.cm.MenuItems.Remove(key);
            }
        }

        public void RemoveVerb(DesignerVerb verb)
        {
            if (this.globalVerbs != null)
            {
                this.globalVerbs.Remove(verb);
                if (this.cm != null)
                {
                    MenuItem key = this.menuItemsFromVerbs[verb] as MenuItem;
                    this.verbsFromMenuItems.Remove(key);
                    this.menuItemsFromVerbs.Remove(verb);
                    this.cm.MenuItems.Remove(key);
                }
            }
        }

        public void ShowContextMenu(CommandID menuID, int x, int y)
        {
            ISelectionService ss = this.host.GetService(typeof(ISelectionService)) as ISelectionService;
            if ((this.lastSelection != null) && (this.lastSelection != ss.PrimarySelection))
            {
                foreach (DesignerVerb verb in this.host.GetDesigner(this.lastSelection).Verbs)
                {
                    this.RemoveLocalVerb(verb);
                }
            }
            if (this.lastSelection != ss.PrimarySelection)
            {
                foreach (DesignerVerb verb in this.host.GetDesigner(ss.PrimarySelection as IComponent).Verbs)
                {
                    this.AddLocalVerb(verb);
                }
            }
            if (this.cm != null)
            {
                Control ps = ss.PrimarySelection as Control;
                Point s = ps.PointToScreen(new Point(0, 0));
                this.cm.Show(ps, new Point(x - s.X, y - s.Y));
            }
            this.lastSelection = ss.PrimarySelection as IComponent;
        }

        public DesignerVerbCollection Verbs
        {
            get
            {
                ArrayList currentVerbs;
                ISelectionService ss = this.host.GetService(typeof(ISelectionService)) as ISelectionService;
                if (this.globalVerbs != null)
                {
                    currentVerbs = new ArrayList(this.globalVerbs);
                }
                else
                {
                    currentVerbs = new ArrayList();
                }
                try
                {
                foreach (DesignerVerb verb in this.host.GetDesigner(ss.PrimarySelection as IComponent).Verbs)
                {
                    if (!currentVerbs.Contains(verb))
                    {
                        currentVerbs.Add(verb);
                    }
                }

                }
                finally
                {

                }
                if (currentVerbs.Count > 0)
                {
                    DesignerVerb[] ret = new DesignerVerb[currentVerbs.Count];
                    currentVerbs.CopyTo(ret);
                    return new DesignerVerbCollection(ret);
                }
                return new DesignerVerbCollection();
            }
        }
    }
}

