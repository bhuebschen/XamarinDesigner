namespace SampleDesignerHost
{
    using System;
    using System.ComponentModel;

    internal class SampleInheritedNameExtenderProvider : SampleNameExtenderProvider
    {
        internal SampleInheritedNameExtenderProvider(SampleDesignerHost host) : base(host)
        {
        }

        public override bool CanExtend(object o)
        {
            if (o == base.Host.RootComponent)
            {
                return false;
            }
            return !TypeDescriptor.GetAttributes(o)[typeof(InheritanceAttribute)].Equals(InheritanceAttribute.NotInherited);
        }

        [ReadOnly(true)]
        public override string GetName(IComponent comp)
        {
            return base.GetName(comp);
        }
    }
}

