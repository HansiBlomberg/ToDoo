using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;
using WcfToDoService;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;

namespace ToDoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TestWcfService();
            
        }


        static void TestWcfService()
        {
            WebServiceHost host = new WebServiceHost(typeof(ToDoService), new Uri("http://localhost:8000/"));
            try
            {
                ServiceEndpoint ep = host.AddServiceEndpoint(typeof(IToDoService), new WebHttpBinding(), "");
                host.Open();
                using (ChannelFactory<IToDoService> cf = new ChannelFactory<IToDoService>(new WebHttpBinding(), "http://localhost:8000"))
                {
                    cf.Endpoint.Behaviors.Add(new WebHttpBehavior());

                    IToDoService channel = cf.CreateChannel();

                    string s;

                    Console.WriteLine("Calling GetData via HTTP GET: ");
                    s = channel.GetData(4711);
                    Console.WriteLine($"   Output: {s}");

                    Console.WriteLine("");
                    Console.WriteLine("This can also be accomplished by navigating to");
                    Console.WriteLine("http://localhost:8000/GetData?s=4711");
                    Console.WriteLine("in a web browser while this sample is running.");

                    Console.WriteLine("");

                    Console.WriteLine("Calling GetData via HTTP POST: ");
                    s = channel.GetData(42);
                    Console.WriteLine($"   Output: {s}");
                    Console.WriteLine("");
                }

                Console.WriteLine("Press <ENTER> to terminate");
                Console.ReadLine();

                host.Close();
            }

            catch (CommunicationException cex)
            {
                Console.WriteLine($"An exception occurred: {cex.Message}");
                host.Abort();
            }

        }

    }


    
}
