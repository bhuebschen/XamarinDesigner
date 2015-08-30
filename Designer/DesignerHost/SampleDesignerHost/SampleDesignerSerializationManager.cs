namespace SampleDesignerHost
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;
    //using IDesignerSerializationManager;

    internal class SampleDesignerSerializationManager : IDesignerSerializationManager, IServiceProvider
    {
        private ContextStack _contextStack;
        private ArrayList _designerSerializationProviders;
        private ArrayList _errorList;
        private Hashtable _instancesByName;
        private SampleDesignerLoader _loader;
        private Hashtable _namesByInstance;
        private PropertyDescriptorCollection _propertyCollection;
        private Hashtable _serializers;

        public event ResolveNameEventHandler ResolveName;

        public event EventHandler SerializationComplete;

        internal SampleDesignerSerializationManager(SampleDesignerLoader loader)
        {
            this._loader = loader;
        }

        internal void AddErrors(ICollection errors)
        {
            if ((errors != null) && (errors.Count > 0))
            {
                if (this._errorList == null)
                {
                    this._errorList = new ArrayList();
                }
                this._errorList.AddRange(errors);
            }
        }

        internal void Initialize()
        {
            if (this._errorList != null)
            {
                this._errorList.Clear();
            }
        }

        void IDesignerSerializationManager.AddSerializationProvider(IDesignerSerializationProvider provider)
        {
            if (this._designerSerializationProviders == null)
            {
                this._designerSerializationProviders = new ArrayList();
            }
            this._designerSerializationProviders.Add(provider);
        }

        object IDesignerSerializationManager.CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
        {
            object[] argArray = null;
            if ((arguments != null) && (arguments.Count > 0))
            {
                argArray = new object[arguments.Count];
                arguments.CopyTo(argArray, 0);
            }
            object instance = null;
            if (addToContainer && typeof(IComponent).IsAssignableFrom(type))
            {
                IDesignerLoaderHost host = this._loader.LoaderHost;
                if (host != null)
                {
                    if (name == null)
                    {
                        instance = host.CreateComponent(type);
                    }
                    else
                    {
                        instance = host.CreateComponent(type, name);
                    }
                }
            }
            if (instance == null)
            {
                Exception ex;
                if (((name != null) && (this._instancesByName != null)) && this._instancesByName.ContainsKey(name))
                {
                    ex = new InvalidOperationException("Duplicate component declaration for " + name + ".");
                    throw ex;
                }
                try
                {
                    instance = Activator.CreateInstance(type, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, argArray, null);
                }
                catch (MissingMethodException)
                {
                    StringBuilder argTypes = new StringBuilder();
                    foreach (object o in argArray)
                    {
                        if (argTypes.Length > 0)
                        {
                            argTypes.Append(", ");
                        }
                        if (o != null)
                        {
                            argTypes.Append(o.GetType().Name);
                        }
                        else
                        {
                            argTypes.Append("null");
                        }
                    }
                    ex = new InvalidOperationException("No matching constructor for " + type.FullName + "(" + argTypes.ToString() + ")");
                    throw ex;
                }
                if (name == null)
                {
                    return instance;
                }
                if (this._instancesByName == null)
                {
                    this._instancesByName = new Hashtable();
                    this._namesByInstance = new Hashtable();
                }
                this._instancesByName[name] = instance;
                this._namesByInstance[instance] = name;
            }
            return instance;
        }

        object IDesignerSerializationManager.GetInstance(string name)
        {
            object instance = null;
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (this._instancesByName != null)
            {
                instance = this._instancesByName[name];
            }
            if ((instance == null) && (this._loader.LoaderHost != null))
            {
                instance = this._loader.LoaderHost.Container.Components[name];
            }
            if ((instance == null) && (this.ResolveName != null))
            {
                ResolveNameEventArgs e = new ResolveNameEventArgs(name);
                this.ResolveName(this, e);
                instance = e.Value;
            }
            return instance;
        }

        string IDesignerSerializationManager.GetName(object value)
        {
            string name = null;
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (this._namesByInstance != null)
            {
                name = (string) this._namesByInstance[value];
            }
            if ((name == null) && (value is IComponent))
            {
                ISite site = ((IComponent) value).Site;
                if (site != null)
                {
                    name = site.Name;
                }
            }
            return name;
        }

        object IDesignerSerializationManager.GetSerializer(Type objectType, Type serializerType)
        {
            object serializer = null;
            if (objectType != null)
            {
                if (this._serializers != null)
                {
                    serializer = this._serializers[objectType];
                    if (!((serializer == null) || serializerType.IsAssignableFrom(serializer.GetType())))
                    {
                        serializer = null;
                    }
                }
                IDesignerLoaderHost host = this._loader.LoaderHost;
                if ((serializer == null) && (host != null))
                {
                    AttributeCollection attributes = TypeDescriptor.GetAttributes(objectType);
                    foreach (Attribute attr in attributes)
                    {
                        if (attr is DesignerSerializerAttribute)
                        {
                            DesignerSerializerAttribute da = (DesignerSerializerAttribute) attr;
                            string typeName = da.SerializerBaseTypeName;
                            if ((((typeName != null) && (host.GetType(typeName) == serializerType)) && (da.SerializerTypeName != null)) && (da.SerializerTypeName.Length > 0))
                            {
                                Type type = host.GetType(da.SerializerTypeName);
                                Debug.Assert(type != null, "Type " + objectType.FullName + " has a serializer that we couldn't bind to: " + da.SerializerTypeName);
                                if (type != null)
                                {
                                    serializer = Activator.CreateInstance(type, BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
                                    break;
                                }
                            }
                        }
                    }
                    if (serializer != null)
                    {
                        if (this._serializers == null)
                        {
                            this._serializers = new Hashtable();
                        }
                        this._serializers[objectType] = serializer;
                    }
                }
            }
            if (this._designerSerializationProviders != null)
            {
                bool continueLoop = true;
                for (int i = 0; continueLoop && (i < this._designerSerializationProviders.Count); i++)
                {
                    continueLoop = false;
                    foreach (IDesignerSerializationProvider provider in this._designerSerializationProviders)
                    {
                        object newSerializer = provider.GetSerializer(this, serializer, objectType, serializerType);
                        if (newSerializer != null)
                        {
                            continueLoop = serializer != newSerializer;
                            serializer = newSerializer;
                        }
                    }
                }
            }
            return serializer;
        }

        Type IDesignerSerializationManager.GetType(string typeName)
        {
            Type t = null;
            if (this._loader.LoaderHost != null)
            {
                while (t == null)
                {
                    t = this._loader.LoaderHost.GetType(typeName);
                    if (((t == null) && (typeName != null)) && (typeName.Length > 0))
                    {
                        int dotIndex = typeName.LastIndexOf('.');
                        if ((dotIndex == -1) || (dotIndex == (typeName.Length - 1)))
                        {
                            return t;
                        }
                        typeName = typeName.Substring(0, dotIndex) + "+" + typeName.Substring(dotIndex + 1, (typeName.Length - dotIndex) - 1);
                    }
                }
            }
            return t;
        }

        void IDesignerSerializationManager.RemoveSerializationProvider(IDesignerSerializationProvider provider)
        {
            if (this._designerSerializationProviders != null)
            {
                this._designerSerializationProviders.Remove(provider);
            }
        }

        void IDesignerSerializationManager.ReportError(object errorInformation)
        {
            if (errorInformation != null)
            {
                if (this._errorList == null)
                {
                    this._errorList = new ArrayList();
                }
                this._errorList.Add(errorInformation);
            }
        }

        void IDesignerSerializationManager.SetName(object instance, string name)
        {
            Exception ex;
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (this._instancesByName == null)
            {
                this._instancesByName = new Hashtable();
                this._namesByInstance = new Hashtable();
            }
            if (this._instancesByName[name] != null)
            {
                ex = new ArgumentException("Designer Loader name " + name + " in use.");
                throw ex;
            }
            if (this._namesByInstance[instance] != null)
            {
                ex = new ArgumentException("Designer loader object has name " + name + ".");
                throw ex;
            }
            this._instancesByName[name] = instance;
            this._namesByInstance[instance] = name;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return this._loader.GetService(serviceType);
        }

        internal ICollection Terminate(ICollection errors)
        {
            try
            {
                if (this.SerializationComplete != null)
                {
                    this.SerializationComplete(this, EventArgs.Empty);
                }
            }
            catch
            {
            }
            if ((this._errorList != null) && (this._errorList.Count > 0))
            {
                if ((errors != null) && (errors.Count > 0))
                {
                    this._errorList.AddRange(errors);
                }
                errors = this._errorList;
            }
            this.ResolveName = null;
            this.SerializationComplete = null;
            this._instancesByName = null;
            this._namesByInstance = null;
            this._serializers = null;
            this._errorList = null;
            this._contextStack = null;
            return errors;
        }

        ContextStack IDesignerSerializationManager.Context
        {
            get
            {
                if (this._contextStack == null)
                {
                    this._contextStack = new ContextStack();
                }
                return this._contextStack;
            }
        }

        PropertyDescriptorCollection IDesignerSerializationManager.Properties
        {
            get
            {
                if (this._propertyCollection == null)
                {
                    this._propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
                }
                return this._propertyCollection;
            }
        }
    }
}

