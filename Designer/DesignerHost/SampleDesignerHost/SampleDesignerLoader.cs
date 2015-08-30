namespace SampleDesignerHost
{
    using Microsoft.CSharp;
    using Microsoft.VisualBasic;
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Windows.Forms;
    using System.Xml;

    public class SampleDesignerLoader : DesignerLoader
    {
        private CodeCompileUnit codeCompileUnit;
        public bool dirty;
        private string executable;
        public string fileName;
        private IDesignerLoaderHost host;
        private static readonly Attribute[] propertyAttributes = new Attribute[] { DesignOnlyAttribute.No };
        private Process run;
        public bool unsaved;
        private XmlDocument xmlDocument;

        public SampleDesignerLoader()
        {
        }

        public SampleDesignerLoader(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            this.fileName = fileName;
        }

        public override void BeginLoad(IDesignerLoaderHost host)
        {
            string baseClassName;
            if (host == null)
            {
                throw new ArgumentNullException("SampleDesignerLoader.BeginLoad: Invalid designerLoaderHost.");
            }
            this.host = host;
            ArrayList errors = new ArrayList();
            bool successful = true;
            host.AddService(typeof(IDesignerSerializationManager), new SampleDesignerSerializationManager(this));
            host.AddService(typeof(IEventBindingService), new SampleEventBindingService(host));
            host.AddService(typeof(ITypeResolutionService), new SampleTypeResolutionService());
            host.AddService(typeof(CodeDomProvider), new CSharpCodeProvider());
            host.AddService(typeof(IResourceService), new SampleResourceService(host));
            if (this.fileName == null)
            {
                baseClassName = host.CreateComponent(typeof(Form)).Site.Name;
            }
            else
            {
                baseClassName = this.ReadFile(this.fileName, errors, out this.xmlDocument);
            }
            IComponentChangeService cs = host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (cs != null)
            {
                cs.ComponentChanged += new ComponentChangedEventHandler(this.OnComponentChanged);
                cs.ComponentAdded += new ComponentEventHandler(this.OnComponentAddedRemoved);
                cs.ComponentRemoved += new ComponentEventHandler(this.OnComponentAddedRemoved);
            }
            host.EndLoad(baseClassName, successful, errors);
            this.dirty = true;
            this.unsaved = false;
        }

        public bool Build()
        {
            if (this.dirty)
            {
                this.Flush();
            }
            if (this.executable == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = "exe";
                dlg.Filter = "Executables|*.exe";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.executable = dlg.FileName;
                }
            }
            if (this.executable != null)
            {
                SampleTypeResolutionService strs = this.host.GetService(typeof(ITypeResolutionService)) as SampleTypeResolutionService;
                CompilerParameters cp = new CompilerParameters();
                foreach (Assembly assm in strs.RefencedAssemblies)
                {
                    cp.ReferencedAssemblies.Add(assm.Location);
                    foreach (AssemblyName refAssmName in assm.GetReferencedAssemblies())
                    {
                        Assembly refAssm = Assembly.Load(refAssmName);
                        cp.ReferencedAssemblies.Add(refAssm.Location);
                    }
                }
                cp.GenerateExecutable = true;
                cp.OutputAssembly = this.executable;
                cp.MainClass = this.host.RootComponent.Site.Name + "Namespace." + this.host.RootComponent.Site.Name;
                CompilerResults cr = new CSharpCodeProvider().CreateCompiler().CompileAssemblyFromDom(cp, this.codeCompileUnit);
                if (cr.Errors.HasErrors)
                {
                    string errors = "";
                    foreach (CompilerError error in cr.Errors)
                    {
                        errors = errors + error.ErrorText + "\n";
                    }
                    MessageBox.Show(errors, "Errors during compile.");
                }
                return !cr.Errors.HasErrors;
            }
            return false;
        }

        public override void Dispose()
        {
            IComponentChangeService cs = this.host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (cs != null)
            {
                cs.ComponentChanged -= new ComponentChangedEventHandler(this.OnComponentChanged);
                cs.ComponentAdded -= new ComponentEventHandler(this.OnComponentAddedRemoved);
                cs.ComponentRemoved -= new ComponentEventHandler(this.OnComponentAddedRemoved);
            }
        }

        public override void Flush()
        {
            if (this.dirty)
            {
                XmlDocument document = new XmlDocument();
                document.AppendChild(document.CreateElement("DOCUMENT_ELEMENT"));
                IComponent root = this.host.RootComponent;
                Hashtable nametable = new Hashtable(this.host.Container.Components.Count);
                document.DocumentElement.AppendChild(this.WriteObject(document, nametable, root));
                foreach (IComponent comp in this.host.Container.Components)
                {
                    if (!((comp == root) || nametable.ContainsKey(comp)))
                    {
                        document.DocumentElement.AppendChild(this.WriteObject(document, nametable, comp));
                    }
                }
                CodeCompileUnit code = new CodeCompileUnit();
                CodeNamespace ns = new CodeNamespace(root.Site.Name + "Namespace");
                ns.Imports.Add(new CodeNamespaceImport("System"));
                SampleTypeResolutionService strs = this.host.GetService(typeof(ITypeResolutionService)) as SampleTypeResolutionService;
                foreach (Assembly assm in strs.RefencedAssemblies)
                {
                    ns.Imports.Add(new CodeNamespaceImport(assm.GetName().Name));
                }
                RootDesignerSerializerAttribute a = TypeDescriptor.GetAttributes(root)[typeof(RootDesignerSerializerAttribute)] as RootDesignerSerializerAttribute;
                CodeDomSerializer cds = Activator.CreateInstance(this.host.GetType(a.SerializerTypeName)) as CodeDomSerializer;
                IDesignerSerializationManager manager = this.host.GetService(typeof(IDesignerSerializationManager)) as IDesignerSerializationManager;
                CodeTypeDeclaration td = cds.Serialize(manager, root) as CodeTypeDeclaration;
                CodeConstructor con = new CodeConstructor();
                con.Attributes = MemberAttributes.Public;
                con.Statements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "InitializeComponent"), new CodeExpression[0]));
                td.Members.Add(con);
                CodeEntryPointMethod main = new CodeEntryPointMethod();
                main.Name = "Main";
                main.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                main.CustomAttributes.Add(new CodeAttributeDeclaration("System.STAThreadAttribute"));
                main.Statements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Application)), "Run"), new CodeExpression[] { new CodeObjectCreateExpression(new CodeTypeReference(root.Site.Name), new CodeExpression[0]) }));
                td.Members.Add(main);
                ns.Types.Add(td);
                code.Namespaces.Add(ns);
                this.dirty = false;
                this.xmlDocument = document;
                this.codeCompileUnit = code;
                this.UpdateCodeWindows();
            }
        }

        private bool GetConversionSupported(TypeConverter converter, System.Type conversionType)
        {
            return (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType));
        }

        public object GetService(System.Type serviceType)
        {
            return this.host.GetService(serviceType);
        }

        private void OnComponentAddedRemoved(object sender, ComponentEventArgs ce)
        {
            this.dirty = true;
            this.unsaved = true;
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs ce)
        {
            this.dirty = true;
            this.unsaved = true;
        }

        public bool PromptDispose()
        {
            if (this.dirty || this.unsaved)
            {
                switch (MessageBox.Show("Save changes to existing designer?", "Unsaved Changes", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Cancel:
                        return false;

                    case DialogResult.Yes:
                        this.Save(false);
                        break;
                }
            }
            return true;
        }

        private void ReadEvent(XmlNode childNode, object instance, ArrayList errors)
        {
            IEventBindingService bindings = this.host.GetService(typeof(IEventBindingService)) as IEventBindingService;
            if (bindings == null)
            {
                errors.Add("Unable to contact event binding service so we can't bind any events");
            }
            else
            {
                XmlAttribute nameAttr = childNode.Attributes["name"];
                if (nameAttr == null)
                {
                    errors.Add("No event name");
                }
                else
                {
                    XmlAttribute methodAttr = childNode.Attributes["method"];
                    if (((methodAttr != null) && (methodAttr.Value != null)) && (methodAttr.Value.Length != 0))
                    {
                        EventDescriptor evt = TypeDescriptor.GetEvents(instance)[nameAttr.Value];
                        if (evt == null)
                        {
                            errors.Add(string.Format("Event {0} does not exist on {1}", nameAttr.Value, instance.GetType().FullName));
                        }
                        else
                        {
                            PropertyDescriptor prop = bindings.GetEventProperty(evt);
                            Debug.Assert(prop != null, "Bad event binding service");
                            try
                            {
                                prop.SetValue(instance, methodAttr.Value);
                            }
                            catch (Exception ex)
                            {
                                errors.Add(ex.Message);
                            }
                        }
                    }
                }
            }
        }

        private string ReadFile(string fileName, ArrayList errors, out XmlDocument document)
        {
            string baseClass = null;
            try
            {
                string cleandown = new StreamReader(fileName).ReadToEnd();
                cleandown = "<DOCUMENT_ELEMENT>" + cleandown + "</DOCUMENT_ELEMENT>";

                cleandown = cleandown.Replace("System.Windows.Forms.Label, CustomPanelControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.Windows.Forms.Label, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                cleandown = cleandown.Replace("Uhr.Uhrzeit, Uhr, Version=1.0.5388.24524, Culture=neutral, PublicKeyToken=null", "System.Windows.Forms.Label, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                cleandown = cleandown.Replace("Fixed, CustomPanelControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.Windows.Forms.ContainerControl, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(cleandown);
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (baseClass == null)
                    {
                        baseClass = node.Attributes["name"].Value;
                    }
                    if (node.Name.Equals("Object"))
                    {
                        this.ReadObject(node, errors);
                    }
                    else
                    {
                        errors.Add(string.Format("Node type {0} is not allowed here.", node.Name));
                    }
                }
                document = doc;
            }
            catch (Exception ex)
            {
                document = null;
                errors.Add(ex);
            }
            return baseClass;
        }

        private object ReadInstanceDescriptor(XmlNode node, ArrayList errors)
        {
            XmlAttribute memberAttr = node.Attributes["member"];
            if (memberAttr == null)
            {
                errors.Add("No member attribute on instance descriptor");
                return null;
            }
            byte[] data = Convert.FromBase64String(memberAttr.Value);
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(data);
            MemberInfo mi = (MemberInfo)formatter.Deserialize(stream);
            object[] args = null;
            if (mi is MethodBase)
            {
                ParameterInfo[] paramInfos = ((MethodBase)mi).GetParameters();
                args = new object[paramInfos.Length];
                int idx = 0;
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Name.Equals("Argument"))
                    {
                        object value;
                        if (!this.ReadValue(child, TypeDescriptor.GetConverter(paramInfos[idx].ParameterType), errors, out value))
                        {
                            return null;
                        }
                        args[idx++] = value;
                    }
                }
                if (idx != paramInfos.Length)
                {
                    errors.Add(string.Format("Member {0} requires {1} arguments, not {2}.", mi.Name, args.Length, idx));
                    return null;
                }
            }
            object instance = new InstanceDescriptor(mi, args).Invoke();
            foreach (XmlNode prop in node.ChildNodes)
            {
                if (prop.Name.Equals("Property"))
                {
                    this.ReadProperty(prop, instance, errors);
                }
            }
            return instance;
        }

        private object ReadObject(XmlNode node, ArrayList errors)
        {
            object instance;
            XmlAttribute typeAttr = node.Attributes["type"];
            if (typeAttr == null)
            {
                errors.Add("<Object> tag is missing required type attribute");
                return null;
            }
            System.Type type = System.Type.GetType(typeAttr.Value);
            if (type == null)
            {
                errors.Add(string.Format("Type {0} could not be loaded.", typeAttr.Value));
                return null;
            }
            XmlAttribute nameAttr = node.Attributes["name"];
            if (typeof(IComponent).IsAssignableFrom(type))
            {
                if (nameAttr == null)
                {
                    instance = this.host.CreateComponent(type);
                }
                else
                {
                    instance = this.host.CreateComponent(type, nameAttr.Value);
                }
            }
            else
            {
                instance = Activator.CreateInstance(type);
            }
            XmlAttribute childAttr = node.Attributes["children"];
            IList childList = null;
            if (childAttr != null)
            {
                PropertyDescriptor childProp = TypeDescriptor.GetProperties(instance)[childAttr.Value];
                if (childProp == null)
                {
                    errors.Add(string.Format("The children attribute lists {0} as the child collection but this is not a property on {1}", childAttr.Value, instance.GetType().FullName));
                }
                else
                {
                    childList = childProp.GetValue(instance) as IList;
                    if (childList == null)
                    {
                        errors.Add(string.Format("The property {0} was found but did not return a valid IList", childProp.Name));
                    }
                }
            }
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name.Equals("Object"))
                {
                    if (childAttr == null)
                    {
                        errors.Add("Child object found but there is no children attribute");
                        continue;
                    }
                    if (childList != null)
                    {
                        object childInstance = this.ReadObject(childNode, errors);
                        childList.Add(childInstance);
                    }
                }
                else if (childNode.Name.Equals("Property"))
                {
                    this.ReadProperty(childNode, instance, errors);
                }
                else if (childNode.Name.Equals("Event"))
                {
                    this.ReadEvent(childNode, instance, errors);
                }
            }
            return instance;
        }

        private void ReadProperty(XmlNode node, object instance, ArrayList errors)
        {
            XmlAttribute nameAttr = node.Attributes["name"];
            if (nameAttr == null)
            {
                errors.Add("Property has no name");
            }
            else
            {
                PropertyDescriptor prop = TypeDescriptor.GetProperties(instance)[nameAttr.Value];
                if (prop == null)
                {
                    errors.Add(string.Format("Property {0} does not exist on {1}", nameAttr.Value, instance.GetType().FullName));
                }
                else
                {
                    object value;
                    Exception ex;
                    if (prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content))
                    {
                        value = prop.GetValue(instance);
                        if (value is IList)
                        {
                        Label_01C1:
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.Name.Equals("Item"))
                                {
                                    object item;
                                    XmlAttribute typeAttr = child.Attributes["type"];
                                    if (typeAttr == null)
                                    {
                                        errors.Add("Item has no type attribute");
                                        goto Label_01C1;
                                    }
                                    System.Type type = System.Type.GetType(typeAttr.Value);
                                    if (type == null)
                                    {
                                        errors.Add(string.Format("Item type {0} could not be found.", typeAttr.Value));
                                        goto Label_01C1;
                                    }
                                    if (this.ReadValue(child, TypeDescriptor.GetConverter(type), errors, out item))
                                    {
                                        try
                                        {
                                            ((IList)value).Add(item);
                                        }
                                        catch (Exception exception1)
                                        {
                                            ex = exception1;
                                            errors.Add(ex.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    errors.Add(string.Format("Only Item elements are allowed in collections, not {0} elements.", child.Name));
                                }
                            }
                        }
                        else
                        {
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.Name.Equals("Property"))
                                {
                                    this.ReadProperty(child, value, errors);
                                }
                                else
                                {
                                    errors.Add(string.Format("Only Property elements are allowed in content properties, not {0} elements.", child.Name));
                                }
                            }
                        }
                    }
                    else if (this.ReadValue(node, prop.Converter, errors, out value))
                    {
                        try
                        {
                            prop.SetValue(instance, value);
                        }
                        catch (Exception exception2)
                        {
                            ex = exception2;
                            errors.Add(ex.Message);
                        }
                    }
                }
            }
        }

        private bool ReadValue(XmlNode node, TypeConverter converter, ArrayList errors, out object value)
        {
            try
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.NodeType == XmlNodeType.Text)
                    {
                        value = converter.ConvertFromInvariantString(node.InnerText);
                        return true;
                    }
                    if (child.Name.Equals("Binary"))
                    {
                        byte[] data = Convert.FromBase64String(child.InnerText);
                        if (this.GetConversionSupported(converter, typeof(byte[])))
                        {
                            value = converter.ConvertFrom(null, CultureInfo.InvariantCulture, data);
                            return true;
                        }
                        BinaryFormatter formatter = new BinaryFormatter();
                        MemoryStream stream = new MemoryStream(data);
                        value = formatter.Deserialize(stream);
                        return true;
                    }
                    if (child.Name.Equals("InstanceDescriptor"))
                    {
                        value = this.ReadInstanceDescriptor(child, errors);
                        return (value != null);
                    }
                    errors.Add(string.Format("Unexpected element type {0}", child.Name));
                    value = null;
                    return false;
                }
                value = null;
                return true;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                value = null;
                return false;
            }
        }

        public void Run()
        {
            if (((this.run == null) || this.run.HasExited) && this.Build())
            {
                this.run = new Process();
                this.run.StartInfo.FileName = this.executable;
                this.run.Start();
            }
        }

        public void Save(bool forceFilePrompt)
        {
            try
            {
                if (this.dirty)
                {
                    this.Flush();
                }
                int filterIndex = 3;
                if ((this.fileName == null) || forceFilePrompt)
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.DefaultExt = "xdx";
                    dlg.Filter = "XamarinDesigner Files (*.xdx)|*.xdx";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        this.fileName = dlg.FileName;
                        filterIndex = dlg.FilterIndex;
                    }
                    else
                    {
                        return;
                    }
                }
                if (this.fileName != null)
                {
                    CodeGeneratorOptions o;
                    StreamWriter sw;
//                    switch (filterIndex)
//                    {
//                        case 1:
//                            {
//                                o = new CodeGeneratorOptions();
//                                o.BlankLinesBetweenMembers = true;
//                                o.BracingStyle = "C";
//                                o.ElseOnClosing = false;
//                                o.IndentString = "    ";
//                                sw = new StreamWriter(this.fileName);
//                                CSharpCodeProvider cs = new CSharpCodeProvider();
//                                cs.CreateGenerator().GenerateCodeFromCompileUnit(this.codeCompileUnit, sw, o);
//                                sw.Close();
//                                break;
//                            }
//                        case 2:
//                            {
//                                o = new CodeGeneratorOptions();
//                                o.BlankLinesBetweenMembers = true;
//                                o.BracingStyle = "C";
//                                o.ElseOnClosing = false;
//                                o.IndentString = "    ";
//                                sw = new StreamWriter(this.fileName);
//                                VBCodeProvider vb = new VBCodeProvider();
//                                ICodeGenerator IGen = vb.CreateGenerator();
////                                this.codeCompileUnit.UserData.Add("A", "B");
//                                IGen.GenerateCodeFromCompileUnit(this.codeCompileUnit, sw, o);
//                                sw.Close();
//                                break;
//                            }
//                        case 3:
                            //{
                                StringWriter sxw = new StringWriter();
                                XmlTextWriter xtw = new XmlTextWriter(sxw);
                                xtw.Formatting = Formatting.Indented;
                                this.xmlDocument.WriteTo(xtw);
                                string cleanup = sxw.ToString().Replace("<DOCUMENT_ELEMENT>", "").Replace("</DOCUMENT_ELEMENT>", "");
                                xtw.Close();
                                Boolean saved = false;
                                while (!saved)
                                {
                                    try
                                    {
                                        StreamWriter file = new StreamWriter(this.fileName);
                                        file.Write(cleanup);
                                        file.Close();
                                        saved = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.ToString();
                                    }
                                }
                    //            break;
                    //        }
                    //}
                    this.unsaved = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during save: " + ex.Message);
            }
        }

        internal void Stop()
        {
            if (!((this.run == null) || this.run.HasExited))
            {
                this.run.Kill();
            }
        }

        private void UpdateCodeWindows()
        {
            TabControl tc = this.host.GetService(typeof(TabControl)) as TabControl;
            CodeGeneratorOptions o = new CodeGeneratorOptions();
            o.BlankLinesBetweenMembers = true;
            o.BracingStyle = "C";
            o.ElseOnClosing = false;
            o.IndentString = "    ";
            StringWriter sw = new StringWriter();
            CSharpCodeProvider cs = new CSharpCodeProvider();
            cs.CreateGenerator().GenerateCodeFromCompileUnit(this.codeCompileUnit, sw, o);
            sw.Close();
            sw = new StringWriter();
            VBCodeProvider vb = new VBCodeProvider();
            vb.CreateGenerator().GenerateCodeFromCompileUnit(this.codeCompileUnit, sw, o);
            sw.Close();
            sw = new StringWriter();
            XmlTextWriter xtw = new XmlTextWriter(sw);
            xtw.Formatting = Formatting.Indented;
            this.xmlDocument.WriteTo(xtw);
            string cleanup = sw.ToString().Replace("<DOCUMENT_ELEMENT>", "").Replace("</DOCUMENT_ELEMENT>", "");
            sw.Close();
        }

        private XmlNode WriteBinary(XmlDocument document, byte[] value)
        {
            XmlNode node = document.CreateElement("Binary");
            node.InnerText = Convert.ToBase64String(value);
            return node;
        }

        private void WriteCollection(XmlDocument document, IList list, XmlNode parent)
        {
            foreach (object obj in list)
            {
                XmlNode node = document.CreateElement("Item");
                XmlAttribute typeAttr = document.CreateAttribute("type");
                typeAttr.Value = obj.GetType().AssemblyQualifiedName;
                node.Attributes.Append(typeAttr);
                this.WriteValue(document, obj, node);
                parent.AppendChild(node);
            }
        }

        private XmlNode WriteInstanceDescriptor(XmlDocument document, InstanceDescriptor desc, object value)
        {
            XmlNode node = document.CreateElement("InstanceDescriptor");
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, desc.MemberInfo);
            XmlAttribute memberAttr = document.CreateAttribute("member");
            memberAttr.Value = Convert.ToBase64String(stream.ToArray());
            node.Attributes.Append(memberAttr);
            foreach (object arg in desc.Arguments)
            {
                XmlNode argNode = document.CreateElement("Argument");
                if (this.WriteValue(document, arg, argNode))
                {
                    node.AppendChild(argNode);
                }
            }
            if (!desc.IsComplete)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(value, propertyAttributes);
                this.WriteProperties(document, props, value, node, "Property");
            }
            return node;
        }

        private XmlNode WriteObject(XmlDocument document, IDictionary nametable, object value)
        {
            Debug.Assert(value != null, "Should not invoke WriteObject with a null value");
            XmlNode node = document.CreateElement("Object");
            XmlAttribute typeAttr = document.CreateAttribute("type");
            typeAttr.Value = value.GetType().AssemblyQualifiedName;
            node.Attributes.Append(typeAttr);
            IComponent component = value as IComponent;
            if (((component != null) && (component.Site != null)) && (component.Site.Name != null))
            {
                XmlAttribute nameAttr = document.CreateAttribute("name");
                nameAttr.Value = component.Site.Name;
                node.Attributes.Append(nameAttr);
                Debug.Assert(nametable[component] == null, "WriteObject should not be called more than once for the same object.  Use WriteReference instead");
                nametable[value] = component.Site.Name;
            }
            bool isControl = value is Control;
            if (isControl)
            {
                XmlAttribute childAttr = document.CreateAttribute("children");
                childAttr.Value = "Controls";
                node.Attributes.Append(childAttr);
            }
            if (component != null)
            {
                if (isControl)
                {
                    foreach (Control child in ((Control)value).Controls)
                    {
                        if ((child.Site != null) && (child.Site.Container == this.host.Container))
                        {
                            node.AppendChild(this.WriteObject(document, nametable, child));
                        }
                    }
                }
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, propertyAttributes);
                if (isControl)
                {
                    PropertyDescriptor controlProp = properties["Controls"];
                    if (controlProp != null)
                    {
                        PropertyDescriptor[] propArray = new PropertyDescriptor[properties.Count - 1];
                        int idx = 0;
                        foreach (PropertyDescriptor p in properties)
                        {
                            if (p != controlProp)
                            {
                                propArray[idx++] = p;
                            }
                        }
                        properties = new PropertyDescriptorCollection(propArray);
                    }
                }
                this.WriteProperties(document, properties, value, node, "Property");
                EventDescriptorCollection events = TypeDescriptor.GetEvents(value, propertyAttributes);
                IEventBindingService bindings = this.host.GetService(typeof(IEventBindingService)) as IEventBindingService;
                if (bindings != null)
                {
                    properties = bindings.GetEventProperties(events);
                    this.WriteProperties(document, properties, value, node, "Event");
                }
                return node;
            }
            this.WriteValue(document, value, node);
            return node;
        }

        private void WriteProperties(XmlDocument document, PropertyDescriptorCollection properties, object value, XmlNode parent, string elementName)
        {
            foreach (PropertyDescriptor prop in properties)
            {
                XmlNode node;
                object propValue;
                PropertyDescriptorCollection props;
                if (prop.Name == "AutoScaleBaseSize")
                {
                    string _DEBUG_ = prop.Name;
                }
                if (prop.ShouldSerializeValue(value))
                {
                    node = document.CreateElement(elementName);
                    XmlAttribute attr = document.CreateAttribute("name");
                    attr.Value = prop.Name;
                    node.Attributes.Append(attr);
                    DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)prop.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
                    switch (visibility.Visibility)
                    {
                        case DesignerSerializationVisibility.Visible:
                            if (!(prop.IsReadOnly || !this.WriteValue(document, prop.GetValue(value), node)))
                            {
                                parent.AppendChild(node);
                            }
                            goto Label_0171;

                        case DesignerSerializationVisibility.Content:
                            propValue = prop.GetValue(value);
                            if (!typeof(IList).IsAssignableFrom(prop.PropertyType))
                            {
                                goto Label_012B;
                            }
                            this.WriteCollection(document, (IList)propValue, node);
                            goto Label_014A;
                    }
                }
                goto Label_0171;
            Label_012B:
                props = TypeDescriptor.GetProperties(propValue, propertyAttributes);
                this.WriteProperties(document, props, propValue, node, elementName);
            Label_014A:
                if (node.ChildNodes.Count > 0)
                {
                    parent.AppendChild(node);
                }
            Label_0171: ;
            }
        }

        private XmlNode WriteReference(XmlDocument document, IComponent value)
        {
            Debug.Assert(((value != null) && (value.Site != null)) && (value.Site.Container == this.host.Container), "Invalid component passed to WriteReference");
            XmlNode node = document.CreateElement("Reference");
            XmlAttribute attr = document.CreateAttribute("name");
            attr.Value = value.Site.Name;
            node.Attributes.Append(attr);
            return node;
        }

        private bool WriteValue(XmlDocument document, object value, XmlNode parent)
        {
            if (value != null)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(value);
                if (this.GetConversionSupported(converter, typeof(string)))
                {
                    parent.InnerText = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(string));
                }
                else if (this.GetConversionSupported(converter, typeof(byte[])))
                {
                    byte[] data = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(byte[]));
                    parent.AppendChild(this.WriteBinary(document, data));
                }
                else if (this.GetConversionSupported(converter, typeof(InstanceDescriptor)))
                {
                    InstanceDescriptor id = (InstanceDescriptor)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(InstanceDescriptor));
                    parent.AppendChild(this.WriteInstanceDescriptor(document, id, value));
                }
                else if (((value is IComponent) && (((IComponent)value).Site != null)) && (((IComponent)value).Site.Container == this.host.Container))
                {
                    parent.AppendChild(this.WriteReference(document, (IComponent)value));
                }
                else if (value.GetType().IsSerializable)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream();
                    formatter.Serialize(stream, value);
                    XmlNode binaryNode = this.WriteBinary(document, stream.ToArray());
                    parent.AppendChild(binaryNode);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public IDesignerLoaderHost LoaderHost
        {
            get
            {
                return this.host;
            }
        }
    }
}

