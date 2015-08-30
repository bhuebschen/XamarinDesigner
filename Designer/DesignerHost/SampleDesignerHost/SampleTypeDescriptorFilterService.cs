namespace SampleDesignerHost
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;

    public class SampleTypeDescriptorFilterService : ITypeDescriptorFilterService
    {
        public IDesignerHost host;

        public SampleTypeDescriptorFilterService(IDesignerHost host)
        {
            this.host = host;
        }

        public bool FilterAttributes(IComponent component, IDictionary attributes)
        {
            IDesignerFilter filter = this.GetDesignerFilter(component);
            if (filter != null)
            {
                filter.PreFilterAttributes(attributes);
                filter.PostFilterAttributes(attributes);
                return true;
            }
            return false;
        }

        public bool FilterEvents(IComponent component, IDictionary events)
        {
            IDesignerFilter filter = this.GetDesignerFilter(component);
            if (filter != null)
            {
                filter.PreFilterEvents(events);
                filter.PostFilterEvents(events);
                return true;
            }
            return false;
        }

        public bool FilterProperties(IComponent component, IDictionary properties)
        {
            IDesignerFilter filter = this.GetDesignerFilter(component);
            if (filter != null)
            {
                filter.PreFilterProperties(properties);
                filter.PostFilterProperties(properties);
                return true;
            }
            return false;
        }

        private IDesignerFilter GetDesignerFilter(IComponent component)
        {
            return (this.host.GetDesigner(component) as IDesignerFilter);
        }
    }
}

