using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sword.BusinessApi.Lifecycles;
using Sword.Fusion.Api.Objects;
using Sword.Fusion.Api.Contents;
using System.Xml.Linq;

namespace CreateTransmittableContainer
{
    public class CreateTransmittableContainerActivity : IActivity
    {
        private const string ActivityName = "CreateTransmittableContainerActivity";
        public bool CanDo(ActivityExecutionContext context)
        {
            IPersistableObject activityObject = context.LifecycleObject;
            if (activityObject == null)
            {
                return false;
            }

            return true;
        }
        public void Do(ActivityExecutionContext context)
        {
            if (context == null) { Console.WriteLine("NULL context", ActivityName); return; }
            if (context.LifecycleObject == null) { Console.WriteLine("NULL lifecycle object", ActivityName); return; }

            //  get our selected doc
            var currentDocument = context.LifecycleObject as IDocument;

            Sword.FusionP8.ApiImpl.Containers.ContainerLogicalDocumentsImpl container =
                new Sword.FusionP8.ApiImpl.Containers.ContainerLogicalDocumentsImpl(currentDocument);

            //  Add self as content
            container.Documents.Add(currentDocument);

            //  How to add label?
        }
    }
}
