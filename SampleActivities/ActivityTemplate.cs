using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

//  required library from FusionP8
using Sword.BusinessApi.Lifecycles;
using Sword.Fusion.Api.Objects;
using Sword.Fusion.Api.Contents;

namespace SampleActivities
{
    public class ActivityTemplate : IActivity
    {
        private const string ActivityName = "ActivityTemplate";
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
        }
    }
}
