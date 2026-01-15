using Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> messages = new List<string>();
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_3CBC4C80E46B4004B90F002D1F4BAD84");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_6CEB43D2589E42DE87E100477D9D3A41");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_531DE88B772942438AC8007C3ECE218B");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_4271982BAAA9422FBDC90109D1B63818");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_D4772D2BE2F6424FAB3B01496E89FADF");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_208E9C594DA4466A82B40184C324D933");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_CD315204881F43348FE0018615965985");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_01BACE27BABD436BB9D1018D0AD7951D");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_0DF728CF756943A49A4D01ACAF70E5E0");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_9189A16BC5604ACBAEA401B014A2351C");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_1998E5D0E3654AD0A8B101B8A711A85F");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_28B8417252804035A13301BE324C6487");
            messages.Add("RepositoryObject_8F23DCAB46194AC68A63E37A72FB67E6_D_4881E4D216834B138E70022197CE9899");

            RabbitMQMessageSubmission mq = new RabbitMQMessageSubmission(
                System.Configuration.ConfigurationManager.AppSettings["RabbitMQ:Host"],
                Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RabbitMQ:Port"]),
                System.Configuration.ConfigurationManager.AppSettings["RabbitMQ:Username"],
                System.Configuration.ConfigurationManager.AppSettings["RabbitMQ:Password"],
                System.Configuration.ConfigurationManager.AppSettings["RabbitMQ:VirtualHost"]);

            Console.WriteLine("Adding GUIDs to queue.");
            mq.PostMessageBatch("fusion.doc.events", messages);

            RabbitMQMessageReceipt mq2 = new RabbitMQMessageReceipt();
            Console.WriteLine("Listening for GUIDs.");
            mq2.MessageReceived += msg =>
            {
                Console.WriteLine("Received: " + msg);
            };
            mq2.StartConsumer();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}
