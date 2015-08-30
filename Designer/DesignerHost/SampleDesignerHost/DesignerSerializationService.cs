namespace SampleDesignerHost
{
    using System;
    using System.Collections;
    using System.ComponentModel.Design.Serialization;
    using System.Windows.Forms;

    internal class DesignerSerializationService : IDesignerSerializationService
    {
        private IServiceProvider serviceProvider;

        public DesignerSerializationService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ICollection Deserialize(object serializationData)
        {
            SerializationStore serializationStore = serializationData as SerializationStore;
            if (serializationStore != null)
            {
                ComponentSerializationService componentSerializationService = this.serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
                return componentSerializationService.Deserialize(serializationStore);
            }
            return new object[0];
        }

        public object Serialize(ICollection objects)
        {
            ComponentSerializationService componentSerializationService = this.serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
            using (SerializationStore serializationStore = componentSerializationService.CreateStore())
            {
                foreach (object obj in objects)
                {
                    if (obj is Control)
                    {
                        ((Control) obj).Name = null;
                        ((Control) obj).Name = null;
                        ((Control) obj).Name = "";
                        ((Control) obj).Name = "fdhgh";
                        componentSerializationService.Serialize(serializationStore, obj);
                    }
                }
                return serializationStore;
            }
        }
    }
}

