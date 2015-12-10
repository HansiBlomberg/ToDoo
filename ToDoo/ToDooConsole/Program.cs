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
using ToDoBase;

namespace ToDoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TestWcfService();
            
        }

        // Needs a comment
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
                    var name = "Hamid";
                    List<ToDo> aToDo = channel.GetToDo(name);
                    Console.WriteLine($"   Output:");
                    aToDo.ForEach(delegate (ToDo todo)
                    {
                        Console.WriteLine(todo.Description);
                    });

                    Console.WriteLine("");
                    Console.WriteLine("This can also be accomplished by navigating to");
                    Console.WriteLine($"http://localhost:8000/GetToDo/{name}");
                    Console.WriteLine("in a web browser while this sample is running.");

                    Console.WriteLine("");




                    Console.WriteLine("Calling RevealAllMySecrets via HTTP GET: ");
                    s = channel.RevealAllMySecrets("wrong_password");
                    Console.WriteLine($"   Output: {s}");
                    Console.WriteLine();


                    // Console.WriteLine("Calling GetData via HTTP POST: ");
                    // s = channel.GetToDo(name);
                    // Console.WriteLine($"   Output: {s}");
                    // Console.WriteLine("");
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
