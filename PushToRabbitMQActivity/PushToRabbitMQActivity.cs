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

namespace idox.eim.fusionp8.customactivities
{
    public class PushToRabbitMQActivity : IActivity
    {
        private const string ActivityName = "PushToRabbitMQActivity";
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
            var currentDocumentId = currentDocument.Id;
            var repositoryId = currentDocument.RepositoryRelatedId;

            string rabbitHost = (string) context.Parameters["RabbitMQ:Host"];
            int rabbitPort = (int) context.Parameters["RabbitMQ:Port"];
            string rabbitUsername = (string)context.Parameters["RabbitMQ:Username"];
            string rabbitPassword = (string) context.Parameters["RabbitMQ:Password"];
            string rabbitVirtualHost= (string) context.Parameters["RabbitMQ:VirtualHost"];
            string queueName = (string) context.Parameters["QueueName"];

            string operation = "";
            
            List<string> rawPayload = new List<string>();
            rawPayload.Add(operation);
            rawPayload.Add(Format.WebApi.Format(repositoryId, currentDocumentId));

            new Messaging.RabbitMQMessageSubmission(rabbitHost, rabbitPort, rabbitUsername, rabbitPassword, rabbitVirtualHost).
                PostMessage(queueName, Format.Json.Format(rawPayload.ToArray()));
        }
    }
}
