namespace SampleDesignerApplication
{
    using System;
    using System.ComponentModel;

    public class PropertyGridSite : ISite, IServiceProvider
    {
        private IComponent component;
        private IServiceProvider sp;

        public PropertyGridSite(IServiceProvider sp, IComponent component)
        {
            this.sp = sp;
            this.component = component;
        }

        public object GetService(Type serviceType)
        {
            if (this.sp != null)
            {
                return this.sp.GetService(serviceType);
            }
            return null;
        }

        public IComponent Component
        {
            get
            {
                return this.component;
            }
        }

        public IContainer Container
        {
            get
            {
                return null;
            }
        }

        public bool DesignMode
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
    }
}

