namespace SampleDesignerHost
{
    using System;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.IO;
    using System.Resources;

    public class SampleResourceService : IResourceService
    {
        private IDesignerHost host;
        private MemoryStream ms;
        private ResourceReader reader;
        private ResourceWriter writer;

        public SampleResourceService(IDesignerHost host)
        {
            this.host = host;
        }

        public IResourceReader GetResourceReader(CultureInfo info)
        {
            if (this.reader == null)
            {
                if (this.ms == null)
                {
                    this.ms = new MemoryStream();
                }
                this.reader = new ResourceReader(this.ms);
            }
            return this.reader;
        }

        public IResourceWriter GetResourceWriter(CultureInfo info)
        {
            if (this.writer == null)
            {
                if (this.ms == null)
                {
                    this.ms = new MemoryStream();
                }
                this.writer = new ResourceWriter(this.ms);
            }
            return this.writer;
        }
    }
}

