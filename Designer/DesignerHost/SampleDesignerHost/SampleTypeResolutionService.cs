namespace SampleDesignerHost
{
    using System;
    using System.Collections;
    using System.ComponentModel.Design;
    using System.Reflection;

    public class SampleTypeResolutionService : ITypeResolutionService
    {
        private Hashtable assemblies;

        public Assembly GetAssembly(AssemblyName name, bool throwOnError)
        {
            Assembly a = null;
            if (this.assemblies != null)
            {
                a = this.assemblies[name] as Assembly;
            }
            if ((a == null) && throwOnError)
            {
                throw new Exception("Assembly " + name.Name + " not found in referenced assemblies.");
            }
            return a;
        }

        public string GetPathOfAssembly(AssemblyName name)
        {
            Assembly a = this.GetAssembly(name, false);
            if (a != null)
            {
                return a.Location;
            }
            return null;
        }

        public Type GetType(string name, bool throwOnError, bool ignoreCase)
        {
            Type t = Type.GetType(name, throwOnError, ignoreCase);
            if ((t == null) && (this.assemblies != null))
            {
                foreach (Assembly a in this.assemblies.Values)
                {
                    t = a.GetType(name, throwOnError, ignoreCase);
                    if (t != null)
                    {
                        break;
                    }
                }
            }
            if ((t == null) && throwOnError)
            {
                throw new Exception("The type " + name + " could not be found. If it is an unqualified name, then its assembly has not been referenced.");
            }
            return t;
        }

        public void ReferenceAssembly(AssemblyName name)
        {
            if (this.assemblies == null)
            {
                this.assemblies = new Hashtable();
            }
            if (!this.assemblies.Contains(name))
            {
                this.assemblies.Add(name, Assembly.Load(name));
            }
        }

        Assembly ITypeResolutionService.GetAssembly(AssemblyName name)
        {
            return this.GetAssembly(name, false);
        }

        Type ITypeResolutionService.GetType(string name)
        {
            return this.GetType(name, false, false);
        }

        Type ITypeResolutionService.GetType(string name, bool throwOnError)
        {
            return this.GetType(name, throwOnError, false);
        }

        public Assembly[] RefencedAssemblies
        {
            get
            {
                if (this.assemblies == null)
                {
                    this.assemblies = new Hashtable();
                }
                Assembly[] ret = new Assembly[this.assemblies.Values.Count];
                this.assemblies.Values.CopyTo(ret, 0);
                return ret;
            }
        }
    }
}

