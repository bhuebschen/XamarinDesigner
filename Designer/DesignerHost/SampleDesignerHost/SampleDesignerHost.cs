namespace SampleDesignerHost
{
    using SampleDesignerApplication;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Drawing.Design;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    public class SampleDesignerHost : IContainer, IDisposable, IDesignerLoaderHost, IDesignerHost, IServiceContainer, IServiceProvider, IComponentChangeService, IExtenderProviderService, IDesignerEventService
    {
        private DesignerComponentCollection components;
        private DesignerLoader designerLoader;
        private IDesignerSerializationService designerSerialService;
        private Hashtable designerTable;
        private SampleDocumentWindow documentWindow;
        private ArrayList extenderProviders;
        private IHelpService helpService;
        private Exception loadError;
        private bool loadingDesigner;
        private IMenuCommandService menuCommandService;
        private IMenuEditorService menuEditorService;
        private INameCreationService nameService;
        private SampleDesignSite newComponentSite;
        private IReferenceService referenceService;
        private bool reloading;
        private IComponent rootComponent;
        private string rootComponentClassName;
        private IRootDesigner rootDesigner;
        private ISelectionService selectionService;
        private IServiceContainer serviceContainer;
        private Hashtable sites;
        private IToolboxService toolboxService;
        private int transactionCount;
        private StringStack transactionDescriptions;
        private ITypeResolutionService typeResolver;

        public event EventHandler Activated;

        public event ActiveDesignerEventHandler ActiveDesignerChanged;

        public event ComponentEventHandler ComponentAdded;

        public event ComponentEventHandler ComponentAdding;

        public event ComponentChangedEventHandler ComponentChanged;

        public event ComponentChangingEventHandler ComponentChanging;

        public event ComponentEventHandler ComponentRemoved;

        public event ComponentEventHandler ComponentRemoving;

        public event ComponentRenameEventHandler ComponentRename;

        public event EventHandler Deactivated;

        public event DesignerEventHandler DesignerCreated;

        public event DesignerEventHandler DesignerDisposed;

        public event EventHandler LoadComplete;

        public event EventHandler SelectionChanged;

        public event DesignerTransactionCloseEventHandler TransactionClosed;

        public event DesignerTransactionCloseEventHandler TransactionClosing;

        public event EventHandler TransactionOpened;

        public event EventHandler TransactionOpening;

        public SampleDesignerHost() : this(new ServiceContainer())
        {
        }

        public SampleDesignerHost(IServiceProvider parentProvider)
        {
            this.serviceContainer = new ServiceContainer(parentProvider);
            this.designerTable = new Hashtable();
            this.sites = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
            this.loadingDesigner = false;
            this.transactionCount = 0;
            this.reloading = false;
            this.serviceContainer.AddService(typeof(IDesignerHost), this);
            this.serviceContainer.AddService(typeof(IContainer), this);
            this.serviceContainer.AddService(typeof(IComponentChangeService), this);
            this.serviceContainer.AddService(typeof(IExtenderProviderService), this);
            this.serviceContainer.AddService(typeof(IDesignerEventService), this);
            CodeDomComponentSerializationService codeDomComponentSerializationService = new CodeDomComponentSerializationService(this.serviceContainer);
            if (codeDomComponentSerializationService != null)
            {
                this.serviceContainer.RemoveService(typeof(ComponentSerializationService), false);
                this.serviceContainer.AddService(typeof(ComponentSerializationService), codeDomComponentSerializationService);
            }
            ServiceCreatorCallback callback = new ServiceCreatorCallback(this.OnCreateService);
            this.serviceContainer.AddService(typeof(IToolboxService), callback);
            this.serviceContainer.AddService(typeof(ISelectionService), callback);
            this.serviceContainer.AddService(typeof(ITypeDescriptorFilterService), callback);
            this.serviceContainer.AddService(typeof(IMenuCommandService), callback);
            this.serviceContainer.AddService(typeof(IDesignerSerializationService), callback);
            ((IExtenderProviderService) this).AddExtenderProvider(new SampleNameExtenderProvider(this));
            ((IExtenderProviderService) this).AddExtenderProvider(new SampleInheritedNameExtenderProvider(this));
        }

        public void Activate()
        {
            this.documentWindow.Focus();
        }

        internal bool CheckName(string name)
        {
            if ((name == null) || (name.Length == 0))
            {
                Exception ex = new Exception("Components must have a name");
                throw ex;
            }
            if (((IContainer) this).Components[name] != null)
            {
                return false;
            }
            if (this.nameService == null)
            {
                IServiceProvider sp = this;
                this.nameService = (INameCreationService) sp.GetService(typeof(INameCreationService));
            }
            if (this.nameService != null)
            {
                this.nameService.ValidateName(name);
            }
            return true;
        }

        public IComponent CreateComponent(System.Type componentClass)
        {
            string name = this.GetNewComponentName(componentClass);
            return this.CreateComponent(componentClass, name);
        }

        public IComponent CreateComponent(System.Type componentClass, string name)
        {
            object obj = null;
            IComponent comp = null;
            this.newComponentSite = new SampleDesignSite(this, name);
            try
            {
                try
                {
                    object[] args = new object[] { this };
                    System.Type[] argTypes = new System.Type[] { typeof(IContainer) };
                    obj = this.CreateObject(componentClass, args, argTypes, false);
                }
                catch (Exception)
                {
                }
                if (null == obj)
                {
                    obj = this.CreateObject(componentClass, null, null, false);
                }
                comp = obj as Component;
                if (comp == null)
                {
                    Exception ex = new Exception("The class is not a component " + componentClass.FullName);
                    throw ex;
                }
                SampleDesignSite site = comp.Site as SampleDesignSite;
                if (site == null)
                {
                    ((IContainer) this).Add(comp);
                }
                Debug.Assert(this.newComponentSite == null, "add didn't use newComponentSite");
            }
            catch (Exception)
            {
                if (comp != null)
                {
                    try
                    {
                        this.DestroyComponent(comp);
                    }
                    catch (Exception)
                    {
                    }
                }
                throw;
            }
            return comp;
        }

        private void CreateComponentDesigner(IComponent component)
        {
            IDesigner designer = null;
            if (this.rootComponent != null)
            {
                designer = TypeDescriptor.CreateDesigner(component, typeof(IDesigner));
            }
            else
            {
                Exception ex;
                this.rootComponent = component;
                this.rootDesigner = (IRootDesigner) TypeDescriptor.CreateDesigner(component, typeof(IRootDesigner));
                if (this.rootDesigner == null)
                {
                    this.rootComponent = null;
                    ex = new Exception("No Top Level Designer for " + component.GetType().FullName);
                    throw ex;
                }
                ViewTechnology[] technologies = this.rootDesigner.SupportedTechnologies;
                bool supported = false;
                foreach (ViewTechnology tech in technologies)
                {
                    if (tech == ViewTechnology.Default)
                    {
                        supported = true;
                        break;
                    }
                }
                if (!supported)
                {
                    ex = new Exception("This designer host does not support the designer for " + component.GetType().FullName);
                    throw ex;
                }
                designer = this.rootDesigner;
                if (this.rootComponentClassName == null)
                {
                    this.rootComponentClassName = component.Site.Name;
                }
            }
            if (designer != null)
            {
                this.designerTable[component] = designer;
                try
                {
                    designer.Initialize(component);
                }
                catch
                {
                    this.designerTable.Remove(component);
                    if (designer == this.rootDesigner)
                    {
                        this.rootDesigner = null;
                    }
                    throw;
                }
                if (!(!(designer is IExtenderProvider) || TypeDescriptor.GetAttributes(designer).Contains(InheritanceAttribute.InheritedReadOnly)))
                {
                    ((IExtenderProviderService) this).AddExtenderProvider((IExtenderProvider) designer);
                }
                if (designer == this.rootDesigner)
                {
                    this.documentWindow.SetDesigner(this.rootDesigner);
                }
            }
        }

        private object CreateObject(System.Type objectClass, object[] args, System.Type[] argTypes, bool fThrowException)
        {
            ConstructorInfo ctr = null;
            object cObject;
            if ((args != null) && (args.Length > 0))
            {
                if (argTypes == null)
                {
                    argTypes = new System.Type[args.Length];
                    for (int i = args.Length - 1; i >= 0; i--)
                    {
                        if (args[i] != null)
                        {
                            argTypes[i] = args[i].GetType();
                        }
                    }
                }
                if ((objectClass.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, argTypes, null) == null) && fThrowException)
                {
                    Exception ex = new Exception("Cannot find a constructor with the right arguments for " + objectClass.FullName);
                    throw ex;
                }
                return null;
            }
            try
            {
                cObject = (ctr == null) ? Activator.CreateInstance(objectClass) : ctr.Invoke(args);
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                {
                    e = e.InnerException;
                }
                if (e.Message == null)
                {
                    string message = e.ToString();
                }
                throw new Exception("Cannot create an instance of " + objectClass.FullName + " because " + e.ToString(), e);
            }
            return cObject;
        }

        public DesignerTransaction CreateTransaction()
        {
            return this.CreateTransaction(null);
        }

        public DesignerTransaction CreateTransaction(string description)
        {
            if (description == null)
            {
                description = string.Empty;
            }
            return new SampleDesignerTransaction(this, description);
        }

        public void DestroyComponent(IComponent comp)
        {
            string name;
            if ((comp.Site != null) && (comp.Site.Name != null))
            {
                name = comp.Site.Name;
            }
            else
            {
                name = comp.GetType().Name;
            }
            InheritanceAttribute ia = (InheritanceAttribute) TypeDescriptor.GetAttributes(comp)[typeof(InheritanceAttribute)];
            if ((ia != null) && (ia.InheritanceLevel != InheritanceLevel.NotInherited))
            {
                throw new InvalidOperationException("CantDestroyInheritedComponent" + name);
            }
            DesignerTransaction t = null;
            try
            {
                t = this.CreateTransaction("DestroyComponentTransaction" + name);
                ((IComponentChangeService) this).OnComponentChanging(comp, null);
                if (comp.Site != null)
                {
                    this.Remove(comp);
                }
                comp.Dispose();
                ((IComponentChangeService) this).OnComponentChanged(comp, null, null, null);
            }
            finally
            {
                if (t != null)
                {
                    t.Commit();
                }
            }
        }

        public void Dispose()
        {
            IDisposable d;
            if (this.designerLoader != null)
            {
                try
                {
                    this.designerLoader.Flush();
                }
                catch (Exception e1)
                {
                    Debug.Fail("Designer loader '" + this.designerLoader.GetType().Name + "' threw during Flush: " + e1.ToString());
                    e1 = null;
                }
                try
                {
                    this.designerLoader.Dispose();
                }
                catch (Exception e2)
                {
                    Debug.Fail("Designer loader '" + this.designerLoader.GetType().Name + "' threw during Dispose: " + e2.ToString());
                    e2 = null;
                }
                this.designerLoader = null;
            }
            this.UnloadDocument();
            this.serviceContainer = null;
            if (this.menuEditorService != null)
            {
                d = this.menuEditorService as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
                this.menuEditorService = null;
            }
            if (this.selectionService != null)
            {
                d = this.selectionService as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
                this.selectionService = null;
            }
            if (this.menuCommandService != null)
            {
                d = this.menuCommandService as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
                this.menuCommandService = null;
            }
            if (this.toolboxService != null)
            {
                d = this.toolboxService as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
                this.toolboxService = null;
            }
            if (this.helpService != null)
            {
                d = this.helpService as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
                this.helpService = null;
            }
            if (this.referenceService != null)
            {
                d = this.referenceService as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
                this.referenceService = null;
            }
            if (this.documentWindow != null)
            {
                this.documentWindow.Dispose();
                this.documentWindow = null;
            }
        }

        public void DoCommand(string Command)
        {
            switch (Command)
            {
                case "copy":
                    this.menuCommandService.GlobalInvoke(StandardCommands.Copy);
                    break;

                case "cut":
                    this.menuCommandService.GlobalInvoke(StandardCommands.Cut);
                    break;
                case "paste2":
                    break;

                case "paste":
                    //ControlFactory.GetCtrlFromClipBoard(this);
                    this.menuCommandService.GlobalInvoke(StandardCommands.Paste);
                    //CBFormCtrl cbCtrl = ido.GetData("CF_DESIGNERCOMPONENTS_V2") as CBFormCtrl;
                    //ctrl = (Control)host.CreateComponent(cbCtrl.GetType(), cbCtrl.PartialName);
                    //ControlFactory.SetControlProperties(ctrl, cbCtrl.PropertyList);

                    break;

                case "delete":
                    this.menuCommandService.GlobalInvoke(StandardCommands.Delete);
                    break;

                case "selall":
                    this.menuCommandService.GlobalInvoke(StandardCommands.SelectAll);
                    break;

                case "undo":
                    this.menuCommandService.GlobalInvoke(StandardCommands.Undo);
                    break;

                case "redo":
                    this.menuCommandService.GlobalInvoke(StandardCommands.Redo);
                    break;
                case "viewgrid":
                    this.menuCommandService.GlobalInvoke(StandardCommands.ViewGrid);
                    break;
                case "showgrid":
                    this.menuCommandService.GlobalInvoke(StandardCommands.ShowGrid);
                    break;
                default:
                    this.menuCommandService.GlobalInvoke((System.ComponentModel.Design.CommandID)Enum.Parse(typeof(StandardCommands), Command));
                    break;
            }
        }

        public IDesigner GetDesigner(IComponent component)
        {
           // Debug.Assert(component != null, "Cannot call GetDesigner with a NULL component.  Check that the root hasn't been disposed.");
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }
            return (IDesigner) this.designerTable[component];
        }

        internal string GetNewComponentName(System.Type compClass)
        {
            IServiceProvider sp = this;
            INameCreationService nameCreate = (INameCreationService) sp.GetService(typeof(INameCreationService));
            if (nameCreate != null)
            {
                return nameCreate.CreateName(this.Container, compClass);
            }
            string baseName = compClass.Name;
            StringBuilder b = new StringBuilder(baseName.Length);
            for (int i = 0; i < baseName.Length; i++)
            {
                if (char.IsUpper(baseName[i]) && (((i == 0) || (i == (baseName.Length - 1))) || char.IsUpper(baseName[i + 1])))
                {
                    b.Append(char.ToLower(baseName[i]));
                }
                else
                {
                    b.Append(baseName.Substring(i));
                    break;
                }
            }
            baseName = b.ToString();
            int idx = 1;
            string finalName = baseName + idx.ToString();
            while (this.Container.Components[finalName] != null)
            {
                idx++;
                finalName = baseName + idx.ToString();
            }
            return finalName;
        }

        public System.Type GetType(string typeName)
        {
            if (this.typeResolver == null)
            {
                IServiceProvider sp = this;
                this.typeResolver = (ITypeResolutionService) sp.GetService(typeof(ITypeResolutionService));
            }
            if (this.typeResolver != null)
            {
                return this.typeResolver.GetType(typeName);
            }
            return System.Type.GetType(typeName);
        }

        private void Load(bool reloading)
        {
            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            this.reloading = reloading;
            try
            {
                if (!(reloading || !(this.designerLoader is IExtenderProvider)))
                {
                    ((IExtenderProviderService) this).AddExtenderProvider((IExtenderProvider) this.designerLoader);
                }
                this.loadingDesigner = true;
                this.loadError = null;
                this.designerLoader.BeginLoad(this);
            }
            catch (Exception e)
            {
                Exception exNew = e;
                if (e is TargetInvocationException)
                {
                    exNew = e.InnerException;
                }
                string message = exNew.Message;
                if ((message == null) || (message.Length == 0))
                {
                    Debug.Fail("Parser has thrown an exception that has no friendly message", exNew.ToString());
                    exNew = new Exception("Parser has thrown an exception that has no friendly message" + exNew.ToString());
                }
                ArrayList errorList = new ArrayList();
                errorList.Add(exNew);
                ((IDesignerLoaderHost) this).EndLoad(null, false, errorList);
            }
            Cursor.Current = oldCursor;
        }

        public void LoadDocument(DesignerLoader designerLoader)
        {
            try
            {
                this.designerLoader = designerLoader;
                this.documentWindow = new SampleDocumentWindow(this);
                this.Load(false);
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                throw;
            }
        }

        private void OnComponentAdded(ComponentEventArgs ce)
        {
            if (this.ComponentAdded != null)
            {
                (((IServiceProvider) this).GetService(typeof(ITypeResolutionService)) as ITypeResolutionService).ReferenceAssembly(ce.Component.GetType().Assembly.GetName());
                this.ComponentAdded(this, ce);
            }
        }

        private void OnComponentAdding(ComponentEventArgs ce)
        {
            if (this.ComponentAdding != null)
            {
                this.ComponentAdding(this, ce);
            }
        }

        private void OnComponentRemoved(ComponentEventArgs ce)
        {
            if (this.ComponentRemoved != null)
            {
                this.ComponentRemoved(this, ce);
            }
        }

        private void OnComponentRemoving(ComponentEventArgs ce)
        {
            if (this.ComponentRemoving != null)
            {
                this.ComponentRemoving(this, ce);
            }
        }

        internal void OnComponentRename(ComponentRenameEventArgs ce)
        {
            if (this.ComponentRename != null)
            {
                this.ComponentRename(this, ce);
            }
        }

        private object OnCreateService(IServiceContainer container, System.Type serviceType)
        {
            if (serviceType == typeof(ISelectionService))
            {
                if (this.selectionService == null)
                {
                    this.selectionService = new SampleSelectionService(this);
                }
                return this.selectionService;
            }
            if (serviceType == typeof(IDesignerSerializationService))
            {
                if (this.designerSerialService == null)
                {
                    this.designerSerialService = new DesignerSerializationService(this);
                }
                return this.designerSerialService;
            }
            if (serviceType == typeof(ITypeDescriptorFilterService))
            {
                return new SampleTypeDescriptorFilterService(this);
            }
            if (serviceType == typeof(IToolboxService))
            {
                if (this.toolboxService == null)
                {
                    this.toolboxService = new SampleToolboxService(this);
                }
                return this.toolboxService;
            }
            if (serviceType == typeof(IMenuCommandService))
            {
                if (this.menuCommandService == null)
                {
                    this.menuCommandService = new SampleMenuCommandService(this);
                }
                return this.menuCommandService;
            }
            Debug.Fail("Service type " + serviceType.FullName + " requested but we don't support it");
            return null;
        }

        internal void OnTransactionClosed(DesignerTransactionCloseEventArgs e)
        {
            if (this.TransactionClosed != null)
            {
                this.TransactionClosed(this, e);
            }
        }

        internal void OnTransactionClosing(DesignerTransactionCloseEventArgs e)
        {
            if (this.TransactionClosing != null)
            {
                this.TransactionClosing(this, e);
            }
        }

        internal void OnTransactionOpened(EventArgs e)
        {
            if (this.TransactionOpened != null)
            {
                this.TransactionOpened(this, e);
            }
        }

        internal void OnTransactionOpening(EventArgs e)
        {
            if (this.TransactionOpening != null)
            {
                this.TransactionOpening(this, e);
            }
        }

        private void Remove(IComponent component)
        {
            if (component != null)
            {
                ISite site = component.Site;
                if ((this.sites.ContainsValue(site) && ((site != null) && (site.Container == this))) && (site is SampleDesignSite))
                {
                    ComponentEventArgs ce = new ComponentEventArgs(component);
                    this.OnComponentRemoving(ce);
                    SampleDesignSite csite = (SampleDesignSite) site;
                    if ((csite.Component != this.rootComponent) && (component is IExtenderProvider))
                    {
                        ((IExtenderProviderService) this).RemoveExtenderProvider((IExtenderProvider) component);
                    }
                    IDesigner designer = (IDesigner) this.designerTable[component];
                    if (designer != null)
                    {
                        designer.Dispose();
                    }
                    this.designerTable.Remove(component);
                    this.sites.Remove(csite.Name);
                    if (this.components != null)
                    {
                        this.components.Remove(site);
                    }
                    try
                    {
                        this.OnComponentRemoved(ce);
                    }
                    catch (Exception)
                    {
                    }
                    component.Site = null;
                }
            }
        }

        void IComponentChangeService.OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue)
        {
            if (!this.Loading && (this.ComponentChanged != null))
            {
                ComponentChangedEventArgs ce = new ComponentChangedEventArgs(component, member, oldValue, newValue);
                this.ComponentChanged(this, ce);
            }
        }

        void IComponentChangeService.OnComponentChanging(object component, MemberDescriptor member)
        {
            if (this.Loading)
            {
                if (member.Name == "Size")
                {
                    string _DEBUG_ = member.Name;
                }
            }
            else if (this.ComponentChanging != null)
            {
                ComponentChangingEventArgs ce = new ComponentChangingEventArgs(component, member);
                this.ComponentChanging(this, ce);
            }
        }

        void IExtenderProviderService.AddExtenderProvider(IExtenderProvider provider)
        {
            if (this.extenderProviders == null)
            {
                this.extenderProviders = new ArrayList();
            }
            this.extenderProviders.Add(provider);
        }

        void IExtenderProviderService.RemoveExtenderProvider(IExtenderProvider provider)
        {
            if (this.extenderProviders != null)
            {
                this.extenderProviders.Remove(provider);
            }
        }

        void IServiceContainer.AddService(System.Type serviceType, ServiceCreatorCallback callback)
        {
            Debug.Assert(this.serviceContainer != null, "We have no sevice container.  Either the host has not been initialized yet or it has been disposed.");
            if (this.serviceContainer != null)
            {
                this.serviceContainer.AddService(serviceType, callback);
            }
        }

        void IServiceContainer.AddService(System.Type serviceType, object serviceInstance)
        {
            Debug.Assert(this.serviceContainer != null, "We have no sevice container.  Either the host has not been initialized yet or it has been disposed.");
            if (this.serviceContainer != null)
            {
                this.serviceContainer.AddService(serviceType, serviceInstance);
            }
        }

        void IServiceContainer.AddService(System.Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            Debug.Assert(this.serviceContainer != null, "We have no sevice container.  Either the host has not been initialized yet or it has been disposed.");
            if (this.serviceContainer != null)
            {
                this.serviceContainer.AddService(serviceType, callback, promote);
            }
        }

        void IServiceContainer.AddService(System.Type serviceType, object serviceInstance, bool promote)
        {
            Debug.Assert(this.serviceContainer != null, "We have no sevice container.  Either the host has not been initialized yet or it has been disposed.");
            if (this.serviceContainer != null)
            {
                this.serviceContainer.AddService(serviceType, serviceInstance, promote);
            }
        }

        void IServiceContainer.RemoveService(System.Type serviceType)
        {
            if (this.serviceContainer != null)
            {
                this.serviceContainer.RemoveService(serviceType);
            }
        }

        void IServiceContainer.RemoveService(System.Type serviceType, bool promote)
        {
            if (this.serviceContainer != null)
            {
                this.serviceContainer.RemoveService(serviceType, promote);
            }
        }

        void IDesignerLoaderHost.EndLoad(string baseClassName, bool successful, ICollection errorCollection)
        {
            bool wasReload = this.reloading;
            bool wasLoading = this.loadingDesigner;
            this.loadingDesigner = false;
            this.reloading = false;
            if (baseClassName != null)
            {
                this.rootComponentClassName = baseClassName;
            }
            if (successful && (this.rootComponent == null))
            {
                ArrayList errorList = new ArrayList();
                errorList.Add(new Exception("No Base Class"));
                errorCollection = errorList;
                successful = false;
            }
            if (successful)
            {
                if (wasLoading && (this.LoadComplete != null))
                {
                    this.LoadComplete(this, EventArgs.Empty);
                }
            }
            else
            {
                try
                {
                    this.UnloadDocument();
                }
                catch (Exception ex)
                {
                    Debug.Fail("Failed to unload after a...failed load.", ex.ToString());
                }
                if (errorCollection != null)
                {
                    foreach (object errorObj in errorCollection)
                    {
                        if (errorObj is Exception)
                        {
                            this.loadError = (Exception) errorObj;
                        }
                        else
                        {
                            this.loadError = new Exception(errorObj.ToString());
                        }
                        break;
                    }
                }
                else
                {
                    this.loadError = new Exception("Unknown Load Error");
                }
                this.documentWindow.SetDesigner(null);
            }
            this.documentWindow.ReportErrors(errorCollection);
            this.documentWindow.DocumentVisible = true;
        }

        void IDesignerLoaderHost.Reload()
        {
            if ((this.designerLoader != null) && (this.documentWindow != null))
            {
                this.designerLoader.Flush();
                Cursor oldCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                ICollection selectedObjects = null;
                ArrayList selectedNames = null;
                IServiceProvider sp = this;
                ISelectionService selectionService = (ISelectionService) sp.GetService(typeof(ISelectionService));
                if (selectionService != null)
                {
                    selectedObjects = selectionService.GetSelectedComponents();
                    selectedNames = new ArrayList();
                    foreach (object comp in selectedObjects)
                    {
                        if ((comp is IComponent) && (((IComponent) comp).Site != null))
                        {
                            selectedNames.Add(((IComponent) comp).Site.Name);
                        }
                    }
                }
                try
                {
                    this.UnloadDocument();
                    this.Load(true);
                }
                finally
                {
                    if (selectionService != null)
                    {
                        ArrayList selection = new ArrayList();
                        foreach (string name in selectedNames)
                        {
                            if (name != null)
                            {
                                IComponent comp = ((IContainer) this).Components[name];
                                if (comp != null)
                                {
                                    selection.Add(comp);
                                }
                            }
                        }
                        selectionService.SetSelectedComponents(selection);
                    }
                    Cursor.Current = oldCursor;
                }
            }
        }

        /// <summary>
        /// Sets the hidden value - these are held in the type descriptor properties.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="name">The name.</param>
        /// <param name="val">The val.</param>
        private static void SetHiddenValue(Component control, string name, object val)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(control)[name];
            if (descriptor != null)
            {
                try
                {
                    descriptor.SetValue(control, val);

                }
                catch (Exception ex)
                {

                }
            }
        }

        void IContainer.Add(IComponent component)
        {
            ((IContainer) this).Add(component, null);
        }

        void IContainer.Add(IComponent component, string name)
        {
            if (null != component)
            {
                if ((this.rootComponent != null) && (string.Compare(component.GetType().FullName, this.rootComponentClassName, true) == 0))
                {
                    Exception ex = new Exception("Can't add a component to itself " + component.GetType().FullName);
                    throw ex;
                }
                ISite site = component.Site;
                if ((site != null) && (site.Container == this))
                {
                    if ((name != null) && !name.Equals(site.Name))
                    {
                        try
                        {
                            this.CheckName(name);
                        }
                        catch (Exception)
                        {
                            name = this.GetNewComponentName(site.GetType());
                            site.Name = name;
                        }
                        site.Name = name;
                    }
                }
                else
                {
                    SampleDesignSite newSite = this.newComponentSite;
                    this.newComponentSite = null;
                    if ((newSite != null) && (name == null))
                    {
                        name = newSite.Name;
                    }
                    if ((name != null) && !this.CheckName(name))
                    {
                    }
                    ComponentEventArgs ce = new ComponentEventArgs(component);
                    this.OnComponentAdding(ce);
                    if (site != null)
                    {
                        site.Container.Remove(component);
                    }
                    if (newSite == null)
                    {
                        newSite = new SampleDesignSite(this, name);
                    }
                    if (this.components[name] != null)
                    {
                        name = this.GetNewComponentName(component.GetType());
                        newSite.Name = name;
                    }
                    newSite.SetComponent(component);
                    Debug.Assert((name == null) || name.Equals(newSite.Name), "Name should match the one in newComponentSite");
                    if (!(!(component is IExtenderProvider) || TypeDescriptor.GetAttributes(component).Contains(InheritanceAttribute.InheritedReadOnly)))
                    {
                        ((IExtenderProviderService) this).AddExtenderProvider((IExtenderProvider) component);
                    }
                    this.sites[newSite.Name] = newSite;
                    component.Site = newSite;
                    if (this.components != null)
                    {
                        this.components.Add(newSite);
                    }
                    try
                    {
                        this.CreateComponentDesigner(component);
                        this.OnComponentAdded(ce);
                    }
                    catch (Exception)
                    {
                        if (!this.loadingDesigner)
                        {
                            ((IContainer) this).Remove(component);
                        }
                        throw;
                    }
                }
            }
        }

        void IContainer.Remove(IComponent component)
        {
            if (component != null)
            {
                ISite site = component.Site;
                if ((this.sites.ContainsValue(site) && ((site != null) && (site.Container == this))) && (site is SampleDesignSite))
                {
                    ComponentEventArgs ce = new ComponentEventArgs(component);
                    this.OnComponentRemoving(ce);
                    SampleDesignSite csite = (SampleDesignSite) site;
                    if ((csite.Component != this.rootComponent) && (component is IExtenderProvider))
                    {
                        ((IExtenderProviderService) this).RemoveExtenderProvider((IExtenderProvider) component);
                    }
                    IDesigner designer = (IDesigner) this.designerTable[component];
                    if (designer != null)
                    {
                        designer.Dispose();
                    }
                    this.designerTable.Remove(component);
                    this.sites.Remove(csite.Name);
                    if (this.components != null)
                    {
                        this.components.Remove(site);
                    }
                    try
                    {
                        this.OnComponentRemoved(ce);
                    }
                    catch (Exception)
                    {
                    }
                    component.Site = null;
                }
            }
        }

        object IServiceProvider.GetService(System.Type serviceType)
        {
            Debug.Assert(this.serviceContainer != null, "We have no sevice container.  Either the host has not been initialized yet or it has been disposed.");
            object service = null;
            if (this.serviceContainer != null)
            {
                service = this.serviceContainer.GetService(serviceType);
                ServiceRequests requests = (ServiceRequests) this.serviceContainer.GetService(typeof(ServiceRequests));
                if (requests == null)
                {
                    return service;
                }
                if (service != null)
                {
                    requests.ServiceSucceeded(serviceType);
                    return service;
                }
                requests.ServiceFailed(serviceType);
            }
            return service;
        }

        private void UnloadDocument()
        {
            if ((this.helpService != null) && (this.rootDesigner != null))
            {
                this.helpService.RemoveContextAttribute("Keyword", "Designer_" + this.rootDesigner.GetType().FullName);
            }
            IServiceProvider sp = this;
            ISelectionService selectionService = (ISelectionService) sp.GetService(typeof(ISelectionService));
            Debug.Assert(selectionService != null, "ISelectionService not found");
            if (selectionService != null)
            {
                selectionService.SetSelectedComponents(null);
            }
            IDesigner rootDesignerHolder = this.rootDesigner;
            IComponent rootComponentHolder = this.rootComponent;
            this.rootDesigner = null;
            this.rootComponent = null;
            this.rootComponentClassName = null;
            SampleDesignSite[] siteArray = new SampleDesignSite[this.sites.Values.Count];
            this.sites.Values.CopyTo(siteArray, 0);
            IDesigner[] designers = new IDesigner[this.designerTable.Values.Count];
            this.designerTable.Values.CopyTo(designers, 0);
            this.designerTable.Clear();
            this.loadingDesigner = true;
            DesignerTransaction trans = this.CreateTransaction();
            try
            {
                int i;
                for (i = 0; i < designers.Length; i++)
                {
                    if (designers[i] != rootDesignerHolder)
                    {
                        try
                        {
                            designers[i].Dispose();
                        }
                        catch
                        {
                            Debug.Fail("Designer " + designers[i].GetType().Name + " threw an exception during Dispose.");
                        }
                    }
                }
                for (i = 0; i < siteArray.Length; i++)
                {
                    SampleDesignSite site = siteArray[i];
                    IComponent comp = site.Component;
                    if ((comp != null) && (comp != rootComponentHolder))
                    {
                        try
                        {
                            comp.Dispose();
                        }
                        catch
                        {
                            Debug.Fail("Component " + site.Name + " threw during dispose.  Bad component!!");
                        }
                        if (comp.Site != null)
                        {
                            Debug.Fail("Component " + site.Name + " did not remove itself from its container");
                            this.Remove(comp);
                        }
                    }
                }
                if (rootDesignerHolder != null)
                {
                    try
                    {
                        rootDesignerHolder.Dispose();
                    }
                    catch
                    {
                        Debug.Fail("Designer " + rootDesignerHolder.GetType().Name + " threw an exception during Dispose.");
                    }
                }
                if (rootComponentHolder != null)
                {
                    try
                    {
                        rootComponentHolder.Dispose();
                    }
                    catch
                    {
                        Debug.Fail("Component " + rootComponentHolder.GetType().Name + " threw during dispose.  Bad component!!");
                    }
                    if (rootComponentHolder.Site != null)
                    {
                        Debug.Fail("Component " + rootComponentHolder.Site.Name + " did not remove itself from its container");
                        this.Remove(rootComponentHolder);
                    }
                }
                this.sites.Clear();
                if (this.components != null)
                {
                    this.components.Clear();
                }
            }
            finally
            {
                this.loadingDesigner = false;
                trans.Commit();
            }
            if (this.documentWindow != null)
            {
                this.documentWindow.SetDesigner(null);
            }
        }

        public IDesignerHost ActiveDesigner
        {
            get
            {
                return this;
            }
        }

        public IContainer Container
        {
            get
            {
                return this;
            }
        }

        public DesignerCollection Designers
        {
            get
            {
                return new DesignerCollection(new IDesignerHost[] { this });
            }
        }

        public bool InTransaction
        {
            get
            {
                return (this.transactionCount > 0);
            }
        }

        public bool Loading
        {
            get
            {
                return (this.loadingDesigner || ((this.designerLoader != null) && this.designerLoader.Loading));
            }
        }

        public IComponent RootComponent
        {
            get
            {
                return this.rootComponent;
            }
        }

        public string RootComponentClassName
        {
            get
            {
                return this.rootComponentClassName;
            }
        }

        internal Hashtable Sites
        {
            get
            {
                return this.sites;
            }
        }

        ComponentCollection IContainer.Components
        {
            get
            {
                if (this.components == null)
                {
                    this.components = new DesignerComponentCollection(this);
                }
                return this.components;
            }
        }

        internal int TransactionCount
        {
            get
            {
                return this.transactionCount;
            }
            set
            {
                this.transactionCount = value;
            }
        }

        public string TransactionDescription
        {
            get
            {
                if (this.transactionDescriptions != null)
                {
                    return this.transactionDescriptions.GetNonNull();
                }
                return "";
            }
        }

        internal StringStack TransactionDescriptions
        {
            get
            {
                if (this.transactionDescriptions == null)
                {
                    this.transactionDescriptions = new StringStack();
                }
                return this.transactionDescriptions;
            }
        }

        public Control View
        {
            get
            {
                return this.documentWindow;
            }
        }
    }
}

