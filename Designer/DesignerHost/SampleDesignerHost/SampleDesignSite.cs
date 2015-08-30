namespace SampleDesignerHost
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;

    internal class SampleDesignSite : ISite, IServiceProvider, IDictionaryService
    {
        private IComponent component;
        private static Attribute[] designerNameAttribute = new Attribute[] { new SampleDesignerNameAttribute(true) };
        private Hashtable dictionary;
        private SampleDesignerHost host;
        private string name;

        internal SampleDesignSite(SampleDesignerHost host, string name)
        {
            this.host = host;
            this.component = null;
            this.name = name;
        }

        public object GetService(Type service)
        {
            if (service == typeof(IDictionaryService))
            {
                return this;
            }
            return ((IServiceProvider) this.host).GetService(service);
        }

        internal void SetComponent(IComponent component)
        {
            Debug.Assert(this.component == null, "Cannot set a component twice");
            this.component = component;
            if (this.name == null)
            {
                this.name = this.host.GetNewComponentName(component.GetType());
            }
            if (!this.host.CheckName(this.name))
            {
                this.name = this.host.GetNewComponentName(component.GetType());
                this.SetName(null);
            }
        }

        internal void SetName(string newName)
        {
            this.name = newName;
        }

        object IDictionaryService.GetKey(object value)
        {
            if (this.dictionary != null)
            {
                foreach (DictionaryEntry de in this.dictionary)
                {
                    if (object.Equals(de.Value, value))
                    {
                        return de.Key;
                    }
                }
            }
            return null;
        }

        object IDictionaryService.GetValue(object key)
        {
            if (this.dictionary != null)
            {
                return this.dictionary[key];
            }
            return null;
        }

        void IDictionaryService.SetValue(object key, object value)
        {
            if (this.dictionary == null)
            {
                this.dictionary = new Hashtable();
            }
            this.dictionary[key] = value;
        }

        public IComponent Component
        {
            get
            {
                Debug.Assert(this.component != null, "Need the component before we've established it");
                return this.component;
            }
        }

        public IContainer Container
        {
            get
            {
                return this.host.Container;
            }
        }

        public bool DesignMode
        {
            get
            {
                return true;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Bad Name Value - cannot be null");
                }
                if (!value.Equals(this.name))
                {
                    this.host.OnComponentRename(new ComponentRenameEventArgs(this.component, this.name, value));
                    this.name = value;
                }
            }
        }
    }
}

