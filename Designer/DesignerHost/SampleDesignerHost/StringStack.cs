namespace SampleDesignerHost
{
    using System;
    using System.Collections;

    internal class StringStack : Stack
    {
        internal StringStack()
        {
        }

        internal string GetNonNull()
        {
            int items = this.Count;
            object[] itemArr = this.ToArray();
            for (int i = items - 1; i >= 0; i--)
            {
                object item = itemArr[i];
                if (((item != null) && (item is string)) && (((string) item).Length > 0))
                {
                    return (string) item;
                }
            }
            return "";
        }
    }
}

