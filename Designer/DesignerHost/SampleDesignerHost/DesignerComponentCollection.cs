namespace SampleDesignerHost
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;

    internal class DesignerComponentCollection : ComponentCollection
    {
        private SampleDesignerHost host;

        internal DesignerComponentCollection(SampleDesignerHost host) : base(new IComponent[0])
        {
            this.host = host;
            if (host.Sites != null)
            {
                foreach (ISite site in host.Sites.Values)
                {
                    base.InnerList.Add(site.Component);
                }
            }
        }

        internal void Add(ISite site)
        {
            base.InnerList.Add(site.Component);
        }

        internal void Clear()
        {
            base.InnerList.Clear();
        }

        internal void Remove(ISite site)
        {
            base.InnerList.Remove(site.Component);
        }

        public override IComponent this[string name]
        {
            get
            {
                Debug.Assert(name != null, "name is null");
                if (name == null)
                {
                    return null;
                }
                if (name.Length == 0)
                {
                    Debug.Assert(this.host.RootComponent != null, "base component is null");
                    return this.host.RootComponent;
                }
                ISite site = (ISite) this.host.Sites[name];
                return ((site == null) ? null : site.Component);
            }
        }
    }
}

