namespace SampleDesignerHost
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;

    [ProvideProperty("Name", typeof(IComponent))]
    internal class SampleNameExtenderProvider : IExtenderProvider
    {
        private static Attribute[] designerNameAttribute = new Attribute[] { new SampleDesignerNameAttribute(true) };
        private SampleDesignerHost host;

        internal SampleNameExtenderProvider(SampleDesignerHost host)
        {
            this.host = host;
        }

        public virtual bool CanExtend(object o)
        {
            if (o == this.Host.RootComponent)
            {
                return false;
            }
            if (!TypeDescriptor.GetAttributes(o)[typeof(InheritanceAttribute)].Equals(InheritanceAttribute.NotInherited))
            {
                return false;
            }
            return true;
        }

        [Description("The name property for the component"), Category("Design"), MergableProperty(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SampleDesignerName(true), ParenthesizePropertyName(true)]
        public virtual string GetName(IComponent comp)
        {
            ISite site = comp.Site;
            if (site != null)
            {
                return site.Name;
            }
            return null;
        }

        public void SetName(IComponent comp, string newName)
        {
            newName = newName.Trim();
            SampleDesignSite cs = (SampleDesignSite) comp.Site;
            if (!newName.Equals(cs.Name))
            {
                if (string.Compare(newName, cs.Name, true) != 0)
                {
                    this.Host.CheckName(newName);
                }
                ((IComponentChangeService) this.Host).OnComponentChanging(comp, TypeDescriptor.GetProperties(comp, designerNameAttribute)["Name"]);
                this.Host.Sites.Remove(cs.Name);
                this.Host.Sites[newName] = cs;
                string oldName = cs.Name;
                cs.SetName(newName);
                this.Host.OnComponentRename(new ComponentRenameEventArgs(comp, oldName, newName));
                ((IComponentChangeService) this.Host).OnComponentChanged(comp, TypeDescriptor.GetProperties(comp, designerNameAttribute)["Name"], oldName, newName);
            }
        }

        public SampleDesignerHost Host
        {
            get
            {
                return this.host;
            }
        }
    }
}

