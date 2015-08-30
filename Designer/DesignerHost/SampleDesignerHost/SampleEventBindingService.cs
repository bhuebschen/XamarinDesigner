namespace SampleDesignerHost
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Text;

    public class SampleEventBindingService : IEventBindingService
    {
        private Hashtable _eventProperties;
        private IServiceProvider _provider;

        public SampleEventBindingService(IServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            this._provider = provider;
        }

        protected string CreateUniqueMethodName(IComponent component, EventDescriptor e)
        {
            string name = component.ToString().Split(new char[] { ' ' })[0];
            return ("handler_" + name + "_" + e.Name);
        }

        protected virtual void FreeMethod(object component, EventDescriptor e, string methodName)
        {
        }

        protected ICollection GetCompatibleMethods(EventDescriptor e)
        {
            return new string[0];
        }

        private string GetEventDescriptorHashCode(EventDescriptor eventDesc)
        {
            StringBuilder builder = new StringBuilder(eventDesc.Name);
            builder.Append(eventDesc.EventType.GetHashCode().ToString());
            foreach (Attribute a in eventDesc.Attributes)
            {
                builder.Append(a.GetHashCode().ToString());
            }
            return builder.ToString();
        }

        protected object GetService(Type serviceType)
        {
            if (this._provider != null)
            {
                return this._provider.GetService(serviceType);
            }
            return null;
        }

        protected bool ShowCode()
        {
            return false;
        }

        protected bool ShowCode(int lineNumber)
        {
            return false;
        }

        protected bool ShowCode(object component, EventDescriptor e, string methodName)
        {
            return false;
        }

        string IEventBindingService.CreateUniqueMethodName(IComponent component, EventDescriptor e)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            return this.CreateUniqueMethodName(component, e);
        }

        ICollection IEventBindingService.GetCompatibleMethods(EventDescriptor e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            return this.GetCompatibleMethods(e);
        }

        EventDescriptor IEventBindingService.GetEvent(PropertyDescriptor property)
        {
            if (property is EventPropertyDescriptor)
            {
                return ((EventPropertyDescriptor) property).Event;
            }
            return null;
        }

        PropertyDescriptorCollection IEventBindingService.GetEventProperties(EventDescriptorCollection events)
        {
            if (events == null)
            {
                throw new ArgumentNullException("events");
            }
            PropertyDescriptor[] props = new PropertyDescriptor[events.Count];
            if (this._eventProperties == null)
            {
                this._eventProperties = new Hashtable();
            }
            for (int i = 0; i < events.Count; i++)
            {
                object eventHashCode = this.GetEventDescriptorHashCode(events[i]);
                props[i] = (PropertyDescriptor) this._eventProperties[eventHashCode];
                if (props[i] == null)
                {
                    props[i] = new EventPropertyDescriptor(events[i], this);
                    this._eventProperties[eventHashCode] = props[i];
                }
            }
            return new PropertyDescriptorCollection(props);
        }

        PropertyDescriptor IEventBindingService.GetEventProperty(EventDescriptor e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (this._eventProperties == null)
            {
                this._eventProperties = new Hashtable();
            }
            object eventHashCode = this.GetEventDescriptorHashCode(e);
            PropertyDescriptor prop = (PropertyDescriptor) this._eventProperties[eventHashCode];
            if (prop == null)
            {
                prop = new EventPropertyDescriptor(e, this);
                this._eventProperties[eventHashCode] = prop;
            }
            return prop;
        }

        bool IEventBindingService.ShowCode()
        {
            return this.ShowCode();
        }

        bool IEventBindingService.ShowCode(int lineNumber)
        {
            return this.ShowCode(lineNumber);
        }

        bool IEventBindingService.ShowCode(IComponent component, EventDescriptor e)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            string methodName = (string) ((IEventBindingService) this).GetEventProperty(e).GetValue(component);
            if (methodName == null)
            {
                return false;
            }
            return this.ShowCode(component, e, methodName);
        }

        protected virtual void UseMethod(object component, EventDescriptor e, string methodName)
        {
        }

        protected virtual void ValidateMethodName(string methodName)
        {
        }

        private class EventPropertyDescriptor : PropertyDescriptor
        {
            private TypeConverter _converter;
            private EventDescriptor _eventDesc;
            private SampleEventBindingService _eventSvc;

            internal EventPropertyDescriptor(EventDescriptor eventDesc, SampleEventBindingService eventSvc) : base(eventDesc, null)
            {
                this._eventDesc = eventDesc;
                this._eventSvc = eventSvc;
            }

            public override bool CanResetValue(object component)
            {
                return (this.GetValue(component) != null);
            }

            public override object GetValue(object component)
            {
                Exception ex;
                if (component == null)
                {
                    throw new ArgumentNullException("component");
                }
                ISite site = null;
                if (component is IComponent)
                {
                    site = ((IComponent) component).Site;
                }
                if (site == null)
                {
                    IReferenceService rs = this._eventSvc._provider.GetService(typeof(IReferenceService)) as IReferenceService;
                    if (rs != null)
                    {
                        IComponent baseComponent = rs.GetComponent(component);
                        if (baseComponent != null)
                        {
                            site = baseComponent.Site;
                        }
                    }
                }
                if (site == null)
                {
                    ex = new InvalidOperationException("There is no site for component" + component.ToString() + ".");
                    throw ex;
                }
                IDictionaryService ds = (IDictionaryService) site.GetService(typeof(IDictionaryService));
                if (ds == null)
                {
                    ex = new InvalidOperationException("Cannot find IDictionaryService.");
                    throw ex;
                }
                return (string) ds.GetValue(new ReferenceEventClosure(component, this));
            }

            public override void ResetValue(object component)
            {
                this.SetValue(component, null);
            }

            public override void SetValue(object component, object value)
            {
                Exception ex;
                if (this.IsReadOnly)
                {
                    ex = new InvalidOperationException("Tried to set a read only event.");
                    throw ex;
                }
                if (!((value == null) || (value is string)))
                {
                    ex = new ArgumentException("Cannot set to value " + value.ToString() + ".");
                    throw ex;
                }
                string name = (string) value;
                if ((name != null) && (name.Length == 0))
                {
                    name = null;
                }
                ISite site = null;
                if (component is IComponent)
                {
                    site = ((IComponent) component).Site;
                }
                if (site == null)
                {
                    IReferenceService rs = this._eventSvc._provider.GetService(typeof(IReferenceService)) as IReferenceService;
                    if (rs != null)
                    {
                        IComponent baseComponent = rs.GetComponent(component);
                        if (baseComponent != null)
                        {
                            site = baseComponent.Site;
                        }
                    }
                }
                if (site == null)
                {
                    ex = new InvalidOperationException("There is no site for component " + component.ToString() + ".");
                    throw ex;
                }
                IDictionaryService ds = (IDictionaryService) site.GetService(typeof(IDictionaryService));
                if (ds == null)
                {
                    ex = new InvalidOperationException("Cannot find IDictionaryService");
                    throw ex;
                }
                ReferenceEventClosure key = new ReferenceEventClosure(component, this);
                string oldName = (string) ds.GetValue(key);
                if (!object.ReferenceEquals(oldName, name) && (((oldName == null) || (name == null)) || !oldName.Equals(name)))
                {
                    if (name != null)
                    {
                        this._eventSvc.ValidateMethodName(name);
                    }
                    IComponentChangeService change = (IComponentChangeService) site.GetService(typeof(IComponentChangeService));
                    if (change != null)
                    {
                        try
                        {
                            change.OnComponentChanging(component, this);
                        }
                        catch (CheckoutException coEx)
                        {
                            if (coEx != CheckoutException.Canceled)
                            {
                                throw;
                            }
                            return;
                        }
                    }
                    if (name != null)
                    {
                        this._eventSvc.UseMethod(component, this._eventDesc, name);
                    }
                    if (oldName != null)
                    {
                        this._eventSvc.FreeMethod(component, this._eventDesc, oldName);
                    }
                    ds.SetValue(key, name);
                    if (change != null)
                    {
                        change.OnComponentChanged(component, this, oldName, name);
                    }
                    this.OnValueChanged(component, EventArgs.Empty);
                }
            }

            public override bool ShouldSerializeValue(object component)
            {
                return this.CanResetValue(component);
            }

            public override Type ComponentType
            {
                get
                {
                    return this._eventDesc.ComponentType;
                }
            }

            public override TypeConverter Converter
            {
                get
                {
                    if (this._converter == null)
                    {
                        this._converter = new EventConverter(this._eventDesc);
                    }
                    return this._converter;
                }
            }

            internal EventDescriptor Event
            {
                get
                {
                    return this._eventDesc;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return this.Attributes[typeof(ReadOnlyAttribute)].Equals(ReadOnlyAttribute.Yes);
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return this._eventDesc.EventType;
                }
            }

            private class EventConverter : TypeConverter
            {
                private EventDescriptor _evt;

                internal EventConverter(EventDescriptor evt)
                {
                    this._evt = evt;
                }

                public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
                {
                    return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
                }

                public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
                {
                    return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
                }

                public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
                {
                    if (value == null)
                    {
                        return value;
                    }
                    if (value is string)
                    {
                        if (((string) value).Length == 0)
                        {
                            return null;
                        }
                        return value;
                    }
                    return base.ConvertFrom(context, culture, value);
                }

                public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
                {
                    if (destinationType == typeof(string))
                    {
                        return ((value == null) ? string.Empty : value);
                    }
                    return base.ConvertTo(context, culture, value, destinationType);
                }

                public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
                {
                    string[] eventMethods = null;
                    if (context != null)
                    {
                        IEventBindingService ebs = (IEventBindingService) context.GetService(typeof(IEventBindingService));
                        if (ebs != null)
                        {
                            ICollection methods = ebs.GetCompatibleMethods(this._evt);
                            eventMethods = new string[methods.Count];
                            int i = 0;
                            foreach (string s in methods)
                            {
                                eventMethods[i++] = s;
                            }
                        }
                    }
                    return new TypeConverter.StandardValuesCollection(eventMethods);
                }

                public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
                {
                    return false;
                }

                public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
                {
                    return true;
                }
            }

            private class ReferenceEventClosure
            {
                private SampleEventBindingService.EventPropertyDescriptor propertyDescriptor;
                private object reference;

                public ReferenceEventClosure(object reference, SampleEventBindingService.EventPropertyDescriptor prop)
                {
                    this.reference = reference;
                    this.propertyDescriptor = prop;
                }

                public override bool Equals(object otherClosure)
                {
                    if (otherClosure is SampleEventBindingService.EventPropertyDescriptor.ReferenceEventClosure)
                    {
                        SampleEventBindingService.EventPropertyDescriptor.ReferenceEventClosure typedClosure = (SampleEventBindingService.EventPropertyDescriptor.ReferenceEventClosure) otherClosure;
                        return ((typedClosure.reference == this.reference) && (typedClosure.propertyDescriptor == this.propertyDescriptor));
                    }
                    return false;
                }

                public override int GetHashCode()
                {
                    return (this.reference.GetHashCode() * this.propertyDescriptor.GetHashCode());
                }
            }
        }
    }
}

