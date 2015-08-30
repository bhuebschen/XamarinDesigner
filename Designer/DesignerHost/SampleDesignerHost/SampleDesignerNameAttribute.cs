namespace SampleDesignerHost
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    internal sealed class SampleDesignerNameAttribute : Attribute
    {
        public static SampleDesignerNameAttribute Default = new SampleDesignerNameAttribute(false);
        private bool designerName;

        public SampleDesignerNameAttribute() : this(false)
        {
        }

        public SampleDesignerNameAttribute(bool designerName)
        {
            this.designerName = designerName;
        }

        public override bool Equals(object obj)
        {
            SampleDesignerNameAttribute da = obj as SampleDesignerNameAttribute;
            if (da == null)
            {
                return false;
            }
            return (da.designerName == this.designerName);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

